using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.Data;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Data.SqlClient;
using System.IO;
using System.CodeDom.Compiler;
using System.Xml;
using System.Data.Linq;
using System.Linq;

namespace Attune.Kernel.Build
{
    public class StoredProcedureParameter
    {
        string parameterName;
        string parameterType;
        int parameterLength;
        int parameterScale;
        int parameterPrecision;
        bool isParameterOutput;

        public StoredProcedureParameter() { }
        public StoredProcedureParameter(string parameterName, string parameterType, int parameterLength, int parameterPrecision, int parameterScale, bool isParameterOutput)
        {
            this.ParameterName = parameterName;
            this.ParameterType = parameterType;
            this.ParameterLength = parameterLength;
            this.IsParameterOutput = isParameterOutput;
            this.ParameterPrecision = parameterPrecision;
            this.ParameterScale = parameterScale;
        }

        public string ParameterName
        {
            get { return parameterName; }
            set { parameterName = value; }
        }

        public string ParameterType
        {
            get { return parameterType; }
            set { parameterType = value; }
        }

        public int ParameterLength
        {
            get { return parameterLength; }
            set { parameterLength = value; }
        }

        public bool IsParameterOutput
        {
            get { return isParameterOutput; }
            set { isParameterOutput = value; }
        }
        public int ParameterPrecision
        {
            get { return parameterPrecision; }
            set { parameterPrecision = value; }
        }

        public int ParameterScale
        {
            get
            {
                return parameterScale;
            }
            set
            {
                parameterScale = value;
            }
        }
    }

    public class StoredProcedureParameterCollection : System.Collections.CollectionBase
    {
        public void Add(StoredProcedureParameter item)
        {
            base.List.Add(item);
        }

        public void Remove(StoredProcedureParameter item)
        {
            base.List.Remove(item);
        }

        public StoredProcedureParameter this[int index]
        {
            get
            {
                return (StoredProcedureParameter)base.List[index];
            }
            set
            {
                base.List[index] = value;
            }
        }
    }
    public class CommandGenerator
    {
        public CommandGenerator()
        {

        }

