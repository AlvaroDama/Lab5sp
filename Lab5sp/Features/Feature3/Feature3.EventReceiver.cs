using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace Lab5sp.Features.Feature3
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("d9822981-011e-4577-874d-99df66a08940")]
    public class Feature3EventReceiver : SPFeatureReceiver
    {

        const string TimerJobName = "TotalFacturasRegistradas";

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPWebApplication webApplication = ((SPSite)properties.Feature.Parent).WebApplication;
            deleteJob(webApplication);

            ManejoFacturasTimerJob timerJob = new ManejoFacturasTimerJob(TimerJobName, webApplication, null, SPJobLockType.Job);

            SPMinuteSchedule schedule = new SPMinuteSchedule();
            schedule.BeginSecond = 1;
            schedule.EndSecond = 5;
            schedule.Interval = 2;

            timerJob.Schedule = schedule;

            SPSecurity.RunWithElevatedPrivileges(delegate {timerJob.Update();});
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            SPWebApplication webApplication = ((SPSite) properties.Feature.Parent).WebApplication;

            deleteJob(webApplication);
        }


      

        private void deleteJob(SPWebApplication webApplication)
        {
            foreach (SPJobDefinition job in webApplication.JobDefinitions)
            {
                if (job.Name.Equals(TimerJobName))
                {
                    job.Delete();
                }
            }
        }
    }
}
