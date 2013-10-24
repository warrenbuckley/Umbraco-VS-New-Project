using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;
using Microsoft.VisualStudio.Shell;
using NuGet.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using NuGet;

namespace Umbraco.VS.NewProject.Wizard
{
    public class Wizard : IWizard
    {
        private UserInputForm inputForm;
        private string dbConnectionString;
        private string templateEnginge;
        private string dbType;

        private IComponentModel serviceHost;
        private string rootSolutionDirectory;

        /// <summary>
        /// Runs custom wizard logic at the beginning of a template wizard run.
        /// </summary>
        /// <param name="automationObject">The automation object being used by the template wizard.</param>
        /// <param name="replacementsDictionary">The list of standard parameters to be replaced.</param>
        /// <param name="runKind">A <see cref="T:Microsoft.VisualStudio.TemplateWizard.WizardRunKind"/> indicating the type of wizard run.</param>
        /// <param name="customParams">The custom parameters with which to perform parameter replacement in the project.</param>
        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            if (serviceHost == null) serviceHost = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));
            rootSolutionDirectory = replacementsDictionary["$solutiondirectory$"];
            MessageBox.Show("RunStarted()");
        }


        /// <summary>
        /// Runs custom wizard logic when a project has finished generating.
        /// </summary>
        /// <param name="project">The project that finished generating.</param>
        public void ProjectFinishedGenerating(EnvDTE.Project project)
        {
            var factory = serviceHost.GetService<IVsPackageManagerFactory>();
            var manager = factory.CreatePackageManager();
            var package = manager.SourceRepository.FindPackage("UmbracoCms");

            manager.InstallPackage(
                new List<Project>() { project },
                package,
                new List<PackageOperation> { new PackageOperation(package, PackageAction.Install) },
                false, false, NuGet.NullLogger.Instance, null);

            try
            {
                var csProj = project.FileName;

                // Display a form to the user. The form collects 
                // input for the custom message.
                inputForm = new UserInputForm();

                //Set a value on inputForm
                inputForm.CSProjPath = csProj;

                //Show the form
                inputForm.ShowDialog();

                //Fetch value from input form
                dbConnectionString  = inputForm.get_dbConnectionString();
                dbType              = inputForm.get_dbType();
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
            //MessageBox.Show("ProjectItemFinishedGenerating()");
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
            //MessageBox.Show("BeforeOpeningFile()");
        }

        /// <summary>
        /// Runs custom wizard logic when the wizard has completed all tasks.
        /// </summary>
        public void RunFinished()
        {
            //MessageBox.Show("Run Finished()");
        }
    }
}