        public void GenerateCommand(string sqlConnectionString, string filePath, List<ModuleConfig> lstModules, ProgressBar _progressBar1, Label lblStatus)
        {

            if (lstModules == null)
            {
                lstModules = new List<ModuleConfig>();
                lstModules.Add(new ModuleConfig() { });
            }

            List<SpNames> lstSps;
            Filldtsp(sqlConnectionString, out lstSps);
            if (lstSps != null && lstSps.Count == 0)
            {
                MessageBox.Show("Command Creation Failed.Check The Stored Procedure");
                return;
            }
            int ProgressCount = 0;
            if (_progressBar1 != null)
            {
                _progressBar1.Maximum = lstModules.Count;
                _progressBar1.Value = 0;
                lblStatus.Text = "Generating Command";
            }
            else
            {
                ConsoleProgressBar.total = lstModules.Count;
                Console.WriteLine();
                Console.WriteLine("Generating Command");
            }
            for (int i = 0; i < lstModules.Count; i++)
            {
                #region CommandFileGeneration
                string NameSpace = "";
                string Class = lstModules[i].Name + "_Command";
                if (string.IsNullOrEmpty(lstModules[i].NameSpace))
                {
                    NameSpace = "Attune.Kernel.Commands";
                }
                else
                {
                    NameSpace = lstModules[i].NameSpace;
                }
                System.Windows.Forms.Application.DoEvents();
                CodeCompileUnit compileUnit = new CodeCompileUnit();
                CodeNamespace ns = new CodeNamespace(NameSpace);
                ns.Imports.Add(new CodeNamespaceImport("System"));
                ns.Imports.Add(new CodeNamespaceImport("System.Text"));
                ns.Imports.Add(new CodeNamespaceImport("System.Data"));
                ns.Imports.Add(new CodeNamespaceImport("System.Data.SqlClient"));

                // create our class
                CodeTypeDeclaration ctd = new CodeTypeDeclaration(Class);
                ctd.IsClass = true;
                ns.Types.Add(ctd);
                compileUnit.Namespaces.Add(ns);
                //GetdtSP
                //DataTable dtSPs = new DataTable();
                //DataColumn dcSpName = new DataColumn("SpName");
                //DataColumn dcParameterName = new DataColumn("ParameterName");
                //DataColumn dcSystemType = new DataColumn("SystemType");
                //DataColumn dcLength = new DataColumn("Length");
                //DataColumn dcPrecision = new DataColumn("Numeric_Precision");
                //DataColumn dcScale = new DataColumn("Numeric_Scale");
                //DataColumn dcIsOutputParameter = new DataColumn("IsOutputParameter");

                //dtSPs = new DataTable();
                //dtSPs.Columns.Add(dcSpName);
                //dtSPs.Columns.Add(dcParameterName);
                //dtSPs.Columns.Add(dcSystemType);
                //dtSPs.Columns.Add(dcLength);
                //dtSPs.Columns.Add(dcPrecision);
                //dtSPs.Columns.Add(dcScale);
                //dtSPs.Columns.Add(dcIsOutputParameter);

                //List<SpNames> dtHelper = lstSps;

                //  dtHelper.PrimaryKey = new DataColumn[] { dtHelper.Columns[0] };
                //foreach (DataRow row in dtSPs.Rows)
                //{
                //bool _isValid = false;
                //if (lstModules[i].SpName != null && lstModules[i].SpName.Count > 0)
                //{
                //    if (lstModules[i].SpName.Contains(row["SpName"].ToString()))
                //    {
                //        _isValid = true;
                //    }
                //}
                //else
                //{
                //    _isValid = true;
                //}

                //if (!dtHelper.Rows.Contains(row["SpName"]) && _isValid)
                //{
                for (int k = 0; k < lstModules[i].SpName.Count; k++)
                {


                    List<SpNames> lstSpParms = (from c in lstSps
                                                where c.SpName.ToLower() == lstModules[i].SpName[k].ToLower() orderby c.ORDINAL_POSITION
                                                select c).ToList();

                    if (lstSpParms.Count > 0)
                    {

                        //dtHelper.ImportRow(row);
                        //DataRow[] rowCol = dtSPs.Select("SpName = '" + row["SpName"].ToString() + "'");
                        StoredProcedureParameterCollection parameterCollection = new StoredProcedureParameterCollection();
                        foreach (SpNames rowParameter in lstSpParms)
                        {
                            if (!string.IsNullOrEmpty(rowParameter.ParameterName))
                            {
                                StoredProcedureParameter parameter = new StoredProcedureParameter();
                                parameter.ParameterName = rowParameter.ParameterName;
                                parameter.ParameterType = rowParameter.SystemType;
                                parameter.ParameterLength = rowParameter.Length;
                                parameter.ParameterPrecision = rowParameter.Numeric_Precision;
                                parameter.ParameterScale = rowParameter.Numeric_Scale;
                                parameter.IsParameterOutput = rowParameter.IsOutputParameter;

                                parameterCollection.Add(parameter);
                            }

                        }
                        //this.PrintToLog(string.Format("Generating method for {0} ...", row["SpName"]));
                        System.Windows.Forms.Application.DoEvents();
                        // create method for each storedProcedure in this loop and pass parameters to this method as StoredProcedureParameterCollection object

                        this.CreateMethodForExecuteSP(ctd, lstModules[i].SpName[k], parameterCollection);
                        // output is in csharp

                        // finally, generate our code to specified codeProvider


                    }
                }
                //}

                if (lstModules[i].SpName.Count > 0)
                {
                    this.GenerateCode(new Microsoft.CSharp.CSharpCodeProvider(), compileUnit, filePath + "\\" + lstModules[i].Name + "\\" + lstModules[i].Name + "_Command\\" + lstModules[i].Name + "_Command");

                }

                #endregion

                if (_progressBar1 != null)
                {
                    _progressBar1.Value = _progressBar1.Value + 1;

                }
                else
                {
                    ProgressCount++;
                   // ConsoleProgressBar.drawTextProgressBar(ProgressCount);
                }
            }
            using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(@"Software\SourceForge\CSharpCommandGen"))
            {
                registryKey.SetValue("Connection", sqlConnectionString, RegistryValueKind.String);
                registryKey.SetValue("Path", filePath, RegistryValueKind.String);
            }

        }

