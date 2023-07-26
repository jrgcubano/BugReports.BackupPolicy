using System.Threading.Tasks;
using Pulumi;

namespace BugReport.BackupPolicy;

class Program
{
    static Task<int> Main() => Deployment.RunAsync<BackupStack>();
}