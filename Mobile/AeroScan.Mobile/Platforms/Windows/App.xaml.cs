using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

namespace AeroScan.Mobile.WinUI;

public partial class App : MauiWinUIApplication
{
    public App()
    {
        this.InitializeComponent();
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);
        SetFullScreen();
    }

    private void SetFullScreen()
    {
        if (Microsoft.Maui.ApplicationModel.WindowStateManager.Default.GetActiveWindow() is
            Microsoft.UI.Xaml.Window nativeWindow)
        {
            var handle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(handle);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
        }
    }
}
