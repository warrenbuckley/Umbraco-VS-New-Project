using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using umbraco;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Core.Persistence.Migrations.Initial;
using Umbraco.Core.Persistence.SqlSyntax;

namespace Umbraco.VS.NewProject.Wizard.WPF
{
    //This is taken from Umbraco.Core but as we can't new up a DBContext
    public class DatabaseContext
    {
        private string _umbracoSitePath;
        public string umbracoSitePath
        {
            get { return _umbracoSitePath; }
            set { _umbracoSitePath = value; }
        }

        private string _providerName;
        public string providerName
        {
            get { return _providerName; }
            set { _providerName = value; }
        }
        
        private string _connectionString;
        public string connectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }


        public enum DatabaseType
        {
            SQLCE,
            SQL,
            Azure,
            MySQL,
            Advanced
        }

        /// <summary>
        /// Configure a ConnectionString for the embedded database.
        /// </summary>
        public void ConfigureEmbeddedDatabaseConnection()
        {
            providerName       = "System.Data.SqlServerCe.4.0";
            connectionString   = @"Data Source=|DataDirectory|\Umbraco.sdf;Flush Interval=1;";

            //Save connection string in web.config
            SaveConnectionString(connectionString, providerName);

            //Create a path to an Umbraco.sdf file in App_Data
            var path = Path.Combine(umbracoSitePath, "App_Data", "Umbraco.sdf");

            //If file does not exist then...
            if (!File.Exists(path))
            {
                //Modified Connection String for CE creation
                var modConnection = string.Format("Data Source={0};Flush Interval=1;", path);

                //Create a new SQL CE DB engine
                var engine = new SqlCeEngine(modConnection);
                engine.CreateDatabase();
            }

        }

        /// <summary>
        /// Configure a ConnectionString that has been entered manually.
        /// </summary>
        /// <remarks>
        /// Please note that we currently assume that the 'System.Data.SqlClient' provider can be used.
        /// </remarks>
        /// <param name="connString"></param>
        public void ConfigureDatabaseConnection(string connString)
        {
            SaveConnectionString(connString, string.Empty);
        }


        public static string GetDatabaseConnection(string server, string databaseName, string user, string password, DatabaseType databaseProvider)
        {
            var connectionString    = string.Empty;
            var providerName        = "System.Data.SqlClient";

            //If provider is MySql
            if (DatabaseType.MySQL == databaseProvider)
            {
                providerName = "MySql.Data.MySqlClient";
                connectionString = string.Format("Server={0}; Database={1};Uid={2};Pwd={3}", server, databaseName, user, password);
            }

            //If provider is Azure
            else if (DatabaseType.Azure == databaseProvider)
            {
                connectionString = BuildAzureConnectionString(server, databaseName, user, password);
            }

            //SQL
            else if (DatabaseType.SQL == databaseProvider)
            {
                connectionString = string.Format("server={0};database={1};user id={2};password={3}", server, databaseName, user, password);
            }

            return connectionString;
        }

        public static string GetIntegratedSecurityDatabaseConnection(string server, string databaseName)
        {
            return String.Format("Server={0};Database={1};Integrated Security=true", server, databaseName);
        }

        /// <summary>
        /// Configures a ConnectionString for the Umbraco database based on the passed in properties from the installer.
        /// </summary>
        /// <param name="server">Name or address of the database server</param>
        /// <param name="databaseName">Name of the database</param>
        /// <param name="user">Database Username</param>
        /// <param name="password">Database Password</param>
        /// <param name="databaseProvider">Type of the provider to be used (Sql, Azure, MySql)</param>
        public void ConfigureDatabaseConnection(string server, string databaseName, string user, string password, DatabaseType databaseProvider)
        {

            connectionString    = string.Empty;
            providerName        = "System.Data.SqlClient";

            //If provider is MySql
            if (DatabaseType.MySQL == databaseProvider)
            {
                providerName        = "MySql.Data.MySqlClient";
                connectionString    = string.Format("Server={0}; Database={1};Uid={2};Pwd={3}", server, databaseName, user, password);
            }

            //If provider is Azure
            else if (DatabaseType.Azure == databaseProvider)
            {
                connectionString = BuildAzureConnectionString(server, databaseName, user, password);
            }

            //SQL
            else if (DatabaseType.SQL == databaseProvider)
            {
                connectionString = string.Format("server={0};database={1};user id={2};password={3}", server, databaseName, user, password);
            }

            SaveConnectionString(connectionString, providerName);
        }

        public static string BuildAzureConnectionString(string server, string databaseName, string user, string password)
        {
            if (server.Contains(".") && ServerStartsWithTcp(server) == false)
            {
                server = string.Format("tcp:{0}", server);
            }
                

            if (server.Contains(".") == false && ServerStartsWithTcp(server))
            {
                string serverName = server.Contains(",") ? server.Substring(0, server.IndexOf(",", StringComparison.Ordinal)) : server;

                var portAddition = string.Empty;

                if (server.Contains(","))
                {
                    portAddition = server.Substring(server.IndexOf(",", StringComparison.Ordinal));
                }

                server = string.Format("{0}.database.windows.net{1}", serverName, portAddition);
            }

            if (ServerStartsWithTcp(server) == false)
            {
                server = string.Format("tcp:{0}.database.windows.net", server);
            }


            if (server.Contains(",") == false)
            {
                server = string.Format("{0},1433", server);
            }
                

            if (user.Contains("@") == false)
            {
                var userDomain = server;

                if (ServerStartsWithTcp(server))
                {
                    userDomain = userDomain.Substring(userDomain.IndexOf(":", StringComparison.Ordinal) + 1);
                }


                if (userDomain.Contains("."))
                {
                    userDomain = userDomain.Substring(0, userDomain.IndexOf(".", StringComparison.Ordinal));
                }

                user = string.Format("{0}@{1}", user, userDomain);
            }

            return string.Format("Server={0};Database={1};User ID={2};Password={3}", server, databaseName, user, password);
        }

