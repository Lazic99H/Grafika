using EESMap_3D.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace EESMap_3D.Engine
{
    public class UILineAdapter:UIEntityAdapter
    {
        private UIEntityAdapter firstEnd;
        private UIEntityAdapter secondEnd;

        public UILineAdapter(UIEntityAdapter firstEnd, UIEntityAdapter secondEnd, EESEntity entity, double approxedX, double approxedY, GeometryModel3D geometry) :
            base(entity, approxedX, approxedY, geometry)
        {
            this.firstEnd = firstEnd;
            this.secondEnd = secondEnd;
        }

        public UIEntityAdapter FirstEnd { get => firstEnd; }
        public UIEntityAdapter SecondEnd { get => secondEnd; }
    }
}
