using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Threading;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using System.Threading;
using EESMap_3D.Model;
using System.Xml;
using EESMap_3D.Engine;
using System.Collections;
using System.Reflection;


namespace EESMap_3D
{
    
    public partial class MainWindow : Window
    {
        #region Consts
        private readonly double __ZOOM_OUT_MAX_COEF = 2.0;
        double __CUBE_DIM = 2000;
        #endregion

        #region Fields
        private enum E3DObjManipulation { PAN=0, ROTATE, ZOOM, NONE };
        private enum ELineEnters { FRONT = 0, BACK, LEFT, RIGHT, INN, NONE };

        private Point start = new Point(0,0);
        private System.Windows.Media.Media3D.Point3D defaultCamPosition = new System.Windows.Media.Media3D.Point3D();
        private E3DObjManipulation manipulation     = E3DObjManipulation.NONE;
        private double currZoomCoef     = 1;
        private Point dispToObjRatio    = new Point();
        private Storyboard sboardRot    = new Storyboard();
        private DoubleAnimation danimX  = null, 
                                danimY  = null;

        double  cumulativeXRot = 0, 
                cumulativeYRot = 0;

        private List<Switch> switches            = null;
        private List<Node> nodes                 = null;
        private List<Substation> substations     = null;
        private List<EESMap_3D.Model.Line> lines = null;
        private List<UIEntityAdapter> entitiesUI = null;
        private Tuple<double, double> coordCorrection = null;
        private GeometryModel3D hitgeo;
        private ArrayList models = new ArrayList();

        Dictionary<string, SolidColorBrush> entityColors;
        int tooltipedEntityIdx = -1;
        int initialCheckFlag =3;

        #endregion
        public MainWindow()
        {
            InitializeComponent();
            LoadMapEnvironment();
        }

        //ima 
        private void LoadMapEnvironment()
        {
            ShowNodes.SetValue(CheckBox.IsCheckedProperty, true);                      
            ShowSWs.SetValue(CheckBox.IsCheckedProperty, true);
            ShowSubs.SetValue(CheckBox.IsCheckedProperty, true);

         //   var mapImg = new BitmapImage( new Uri(System.IO.Path.Combine(Environment.CurrentDirectory, "map.jpg"), UriKind.RelativeOrAbsolute));
          //  var mapWidth = mapImg.Width;
          //  var mapHeight = mapImg.Height;

            //Make 2D geometry sizeof map image
           // var mapPositions = new System.Windows.Media.Media3D.Point3DCollection();
           // mapPositions.Add(new System.Windows.Media.Media3D.Point3D() { X=0, Y=0, Z=0});
           //mapPositions.Add(new System.Windows.Media.Media3D.Point3D() { X = mapWidth*1000, Y = 0, Z = 0 });
           // mapPositions.Add(new System.Windows.Media.Media3D.Point3D() { X = 0, Y = mapHeight*1000, Z = 0 });
           // mapPositions.Add(new System.Windows.Media.Media3D.Point3D() { X = mapWidth*1000, Y = mapHeight*1000, Z = 0 });         
           // mapMesh.Positions = mapPositions;
           // matDiffuseMap.Brush = new ImageBrush() { ImageSource = mapImg };
            mapViewPort.Loaded += (sender, e) => DissableHelixOptions();
            mapViewPort.Loaded += (sender, e) => DoDisplayFit();
            mapViewPort.Loaded += (sender, e) => LoadEntities();
            mapViewPort.Loaded += (sender, e) => CalculateFitParameters();
            mapViewPort.Loaded += (sender, e) => PlotEntities();
        }
        
        //Ovdje racuna prvo razliku izmedju nablizeg X i najdaljeg, isto to i za Y...... onda posto ima ModelVideo3D koji ima svoje kordinate gleda omjer da pocev od 0 do 20000 i za 
        //X i za Y i onda dijeli te kordinate sa omjerom X min i Xmax u realonm svijetu pa ce u ... MA OBJASNITI UZIVO JEBEM TI DAN
        private void CalculateFitParameters()
        {
            var xSpan = EntityLoader.GetEnvironmentLimit_UppRight_Converted().Item1 - EntityLoader.GetEnvironmentLimit_BtmLeft_Converted().Item1;//razlika po X-u 19.nesto
            var ySpan = EntityLoader.GetEnvironmentLimit_UppRight_Converted().Item2 - EntityLoader.GetEnvironmentLimit_BtmLeft_Converted().Item2;//razlika po Y-u 45.nest

            //Odnos velicina mape i opsega koordinata
            coordCorrection = new Tuple<double, double>
                (   
                    entityModel.Content.Bounds.SizeX / xSpan,//entityModel je ModelVisual3D i onda on ima svoje X
                    entityModel.Content.Bounds.SizeY / ySpan//entityModel je ModelVisual3D i onda on ima svoje Y
                 );
        }

        //ZATO STO IMA HELIX umjesto VIEWPORT3d, a helix ima odradjene funkcije a on ih je samo disablovo da mu ne seru
        private void DissableHelixOptions()
        {
            mapViewPort.SetValue(HelixToolkit.Wpf.HelixViewport3D.IsZoomEnabledProperty,         false);
            mapViewPort.SetValue(HelixToolkit.Wpf.HelixViewport3D.IsMoveEnabledProperty,         false);
            mapViewPort.SetValue(HelixToolkit.Wpf.HelixViewport3D.IsPanEnabledProperty,          false);
            mapViewPort.SetValue(HelixToolkit.Wpf.HelixViewport3D.IsRotationEnabledProperty,     false);
            mapViewPort.SetValue(HelixToolkit.Wpf.HelixViewport3D.ZoomExtentsWhenLoadedProperty, false);

            //Prevent from intercepting events
            //mapViewPort.IsViewCubeEdgeClicksEnabled = false;
            var dummyGesture = new MouseGesture(MouseAction.None);
            mapViewPort.BackViewGesture             = dummyGesture;
            mapViewPort.BottomViewGesture           = dummyGesture;
            mapViewPort.ChangeFieldOfViewGesture    = dummyGesture;
            mapViewPort.ChangeLookAtGesture         = dummyGesture;
            mapViewPort.FrontViewGesture            = dummyGesture;
            mapViewPort.LeftViewGesture             = dummyGesture;
            mapViewPort.OrthographicToggleGesture   = dummyGesture;
            mapViewPort.PanGesture                  = dummyGesture;
            mapViewPort.PanGesture2                 = dummyGesture;
            mapViewPort.RotateGesture               = dummyGesture;
            mapViewPort.RotateGesture2              = dummyGesture;
            mapViewPort.ResetCameraGesture          = dummyGesture;
            mapViewPort.ResetCameraKeyGesture       = new KeyGesture(Key.None);
            mapViewPort.RightViewGesture            = dummyGesture;
            mapViewPort.ZoomExtentsGesture          = dummyGesture;
            mapViewPort.ZoomGesture                 = dummyGesture;
            mapViewPort.ZoomGesture2                = dummyGesture;
            mapViewPort.ZoomRectangleGesture        = dummyGesture;
        }

