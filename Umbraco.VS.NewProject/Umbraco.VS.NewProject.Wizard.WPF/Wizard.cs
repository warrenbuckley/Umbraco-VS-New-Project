using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Windows;
using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;
using NuGet;
using NuGet.VisualStudio;
using Window = System.Windows.Window;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.ComponentModelHost;

namespace Umbraco.VS.NewProject.Wizard.WPF
{
    public class Wizard : IWizard
    {
        private DTE _dte;
        private string _csProjPath;
        private string _projectPath;
        private string _solutionPath;
        private string _packagePath;
        private string _solutionFolder;
        private string _nugetPackageFolder;
        private string _destinationFolder;
        private string _packageVersion;
        private IComponentModel _serviceHost;

        /// <summary>
        /// Runs custom wizard logic at the beginning of a template wizard run.
        /// </summary>
        /// <param name="automationObject">The automation object being used by the template wizard.</param>
        /// <param name="replacementsDictionary">The list of standard parameters to be replaced.</param>
        /// <param name="runKind">A <see cref="T:Microsoft.VisualStudio.TemplateWizard.WizardRunKind"/> indicating the type of wizard run.</param>
        /// <param name="customParams">The custom parameters with which to perform parameter replacement in the project.</param>
        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            //Get the DTE object
            _dte = (DTE)automationObject;

