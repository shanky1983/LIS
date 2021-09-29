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
using Microsoft.Web.Administration;
using Microsoft.Web.Management;
using System.Globalization;
using System.DirectoryServices;
using System.Data.SqlClient;


using System.Diagnostics;
using System.DirectoryServices;
using System.Management;
using System.Net;
using Microsoft.Win32;
using System.Web;
using System.Xml.Linq;

namespace Attune.Kernel.Build
{
    public partial class ModuleSelector : Form
    {
        private string Connectionstring;
        private string AppPath;
        private string XmlPath;
        private string ApiPath;
        private string NameSpace;
        private string EntityPath;

        string serverName = System.Environment.MachineName;
        public ModuleSelector()
        {

        }
        public void GetIISInfo()
        {
            using (Microsoft.Web.Administration.ServerManager serverManager = Microsoft.Web.Administration.ServerManager.OpenRemote(serverName))
            {
                comboBox1.Items.Clear();
                foreach (var site in serverManager.Sites)
                {
                    foreach (var application in site.Applications)
                    {
                        if (application.Path.Replace("/", "") != "")
                        {
                            comboBox1.Items.Add(application.Path.Replace("/", ""));
                        }
                    }
                }
                comboBox2.Items.Clear();
                foreach (var apppools in serverManager.ApplicationPools)
                {
                    comboBox2.Items.Add(apppools.Name);
                }
            }
        }
        public ModuleSelector(string _connectionstring, string _appPath, string _xmlPath, string _apiPath, string _NameSpace, string _entityPath)
        {
            Connectionstring = _connectionstring;
            AppPath = _appPath;
            XmlPath = _xmlPath;
            ApiPath = _apiPath;
            NameSpace = _NameSpace;
            EntityPath = _entityPath;
            InitializeComponent();
            lstModule = GetConfig(XmlPath);
            lstApiConfig = GetApiConfig(ApiPath);
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

            List<String> lstBuildFlow = new List<string>() { "DataBase", "Entity", "Command", "Module", "WebApi", "WebApp" };
            foreach (var item in lstBuildFlow)
            {
                CheckBox btn = new CheckBox();
                btn.Name = item;
                btn.Text = item;
                btn.Size = new Size(100, 25);
                flowLayoutPanel3.Controls.Add(btn);
            }

            #region iis config
            lblServerName.Text = System.Environment.MachineName;
            string[] DatabaseName = Connectionstring.Split(';', '=');
            lblDatabaseName.Text = DatabaseName[3];
            GetIISInfo();
            #endregion

            XDocument WebApp = XDocument.Load("F:\\VSSProduct\\Application\\App\\WebApp\\Web.config");
            var WebAppconnection = WebApp.Descendants("connectionStrings");
            txtwebAppConnection.Text = WebAppconnection.First().LastNode.ToString();

            XDocument Api = XDocument.Load("F:\\VSSProduct\\Application\\PlatFormAPI\\PlatForm_API\\Web.config");
            var Apiconnection = Api.Descendants("connectionStrings");
            txtApiConnection.Text = Apiconnection.First().LastNode.ToString();
                
        }
        List<ModuleConfig> lstModule { get; set; }
        List<ApiConfig> lstApiConfig { get; set; }

        private void button1_Click(object sender, EventArgs e)
        {

            button1.Enabled = false;
            System.Windows.Forms.Application.DoEvents();
            StartBuild();
            button1.Enabled = true;
        }

