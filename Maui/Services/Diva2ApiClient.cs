using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Security;
using System.Text.Json;
using Diva2Maui.Models;

namespace Diva2Maui.Services;

public sealed class Diva2ApiClient
{
    private static readonly TimeSpan LessonCacheLifetime = TimeSpan.FromMinutes(10);
    public const string DefaultCatalogUrl = "https://api.diva2.cz/";
    private readonly HttpClient catalogClient;
    private HttpClient? tenantClient;
    private TenantInfo? selectedTenant;
    public TenantInfo? SelectedTenant => selectedTenant;
    public string DisplayName { get; private set; } = string.Empty;
    public int ReservationVersion { get; private set; }

    public Diva2ApiClient()
    {
        catalogClient = CreateClient(new Uri(DefaultCatalogUrl));
    }

    public async Task<IReadOnlyList<TenantInfo>> GetTenantsAsync(CancellationToken cancellationToken = default)
        => await catalogClient.GetFromJsonAsync<List<TenantInfo>>("api/v1/tenants", cancellationToken) ?? [];

    public void SelectTenant(TenantInfo tenant)
    {
        selectedTenant = tenant;
        tenantClient?.Dispose();
        var baseAddress = catalogClient.BaseAddress?.IsLoopback == true
            ? catalogClient.BaseAddress
            : new Uri(EnsureTrailingSlash(tenant.BaseUrl));
        tenantClient = CreateClient(baseAddress!);
    }

