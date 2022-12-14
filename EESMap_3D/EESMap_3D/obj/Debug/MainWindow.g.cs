#pragma checksum "..\..\MainWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "A0EC6D3507BE8A4385BE230643B01D08D30CCBF14B5BC33A04340054D5282B0A"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using EESMap_3D;
using HelixToolkit.Wpf;
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


namespace EESMap_3D {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 10 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal EESMap_3D.MainWindow EES_Map_MainWindow;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid ContentGrid;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid viewPortContainer;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal HelixToolkit.Wpf.HelixViewport3D mapViewPort;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.Media3D.PerspectiveCamera mainCam;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.Media3D.Transform3DGroup rotTGroup;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.Media3D.RotateTransform3D rotX;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.Media3D.AxisAngleRotation3D camRotationX;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.Media3D.RotateTransform3D rotY;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.Media3D.AxisAngleRotation3D camRotationY;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.Media3D.AmbientLight ambientLight;
        
        #line default
        #line hidden
        
        
        #line 48 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.Media3D.ModelVisual3D entityModel;
        
        #line default
        #line hidden
        
        
        #line 50 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.Media3D.GeometryModel3D mapEnvironment;
        
        #line default
        #line hidden
        
        
        #line 59 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.Media3D.MeshGeometry3D mapMesh;
        
        #line default
        #line hidden
        
        
        #line 68 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.Media3D.DiffuseMaterial matDiffuseMap;
        
        #line default
        #line hidden
        
        
        #line 81 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid Options;
        
        #line default
        #line hidden
        
        
        #line 97 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox ShowNodes;
        
        #line default
        #line hidden
        
        
        #line 98 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox ShowSWs;
        
        #line default
        #line hidden
        
        
        #line 99 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox ShowSubs;
        
        #line default
        #line hidden
        
        
        #line 100 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox InactiveSW;
        
        #line default
        #line hidden
        
        
        #line 101 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox MarkClosedSWs;
        
        #line default
        #line hidden
        
        
        #line 102 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox ApplyResistanceColor;
        
        #line default
        #line hidden
        
        
        #line 103 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image RestartPosition;
        
        #line default
        #line hidden
        
        
        #line 106 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TooltipLbl;
        
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
            System.Uri resourceLocater = new System.Uri("/EESMap_3D;component/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\MainWindow.xaml"
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
            this.EES_Map_MainWindow = ((EESMap_3D.MainWindow)(target));
            return;
            case 2:
            this.ContentGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 3:
            this.viewPortContainer = ((System.Windows.Controls.Grid)(target));
            return;
            case 4:
            this.mapViewPort = ((HelixToolkit.Wpf.HelixViewport3D)(target));
            
            #line 17 "..\..\MainWindow.xaml"
            this.mapViewPort.MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.MapViewPort_MouseWheel);
            
            #line default
            #line hidden
            
            #line 17 "..\..\MainWindow.xaml"
            this.mapViewPort.MouseMove += new System.Windows.Input.MouseEventHandler(this.MapViewPort_MouseMove);
            
            #line default
            #line hidden
            
