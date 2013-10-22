using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TemplateWizard;
using System.Windows.Forms;
using EnvDTE;
using NuGet;
using NuGet.VisualStudio;
using Umbraco.Core;
using umbraco.presentation.install.utills;
using WebProjectSystem = System.Web.WebPages.Administration.PackageManager.WebProjectSystem;

namespace Umbraco.VS.NewProject.Wizard
{
    public class Wizard : IWizard
    {
        private UserInputForm inputForm;
        private DTE _dte;
        private string _csProjPath;
        private string _projectPath;
        private string _solutionPath;
        private string _packagePath;
        private ServiceProvider _services;

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

            //Get services
            _services = new ServiceProvider(automationObject as Microsoft.VisualStudio.OLE.Interop.IServiceProvider);

           
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

                // Display a form to the user. The form collects 
                // input for the custom message.
                /*
                inputForm = new UserInputForm();

                //Set a value on inputForm
                inputForm.CSProjPath = csProj;

                //Show the form
                inputForm.ShowDialog();
                */

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
            IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");



            //Get Component Model
            //http://docs.nuget.org/docs/reference/extensibility-apis
            //http://docs.nuget.org/docs/reference/invoking-nuget-services-from-inside-visual-studio
            //https://nuget.codeplex.com/discussions/246688
            //var componentModel = (IComponentModel)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SComponentModel));
            var componentModel = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SComponentModel)) as IComponentModel;

            if (componentModel != null)
            {
                //Get Nuget Extensibilty points
                var nugetEvents = componentModel.GetService<IVsPackageInstallerEvents>();
                var installer   = componentModel.GetService<IVsPackageInstaller>();
                var uninstaller = componentModel.GetService<IVsPackageUninstaller>();
                var packageMeta = componentModel.GetService<IVsPackageMetadata>();
                var packageInfo = componentModel.GetService<IVsPackageInstallerServices>();


                //Update IDE status bar bottom left
                _dte.StatusBar.Text = "Umbraco New Project - Installing Nuget Packages";

                //Wire up events
                nugetEvents.PackageReferenceAdded += nugetEvents_PackageReferenceAdded;
                nugetEvents.PackageInstalling += nugetEvents_PackageInstalling;
                nugetEvents.PackageInstalled += nugetEvents_PackageInstalled;

                //Download and unzip the package/s - Gets all the dependcies needed as well
                installer.InstallPackage(repo, project, packageID, null, false, false);
            }
        }

        void nugetEvents_PackageInstalled(IVsPackageMetadata metadata)
        {
            //Debug
            Debug.WriteLine("Umbraco New Project - Nuget PackageInstalled()");

            //Update IDE status bar bottom left
            _dte.StatusBar.Text = string.Format("Installed Nuget {0} {1}", metadata.Title, metadata.VersionString);
        }

        void nugetEvents_PackageInstalling(IVsPackageMetadata metadata)
        {
            //Debug
            Debug.WriteLine("Umbraco New Project - Nuget PackageInstalling()");

            //Update IDE status bar bottom left
            _dte.StatusBar.Text = string.Format("Installing Nuget {0} {1}", metadata.Title, metadata.VersionString);
        }

        void nugetEvents_PackageReferenceAdded(IVsPackageMetadata metadata)
        {
            //Debug
            Debug.WriteLine("Umbraco New Project - Nuget PackageReferenceAdded()");

            //Update IDE status bar bottom left
            _dte.StatusBar.Text = string.Format("Adding Nuget to Project {0} {1}", metadata.Title, metadata.VersionString);
        }
    }
}
