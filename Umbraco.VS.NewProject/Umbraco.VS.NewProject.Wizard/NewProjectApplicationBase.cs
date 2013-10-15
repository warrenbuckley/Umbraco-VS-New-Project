using System;
using Umbraco.Core;

namespace Umbraco.VS.NewProject.Wizard {
    
    /// <summary>
    /// Extends the UmbracoApplicationBase, which is needed to start the application with out own BootManager.
    /// </summary>
    public class NewProjectApplicationBase : UmbracoApplicationBase {
        
        protected override IBootManager GetBootManager() {
            return new NewProjectBootManager(this);
        }

        public void Start(object sender, EventArgs e) {
            Application_Start(sender, e);
        }
    
    }

}
