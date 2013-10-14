﻿using Umbraco.Core;
using umbraco.editorControls;
using umbraco.interfaces;

namespace Umbraco.VS.NewProject.Wizard {

    /// <summary>
    /// Extends the CoreBootManager for use in this Console app.
    /// </summary>
    public class NewProjectBootManager : CoreBootManager {
            
        public NewProjectBootManager(UmbracoApplicationBase umbracoApplication) : base(umbracoApplication) {
            
            //This is only here to ensure references to the assemblies needed for the DataTypesResolver
            //otherwise they won't be loaded into the AppDomain.
            var interfacesAssemblyName = typeof(IDataType).Assembly.FullName;
            var editorControlsAssemblyName = typeof(uploadField).Assembly.FullName;
        
        }

        /// <summary>
        /// Can be used to initialize our own Application Events
        /// </summary>
        protected override void InitializeApplicationEventsResolver() {
            base.InitializeApplicationEventsResolver();
        }

        /// <summary>
        /// Can be used to add custom resolvers or overwrite existing resolvers once they are made public
        /// </summary>
        protected override void InitializeResolvers() {
            base.InitializeResolvers();
        }
    
    }

}
