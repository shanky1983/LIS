using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Attune.Kernel.Build
{
    public partial class BuildHome : Form
    {
        public BuildHome()
        {
            InitializeComponent();

            using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(@"Software\SourceForge\CSharpDataTier"))
            {
                txtServer.Text = (string)registryKey.GetValue("Server", String.Empty);
                //txtDatabase.Text = (string)registryKey.GetValue("Database", String.Empty);
			    ddldatabae.SelectedText = (string)registryKey.GetValue("Database", String.Empty);
                if (((int)registryKey.GetValue("AuthenticationType", 1)) == 1)
                {
                    rbWindowsAuth.Checked = true;
                }
                else
                {
                    rbSQLAuth.Checked = true;
                }
                txtLogin.Text = (string)registryKey.GetValue("Login", String.Empty);

                txtNameSpace.Text = (string)registryKey.GetValue("Namespace", String.Empty);
                txtSuffix.Text = (string)registryKey.GetValue("DaoSuffix", String.Empty);
                txtAppPath.Text = (string)registryKey.GetValue("AppPath", String.Empty);
                txtXMLPath.Text = (string)registryKey.GetValue("BuildConfigxml", String.Empty);
                txtAdditionalEntity.Text = (string)registryKey.GetValue("Entityxml", String.Empty);
            }
        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }
        string connectionString = null;
        private void button3_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtServer.Text) && !string.IsNullOrEmpty(ddldatabae.Text) && ddldatabae.Items.Count > 0)
            {
                if (rbWindowsAuth.Checked)
                {
                    connectionString = "Server=" + txtServer.Text + "; Database=" + ddldatabae.Text + "; Integrated Security=SSPI;";
                }
                else
                {
                    if (!string.IsNullOrEmpty(txtLogin.Text) && !string.IsNullOrEmpty(txtPassword.Text))
                    {
                        connectionString = "Server=" + txtServer.Text + "; Database=" + ddldatabae.Text + "; User ID=" + txtLogin.Text + "; Password=" + txtPassword.Text + ";";
                    }
                    //else
                    //{
                     //   MessageBox.Show("Please enter username and password");
                     //   return;
                    //}

                }
                if (Directory.Exists(txtAppPath.Text) && File.Exists(txtXMLPath.Text) && File.Exists(txtApiPath.Text) && !string.IsNullOrEmpty(txtNameSpace.Text))
                {
                   
                    string[] subdir = Directory.GetDirectories(txtAppPath.Text);
                    if (subdir.Contains(Path.Combine(txtAppPath.Text,"BusinessEntities")))
                    {
                        this.Hide();
                        new ModuleSelector(connectionString, txtAppPath.Text, txtXMLPath.Text, txtApiPath.Text, txtNameSpace.Text, txtAdditionalEntity.Text).Show();
                        using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(@"Software\SourceForge\CSharpDataTier"))
                        {
                            registryKey.SetValue("Server", txtServer.Text);
                            //registryKey.SetValue("Database", txtDatabase.Text);
                            registryKey.SetValue("Database", ddldatabae.SelectedText);
                            registryKey.SetValue("AuthenticationType", rbWindowsAuth.Checked ? 1 : 2);
                            registryKey.SetValue("Login", txtLogin.Text);
                            registryKey.SetValue("Namespace", txtNameSpace.Text);
                            registryKey.SetValue("DaoSuffix", txtSuffix.Text);
                            registryKey.SetValue("AppPath", txtAppPath.Text);
                            registryKey.SetValue("BuildConfigxml", txtXMLPath.Text);
                            registryKey.SetValue("Entityxml", txtAdditionalEntity.Text);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Business entities folder not found in this application path");
                        return;
                    }
                }
                else
                {
                    if (!Directory.Exists(txtAppPath.Text))
                    {
                        MessageBox.Show("Provide Application path");
                        return;
                    }
                    if (!File.Exists(txtXMLPath.Text))
                    {
                        MessageBox.Show("Provide Module Config path");
                        return;
                    }
                    if (!File.Exists(txtApiPath.Text))
                    {
                        MessageBox.Show("Provide Api Config path");
                        return;
                    }
                    if (string.IsNullOrEmpty(txtNameSpace.Text))
                    {
                        MessageBox.Show("Provide Name Space");
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("No Database Available ! ");
                return;
            }


        }

        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            txtAppPath.Text = folderBrowserDialog1.SelectedPath;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            if (openFileDialog1.CheckFileExists)
            {
                txtXMLPath.Text = openFileDialog1.FileName;
            }

        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            openFileDialog2.ShowDialog();
            if (openFileDialog2.CheckFileExists)
            {
                txtAdditionalEntity.Text = openFileDialog2.FileName;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            openFileDialog3.ShowDialog();
            if (openFileDialog3.CheckFileExists)
            {
                txtApiPath.Text = openFileDialog3.FileName;
            }
        }

        private void BuildHome_Load(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
           
            if (!string.IsNullOrEmpty(txtServer.Text))
            {
                if (rbWindowsAuth.Checked)
                {
                    connectionString = "Server=" + txtServer.Text + "; Integrated Security=SSPI;";
                }
                else
                {
                    if (!string.IsNullOrEmpty(txtLogin.Text) && !string.IsNullOrEmpty(txtPassword.Text))
                    {
                        connectionString = "Server=" + txtServer.Text + "; User ID=" + txtLogin.Text + "; Password=" + txtPassword.Text + ";";
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(txtLogin.Text))
                        {
                            MessageBox.Show("Please Enter SQL User Name !");
                            button6.Enabled = true;
                            button8.Enabled = false;
                            return;
                        }
                        if (string.IsNullOrEmpty(txtPassword.Text))
                        {
                            MessageBox.Show("Please Enter SQL Password !");
                            button6.Enabled = true;
                            button8.Enabled = false;
                            return;
                        }

                    }

                }
            }
            else
            {
                if (!string.IsNullOrEmpty(txtServer.Text))
                {
                    MessageBox.Show("Please Enter Server Name !");
                    button6.Enabled = true;
                    button8.Enabled = false;
                    return;
                }
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    if (con.State != System.Data.ConnectionState.Open)
                        con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "Select * from master.dbo.sysdatabases where name Not In ('master','tempdb','model','msdb','ReportServer$SA','ReportServer$SATempDB')";
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
                    if (lstStoredProcedures.Count > 0)
                    {
                        ddldatabae.DataSource = lstStoredProcedures;
                        button6.Enabled = false;
                        button8.Enabled = true;
                        txtServer.Enabled = false;
                        txtLogin.Enabled = false;
                        txtPassword.Enabled = false;
                        rbWindowsAuth.Enabled = false;
                        rbSQLAuth.Enabled = false;
                    }
                    else
                    {
                        MessageBox.Show("No Database Available");
                        button6.Enabled = true;
                        button8.Enabled = false;
                        return;
                    }
                }
                catch (Exception ex)
                {
                    CLogger.LogWarning(ex, "Error in SQL Login");
                    MessageBox.Show("SQL Login Failed ! Provide Valid Details");
                    button6.Enabled = true;
                    button8.Enabled = false;
                    return;
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (txtPassword.PasswordChar != '\0') 
            {
                txtPassword.PasswordChar = '\0';
            }
            else
            {
                txtPassword.PasswordChar='*';
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            List<string> lstEmpty = new List<string>();
            ddldatabae.DataSource = lstEmpty;
            button6.Enabled = true;
            button8.Enabled = false;
            txtPassword.Text = "";
            txtServer.Enabled = true;
            txtLogin.Enabled = true;
            txtPassword.Enabled = true;
            rbWindowsAuth.Enabled = true;
            rbSQLAuth.Enabled = true;
            MessageBox.Show("SQL Connection Closed");
            return;
            
        }

        private void rbSQLAuth_CheckedChanged(object sender, EventArgs e)
        {
            txtLogin.Enabled = true;
            txtPassword.Enabled = true;
        }

        private void rbWindowsAuth_CheckedChanged(object sender, EventArgs e)
        {
            txtLogin.Enabled = false;
            txtPassword.Enabled = false;
            txtLogin.Text = "";
            txtPassword.Text = "";
        }
    }
}