        //izracunam omjer opsega koordinata modela i ekrana, ma to ni ne treba
        private void DoDisplayFit()
        {
            var zoomBase = Math.Min(entityModel.Content.Bounds.SizeX, entityModel.Content.Bounds.SizeY)/1000;
            var imgDiagHlf = Math.Sqrt(2) * zoomBase / 2;
            var fieldHyp = imgDiagHlf / Math.Sin(mainCam.FieldOfView*(Math.PI/180.0)/2);
            var camDist = Math.Sqrt(Math.Pow(fieldHyp,2) - Math.Pow(imgDiagHlf, 2));

            mainCam.Position = new System.Windows.Media.Media3D.Point3D()
            {
                X = mainCam.Position.X,
                Y = mainCam.Position.Y,
                Z = camDist*1000
            };

            defaultCamPosition.X = mainCam.Position.X;
            defaultCamPosition.Y = mainCam.Position.Y;
            defaultCamPosition.Z = mainCam.Position.Z;

            danimX = new DoubleAnimation()
            {
                IsAdditive = true,
                IsCumulative = false,
                By = 1,
                Duration = new Duration(TimeSpan.FromMilliseconds(70))
            };
            NameScope.SetNameScope(mapViewPort, new NameScope());
            mapViewPort.RegisterName("camRotationX", camRotationX);
            sboardRot.Children.Add(danimX);
            Storyboard.SetTargetName(danimX, "camRotationX");
            Storyboard.SetTargetProperty(danimX, new PropertyPath(AxisAngleRotation3D.AngleProperty));

            danimY = new DoubleAnimation()
            {
                IsAdditive = true,
                IsCumulative = false,
                By = 1,
                Duration = danimX.Duration
            };
            mapViewPort.RegisterName("camRotationY", camRotationY);
            sboardRot.Children.Add(danimY);
            Storyboard.SetTargetName(danimY, "camRotationY");
            Storyboard.SetTargetProperty(danimY, new PropertyPath(AxisAngleRotation3D.AngleProperty));

            rotX.CenterX = entityModel.Content.Bounds.SizeX / 2;
            rotX.CenterY = entityModel.Content.Bounds.SizeY / 2;
            rotY.CenterX = rotX.CenterX;
            rotY.CenterY = rotX.CenterY;

            camRotationX.Axis = new Vector3D(0, 1, 0);
            camRotationY.Axis = new Vector3D(1, 0, 0);
          
            dispToObjRatio.X = mapViewPort.ActualWidth / entityModel.Content.Bounds.SizeX / 1000;
            dispToObjRatio.Y = mapViewPort.ActualHeight / entityModel.Content.Bounds.SizeY / 1000;
        }
        //OVDJE JE UCITAO ELEMENT
        private void LoadEntities()
        {
            var xmlDocument = new XmlDocument();
            var sourcePath = System.IO.Path.Combine(Environment.CurrentDirectory, "Geographic.xml");
            xmlDocument.Load(sourcePath);
            EntityLoader.LoadModelFrom(xmlDocument, out nodes, out substations, out switches, out lines);
            xmlDocument.Save(sourcePath);
            xmlDocument = null;
        }
        private ModelVisual3D GenerateVisual(UIEntityAdapter entity, double cubeDim, SolidColorBrush scb, List<Tuple<double, double>> overlays, ref List<int> overlayHeights)
        {
            ModelVisual3D entityVisual = new ModelVisual3D();
            GeometryModel3D entityGeometryModel = new GeometryModel3D();
            MeshGeometry3D entityMesh = new MeshGeometry3D();

            var targetIndex = overlays.FindIndex(x => x.Item1 == entity.Entity.X && x.Item2 == entity.Entity.Y);
            int heightOffset = targetIndex == -1 ? 0 : overlayHeights[targetIndex];


            //-__CUBE_DIM / 2               to set cube at the center of long lat line
            //+ heightOffset * __CUBE_DIM   for towering

            //    p6_______________p7
            //    /|               /|
            //   / |              / |
            //  p4_|____________p5  |
            //  |  |            |   |
            //  | p2____________|___p3
            //  | /             |  /
            //  |/              | /
            //  p0______________p1

            entityMesh.Positions = new Point3DCollection();
            {
                entityMesh.Positions.Add(new Point3D(-__CUBE_DIM / 2 + entity.ApproxedX,            -__CUBE_DIM / 2 + entity.ApproxedY,             0.0         + heightOffset * __CUBE_DIM));  //p0
                entityMesh.Positions.Add(new Point3D(-__CUBE_DIM / 2 + entity.ApproxedX + cubeDim,  -__CUBE_DIM / 2 + entity.ApproxedY,             0.0         + heightOffset * __CUBE_DIM));  //p1
                entityMesh.Positions.Add(new Point3D(-__CUBE_DIM / 2 + entity.ApproxedX,            -__CUBE_DIM / 2 + entity.ApproxedY + cubeDim,   0.0         + heightOffset * __CUBE_DIM));  //p2
                entityMesh.Positions.Add(new Point3D(-__CUBE_DIM / 2 + entity.ApproxedX + cubeDim,  -__CUBE_DIM / 2 + entity.ApproxedY + cubeDim,   0.0         + heightOffset * __CUBE_DIM));  //p3
                entityMesh.Positions.Add(new Point3D(-__CUBE_DIM / 2 + entity.ApproxedX,            -__CUBE_DIM / 2 + entity.ApproxedY,             cubeDim     + heightOffset * __CUBE_DIM));  //p4
                entityMesh.Positions.Add(new Point3D(-__CUBE_DIM / 2 + entity.ApproxedX + cubeDim,  -__CUBE_DIM / 2 + entity.ApproxedY,             cubeDim     + heightOffset * __CUBE_DIM));  //p5
                entityMesh.Positions.Add(new Point3D(-__CUBE_DIM / 2 + entity.ApproxedX,            -__CUBE_DIM / 2 + entity.ApproxedY + cubeDim,   cubeDim     + heightOffset * __CUBE_DIM));  //p6
                entityMesh.Positions.Add(new Point3D(-__CUBE_DIM / 2 + entity.ApproxedX + cubeDim,  -__CUBE_DIM / 2 + entity.ApproxedY + cubeDim,   cubeDim     + heightOffset * __CUBE_DIM));  //p7
            };

            entityMesh.TriangleIndices = new Int32Collection()
            {
                2, 3, 1,    3, 1, 0,
                7, 1, 3,    7, 5, 1,
                6, 5, 7,    6, 4, 5,
                6, 2, 0,    2, 0, 4,
                2, 7, 3,    2, 6, 7,
                0, 1, 5,    0, 5, 4
            };
            if (targetIndex!=-1)
                ++overlayHeights[targetIndex];

            entityVisual.Content = entityGeometryModel;
            entityGeometryModel.Geometry = entityMesh;
            entityGeometryModel.Material = new DiffuseMaterial() { Brush = scb };

            //entityMesh.Normals = new Vector3DCollection()
            //{
            //    new Vector3D(0, 0, 1),
            //    new Vector3D(0, 0, 1),
            //    new Vector3D(0, 0, 1),
            //    new Vector3D(0, 0, 1)
            //};

            return entityVisual;
        }

