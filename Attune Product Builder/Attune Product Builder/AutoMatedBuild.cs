
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Attune.Kernel.Build
{
    public class AutoMatedBuild
    {
        public string AppPath;
        public string ApiPublishPath;
        public string Connectionstring;
        public string NameSpace;
        public string EntityPath;
        public List<ModuleConfig> lstPackageModule;
        List<ApiConfig> lstApiConfig = new List<ApiConfig>();
        List<string> lstBuildFlow = new List<string>();
        public string StartupPath;
        Label lblStatus = null;
        ProgressBar progressBar1 = null;
        public AutoMatedBuild(string _appPath, string _connectionString, string _nameSpace, List<ModuleConfig> _lstPackageModule, List<string> _lstBuildFlow, List<ApiConfig> _lstApiConfig, string _entityPath, string _startupPath, Label _lblStatus, ProgressBar _progress)
        {
            AppPath = _appPath;
            Connectionstring = _connectionString;
            NameSpace = _nameSpace;
            lstPackageModule = _lstPackageModule;
            lstBuildFlow = _lstBuildFlow;
            lstApiConfig = _lstApiConfig;
            EntityPath = _entityPath;
            StartupPath = _startupPath;
            if (_lblStatus != null)
                lblStatus = _lblStatus;
            if (_progress != null)
                progressBar1 = _progress;
        }
        public bool StartBuild()
        {
            ModuleConfig objConfig;
            #region ModuleSelection
            #endregion
            Application.DoEvents();

            #region DBExecution
            if (lstBuildFlow.Contains("DataBase"))
            {
                #region EntityGeneration
                DataBaseExecutor de = new DataBaseExecutor(Connectionstring, lblStatus, progressBar1);
                de.ExecuteDbFiles(AppPath);
                #endregion

            }
            #endregion


            if (lstBuildFlow.Contains("Entity"))
            {
                #region EntityGeneration
                Generator.Generate(AppPath + "\\BusinessEntities\\", Connectionstring, "", "", false, NameSpace, "", progressBar1, lblStatus, EntityPath);
                #endregion

            }
            Application.DoEvents();
            if (lstBuildFlow.Contains("Command"))
            {
                #region CommandGeneration
                CommandGenerator CG = new CommandGenerator();
                CG.GenerateCommand(Connectionstring, AppPath, lstPackageModule, progressBar1, lblStatus);
                #endregion
            }
            Application.DoEvents();
            #region Build
            if (lstBuildFlow.Contains("Module"))
            {
                int ProgressCount = 0;
                if (progressBar1 != null)
                {
                    progressBar1.Value = 0;
                    progressBar1.Maximum = lstPackageModule.Count;
                }
                else
                {
                    ConsoleProgressBar.total = lstPackageModule.Count;
                    Console.WriteLine();
                    Console.WriteLine("Building Modules...");
                }
                for (int j = 0; j < lstPackageModule.Count; j++)
                {
                    Application.DoEvents();
                    if (!PreBuildModule(lstPackageModule[j], AppPath))
                    {
                        return false;
                    }
                    if (progressBar1 != null)
                    {
                        progressBar1.Value = progressBar1.Value + 1;
                    }
                    else
                    {
                        ProgressCount++;
                        //ConsoleProgressBar.drawTextProgressBar(ProgressCount);
                    }
                }

                #region CopyReference dll
                if (progressBar1 != null)
                {
                    lblStatus.ForeColor = Color.YellowGreen;
                    lblStatus.Text = "Copying Dll Files...";
                }
                Directory.GetFiles(AppPath + "\\Bin").ToList().ForEach(f => File.Copy(f, AppPath + "\\App\\WebApp\\Bin" + "\\" + Path.GetFileName(f), true));
                #endregion
            }
            #endregion
            Application.DoEvents();
            #region Api Build

            if (lstBuildFlow.Contains("WebApi"))
            {

                int ProgressCount = 0;
                if (progressBar1 != null)
                {
                    progressBar1.Value = 0;
                    progressBar1.Maximum = lstApiConfig.Count;
                    Console.WriteLine();
                    Console.WriteLine("Building Api...");
                    lblStatus.ForeColor = Color.Red;
                    lblStatus.Text = "Building Api " + lstApiConfig[0].Name + " ...";
                }
                else
                {
                    ConsoleProgressBar.total = lstApiConfig.Count;
                    Console.WriteLine();
                    Console.WriteLine("Building Api...");
                    lblStatus.ForeColor = Color.Red;
                    lblStatus.Text = "Building Api " + lstApiConfig[0].Name + " ...";
                }

                BuildResult _buildResult = null;
                MyBuildLogger _buildlog = new MyBuildLogger();
                objConfig = new ModuleConfig() { Name = lstApiConfig[0].Name, SolutionPath = lstApiConfig[0].SolutionPath};
                if (lstApiConfig != null && lstApiConfig[0].ApiPublishPath!="")
                {
                    ApiPublishPath = lstApiConfig[0].ApiPublishPath;
                }
                else
                {
                    MessageBox.Show("Provide Api Publish Path");
                    return false;
                }

                _buildResult = BuildApi(objConfig, ref _buildlog, AppPath, ApiPublishPath);
                if (_buildResult != null && _buildResult.OverallResult == BuildResultCode.Failure)
                {
                    MessageBox.Show("Error in " + objConfig.Name);
                    MessageBox.Show(_buildlog.BuildErrors);
                    return false;
                }
                if (progressBar1 != null)
                {
                    progressBar1.Value = progressBar1.Value + 1;
                }
                else
                {
                    ProgressCount++;
                    ConsoleProgressBar.drawTextProgressBar(ProgressCount);
                }

            }
            #endregion
           
            Application.DoEvents();
            if (lstBuildFlow.Contains("WebApp"))
            {

                Application.DoEvents();
                #region WebAppBuild
                if (progressBar1 != null)
                {
                    lblStatus.ForeColor = Color.Green;
                    lblStatus.Text = "Building WebApp ...";
                    progressBar1.Style = ProgressBarStyle.Marquee;
                    progressBar1.Maximum = 100;
                    progressBar1.Value = 10;
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("Building WebApp ...");
                }
                BuildResult _buildResult = null;
                MyBuildLogger _buildlog = new MyBuildLogger() { Verbosity = LoggerVerbosity.Detailed };
                objConfig = new ModuleConfig() { Name = "WebApp", SolutionPath = "App/WebApp.sln" };
                // _buildResult = BuildModule(objConfig, ref _buildlog, AppPath);
                var task = System.Threading.Tasks.Task.Factory.StartNew(() => _buildResult = BuildModule(objConfig, ref _buildlog, AppPath), TaskCreationOptions.LongRunning);
                while (!task.Wait(50))  // wait for 50 milliseconds (make shorter for smoother UI animation)
                {
                    Application.DoEvents();

                }
                Application.DoEvents();
                if (_buildResult.OverallResult == BuildResultCode.Failure)
                {
                    MessageBox.Show(_buildlog.BuildErrors);
                    return false;
                }
                #endregion
            }
            
            string logFile=ConfigurationManager.AppSettings["loggerPath"];
            if (File.Exists(logFile))
            {
                System.Diagnostics.Process.Start(logFile);
            }
            
            return true;
        }

        private bool PreBuildModule(ModuleConfig objConfig, string _webApp)
        {
            //_webApp = _webApp + "\\App\\WebApp\\";
            BuildResult _buildResult = null;
            MyBuildLogger _buildlog = new MyBuildLogger();
            if (progressBar1 != null)
            {
                lblStatus.ForeColor = Color.Blue;
                lblStatus.Text = "Building module " + objConfig.Name + " ...";
            }
            _buildResult = BuildModule(objConfig, ref _buildlog, _webApp);
            if (_buildResult != null && _buildResult.OverallResult == BuildResultCode.Failure)
            {
                MessageBox.Show("Error in " + objConfig.Name);
                MessageBox.Show(_buildlog.BuildErrors);
                return false;
            }
            else
            {
                Application.DoEvents();
                string _moduleWebApp = _webApp + "\\" + objConfig.Name + "\\WebApp\\";
                if (progressBar1 != null)
                {
                    lblStatus.Text = "Copying module " + objConfig.Name + " ...";
                }
                if (Directory.Exists(_moduleWebApp))
                {
                    foreach (string dirPath in Directory.GetDirectories(_moduleWebApp, "*",
                      SearchOption.AllDirectories))
                        Directory.CreateDirectory(dirPath.Replace(_moduleWebApp, _webApp + "\\App\\WebApp\\"));
                    foreach (string newPath in Directory.GetFiles(_moduleWebApp, "*.*", SearchOption.AllDirectories))
                        File.Copy(newPath, newPath.Replace(_moduleWebApp, _webApp + "\\App\\WebApp\\"), true);

                }
                if (!Directory.Exists( _webApp + "\\App\\WebApp\\AppGlobalResources\\"))
                {
                    Directory.CreateDirectory(_webApp + "\\App\\WebApp\\AppGlobalResources\\");
                   
                }
                if (Directory.Exists(_moduleWebApp + "App_GlobalResources"))
                {
                    foreach (string newPath in Directory.GetFiles(_moduleWebApp + "App_GlobalResources", "*.*", SearchOption.AllDirectories))
                        File.Copy(newPath, newPath.Replace(_moduleWebApp + "App_GlobalResources", _webApp + "\\App\\WebApp\\AppGlobalResources\\"), true);
                }
                
                
            }
            return true;
        }

        private BuildResult BuildModule(ModuleConfig objConfig, ref MyBuildLogger buildlog, string _webApp)
        {

            BuildResult buildResult = null;
            if (!string.IsNullOrEmpty(objConfig.SolutionPath))
            {
                string projectFileName = Path.Combine(_webApp, objConfig.SolutionPath);
                if (File.Exists(projectFileName))
                {
                    ProjectCollection pc = new ProjectCollection();
                    Dictionary<string, string> GlobalProperty = new Dictionary<string, string>();
                    GlobalProperty.Add("Configuration", "Debug");
                    if (objConfig.FrameWork==4.5)
                    {
                        GlobalProperty.Add("OutputPath", _webApp + "\\Bin_45");
                    }
                    else
                    {
                        GlobalProperty.Add("OutputPath", _webApp + "\\Bin");
                    }
                    GlobalProperty.Add("BuildInParallel", "true");

                    BuildParameters bp = new BuildParameters(pc);
                    bp.DetailedSummary = true;
                    bp.Loggers = new List<ILogger>() { buildlog };

                    BuildRequestData BuidlRequest = new BuildRequestData(projectFileName, GlobalProperty, "4.0", new string[] { "Rebuild" }, null);
                    buildResult = BuildManager.DefaultBuildManager.Build(bp, BuidlRequest);
                    Application.DoEvents();
                  
                    CLogger.LogWarning(buildlog);

                }

            }
            return buildResult;
        }
        private BuildResult BuildApi(ModuleConfig objConfig, ref MyBuildLogger buildlog, string _buildbinpath, string _apipublishpath)
        {

            BuildResult buildResult = null;
            if (!string.IsNullOrEmpty(objConfig.SolutionPath))
            {
                string projectFileName = Path.Combine(_buildbinpath, objConfig.SolutionPath);
                if (File.Exists(projectFileName))
                {
                    ProjectCollection pc = new ProjectCollection();
                    Dictionary<string, string> GlobalProperty = new Dictionary<string, string>();
                    GlobalProperty.Add("Configuration", "Debug");
                    
                    GlobalProperty.Add("OutputPath", _apipublishpath + "\\Bin");
                    GlobalProperty.Add("BuildInParallel", "true");

                    BuildParameters bp = new BuildParameters(pc);
                    bp.DetailedSummary = true;
                    bp.Loggers = new List<ILogger>() { buildlog };

                    BuildRequestData BuidlRequest = new BuildRequestData(projectFileName, GlobalProperty, "4.0", new string[] { "Rebuild" }, null);
                    buildResult = BuildManager.DefaultBuildManager.Build(bp, BuidlRequest);
                    Application.DoEvents();
                    
                    CLogger.LogWarning(buildlog);

                    //string sourceDirectory = _apipublishpath + "\\Bin\\_PublishedWebsites";
                    //string destinationDirectory = _apipublishpath+"\\Api";
                                        
                    //try
                    //{
                    //    if (Directory.Exists(sourceDirectory))
                    //    {
                    //        if (Directory.Exists(destinationDirectory))
                    //        {
                    //            Directory.Delete(destinationDirectory);
                    //            Directory.Move(sourceDirectory, destinationDirectory);
                    //            Directory.Delete(sourceDirectory);
                    //        }
                    //        else
                    //        {
                    //            Directory.Move(sourceDirectory, destinationDirectory);
                    //        }
                    //    }

                    //}
                    //catch (Exception ex)
                    //{
                    //    Console.WriteLine(ex.Message);
                    //}
                    //Console.ReadLine();
                }

            }
            return buildResult;
        }

    }
}
