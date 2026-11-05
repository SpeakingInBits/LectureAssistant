using System.Diagnostics;
using Velopack;
using Velopack.Sources;

namespace LectureAssistant.App.Services;

/// <summary>
/// Updates the app from the project's GitHub releases with Velopack. The Release workflow publishes each architecture's
/// package and release list (<c>releases.&lt;channel&gt;.json</c>) there, and the app only looks at its own channel.
/// The Store package isn't installed by Velopack, so the Store keeps updating that one.
/// </summary>
public sealed class AppUpdater
{
    // "latest/download" always serves the newest release's files. Asking GitHub's API instead would be limited to
    // 60 requests an hour per IP address, which a campus sharing one address could use up.
    private const string ReleaseFiles = "https://github.com/SpeakingInBits/LectureAssistant/releases/latest/download/";

    // LA_UPDATE_SOURCE, a folder or URL holding packaged releases, tries out an update before it's published.
    private readonly UpdateManager _manager = Environment.GetEnvironmentVariable("LA_UPDATE_SOURCE") is { Length: > 0 } source
        ? new(source)
        : new(new SimpleWebSource(ReleaseFiles));
    private UpdateInfo? _update;

    /// <summary>
    /// Downloads the newest release if it's newer than this one and returns its version, or null when the app is up to date,
    /// wasn't installed from a release (the Store package or a debug build), or the check failed (offline).
    /// </summary>
    public async Task<string?> DownloadUpdateAsync()
    {
        if (!_manager.IsInstalled)
            return null;

        try
        {
            _update = await _manager.CheckForUpdatesAsync();
            if (_update is null)
                return null;

            await _manager.DownloadUpdatesAsync(_update);
            return _update.TargetFullRelease.Version.ToString();
        }
        catch (Exception ex)
        {
            // Not worth bothering the instructor with; the app checks again next time it opens.
            Trace.WriteLine($"Update check failed: {ex}");
            _update = null;
            return null;
        }
    }

    /// <summary>Closes the app, installs the downloaded update and opens the new version.</summary>
    public void RestartToUpdate()
    {
        if (_update is not null)
            _manager.ApplyUpdatesAndRestart(_update.TargetFullRelease);
    }
}
