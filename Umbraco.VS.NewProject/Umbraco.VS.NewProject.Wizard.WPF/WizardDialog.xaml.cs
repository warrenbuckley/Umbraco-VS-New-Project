using System;
using System.Linq;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Linq;
using umbraco;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Core.Persistence;
using Path = System.IO.Path;

namespace Umbraco.VS.NewProject.Wizard.WPF
{
    /// <summary>
    /// Interaction logic for WizardDialog.xaml
    /// </summary>
    public partial class WizardDialog : UserControl
    {
        private string _umbracoSitePath;
        public string umbracoSitePath
        {
            get { return _umbracoSitePath; }
            set { _umbracoSitePath = value; }
        }

        public string umbracoVersionNumber { get; set; }

        public string umbracoVersion
        {
            get { return umbracoVersionStatusBar.Content.ToString(); }
            set { umbracoVersionStatusBar.Content = string.Format("Umbraco Nuget Version: {0}", value); }
        }

        public DatabaseContext.DatabaseType dbType { get; set; }

        public DatabaseContext _db;

        public WizardDialog()
        {
            InitializeComponent();

            //New Up DB Object
            _db = new DatabaseContext();
        }

        private void CreateProjectClick(object sender, RoutedEventArgs e)
        {
            //Get the selected value from the dropdown
            var selectedEngine = (engineChoice.SelectedValue as ComboBoxItem).Content.ToString();

            //Set rendeing mode dependant on choice
            switch (selectedEngine)
            {
                case "MVC (Recommended)":
                    //Updates config value...
                    UpdateRenderingEngine(RenderingEngine.Mvc);
                    break;

                case "WebForms":
                    //Updates config value...
                    UpdateRenderingEngine(RenderingEngine.WebForms);
                    break;

                default:
                    //Updates config value...
                    UpdateRenderingEngine(RenderingEngine.Mvc);
                    break;
            }

            //Set DB type
            //Get the selected value from the dropdown
            var selectedDB = (databaseType.SelectedValue as ComboBoxItem).Content.ToString();

            switch (selectedDB)
            {
                case "SQL CE File based Database (Recommended)":
                    dbType = DatabaseContext.DatabaseType.SQLCE;
                    UpdateDatabase(DatabaseContext.DatabaseType.SQLCE);
                    break;

                case "SQL Server":
                    //Update Database deals with the intergrated security check
                    dbType = DatabaseContext.DatabaseType.SQL;
                    UpdateDatabase(DatabaseContext.DatabaseType.SQL);
                    break;

                case "SQL Azure":
                    dbType = DatabaseContext.DatabaseType.SQL;
                    UpdateDatabase(DatabaseContext.DatabaseType.Azure);
                    break;

                case "MySQL":
                    dbType = DatabaseContext.DatabaseType.MySQL;
                    UpdateDatabase(DatabaseContext.DatabaseType.MySQL);
                    break;

                case "Advanced":
                    dbType = DatabaseContext.DatabaseType.SQL;
                    UpdateDatabase(DatabaseContext.DatabaseType.Advanced);
                    break;

                default:
                    //As a default fallback - use SQL CE
                    dbType = DatabaseContext.DatabaseType.SQLCE;
                    UpdateDatabase(DatabaseContext.DatabaseType.SQLCE);
                    break;
            }

            var skipInstaller = skipWebInstaller.IsChecked;

            //Only add config status version flag if skipping web installer
            if (skipInstaller == true)
            {
                //Update Config Status - Updates version number- means all config'd & skips installer
                //Substring - First 5 characters to get 7.0.0 as opposed to 7.0.0-RC
                UpdateConfigStatus(umbracoSitePath, umbracoVersionNumber.Substring(0, 5));

                //Create DB Schema in configured/chosen DB & update admin user
                _db.CreateDatabaseSchema(dbType, skipInstaller);

            }

            //Need to figure a way to close dialog from usercontrol
            Window parentWindow = Window.GetWindow(this);
            parentWindow.Close();
        }

