using Pulumi;

namespace BugReport.BackupPolicy;

public class BackupStack : Stack
{
    public BackupStack()
    {
        var backupServices = new BackupServices();
    }
}