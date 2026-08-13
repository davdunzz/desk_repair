using System.Windows;
using QuestPDF.Infrastructure;

namespace RepairDesk;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        Database.Initialize();
        base.OnStartup(e);
    }
}
