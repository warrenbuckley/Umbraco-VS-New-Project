using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using Umbraco.Core;
using Path = System.IO.Path;

namespace Umbraco.VS.NewProject.WizardWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            //Main init component
            InitializeComponent();

            //TODO: Get from NuGet package
            var umbracoVersionNumber = "6.X.Y";

            //Set Umbraco Nuget Version in Toolbar
            UmbracoVersion.Content = string.Format("Umbraco NuGet Version {0}", umbracoVersionNumber);

        }

        private void CreateProjectClick(object sender, RoutedEventArgs e)
        {
            //Get the dropdown rendering engine value
            ComboBoxItem engineDropdown = (ComboBoxItem)renderingEngine.SelectedItem;

            //Variable to set engine mode in Umbraco
            //Set it to MVC by default;
            var engine = RenderingEngine.Mvc;

            //Check what value was selected
            switch (engineDropdown.Content.ToString())
            {
                case "Mvc (Recommended)":
                    engine = RenderingEngine.Mvc;
                    break;

                case "WebForms":
                    engine = RenderingEngine.WebForms;
                    break;

                default:
                    engine = RenderingEngine.Mvc;
                    break;
            }

            //Update Rendering Engine for Umbraco Project
            UpdateRenderingEngine(engine, "");


        }

        private void UpdateRenderingEngine(Umbraco.Core.RenderingEngine engine, string CSProjPath)
        {
            try
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
                var xml = new XmlDocument();


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
    }
}