            #line 17 "..\..\MainWindow.xaml"
            this.mapViewPort.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.MapViewPort_MouseLeftButtonDown);
            
            #line default
            #line hidden
            
            #line 18 "..\..\MainWindow.xaml"
            this.mapViewPort.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.MapViewPort_MouseLeftButtonUp);
            
            #line default
            #line hidden
            
            #line 18 "..\..\MainWindow.xaml"
            this.mapViewPort.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.MapViewPort_MouseDown);
            
            #line default
            #line hidden
            
            #line 18 "..\..\MainWindow.xaml"
            this.mapViewPort.MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.MapViewPort_MouseUp);
            
            #line default
            #line hidden
            
            #line 19 "..\..\MainWindow.xaml"
            this.mapViewPort.MouseRightButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.MapViewPort_MouseRightButtonDown);
            
            #line default
            #line hidden
            return;
            case 5:
            this.mainCam = ((System.Windows.Media.Media3D.PerspectiveCamera)(target));
            return;
            case 6:
            this.rotTGroup = ((System.Windows.Media.Media3D.Transform3DGroup)(target));
            return;
            case 7:
            this.rotX = ((System.Windows.Media.Media3D.RotateTransform3D)(target));
            return;
            case 8:
            this.camRotationX = ((System.Windows.Media.Media3D.AxisAngleRotation3D)(target));
            return;
            case 9:
            this.rotY = ((System.Windows.Media.Media3D.RotateTransform3D)(target));
            return;
            case 10:
            this.camRotationY = ((System.Windows.Media.Media3D.AxisAngleRotation3D)(target));
            return;
            case 11:
            this.ambientLight = ((System.Windows.Media.Media3D.AmbientLight)(target));
            return;
            case 12:
            this.entityModel = ((System.Windows.Media.Media3D.ModelVisual3D)(target));
            return;
            case 13:
            this.mapEnvironment = ((System.Windows.Media.Media3D.GeometryModel3D)(target));
            return;
            case 14:
            this.mapMesh = ((System.Windows.Media.Media3D.MeshGeometry3D)(target));
            return;
            case 15:
            this.matDiffuseMap = ((System.Windows.Media.Media3D.DiffuseMaterial)(target));
            return;
            case 16:
            this.Options = ((System.Windows.Controls.Grid)(target));
            return;
            case 17:
            this.ShowNodes = ((System.Windows.Controls.CheckBox)(target));
            
            #line 97 "..\..\MainWindow.xaml"
            this.ShowNodes.Checked += new System.Windows.RoutedEventHandler(this.ShowNodes_Checked);
            
            #line default
            #line hidden
            
            #line 97 "..\..\MainWindow.xaml"
            this.ShowNodes.Unchecked += new System.Windows.RoutedEventHandler(this.ShowNodes_Unchecked);
            
            #line default
            #line hidden
            return;
            case 18:
            this.ShowSWs = ((System.Windows.Controls.CheckBox)(target));
            
            #line 98 "..\..\MainWindow.xaml"
            this.ShowSWs.Checked += new System.Windows.RoutedEventHandler(this.ShowSWs_Checked);
            
            #line default
            #line hidden
            
            #line 98 "..\..\MainWindow.xaml"
            this.ShowSWs.Unchecked += new System.Windows.RoutedEventHandler(this.ShowSWs_Unchecked);
            
            #line default
            #line hidden
            return;
            case 19:
            this.ShowSubs = ((System.Windows.Controls.CheckBox)(target));
            
            #line 99 "..\..\MainWindow.xaml"
            this.ShowSubs.Checked += new System.Windows.RoutedEventHandler(this.ShowSubs_Checked);
            
            #line default
            #line hidden
            
            #line 99 "..\..\MainWindow.xaml"
            this.ShowSubs.Unchecked += new System.Windows.RoutedEventHandler(this.ShowSubs_Unchecked);
            
            #line default
            #line hidden
            return;
            case 20:
            this.InactiveSW = ((System.Windows.Controls.CheckBox)(target));
            
            #line 100 "..\..\MainWindow.xaml"
            this.InactiveSW.Checked += new System.Windows.RoutedEventHandler(this.InactiveSW_Checked);
            
            #line default
            #line hidden
            
            #line 100 "..\..\MainWindow.xaml"
            this.InactiveSW.Unchecked += new System.Windows.RoutedEventHandler(this.InactiveSW_Unchecked);
            
            #line default
            #line hidden
            return;
            case 21:
            this.MarkClosedSWs = ((System.Windows.Controls.CheckBox)(target));
            
            #line 101 "..\..\MainWindow.xaml"
            this.MarkClosedSWs.Checked += new System.Windows.RoutedEventHandler(this.MarkClosedSWs_Checked);
            
            #line default
            #line hidden
            
            #line 101 "..\..\MainWindow.xaml"
            this.MarkClosedSWs.Unchecked += new System.Windows.RoutedEventHandler(this.MarkClosedSWs_Unchecked);
            
            #line default
            #line hidden
            return;
            case 22:
            this.ApplyResistanceColor = ((System.Windows.Controls.CheckBox)(target));
            
            #line 102 "..\..\MainWindow.xaml"
            this.ApplyResistanceColor.Checked += new System.Windows.RoutedEventHandler(this.ApplyResistanceColor_Checked);
            
            #line default
            #line hidden
            
            #line 102 "..\..\MainWindow.xaml"
            this.ApplyResistanceColor.Unchecked += new System.Windows.RoutedEventHandler(this.ApplyResistanceColor_Unchecked);
            
            #line default
            #line hidden
            return;
            case 23:
            this.RestartPosition = ((System.Windows.Controls.Image)(target));
            
            #line 103 "..\..\MainWindow.xaml"
            this.RestartPosition.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.RestartPosition_MouseDown);
            
            #line default
            #line hidden
            return;
            case 24:
            this.TooltipLbl = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

