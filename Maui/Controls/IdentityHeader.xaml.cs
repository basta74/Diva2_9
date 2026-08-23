namespace Diva2Maui.Controls;

public partial class IdentityHeader : ContentView
{
    public static readonly BindableProperty DisplayNameProperty = BindableProperty.Create(
        nameof(DisplayName), typeof(string), typeof(IdentityHeader), string.Empty);

    public static readonly BindableProperty TenantNameProperty = BindableProperty.Create(
        nameof(TenantName), typeof(string), typeof(IdentityHeader), string.Empty);

    public IdentityHeader() => InitializeComponent();

    public string DisplayName
    {
        get => (string)GetValue(DisplayNameProperty);
        set => SetValue(DisplayNameProperty, value);
    }

    public string TenantName
    {
        get => (string)GetValue(TenantNameProperty);
        set => SetValue(TenantNameProperty, value);
    }
}
