using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using umbraco;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace Umbraco.VS.NewProject.Wizard
{
    public partial class UserInputForm : Form
    {
        //New items
        private string dbConnectionString;
        private string dbType;

        public UserInputForm()
        {
            InitializeComponent();
        }

        public string get_dbConnectionString()
        {
            return dbConnectionString;
        }

        public string get_dbType()
        {
            return dbType;
        }

        private string _csProjPath;
        public string CSProjPath
        {
            get { return _csProjPath; }
            set { _csProjPath = value; }
        }

       

        private void rdoCustomDB_CheckedChanged(object sender, EventArgs e)
        {
            //If custom db checked
            if (rdoCustomDB.Checked)
            {
                //Show custom DB connection panel
                pnlCustomDB.Visible = true;

                //Disable main button (will be enabled with test db connection)
                btnCreateProj.Enabled = false;

            }
            else
            {
                //Hide the panel...
                pnlCustomDB.Visible = false;

                //Enable the main button - in case user changes mind
                btnCreateProj.Enabled = true;
            }
        }

        private void btnTestConnection_Click(object sender, EventArgs e)
        {
            //Check DB connection string..
            var connectionString = txtDBConnection.Text;

            //Only do db chekc if not null or empty
            if (!String.IsNullOrEmpty(connectionString))
            {
                SqlConnection dbCon = new SqlConnection();

                try
                {
                    dbCon.ConnectionString = connectionString;
                    dbCon.Open();

                    //If valid connection enable main button...
                    MessageBox.Show("Test Connection Succeeded.", "DB Success", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    btnCreateProj.Enabled = true;
                }
                catch (Exception ex)
                {
                    //Otherwise disable the button & show error message
                    MessageBox.Show("Test Connection Failed - exception: " + ex.Message, "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnCreateProj.Enabled = false;
                }
                finally
                {
                    try
                    {
                        dbCon.Close();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            else
            {
                //Otherwise disable the button & show error message
                MessageBox.Show("Test Connection Failed: Please enter a connection string", "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnCreateProj.Enabled = false;
            }
        }

        private void btnCreateProj_Click(object sender, EventArgs e)
        {

            //Fetch value if MVC or WebForms
            if (rdoMVC.Checked)
            {
                //Updates config value...
                UpdateRenderingEngine(RenderingEngine.Mvc);

            }
            else if (rdoWebForms.Checked)
            {
                //Updates config value...
                UpdateRenderingEngine(RenderingEngine.WebForms);
            }



            //Fetch DB choice (CE or custom)
            //If CE create CE db file and update web.config connection
            if (rdoSQLCE.Checked)
            {
                //Create SQL CE DB file on disk

                //Set web.config value
                //<add name="umbracoDbDSN" connectionString="Datasource=|DataDirectory|Umbraco.sdf" providerName="System.Data.SqlServerCe.4.0" />
                dbConnectionString  = "Datasource=|DataDirectory|Umbraco.sdf";
                dbType              = "CE";


                //Use Umbraco DB API - Create CE DB & updates web.config
                //Values aboved not needed/used, as API does it for us

                if (Umbraco.Core.ApplicationContext.Current != null)
                {
                    //Setup DB config - SQL CE
                    Umbraco.Core.ApplicationContext.Current.DatabaseContext.ConfigureEmbeddedDatabaseConnection();

                    //Use API to create DB schema... (Skips Web Installer)
                    Umbraco.Core.ApplicationContext.Current.DatabaseContext.Database.CreateDatabaseSchema();
                }


            }
            else if (rdoCustomDB.Checked)
            {
                //else custom DB - just update web.config connection
                //Just use the DB value from the textbox
                dbConnectionString  = txtDBConnection.Text;
                dbType              = "Custom";

                //Use Umbraco DB API - to update web.config
                if (Umbraco.Core.ApplicationContext.Current != null)
                {
                    //Setup DB config - manual DB connection
                    Umbraco.Core.ApplicationContext.Current.DatabaseContext.ConfigureDatabaseConnection(dbConnectionString);

                    //Use API to create DB schema... (Skips Web Installer)
                    Umbraco.Core.ApplicationContext.Current.DatabaseContext.Database.CreateDatabaseSchema();
                }
            }


            //All done - close form
            this.Dispose();
        }

        private void UpdateRenderingEngine(Umbraco.Core.RenderingEngine engine)
        {
            //Local variables

            // C:\\inetpub\\wwwroot\\UmbracoWebsite5\\UmbracoWebsite5\\UmbracoWebsite5.csproj
            var pathToCSProj = CSProjPath;
            
            // C:\\inetpub\\wwwroot\\UmbracoWebsite5\\UmbracoWebsite5\\
            var rootFolder = pathToCSProj.Replace(Path.GetFileName(pathToCSProj), string.Empty);

            // C:\\inetpub\\wwwroot\\UmbracoWebsite5\\UmbracoWebsite5\\config\
            var path = rootFolder + "config" + Path.DirectorySeparatorChar;

            // C:\\inetpub\\wwwroot\\UmbracoWebsite5\\UmbracoWebsite5\\config\umbracoSettings.config
            var filePath = path + "umbracoSettings.config";

            //Template mode (Mvc or WebForms)
            var value = engine;

            //Load the config file as xml
            var xml             = new XmlDocument();
            var settingsReader  = new XmlTextReader(filePath);

            try
            {
                //Load the XML file
                xml.Load(settingsReader);

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

                //Save the file back down...
                xml.Save(filePath); 
            }
            catch (Exception ex)
            {
                //Otherwise disable the button & show error message
                MessageBox.Show("Exception: " + ex.Message, "Update Rendering Engine Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            

            
        }
    }
}
