using Pulumi;
using Pulumi.Azure.Core;
using Pulumi.Azure.RecoveryServices.Inputs;

namespace BugReport.BackupPolicy;

public class BackupServices
{
    const string RoleName = "bugrbak";

    public BackupServices()
    {
        var location = "eun";
        var environment = "qa";
            
        // Create the Resource Group
        var resourceGroupName = $"rg-{environment}-{RoleName}-{location}";
        var resourceGroup = new ResourceGroup(
            resourceGroupName,
            new ResourceGroupArgs
            {
                Name = resourceGroupName
            }
        );
            
        // Create a recovery services vault for backing up virtual machines
        var recoveryServicesVaultName = $"rsv-{environment}-{RoleName}-{location}";
        var recoveryServicesVault = new global::Pulumi.Azure.RecoveryServices.Vault(recoveryServicesVaultName, new global::Pulumi.Azure.RecoveryServices.VaultArgs
        {
            Name = recoveryServicesVaultName,
            ResourceGroupName = resourceGroup.Name,
            Sku = "Standard",
            SoftDeleteEnabled = true,
            Identity = new VaultIdentityArgs
            {
                Type = "SystemAssigned"
            }
        }, new CustomResourceOptions
        {
            Parent = resourceGroup,
            Protect = true
        });
            
        // Create a recovery services retention policy for retaining backups
        var recoveryServicesVmBackupPolicyName = $"rsvpol-{environment}-{RoleName}-{location}-vmbackuppolicy";
        var recoveryServicesVmBackupPolicy = new global::Pulumi.Azure.Backup.PolicyVM(recoveryServicesVmBackupPolicyName, new global::Pulumi.Azure.Backup.PolicyVMArgs
        {
            Name = recoveryServicesVmBackupPolicyName,
            ResourceGroupName = resourceGroup.Name,
            RecoveryVaultName = recoveryServicesVault.Name,
            Timezone = "UTC",
            Backup = new global::Pulumi.Azure.Backup.Inputs.PolicyVMBackupArgs
            {
                Frequency = "Daily",
                Time = "01:00",
            },
            RetentionDaily = new global::Pulumi.Azure.Backup.Inputs.PolicyVMRetentionDailyArgs
            {
                Count = 30
            },
            RetentionWeekly = new global::Pulumi.Azure.Backup.Inputs.PolicyVMRetentionWeeklyArgs
            {
                Count = 12,
                Weekdays = 
                {
                    "Sunday"
                },
            },
            RetentionMonthly = new global::Pulumi.Azure.Backup.Inputs.PolicyVMRetentionMonthlyArgs
            {
                Count = 12,
                Weeks = 
                {
                    "Last"
                },
                Weekdays =
                {
                    "Sunday"
                }
            },
            RetentionYearly = new global::Pulumi.Azure.Backup.Inputs.PolicyVMRetentionYearlyArgs
            {
                Count = 7,
                Months = 
                {
                    "January",
                },
                Weeks =
                {
                    "Last"
                },
                Weekdays =
                {
                    "Sunday"
                }
            }
        });
            
         
        ResourceGroupName = resourceGroup.Name;
        RecoveryServicesVaultName = recoveryServicesVault.Name;
        VmBackupPolicyId = recoveryServicesVmBackupPolicy.Id;
    }

    public Output<string> ResourceGroupName { get; set; }
    public Output<string> RecoveryServicesVaultName { get; set; }
    public Output<string> VmBackupPolicyId { get; set; }
}