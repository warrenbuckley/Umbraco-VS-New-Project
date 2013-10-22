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


            //TODO: Get from assembly
            var versionNumber = "1.1.X";

            //TODO: Get from NuGet package
            var umbracoVersionNumber = "6.1.X";

            //Set Template Project Version in Toolbar
            TemplateProjectVersion.Content = string.Format("Version {0}", versionNumber);

            //Set Umbraco Nuget Version in Toolbar
            UmbracoVersion.Content = string.Format("Umbraco NuGet Version {0}", umbracoVersionNumber);

        }

        private void CreateProjectClick(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
