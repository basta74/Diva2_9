using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using Diva2.Data.Infrastructure;
using Diva2Maui.Services;

static void Check(bool condition, string name)
{
    if (!condition) { throw new Exception(name); }
    Console.WriteLine($"PASS {name}");
}

var domainA = Guid.NewGuid().ToString();
var domainB = Guid.NewGuid().ToString();
var a = DomainChangeStamp.Get(domainA);
var b = DomainChangeStamp.Get(domainB);
Check(a == DomainChangeStamp.Get(domainA), "Reading the stamp does not change it");
var stamps = new System.Collections.Concurrent.ConcurrentBag<string>();
Parallel.For(0, 1000, _ => stamps.Add(DomainChangeStamp.Touch(domainA)));
Check(stamps.Distinct().Count() == 1000 && !stamps.Contains(a), "Concurrent writes receive distinct timestamps");
Check(b == DomainChangeStamp.Get(domainB), "Changes are isolated by domain");

using var handler = new FakeApi();
using var http = new HttpClient(handler) { BaseAddress = new Uri("https://test.invalid/") };
http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "test");
var api = new Diva2ApiClient();
typeof(Diva2ApiClient).GetField("tenantClient", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(api, http);
Check(await api.SynchronizeDomainAsync(), "Initial sync downloads a snapshot");
Check(api.Data.Timestamp == "v1" && api.Data.Lessons[1].Count == 1
    && api.Data.Reservations.Count == 1 && api.Data.Participants[(1, 10)].Count == 1, "Snapshot contains lessons, reservations and participants");
Check(handler.Paths.IndexOf("/api/v1/branches/1/lessons") < handler.Paths.IndexOf("/api/v1/reservations/me")
    && handler.Paths.IndexOf("/api/v1/reservations/me") < handler.Paths.IndexOf("/api/v1/branches/1/lessons/10/participants"), "Download order is preserved");
handler.Paths.Clear();
Check(!await api.SynchronizeDomainAsync() && handler.Paths.SequenceEqual(new[] { "/api/v1/changes" }), "Unchanged stamp makes only one small request");
var snapshot = api.Data;
handler.Stamp = "v2";
handler.FailCredits = true;
try { await api.SynchronizeDomainAsync(); throw new Exception("Expected HTTP failure"); }
catch (HttpRequestException) { }
Check(ReferenceEquals(snapshot, api.Data) && api.Data.Timestamp == "v1", "Failed download retains the previous complete snapshot");
handler.FailCredits = false;
Check(await api.SynchronizeDomainAsync() && api.Data.Timestamp == "v2", "Failed stamp is retried successfully");
handler.Stamp = "v3";
handler.ShowParticipants = false;
handler.Paths.Clear();
Check(await api.SynchronizeDomainAsync() && api.Data.Participants.Count == 0
    && !handler.Paths.Any(path => path.EndsWith("/participants")), "Disabled participant visibility is respected");
handler.Stamp = "v4";
handler.BlockChanges = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
var ongoing = api.SynchronizeDomainAsync();
Check(!await api.SynchronizeDomainAsync(), "Overlapping sync is skipped");
api.Logout();
handler.BlockChanges.SetResult();
Check(!await ongoing && api.Data.Timestamp is null, "Logout discards an in-flight snapshot");
Console.WriteLine("All synchronization checks passed.");

sealed class FakeApi : HttpMessageHandler
{
    public string Stamp = "v1";
    public bool FailCredits;
    public bool ShowParticipants = true;
    public List<string> Paths = [];
    public TaskCompletionSource? BlockChanges;
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
    {
        var path = request.RequestUri!.AbsolutePath;
        Paths.Add(path);
        if (path == "/api/v1/changes" && BlockChanges is not null)
        {
            await BlockChanges.Task.WaitAsync(token);
        }
        if (FailCredits && path == "/api/v1/credits/me")
        {
            return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
        }
        var json = path switch
        {
            "/api/v1/changes" => $"{{\"timestamp\":\"{Stamp}\"}}",
            "/api/v1/branches" => "[{\"id\":1,\"name\":\"Studio\"}]",
            "/api/v1/branches/1/lessons" => "[{\"id\":10,\"name\":\"Lesson\"}]",
            "/api/v1/branches/1/configuration" => "{\"branchId\":1,\"settings\":{\"lekceUkazZakazniky\":\"" + (ShowParticipants ? "1" : "0") + "\"}}",
            "/api/v1/reservations/me" => "[{\"branchId\":1,\"lessonId\":10}]",
            "/api/v1/branches/1/lessons/10/participants" => "[{\"displayName\":\"Customer\"}]",
            "/api/v1/credits/me" => "[]",
            _ => throw new Exception($"Unexpected URL {path}")
        };
        return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json") };
    }
}
