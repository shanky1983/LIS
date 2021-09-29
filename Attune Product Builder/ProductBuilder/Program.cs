using Attune.Kernel.Build;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProductBuilder
{
    class Program
    {
        static void Main(string[] args)
        {


#if DEBUG
//            string AppPath = "F:\\VSSProduct\\Application";
//            string ServerName = ".\\SQL2012";
//            string UserName = "sa";
//            string password = "A$$une";
//            string dbName = "HIS_Pdt_QA";
//            string entityPath = "F:\\VSSProduct\\Build Utilities\\AdditionalEntityModel.xml";
//            string BuildConfig = "F:\\VSSProduct\\Build Utilities\\ModuleConfig.xml";
//            string BuildOptions = null;
//#else
            if (args.Count() > 6)
            {
                string AppPath = args[0];
                string ServerName = args[1];
                string UserName = args[2];
                string password = args[3];
                string dbName = args[4];
                string entityPath = args[5];
                string BuildConfig = args[6];
                string BuildOptions = null;
                if (args.Count() == 8)
                {
                    BuildOptions = args[7];
                }
#endif
            string connectionString = "";
                if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(password))
                {
                    connectionString = "Server=" + ServerName + "; Database=" + dbName + "; User ID=" + UserName + "; Password=" + password + ";";
                }

                ModuleSelector ms = new ModuleSelector();
                List<ModuleConfig> lstPackageModule = ms.GetConfig(BuildConfig);
                List<string> buildFlow = null;
                List<ApiConfig> lstApiConfig = null;
                if (BuildOptions == null)
                {
                    buildFlow = new List<string>() { "DataBase", "Entity", "Command", "Module", "WebApi", "WebApp" };

                }
                else if (BuildOptions.Contains(","))
                {
                    buildFlow = BuildOptions.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                else
                {
                    buildFlow = new List<string> { BuildOptions.Trim() };
                }

                string startupPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                new AutoMatedBuild(AppPath, connectionString, "Attune.Kernel.BusinessEntities", lstPackageModule, buildFlow,lstApiConfig, entityPath, startupPath, null, null).StartBuild();
            }
            Console.WriteLine("Arguments missing. Please check the arguments.");
        }

    }
}
