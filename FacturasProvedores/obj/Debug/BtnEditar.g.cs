﻿#pragma checksum "..\..\BtnEditar.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "B2E148DDF5F2F054DE2D00B3AAD9A18305EA3FB8BA1C9D400BAA83D351C2858E"
//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

using FacturasProvedores;
using Syncfusion;
using Syncfusion.UI.Xaml.Controls.DataPager;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.UI.Xaml.Grid.RowFilter;
using Syncfusion.UI.Xaml.TreeGrid;
using Syncfusion.Windows;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;
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


namespace FacturasProvedores {
    
    
    /// <summary>
    /// BtnEditar
    /// </summary>
    public partial class BtnEditar : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 39 "..\..\BtnEditar.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DatePicker TxFecIni;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\BtnEditar.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DatePicker TxFecFin;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\BtnEditar.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnConsultar;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\BtnEditar.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnChange;
        
        #line default
        #line hidden
        
        
        #line 53 "..\..\BtnEditar.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Syncfusion.UI.Xaml.Grid.SfDataGrid dataGrid;
        
        #line default
        #line hidden
        
        
        #line 69 "..\..\BtnEditar.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TxTotal;
        
        #line default
        #line hidden
        
        
        #line 73 "..\..\BtnEditar.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnSelect;
        
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
            System.Uri resourceLocater = new System.Uri("/FacturasProvedores;component/btneditar.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\BtnEditar.xaml"
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
            
            #line 8 "..\..\BtnEditar.xaml"
            ((FacturasProvedores.BtnEditar)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.TxFecIni = ((System.Windows.Controls.DatePicker)(target));
            return;
            case 3:
            this.TxFecFin = ((System.Windows.Controls.DatePicker)(target));
            return;
            case 4:
            this.BtnConsultar = ((System.Windows.Controls.Button)(target));
            
            #line 45 "..\..\BtnEditar.xaml"
            this.BtnConsultar.Click += new System.Windows.RoutedEventHandler(this.BtnConsultar_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.BtnChange = ((System.Windows.Controls.Button)(target));
            
            #line 46 "..\..\BtnEditar.xaml"
            this.BtnChange.Click += new System.Windows.RoutedEventHandler(this.BtnChange_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.dataGrid = ((Syncfusion.UI.Xaml.Grid.SfDataGrid)(target));
            return;
            case 7:
            this.TxTotal = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 8:
            this.BtnSelect = ((System.Windows.Controls.Button)(target));
            
            #line 73 "..\..\BtnEditar.xaml"
            this.BtnSelect.Click += new System.Windows.RoutedEventHandler(this.BtnSelect_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

