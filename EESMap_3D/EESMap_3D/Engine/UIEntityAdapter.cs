using EESMap_3D.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace EESMap_3D.Engine
{
    public class UIEntityAdapter
    {
        private EESEntity entity;
        private double approxedX;
        private double approxedY;
        GeometryModel3D geometry;

        public UIEntityAdapter(EESEntity entity, double approxedX, double approxedY, GeometryModel3D geometry)
        {
            this.entity = entity;
            this.approxedX = approxedX;
            this.approxedY = approxedY;
            this.geometry = geometry;
        }

        public EESEntity Entity { get => entity;}
        public double ApproxedX { get => approxedX;}
        public double ApproxedY { get => approxedY;}
        public GeometryModel3D Geometry { get => geometry; set => geometry = value; }
    }
}