        public List<ModuleConfig> GetSPfromConfig()
        {
            List<ModuleConfig> lstSpList = new List<ModuleConfig>();
            XmlDocument xdoc = new XmlDocument();
            string FolderPath = Application.StartupPath;
            xdoc.Load(FolderPath + "\\ModuleCommandConfig.xml");
            foreach (XmlNode item in xdoc)
            {
                foreach (XmlElement item2 in item.ChildNodes)
                {
                    //foreach (XmlElement item3 in item2.ChildNodes)
                    //{
                    string _moduleName = Convert.ToString(item2.Attributes["Name"].Value);
                    string _nameSpace = Convert.ToString(item2.Attributes["NameSpace"].Value);
                    double _frameWork = 3.5;
                    if (item2.HasAttribute("FrameWork"))
                    {
                        _frameWork = Convert.ToDouble(item2.Attributes["FrameWork"].Value);
                    }

                    // }
                    List<string> _spList = new List<string>();
                    foreach (XmlElement item3 in item2.ChildNodes)
                    {
                        _spList.Add(item3.InnerText);
                    }
                    lstSpList.Add(new ModuleConfig() { Name = _moduleName, NameSpace = _nameSpace, IsSelected = false, SpName = _spList, FrameWork = _frameWork });
                }
            }
            return lstSpList;
        }

