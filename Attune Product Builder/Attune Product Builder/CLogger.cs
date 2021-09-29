using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace Attune.Kernel.Build
{
    public static class CLogger
    {
        static string filepath = ConfigurationManager.AppSettings["loggerPath"];
        
        public static void LogWarning(Exception ex)
        {
            using (StreamWriter sw = new StreamWriter(filepath, true))
            {
                if (ex.Message.ToString() != "System.Collections.Generic.List`1[System.String]" && ex.Message.ToString() != "")
                {
                    sw.WriteLine("Error occured at : " + DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt"));
                    sw.WriteLine("Error Description : " + ex.Message);
                }
            }
        }
        public static void LogWarning(Exception ex, string strMsg)
        {
            using (StreamWriter sw = new StreamWriter(filepath, true))
            {
                sw.WriteLine("Error occured at : " + DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt"));
                sw.WriteLine("Error Message : " + strMsg);
                sw.WriteLine("Error Description : " + ex.Message);
            }
        }
        public static void LogWarning(MyBuildLogger buildlog)
        {
           
            using (StreamWriter sw = new StreamWriter(filepath, true))
            {
                if (buildlog.BuildErrors.ToString() != "System.Collections.Generic.List`1[System.String]" && buildlog.BuildErrors.ToString() !="")
                {
                    sw.WriteLine("Error occured at : " + DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt"));
                    sw.WriteLine(buildlog.BuildDetails);
                    sw.WriteLine(buildlog.BuildErrors);
                }
            }
        }
    }
}
