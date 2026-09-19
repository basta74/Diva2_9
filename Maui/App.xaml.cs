using Microsoft.Extensions.DependencyInjection;

namespace Diva2Maui;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		var window = new Window(new AppShell());
        window.Activated += (_, _) => (window.Page as AuthenticatedShell)?.StartSynchronization();
        window.Resumed += (_, _) => (window.Page as AuthenticatedShell)?.StartSynchronization();
        window.Deactivated += (_, _) => (window.Page as AuthenticatedShell)?.StopSynchronization();
        window.Stopped += (_, _) => (window.Page as AuthenticatedShell)?.StopSynchronization();
        window.Destroying += (_, _) => (window.Page as AuthenticatedShell)?.StopSynchronization();
        return window;
	}
}