        private void UpdateRenderingEngine(RenderingEngine engine)
        {
            //Local variables

            // C:\\inetpub\\wwwroot\\UmbracoWebsite5\\UmbracoWebsite5
            var rootFolder = umbracoSitePath;

            // C:\\inetpub\\wwwroot\\UmbracoWebsite5\\UmbracoWebsite5\\config\
            var path = rootFolder + Path.DirectorySeparatorChar + "config" + Path.DirectorySeparatorChar;

            // C:\\inetpub\\wwwroot\\UmbracoWebsite5\\UmbracoWebsite5\\config\umbracoSettings.config
            var filePath = path + "umbracoSettings.config";

            //Template mode (Mvc or WebForms)
            var value = engine;

            //Load the config file as xml
            var xml = new XmlDocument();

            try
            {
                using (XmlReader reader = XmlReader.Create(filePath))
                {
                    //Load XML
                    xml.Load(reader);
                }

                //Find the correct node for the 'defaultRenderingEngine' and update the value
                if (xml.DocumentElement != null)
                {
                    //Get the defaultRendering engine XML node, so we can update it's value
                    var node = xml.DocumentElement.SelectSingleNode("/settings/templates/defaultRenderingEngine");

                    if (node != null)
                    {
                        //Update the value in the XML
                        node.InnerText = value.ToString();
                    }
                }

                //Save file
                xml.Save(filePath);

            }
            catch (Exception ex)
            {
                //Otherwise disable the button & show error message
                MessageBox.Show("Exception: " + ex.Message, "Update Rendering Engine Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void databaseTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            //Get the selected value from the dropdown
            if ((databaseType.SelectedValue as ComboBoxItem).Content != null)
            {
                var dbTypeItem = (databaseType.SelectedValue as ComboBoxItem).Content.ToString();

                if (!string.IsNullOrEmpty(dbTypeItem))
                {

                    //Enable/disable controls depending on choice
                    switch (dbTypeItem)
                    {
                        case "SQL CE File based Database (Recommended)":
                            //Disable datbase details group
                            databaseDetailsGroup.IsEnabled  = false;
                            CreateProjectBtn.IsEnabled      = true;
                            break;

                        case "SQL Server":
                            databaseDetailsGroup.IsEnabled  = true;
                            serverLabel.IsEnabled           = true;
                            server.IsEnabled                = true;
                            databaseNameLabel.IsEnabled     = true;
                            databaseName.IsEnabled          = true;
                            usernameLabel.IsEnabled         = true;
                            username.IsEnabled              = true;
                            passwordLabel.IsEnabled         = true;
                            password.IsEnabled              = true;
                            securityLabel.IsEnabled         = true;
                            security.IsEnabled              = true;
                            connectionLabel.IsEnabled       = false;
                            connection.IsEnabled            = false;
                            testConnectionButton.IsEnabled  = true;
                            CreateProjectBtn.IsEnabled      = false;
                            break;

                        case "SQL Azure":
                            databaseDetailsGroup.IsEnabled  = true;
                            serverLabel.IsEnabled           = true;
                            server.IsEnabled                = true;
                            databaseNameLabel.IsEnabled     = true;
                            databaseName.IsEnabled          = true;
                            usernameLabel.IsEnabled         = true;
                            username.IsEnabled              = true;
                            passwordLabel.IsEnabled         = true;
                            password.IsEnabled              = true;
                            securityLabel.IsEnabled         = false;
                            security.IsEnabled              = false;
                            connectionLabel.IsEnabled       = false;
                            connection.IsEnabled            = false;
                            testConnectionButton.IsEnabled  = true;
                            CreateProjectBtn.IsEnabled      = false;
                            break;

                        case "MySQL":
                            databaseDetailsGroup.IsEnabled  = true;
                            serverLabel.IsEnabled           = true;
                            server.IsEnabled                = true;
                            databaseNameLabel.IsEnabled     = true;
                            databaseName.IsEnabled          = true;
                            usernameLabel.IsEnabled         = true;
                            username.IsEnabled              = true;
                            passwordLabel.IsEnabled         = true;
                            password.IsEnabled              = true;
                            securityLabel.IsEnabled         = false;
                            security.IsEnabled              = false;
                            connectionLabel.IsEnabled       = false;
                            connection.IsEnabled            = false;
                            testConnectionButton.IsEnabled  = true;
                            CreateProjectBtn.IsEnabled      = false;
                            break;

                        case "Advanced":
                            databaseDetailsGroup.IsEnabled  = true;
                            serverLabel.IsEnabled           = false;
                            server.IsEnabled                = false;
                            databaseNameLabel.IsEnabled     = false;
                            databaseName.IsEnabled          = false;
                            usernameLabel.IsEnabled         = false;
                            username.IsEnabled              = false;
                            passwordLabel.IsEnabled         = false;
                            password.IsEnabled              = false;
                            securityLabel.IsEnabled         = false;
                            security.IsEnabled              = false;
                            connectionLabel.IsEnabled       = true;
                            connection.IsEnabled            = true;
                            testConnectionButton.IsEnabled  = true;
                            CreateProjectBtn.IsEnabled      = false;
                            break;

                        default:
                            //Disable datbase details group
                            databaseDetailsGroup.IsEnabled  = false;
                            CreateProjectBtn.IsEnabled      = true;
                            break;
                    }
                }
            }
        }

        private void UpdateDatabase(DatabaseContext.DatabaseType dbType)
        {
            //Our copy of DatbaseContext from Umbraco.Core
            _db.umbracoSitePath = umbracoSitePath;

            switch (dbType)
            {
                case DatabaseContext.DatabaseType.SQLCE:
                    //Setup Datbase to use SQL CE
                    _db.ConfigureEmbeddedDatabaseConnection();
                    break;

                case DatabaseContext.DatabaseType.SQL:
                    //If intergated security checked then
                    if (security.IsChecked == true)
                    {
                        //Intergrated security SQL connection
                        _db.ConfigureIntegratedSecurityDatabaseConnection(server.Text, databaseName.Text);
                        break;
                    }
                    //Normal SQL connection
                    _db.ConfigureDatabaseConnection(server.Text, databaseName.Text, username.Text, password.Text, DatabaseContext.DatabaseType.SQL);
                    break;

                case DatabaseContext.DatabaseType.Azure:
                    //Normal Azure SQL connection
                    _db.ConfigureDatabaseConnection(server.Text, databaseName.Text, username.Text, password.Text, DatabaseContext.DatabaseType.Azure);
                    break;

                case DatabaseContext.DatabaseType.MySQL:
                    //Normal MySQL connection
                    _db.ConfigureDatabaseConnection(server.Text, databaseName.Text, username.Text, password.Text, DatabaseContext.DatabaseType.MySQL);
                    break;

                case DatabaseContext.DatabaseType.Advanced:
                    //Configure using custom DB connection string supplied
                    _db.ConfigureDatabaseConnection(connection.Text);
                    break;

                default:
                    //Setup Datbase to use SQL CE
                    _db.ConfigureEmbeddedDatabaseConnection();
                    break;
            }
        }

        private void intergratedSecurityClick(object sender, RoutedEventArgs e)
        {
            //Security checkbox checked
            if (security.IsChecked == true)
            {
                usernameLabel.IsEnabled = false;
                username.IsEnabled      = false;
                passwordLabel.IsEnabled = false;
                password.IsEnabled      = false;

            }
            else if (security.IsChecked == false)
            {
                usernameLabel.IsEnabled = true;
                username.IsEnabled      = true;
                passwordLabel.IsEnabled = true;
                password.IsEnabled      = true;
            }
        }

        private void testConnectionButtonClick(object sender, RoutedEventArgs e)
        {
            //Get Connection String & Provider Name
            var connectionString    = string.Empty;
            var providerName        = string.Empty;

            //Get the selected DB value from the dropdown
            if ((databaseType.SelectedValue as ComboBoxItem).Content != null)
            {
                var dbTypeItem = (databaseType.SelectedValue as ComboBoxItem).Content.ToString();

                var dbName      = databaseName.Text;
                var dbServer    = server.Text;
                var dbUser      = username.Text;
                var dbPass      = password.Text;
                var conn        = string.Empty;

                //If dbName or dbServer empty show messagebox & return
                if (string.IsNullOrEmpty(dbName) || string.IsNullOrEmpty(dbServer))
                {
                    MessageBox.Show("Please provide a Database Name and/or Database Server", "Database Connection Test", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                switch (dbTypeItem)
                {
                    case "SQL Server":
                        if (security.IsChecked == true)
                        {
                            //Intergrated security SQL connection
                            conn = DatabaseContext.GetIntegratedSecurityDatabaseConnection(dbServer, dbName);
                            TestDB(DatabaseContext.DatabaseType.SQL, conn, "System.Data.SqlClient");
                            break;
                        }
                        //Normal SQL connection
                        conn = DatabaseContext.GetDatabaseConnection(dbServer, dbName, dbUser, dbPass, DatabaseContext.DatabaseType.SQL);
                        TestDB(DatabaseContext.DatabaseType.SQL, conn, "System.Data.SqlClient");
                        break;

                    case "SQL Azure":
                        conn = DatabaseContext.GetDatabaseConnection(dbServer, dbName, dbUser, dbPass, DatabaseContext.DatabaseType.Azure);
                        TestDB(DatabaseContext.DatabaseType.Azure, conn, "System.Data.SqlClient");
                        break;

                    case "MySQL":
                        conn = DatabaseContext.GetDatabaseConnection(dbServer, dbName, dbUser, dbPass, DatabaseContext.DatabaseType.MySQL);
                        TestDB(DatabaseContext.DatabaseType.MySQL, conn, "MySql.Data.MySqlClient");
                        break;

                    case "Advanced":
                        TestDB(DatabaseContext.DatabaseType.Advanced, connection.Text, string.Empty);
                        break;
                }
            }
        }

        private void TestDB(DatabaseContext.DatabaseType dbType, string connectionString, string providerName)
        {
            //Let's try....
            try
            {
                //Try and create & connect to the Database
                //If Advanced provider name etc all in the connection string
                if (dbType == DatabaseContext.DatabaseType.Advanced)
                {
                    var db = new UmbracoDatabase(connectionString);
                }
                else
                {
                    //Providing both connection string & provider name
                    var db = new UmbracoDatabase(connectionString, providerName);

                    //Now open the connection to test it out...
                    //Exception will fire if it can connect
                    db.OpenSharedConnection();
                }


                //Show a Success Message Box
                MessageBox.Show("Database Connection Sucessful", "Database Connection Test", MessageBoxButton.OK, MessageBoxImage.Information);

                //Enable Button
                CreateProjectBtn.IsEnabled = true;
            }
            catch (Exception ex)
            {
                var error = string.Format("Database Connection Error: {0}", ex.Message);

                //Show an Error Message Box
                MessageBox.Show(error, "Database Connection Test", MessageBoxButton.OK, MessageBoxImage.Error);

                //Disable Button
                CreateProjectBtn.IsEnabled = false;
            }

        }

        public static void UpdateConfigStatus(string path, string version)
        {
            //Check if we are configured already
            if (!GlobalSettings.Configured)
            {

                //Umbraco Version
                var umbVersion = version;


                //Update the version number in the web.config or in newer from /config/appsettings.config
                //Path to web.config
                var webConfig = Path.Combine(path, "Web.config");

                //Open web.config
                var xml = XDocument.Load(webConfig, LoadOptions.PreserveWhitespace);

                //Get First XML element <connectionStrings>
                var appSettings = xml.Root.Descendants("appSettings").Single();

                //Check if attribute on <appSettings configSource="config\appSettings.config">
                var appSettingsSource = appSettings.Attribute("configSource");


                //If we find it - load in config file from configSource attrbiute
                if (appSettingsSource != null)
                {
                    //Get the value from the attribute as it exists
                    var configSourcePath = appSettingsSource.Value;

                    //Path to appsettings.config
                    var appSettingConfig = Path.Combine(path, configSourcePath);

                    //Open web.config
                    var appXML = XDocument.Load(appSettingConfig, LoadOptions.PreserveWhitespace);

                    //Get First XML element <appSettings>
                    var appSettingsConfig = appXML.Root;

                    // Update add if it exists, or else create a new connection string
                    var settingConfig = appSettingsConfig.Descendants("add").FirstOrDefault(s => s.Attribute("key").Value == "umbracoConfigurationStatus");


                    //Not found a connection string XML element - lets add it
                    if (settingConfig == null)
                    {
                        //Add an XML element into <appSettings>
                        //<add key="umbracoConfigurationStatus" value="" />
                        appSettingsConfig.Add(new XElement("add", new XAttribute("key", "umbracoConfigurationStatus"),new XAttribute("value", umbVersion)));
                    }
                    else
                    {
                        //Update the existing attribute values on the <add /> XML elements
                        settingConfig.Attribute("value").Value = umbVersion;
                    }

                    //Save file
                    appXML.Save(appSettingConfig);

                    //Finished up
                    return;
                }

                //OK no configSource - it's in the web.config
                // Update connectionString if it exists, or else create a new connection string
                var setting = appSettings.Descendants("add").FirstOrDefault(s => s.Attribute("key").Value == "umbracoConfigurationStatus");

                //Not found a connection string XML element - lets add it
                if (setting == null)
                {
                    //Add an XML element into <connectionStrings>
                    //<add name="connectionName" connectionString="connectionString" providerName="providerName" />
                    appSettings.Add(new XElement("add", new XAttribute("key", "umbracoConfigurationStatus"), new XAttribute("value", umbVersion)));
                }
                else
                {
                    //Update the existing attribute values on the <add /> XML elements
                    setting.Attribute("value").Value = umbVersion;
                }

                //Save file
                xml.Save(webConfig);
            }
        }

        private void skipWebInstallerClick(object sender, RoutedEventArgs e)
        {
            //Security checkbox checked
            if (skipWebInstaller.IsChecked == true)
            {
                MessageBox.Show(
                    "Remember the installer will be skipped and you can login straight in, with admin & admin.",
                    "Remember", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

    }
}
