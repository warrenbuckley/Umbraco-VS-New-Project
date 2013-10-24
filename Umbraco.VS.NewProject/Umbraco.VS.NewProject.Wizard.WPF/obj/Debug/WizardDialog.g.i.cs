﻿#pragma checksum "..\..\WizardDialog.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "541E0C8227CC8800FD5BB2284E4D2CD8"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34003
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Umbraco.VS.NewProject.Wizard.WPF {
    
    
    /// <summary>
    /// WizardDialog
    /// </summary>
    public partial class WizardDialog : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 12 "..\..\WizardDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.GroupBox engineChoiceGroup;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\WizardDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox engineChoice;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\WizardDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button CreateProjectBtn;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\WizardDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.StatusBar statusBar;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\WizardDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.StatusBarItem umbracoVersionStatusBar;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\WizardDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.GroupBox databaseGroup;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Umbraco.VS.NewProject.Wizard;component/wizarddialog.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\WizardDialog.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.engineChoiceGroup = ((System.Windows.Controls.GroupBox)(target));
            return;
            case 2:
            this.engineChoice = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 3:
            this.CreateProjectBtn = ((System.Windows.Controls.Button)(target));
            
            #line 23 "..\..\WizardDialog.xaml"
            this.CreateProjectBtn.Click += new System.Windows.RoutedEventHandler(this.CreateProjectClick);
            
            #line default
            #line hidden
            return;
            case 4:
            this.statusBar = ((System.Windows.Controls.Primitives.StatusBar)(target));
            return;
            case 5:
            this.umbracoVersionStatusBar = ((System.Windows.Controls.Primitives.StatusBarItem)(target));
            return;
            case 6:
            this.databaseGroup = ((System.Windows.Controls.GroupBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

