using EESMap_3D.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace EESMap_3D.Engine
{
    public class EntityLoader
    {
        private static readonly int __ZONE_UTM = 34;
        private static readonly Tuple<double, double> __ENVIRONMENT_LIMIT_BTM_LEFT_CONVERTED  = new Tuple<double, double> (19.793909, 45.2325);
        private static readonly Tuple<double, double> __ENVIRONMENT_LIMIT_UPP_RIGHT_CONVERTED = new Tuple<double, double> (19.894459, 45.277031);
        public static Tuple<double, double> GetEnvironmentLimit_BtmLeft_Converted() { return __ENVIRONMENT_LIMIT_BTM_LEFT_CONVERTED; }
        public static Tuple<double, double> GetEnvironmentLimit_UppRight_Converted() { return __ENVIRONMENT_LIMIT_UPP_RIGHT_CONVERTED; }

        public static List<Tuple<double, double>> GetEntityOverlays(List<EESEntity> EESEntities)
        {
            List<Tuple<double, double>> overlays = new List<Tuple<double, double>>();
            foreach(var entity in EESEntities)
            {
                if (EESEntities.Where(x => x.X == entity.X && x.Y == entity.Y && x.ID != entity.ID).FirstOrDefault() != null &&
                    overlays.Where(x => x.Item1 == entity.X && x.Item2 == entity.Y).FirstOrDefault() == null)
                    overlays.Add(new Tuple<double, double>(entity.X, entity.Y));
            }

            return overlays;
        }
        private static bool ValidateCoordsRangeFit(double x, double y)
        {
            return  x >= __ENVIRONMENT_LIMIT_BTM_LEFT_CONVERTED.Item1  &&
                    x <= __ENVIRONMENT_LIMIT_UPP_RIGHT_CONVERTED.Item1 &&
                    y >= __ENVIRONMENT_LIMIT_BTM_LEFT_CONVERTED.Item2  &&
                    y <= __ENVIRONMENT_LIMIT_UPP_RIGHT_CONVERTED.Item2;
        }
        private static bool ValidateCoordsRangeFit(EESEntity entity)
        {
            return ValidateCoordsRangeFit(entity.X, entity.Y);
        }

        public static void LoadModelFrom(XmlDocument xmlDoc, out List<Node> nodes, out List<Substation> substations, out List<Switch> switches, out List<Line> lines)
        {
            nodes       = new List<Node>();
            substations = new List<Substation>();
            switches    = new List<Switch>();
            lines       = new List<Line>();

            Node        tmpNode         = null;
            Substation  tmpSubstation   = null;
            Switch      tmpSwitch       = null;
            Line        tmpLine         = null;
            try
            {
                foreach (XmlNode node in xmlDoc.SelectNodes("/NetworkModel/Nodes/NodeEntity"))
                {
                    tmpNode = ParseNode(node);
                    if(ValidateCoordsRangeFit(tmpNode)) nodes.Add(tmpNode);
                }
            }
            catch (Exception exc) {/*TODO: Handle at front*/ }

            try
            {
                foreach (XmlNode substation in xmlDoc.SelectNodes("/NetworkModel/Substations/SubstationEntity"))
                {
                    tmpSubstation = ParseSubstation(substation);
                    if (ValidateCoordsRangeFit(tmpSubstation)) substations.Add(tmpSubstation);
                }
            }
            catch (Exception exc) {/*TODO: Handle at front*/}

            try
            {
                foreach (XmlNode sw in xmlDoc.SelectNodes("/NetworkModel/Switches/SwitchEntity"))
                {
                    tmpSwitch = ParseSwitch(sw);
                    if (ValidateCoordsRangeFit(tmpSwitch)) switches.Add(tmpSwitch);
                }
            }
            catch (Exception exc) {/*TODO: Handle at front*/}

            try
            {
                List<EESEntity> mergedEntities = new List<EESEntity>();
                mergedEntities=((nodes.ToList<EESEntity>()).Concat(substations)
                                                           .Concat(substations.ToList<EESEntity>())
                                                           .Concat(switches.ToList<EESEntity>())).ToList<EESEntity>();

                foreach (XmlNode line in xmlDoc.SelectNodes("/NetworkModel/Lines/LineEntity"))
                {
                    if((tmpLine = ParseLine(line, mergedEntities)) is null) continue;

                    var fistEnd   = mergedEntities.Where(x => x.ID==tmpLine.FirstEnd).FirstOrDefault();
                    var secondEnd = mergedEntities.Where(x => x.ID== tmpLine.SecondEnd).FirstOrDefault();
                    if (fistEnd != null && secondEnd != null && 
                        lines.Where(l => l.FirstEnd.Equals(tmpLine.FirstEnd) && l.SecondEnd.Equals(tmpLine.SecondEnd)).FirstOrDefault()==null)
                            lines.Add(tmpLine);
                }
                mergedEntities = null;
            }
            catch (Exception exc) {/*TODO: Handle at front*/}
        }

        private static Node ParseNode(XmlNode node)
        {
            ulong id = 0;
            string name = "BAD_SUB";
            double x = 0;
            double y = 0;

            foreach (XmlNode nodeParam in node.ChildNodes)
            {
                if (nodeParam.NodeType == XmlNodeType.Element)
                {
                    switch (nodeParam.Name)
                    {
                        //Common for ees entity
                        case "Id": { ulong.TryParse(nodeParam.InnerText, out id); break; }
                        case "Name": { name = nodeParam.InnerText; break; }
                        case "X": { double.TryParse(nodeParam.InnerText, out x); break; }
                        case "Y": { double.TryParse(nodeParam.InnerText, out y); break; }

                        //Extra Node attributes
                    }
                }
            }
            ToLatLon(x, y, __ZONE_UTM, out y, out x);
            return new Node(id, name, x, y);
        }

        private static Substation ParseSubstation(XmlNode substation)
        {
            ulong id = 0;
            string name = "BAD_SUB";
            double x = 0;
            double y = 0;

            foreach (XmlNode subParam in substation.ChildNodes)
            {
                if (subParam.NodeType == XmlNodeType.Element)
                {
                    switch (subParam.Name)
                    {
                        //Common for ees entity
                        case "Id": { ulong.TryParse(subParam.InnerText, out id); break; }
                        case "Name": { name = subParam.InnerText; break; }
                        case "X": { double.TryParse(subParam.InnerText, out x); break; }
                        case "Y": { double.TryParse(subParam.InnerText, out y); break; }

                            //Extra Substation attributes
                    }
                }
            }
            ToLatLon(x, y, __ZONE_UTM, out y, out x);
            return new Substation(id, name, x, y);
        }

        private static Switch ParseSwitch(XmlNode sw)
        {
            ulong id = 0;
            string name = "BAD_SUB";
            double x = 0;
            double y = 0;
            string status = "";

            foreach (XmlNode swParam in sw.ChildNodes)
            {
                if (swParam.NodeType == XmlNodeType.Element)
                {
                    switch (swParam.Name)
                    {
                        //Common for ees entity
                        case "Id": { ulong.TryParse(swParam.InnerText, out id); break; }
                        case "Name": { name = swParam.InnerText; break; }
                        case "X": { double.TryParse(swParam.InnerText, out x); break; }
                        case "Y": { double.TryParse(swParam.InnerText, out y); break; }
                        case "Status": { status = swParam.InnerText;  break; }
                            //Extra Switch parameters
                    }
                }
            }
            ToLatLon(x, y, __ZONE_UTM, out y, out x);
            return new Switch(id, name, x, y, status);
        }

        private static Line ParseLine(XmlNode node, List<EESEntity> entities)
        {
            uint id = 0;
            string name = "";
            bool isUnderground = false;
            double r = 0.0;
            string conductorMaterial = "";
            string lineType = "";
            uint thermalConstantHeat = 0;
            uint firstEnd = 0;
            uint secondEnd = 0;
            List<Point> vertices = new List<Point>(); 

            foreach (XmlNode nodeParam in node.ChildNodes)
            {
                if (nodeParam.NodeType == XmlNodeType.Element)
                {
                    switch (nodeParam.Name)
                    {
                        case "Id":
                            {
                                if (!uint.TryParse(nodeParam.InnerText, out id)) return null;
                                break;
                            }
                        case "Name":
                            {
                                if (nodeParam.InnerText == "") return null;
                                name = nodeParam.InnerText;
                                break;
                            }
                        case "IsUnderground":
                            {
                                if (!bool.TryParse(nodeParam.InnerText, out isUnderground)) return null;
                                break;
                            }
                        case "R":
                            {
                                if (!Double.TryParse(nodeParam.InnerText, out r)) return null;
                                break;
                            }
                        case "ConductorMaterial":
                            {
                                if (nodeParam.InnerText == "") return null;
                                conductorMaterial = nodeParam.InnerText;
                                break;
                            }
                        case "LineType":
                            {
                                if (nodeParam.InnerText == "") return null;
                                lineType = nodeParam.InnerText;
                                break;
                            }
                        case "ThermalConstantHeat":
                            {
                                if (!uint.TryParse(nodeParam.InnerText, out thermalConstantHeat)) return null;
                                break;
                            }
                        case "FirstEnd":
                            {
                                if (!uint.TryParse(nodeParam.InnerText, out firstEnd)) return null;
                                if((entities.Where(x => x.ID == firstEnd).FirstOrDefault() == null)) return null;
                                break;
                            }
                        case "SecondEnd":
                            {
                                if (!uint.TryParse(nodeParam.InnerText, out secondEnd) || (entities.Where(x => x.ID == secondEnd).FirstOrDefault() == null)) return null;
                                break;
                            }
                        case "Vertices":
                            {
                                foreach(XmlNode point in nodeParam.ChildNodes)
                                {
                                    double x, y;
                                    if (!double.TryParse(point.ChildNodes[0].InnerText, out x)) return null;
                                    if (!double.TryParse(point.ChildNodes[1].InnerText, out y)) return null;
                                    ToLatLon(x, y, __ZONE_UTM, out y, out x);
                                    
                                    if(ValidateCoordsRangeFit(x, y))        //Ignore out of map
                                        vertices.Add( new Point(x, y));
                                }
                                break;
                            }
                    }
                }
            }

            return new Line(id, name, isUnderground, r, conductorMaterial, lineType, thermalConstantHeat, firstEnd, secondEnd, vertices);
        }
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

        public static Tuple<double,double> ToLatLon(double utmX, double utmY, int zoneUTM)
        {
            double convertedX, convertedY;
            ToLatLon(utmX, utmY, zoneUTM, out convertedX, out convertedY);
            return new Tuple<double, double>(convertedX, convertedY);
        }
    }
}
