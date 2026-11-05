using Velopack;

namespace LectureAssistant.App;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        // Must run first. The installer and updater start the app with special arguments to finish their work
        // (shortcuts, uninstalling), and Run() handles those and exits. It also installs an update downloaded last time.
        // It does nothing in the Store package or a debug build, which Velopack didn't install.
        VelopackApp.Build().Run();

        XamlGeneratedProgram.XamlGeneratedMain();
    }
}