        private static bool ServerStartsWithTcp(string server)
        {
            return server.ToLower().StartsWith("tcp:".ToLower());
        }

        /// <summary>
        /// Configures a ConnectionString for the Umbraco database that uses Microsoft SQL Server integrated security.
        /// </summary>
        /// <param name="server">Name or address of the database server</param>
        /// <param name="databaseName">Name of the database</param>
        public void ConfigureIntegratedSecurityDatabaseConnection(string server, string databaseName)
        {
            providerName        = "System.Data.SqlClient";
            connectionString    = String.Format("Server={0};Database={1};Integrated Security=true", server, databaseName);

            SaveConnectionString(connectionString, providerName);
        }

        /// <summary>
        /// Saves the connection string as a proper .net ConnectionString and the legacy AppSettings key/value.
        /// </summary>
        /// <remarks>
        /// Saves the ConnectionString in the very nasty 'medium trust'-supportive way.
        /// </remarks>
        /// <param name="connString"></param>
        /// <param name="provider"></param>
        private void SaveConnectionString(string connString, string provider)
        {
            //Path to web.config
            var webConfig = Path.Combine(umbracoSitePath, "Web.config");

            //Set Global Properties
            connectionString    = connString;
            providerName        = provider;


            //Open web.config
            var xml = XDocument.Load(webConfig, LoadOptions.PreserveWhitespace);

            //Get First XML element <connectionStrings>
            var connectionstrings = xml.Root.Descendants("connectionStrings").Single();

            //Check if attribute on <connectionStrings configSource="/config/connectionStrings.config">
            var configSource = connectionstrings.Attribute("configSource");

            //If we find it - load in config file from configSource attrbiute
            if (configSource != null)
            {
                //Get the value from the attribute as it exists
                var configSourcePath = configSource.Value;

                //Path to web.config
                var dbConfig = Path.Combine(umbracoSitePath, configSourcePath);

                //Open web.config
                var dbXML = XDocument.Load(dbConfig, LoadOptions.PreserveWhitespace);

                //Get First XML element <connectionStrings>
                var connectionstringsConfig = dbXML.Root;

                // Update connectionString if it exists, or else create a new connection string
                var settingConfig = connectionstringsConfig.Descendants("add").FirstOrDefault(s => s.Attribute("name").Value == "umbracoDbDSN");


                //Not found a connection string XML element - lets add it
                if (settingConfig == null)
                {
                    //Add an XML element into <connectionStrings>
                    //<add name="connectionName" connectionString="connectionString" providerName="providerName" />
                    connectionstringsConfig.Add(new XElement("add", new XAttribute("name", "umbracoDbDSN"), new XAttribute("connectionString", connString), new XAttribute("providerName", provider)));
                }
                else
                {
                    //Update the existing attribute values on the <add /> XML elements
                    settingConfig.Attribute("connectionString").Value = connString;
                    settingConfig.Attribute("providerName").Value = provider;
                }

                //Save file
                dbXML.Save(dbConfig);

                //Finished up
                return;
            }
            

            // Update connectionString if it exists, or else create a new connection string
            var setting = connectionstrings.Descendants("add").FirstOrDefault(s => s.Attribute("name").Value == "umbracoDbDSN");

            //Not found a connection string XML element - lets add it
            if (setting == null)
            {
                //Add an XML element into <connectionStrings>
                //<add name="connectionName" connectionString="connectionString" providerName="providerName" />
                connectionstrings.Add(new XElement("add", new XAttribute("name", "umbracoDbDSN"), new XAttribute("connectionString", connString), new XAttribute("providerName", provider)));
            }
            else
            {
                //Update the existing attribute values on the <add /> XML elements
                setting.Attribute("connectionString").Value = connString;
                setting.Attribute("providerName").Value     = provider;
            }

            //Save file
            xml.Save(webConfig);
        }


        
        public void CreateDatabaseSchema(DatabaseType dbType)
        {
            //Create a new Umbraco DB object using connection details
            var db = new UmbracoDatabase(connectionString, providerName);

            //Depending on DB Type - Change Provider
            if (dbType == DatabaseType.MySQL)
            {
                SqlSyntaxContext.SqlSyntaxProvider = new MySqlSyntaxProvider();
            }
            else if (dbType == DatabaseType.SQLCE)
            {
                SqlSyntaxContext.SqlSyntaxProvider = new SqlCeSyntaxProvider();
            }
            else
            {
                SqlSyntaxContext.SqlSyntaxProvider = new SqlServerSyntaxProvider();
            }

            //Create DB Schema
            //Get the method we want to run
            var methodToRun = typeof (PetaPocoExtensions).GetMethod("CreateDatabaseSchema", BindingFlags.Static | BindingFlags.NonPublic);

            //Invoke the Method - CreateDatabaseSchema
            methodToRun.Invoke(null, new object[]{ db, false });



            //Add/update default admin user of admin/admin
            db.Update<UserDto>("set userPassword = @password where id = @id", new { password = "d9xnUXsUah9gycu7D0TpRYcx19c=", id = 0 });

        }
    }
}
