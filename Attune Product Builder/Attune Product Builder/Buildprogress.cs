using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Attune.Kernel.Build
{
    public partial class buildProgress : Form
    {
        public string AppPath;
        public string Connectionstring;
        public string NameSpace;
        public string EntityPath;
        public List<ModuleConfig> lstPackageModule;
        public List<ApiConfig> lstApiConfig;

        List<string> lstBuildFlow = new List<string>();

        public buildProgress(string _appPath, string _connectionString, string _nameSpace, List<ModuleConfig> _lstPackageModule, List<string> _lstBuildFlow, List<ApiConfig> _lstApiConfig, string _entityPath)
        {
            InitializeComponent();
            AppPath = _appPath;
            Connectionstring = _connectionString;
            NameSpace = _nameSpace;
            lstPackageModule = _lstPackageModule;
            lstBuildFlow = _lstBuildFlow;
            lstApiConfig = _lstApiConfig;
            EntityPath = _entityPath;

        }





        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception)
            {

                throw;
            }

        }

        private void buildProgress_Load(object sender, EventArgs e)
        {

        }

        private void buildProgress_Activated(object sender, EventArgs e)
        {

        }

        private void buildProgress_Shown(object sender, EventArgs e)
        {
            if (new AutoMatedBuild(AppPath, Connectionstring, NameSpace,  lstPackageModule, lstBuildFlow, lstApiConfig, EntityPath, Application.StartupPath, lblStatus, progressBar1).StartBuild())
            {
                this.DialogResult = DialogResult.Yes;
            }
            else
            {
                this.DialogResult = DialogResult.No;
            }
        }
        private void progressBar1_Click(object sender, EventArgs e)
        {

        }
    }
}
