using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TemplateWizard;
using System.Windows.Forms;
using EnvDTE;
using NuGet;
using Umbraco.Core;
using umbraco.presentation.install.utills;

namespace Umbraco.VS.NewProject.Wizard
{
    public class Wizard : IWizard
    {
        private UserInputForm inputForm;
        private DTE _dte;


        /// <summary>
        /// Runs custom wizard logic at the beginning of a template wizard run.
        /// </summary>
        /// <param name="automationObject">The automation object being used by the template wizard.</param>
        /// <param name="replacementsDictionary">The list of standard parameters to be replaced.</param>
        /// <param name="runKind">A <see cref="T:Microsoft.VisualStudio.TemplateWizard.WizardRunKind"/> indicating the type of wizard run.</param>
        /// <param name="customParams">The custom parameters with which to perform parameter replacement in the project.</param>
        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            _dte = (DTE)automationObject;

            Debug.Write("Umbraco New Project - RunStarted() event");
        }

        /// <summary>
        /// Runs custom wizard logic when a project has finished generating.
        /// </summary>
        /// <param name="project">The project that finished generating.</param>
        public void ProjectFinishedGenerating(Project project)
        {
            Debug.Write("Umbraco New Project - ProjectFinishedGenerating() event");

            try
            {
                var csProj          = project.FileName;
                var projectPath     = Path.GetDirectoryName(csProj);
                var solutionPath    = Path.GetDirectoryName(projectPath);

                //Go Get Umbraco from Nuget
                GetUmbraco(solutionPath, project);

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
            }
        }


        /// <summary>
        /// Runs custom wizard logic when a project item has finished generating.
        /// </summary>
        /// <param name="projectItem">The project item that finished generating.</param>
        public void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
            Debug.Write("Umbraco New Project - ProjectItemFinishedGenerating() event");
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
            Debug.Write("Umbraco New Project - BeforeOpeningFile() event");
        }

        /// <summary>
        /// Runs custom wizard logic when the wizard has completed all tasks.
        /// </summary>
        public void RunFinished()
        {
            Debug.Write("Umbraco New Project - RunFinished() event");
        }


        public void GetUmbraco(string solutionPath, Project project)
        {
            //Update IDE status bar bottom left
            _dte.StatusBar.Text = "Umbraco New Project - Getting Umbraco (Please Wait)";

            //Debug
            Debug.Write("Umbraco New Project - GetUmbraco() event");
            Debug.Write("FilePath: " + solutionPath);

            //Get Packages folder at solution level
            var packagePath = Path.Combine(solutionPath, "packages");


            //ID of the package to be looked up
            string packageID = "UmbracoCMS";

            //Connect to the official package repository
            IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");

            //Get the list of all NuGet packages with ID 'UmbracoCMS'       
            List<IPackage> packages = repo.FindPackagesById(packageID).ToList();

            //Get the latest Umbraco Nuget but only release (not pre-release, alpha stuff)
            var umbracoNuget = packages.SingleOrDefault(x => x.IsLatestVersion && x.IsReleaseVersion());

            //Initialize the package manager
            var packageManager = new PackageManager(repo, solutionPath);

            //Hook up Nuget events
            packageManager.PackageInstalled += packageManager_PackageInstalled;
            packageManager.PackageInstalling += packageManager_PackageInstalling;

            //Download and unzip the package/s
            //Gets all the dependcies needed as well
            if (umbracoNuget != null)
            {
                packageManager.InstallPackage(umbracoNuget, false, false);
            }
        }

        /// <summary>
        /// Package Installing from Nuget event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void packageManager_PackageInstalling(object sender, PackageOperationEventArgs e)
        {
            var package = e.Package;
            Debug.WriteLine("Umbraco New Project - PackageInstalling() event");
            Debug.WriteLine(string.Format("Installing {0} {1}", package.Title, package.Version.ToString()));
            Debug.WriteLine(string.Empty);

            //Update IDE status bar bottom left
            _dte.StatusBar.Text = string.Format("Installing {0} {1}", package.Title, package.Version.ToString());
        }

        /// <summary>
        /// Package Installed from Nuget event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void packageManager_PackageInstalled(object sender, PackageOperationEventArgs e)
        {
            var package     = e.Package;
            var fileSystem  = e.FileSystem;
            var installPath = e.InstallPath;
            var targetPath  = e.TargetPath;

            var packageContentFiles = e.Package.GetContentFiles();
            var packageFiles        = e.Package.GetFiles();

            Debug.WriteLine("Umbraco New Project - PackageInstalled() event");
            Debug.WriteLine(string.Format("Installed {0} {1}", package.Title, package.Version.ToString()));
            Debug.WriteLine(string.Empty);

            /*
            string localSource = Path.Combine(targetPath, "packages");

            IPackageRepository sourceRepo       =  PackageRepositoryFactory.Default.CreateRepository(new PackageSource("https://packages.nuget.org/api/v2"));
            IPackagePathResolver pathResolver   = new DefaultPackagePathResolver(localSource);
            IProjectSystem project;
            IPackageRepository localRepo        = PackageRepositoryFactory.Default.CreateRepository(new PackageSource(localSource));


            //Add nuget into project
            ProjectManager projectManager = new ProjectManager(sourceRepo, pathResolver, project, localRepo);

            if (projectManager != null)
            {
                //Add package to project
                projectManager.AddPackageReference(package, true, false);  
            }
            */
        }
    }
}