    public async Task<LoginResponse> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var client = RequireTenantClient();
        using var response = await client.PostAsJsonAsync("api/v1/auth/login", new LoginRequest { Email = email, Password = password }, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        var login = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken)
            ?? throw new InvalidOperationException("Server nevrátil přihlašovací údaje.");
        if (string.IsNullOrWhiteSpace(login.AccessToken)) throw new InvalidOperationException("Server nevrátil přístupový token.");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login.AccessToken);
        await SecureStorage.Default.SetAsync(TokenKey(selectedTenant!.Id), login.AccessToken);
        await SecureStorage.Default.SetAsync(TokenExpiryKey(selectedTenant.Id), login.ExpiresAt.ToString("O"));
        Preferences.Default.Set(DisplayNameKey(selectedTenant.Id), login.DisplayName);
        DisplayName = login.DisplayName;
        return login;
    }

    public async Task<string?> TryRestoreSessionAsync()
    {
        if (selectedTenant is null || tenantClient is null) return null;
        var token = await SecureStorage.Default.GetAsync(TokenKey(selectedTenant.Id));
        var expiryText = await SecureStorage.Default.GetAsync(TokenExpiryKey(selectedTenant.Id));
        if (string.IsNullOrWhiteSpace(token) || !DateTimeOffset.TryParse(expiryText, out var expiry) || expiry <= DateTimeOffset.UtcNow)
        {
            RemoveStoredSession(selectedTenant.Id);
            return null;
        }

        tenantClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        DisplayName = Preferences.Default.Get(DisplayNameKey(selectedTenant.Id), selectedTenant.Name);
        return DisplayName;
    }

    public async Task<IReadOnlyList<BranchInfo>> GetBranchesAsync(CancellationToken cancellationToken = default)
        => await RequireTenantClient().GetFromJsonAsync<List<BranchInfo>>("api/v1/branches", cancellationToken) ?? [];

    public async Task<BranchConfiguration> GetBranchConfigurationAsync(int branchId, CancellationToken cancellationToken = default)
        => await RequireTenantClient().GetFromJsonAsync<BranchConfiguration>($"api/v1/branches/{branchId}/configuration", cancellationToken)
           ?? new BranchConfiguration { BranchId = branchId };

    public async Task<IReadOnlyList<MyReservationInfo>> GetMyReservationsAsync(CancellationToken cancellationToken = default)
        => await RequireTenantClient().GetFromJsonAsync<List<MyReservationInfo>>("api/v1/reservations/me", cancellationToken) ?? [];

    public async Task<IReadOnlyList<BranchCreditsInfo>> GetMyCreditsAsync(CancellationToken cancellationToken = default)
        => await RequireTenantClient().GetFromJsonAsync<List<BranchCreditsInfo>>("api/v1/credits/me", cancellationToken) ?? [];

    public async Task<IReadOnlyList<NotificationInfo>> GetNotificationsAsync(CancellationToken cancellationToken = default)
        => await RequireTenantClient().GetFromJsonAsync<List<NotificationInfo>>("api/v1/notifications", cancellationToken) ?? [];

    public async Task<IReadOnlyList<LessonParticipantInfo>> GetLessonParticipantsAsync(int branchId, int lessonId, CancellationToken cancellationToken = default)
        => await RequireTenantClient().GetFromJsonAsync<List<LessonParticipantInfo>>($"api/v1/branches/{branchId}/lessons/{lessonId}/participants", cancellationToken) ?? [];

    public async Task<CreateReservationResponse> CreateReservationAsync(int branchId, int lessonId, CancellationToken cancellationToken = default)
    {
        using var response = await RequireTenantClient().PostAsync($"api/v1/reservations/lessons/{lessonId}", null, cancellationToken);
        var result = await response.Content.ReadFromJsonAsync<CreateReservationResponse>(cancellationToken)
            ?? new CreateReservationResponse { Message = $"Server vrátil chybu {(int)response.StatusCode}." };
        if (!response.IsSuccessStatusCode || !result.Success)
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(result.Message) ? "Objednávku se nepodařilo vytvořit." : result.Message);
        try { File.Delete(LessonCacheFile(branchId)); } catch (IOException) { }
        ReservationVersion++;
        return result;
    }

    public async Task<CreateReservationResponse> CancelReservationAsync(int branchId, int lessonId, CancellationToken cancellationToken = default)
    {
        using var response = await RequireTenantClient().DeleteAsync($"api/v1/reservations/lessons/{lessonId}", cancellationToken);
        var result = await response.Content.ReadFromJsonAsync<CreateReservationResponse>(cancellationToken)
            ?? new CreateReservationResponse { Message = $"Server vrátil chybu {(int)response.StatusCode}." };
        if (!response.IsSuccessStatusCode || !result.Success)
        {
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(result.Message) ? "Odhlášení se nepodařilo." : result.Message);
        }

        try
        {
            File.Delete(LessonCacheFile(branchId));
        }
        catch (IOException)
        {
        }

        ReservationVersion++;
        return result;
    }

    public async Task<IReadOnlyList<LessonInfo>> GetLessonsAsync(int branchId, CancellationToken cancellationToken = default)
    {
        var cacheFile = LessonCacheFile(branchId);
        if (File.Exists(cacheFile) && DateTimeOffset.UtcNow - File.GetLastWriteTimeUtc(cacheFile) < LessonCacheLifetime)
        {
            try
            {
                var cached = JsonSerializer.Deserialize<List<LessonInfo>>(await File.ReadAllTextAsync(cacheFile, cancellationToken));
                if (cached is not null) return ClearReservationStatuses(cached);
            }
            catch (JsonException) { }
            catch (IOException) { }
        }

        var lessons = await RequireTenantClient().GetFromJsonAsync<List<LessonInfo>>($"api/v1/branches/{branchId}/lessons", cancellationToken) ?? [];
        try { await File.WriteAllTextAsync(cacheFile, JsonSerializer.Serialize(lessons), cancellationToken); }
        catch (IOException) { }
        return ClearReservationStatuses(lessons);
    }

    private static IReadOnlyList<LessonInfo> ClearReservationStatuses(List<LessonInfo> lessons)
    {
        foreach (var lesson in lessons)
        {
            lesson.ReservationStatus = null;
            lesson.IsReservedByCurrentUser = false;
        }
        return lessons;
    }

    public void Logout()
    {
        if (selectedTenant != null)
        {
            RemoveStoredSession(selectedTenant.Id);
            ClearLessonCache(selectedTenant.Id);
        }
        if (tenantClient != null) tenantClient.DefaultRequestHeaders.Authorization = null;
        DisplayName = string.Empty;
    }

    private HttpClient RequireTenantClient() => tenantClient ?? throw new InvalidOperationException("Nejprve vyberte provozovatele.");
    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode) return;
        var message = await response.Content.ReadAsStringAsync(cancellationToken);
        throw new HttpRequestException(string.IsNullOrWhiteSpace(message) ? $"Server vrátil chybu {(int)response.StatusCode}." : message);
    }
    private static string EnsureTrailingSlash(string value) => value.EndsWith('/') ? value : value + "/";
    private static string TokenKey(string tenantId) => $"diva2_token_{tenantId}";
    private static string TokenExpiryKey(string tenantId) => $"diva2_token_expiry_{tenantId}";
    private static string DisplayNameKey(string tenantId) => $"diva2_display_name_{tenantId}";
    private string LessonCacheFile(int branchId) => Path.Combine(FileSystem.CacheDirectory, $"diva2_lessons_v3_{selectedTenant!.Id}_{branchId}.json");

    private static void RemoveStoredSession(string tenantId)
    {
        SecureStorage.Default.Remove(TokenKey(tenantId));
        SecureStorage.Default.Remove(TokenExpiryKey(tenantId));
        Preferences.Default.Remove(DisplayNameKey(tenantId));
    }

    private static void ClearLessonCache(string tenantId)
    {
        foreach (var file in Directory.EnumerateFiles(FileSystem.CacheDirectory, $"diva2_lessons*_{tenantId}_*.json"))
            try { File.Delete(file); } catch (IOException) { }
    }

    private static HttpClient CreateClient(Uri baseAddress)
    {
#if DEBUG
        var transport = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (request, _, _, errors) =>
                request.RequestUri?.IsLoopback == true || errors == SslPolicyErrors.None
        };
        return new HttpClient(new ApiDebugHandler(transport)) { BaseAddress = baseAddress, Timeout = TimeSpan.FromSeconds(20) };
#else
        return new HttpClient { BaseAddress = baseAddress, Timeout = TimeSpan.FromSeconds(20) };
#endif
    }

#if DEBUG
    private sealed class ApiDebugHandler(HttpMessageHandler innerHandler) : DelegatingHandler(innerHandler)
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            if (!request.RequestUri!.AbsolutePath.Equals("/api/v1/auth/login", StringComparison.OrdinalIgnoreCase))
            {
                var json = response.Content is null ? "" : await response.Content.ReadAsStringAsync(cancellationToken);
                System.Diagnostics.Debug.WriteLine($"DIVA2 API {request.Method} {request.RequestUri} -> {(int)response.StatusCode}");
                System.Diagnostics.Debug.WriteLine(json);
            }
            return response;
        }
    }
#endif
}