        //PRETVARA U lat i lon y - x 
        public static void ToLatLon(double utmX, double utmY, int zoneUTM, out double latitude, out double longitude)
        {
            bool isNorthHemisphere = true;

            var diflat = -0.00066286966871111111111111111111111111;
            var diflon = -0.0003868060578;

            var zone = zoneUTM;
            var c_sa = 6378137.000000;
            var c_sb = 6356752.314245;
            var e2 = Math.Pow((Math.Pow(c_sa, 2) - Math.Pow(c_sb, 2)), 0.5) / c_sb;
            var e2cuadrada = Math.Pow(e2, 2);
            var c = Math.Pow(c_sa, 2) / c_sb;
            var x = utmX - 500000;
            var y = isNorthHemisphere ? utmY : utmY - 10000000;

            var s = ((zone * 6.0) - 183.0);
            var lat = y / (c_sa * 0.9996);
            var v = (c / Math.Pow(1 + (e2cuadrada * Math.Pow(Math.Cos(lat), 2)), 0.5)) * 0.9996;
            var a = x / v;
            var a1 = Math.Sin(2 * lat);
            var a2 = a1 * Math.Pow((Math.Cos(lat)), 2);
            var j2 = lat + (a1 / 2.0);
            var j4 = ((3 * j2) + a2) / 4.0;
            var j6 = ((5 * j4) + Math.Pow(a2 * (Math.Cos(lat)), 2)) / 3.0;
            var alfa = (3.0 / 4.0) * e2cuadrada;
            var beta = (5.0 / 3.0) * Math.Pow(alfa, 2);
            var gama = (35.0 / 27.0) * Math.Pow(alfa, 3);
            var bm = 0.9996 * c * (lat - alfa * j2 + beta * j4 - gama * j6);
            var b = (y - bm) / v;
            var epsi = ((e2cuadrada * Math.Pow(a, 2)) / 2.0) * Math.Pow((Math.Cos(lat)), 2);
            var eps = a * (1 - (epsi / 3.0));
            var nab = (b * (1 - epsi)) + lat;
            var senoheps = (Math.Exp(eps) - Math.Exp(-eps)) / 2.0;
            var delt = Math.Atan(senoheps / (Math.Cos(nab)));
            var tao = Math.Atan(Math.Cos(delt) * Math.Tan(nab));

            longitude = ((delt * (180.0 / Math.PI)) + s) + diflon;
            latitude = ((lat + (1 + e2cuadrada * Math.Pow(Math.Cos(lat), 2) - (3.0 / 2.0) * e2cuadrada * Math.Sin(lat) * Math.Cos(lat) * (tao - lat)) * (tao - lat)) * (180.0 / Math.PI)) + diflat;
        }

