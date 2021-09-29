using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Attune.Kernel.Build
{
    /// <summary>
    /// Delegate to count the update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    internal delegate void CountUpdate(object sender, CountEventArgs e);

    /// <summary>
    /// Generates C# and SQL code for accessing a database.
    /// </summary>
    internal static class Generator
    {
        /// <summary>
        /// Database counted - Test
        /// </summary>
        public static event CountUpdate DatabaseCounted;

        /// <summary>
        /// Tables counted
        /// </summary>
        public static event CountUpdate TableCounted;

        /// <summary>
        /// Generates the SQL and C# code for the specified database.
        /// </summary>
        /// <param name="outputDirectory">The directory where the C# and SQL code should be created.</param>
        /// <param name="connectionString">The connection string to be used to connect the to the database.</param>
        /// <param name="grantLoginName">The SQL Server login name that should be granted execute rights on the generated stored procedures.</param>
        /// <param name="storedProcedurePrefix">The prefix that should be used when creating stored procedures.</param>
        /// <param name="createMultipleFiles">A flag indicating if the generated stored procedures should be created in one file or separate files.</param>
        /// <param name="targetNamespace">The namespace that the generated C# classes should contained in.</param>
        /// <param name="daoSuffix">The suffix to be applied to all generated DAO classes.</param>
        public static void Generate(string outputDirectory, string connectionString, string grantLoginName, string storedProcedurePrefix, bool createMultipleFiles, string targetNamespace, string daoSuffix, ProgressBar _ProgressBar1, Label lblStatus, string EntityPath)
        {
            List<Table> tableList = new List<Table>();
            string databaseName;
            string sqlPath;
            string csPath;
            int ProgressCount = 0;
            List<string> lstIllegalStrings = new List<string>() { "#", " ", "$" };
            if (_ProgressBar1 != null)
            {
                _ProgressBar1.BackColor = System.Drawing.Color.Red;
                lblStatus.Text = "Getting table Information ...";
            }
            else
            {
                Console.WriteLine("Getting table Information ...");
            }
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                databaseName = Utility.FormatPascal(connection.Database);
                sqlPath = Path.Combine(outputDirectory, "SQL");
                csPath = outputDirectory;

                connection.Open();

                // Get a list of the entities in the database
                DataTable dataTable = new DataTable();
                SqlDataAdapter dataAdapter = new SqlDataAdapter(Utility.GetTableQuery(connection.Database), connection);
                dataAdapter.Fill(dataTable);
                if (_ProgressBar1 != null)
                {
                    _ProgressBar1.Maximum = dataTable.Rows.Count;
                    _ProgressBar1.Value = 0;
                }
                else
                {
                    ConsoleProgressBar.total = dataTable.Rows.Count;
                }

                // Process each table
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    Application.DoEvents();
                    Table table = new Table();
                    table.Name = (string)dataRow["TABLE_NAME"];
                    if (lstIllegalStrings.Where(o => table.Name.Contains(o)).Count() == 0)
                    {
                        QueryTable(connection, table);
                        if (lstIllegalStrings.Where(o => table.Columns.Where(a => a.Name.Contains(o)).Count() > 0).Count() == 0)
                        {
                            tableList.Add(table);
                        }
                    }
                    if (_ProgressBar1 != null)
                    {
                        _ProgressBar1.Value = _ProgressBar1.Value + 1;
                    }
                    else
                    {
                        ProgressCount++;
                       // ConsoleProgressBar.drawTextProgressBar(ProgressCount);
                    }
                }
            }

            // Generate the necessary SQL and C# code for each table
            int count = 0;
            if (_ProgressBar1 != null)
            {
                _ProgressBar1.Value = 0;
                lblStatus.Text = "Generating Entities ...";
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Generating Entities ...");
                ProgressCount = 0;
                //ConsoleProgressBar.drawTextProgressBar(ProgressCount);


            }

            if (tableList.Count > 0)
            {
                Application.DoEvents();
                SqlGenerator.CreateUserQueries(databaseName, grantLoginName, sqlPath, createMultipleFiles);

                // Create the CRUD stored procedures and data access code for each table
                List<string> lstEntity = new List<string>();
                foreach (Table table in tableList)
                {
                    Application.DoEvents();
                    table.Name = Utility.FormatClassName(table.Name);
                    string FileItemName = table.Name + ".cs";
                    lstEntity.Add(FileItemName);
                    CsGenerator.CreateDataTransferClass(table, targetNamespace, storedProcedurePrefix, csPath, EntityPath);
                    count++;
                    //TableCounted(null, new CountEventArgs(count));
                    if (_ProgressBar1 != null)
                    {
                        _ProgressBar1.Value = _ProgressBar1.Value + 1;
                    }
                    else
                    {
                        ProgressCount++;
                      //  ConsoleProgressBar.drawTextProgressBar(ProgressCount);
                    }
                }
                string EntityProjName = Path.Combine(csPath + "BusinessEntities.csproj");
                CsGenerator.AddFilesToUnitTestProject(lstEntity, EntityProjName, "");

            }
        }

        /// <summary>
        /// Retrieves the column, primary key, and foreign key information for the specified table.
        /// </summary>
        /// <param name="connection">The SqlConnection to be used when querying for the table information.</param>
        /// <param name="table">The table instance that information should be retrieved for.</param>
        private static void QueryTable(SqlConnection connection, Table table)
        {
            // Get a list of the entities in the database
            DataTable dataTable = new DataTable();
            SqlDataAdapter dataAdapter = new SqlDataAdapter(Utility.GetColumnQuery(table.Name), connection);
            dataAdapter.Fill(dataTable);

            foreach (DataRow columnRow in dataTable.Rows)
            {
                Column column = new Column();
                column.Name = columnRow["COLUMN_NAME"].ToString();
                column.Type = columnRow["DATA_TYPE"].ToString();
                column.Precision = columnRow["NUMERIC_PRECISION"].ToString();
                column.Scale = columnRow["NUMERIC_SCALE"].ToString();

                // Determine the column's length
                if (columnRow["CHARACTER_MAXIMUM_LENGTH"] != DBNull.Value)
                {
                    column.Length = columnRow["CHARACTER_MAXIMUM_LENGTH"].ToString();
                }
                else
                {
                    column.Length = columnRow["COLUMN_LENGTH"].ToString();
                }

                // Is the column a RowGuidCol column?
                if (columnRow["IS_ROWGUIDCOL"].ToString() == "1")
                {
                    column.IsRowGuidCol = true;
                }

                // Is the column an Identity column?
                if (columnRow["IS_IDENTITY"].ToString() == "1")
                {
                    column.IsIdentity = true;
                }

                // Is columnRow column a computed column?
                if (columnRow["IS_COMPUTED"].ToString() == "1")
                {
                    column.IsComputed = true;
                }

                table.Columns.Add(column);
            }

            // Get the list of primary keys
            DataTable primaryKeyTable = Utility.GetPrimaryKeyList(connection, table.Name);
            foreach (DataRow primaryKeyRow in primaryKeyTable.Rows)
            {
                string primaryKeyName = primaryKeyRow["COLUMN_NAME"].ToString();

                foreach (Column column in table.Columns)
                {
                    if (column.Name == primaryKeyName)
                    {
                        table.PrimaryKeys.Add(column);
                        break;
                    }
                }
            }

            // Get the list of foreign keys
            DataTable foreignKeyTable = Utility.GetForeignKeyList(connection, table.Name);
            foreach (DataRow foreignKeyRow in foreignKeyTable.Rows)
            {
                string name = foreignKeyRow["FK_NAME"].ToString();
                string columnName = foreignKeyRow["FKCOLUMN_NAME"].ToString();

                if (table.ForeignKeys.ContainsKey(name) == false)
                {
                    table.ForeignKeys.Add(name, new List<Column>());
                }

                List<Column> foreignKeys = table.ForeignKeys[name];

                foreach (Column column in table.Columns)
                {
                    if (column.Name == columnName)
                    {
                        foreignKeys.Add(column);
                        break;
                    }
                }
            }
        }
    }
}
