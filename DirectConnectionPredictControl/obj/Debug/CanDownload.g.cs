﻿#pragma checksum "..\..\CanDownload.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "3DA2A23622218077A7F104D3670B2575FC000195"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using DirectConnectionPredictControl;
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


namespace DirectConnectionPredictControl {
    
    
    /// <summary>
    /// CanDownload
    /// </summary>
    public partial class CanDownload : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 30 "..\..\CanDownload.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button miniumBtn;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\CanDownload.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button closeBtn;
        
        #line default
        #line hidden
        
        
        #line 49 "..\..\CanDownload.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button startDownloadBtn;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\CanDownload.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button cancelDownloadBtn;
        
        #line default
        #line hidden
        
        
        #line 80 "..\..\CanDownload.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button openCanBtn;
        
        #line default
        #line hidden
        
        
        #line 83 "..\..\CanDownload.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button closeCanBtn;
        
        #line default
        #line hidden
        
        
        #line 96 "..\..\CanDownload.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button chooseFileBtn;
        
        #line default
        #line hidden
        
        
        #line 108 "..\..\CanDownload.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal DirectConnectionPredictControl.ComboBoxControl boundRateCbx;
        
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
            System.Uri resourceLocater = new System.Uri("/DirectConnectionPredictControl;component/candownload.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\CanDownload.xaml"
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
            
            #line 15 "..\..\CanDownload.xaml"
            ((System.Windows.Controls.Grid)(target)).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Grid_MouseLeftButtonDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.miniumBtn = ((System.Windows.Controls.Button)(target));
            
            #line 30 "..\..\CanDownload.xaml"
            this.miniumBtn.Click += new System.Windows.RoutedEventHandler(this.miniumBtn_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.closeBtn = ((System.Windows.Controls.Button)(target));
            
            #line 31 "..\..\CanDownload.xaml"
            this.closeBtn.Click += new System.Windows.RoutedEventHandler(this.closeBtn_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.startDownloadBtn = ((System.Windows.Controls.Button)(target));
            
            #line 49 "..\..\CanDownload.xaml"
            this.startDownloadBtn.Click += new System.Windows.RoutedEventHandler(this.startDownloadBtn_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.cancelDownloadBtn = ((System.Windows.Controls.Button)(target));
            
            #line 51 "..\..\CanDownload.xaml"
            this.cancelDownloadBtn.Click += new System.Windows.RoutedEventHandler(this.cancelDownloadBtn_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.openCanBtn = ((System.Windows.Controls.Button)(target));
            return;
            case 7:
            this.closeCanBtn = ((System.Windows.Controls.Button)(target));
            return;
            case 8:
            this.chooseFileBtn = ((System.Windows.Controls.Button)(target));
            
            #line 96 "..\..\CanDownload.xaml"
            this.chooseFileBtn.Click += new System.Windows.RoutedEventHandler(this.chooseFileBtn_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.boundRateCbx = ((DirectConnectionPredictControl.ComboBoxControl)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

