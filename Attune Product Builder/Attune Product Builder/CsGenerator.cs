using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Collections;
using System.Data.Linq;
using System.Xml.Linq;
using System.Linq;

namespace Attune.Kernel.Build
{
    /// <summary>
    /// Generates C# data access and data transfer classes.
    /// </summary>
    internal static class CsGenerator
    {
        /// <summary>
        /// Creates a project file that references each generated C# code file for data access.
        /// </summary>
        /// <param name="path">The path where the project file should be created.</param>
        /// <param name="projectName">The name of the project.</param>
        /// <param name="tableList">The list of tables code files were created for.</param>
        /// <param name="daoSuffix">The suffix to append to the name of each data access class.</param>
        internal static void CreateProjectFile(string path, string projectName, List<Table> tableList, string daoSuffix)
        {
            string projectXml = Utility.GetResource("DataTierGenerator.Resources.Project.xml");
            XmlDocument document = new XmlDocument();
            document.LoadXml(projectXml);

            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(document.NameTable);
            namespaceManager.AddNamespace(String.Empty, "http://schemas.microsoft.com/developer/msbuild/2003");
            namespaceManager.AddNamespace("msbuild", "http://schemas.microsoft.com/developer/msbuild/2003");

            document.SelectSingleNode("/msbuild:Project/msbuild:PropertyGroup/msbuild:ProjectGuid", namespaceManager).InnerText = "{" + Guid.NewGuid().ToString() + "}";
            document.SelectSingleNode("/msbuild:Project/msbuild:PropertyGroup/msbuild:RootNamespace", namespaceManager).InnerText = projectName;
            document.SelectSingleNode("/msbuild:Project/msbuild:PropertyGroup/msbuild:AssemblyName", namespaceManager).InnerText = projectName;

            XmlNode itemGroupNode = document.SelectSingleNode("/msbuild:Project/msbuild:ItemGroup[msbuild:Compile]", namespaceManager);
            foreach (Table table in tableList)
            {
                string className = Utility.FormatClassName(table.Name);

                XmlNode dtoCompileNode = document.CreateElement("Compile", "http://schemas.microsoft.com/developer/msbuild/2003");
                XmlAttribute dtoAttribute = document.CreateAttribute("Include");
                dtoAttribute.Value = className + ".cs";
                dtoCompileNode.Attributes.Append(dtoAttribute);
                itemGroupNode.AppendChild(dtoCompileNode);

                XmlNode dataCompileNode = document.CreateElement("Compile", "http://schemas.microsoft.com/developer/msbuild/2003");
                XmlAttribute dataAttribute = document.CreateAttribute("Include");
                dataAttribute.Value = Path.Combine("Data", Utility.FormatClassName(table.Name) + daoSuffix + ".cs");
                dataCompileNode.Attributes.Append(dataAttribute);
                itemGroupNode.AppendChild(dataCompileNode);

                XmlNode mockCompileNode = document.CreateElement("Compile", "http://schemas.microsoft.com/developer/msbuild/2003");
                XmlAttribute mockAttribute = document.CreateAttribute("Include");
                mockAttribute.Value = Path.Combine("Data\\Mocks", Utility.FormatClassName(table.Name) + daoSuffix + "Mock.cs");
                mockCompileNode.Attributes.Append(mockAttribute);
                itemGroupNode.AppendChild(mockCompileNode);
            }

            XmlNode serviceCompileNode = document.CreateElement("Compile", "http://schemas.microsoft.com/developer/msbuild/2003");
            XmlAttribute serviceAttribute = document.CreateAttribute("Include");
            serviceAttribute.Value = "Services\\Service1.cs";
            serviceCompileNode.Attributes.Append(serviceAttribute);
            itemGroupNode.AppendChild(serviceCompileNode);

            XmlNode serviceUtilityCompileNode = document.CreateElement("Compile", "http://schemas.microsoft.com/developer/msbuild/2003");
            XmlAttribute serviceUtilityAttribute = document.CreateAttribute("Include");
            serviceUtilityAttribute.Value = "Services\\ServiceUtility.cs";
            serviceUtilityCompileNode.Attributes.Append(serviceUtilityAttribute);
            itemGroupNode.AppendChild(serviceUtilityCompileNode);

            document.Save(Path.Combine(path, projectName + ".csproj"));
        }

        /// <summary>
        /// Creates the AssemblyInfo.cs file for the project.
        /// </summary>
        /// <param name="path">The root path of the project.</param>
        /// <param name="assemblyTitle">The title of the assembly.</param>
        /// <param name="databaseName">The name of the database the assembly provides access to.</param>
        internal static void CreateAssemblyInfo(string path, string assemblyTitle, string databaseName)
        {
            string assemblyInfo = Utility.GetResource("DataTierGenerator.Resources.AssemblyInfo.txt");
            assemblyInfo.Replace("#AssemblyTitle", assemblyTitle);
            assemblyInfo.Replace("#DatabaseName", databaseName);

            string propertiesDirectory = Path.Combine(path, "Properties");
            if (Directory.Exists(propertiesDirectory) == false)
            {
                Directory.CreateDirectory(propertiesDirectory);
            }

            File.WriteAllText(Path.Combine(propertiesDirectory, "AssemblyInfo.cs"), assemblyInfo);
        }

        /// <summary>
        /// Creates the Service1.cs file for the project.
        /// </summary>
        /// <param name="path">The root path of the project.</param>
        /// <param name="targetNamespace">The namespace that the generated C# classes should contained in.</param>
        internal static void CreateService(string path, string targetNamespace)
        {
            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(path, "Services\\Service1.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine("using System;");
                streamWriter.WriteLine("using System.Collections.Generic;");
                streamWriter.WriteLine("using System.Transactions;");
                streamWriter.WriteLine();
                streamWriter.WriteLine("using SharpCore.Objects;");
                streamWriter.WriteLine("using SharpCore.Utilities;");
                streamWriter.WriteLine();
                streamWriter.WriteLine("namespace " + targetNamespace);
                streamWriter.WriteLine("{");

                streamWriter.WriteLine("\tpublic class Service1");
                streamWriter.WriteLine("\t{");

                // Create the members
                streamWriter.WriteLine("\t\t#region Fields");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\tprotected ServiceUtility serviceUtility;");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t#endregion");
                streamWriter.WriteLine();

                // Create the constructors
                streamWriter.WriteLine("\t\t#region Constructors");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\tpublic Service1(ServiceUtility serviceUtility)");
                streamWriter.WriteLine("\t\t{");
                streamWriter.WriteLine("\t\t\tthis.serviceUtility = serviceUtility;");
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t#endregion");
                streamWriter.WriteLine();

                // Create the methods
                streamWriter.WriteLine("\t\t#region Methods");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t//TODO: Create service methods here");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t#endregion");
                streamWriter.WriteLine();

                // Create the helpers
                streamWriter.WriteLine("\t\t#region Helpers");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t// TODO: Create helper methods here for XxxData classes");
                streamWriter.WriteLine("\t\t/*");
                streamWriter.WriteLine("\t\tprotected DataClass dataClass;");
                streamWriter.WriteLine("\t\tpublic virtual DataClass GetDataClass()");
                streamWriter.WriteLine("\t\t{");
                streamWriter.WriteLine("\t\t\tif (dataClass == null)");
                streamWriter.WriteLine("\t\t\t{");
                streamWriter.WriteLine("\t\t\t\tdataClass = (DataClass) ObjectFactory.GetObject(\"DataClass\");");
                streamWriter.WriteLine("\t\t\t}");
                streamWriter.WriteLine("\t\t\t");
                streamWriter.WriteLine("\t\t\treturn dataClass;");
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine("\t\t*/");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t#endregion");

                // Close out the class and namespace
                streamWriter.WriteLine("\t}");
                streamWriter.WriteLine("}");
            }
        }

