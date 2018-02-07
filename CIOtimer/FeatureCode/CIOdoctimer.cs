using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace CIOtimer
{
    class CIOdoctimer : SPFeatureReceiver
    {
        const string TASK_LOGGER_JOB_NAME = "CIOTimeTaskLogger";

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            //throw new Exception("The method or operation is not implemented.");
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                // register the the current web
                SPSite site = properties.Feature.Parent as SPSite;

                // make sure the job isn't already registered
                foreach (SPJobDefinition job in site.WebApplication.JobDefinitions)
                {
                    if (job.Name == TASK_LOGGER_JOB_NAME)
                        job.Delete();
                }

                DocTimer docJob = new DocTimer(TASK_LOGGER_JOB_NAME, site.WebApplication);

                //SPWeeklySchedule weekSched = new SPWeeklySchedule();
                //weekSched.BeginDayOfWeek = DayOfWeek.Monday;
                //weekSched.EndDayOfWeek = DayOfWeek.Monday;
                //weekSched.BeginHour = 0;
                //weekSched.EndHour = 1;



                SPDailySchedule schedule = new SPDailySchedule();
                schedule.BeginHour = 0;
                schedule.EndHour = 1;
                docJob.Schedule = schedule;
                docJob.Update();
            });
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            SPSite objSite = SPContext.Current.Site;
            foreach (SPJobDefinition job in objSite.WebApplication.JobDefinitions)
            {
                if (job.Name == TASK_LOGGER_JOB_NAME)
                {
                    job.Delete();
                    break;
                }
            }

            //throw new Exception("The method or operation is not implemented.");
        }

        public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        {
           // throw new Exception("The method or operation is not implemented.");
        }

        public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        {
          //  throw new Exception("The method or operation is not implemented.");
        }
    }
}
