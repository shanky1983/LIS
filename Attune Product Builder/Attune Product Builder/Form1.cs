using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Build;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using System.Xml;
using System.IO;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            lstModule = GetConfig();
            List<string> lstCategory = new List<string>();
            foreach (var item in lstModule)
            {
                CheckBox btn = new CheckBox();
                btn.Name = item.Name;
                btn.Text = item.Name;
                btn.Size = new Size(150, 25);
                flowLayoutPanel1.Controls.Add(btn);
                lstCategory.AddRange(item.Category);
            }
            lstCategory = (from c in lstCategory
                           group c by c into d
                           select d.Key).ToList();
            foreach (var item in lstCategory)
            {
                RadioButton btn = new RadioButton();
                btn.Name = item;
                btn.Text = item;
                btn.Size = new Size(150, 25);
                flowLayoutPanel2.Controls.Add(btn);
            }

        }
        List<ModuleConfig> lstModule { get; set; }

        private void button1_Click(object sender, EventArgs e)
        {

            ModuleConfig objConfig;
            if (tabControl1.SelectedIndex == 0)
            {
                for (int i = 0; i < flowLayoutPanel1.Controls.Count; i++)
                {
                    if (flowLayoutPanel1.Controls[i].GetType() == typeof(CheckBox) && ((CheckBox)flowLayoutPanel1.Controls[i]).Checked)
                    {
                        objConfig = lstModule.Where(o => o.Name == flowLayoutPanel1.Controls[i].Name).FirstOrDefault();

                        if (!PreBuildModule(objConfig))
                        {
                            break;
                        }
                    }

                }
            }
            if (tabControl1.SelectedIndex == 1)
            {
                List<ModuleConfig> lstPackageModule = new List<ModuleConfig>();
                for (int i = 0; i < flowLayoutPanel2.Controls.Count; i++)
                {
                    if (flowLayoutPanel2.Controls[i].GetType() == typeof(RadioButton) && ((RadioButton)flowLayoutPanel2.Controls[i]).Checked)
                    {
                        lstPackageModule = lstModule.Where(o => o.Category.Contains(flowLayoutPanel2.Controls[i].Name)).ToList();
                        for (int j = 0; j < lstPackageModule.Count; j++)
                        {
                            if (!PreBuildModule(lstPackageModule[j]))
                            {
                                break;

                            }
                        }

                    }

                }
            }

            #region CopyReference dll
            Directory.GetFiles("F:\\VSSProduct\\Application\\Bin").ToList().ForEach(f => File.Copy(f, @"F:\VSSProduct\Application\App\WebApp\Bin" + "\\" + Path.GetFileName(f), true));
            #endregion
            if (MessageBox.Show("Do you want to build the Web Application?", "Confirm pre build...", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                #region WebAppBuild
                BuildResult _buildResult = null;
                MyBuildLogger _buildlog = new MyBuildLogger();
                objConfig = new ModuleConfig() { Name = "WebApp", SolutionPath = "App/WebApp.sln" };
                _buildResult = BuildModule(objConfig, ref _buildlog);
                if (_buildResult.OverallResult == BuildResultCode.Failure)
                {
                    MessageBox.Show(_buildlog.BuildErrors);
                    return;
                }
                #endregion
            }



            MessageBox.Show("Build Completed");

        }

        private bool PreBuildModule(ModuleConfig objConfig)
        {
            string _webApp = @"F:\VSSProduct\Application\App\WebApp\";
            BuildResult _buildResult = null;
            MyBuildLogger _buildlog = new MyBuildLogger();
            _buildResult = BuildModule(objConfig, ref _buildlog);

            if (_buildResult != null && _buildResult.OverallResult == BuildResultCode.Failure)
            {
                MessageBox.Show("Error in " + objConfig.Name);
                MessageBox.Show(_buildlog.BuildErrors);
                return false;
            }
            else
            {
                string _moduleWebApp = "F:\\VSSProduct\\Application\\" + objConfig.Name + "\\WebApp\\";

                if (Directory.Exists(_moduleWebApp))
                {
                    foreach (string dirPath in Directory.GetDirectories(_moduleWebApp, "*",
                      SearchOption.AllDirectories))
                        Directory.CreateDirectory(dirPath.Replace(_moduleWebApp, _webApp));
                    foreach (string newPath in Directory.GetFiles(_moduleWebApp, "*.*", SearchOption.AllDirectories))
                        File.Copy(newPath, newPath.Replace(_moduleWebApp, _webApp), true);


                }
            }
            return true;
        }

        private BuildResult BuildModule(ModuleConfig objConfig, ref MyBuildLogger buildlog)
        {
            BuildResult buildResult = null;

            if (!string.IsNullOrEmpty(objConfig.SolutionPath))
            {
                string projectFileName = @"F:/VSSProduct/Application/" + objConfig.SolutionPath;
                if (File.Exists(projectFileName))
                {
                    ProjectCollection pc = new ProjectCollection();
                    Dictionary<string, string> GlobalProperty = new Dictionary<string, string>();
                    GlobalProperty.Add("Configuration", "Debug");
                    GlobalProperty.Add("OutputPath", @"F:\VSSProduct\Application\Bin");

                    BuildParameters bp = new BuildParameters(pc);

                    bp.Loggers = new List<ILogger>() { buildlog };

                    BuildRequestData BuidlRequest = new BuildRequestData(projectFileName, GlobalProperty, "3.5", new string[] { "Rebuild" }, null);
                    buildResult = BuildManager.DefaultBuildManager.Build(bp, BuidlRequest);
                }
            }
            return buildResult;
        }

        private List<ModuleConfig> GetConfig()
        {
            List<ModuleConfig> lstSpList = new List<ModuleConfig>();
            XmlDocument xdoc = new XmlDocument();
            string FolderPath = Application.StartupPath;
            xdoc.Load(FolderPath + "\\ModuleConfig.xml");
            foreach (XmlNode item in xdoc)
            {
                foreach (XmlElement item2 in item.ChildNodes)
                {
                    string _moduleName = Convert.ToString(item2.Attributes["Name"].Value);
                    string _solutionPath = Convert.ToString(item2.Attributes["SolutionPath"].Value);
                    string _Category = "";
                    if (item2.HasAttribute("Category"))
                    {
                        _Category = Convert.ToString(item2.Attributes["Category"].Value);
                    }
                    ModuleConfig mc = new ModuleConfig();
                    mc.Name = _moduleName;
                    mc.SolutionPath = _solutionPath;
                    mc.Category = new List<string>();
                    mc.Category.Add("AllModule");
                    mc.Category.AddRange(_Category.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                    lstSpList.Add(mc);
                }
            }
            return lstSpList;
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            foreach (var chkbox in flowLayoutPanel1.Controls)
            {
                if (chkbox.GetType() == typeof(CheckBox))
                {
                    ((CheckBox)chkbox).Checked = checkBox1.Checked;
                }
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void flowLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