        //UIAdaptCoords ti vrsi prevodjenje iz lat-lon u koordinate na samom ekranu
        public void UIAdaptCoords(double x, double y, out double approxedX, out double approxedY)
        {
            approxedX = (x - EntityLoader.GetEnvironmentLimit_BtmLeft_Converted().Item1) * coordCorrection.Item1;//ovo getEnvrLimit to treba istrazit
            approxedY = (y - EntityLoader.GetEnvironmentLimit_BtmLeft_Converted().Item2) * coordCorrection.Item2;
        }
        private ModelVisual3D GenerateLineSegment(UIEntityAdapter entity, double cubeDim, SolidColorBrush scb, List<Tuple<double, double>> overlays, ref List<int> overlayHeights)
        {
            ModelVisual3D entityVisual = new ModelVisual3D();
            GeometryModel3D entityGeometryModel = new GeometryModel3D();
            MeshGeometry3D entityMesh = new MeshGeometry3D();

           
            return entityVisual;
        }
        private void DetermineOrientation(double posX, double posY, double targX, double targY,ref ELineEnters lineEnters)
        {
            if (targX < posX)            //Connects from left 
                lineEnters = ELineEnters.LEFT;
            
            else if (targX > posX)      //Connects from right
                lineEnters = ELineEnters.RIGHT;
            
            else if (targY < posY)          //Connects from front

                lineEnters = ELineEnters.FRONT;
            
            else if (targY > posY)     ////Connects from back
                lineEnters = ELineEnters.BACK;

            else lineEnters = ELineEnters.INN;
        }
        private void GenerateLineSegmentStart(double cubeDim, Model.Line line, UIEntityAdapter startNode, UIEntityAdapter endNode, ref MeshGeometry3D entityMesh, ref ELineEnters lineEnters)
        {
            Point3D casted = new Point3D();
            if(line.Verticies.Count>0)
                 DetermineOrientation(startNode.ApproxedX, startNode.ApproxedY, line.Verticies[0].X, line.Verticies[0].Y, ref lineEnters);
            else
                DetermineOrientation(startNode.ApproxedX, startNode.ApproxedY, endNode.ApproxedX, endNode.ApproxedY, ref lineEnters);

            switch (lineEnters)
            {
                case ELineEnters.LEFT://Connects from left
                    {
                        entityMesh.Positions = new Point3DCollection();
                        casted = (startNode.Geometry.Geometry as MeshGeometry3D).Positions[2];
                        entityMesh.Positions.Add(new Point3D(casted.X, casted.Y, casted.Z));

                        casted = (startNode.Geometry.Geometry as MeshGeometry3D).Positions[0];
                        entityMesh.Positions.Add(new Point3D(casted.X + __CUBE_DIM / 2, casted.Y, casted.Z));

                        casted = (startNode.Geometry.Geometry as MeshGeometry3D).Positions[4];
                        entityMesh.Positions.Add(new Point3D(casted.X, casted.Y, casted.Z));
                        break;
                    }
                case ELineEnters.RIGHT://Connects from right
                    {
                        entityMesh.Positions = new Point3DCollection();
                        casted = (startNode.Geometry.Geometry as MeshGeometry3D).Positions[3];
                        entityMesh.Positions.Add(new Point3D(casted.X, casted.Y, casted.Z));

                        casted = (startNode.Geometry.Geometry as MeshGeometry3D).Positions[5];
                        entityMesh.Positions.Add(new Point3D(casted.X + __CUBE_DIM / 2, casted.Y, casted.Z));

                        casted = (startNode.Geometry.Geometry as MeshGeometry3D).Positions[1];
                        entityMesh.Positions.Add(new Point3D(casted.X, casted.Y, casted.Z));

                        entityMesh.TriangleIndices = new Int32Collection() { 0, 1, 2 };
                        break;
                    }
                case ELineEnters.FRONT: //Connects from front
                    {
                        entityMesh.Positions = new Point3DCollection();
                        casted = (startNode.Geometry.Geometry as MeshGeometry3D).Positions[0];
                        entityMesh.Positions.Add(new Point3D(casted.X, casted.Y, casted.Z));

                        casted = (startNode.Geometry.Geometry as MeshGeometry3D).Positions[1];
                        entityMesh.Positions.Add(new Point3D(casted.X, casted.Y, casted.Z));

                        casted = (startNode.Geometry.Geometry as MeshGeometry3D).Positions[4];
                        entityMesh.Positions.Add(new Point3D(casted.X + __CUBE_DIM / 2, casted.Y, casted.Z));

                        entityMesh.TriangleIndices = new Int32Collection() { 0, 1, 2 };
                        break;
                    }
                case ELineEnters.BACK: //Connects from back
                    {
                        entityMesh.Positions = new Point3DCollection();
                        casted = (startNode.Geometry.Geometry as MeshGeometry3D).Positions[2];
                        entityMesh.Positions.Add(new Point3D(casted.X, casted.Y, casted.Z));

                        casted = (startNode.Geometry.Geometry as MeshGeometry3D).Positions[6];
                        entityMesh.Positions.Add(new Point3D(casted.X + __CUBE_DIM / 2, casted.Y, casted.Z));

                        casted = (startNode.Geometry.Geometry as MeshGeometry3D).Positions[2];
                        entityMesh.Positions.Add(new Point3D(casted.X, casted.Y, casted.Z));

                        entityMesh.TriangleIndices = new Int32Collection() { 0, 1, 2 };
                        lineEnters = ELineEnters.BACK;
                        break;
                    }
                case ELineEnters.INN: { break; }
                default:
                    {throw new NotImplementedException("Bad orientation");}
            }           
        }       
        private void PlotEntities()
        {
            double x = 0.0, y = 0.0;
            entityColors = new Dictionary<string, SolidColorBrush>();
            entityColors.Add("node", (SolidColorBrush)new BrushConverter().ConvertFrom("#4287f5"));
            entityColors.Add("switch", (SolidColorBrush)new BrushConverter().ConvertFrom("#A025A0 "));
            entityColors.Add("substation", (SolidColorBrush)new BrushConverter().ConvertFrom("#88FE24"));
            entityColors.Add("line_Steel", (SolidColorBrush)new BrushConverter().ConvertFrom("#71797E"));
            entityColors.Add("line_Copper", (SolidColorBrush)new BrushConverter().ConvertFrom("#b87333"));
            entityColors.Add("line_Acsr", (SolidColorBrush)new BrushConverter().ConvertFrom("#888B8D"));
            entityColors.Add("transparent", (SolidColorBrush)new BrushConverter().ConvertFrom("#00FFFFFF"));
            entityColors.Add("switch_Open", (SolidColorBrush)new BrushConverter().ConvertFrom("#00FF00"));
            entityColors.Add("switch_Closed", (SolidColorBrush)new BrushConverter().ConvertFrom("#FF0000"));
            entityColors.Add("line_RLT1", (SolidColorBrush)new BrushConverter().ConvertFrom("#FF0000"));
            entityColors.Add("line_R1TO2", (SolidColorBrush)new BrushConverter().ConvertFrom("#FF8C00"));
            entityColors.Add("line_GT2", (SolidColorBrush)new BrushConverter().ConvertFrom("#FFFF00"));

            UIEntityAdapter uiAdapter = null;
            entitiesUI = new List<UIEntityAdapter>();

            List<EESEntity> mergedEntities = new List<EESEntity>();
            mergedEntities = ((nodes.ToList<EESEntity>()).Concat(substations)
                                                       .Concat(substations.ToList<EESEntity>())
                                                       .Concat(switches.ToList<EESEntity>())).ToList<EESEntity>();

            var overlays = EntityLoader.GetEntityOverlays(mergedEntities);
            var overlayHeights = new List<int>(overlays.Count());
            for (var heightIt = 0; heightIt < overlays.Count(); ++heightIt)
                overlayHeights.Add(0);

            ModelVisual3D generatedVisual = null;
            foreach (var node in nodes)
            {
                x = node.X;
                y = node.Y;
                UIAdaptCoords(x, y, out x, out y);
                uiAdapter = new UIEntityAdapter(node, x, y, null);
                entitiesUI.Add(uiAdapter);
                generatedVisual = GenerateVisual(uiAdapter, __CUBE_DIM, entityColors["node"], overlays, ref overlayHeights);
                uiAdapter.Geometry = generatedVisual.Content as GeometryModel3D;
                mapViewPort.Children.Add(generatedVisual);
                models.Add(generatedVisual.Content as GeometryModel3D);
            }

            foreach (var sw in switches)
            {
                x = sw.X;
                y = sw.Y;
                UIAdaptCoords(x, y, out x, out y);
                uiAdapter = new UIEntityAdapter(sw, x, y, null);
                entitiesUI.Add(uiAdapter);
                generatedVisual = GenerateVisual(uiAdapter, __CUBE_DIM, entityColors["switch"], overlays, ref overlayHeights);
                uiAdapter.Geometry = generatedVisual.Content as GeometryModel3D;
                mapViewPort.Children.Add(generatedVisual);
                models.Add(generatedVisual.Content as GeometryModel3D);
            }

            foreach (var substation in substations)
            {
                x = substation.X;
                y = substation.Y;
                UIAdaptCoords(x, y, out x, out y);
                uiAdapter = new UIEntityAdapter(substation, x, y, null);
                entitiesUI.Add(uiAdapter);
                generatedVisual = GenerateVisual(uiAdapter, __CUBE_DIM, entityColors["substation"], overlays, ref overlayHeights);
                uiAdapter.Geometry = generatedVisual.Content as GeometryModel3D;
                mapViewPort.Children.Add(generatedVisual);
                models.Add(generatedVisual.Content as GeometryModel3D);
            }

            ModelVisual3D entityVisual           = null;
            GeometryModel3D entityGeometryModel  = null;
            MeshGeometry3D entityMesh            = null;
            ELineEnters lineEnters = ELineEnters.NONE;

            UIEntityAdapter startNode = null;
            UIEntityAdapter endNode = null;
            UIEntityAdapter lineUI = null;
            foreach (var line in lines)
            {
                startNode = entitiesUI.Find(p => p.Entity.ID == line.FirstEnd);
                endNode = entitiesUI.Find(p => p.Entity.ID == line.SecondEnd);

                if (startNode.ApproxedX != endNode.ApproxedX || startNode.ApproxedY != endNode.ApproxedY)
                {
                    for (int vtx = 0; vtx < line.Verticies.Count(); ++vtx)
                    {
                        UIAdaptCoords(line.Verticies[vtx].X, line.Verticies[vtx].Y, out x, out y);
                        line.Verticies[vtx] = new Point(x, y);
                    };

                    entityVisual = new ModelVisual3D();
                    entityGeometryModel = new GeometryModel3D();
                    entityMesh = new MeshGeometry3D();

                    //entityMesh.Normals = new Vector3DCollection()
                    //{
                    //    new Vector3D(0, 0, 1),
                    //    new Vector3D(0, 0, 1),
                    //    new Vector3D(0, 0, 1),
                    //    new Vector3D(0, 0, 1)
                    //};

                    int positionsCNT = 0;
                    for (int vtx = -1; vtx < line.Verticies.Count() + 1; ++vtx) //Fix range to aquire First & second end
                    {
                        if (vtx == -1)  //First end
                        {
                            GenerateLineSegmentStart(__CUBE_DIM, line, startNode, endNode, ref entityMesh, ref lineEnters);
                            positionsCNT += 3;
                            vtx = 0;
                        }
                        else
                        {

                            if (vtx == line.Verticies.Count())  //Second end
                            {
                                x = endNode.ApproxedX;
                                y = endNode.ApproxedY;
                                DetermineOrientation(endNode.ApproxedX, endNode.ApproxedY, line.Verticies.Last().X, line.Verticies.Last().Y, ref lineEnters);
                            }
                            else
                            {
                                DetermineOrientation(line.Verticies[vtx - 1].X, line.Verticies[vtx - 1].Y, line.Verticies[vtx].X, line.Verticies[vtx].Y, ref lineEnters);
                            }

                            x -= (__CUBE_DIM / 2);
                            y -= (__CUBE_DIM / 2);
                            entityMesh.Positions.Add(new Point3D(x, y, 0));
                            switch (lineEnters)
                            {
                                case ELineEnters.BACK:
                                case ELineEnters.FRONT:
                                    {
                                        entityMesh.Positions.Add(new Point3D(x + __CUBE_DIM, y, 0));
                                        entityMesh.Positions.Add(new Point3D(x + __CUBE_DIM / 2, y, +__CUBE_DIM));
                                        break;
                                    }
                                case ELineEnters.LEFT:
                                case ELineEnters.RIGHT:
                                    {
                                        entityMesh.Positions.Add(new Point3D(x, y + __CUBE_DIM, 0));
                                        entityMesh.Positions.Add(new Point3D(x, y + __CUBE_DIM / 2, +__CUBE_DIM));
                                        break;
                                    }
                            }
                            positionsCNT += 3;

                            entityMesh.TriangleIndices.Add(positionsCNT - 6);  //0
                            entityMesh.TriangleIndices.Add(positionsCNT - 5);  //1
                            entityMesh.TriangleIndices.Add(positionsCNT - 4);  //2

                            entityMesh.TriangleIndices.Add(positionsCNT - 3);  //3
                            entityMesh.TriangleIndices.Add(positionsCNT - 2);  //4
                            entityMesh.TriangleIndices.Add(positionsCNT - 1);  //5

                            //See TriangleIndicates.png
                            switch (lineEnters)
                            {
                                case ELineEnters.BACK:
                                    {
                                        entityMesh.TriangleIndices.Add(positionsCNT - 5);  //1
                                        entityMesh.TriangleIndices.Add(positionsCNT - 2);  //4
                                        entityMesh.TriangleIndices.Add(positionsCNT - 1);  //5

                                        entityMesh.TriangleIndices.Add(positionsCNT - 5);  //1
                                        entityMesh.TriangleIndices.Add(positionsCNT - 1);  //5
                                        entityMesh.TriangleIndices.Add(positionsCNT - 4);  //2

                                        entityMesh.TriangleIndices.Add(positionsCNT - 3);  //3
                                        entityMesh.TriangleIndices.Add(positionsCNT - 6);  //0
                                        entityMesh.TriangleIndices.Add(positionsCNT - 4);  //2

                                        entityMesh.TriangleIndices.Add(positionsCNT - 3);  //3
                                        entityMesh.TriangleIndices.Add(positionsCNT - 4);  //2
                                        entityMesh.TriangleIndices.Add(positionsCNT - 1);  //5

                                        entityMesh.TriangleIndices.Add(positionsCNT - 6);  //0
                                        entityMesh.TriangleIndices.Add(positionsCNT - 5);  //1
                                        entityMesh.TriangleIndices.Add(positionsCNT - 2);  //4

                                        entityMesh.TriangleIndices.Add(positionsCNT - 6);  //0
                                        entityMesh.TriangleIndices.Add(positionsCNT - 2);  //4
                                        entityMesh.TriangleIndices.Add(positionsCNT - 3);  //3

                                        break;
                                    }
                                case ELineEnters.FRONT:
                                    {
                                        entityMesh.TriangleIndices.Add(positionsCNT - 2);  //4
                                        entityMesh.TriangleIndices.Add(positionsCNT - 5);  //1
                                        entityMesh.TriangleIndices.Add(positionsCNT - 1);  //5

                                        entityMesh.TriangleIndices.Add(positionsCNT - 5);  //1
                                        entityMesh.TriangleIndices.Add(positionsCNT - 4);  //2
                                        entityMesh.TriangleIndices.Add(positionsCNT - 1);  //5

                                        entityMesh.TriangleIndices.Add(positionsCNT - 6);  //0
                                        entityMesh.TriangleIndices.Add(positionsCNT - 3);  //3
                                        entityMesh.TriangleIndices.Add(positionsCNT - 1);  //5
                                     
                                        entityMesh.TriangleIndices.Add(positionsCNT - 6);  //0
                                        entityMesh.TriangleIndices.Add(positionsCNT - 1);  //5
                                        entityMesh.TriangleIndices.Add(positionsCNT - 4);  //2

                                        entityMesh.TriangleIndices.Add(positionsCNT - 3);  //3
                                        entityMesh.TriangleIndices.Add(positionsCNT - 2);  //4
                                        entityMesh.TriangleIndices.Add(positionsCNT - 6);  //0
                                        
                                        
                                        entityMesh.TriangleIndices.Add(positionsCNT - 2);  //4
                                        entityMesh.TriangleIndices.Add(positionsCNT - 5);  //1
                                        entityMesh.TriangleIndices.Add(positionsCNT - 6);  //0
                                        break;
                                    }
                                case ELineEnters.LEFT:
                                    {
                                        entityMesh.TriangleIndices.Add(positionsCNT - 6);  //0
                                        entityMesh.TriangleIndices.Add(positionsCNT - 3);  //3
                                        entityMesh.TriangleIndices.Add(positionsCNT - 4);  //2

                                        entityMesh.TriangleIndices.Add(positionsCNT - 3);  //3
                                        entityMesh.TriangleIndices.Add(positionsCNT - 1);  //5
                                        entityMesh.TriangleIndices.Add(positionsCNT - 4);  //2

                                        entityMesh.TriangleIndices.Add(positionsCNT - 5);  //1
                                        entityMesh.TriangleIndices.Add(positionsCNT - 2);  //4
                                        entityMesh.TriangleIndices.Add(positionsCNT - 4);  //2

                                        entityMesh.TriangleIndices.Add(positionsCNT - 2);  //4
                                        entityMesh.TriangleIndices.Add(positionsCNT - 1);  //5
                                        entityMesh.TriangleIndices.Add(positionsCNT - 4);  //2

                                        entityMesh.TriangleIndices.Add(positionsCNT - 6);  //0
                                        entityMesh.TriangleIndices.Add(positionsCNT - 3);  //3
                                        entityMesh.TriangleIndices.Add(positionsCNT - 2);  //4

                                        entityMesh.TriangleIndices.Add(positionsCNT - 6);  //0
                                        entityMesh.TriangleIndices.Add(positionsCNT - 5);  //1
                                        entityMesh.TriangleIndices.Add(positionsCNT - 2);  //4

                                        break;
                                    }
                                case ELineEnters.RIGHT:
                                    {
                                        entityMesh.TriangleIndices.Add(positionsCNT - 6);  //0
                                        entityMesh.TriangleIndices.Add(positionsCNT - 4);  //2
                                        entityMesh.TriangleIndices.Add(positionsCNT - 3);  //3

                                        entityMesh.TriangleIndices.Add(positionsCNT - 3);  //3
                                        entityMesh.TriangleIndices.Add(positionsCNT - 4);  //2
                                        entityMesh.TriangleIndices.Add(positionsCNT - 1);  //5

                                        entityMesh.TriangleIndices.Add(positionsCNT - 4);  //2
                                        entityMesh.TriangleIndices.Add(positionsCNT - 5);  //1
                                        entityMesh.TriangleIndices.Add(positionsCNT - 2);  //4

                                        entityMesh.TriangleIndices.Add(positionsCNT - 2);  //4
                                        entityMesh.TriangleIndices.Add(positionsCNT - 4);  //2
                                        entityMesh.TriangleIndices.Add(positionsCNT - 1);  //5

                                        entityMesh.TriangleIndices.Add(positionsCNT - 6);  //0
                                        entityMesh.TriangleIndices.Add(positionsCNT - 3);  //3
                                        entityMesh.TriangleIndices.Add(positionsCNT - 5);  //1

                                        entityMesh.TriangleIndices.Add(positionsCNT - 3);  //3
                                        entityMesh.TriangleIndices.Add(positionsCNT - 2);  //4
                                        entityMesh.TriangleIndices.Add(positionsCNT - 5);  //1
                                        break;
                                    }
                            }
                        }
                    }
                    //Draw
                    entityVisual.Content = entityGeometryModel;
                    entityGeometryModel.Geometry = entityMesh;
                    entityGeometryModel.Material = new DiffuseMaterial() { Brush = entityColors[$"line_{line.ConductorMaterial}"] };
                    mapViewPort.Children.Add(entityVisual);
                    models.Add(entityGeometryModel);


                    lineUI = new UILineAdapter(startNode, endNode, line, 0, 0, entityGeometryModel);
                    entitiesUI.Add(lineUI);
                }
            }
        }
        private void MapViewPort_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            manipulation = E3DObjManipulation.ZOOM;
            double newVal;
            if (e.Delta > 0)    //Zoom In
            {
                newVal = mainCam.Position.Z * 0.9;
                mainCam.Position = new System.Windows.Media.Media3D.Point3D()
                {
                    X = mainCam.Position.X,
                    Y = mainCam.Position.Y,
                    Z = newVal
                };
            }
            else if (e.Delta <= 0)//Zoom out
            {
                newVal = mainCam.Position.Z / 0.9;
                if (newVal > defaultCamPosition.Z*__ZOOM_OUT_MAX_COEF) return;

                mainCam.Position = new System.Windows.Media.Media3D.Point3D()
                {
                    X = mainCam.Position.X,
                    Y = mainCam.Position.Y,
                    Z = newVal
                };
            }
            currZoomCoef = mainCam.Position.Z / defaultCamPosition.Z;
            manipulation = E3DObjManipulation.NONE;
        }
        private void MapViewPort_MouseMove(object sender, MouseEventArgs e)
        {
            if (mapViewPort.IsMouseCaptured && manipulation == E3DObjManipulation.PAN)
            {
                Point end = e.GetPosition(this);
                double offsetX = (end.X - start.X)*15*currZoomCoef;
                double offsetY = -(end.Y - start.Y)*15*currZoomCoef;
                mainCam.Position = new System.Windows.Media.Media3D.Point3D()
                {
                    X = mainCam.Position.X + offsetX,
                    Y = mainCam.Position.Y + offsetY,
                    Z = mainCam.Position.Z
                };
            }
            else if(mapViewPort.IsMouseCaptured && manipulation == E3DObjManipulation.ROTATE)
            {              

                Point end = e.GetPosition(this);
                double deltaX=0, deltaY=0;

                if ((start.X == 0) && (start.Y == 0)) return;
                if (end.X > start.X)
                {
                    deltaX= -((end.X - start.X) * 360 / (entityModel.Content.Bounds.SizeX / 1000));
                    danimX.By = deltaX;  
                }
                else if (end.X < start.X)
                {
                    deltaX = ((start.X - end.X) * 360 / (entityModel.Content.Bounds.SizeX / 1000));
                    danimX.By = deltaX;
                }
                if (end.Y > start.Y)
                {
                    deltaY = -((end.Y - start.Y) * 360 / (entityModel.Content.Bounds.SizeY / 1000));
                    danimY.By = deltaY;
                }
                else if (end.Y < start.Y)
                {
                    deltaY = ((start.Y - end.Y) * 360 / (entityModel.Content.Bounds.SizeY / 1000));
                    danimY.By = deltaY;
                }

                danimX.By = deltaX;
                danimY.By = deltaY;
                sboardRot.Begin(mapViewPort);

                cumulativeXRot += deltaX;
                cumulativeYRot += deltaY;
                cumulativeXRot %= 360;
                cumulativeYRot %= 360;
            }
        }
        private void MapViewPort_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TooltipLbl.Visibility = Visibility.Hidden;
            manipulation = E3DObjManipulation.PAN;
            mapViewPort.CaptureMouse();
            start = e.GetPosition(this);
            Mouse.OverrideCursor = Cursors.Hand;
        }
        private void MapViewPort_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mapViewPort.ReleaseMouseCapture();
            manipulation = E3DObjManipulation.NONE;
            Mouse.OverrideCursor = Cursors.Arrow;
        }
        private void RevokeLine_MouseRightButtonDown()
        {
            if(tooltipedEntityIdx!=-1)
            {
                DiffuseMaterial diffMat = null;
                switch ((entitiesUI[tooltipedEntityIdx] as UILineAdapter).FirstEnd.Entity.Type)
                {
                    case EESEntityType.Node: { diffMat = new DiffuseMaterial() { Brush = entityColors["node"] }; break; }
                    case EESEntityType.Substation: { diffMat = new DiffuseMaterial() { Brush = entityColors["substation"] }; break; }
                    case EESEntityType.Switch: { diffMat = new DiffuseMaterial() { Brush = entityColors["switch"] }; break; }
                }
                (entitiesUI[tooltipedEntityIdx] as UILineAdapter).FirstEnd.Geometry.Material = diffMat;

                switch ((entitiesUI[tooltipedEntityIdx] as UILineAdapter).SecondEnd.Entity.Type)
                {
                    case EESEntityType.Node:        { diffMat = new DiffuseMaterial() { Brush = entityColors["node"] }; break; }
                    case EESEntityType.Substation:  { diffMat = new DiffuseMaterial() { Brush = entityColors["substation"] }; break; }
                    case EESEntityType.Switch:      { diffMat = new DiffuseMaterial() { Brush = entityColors["switch"] }; break; }
                }
                (entitiesUI[tooltipedEntityIdx] as UILineAdapter).SecondEnd.Geometry.Material = diffMat;
                tooltipedEntityIdx = -1;
            }
        }
        private void MapViewPort_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton.ToString().Equals("Middle") && e.MiddleButton.ToString().Equals("Pressed"))
            {
                manipulation = E3DObjManipulation.ROTATE;
                mapViewPort.CaptureMouse();
                start = e.GetPosition(this);
                Mouse.OverrideCursor = Cursors.Cross;
            }

            if(!e.ChangedButton.ToString().Equals("Right"))
            {
                TooltipLbl.Visibility = Visibility.Hidden;
            }
        }
        private HitTestResultBehavior HTResult(System.Windows.Media.HitTestResult rawresult)
        {

            RayHitTestResult rayResult = rawresult as RayHitTestResult;

            RevokeLine_MouseRightButtonDown();  //Only one selection at the moment, moldify if neccessary
            if (rayResult != null)
            {
                bool gasit = false;
                for (int i = 0; i < models.Count; i++)
                {
                    if ((GeometryModel3D)models[i] == rayResult.ModelHit)
                    {
                        hitgeo = (GeometryModel3D)rayResult.ModelHit;
                        gasit = true;
                        System.Windows.Point mousePos = Mouse.GetPosition(ContentGrid);
                        TooltipLbl.Margin = new Thickness(mousePos.X, mousePos.Y, 0,0);
                        TooltipLbl.Visibility = Visibility.Visible;
                        var index = models.IndexOf(hitgeo);
                        if (index == -1) break;
                        TooltipLbl.Text = entitiesUI[index].Entity.ToString();

                        if(entitiesUI[index].Entity.Type == EESEntityType.Line)
                        {
                            
                            (entitiesUI[index] as UILineAdapter).FirstEnd.Geometry.Material= new DiffuseMaterial()
                            { Brush = (SolidColorBrush)new BrushConverter().ConvertFrom("#cd5c5c ") };//Indianred

                            (entitiesUI[index] as UILineAdapter).SecondEnd.Geometry.Material = new DiffuseMaterial()
                            { Brush = (SolidColorBrush)new BrushConverter().ConvertFrom("#cd5c5c ") };//Indianred

                            tooltipedEntityIdx = index;
                        }

                        TooltipLbl.Visibility = Visibility.Visible;
                        break;
                    }
                }
                if (!gasit)
                {
                    hitgeo = null;
                    TooltipLbl.Visibility = Visibility.Hidden;
                }
            }

            return HitTestResultBehavior.Stop;
        }
        private void MapViewPort_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point mousePos = e.GetPosition(mapViewPort);
            Point3D testpoint3D = new Point3D(mousePos.X, mousePos.Y, 0);
            Vector3D testdirection = new Vector3D(mousePos.X, mousePos.Y, 10);
            PointHitTestParameters pointparams =new PointHitTestParameters(mousePos);
            RayHitTestParameters rayparams = new RayHitTestParameters(testpoint3D, testdirection);

            hitgeo = null;
            VisualTreeHelper.HitTest(mapViewPort, null, HTResult, pointparams);
        }
        private void InactiveSW_Checked(object sender, RoutedEventArgs e)
        {
            var targs =entitiesUI.FindAll(x => x.Entity.Type == EESEntityType.Switch && (x.Entity as Switch).Status.Equals("Open")) as List<UIEntityAdapter>;   //Get all switches
            foreach(var targ in targs)
            {
                (targ as UIEntityAdapter).Geometry.Material = new DiffuseMaterial()  //Hide switch
                { Brush = entityColors["transparent"] };

                var tmp = entitiesUI.FindAll(x => x.Entity.Type == EESEntityType.Line ) as List<UIEntityAdapter>; //Get lines exiting from it
                List<UILineAdapter>exits = new List<UILineAdapter>(); ;
                foreach (var exit in tmp)
                {
                    if ((exit as UILineAdapter).FirstEnd.Entity.ID.Equals(targ.Entity.ID)) exits.Add(exit as UILineAdapter);
                }
                tmp = null;

                foreach (var exitLine in exits)
                {

                    exitLine.SecondEnd.Geometry.Material = new DiffuseMaterial()  //Hide line end
                    { Brush = entityColors["transparent"] };
                    
                    exitLine.Geometry.Material = new DiffuseMaterial()  //Hide line itself
                    { Brush = entityColors["transparent"] };
                }              
            }         
        }
        private void InactiveSW_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!((bool)ShowSWs.IsChecked)) return;

            var targs = entitiesUI.FindAll(x => x.Entity.Type == EESEntityType.Switch && (x.Entity as Switch).Status.Equals("Open")) as List<UIEntityAdapter>;   //Get all switches
            foreach (var targ in targs)
            {
                (targ as UIEntityAdapter).Geometry.Material = new DiffuseMaterial()  //Show switch
                { Brush = entityColors["switch"] };

                var tmp = entitiesUI.FindAll(x => x.Entity.Type == EESEntityType.Line) as List<UIEntityAdapter>; //Get lines exiting from it
                List<UILineAdapter> exits = new List<UILineAdapter>(); ;
                foreach (var exit in tmp)
                {
                    if ((exit as UILineAdapter).FirstEnd.Entity.ID.Equals(targ.Entity.ID)) exits.Add(exit as UILineAdapter);
                }
                tmp = null;

                foreach (var exitLine in exits)
                {  
                    exitLine.SecondEnd.Geometry.Material = new DiffuseMaterial()  //Hide line end
                    { Brush = entityColors[exitLine.SecondEnd.Entity.Type.ToString().ToLower()] };

                    exitLine.Geometry.Material = new DiffuseMaterial()  //Hide line itself
                    { Brush = entityColors[$"line_{(exitLine.Entity as EESMap_3D.Model.Line).ConductorMaterial}"] };
                }
            }
        }
        private void MarkClosedSWs_Checked(object sender, RoutedEventArgs e)
        {
            if (!(bool)(ShowSWs.GetValue(CheckBox.IsCheckedProperty))) return;

            var targs = entitiesUI.FindAll(x => x.Entity.Type == EESEntityType.Switch) as List<UIEntityAdapter>;
            foreach (var targ in targs)
            {
                (targ as UIEntityAdapter).Geometry.Material = new DiffuseMaterial() 
                { Brush = entityColors[$"switch_{(targ.Entity as Switch).Status}"] };
            }
        }
        private void MarkClosedSWs_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!(bool)(ShowSWs.GetValue(CheckBox.IsCheckedProperty))) return;

            var targs = entitiesUI.FindAll(x => x.Entity.Type == EESEntityType.Switch) as List<UIEntityAdapter>;
            foreach (var targ in targs)
            {
                (targ as UIEntityAdapter).Geometry.Material = new DiffuseMaterial()  
                { Brush = entityColors["switch"] };
            }
        }
        private void ApplyResistanceColor_Checked(object sender, RoutedEventArgs e)
        {
            var tmp = entitiesUI.FindAll(x => x.Entity.Type == EESEntityType.Line) as List<UIEntityAdapter>;
            double r = 0.0;
            foreach (var line in tmp)
            {
                r = ((line as UILineAdapter).Entity as EESMap_3D.Model.Line).R; 
                if (r < 1)
                {
                    line.Geometry.Material = new DiffuseMaterial()  
                    { Brush = entityColors["line_RLT1"] };
                }
                else if (r >= 1 && r <= 2)
                {
                    line.Geometry.Material = new DiffuseMaterial()  
                    { Brush = entityColors["line_R1TO2"] };
                }
                else
                {
                    line.Geometry.Material = new DiffuseMaterial()  
                    { Brush = entityColors["line_GT2"] };
                }
            }
        }
        private void ApplyResistanceColor_Unchecked(object sender, RoutedEventArgs e)
        {
            var tmp = entitiesUI.FindAll(x => x.Entity.Type == EESEntityType.Line) as List<UIEntityAdapter>;
            foreach (var line in tmp)
            {
                line.Geometry.Material = new DiffuseMaterial() { Brush = entityColors[$"line_{(line.Entity as EESMap_3D.Model.Line) .ConductorMaterial}"] };
            }
        }

        private void ShowNodes_Checked(object sender, RoutedEventArgs e)
        {
            if (initialCheckFlag > 0) { --initialCheckFlag; return; }
            var targs = entitiesUI.FindAll(x => x.Entity.Type == EESEntityType.Node) as List<UIEntityAdapter>;
            foreach (var targ in targs)
            {
                (targ as UIEntityAdapter).Geometry.Material = new DiffuseMaterial()
                { Brush = entityColors[$"node"] };
            }
        }

        private void ShowNodes_Unchecked(object sender, RoutedEventArgs e)
        {
            var targs = entitiesUI.FindAll(x => x.Entity.Type == EESEntityType.Node) as List<UIEntityAdapter>;
            foreach (var targ in targs)
            {
                (targ as UIEntityAdapter).Geometry.Material = new DiffuseMaterial()
                { Brush = entityColors[$"transparent"] };
            }
        }

        private void ShowSWs_Checked(object sender, RoutedEventArgs e)
        {
            if (initialCheckFlag>0) { --initialCheckFlag; return; }
            var targs = entitiesUI.FindAll(x => x.Entity.Type == EESEntityType.Switch) as List<UIEntityAdapter>;
            foreach (var targ in targs)
            {
                (targ as UIEntityAdapter).Geometry.Material = new DiffuseMaterial()
                { Brush = entityColors[$"switch"] };
            }
        }

        private void ShowSWs_Unchecked(object sender, RoutedEventArgs e)
        {
            var targs = entitiesUI.FindAll(x => x.Entity.Type == EESEntityType.Switch) as List<UIEntityAdapter>;
            foreach (var targ in targs)
            {
                (targ as UIEntityAdapter).Geometry.Material = new DiffuseMaterial()
                { Brush = entityColors[$"transparent"] };
            }
        }

        private void ShowSubs_Checked(object sender, RoutedEventArgs e)
        {
            if (initialCheckFlag > 0) { --initialCheckFlag; return; }
            var targs = entitiesUI.FindAll(x => x.Entity.Type == EESEntityType.Substation) as List<UIEntityAdapter>;
            foreach (var targ in targs)
            {
                (targ as UIEntityAdapter).Geometry.Material = new DiffuseMaterial()
                { Brush = entityColors[$"substation"] };
            }

        }

        private void ShowSubs_Unchecked(object sender, RoutedEventArgs e)
        {
            var targs = entitiesUI.FindAll(x => x.Entity.Type == EESEntityType.Substation) as List<UIEntityAdapter>;
            foreach (var targ in targs)
            {
                (targ as UIEntityAdapter).Geometry.Material = new DiffuseMaterial()
                { Brush = entityColors[$"transparent"] };
            }
        }

        private void RestartPosition_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //danimX.By = (-cumulativeXRot);
            //danimY.By = (-cumulativeYRot);
            //sboardRot.Begin(mapViewPort);
            //cumulativeXRot = 0;
            //cumulativeYRot = 0;
            //Thread.Sleep((int)Math.Ceiling(Math.Max(danimX.Duration.TimeSpan.TotalMilliseconds, danimY.Duration.TimeSpan.TotalMilliseconds)));

            mainCam.Position = new Point3D()
            {
                X = defaultCamPosition.X,
                Y = defaultCamPosition.Y,
                Z = defaultCamPosition.Z
            };
        }
        private void MapViewPort_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton.ToString().Equals("Middle") && e.MiddleButton.ToString().Equals("Released"))
            {
                mapViewPort.ReleaseMouseCapture();
                manipulation = E3DObjManipulation.NONE;
                Mouse.OverrideCursor = Cursors.Arrow;
            }           
        }
    }
}
