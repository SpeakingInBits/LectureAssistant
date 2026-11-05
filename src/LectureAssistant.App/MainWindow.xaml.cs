using LectureAssistant.App.Services;
using LectureAssistant.App.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace LectureAssistant.App;

public sealed partial class MainWindow : Window
{
    private readonly AppUpdater _updater = App.Services.GetRequiredService<AppUpdater>();

    public MainWindow()
    {
        InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets", "AppIcon.ico"));
        AppWindow.Resize(new Windows.Graphics.SizeInt32(1400, 900));

        ContentFrame.Navigate(typeof(ProjectsPage));
        Nav.SelectedItem = LecturesItem;
        Closed += async (_, _) =>
        {
            PreviewWindow.CloseAll();
            if (ContentFrame.Content is LecturePage page) await page.ViewModel.SaveIfDirtyAsync();
        };

        _ = OfferUpdateAsync();
    }

    public void OpenLecture(string projectId) => ContentFrame.Navigate(typeof(LecturePage), projectId);

    /// <summary>Downloads a newer version in the background, if there is one, and offers to restart into it.</summary>
    private async Task OfferUpdateAsync()
    {
        if (await _updater.DownloadUpdateAsync() is not { } version) return;

        UpdateBar.Message = $"Version {version} is ready. It installs the next time you open the app, or restart now.";
        UpdateBar.IsOpen = true;
    }

    private async void RestartNow_Click(object sender, RoutedEventArgs e)
    {
        if (ContentFrame.Content is LecturePage page) await page.ViewModel.SaveIfDirtyAsync();
        _updater.RestartToUpdate();
    }

    private void Nav_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        var target = args.IsSettingsInvoked ? typeof(SettingsPage) : typeof(ProjectsPage);
        if (ContentFrame.CurrentSourcePageType != target) ContentFrame.Navigate(target);
    }

    private void Nav_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
    {
        if (ContentFrame.CanGoBack) ContentFrame.GoBack();
    }

    private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
    {
        Nav.SelectedItem = e.SourcePageType == typeof(SettingsPage) ? Nav.SettingsItem : LecturesItem;
    }
}
