using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using System.Windows.Forms;
using System.Configuration;

namespace Attune.Kernel.Build
{
    public class DataBaseExecutor
    {
        private string _Connectionstring;
        Label lblStatus = null;
        ProgressBar ProgressBar1 = null;
        public DataBaseExecutor(string Connectionstring, Label _lblStatus, ProgressBar _progressBar)
        {
            _Connectionstring = Connectionstring;
            if (_lblStatus != null)
                lblStatus = _lblStatus;
            if (_progressBar != null)
            {
                ProgressBar1 = _progressBar;
            }
        }
        public bool ExecuteDbFiles(string ApplicationPath)
        {
            #region Drop Sps
            using (SqlConnection con = new SqlConnection(_Connectionstring))
            {
                if (con.State != System.Data.ConnectionState.Open)
                    con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "Select name from sys.procedures";
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
                if (ProgressBar1 == null)
                {
                    ConsoleProgressBar.total = lstStoredProcedures.Count();
                    Console.WriteLine("Deleting Stored procedures...");
                }
                else
                {
                    lblStatus.Text = "Deleting Stored procedures...";
                    ProgressBar1.Maximum = lstStoredProcedures.Count();
                    ProgressBar1.Value = 0;
                }
                int ProgressCount = 0;
                foreach (string sp in lstStoredProcedures)
                {
                    SqlCommand cmd2 = new SqlCommand();
                    cmd2.Connection = con;
                    cmd2.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "drop procedure [" + sp+"]";
                    cmd.ExecuteNonQuery();

                    ProgressCount++;
                    if (ProgressBar1 == null)
                    {
                        //  ConsoleProgressBar.drawTextProgressBar(ProgressCount);
                    }
                    else
                    {
                        ProgressBar1.Value = ProgressCount;
                    }
                    Application.DoEvents();
                }


            }

            #endregion

            executeSQl(ApplicationPath + "\\DatabaseFiles\\Table.sql");
            executeSQl(ApplicationPath + "\\DatabaseFiles\\AlterScript.sql");
            executeSQl(ApplicationPath + "\\DatabaseFiles\\UDT.sql");
            executeSQl(ApplicationPath + "\\DatabaseFiles\\Function Script.sql");
            //executeSQl(ApplicationPath + "\\DatabaseFiles\\GeneralScripts.sql");
			//executeSQl(ApplicationPath + "\\DatabaseFiles\\OrgBaseScript.sql");
            ExecuteSPs(ApplicationPath + "\\DatabaseFiles\\SP_Scripts\\");
            return true;
        }

        private void executeSQl(string strFile)
        {
            if (lblStatus == null)
            {
                Console.WriteLine("Executing " + strFile);
            }
            else
            {
                lblStatus.Text = "Executing " + strFile;
            }
            try
            {

                using (SqlConnection con = new SqlConnection(_Connectionstring))
                {
                    if (con.State != System.Data.ConnectionState.Open)
                        con.Open();
                    using (StreamReader sr = new StreamReader(strFile))
                    {
                        string sqlQuery = sr.ReadToEnd();
                        Server server = new Server(new ServerConnection(con));
                        server.ConnectionContext.ExecuteNonQuery(sqlQuery);
                    }
                }
            }
            catch (Exception ex)
            {
                CLogger.LogWarning(ex, "Error in " + strFile);
                MessageBox.Show(ex.ToString());
                return;
            }
            Application.DoEvents();
        }

        public void ExecuteSPs(string strFolder)
        {
            //if (lblStatus == null)
            //{
            //    Console.WriteLine("Executing Stored procedures...");
            //}
            //else
            //{
            //    lblStatus.Text = "Executing Stored procedures...";
            //}

            using (SqlConnection con = new SqlConnection(_Connectionstring))
            {
                if (con.State != System.Data.ConnectionState.Open)
                    con.Open();

                DirectoryInfo di = new DirectoryInfo(strFolder);
                FileInfo[] Fi = di.GetFiles("*.txt", SearchOption.AllDirectories);
                if (ProgressBar1 == null)
                {
                    ConsoleProgressBar.total = Fi.Count();
                }
                else
                {
                    ProgressBar1.Maximum = Fi.Count();
                    ProgressBar1.Value = 0;
                }
                int ProgressCount = 0;
                foreach (FileInfo file in Fi)
                {
                    try
                    {
                        using (StreamReader sr = new StreamReader(file.FullName))
                        {
                            if (lblStatus == null)
                            {
                                Console.WriteLine("Executing Stored procedures " + file.Name + "");
                            }
                            else
                            {
                                lblStatus.Text = "Executing Stored procedures " + file.Name + "";
                            }
                            string sqlQuery = sr.ReadToEnd();
                            Server server = new Server(new ServerConnection(con));
                            server.ConnectionContext.ExecuteNonQuery(sqlQuery);
                        }
                        ProgressCount++;
                        if (ProgressBar1 == null)
                        {
                            ConsoleProgressBar.drawTextProgressBar(ProgressCount);
                        }
                        else
                        {
                            ProgressBar1.Value = ProgressCount;
                        }
                        Application.DoEvents();
                    }
                    catch (Exception ex)
                    {
                        CLogger.LogWarning(ex, "Error in " + file.Name);
                    }
                }


            }
        }
    }

}