            //Get service host
            if (_serviceHost == null)
            {
                _serviceHost = (IComponentModel)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SComponentModel));
            }

            //Root Solution Folder - C:\\inetpub\\wwwroot\\UmbracoWebsite2
            _solutionFolder = replacementsDictionary["$solutiondirectory$"];

            //Nuget Packages Folder - ..\\packages\\
            _nugetPackageFolder = replacementsDictionary["$nugetpackagesfolder$"];

            //Destination Folder - C:\\inetpub\\wwwroot\\UmbracoWebsite2\\UmbracoWebsite2
            _destinationFolder = replacementsDictionary["$destinationdirectory$"];
	

            //Debug
            Debug.WriteLine("Umbraco New Project - RunStarted() event");
        }

        /// <summary>
        /// Runs custom wizard logic when a project has finished generating.
        /// </summary>
        /// <param name="project">The project that finished generating.</param>
        public void ProjectFinishedGenerating(Project project)
        {
            //Debug
            Debug.WriteLine("Umbraco New Project - ProjectFinishedGenerating() event");

            try
            {
                //File Path stuff
                _csProjPath     = project.FileName;
                _projectPath    = Path.GetDirectoryName(_csProjPath);
                _solutionPath   = Path.GetDirectoryName(_projectPath);
                _packagePath    = Path.Combine(_solutionPath, "packages");


                //Go Get Umbraco from Nuget
                GetUmbraco(project);

                //Wizard Dialog (WPF Usercontrol)
                var wizard              = new WizardDialog();
                wizard.umbracoSitePath  = _destinationFolder;
                wizard.umbracoVersion   = _packageVersion;

                //Create a WPF Window
                //Add our WPF UserControl to the window
                Window myWindow = new Window
                {
                    Title                   = "Create New Umbraco Project Wizard",
                    Content                 = wizard,
                    SizeToContent           = SizeToContent.WidthAndHeight,
                    ResizeMode              = ResizeMode.NoResize,
                    WindowStartupLocation   = WindowStartupLocation.CenterScreen
                };

                //Show the window/dialog
                myWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Runs custom wizard logic when a project item has finished generating.
        /// </summary>
        /// <param name="projectItem">The project item that finished generating.</param>
        public void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
            Debug.WriteLine("Umbraco New Project - ProjectItemFinishedGenerating() event");
        }


        /// <summary>
        /// Indicates whether the specified project item should be added to the project.
        /// </summary>
        /// <returns>
        /// true if the project item should be added to the project; otherwise, false.
        /// </returns>
        /// <param name="filePath">The path to the project item.</param>
        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }


        /// <summary>
        /// Runs custom wizard logic before opening an item in the template.
        /// </summary>
        /// <param name="projectItem">The project item that will be opened.</param>
        public void BeforeOpeningFile(ProjectItem projectItem)
        {
            Debug.WriteLine("Umbraco New Project - BeforeOpeningFile() event");
        }

        /// <summary>
        /// Runs custom wizard logic when the wizard has completed all tasks.
        /// </summary>
        public void RunFinished()
        {
            Debug.WriteLine("Umbraco New Project - RunFinished() event");
        }


        public void GetUmbraco(Project project)
        {
            //Update IDE status bar bottom left
            _dte.StatusBar.Text = "Umbraco New Project - Getting Umbraco (Please Wait)";

            //Debug
            Debug.WriteLine("Umbraco New Project - GetUmbraco() event");

            //ID of the package to be looked up
            string packageID = "UmbracoCMS";


            //Connect to the official package repository
            IPackageRepository onlineRepo = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");

            //Get the list of all NuGet packages with ID 'UmbracoCMS' then get latest 'stable' version (6.1.6)
            var umbracoPackages = onlineRepo.FindPackagesById(packageID).ToList();
            var umbraco         = umbracoPackages.FirstOrDefault(x => x.IsLatestVersion && x.IsReleaseVersion());
            _packageVersion     = umbraco.Version.ToString();


            //Check serviceHost is not null otherwise NuGet extension points will be able to be fetched
            if (_serviceHost != null)
            {
                //Get Nuget Extensibilty points
                var nugetEvents = _serviceHost.GetService<IVsPackageInstallerEvents>();
                var installer   = _serviceHost.GetService<IVsPackageInstaller>();
                var uninstaller = _serviceHost.GetService<IVsPackageUninstaller>();
                var packageInfo = _serviceHost.GetService<IVsPackageInstallerServices>();

                //Debug
                Debug.WriteLine("Umbraco New Project - Installing Nuget Packages");

                //Update IDE status bar bottom left
                _dte.StatusBar.Text = "Umbraco New Project - Installing Nuget Packages";

                //Wire up events
                nugetEvents.PackageInstalling       += nugetEvents_PackageInstalling;
                nugetEvents.PackageInstalled        += nugetEvents_PackageInstalled;
                nugetEvents.PackageReferenceAdded   += nugetEvents_PackageReferenceAdded;


                //Get local %LocalAppData% folder - C:\Users\Warren Buckley\AppData\Local
                var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

                //Get the NuGet Local Cache Folder - C:\Users\Warren Buckley\AppData\Local\Nuget\Cache
                var nugetCacheFolder = Path.Combine(appDataFolder, "NuGet\\Cache");

                //Connect to a local package repo
                IPackageRepository localRepo = PackageRepositoryFactory.Default.CreateRepository(nugetCacheFolder);

                //Try and find Umbraco Package with specific version from live repo in our local repo cache
                //If we have it in our local cache most likely to have all other depenadcies in local repo cache too
                var findLocalUmbraco = localRepo.FindPackagesById(packageID).SingleOrDefault(x => x.Version.ToString() == _packageVersion);

                //If we found the package in local cache - lets install from local cache repo
                if (findLocalUmbraco != null && localRepo != null)
                {
                    //Download and unzip the package/s - Gets all the dependcies needed as well
                    installer.InstallPackage(localRepo, project, packageID, _packageVersion, false, false);
                }
                else
                {
                    //Use Online Repo to install packages - will be slower as got to fetch them over thew wire

                    //Download and unzip the package/s - Gets all the dependcies needed as well
                    installer.InstallPackage(onlineRepo, project, packageID, _packageVersion, false, false);
                }
            }
        }
        

        private void nugetEvents_PackageInstalling(IVsPackageMetadata metadata)
        {
            //Debug
            Debug.WriteLine(string.Format("Umbraco New Project - PackageInstalling() event {0} {1}", metadata.Title, metadata.VersionString));

            //Update IDE status bar bottom left
            _dte.StatusBar.Text = string.Format("Umbraco New Project - Installing Nuget Package {0} {1}", metadata.Title, metadata.VersionString);
        }

        private void nugetEvents_PackageInstalled(IVsPackageMetadata metadata)
        {
            //Debug
            Debug.WriteLine(string.Format("Umbraco New Project - PackageInstalled() event {0} {1}", metadata.Title, metadata.VersionString));

            //Update IDE status bar bottom left
            _dte.StatusBar.Text = string.Format("Umbraco New Project - Installed Nuget Package {0} {1}", metadata.Title, metadata.VersionString);
        }

        private void nugetEvents_PackageReferenceAdded(IVsPackageMetadata metadata)
        {
            //Debug
            Debug.WriteLine(string.Format("Umbraco New Project - PackageReferencedAdded() event {0} {1}", metadata.Title, metadata.VersionString));

            //Update IDE status bar bottom left
            _dte.StatusBar.Text = string.Format("Umbraco New Project - Adding Nuget Package Reference {0} {1}", metadata.Title, metadata.VersionString);
        }
    }
}
