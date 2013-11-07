using System;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
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

        public string umbracoVersion
        {
            get { return umbracoVersionStatusBar.Content.ToString(); }
            set { umbracoVersionStatusBar.Content = string.Format("Umbraco Nuget Version: {0}", value); }
        }

        public enum DatabaseType
        {
            SQLCE,
            SQL,
            Azure,
            MySQL,
            Advanced
        }

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
                    UpdateDatabase(DatabaseType.SQLCE);
                    break;

                case "SQL Server":
                    //Update Database deals with the intergrated security check
                    UpdateDatabase(DatabaseType.SQL);
                    break;

                case "SQL Azure":
                    UpdateDatabase(DatabaseType.Azure);
                    break;

                case "MySQL":
                    UpdateDatabase(DatabaseType.MySQL);
                    break;

                case "Advanced":
                    UpdateDatabase(DatabaseType.Advanced);
                    break;

                default:
                    //As a default fallback - use SQL CE
                    UpdateDatabase(DatabaseType.SQLCE);
                    break;
            }


            //Create DB Schema in configured/chosen DB
            _db.CreateDatabaseSchema();

            //Update Config Status - Updates version number- means all config'd & skips installer
            UpdateConfigStatus();

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

        private void UpdateDatabase(DatabaseType dbType)
        {
            //Our copy of DatbaseContext from Umbraco.Core
            _db.umbracoSitePath = umbracoSitePath;

            switch (dbType)
            {
                case DatabaseType.SQLCE:
                    //Setup Datbase to use SQL CE
                    _db.ConfigureEmbeddedDatabaseConnection();
                    break;

                case DatabaseType.SQL:
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

                case DatabaseType.Azure:
                    //Normal Azure SQL connection
                    _db.ConfigureDatabaseConnection(server.Text, databaseName.Text, username.Text, password.Text, DatabaseContext.DatabaseType.Azure);
                    break;

                case DatabaseType.MySQL:
                    //Normal MySQL connection
                    _db.ConfigureDatabaseConnection(server.Text, databaseName.Text, username.Text, password.Text, DatabaseContext.DatabaseType.MySQL);
                    break;

                case DatabaseType.Advanced:
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
                            TestDB(DatabaseType.SQL, conn, "System.Data.SqlClient");
                            break;
                        }
                        //Normal SQL connection
                        conn = DatabaseContext.GetDatabaseConnection(dbServer, dbName, dbUser, dbPass, DatabaseContext.DatabaseType.SQL);
                        TestDB(DatabaseType.SQL, conn, "System.Data.SqlClient");
                        break;

                    case "SQL Azure":
                        conn = DatabaseContext.GetDatabaseConnection(dbServer, dbName, dbUser, dbPass, DatabaseContext.DatabaseType.Azure);
                        TestDB(DatabaseType.Azure, conn, "System.Data.SqlClient");
                        break;

                    case "MySQL":
                        conn = DatabaseContext.GetDatabaseConnection(dbServer, dbName, dbUser, dbPass, DatabaseContext.DatabaseType.MySQL);
                        TestDB(DatabaseType.MySQL, conn, "MySql.Data.MySqlClient");
                        break;

                    case "Advanced":
                        TestDB(DatabaseType.Advanced, connection.Text, string.Empty);
                        break;
                }
            }
        }

        private void TestDB(DatabaseType dbType, string connectionString, string providerName)
        {
            //Let's try....
            try
            {
                //Try and create & connect to the Database
                //If Advanced provider name etc all in the connection string
                if (dbType == DatabaseType.Advanced)
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

        public static void UpdateConfigStatus()
        {
            //Check if we are configured already
            if (!GlobalSettings.Configured)
            {
                //Update the version number in the web.config
                GlobalSettings.ConfigurationStatus = UmbracoVersion.Current.ToString(3);
            }

            
        }

    }
}