        private bool StartBuild()
        {
            List<String> lstBuildFlow = new List<string>();
            ApiConfig objApiConfig = new ApiConfig();
            #region BuildFlowSelection
            for (int k = 0; k < flowLayoutPanel3.Controls.Count; k++)
            {
                if (flowLayoutPanel3.Controls[k].GetType() == typeof(CheckBox) && ((CheckBox)flowLayoutPanel3.Controls[k]).Checked)
                {

                    lstBuildFlow.Add(flowLayoutPanel3.Controls[k].Name);

                    if (flowLayoutPanel3.Controls[k].Name == "WebApi")
                    {
                        if (txtApiPublishPath.Text != "")
                        {
                            for (int i = 0; i < lstApiConfig.Count; i++)
                            {
                                lstApiConfig[i].Name = "PlatFormApi";
                                lstApiConfig[i].IsSelected = true;
                                lstApiConfig[i].ApiPublishPath = txtApiPublishPath.Text;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Provide Api Publish Path");
                            return false;
                        }
                    }

                }
            }
            #endregion

            if (lstBuildFlow.Count > 0)
            {
                #region ModuleSelection
                ModuleConfig objConfig;
                List<ModuleConfig> lstPackageModule = new List<ModuleConfig>();
                if (tabControl1.SelectedIndex == 0)
                {
                    for (int i = 0; i < flowLayoutPanel1.Controls.Count; i++)
                    {
                        if (flowLayoutPanel1.Controls[i].GetType() == typeof(CheckBox) && ((CheckBox)flowLayoutPanel1.Controls[i]).Checked)
                        {
                            objConfig = lstModule.Where(o => o.Name == flowLayoutPanel1.Controls[i].Name).FirstOrDefault();
                            lstPackageModule.Add(objConfig);
                        }

                }
            }
            if (tabControl1.SelectedIndex == 1)
            {
                for (int i = 0; i < flowLayoutPanel2.Controls.Count; i++)
                {
                    if (flowLayoutPanel2.Controls[i].GetType() == typeof(RadioButton) && ((RadioButton)flowLayoutPanel2.Controls[i]).Checked)
                    {

                        lstPackageModule = lstModule.Where(o => o.Category.Contains(flowLayoutPanel2.Controls[i].Name)).ToList();

                    }

                    }
                }
                #endregion


                buildProgress bp = new buildProgress(AppPath, Connectionstring, NameSpace, lstPackageModule, lstBuildFlow, lstApiConfig, EntityPath);
                DialogResult dr = bp.ShowDialog();
                if (dr == DialogResult.No)
                {
                    MessageBox.Show("Build failed...");
                }
                else
                {
                    MessageBox.Show("Build succeed...");
                }
                return true;
            }
            else
            {
                MessageBox.Show("There was no option select for build");
                return false;
            }

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



        public List<ModuleConfig> GetConfig(string _xmlPath)
        {
            List<ModuleConfig> lstSpList = new List<ModuleConfig>();
            XmlDocument xdoc = new XmlDocument();
            string FolderPath = System.Windows.Forms.Application.StartupPath;

            xdoc.Load(_xmlPath);
            foreach (XmlNode item in xdoc)
            {
                foreach (XmlElement item2 in item.SelectNodes("Module"))
                {
                    string _solutionPath = "";
                    string _moduleName = Convert.ToString(item2.Attributes["Name"].Value);
                    string _nameSpace = "";
                    if (item2.HasAttribute("SolutionPath"))
                    {
                        _solutionPath = Convert.ToString(item2.Attributes["SolutionPath"].Value);
                    }
                    string _Category = "";
                    if (item2.HasAttribute("Category"))
                    {
                        _Category = Convert.ToString(item2.Attributes["Category"].Value);
                    }
                    if (item2.HasAttribute("NameSpace"))
                    {
                        _nameSpace = Convert.ToString(item2.Attributes["NameSpace"].Value);
                    }
                    double _frameWork = 3.5;
                    if (item2.HasAttribute("FrameWork"))
                    {
                        _frameWork = Convert.ToDouble(item2.Attributes["FrameWork"].Value);
                    }
                    List<string> _spList = new List<string>();
                    foreach (XmlElement item3 in item2.SelectNodes("SPName"))
                    {
                        _spList.Add(item3.InnerText);
                    }
                    ModuleConfig mc = new ModuleConfig();
                    mc.Name = _moduleName;
                    mc.SolutionPath = _solutionPath;
                    mc.Category = new List<string>();
                    mc.Category.Add("AllModule");
                    mc.Category.AddRange(_Category.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                    mc.SpName = _spList;
                    mc.NameSpace = _nameSpace;
                    mc.FrameWork = _frameWork;
                    lstSpList.Add(mc);
                }
            }
            return lstSpList;
        }
        public List<ApiConfig> GetApiConfig(string _apiPath)
        {
            List<ApiConfig> lstSpList = new List<ApiConfig>();
            XmlDocument xdoc = new XmlDocument();
            string FolderPath = System.Windows.Forms.Application.StartupPath;

            xdoc.Load(_apiPath);
            foreach (XmlNode item in xdoc)
            {
                foreach (XmlElement item2 in item.SelectNodes("Api"))
                {
                    string _solutionPath = "";
                    string _moduleName = Convert.ToString(item2.Attributes["Name"].Value);
                    if (item2.HasAttribute("SolutionPath"))
                    {
                        _solutionPath = Convert.ToString(item2.Attributes["SolutionPath"].Value);
                    }
                    ApiConfig mc = new ApiConfig();
                    mc.Name = _moduleName;
                    mc.SolutionPath = _solutionPath;
                    mc.IsSelected = false;
                    mc.ApiPublishPath = "";
                    lstSpList.Add(mc);
                }
            }
            return lstSpList;
        }
        private void flowLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void ModuleSelector_Activated(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            txtApiPublishPath.Text = folderBrowserDialog1.SelectedPath;
            txtApplicationPath.Text = folderBrowserDialog1.SelectedPath + "\\Api";
        }

        private void Back_Click(object sender, EventArgs e)
        {
            ModuleSelector ms = new ModuleSelector();
            ms.Close();

            BuildHome frm = new BuildHome();
            frm.Show();
            Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (txtConfigValue.Text != "")
            {
                using (SqlConnection con = new SqlConnection(Connectionstring))
                {
                    try
                    {
                        if (con.State != System.Data.ConnectionState.Open)
                            con.Open();
                        SqlCommand cmd = new SqlCommand();
                        cmd.CommandText = "UPDATE COM SET COM.ConfigValue = '"+txtConfigValue.Text+"' FROM ConfigOrgMaster COM JOIN ConfigKeyMaster CKM ON COM.ConFigKeyID = CKM.ConFigKeyID WHERE CKM.ConFigKey = '"+txtConfigKey.Text+"'";
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.Connection = con;
                        List<string> lstStoredProcedures = new List<string>();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr != null)
                            {
                                while (dr.Read())
                                {
                                    lstStoredProcedures.Add(Convert.ToString(dr["name"]));
                                }
                            }
                        }
                        MessageBox.Show("Api Config Updated Successfully");
                        #region Clear Cache
                        string[] filePaths = System.IO.Directory.GetFiles(@"F:\VSSProduct\Application\App\WebApp\App_Data\", "*.txt");
                        DialogResult res = MessageBox.Show("Are You Want Clear Appliction Cache Files?", "Clear Cache Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (res == DialogResult.Yes)
                        {
                            if (filePaths.Length > 0)
                            {
                                foreach (string file in filePaths)
                                {
                                    System.IO.File.Delete(file);
                                }
                                MessageBox.Show("Application Cache Cleared  Successfully");
                            }
                            else
                            {
                                MessageBox.Show("Application Cache Already Cleared !");
                            }
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        CLogger.LogWarning(ex, "Error in SQL Login");
                        MessageBox.Show("Error Updating Api Config");
                    }
                }
            }
            else
            {
                MessageBox.Show("Provide Config Value");
                return;
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            folderBrowserDialog2.ShowDialog();
            txtApplicationPath.Text = folderBrowserDialog2.SelectedPath;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (txtAppPoolName.Text != "" && ddlframework.SelectedItem.ToString() != "")
            {
                #region Create App Pool
                using (ServerManager serverManager = new ServerManager())
                {
                    if (serverManager.ApplicationPools[txtAppPoolName.Text] != null)
                        return;
                    ApplicationPool newPool = serverManager.ApplicationPools.Add(txtAppPoolName.Text);
                    newPool.ManagedRuntimeVersion = ddlframework.SelectedItem.ToString();
                    serverManager.CommitChanges();


                    MessageBox.Show(txtAppPoolName.Text+" App Pool Created Successfully");

                    txtAppPoolName.Text = "";
                    ddlframework.SelectedItem = "";
                    GetIISInfo();

                    return;
                }
                #endregion
            }
            else
            {
                if (txtAppPoolName.Text == "")
                {
                    MessageBox.Show("Provide App Pool Name");
                    return;
                }
                else if (ddlframework.SelectedItem.ToString() == "")
                {
                    MessageBox.Show("Provide App Pool Frame Work");
                    return;
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (txtApplicationName.Text != "" && txtApplicationPath.Text != "")
            {
                #region Create Application
                using (ServerManager serverManager = new ServerManager())
                {
                    foreach (var site in serverManager.Sites)
                    {
                        var applications = site.Applications;
                        applications.Add("/" + txtApplicationName.Text, txtApplicationPath.Text);
                        serverManager.CommitChanges();

                        MessageBox.Show(txtApplicationName.Text + " Application Created Successfully");

                        txtApplicationName.Text = "";
                        txtApplicationPath.Text = "";
                        GetIISInfo();
                    }
                }
                #endregion
            }
            else
            {
                if (txtApplicationName.Text == "")
                {
                    MessageBox.Show("Provide Application Name");
                    return;
                }
                else if (txtApplicationPath.Text == "")
                {
                    MessageBox.Show("Provide Application Path");
                    return;
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (Microsoft.Web.Administration.ServerManager serverManager = Microsoft.Web.Administration.ServerManager.OpenRemote(serverName))
            {
                foreach (var site in serverManager.Sites)
                {
                    foreach (var application in site.Applications)
                    {
                        if (application.Path.Replace("/","").ToString() == comboBox1.SelectedItem.ToString())
                        {
                            txtHostedAppPath.Text = application.VirtualDirectories[0].PhysicalPath;
                             
                            comboBox2.SelectedItem = application.ApplicationPoolName;
                            txtConfigValue.Text = application.EnabledProtocols + "://" + serverName+"/"+comboBox1.SelectedItem.ToString()+"/";
                        }
                    }
                }
                foreach (var apppools in serverManager.ApplicationPools)
                {
                    if (comboBox2.SelectedItem.ToString() == apppools.Name)
                    {
                        ddlHostedFrameWork.SelectedItem = apppools.ManagedRuntimeVersion;
                    }
                }
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {

            folderBrowserDialog3.ShowDialog();
            txtHostedAppPath.Text = folderBrowserDialog3.SelectedPath;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            using (Microsoft.Web.Administration.ServerManager serverManager = Microsoft.Web.Administration.ServerManager.OpenRemote(serverName))
            {
                foreach (var site in serverManager.Sites)
                {
                    foreach (var application in site.Applications)
                    {
                        if (application.Path.Replace("/", "").ToString() == comboBox1.SelectedItem.ToString())
                        {
                            application.VirtualDirectories[0].PhysicalPath = txtHostedAppPath.Text;
                            application.ApplicationPoolName = comboBox2.SelectedItem.ToString();
                            serverManager.CommitChanges();

                            MessageBox.Show("Changes Saved Successfully ");
                        }
                    }
                    foreach (var apppools in serverManager.ApplicationPools)
                    {
                        if (comboBox2.SelectedItem.ToString() == apppools.Name)
                        {
                            apppools.ManagedRuntimeVersion = ddlHostedFrameWork.SelectedItem.ToString();
                        }
                    }
                }
            }
        }
        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (Microsoft.Web.Administration.ServerManager serverManager = Microsoft.Web.Administration.ServerManager.OpenRemote(serverName))
            {
                foreach (var apppools in serverManager.ApplicationPools)
                {
                    if (comboBox2.SelectedItem.ToString() == apppools.Name)
                    {
                        ddlHostedFrameWork.SelectedItem = apppools.ManagedRuntimeVersion;
                    }
                }
            }
        }
    }
}