        /// <summary>
        /// Creates the ServiceUtility.cs file for the project.
        /// </summary>
        /// <param name="path">The root path of the project.</param>
        /// <param name="targetNamespace">The namespace that the generated C# classes should contained in.</param>
        internal static void CreateServiceUtility(string path, string targetNamespace)
        {
            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(path, "Services\\ServiceUtility.cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine("using System;");
                streamWriter.WriteLine();
                streamWriter.WriteLine("using SharpCore.Objects;");
                streamWriter.WriteLine();
                streamWriter.WriteLine("using " + targetNamespace + ";");
                streamWriter.WriteLine("using " + targetNamespace + ".Data;");
                streamWriter.WriteLine();
                streamWriter.WriteLine("namespace " + targetNamespace);
                streamWriter.WriteLine("{");

                streamWriter.WriteLine("\tpublic class ServiceUtility");
                streamWriter.WriteLine("\t{");

                // Create the methods
                streamWriter.WriteLine("\t\t#region Methods");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t// TODO: Create GetXxxService methods here");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t/*");
                streamWriter.WriteLine("\t\tprotected ServiceClass serviceClass;");
                streamWriter.WriteLine("\t\tpublic virtual ServiceClass GetServiceClass()");
                streamWriter.WriteLine("\t\t{");
                streamWriter.WriteLine("\t\t\tif (serviceClass == null)");
                streamWriter.WriteLine("\t\t\t{");
                streamWriter.WriteLine("\t\t\t\tserviceClass = (ServiceClass) ObjectFactory.GetObject(\"ServiceClass\");");
                streamWriter.WriteLine("\t\t\t}");
                streamWriter.WriteLine("\t\t\t");
                streamWriter.WriteLine("\t\t\treturn serviceClass;");
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine("\t\t*/");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t#endregion");

                // Close out the class and namespace
                streamWriter.WriteLine("\t}");
                streamWriter.WriteLine("}");
            }
        }

        /// <summary>
        /// Creates the SharpCore DLLs required by the generated code.
        /// </summary>
        /// <param name="path">The root path of the project</param>
        internal static void CreateSharpCore(string path)
        {
            string sharpCoreDirectory = Path.Combine(path, "SharpCore");
            if (Directory.Exists(sharpCoreDirectory) == false)
            {
                Directory.CreateDirectory(sharpCoreDirectory);
            }

            Utility.WriteResourceToFile("DataTierGenerator.Resources.SharpCore.SharpCore.Collections.dll", Path.Combine(sharpCoreDirectory, "SharpCore.Collections.dll"));
            Utility.WriteResourceToFile("DataTierGenerator.Resources.SharpCore.SharpCore.Collections.pdb", Path.Combine(sharpCoreDirectory, "SharpCore.Collections.pdb"));
            Utility.WriteResourceToFile("DataTierGenerator.Resources.SharpCore.SharpCore.Data.dll", Path.Combine(sharpCoreDirectory, "SharpCore.Data.dll"));
            Utility.WriteResourceToFile("DataTierGenerator.Resources.SharpCore.SharpCore.Data.pdb", Path.Combine(sharpCoreDirectory, "SharpCore.Data.pdb"));
            Utility.WriteResourceToFile("DataTierGenerator.Resources.SharpCore.SharpCore.Objects.dll", Path.Combine(sharpCoreDirectory, "SharpCore.Objects.dll"));
            Utility.WriteResourceToFile("DataTierGenerator.Resources.SharpCore.SharpCore.Objects.pdb", Path.Combine(sharpCoreDirectory, "SharpCore.Objects.pdb"));
            Utility.WriteResourceToFile("DataTierGenerator.Resources.SharpCore.SharpCore.Utilities.dll", Path.Combine(sharpCoreDirectory, "SharpCore.Utilities.dll"));
            Utility.WriteResourceToFile("DataTierGenerator.Resources.SharpCore.SharpCore.Utilities.pdb", Path.Combine(sharpCoreDirectory, "SharpCore.Utilities.pdb"));
        }

