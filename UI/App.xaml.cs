using System.Windows;
using Squirrel;

namespace UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            SquirrelAwareApp.HandleEvents(OnInstall, null, null, OnUninstall, OnRun);
        }

        private void OnInstall(SemanticVersion version, IAppTools tools)
        {
            tools.CreateShortcutForThisExe();
        }

        private void OnUninstall(SemanticVersion version, IAppTools tools)
        {
            tools.RemoveShortcutForThisExe();
        }

        private void OnRun(SemanticVersion version, IAppTools tools, bool firstRun)
        {
            tools.SetProcessAppUserModelId();
        }
    }
}