        private void CreateMethodForExecuteSP(CodeTypeDeclaration ctd, string spName, StoredProcedureParameterCollection parameterCollection)
        {
            int iTimeoutValue = 180;
            StoredProcedureParameter outPutParameterInfo = null;
            CodeVariableDeclarationStatement commandOutputParameterInfo = null;
            CodeAssignStatement assignReturnedValueToOutputParameter = null;

            // declaration method
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = (method.Attributes & ~MemberAttributes.AccessMask) | MemberAttributes.Public;
            method.Attributes = (method.Attributes & ~MemberAttributes.ScopeMask) | MemberAttributes.Static;
            method.ReturnType = new CodeTypeReference(typeof(SqlCommand));
            method.Name = spName + "Command";


            CodeVariableDeclarationStatement cvds_cmd = new CodeVariableDeclarationStatement(typeof(SqlCommand), "cmd");
            CodeAssignStatement assignment_new_cmd = new CodeAssignStatement(new CodeVariableReferenceExpression(cvds_cmd.Name), new CodeObjectCreateExpression(typeof(SqlCommand)));
            CodeAssignStatement assignment_cmd_commandText = new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(cvds_cmd.Name), "CommandText"), new CodePrimitiveExpression(spName));
            CodeAssignStatement assignment_cmd_commandtimeout = new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(cvds_cmd.Name), "CommandTimeout"), new CodePrimitiveExpression(iTimeoutValue));
            CodeAssignStatement assignment_cmd_commandType = new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(cvds_cmd.Name), "CommandType"), new CodeSnippetExpression("CommandType.StoredProcedure"));
            // return Command object
            CodeMethodReturnStatement return_method = new CodeMethodReturnStatement(new CodeVariableReferenceExpression(cvds_cmd.Name));

            method.Statements.Add(cvds_cmd);
            method.Statements.Add(assignment_new_cmd);
            method.Statements.Add(assignment_cmd_commandText);
            method.Statements.Add(assignment_cmd_commandtimeout);
            method.Statements.Add(assignment_cmd_commandType);

            // define method parameters
            if (parameterCollection.Count > 0)
            {
                foreach (StoredProcedureParameter parameter in parameterCollection)
                {
                    if (parameter.ParameterName != "NULL")
                    {
                        CodeParameterDeclarationExpression method_parameter = new CodeParameterDeclarationExpression(this.GetParameterType(parameter), parameter.ParameterName.Replace("@", ""));
                        if (parameter.IsParameterOutput)
                            method_parameter.Direction = FieldDirection.Out;
                        method.Parameters.Add(method_parameter);

                        if (parameter.IsParameterOutput)
                        {
                            commandOutputParameterInfo = new CodeVariableDeclarationStatement(this.GetParameterType(parameter), "_" + parameter.ParameterName.Replace("@", ""), new CodeObjectCreateExpression(typeof(System.Data.SqlClient.SqlParameter)));
                            if (this.GetParameterType(parameter) == typeof(int)
                                || this.GetParameterType(parameter) == typeof(Int16)
                                || this.GetParameterType(parameter) == typeof(Int32)
                                || this.GetParameterType(parameter) == typeof(Int64)
                                || this.GetParameterType(parameter) == typeof(decimal)
                                || this.GetParameterType(parameter) == typeof(float)
                                || this.GetParameterType(parameter) == typeof(double))
                            {
                                CodeAssignStatement initializeOutputVariable = new CodeAssignStatement(new CodeVariableReferenceExpression(method_parameter.Name), new CodeSnippetExpression("-1"));
                                method.Statements.Add(initializeOutputVariable);
                            }
                            else if (this.GetParameterType(parameter) == typeof(byte))
                            {
                                CodeAssignStatement initializeOutputVariable = new CodeAssignStatement(new CodeVariableReferenceExpression(method_parameter.Name), new CodeSnippetExpression("0"));
                                method.Statements.Add(initializeOutputVariable);
                            }
                            else if (this.GetParameterType(parameter) == typeof(bool))
                            {
                                CodeAssignStatement initializeOutputVariable = new CodeAssignStatement(new CodeVariableReferenceExpression(method_parameter.Name), new CodeSnippetExpression("false"));
                                method.Statements.Add(initializeOutputVariable);
                            }
                            else if (this.GetParameterType(parameter) == typeof(Guid))
                            {
                                CodeAssignStatement initializeOutputVariable = new CodeAssignStatement(new CodeVariableReferenceExpression(method_parameter.Name), new CodeSnippetExpression("System.Guid.Empty"));
                                method.Statements.Add(initializeOutputVariable);
                            }

                            else if (this.GetParameterType(parameter) == typeof(DataTable))
                            {
                                CodeAssignStatement initializeOutputVariable = new CodeAssignStatement(new CodeVariableReferenceExpression(method_parameter.Name), new CodeSnippetExpression("null"));
                                method.Statements.Add(initializeOutputVariable);
                            }
                            else if (this.GetParameterType(parameter) == typeof(DateTime))
                            {
                                CodeAssignStatement initializeOutputVariable = new CodeAssignStatement(new CodeVariableReferenceExpression(method_parameter.Name), new CodeSnippetExpression("System.DateTime.Now"));
                                method.Statements.Add(initializeOutputVariable);
                            }



                            else
                            {
                                CodeAssignStatement initializeOutputVariable = new CodeAssignStatement(new CodeVariableReferenceExpression(method_parameter.Name), new CodeSnippetExpression("null"));
                                method.Statements.Add(initializeOutputVariable);
                            }
                        }

                        CodeVariableDeclarationStatement commandParameter = new CodeVariableDeclarationStatement(typeof(System.Data.SqlClient.SqlParameter), "_" + parameter.ParameterName.Replace("@", ""), new CodeObjectCreateExpression(typeof(System.Data.SqlClient.SqlParameter)));
                        CodeAssignStatement assign_parameter_name = new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(commandParameter.Name), "ParameterName"), new CodePrimitiveExpression(parameter.ParameterName));
                        CodeAssignStatement assign_parameter_size = new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(commandParameter.Name), "Size"), new CodeSnippetExpression(parameter.ParameterLength.ToString()));
                        CodeAssignStatement assign_parameter_value = new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(commandParameter.Name), "Value"), new CodeArgumentReferenceExpression(method_parameter.Name));
                        CodeAssignStatement assign_parameter_precision = new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(commandParameter.Name), "Precision"), new CodeArgumentReferenceExpression(parameter.ParameterPrecision.ToString()));
                        CodeAssignStatement assign_parameter_scale = new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(commandParameter.Name), "Scale"), new CodeArgumentReferenceExpression(parameter.ParameterScale.ToString()));
                        CodeMethodInvokeExpression add_parameter = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(cvds_cmd.Name), "Parameters.Add", new CodeVariableReferenceExpression(commandParameter.Name));
                        method.Statements.Add(commandParameter);
                        method.Statements.Add(assign_parameter_name);
                        method.Statements.Add(assign_parameter_size);
                        method.Statements.Add(assign_parameter_value);
                        method.Statements.Add(assign_parameter_precision);
                        method.Statements.Add(assign_parameter_scale);
                        method.Statements.Add(add_parameter);


                        if (parameter.IsParameterOutput)
                        {
                            CodeAssignStatement assign_parameter_direction = new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(commandParameter.Name), "Direction"), new CodeSnippetExpression("ParameterDirection.Output"));
                            method.Statements.Add(assign_parameter_direction);

                            // if parameter is output, then copy parameter information to outPutParameterInfo
                            outPutParameterInfo = new StoredProcedureParameter(parameter.ParameterName, parameter.ParameterType, parameter.ParameterLength, parameter.ParameterPrecision, parameter.ParameterScale, parameter.IsParameterOutput);
                        }
                    }
                }

                // check if outPutParameterInfo is not null (output parameter exist in the method) then pass returned value from 
                // storedProcedure parameter to this object
                if (outPutParameterInfo != null)
                {
                    if (commandOutputParameterInfo != null)
                    {
                        assignReturnedValueToOutputParameter = new CodeAssignStatement(new CodeVariableReferenceExpression(commandOutputParameterInfo.Name.Replace("_", "")), new CodeCastExpression(commandOutputParameterInfo.Type, new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("_" + outPutParameterInfo.ParameterName.Replace("@", "")), "Value")));
                    }
                }
            }

            if (assignReturnedValueToOutputParameter != null)
                method.Statements.Add(assignReturnedValueToOutputParameter);
            method.Statements.Add(return_method);
            ctd.Members.Add(method);
        }

        // get type of each stored procedure parameter
        private Type GetParameterType(StoredProcedureParameter item)
        {
            Type fieldType;
            if (item.ParameterType.ToLower() == SqlDbType.TinyInt.ToString().ToLower())
                fieldType = typeof(System.Byte);
            else if (item.ParameterType.ToLower() == SqlDbType.SmallInt.ToString().ToLower())
                fieldType = typeof(System.Int16);
            else if (item.ParameterType.ToLower() == SqlDbType.Int.ToString().ToLower())
                fieldType = typeof(System.Int32);
            else if (item.ParameterType.ToLower() == SqlDbType.BigInt.ToString().ToLower())
                fieldType = typeof(System.Int64);
            else if (item.ParameterType.ToLower() == SqlDbType.Money.ToString().ToLower())
                fieldType = typeof(System.Single);
            else if (item.ParameterType.ToLower() == SqlDbType.Float.ToString().ToLower())
                fieldType = typeof(System.Double);
            else if (item.ParameterType.ToLower() == SqlDbType.Char.ToString().ToLower() || item.ParameterType.ToLower() == SqlDbType.NChar.ToString().ToLower() || item.ParameterType.ToLower() == SqlDbType.VarChar.ToString().ToLower() || item.ParameterType.ToLower() == SqlDbType.NVarChar.ToString().ToLower() || item.ParameterType.ToLower() == SqlDbType.Text.ToString().ToLower() || item.ParameterType.ToLower() == SqlDbType.NText.ToString().ToLower() || item.ParameterType.ToLower() == SqlDbType.Xml.ToString().ToLower())
                fieldType = typeof(System.String);
            else if (item.ParameterType.ToLower() == SqlDbType.Decimal.ToString().ToLower() || item.ParameterType.ToLower() == SqlDbType.Real.ToString().ToLower())
                fieldType = typeof(System.Decimal);
            else if (item.ParameterType.ToLower() == SqlDbType.Image.ToString().ToLower() || item.ParameterType.ToLower() == SqlDbType.VarBinary.ToString().ToLower())
                fieldType = typeof(System.Byte[]);
            else if (item.ParameterType.ToLower() == SqlDbType.DateTime.ToString().ToLower() || item.ParameterType.ToLower() == SqlDbType.SmallDateTime.ToString().ToLower() || item.ParameterType.ToLower() == SqlDbType.Date.ToString().ToLower() || item.ParameterType.ToLower() == SqlDbType.DateTime.ToString().ToLower() || item.ParameterType.ToLower() == SqlDbType.DateTime2.ToString().ToLower())
                fieldType = typeof(System.DateTime);
            else if (item.ParameterType.ToLower() == SqlDbType.DateTimeOffset.ToString().ToLower())
                fieldType = typeof(System.DateTimeOffset);
            else if (item.ParameterType.ToLower() == SqlDbType.Bit.ToString().ToLower())
                fieldType = typeof(System.Boolean);
            else if (item.ParameterType.ToLower() == SqlDbType.UniqueIdentifier.ToString().ToLower())
                fieldType = typeof(System.Guid);
            else if (item.ParameterType.ToLower() == "table type")
                fieldType = typeof(System.Data.DataTable);
            else
                fieldType = typeof(System.Object);
            return fieldType;
        }

        public void GenerateCode(Microsoft.CSharp.CSharpCodeProvider provider, CodeCompileUnit compileUnit, string fileName)
        {
            // Build the source file name with the appropriate
            // language extension.
            String sourceFile;
            if (provider.FileExtension[0] == '.')
            {
                sourceFile = fileName + provider.FileExtension;
            }
            else
            {
                sourceFile = fileName + "." + provider.FileExtension;
            }

            // Create an IndentedTextWriter, constructed with
            // a StreamWriter to the source file.
            if (File.Exists(sourceFile))
            {
                File.Delete(sourceFile);
            }
            IndentedTextWriter tw = new IndentedTextWriter(new StreamWriter(sourceFile, true), "    ");

            // Generate source code using the code generator.
            provider.GenerateCodeFromCompileUnit(compileUnit, tw, new CodeGeneratorOptions());

            // Close the output file.
            tw.Close();
        }

        private void Filldtsp(string sqlConnectionString, out List<SpNames> lstSps)
        {
            lstSps = new List<SpNames>();
            using (SqlConnection con = new SqlConnection(sqlConnectionString))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "spRetrieveSPDetails";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = con;
                cmd.CommandTimeout = 180;
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                SqlDataReader dr = cmd.ExecuteReader();
                int nSpName = dr.GetOrdinal("SpName");
                int nParameterName = dr.GetOrdinal("ParameterName");
                int nSystemType = dr.GetOrdinal("SystemType");
                int nLength = dr.GetOrdinal("Length");
                int nNumeric_Precision = dr.GetOrdinal("Numeric_Precision");
                int nNumeric_Scale = dr.GetOrdinal("Numeric_Scale");
                int nIsOutputParameter = dr.GetOrdinal("IsOutputParameter");

                while (dr.Read())
                {
                    SpNames objSp = new SpNames();
                    objSp.SpName = Convert.ToString(dr[nSpName]);
                    objSp.ParameterName = Convert.ToString(dr[nParameterName]);
                    objSp.SystemType = Convert.ToString(dr[nSystemType]);
                    objSp.Length = dr.IsDBNull(nLength) ? 0 : Convert.ToInt32(dr[nLength]);
                    objSp.Numeric_Precision = dr.IsDBNull(nNumeric_Precision) ? 0 : Convert.ToInt32(dr[nNumeric_Precision]);
                    objSp.Numeric_Scale = dr.IsDBNull(nNumeric_Scale) ? 0 : Convert.ToInt32(dr[nNumeric_Scale]);
                    objSp.IsOutputParameter = dr.IsDBNull(nIsOutputParameter) ? false : Convert.ToInt64(dr[nIsOutputParameter]) == 1 ? true : false;
                    lstSps.Add(objSp);
                }
                if (con.State != ConnectionState.Closed)
                {
                    con.Close();
                }
            }
        }
        // set first charachter to LowerCase 
        private string ToLowerFirstChar(string text)
        {
            char[] chars = text.ToCharArray();
            string s = chars[0].ToString();
            s = s.ToLower();
            chars[0] = s[0];
            StringBuilder sb = new StringBuilder();
            foreach (char ch in chars)
            {
                sb.Append(ch);
            }
            return sb.ToString();
        }

        // set first charachter to UpperCase 
        private string ToUpperFirstChar(string text)
        {
            char[] chars = text.ToCharArray();
            string s = chars[0].ToString();
            s = s.ToUpper();
            chars[0] = s[0];
            StringBuilder sb = new StringBuilder();
            foreach (char ch in chars)
            {
                sb.Append(ch);
            }
            return sb.ToString();
        }
    }

}