        public static void AddFilesToUnitTestProject(List<string> files, string _unitTestProjectFile, string measureDataDirSuffix)
        {
            var unitTestProjectPath = _unitTestProjectFile;
            var unitTestProjectFile = XDocument.Load(unitTestProjectPath);
            var itemGroup = unitTestProjectFile.Nodes()
                              .OfType<XElement>()
                              .DescendantNodes()
                              .OfType<XElement>().First(xy => xy.Name.LocalName == "ItemGroup");

            itemGroup.RemoveAll();
            itemGroup.Add(AddProjectReference("System", unitTestProjectFile));
            itemGroup.Add(AddProjectReference("System.Data", unitTestProjectFile));
            itemGroup.Add(AddProjectReference("System.Drawing", unitTestProjectFile));
            itemGroup.Add(AddProjectReference("System.Runtime.Serialization", unitTestProjectFile));
            itemGroup.Add(AddProjectReference("System.Transactions", unitTestProjectFile));
            itemGroup.Add(AddProjectReference("System.Windows.Forms", unitTestProjectFile));
            itemGroup.Add(AddProjectReference("System.Xml", unitTestProjectFile));
            foreach (var fileInfo in files)
            {
                var xelem = AddProjectContent(measureDataDirSuffix + fileInfo, unitTestProjectFile);
                itemGroup.Add(xelem);
            }
            unitTestProjectFile.Save(unitTestProjectPath);
        }
        private static XElement AddProjectContent(string pathToAdd, XDocument doc)
        {
            XNamespace rootNamespace = doc.Root.Name.NamespaceName;
            var xelem = new XElement(rootNamespace + "Compile");
            xelem.Add(new XAttribute("Include", pathToAdd));
            // xelem.Add(new XElement(rootNamespace + "CopyToOutputDirectory", "Always"));
            return xelem;
        }
        private static XElement AddProjectReference(string pathToAdd, XDocument doc)
        {
            XNamespace rootNamespace = doc.Root.Name.NamespaceName;
            var xelem = new XElement(rootNamespace + "Reference");
            xelem.Add(new XAttribute("Include", pathToAdd));
            // xelem.Add(new XElement(rootNamespace + "CopyToOutputDirectory", "Always"));
            return xelem;
        }
        /// <summary>
        /// Creates a C# class for all of the table's stored procedures.
        /// </summary>
        /// <param name="table">Instance of the Table class that represents the table this class will be created for.</param>
        /// <param name="targetNamespace">The namespace that the generated C# classes should contained in.</param>
        /// <param name="storedProcedurePrefix">Prefix to be appended to the name of the stored procedure.</param>
        /// <param name="path">Path where the class should be created.</param>
        internal static void CreateDataTransferClass(Table table, string targetNamespace, string storedProcedurePrefix, string path,string EntityPath)
        {
            string className = table.Name;

            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(path, className + ".cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine("using System;");
                streamWriter.WriteLine("using System.Collections;");
                streamWriter.WriteLine("using System.Collections.Generic;");
                streamWriter.WriteLine("using System.Text;");

                streamWriter.WriteLine();
                streamWriter.WriteLine("namespace " + targetNamespace);
                streamWriter.WriteLine("{");

                streamWriter.WriteLine("\tpublic class " + className);
                streamWriter.WriteLine("\t{");

                // Append the private members
                streamWriter.WriteLine("\t\t#region Fields");
                streamWriter.WriteLine();
                foreach (Column column in table.Columns)
                {
                    if (column.Type.ToLower() != "hierarchyid")
                        streamWriter.WriteLine("\t\tprivate " + Utility.CreateMethodParameterwithDefault(column) + ";");
                }

                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t#endregion");
                streamWriter.WriteLine();

                Dictionary<string, string> lstProperties = new Dictionary<string, string>();
                // Append the public properties
                streamWriter.WriteLine("\t\t#region Properties");
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    Column column = table.Columns[i];
                    if (!lstProperties.ContainsKey(column.Name))
                        lstProperties.Add(column.Name.Trim(), column.Type.Trim());

                    if (column.Type.ToLower() != "hierarchyid")
                    {
                        string parameter = Utility.CreateMethodParameter(column);
                        string type = parameter.Split(' ')[0];
                        string propertyname = parameter.Split(' ')[1];
                        string name = string.Empty;
                        if (propertyname.ToLower().Equals("value"))
                            name = "valu";
                        else
                            name = propertyname;

                        streamWriter.WriteLine("\t\t/// <summary>");
                        streamWriter.WriteLine("\t\t/// Gets or sets the " + Utility.FormatPascal(name) + " value.");
                        streamWriter.WriteLine("\t\t/// </summary>");
                        streamWriter.WriteLine("\t\tpublic " + type + " " + Utility.FormatPascal(propertyname));
                        streamWriter.WriteLine("\t\t{");
                        streamWriter.WriteLine("\t\t\tget { return " + name + "; }");
                        streamWriter.WriteLine("\t\t\tset { " + name + " = value; }");
                        streamWriter.WriteLine("\t\t}");

                        if (i < (table.Columns.Count - 1))
                        {
                            streamWriter.WriteLine();
                        }
                    }
                }
                streamWriter.WriteLine();
                //Added by Ramki
                //Create any additional entity object methods if spedified in XML
                XmlDocument doc = new XmlDocument();
                doc.Load(EntityPath);
                XmlNode rnode = doc.SelectSingleNode("Tables/" + className);
                string strUseGenerics = "";
                string strPropType = "";
                string strPropName = "";
                string StrXmlMultiple = "";
                if (rnode != null)
                {
                    if (rnode.HasChildNodes)
                    {
                        foreach (XmlNode cnode in rnode.ChildNodes)
                        {
                            strPropName = cnode.Attributes["Name"].InnerText;
                            strPropType = cnode.Attributes["Type"].InnerText;
                            strPropName = strPropName.Trim();
                            strPropType = strPropType.Trim();
                            StrXmlMultiple = cnode.Attributes["Multiple"].InnerText;
                            strUseGenerics = cnode.Attributes["UseGenerics"].Value;
                            if (!lstProperties.ContainsKey(strPropName))
                            {
                                lstProperties.Add(strPropName, strPropType);
                                streamWriter.WriteLine("\t\t/// <summary>");
                                streamWriter.WriteLine("\t\t/// Gets or sets the " + Utility.FormatPascal(strPropName) + " value.");
                                streamWriter.WriteLine("\t\t/// </summary>");

                                if (StrXmlMultiple == "True")
                                {
                                    if (strUseGenerics == "True")
                                    {
                                        streamWriter.WriteLine("\t\tList<" + strPropType + ">" + " " + "_" + strPropName.ToLower() + ";");
                                        streamWriter.WriteLine("\t\tpublic List<" + strPropType + ">" + " " + Utility.FormatPascal(strPropName));
                                        streamWriter.WriteLine("\t\t{");
                                        streamWriter.WriteLine("\t\t\tget { return " + "_" + strPropName.ToLower() + "; }");
                                        streamWriter.WriteLine("\t\t\tset { " + "_" + strPropName.ToLower() + " = value; }");
                                        streamWriter.WriteLine("\t\t}");
                                    }
                                    else
                                    {
                                        streamWriter.WriteLine("\t\t" + strPropType + "[] " + "_" + strPropName.ToLower() + ";");
                                        streamWriter.WriteLine("\t\tpublic " + strPropType + "[] " + Utility.FormatPascal(strPropName));
                                        streamWriter.WriteLine("\t\t{");
                                        streamWriter.WriteLine("\t\t\tget { return " + "_" + strPropName.ToLower() + "; }");
                                        streamWriter.WriteLine("\t\t\tset { " + "_" + strPropName.ToLower() + " = value; }");
                                        streamWriter.WriteLine("\t\t}");
                                    }
                                }
                                else
                                {
                                    //streamWriter.WriteLine("\t\t" + strPropType +  " _" + strPropName.ToLower() + ";");
                                    streamWriter.WriteLine("\t\tprivate " + Utility.CreateMethodParameterwithDefault(strPropName, strPropType) + ";");
                                    streamWriter.WriteLine("\t\tpublic " + strPropType + " " + Utility.FormatPascal(strPropName));
                                    streamWriter.WriteLine("\t\t{");
                                    streamWriter.WriteLine("\t\t\tget { return " + Utility.FormatCamel(strPropName) + "; }");
                                    streamWriter.WriteLine("\t\t\tset { " + Utility.FormatCamel(strPropName) + " = value; }");
                                    streamWriter.WriteLine("\t\t}");
                                }
                                streamWriter.WriteLine();
                            }
                        }
                    }
                }
                //Ramki addition ends here

                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t#endregion");

                // Close out the class and namespace
                streamWriter.WriteLine("}");
                streamWriter.WriteLine("}");
            }
        }

        /// <summary>
        /// Creates a C# data access class for all of the table's stored procedures.
        /// </summary>
        /// <param name="databaseName">The name of the database.</param>
        /// <param name="table">Instance of the Table class that represents the table this class will be created for.</param>
        /// <param name="targetNamespace">The namespace that the generated C# classes should contained in.</param>
        /// <param name="storedProcedurePrefix">Prefix to be appended to the name of the stored procedure.</param>
        /// <param name="daoSuffix">The suffix to be appended to the data access class.</param>
        /// <param name="path">Path where the class should be created.</param>
        internal static void CreateDataAccessClass(string databaseName, Table table, string targetNamespace, string storedProcedurePrefix, string daoSuffix, string path)
        {
            string className = Utility.FormatClassName(table.Name) + daoSuffix;
            path = Path.Combine(path, "Data");

            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(path, className + ".cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine("using System;");
                streamWriter.WriteLine("using System.Collections.Generic;");
                streamWriter.WriteLine("using System.Data;");
                streamWriter.WriteLine("using System.Data.SqlClient;");
                streamWriter.WriteLine();
                streamWriter.WriteLine("using SharpCore.Data;");
                streamWriter.WriteLine("using SharpCore.Utilities;");
                streamWriter.WriteLine();
                streamWriter.WriteLine("namespace " + targetNamespace + "." + "DAL");
                streamWriter.WriteLine("{");

                streamWriter.WriteLine("\tpublic class " + className);
                streamWriter.WriteLine("\t{");

                // Append the fields
                streamWriter.WriteLine("\t\t#region Fields");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\tprotected string connectionStringName;");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t#endregion");
                streamWriter.WriteLine();

                //// Append the constructors
                //streamWriter.WriteLine("\t\t#region Constructors");
                //streamWriter.WriteLine();
                //streamWriter.WriteLine("\t\tpublic " + className + "(string connectionStringName)");
                //streamWriter.WriteLine("\t\t{");
                //streamWriter.WriteLine("\t\t\tValidationUtility.ValidateArgument(\"connectionStringName\", connectionStringName);");
                //streamWriter.WriteLine();
                //streamWriter.WriteLine("\t\t\tthis.connectionStringName = connectionStringName;");
                //streamWriter.WriteLine("\t\t}");
                //streamWriter.WriteLine();
                //streamWriter.WriteLine("\t\t#endregion");
                //streamWriter.WriteLine();

                // Append the access methods
                streamWriter.WriteLine("\t\t#region Methods");
                streamWriter.WriteLine();

                CreateInsertMethod(table, storedProcedurePrefix, streamWriter);
                CreateUpdateMethod(table, storedProcedurePrefix, streamWriter);
                CreateDeleteMethod(table, storedProcedurePrefix, streamWriter);
                CreateDeleteAllByMethods(table, storedProcedurePrefix, streamWriter);
                CreateSelectMethod(table, storedProcedurePrefix, streamWriter);
                CreateSelectAllMethod(table, storedProcedurePrefix, streamWriter);
                CreateSelectAllByMethods(table, storedProcedurePrefix, streamWriter);
                CreateMakeMethod(table, streamWriter);

                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t#endregion");

                // Close out the class and namespace
                streamWriter.WriteLine("\t}");
                streamWriter.WriteLine("}");
            }
        }

        /// <summary>
        /// Creates a mock C# data access class for all of the table's stored procedures.
        /// </summary>
        /// <param name="table">Instance of the Table class that represents the table this class will be created for.</param>
        /// <param name="targetNamespace">The namespace that the generated C# classes should contained in.</param>
        /// <param name="storedProcedurePrefix">Prefix to be appended to the name of the stored procedure.</param>
        /// <param name="daoSuffix">The suffix to be appended to the name of the data access class.</param>
        /// <param name="path">Path where the class should be created.</param>
        internal static void CreateMockClass(Table table, string targetNamespace, string storedProcedurePrefix, string daoSuffix, string path)
        {
            string className = Utility.FormatClassName(table.Name) + daoSuffix + "Mock";
            path = Path.Combine(path, "Data\\Mocks");

            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(path, className + ".cs")))
            {
                // Create the header for the class
                streamWriter.WriteLine("using System;");
                streamWriter.WriteLine("using System.Collections.Generic;");
                streamWriter.WriteLine("using System.Data;");
                streamWriter.WriteLine("using System.Data.SqlClient;");
                streamWriter.WriteLine();
                streamWriter.WriteLine("using SharpCore.Utilities;");
                streamWriter.WriteLine();
                streamWriter.WriteLine("namespace " + targetNamespace + ".Mocks");
                streamWriter.WriteLine("{");

                streamWriter.WriteLine("\tpublic class " + className + " : " + Utility.FormatClassName(table.Name) + daoSuffix);
                streamWriter.WriteLine("\t{");

                // Append the constructors
                streamWriter.WriteLine("\t\t#region Constructors");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\tpublic " + className + "()");
                //streamWriter.WriteLine("\t\t\\t: base(String.Empty)");
                streamWriter.WriteLine("\t\t{");
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t#endregion");
                streamWriter.WriteLine();

                // Append the access methods
                streamWriter.WriteLine("\t\t#region Methods");
                streamWriter.WriteLine();

                CreateMockInsertMethod(table, storedProcedurePrefix, streamWriter);
                CreateMockUpdateMethod(table, storedProcedurePrefix, streamWriter);
                CreateMockDeleteMethod(table, storedProcedurePrefix, streamWriter);
                CreateMockDeleteAllByMethods(table, storedProcedurePrefix, streamWriter);
                CreateMockSelectMethod(table, storedProcedurePrefix, streamWriter);
                CreateMockSelectAllMethod(table, storedProcedurePrefix, streamWriter);
                CreateMockSelectAllByMethods(table, storedProcedurePrefix, streamWriter);
                CreateMockMakeMethod(table, streamWriter);

                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t#endregion");

                // Close out the class and namespace
                streamWriter.WriteLine("\t}");
                streamWriter.WriteLine("}");
            }
        }

        /// <summary>
        /// Creates a string that represents the insert functionality of the data access class.
        /// </summary>
        /// <param name="table">The Table instance that this method will be created for.</param>
        /// <param name="storedProcedurePrefix">The prefix that is used on the stored procedure that this method will call.</param>
        /// <param name="streamWriter">The StreamWriter instance that will be used to create the method.</param>
        private static void CreateInsertMethod(Table table, string storedProcedurePrefix, StreamWriter streamWriter)
        {
            string className = Utility.FormatClassName(table.Name);
            string variableName = Utility.FormatCamel(className);

            // Append the method header
            streamWriter.WriteLine("\t\t/// <summary>");
            streamWriter.WriteLine("\t\t/// Saves a record to the " + table.Name + " table.");
            streamWriter.WriteLine("\t\t/// </summary>");
            streamWriter.WriteLine("\t\tpublic virtual void Insert(" + className + " " + variableName + ")");
            streamWriter.WriteLine("\t\t{");

            // Append validation for the parameter
            streamWriter.WriteLine("\t\t\tValidationUtility.ValidateArgument(\"" + variableName + "\", " + variableName + ");");
            streamWriter.WriteLine();

            // Append the parameter declarations
            streamWriter.WriteLine("\t\t\tSqlParameter[] parameters = new SqlParameter[]");
            streamWriter.WriteLine("\t\t\t{");
            for (int i = 0; i < table.Columns.Count; i++)
            {
                Column column = table.Columns[i];
                if (column.IsIdentity == false && column.IsRowGuidCol == false)
                {
                    streamWriter.Write("\t\t\t\t" + Utility.CreateSqlParameter(table, column));
                    if (i < (table.Columns.Count - 1))
                    {
                        streamWriter.Write(",");
                    }

                    streamWriter.WriteLine();
                }
            }

            streamWriter.WriteLine("\t\t\t};");
            streamWriter.WriteLine();

            bool hasReturnValue = false;
            foreach (Column column in table.Columns)
            {
                if (column.IsIdentity || column.IsRowGuidCol)
                {
                    if (column.IsIdentity && Convert.ToInt32(column.Length) == 4)
                    {
                        streamWriter.WriteLine("\t\t\t" + variableName + "." + Utility.FormatPascal(column.Name) + " = (int) SqlClientUtility.ExecuteScalar(connectionStringName, CommandType.StoredProcedure, \"" + table.Name + "Insert\", parameters);");
                        hasReturnValue = true;
                    }
                    else if (column.IsIdentity && Convert.ToInt32(column.Length) == 8)
                    {
                        streamWriter.WriteLine("\t\t\t" + variableName + "." + Utility.FormatPascal(column.Name) + " = (long) SqlClientUtility.ExecuteScalar(connectionStringName, CommandType.StoredProcedure, \"" + table.Name + "Insert\", parameters);");
                        hasReturnValue = true;
                    }
                    else if (column.IsRowGuidCol)
                    {
                        streamWriter.WriteLine("\t\t\t" + variableName + "." + Utility.FormatPascal(column.Name) + " = (Guid) SqlClientUtility.ExecuteScalar(connectionStringName, CommandType.StoredProcedure, \"" + table.Name + "Insert\", parameters);");
                        hasReturnValue = true;
                    }
                }
            }

            if (hasReturnValue == false)
            {
                streamWriter.WriteLine("\t\t\tSqlClientUtility.ExecuteNonQuery(connectionStringName, CommandType.StoredProcedure, \"" + table.Name + "Insert\", parameters);");
            }

            // Append the method footer
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine();
        }

        /// <summary>
        /// Creates a string that represents the update functionality of the data access class.
        /// </summary>
        /// <param name="table">The Table instance that this method will be created for.</param>
        /// <param name="storedProcedurePrefix">The prefix that is used on the stored procedure that this method will call.</param>
        /// <param name="streamWriter">The StreamWriter instance that will be used to create the method.</param>
        private static void CreateUpdateMethod(Table table, string storedProcedurePrefix, StreamWriter streamWriter)
        {
            if (table.PrimaryKeys.Count > 0 && table.Columns.Count != table.PrimaryKeys.Count && table.Columns.Count != table.ForeignKeys.Count)
            {
                string className = Utility.FormatClassName(table.Name);
                string variableName = Utility.FormatCamel(className);

                // Append the method header
                streamWriter.WriteLine("\t\t/// <summary>");
                streamWriter.WriteLine("\t\t/// Updates a record in the " + table.Name + " table.");
                streamWriter.WriteLine("\t\t/// </summary>");
                streamWriter.WriteLine("\t\tpublic virtual void Update(" + className + " " + variableName + ")");
                streamWriter.WriteLine("\t\t{");

                // Append validation for the parameter
                streamWriter.WriteLine("\t\t\tValidationUtility.ValidateArgument(\"" + variableName + "\", " + variableName + ");");
                streamWriter.WriteLine();

                // Append the parameter declarations
                streamWriter.WriteLine("\t\t\tSqlParameter[] parameters = new SqlParameter[]");
                streamWriter.WriteLine("\t\t\t{");
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    Column column = table.Columns[i];
                    streamWriter.Write("\t\t\t\t" + Utility.CreateSqlParameter(table, column));
                    if (i < (table.Columns.Count - 1))
                    {
                        streamWriter.Write(",");
                    }

                    streamWriter.WriteLine();
                }

                streamWriter.WriteLine("\t\t\t};");
                streamWriter.WriteLine();

                streamWriter.WriteLine("\t\t\tSqlClientUtility.ExecuteNonQuery(connectionStringName, CommandType.StoredProcedure, \"" + table.Name + "Update\", parameters);");

                // Append the method footer
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine();
            }
        }

        /// <summary>
        /// Creates a string that represents the delete functionality of the data access class.
        /// </summary>
        /// <param name="table">The Table instance that this method will be created for.</param>
        /// <param name="storedProcedurePrefix">The prefix that is used on the stored procedure that this method will call.</param>
        /// <param name="streamWriter">The StreamWriter instance that will be used to create the method.</param>
        private static void CreateDeleteMethod(Table table, string storedProcedurePrefix, StreamWriter streamWriter)
        {
            if (table.PrimaryKeys.Count > 0)
            {
                // Append the method header
                streamWriter.WriteLine("\t\t/// <summary>");
                streamWriter.WriteLine("\t\t/// Deletes a record from the " + table.Name + " table by its primary key.");
                streamWriter.WriteLine("\t\t/// </summary>");
                streamWriter.Write("\t\tpublic virtual void Delete(");
                for (int i = 0; i < table.PrimaryKeys.Count; i++)
                {
                    Column column = table.PrimaryKeys[i];
                    streamWriter.Write(Utility.CreateMethodParameter(column));
                    if (i < (table.PrimaryKeys.Count - 1))
                    {
                        streamWriter.Write(", ");
                    }
                }
                streamWriter.WriteLine(")");
                streamWriter.WriteLine("\t\t{");

                // Append the parameter declarations
                streamWriter.WriteLine("\t\t\tSqlParameter[] parameters = new SqlParameter[]");
                streamWriter.WriteLine("\t\t\t{");
                for (int i = 0; i < table.PrimaryKeys.Count; i++)
                {
                    Column column = table.Columns[i];
                    streamWriter.Write("\t\t\t\tnew SqlParameter(\"@" + column.Name + "\", " + Utility.FormatCamel(column.Name) + ")");
                    if (i < (table.PrimaryKeys.Count - 1))
                    {
                        streamWriter.Write(",");
                    }

                    streamWriter.WriteLine();
                }

                streamWriter.WriteLine("\t\t\t};");
                streamWriter.WriteLine();

                // Append the stored procedure execution
                streamWriter.WriteLine("\t\t\tSqlClientUtility.ExecuteNonQuery(connectionStringName, CommandType.StoredProcedure, \"" + table.Name + "Delete\", parameters);");

                // Append the method footer
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine();
            }
        }

        /// <summary>
        /// Creates a string that represents the "delete by" functionality of the data access class.
        /// </summary>
        /// <param name="table">The Table instance that this method will be created for.</param>
        /// <param name="storedProcedurePrefix">The prefix that is used on the stored procedure that this method will call.</param>
        /// <param name="streamWriter">The StreamWriter instance that will be used to create the method.</param>
        private static void CreateDeleteAllByMethods(Table table, string storedProcedurePrefix, StreamWriter streamWriter)
        {
            // Create a stored procedure for each foreign key
            foreach (List<Column> compositeKeyList in table.ForeignKeys.Values)
            {
                // Create the stored procedure name
                StringBuilder stringBuilder = new StringBuilder(255);
                stringBuilder.Append("DeleteAllBy");
                for (int i = 0; i < compositeKeyList.Count; i++)
                {
                    Column column = compositeKeyList[i];

                    if (i > 0)
                    {
                        stringBuilder.Append("_" + Utility.FormatPascal(column.Name));
                    }
                    else
                    {
                        stringBuilder.Append(Utility.FormatPascal(column.Name));
                    }
                }
                string methodName = stringBuilder.ToString();
                string procedureName = storedProcedurePrefix + table.Name + methodName;

                // Create the delete function based on keys
                // Append the method header
                streamWriter.WriteLine("\t\t/// <summary>");
                streamWriter.WriteLine("\t\t/// Deletes a record from the " + table.Name + " table by a foreign key.");
                streamWriter.WriteLine("\t\t/// </summary>");

                streamWriter.Write("\t\tpublic virtual void " + methodName + "(");
                for (int i = 0; i < compositeKeyList.Count; i++)
                {
                    Column column = compositeKeyList[i];
                    streamWriter.Write(Utility.CreateMethodParameter(column));
                    if (i < (compositeKeyList.Count - 1))
                    {
                        streamWriter.Write(", ");
                    }
                }
                streamWriter.WriteLine(")");
                streamWriter.WriteLine("\t\t{");

                // Append the parameter declarations
                streamWriter.WriteLine("\t\t\tSqlParameter[] parameters = new SqlParameter[]");
                streamWriter.WriteLine("\t\t\t{");
                for (int i = 0; i < compositeKeyList.Count; i++)
                {
                    Column column = compositeKeyList[i];
                    streamWriter.Write("\t\t\t\tnew SqlParameter(\"@" + column.Name + "\", " + Utility.FormatCamel(column.Name) + ")");
                    if (i < (compositeKeyList.Count - 1))
                    {
                        streamWriter.Write(",");
                    }

                    streamWriter.WriteLine();
                }

                streamWriter.WriteLine("\t\t\t};");
                streamWriter.WriteLine();

                // Append the stored procedure execution
                streamWriter.WriteLine("\t\t\tSqlClientUtility.ExecuteNonQuery(connectionStringName, CommandType.StoredProcedure, \"" + procedureName + "\", parameters);");

                // Append the method footer
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine();
            }
        }

        /// <summary>
        /// Creates a string that represents the select by primary key functionality of the data access class.
        /// </summary>
        /// <param name="table">The Table instance that this method will be created for.</param>
        /// <param name="storedProcedurePrefix">The prefix that is used on the stored procedure that this method will call.</param>
        /// <param name="streamWriter">The StreamWriter instance that will be used to create the method.</param>
        private static void CreateSelectMethod(Table table, string storedProcedurePrefix, StreamWriter streamWriter)
        {
            if (table.PrimaryKeys.Count > 0 && table.Columns.Count != table.ForeignKeys.Count)
            {
                string className = Utility.FormatClassName(table.Name);

                // Append the method header
                streamWriter.WriteLine("\t\t/// <summary>");
                streamWriter.WriteLine("\t\t/// Selects a single record from the " + table.Name + " table.");
                streamWriter.WriteLine("\t\t/// </summary>");

                streamWriter.Write("\t\tpublic virtual " + className + " Select(");
                for (int i = 0; i < table.PrimaryKeys.Count; i++)
                {
                    Column column = table.PrimaryKeys[i];
                    streamWriter.Write(Utility.CreateMethodParameter(column));
                    if (i < (table.PrimaryKeys.Count - 1))
                    {
                        streamWriter.Write(", ");
                    }
                }
                streamWriter.WriteLine(")");
                streamWriter.WriteLine("\t\t{");

                // Append the parameter declarations
                streamWriter.WriteLine("\t\t\tSqlParameter[] parameters = new SqlParameter[]");
                streamWriter.WriteLine("\t\t\t{");
                for (int i = 0; i < table.PrimaryKeys.Count; i++)
                {
                    Column column = table.Columns[i];
                    streamWriter.Write("\t\t\t\tnew SqlParameter(\"@" + column.Name + "\", " + Utility.FormatCamel(column.Name) + ")");
                    if (i < (table.PrimaryKeys.Count - 1))
                    {
                        streamWriter.Write(",");
                    }

                    streamWriter.WriteLine();
                }

                streamWriter.WriteLine("\t\t\t};");
                streamWriter.WriteLine();

                // Append the stored procedure execution
                streamWriter.WriteLine("\t\t\tusing (SqlDataReader dataReader = SqlClientUtility.ExecuteReader(connectionStringName, CommandType.StoredProcedure, \"" + table.Name + "Select\", parameters))");
                streamWriter.WriteLine("\t\t\t{");
                streamWriter.WriteLine("\t\t\t\tif (dataReader.Read())");
                streamWriter.WriteLine("\t\t\t\t{");
                streamWriter.WriteLine("\t\t\t\t\treturn Make" + className + "(dataReader);");
                streamWriter.WriteLine("\t\t\t\t}");
                streamWriter.WriteLine("\t\t\t\telse");
                streamWriter.WriteLine("\t\t\t\t{");
                streamWriter.WriteLine("\t\t\t\t\treturn null;");
                streamWriter.WriteLine("\t\t\t\t}");
                streamWriter.WriteLine("\t\t\t}");

                // Append the method footer
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine();
            }
        }

        /// <summary>
        /// Creates a string that represents the select functionality of the data access class.
        /// </summary>
        /// <param name="table">The Table instance that this method will be created for.</param>
        /// <param name="storedProcedurePrefix">The prefix that is used on the stored procedure that this method will call.</param>
        /// <param name="streamWriter">The StreamWriter instance that will be used to create the method.</param>
        private static void CreateSelectAllMethod(Table table, string storedProcedurePrefix, StreamWriter streamWriter)
        {
            if (table.Columns.Count != table.PrimaryKeys.Count && table.Columns.Count != table.ForeignKeys.Count)
            {
                string className = Utility.FormatClassName(table.Name);
                string dtoVariableName = Utility.FormatCamel(className);

                // Append the method header
                streamWriter.WriteLine("\t\t/// <summary>");
                streamWriter.WriteLine("\t\t/// Selects all records from the " + table.Name + " table.");
                streamWriter.WriteLine("\t\t/// </summary>");
                streamWriter.WriteLine("\t\tpublic virtual List<" + className + "> SelectAll()");
                streamWriter.WriteLine("\t\t{");

                // Append the stored procedure execution
                streamWriter.WriteLine("\t\t\tusing (SqlDataReader dataReader = SqlClientUtility.ExecuteReader(connectionStringName, CommandType.StoredProcedure, \"" + table.Name + "SelectAll\"))");
                streamWriter.WriteLine("\t\t\t{");
                streamWriter.WriteLine("\t\t\t\tList<" + className + "> " + dtoVariableName + "List = new List<" + className + ">();");
                streamWriter.WriteLine("\t\t\t\twhile (dataReader.Read())");
                streamWriter.WriteLine("\t\t\t\t{");
                streamWriter.WriteLine("\t\t\t\t\t" + className + " " + dtoVariableName + " = Make" + className + "(dataReader);");
                streamWriter.WriteLine("\t\t\t\t\t" + dtoVariableName + "List.Add(" + dtoVariableName + ");");
                streamWriter.WriteLine("\t\t\t\t}");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t\t\treturn " + dtoVariableName + "List;");
                streamWriter.WriteLine("\t\t\t}");

                // Append the method footer
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine();
            }
        }

        /// <summary>
        /// Creates a string that represents the "select by" functionality of the data access class.
        /// </summary>
        /// <param name="table">The Table instance that this method will be created for.</param>
        /// <param name="storedProcedurePrefix">The prefix that is used on the stored procedure that this method will call.</param>
        /// <param name="streamWriter">The StreamWriter instance that will be used to create the method.</param>
        private static void CreateSelectAllByMethods(Table table, string storedProcedurePrefix, StreamWriter streamWriter)
        {
            string className = Utility.FormatClassName(table.Name);
            string dtoVariableName = Utility.FormatCamel(className);

            // Create a stored procedure for each foreign key
            foreach (List<Column> compositeKeyList in table.ForeignKeys.Values)
            {
                // Create the stored procedure name
                StringBuilder stringBuilder = new StringBuilder(255);
                stringBuilder.Append("SelectAllBy");
                for (int i = 0; i < compositeKeyList.Count; i++)
                {
                    Column column = compositeKeyList[i];

                    if (i > 0)
                    {
                        stringBuilder.Append("_" + Utility.FormatPascal(column.Name));
                    }
                    else
                    {
                        stringBuilder.Append(Utility.FormatPascal(column.Name));
                    }
                }
                string methodName = stringBuilder.ToString();
                string procedureName = storedProcedurePrefix + table.Name + methodName;

                // Create the select function based on keys
                // Append the method header
                streamWriter.WriteLine("\t\t/// <summary>");
                streamWriter.WriteLine("\t\t/// Selects all records from the " + table.Name + " table by a foreign key.");
                streamWriter.WriteLine("\t\t/// </summary>");

                streamWriter.Write("\t\tpublic virtual List<" + className + "> " + methodName + "(");
                for (int i = 0; i < compositeKeyList.Count; i++)
                {
                    Column column = compositeKeyList[i];
                    streamWriter.Write(Utility.CreateMethodParameter(column));
                    if (i < (compositeKeyList.Count - 1))
                    {
                        streamWriter.Write(", ");
                    }
                }
                streamWriter.WriteLine(")");
                streamWriter.WriteLine("\t\t{");

                // Append the parameter declarations
                streamWriter.WriteLine("\t\t\tSqlParameter[] parameters = new SqlParameter[]");
                streamWriter.WriteLine("\t\t\t{");
                for (int i = 0; i < compositeKeyList.Count; i++)
                {
                    Column column = compositeKeyList[i];
                    streamWriter.Write("\t\t\t\tnew SqlParameter(\"@" + column.Name + "\", " + Utility.FormatCamel(column.Name) + ")");
                    if (i < (compositeKeyList.Count - 1))
                    {
                        streamWriter.Write(",");
                    }

                    streamWriter.WriteLine();
                }

                streamWriter.WriteLine("\t\t\t};");
                streamWriter.WriteLine();

                // Append the stored procedure execution
                streamWriter.WriteLine("\t\t\tusing (SqlDataReader dataReader = SqlClientUtility.ExecuteReader(connectionStringName, CommandType.StoredProcedure, \"" + procedureName + "\", parameters))");
                streamWriter.WriteLine("\t\t\t{");
                streamWriter.WriteLine("\t\t\t\tList<" + className + "> " + dtoVariableName + "List = new List<" + className + ">();");
                streamWriter.WriteLine("\t\t\t\twhile (dataReader.Read())");
                streamWriter.WriteLine("\t\t\t\t{");
                streamWriter.WriteLine("\t\t\t\t\t" + className + " " + dtoVariableName + " = Make" + className + "(dataReader);");
                streamWriter.WriteLine("\t\t\t\t\t" + dtoVariableName + "List.Add(" + dtoVariableName + ");");
                streamWriter.WriteLine("\t\t\t\t}");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t\t\treturn " + dtoVariableName + "List;");
                streamWriter.WriteLine("\t\t\t}");

                // Append the method footer
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine();
            }
        }

        /// <summary>
        /// Creates a string that represents the "make" functionality of the data access class.
        /// </summary>
        /// <param name="table">The Table instance that this method will be created for.</param>
        /// <param name="streamWriter">The StreamWriter instance that will be used to create the method.</param>
        private static void CreateMakeMethod(Table table, StreamWriter streamWriter)
        {
            string className = Utility.FormatClassName(table.Name);
            string dtoVariableName = Utility.FormatCamel(className);

            streamWriter.WriteLine("\t\t/// <summary>");
            streamWriter.WriteLine("\t\t/// Creates a new instance of the " + table.Name + " class and populates it with data from the specified SqlDataReader.");
            streamWriter.WriteLine("\t\t/// </summary>");
            streamWriter.WriteLine("\t\tprotected virtual " + className + " Make" + className + "(SqlDataReader dataReader)");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine("\t\t\t" + className + " " + dtoVariableName + " = new " + className + "();");

            foreach (Column column in table.Columns)
            {
                string columnNamePascal = Utility.FormatPascal(column.Name);
                streamWriter.WriteLine("\t\t\t" + dtoVariableName + "." + columnNamePascal + " = SqlClientUtility." + Utility.GetXxxMethod(column) + "(dataReader, \"" + column.Name + "\", " + Utility.GetDefaultValue(column) + ");");
            }

            streamWriter.WriteLine();
            streamWriter.WriteLine("\t\t\treturn " + dtoVariableName + ";");
            streamWriter.WriteLine("\t\t}");
        }

        /// <summary>
        /// Creates a string that represents the insert functionality of the data access class.
        /// </summary>
        /// <param name="table">The Table instance that this method will be created for.</param>
        /// <param name="storedProcedurePrefix">The prefix that is used on the stored procedure that this method will call.</param>
        /// <param name="streamWriter">The StreamWriter instance that will be used to create the method.</param>
        private static void CreateMockInsertMethod(Table table, string storedProcedurePrefix, StreamWriter streamWriter)
        {
            string className = Utility.FormatClassName(table.Name);
            string variableName = Utility.FormatCamel(className);

            // Append the method header
            streamWriter.WriteLine("\t\t/// <summary>");
            streamWriter.WriteLine("\t\t/// Saves a record to the " + table.Name + " table.");
            streamWriter.WriteLine("\t\t/// </summary>");
            streamWriter.WriteLine("\t\tpublic override void Insert(" + className + " " + variableName + ")");
            streamWriter.WriteLine("\t\t{");

            // Append validation for the parameter
            streamWriter.WriteLine("\t\t\tValidationUtility.ValidateArgument(\"" + variableName + "\", " + variableName + ");");

            // Append the method footer
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine();
        }

        /// <summary>
        /// Creates a string that represents the update functionality of the data access class.
        /// </summary>
        /// <param name="table">The Table instance that this method will be created for.</param>
        /// <param name="storedProcedurePrefix">The prefix that is used on the stored procedure that this method will call.</param>
        /// <param name="streamWriter">The StreamWriter instance that will be used to create the method.</param>
        private static void CreateMockUpdateMethod(Table table, string storedProcedurePrefix, StreamWriter streamWriter)
        {
            if (table.PrimaryKeys.Count > 0 && table.Columns.Count != table.PrimaryKeys.Count && table.Columns.Count != table.ForeignKeys.Count)
            {
                string className = Utility.FormatClassName(table.Name);
                string variableName = Utility.FormatCamel(className);

                // Append the method header
                streamWriter.WriteLine("\t\t/// <summary>");
                streamWriter.WriteLine("\t\t/// Updates a record in the " + table.Name + " table.");
                streamWriter.WriteLine("\t\t/// </summary>");
                streamWriter.WriteLine("\t\tpublic override void Update(" + className + " " + variableName + ")");
                streamWriter.WriteLine("\t\t{");

                // Append validation for the parameter
                streamWriter.WriteLine("\t\t\tValidationUtility.ValidateArgument(\"" + variableName + "\", " + variableName + ");");

                // Append the method footer
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine();
            }
        }

        /// <summary>
        /// Creates a string that represents the delete functionality of the data access class.
        /// </summary>
        /// <param name="table">The Table instance that this method will be created for.</param>
        /// <param name="storedProcedurePrefix">The prefix that is used on the stored procedure that this method will call.</param>
        /// <param name="streamWriter">The StreamWriter instance that will be used to create the method.</param>
        private static void CreateMockDeleteMethod(Table table, string storedProcedurePrefix, StreamWriter streamWriter)
        {
            if (table.PrimaryKeys.Count > 0)
            {
                // Append the method header
                streamWriter.WriteLine("\t\t/// <summary>");
                streamWriter.WriteLine("\t\t/// Deletes a record from the " + table.Name + " table by its primary key.");
                streamWriter.WriteLine("\t\t/// </summary>");
                streamWriter.Write("\t\tpublic override void Delete(");
                for (int i = 0; i < table.PrimaryKeys.Count; i++)
                {
                    Column column = table.PrimaryKeys[i];
                    streamWriter.Write(Utility.CreateMethodParameter(column));
                    if (i < (table.PrimaryKeys.Count - 1))
                    {
                        streamWriter.Write(", ");
                    }
                }
                streamWriter.WriteLine(")");
                streamWriter.WriteLine("\t\t{");

                // Append the method footer
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine();
            }
        }

        /// <summary>
        /// Creates a string that represents the "delete by" functionality of the data access class.
        /// </summary>
        /// <param name="table">The Table instance that this method will be created for.</param>
        /// <param name="storedProcedurePrefix">The prefix that is used on the stored procedure that this method will call.</param>
        /// <param name="streamWriter">The StreamWriter instance that will be used to create the method.</param>
        private static void CreateMockDeleteAllByMethods(Table table, string storedProcedurePrefix, StreamWriter streamWriter)
        {
            // Create a stored procedure for each foreign key
            foreach (List<Column> compositeKeyList in table.ForeignKeys.Values)
            {
                // Create the stored procedure name
                StringBuilder stringBuilder = new StringBuilder(255);
                stringBuilder.Append("DeleteAllBy");
                for (int i = 0; i < compositeKeyList.Count; i++)
                {
                    Column column = compositeKeyList[i];

                    if (i > 0)
                    {
                        stringBuilder.Append("_" + Utility.FormatPascal(column.Name));
                    }
                    else
                    {
                        stringBuilder.Append(Utility.FormatPascal(column.Name));
                    }
                }
                string methodName = stringBuilder.ToString();
                string procedureName = storedProcedurePrefix + table.Name + methodName;

                // Create the delete function based on keys
                // Append the method header
                streamWriter.WriteLine("\t\t/// <summary>");
                streamWriter.WriteLine("\t\t/// Deletes a record from the " + table.Name + " table by a foreign key.");
                streamWriter.WriteLine("\t\t/// </summary>");

                streamWriter.Write("\t\tpublic override void " + methodName + "(");
                for (int i = 0; i < compositeKeyList.Count; i++)
                {
                    Column column = compositeKeyList[i];
                    streamWriter.Write(Utility.CreateMethodParameter(column));
                    if (i < (compositeKeyList.Count - 1))
                    {
                        streamWriter.Write(", ");
                    }
                }
                streamWriter.WriteLine(")");
                streamWriter.WriteLine("\t\t{");

                // Append the method footer
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine();
            }
        }

        /// <summary>
        /// Creates a string that represents the select by primary key functionality of the data access class.
        /// </summary>
        /// <param name="table">The Table instance that this method will be created for.</param>
        /// <param name="storedProcedurePrefix">The prefix that is used on the stored procedure that this method will call.</param>
        /// <param name="streamWriter">The StreamWriter instance that will be used to create the method.</param>
        private static void CreateMockSelectMethod(Table table, string storedProcedurePrefix, StreamWriter streamWriter)
        {
            if (table.PrimaryKeys.Count > 0 && table.Columns.Count != table.ForeignKeys.Count)
            {
                string className = Utility.FormatClassName(table.Name);
                string dtoVariableName = Utility.FormatCamel(className);

                // Append the method header
                streamWriter.WriteLine("\t\t/// <summary>");
                streamWriter.WriteLine("\t\t/// Selects a single record from the " + table.Name + " table.");
                streamWriter.WriteLine("\t\t/// </summary>");

                streamWriter.Write("\t\tpublic override " + className + " Select(");
                for (int i = 0; i < table.PrimaryKeys.Count; i++)
                {
                    Column column = table.PrimaryKeys[i];
                    streamWriter.Write(Utility.CreateMethodParameter(column));
                    if (i < (table.PrimaryKeys.Count - 1))
                    {
                        streamWriter.Write(", ");
                    }
                }
                streamWriter.WriteLine(")");
                streamWriter.WriteLine("\t\t{");

                streamWriter.WriteLine("\t\t\treturn Make" + className + "(null);");

                // Append the method footer
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine();
            }
        }

        /// <summary>
        /// Creates a string that represents the select functionality of the data access class.
        /// </summary>
        /// <param name="table">The Table instance that this method will be created for.</param>
        /// <param name="storedProcedurePrefix">The prefix that is used on the stored procedure that this method will call.</param>
        /// <param name="streamWriter">The StreamWriter instance that will be used to create the method.</param>
        private static void CreateMockSelectAllMethod(Table table, string storedProcedurePrefix, StreamWriter streamWriter)
        {
            if (table.Columns.Count != table.PrimaryKeys.Count && table.Columns.Count != table.ForeignKeys.Count)
            {
                string className = Utility.FormatClassName(table.Name);
                string dtoVariableName = Utility.FormatCamel(className);

                // Append the method header
                streamWriter.WriteLine("\t\t/// <summary>");
                streamWriter.WriteLine("\t\t/// Selects all records from the " + table.Name + " table.");
                streamWriter.WriteLine("\t\t/// </summary>");
                streamWriter.WriteLine("\t\tpublic override List<" + className + "> SelectAll()");
                streamWriter.WriteLine("\t\t{");

                // Append the stored procedure execution
                streamWriter.WriteLine("\t\t\tList<" + className + "> " + dtoVariableName + "List = new List<" + className + ">();");
                streamWriter.WriteLine("\t\t\t" + className + " " + dtoVariableName + " = Make" + className + "(null);");
                streamWriter.WriteLine("\t\t\t" + dtoVariableName + "List.Add(" + dtoVariableName + ");");
                streamWriter.WriteLine("\t\t\treturn " + dtoVariableName + "List;");

                // Append the method footer
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine();
            }
        }

        /// <summary>
        /// Creates a string that represents the "select by" functionality of the data access class.
        /// </summary>
        /// <param name="table">The Table instance that this method will be created for.</param>
        /// <param name="storedProcedurePrefix">The prefix that is used on the stored procedure that this method will call.</param>
        /// <param name="streamWriter">The StreamWriter instance that will be used to create the method.</param>
        private static void CreateMockSelectAllByMethods(Table table, string storedProcedurePrefix, StreamWriter streamWriter)
        {
            string className = Utility.FormatClassName(table.Name);
            string dtoVariableName = Utility.FormatCamel(className);

            // Create a stored procedure for each foreign key
            foreach (List<Column> compositeKeyList in table.ForeignKeys.Values)
            {
                // Create the stored procedure name
                StringBuilder stringBuilder = new StringBuilder(255);
                stringBuilder.Append("SelectAllBy");
                for (int i = 0; i < compositeKeyList.Count; i++)
                {
                    Column column = compositeKeyList[i];

                    if (i > 0)
                    {
                        stringBuilder.Append("_" + Utility.FormatPascal(column.Name));
                    }
                    else
                    {
                        stringBuilder.Append(Utility.FormatPascal(column.Name));
                    }
                }
                string methodName = stringBuilder.ToString();
                string procedureName = storedProcedurePrefix + table.Name + methodName;

                // Create the select function based on keys
                // Append the method header
                streamWriter.WriteLine("\t\t/// <summary>");
                streamWriter.WriteLine("\t\t/// Selects all records from the " + table.Name + " table by a foreign key.");
                streamWriter.WriteLine("\t\t/// </summary>");

                streamWriter.Write("\t\tpublic override List<" + className + "> " + methodName + "(");
                for (int i = 0; i < compositeKeyList.Count; i++)
                {
                    Column column = compositeKeyList[i];
                    streamWriter.Write(Utility.CreateMethodParameter(column));
                    if (i < (compositeKeyList.Count - 1))
                    {
                        streamWriter.Write(", ");
                    }
                }
                streamWriter.WriteLine(")");
                streamWriter.WriteLine("\t\t{");

                streamWriter.WriteLine("\t\t\tList<" + className + "> " + dtoVariableName + "List = new List<" + className + ">();");
                streamWriter.WriteLine("\t\t\t" + className + " " + dtoVariableName + " = Make" + className + "(null);");
                streamWriter.WriteLine("\t\t\t" + dtoVariableName + "List.Add(" + dtoVariableName + ");");
                streamWriter.WriteLine("\t\t\treturn " + dtoVariableName + "List;");

                // Append the method footer
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine();
            }
        }

        /// <summary>
        /// Creates a string that represents the "make" functionality of the data access class.
        /// </summary>
        /// <param name="table">The Table instance that this method will be created for.</param>
        /// <param name="streamWriter">The StreamWriter instance that will be used to create the method.</param>
        private static void CreateMockMakeMethod(Table table, StreamWriter streamWriter)
        {
            string className = Utility.FormatClassName(table.Name);
            string dtoVariableName = Utility.FormatCamel(className);

            streamWriter.WriteLine("\t\t/// <summary>");
            streamWriter.WriteLine("\t\t/// Creates a new instance of the " + table.Name + " class and populates it with data from the specified SqlDataReader.");
            streamWriter.WriteLine("\t\t/// </summary>");
            streamWriter.WriteLine("\t\tprotected override " + className + " Make" + className + "(SqlDataReader dataReader)");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine("\t\t\t" + className + " " + dtoVariableName + " = new " + className + "();");
            streamWriter.WriteLine("\t\t\treturn " + dtoVariableName + ";");
            streamWriter.WriteLine("\t\t}");
        }
    }
}
