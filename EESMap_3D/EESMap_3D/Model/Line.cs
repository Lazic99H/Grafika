using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EESMap_3D.Model
{
    public class Line:EESEntity
    {
        private bool isUnderground;
        private double r;
        private string conductorMaterial;
        private string lineType;
        private uint thermalConstantHeat;
        private uint firstEnd;
        private uint secondEnd;
        private List<Point> verticies;

        public Line(uint id, string name, bool isUnderground, double r, string conductorMaterial, string lineType, uint thermalConstantHeat, uint firstEnd, uint secondEnd, List<Point> verticies)
            :base(id, name, 0, 0, EESEntityType.Line)
        {
            this.isUnderground = isUnderground;
            this.r = r;
            this.conductorMaterial = conductorMaterial;
            this.lineType = lineType;
            this.thermalConstantHeat = thermalConstantHeat;
            this.firstEnd = firstEnd;
            this.secondEnd = secondEnd;
            this.verticies = verticies;
        }
        public override string ToString()
        {
            return $"Type:\t{Enum.GetName(typeof(EESEntityType), Type)}{Environment.NewLine}" +
                   $"ID:\t{ID}{Environment.NewLine}" +
                   $"Name:\t{Name}{Environment.NewLine}" +
                   $"FirstEnd ID:\t{FirstEnd}{Environment.NewLine}" +
                   $"SecondEnd ID:\t{SecondEnd}{Environment.NewLine}" +
                   $"Conductor:\t{ConductorMaterial}{Environment.NewLine}" +
                   $"LineType:\t{LineType}{Environment.NewLine}" +
                   $"THConst:\t\t{ThermalConstantHeat}{Environment.NewLine}" +
                   $"R:\t\t{R}{Environment.NewLine}" +
                   $"IsUnderground:\t{(IsUnderground ? "Y" : "N")}";
        }

        public ulong Id                  { get =>base.ID;               }
        public string Name              { get => base.Name;             }
        public bool IsUnderground       { get => isUnderground;         }
        public double R                 { get => r;                     }
        public string ConductorMaterial { get => conductorMaterial;     }
        public string LineType          { get => lineType;              }
        public uint ThermalConstantHeat { get => thermalConstantHeat;   }
        public uint FirstEnd            { get => firstEnd;              }
        public uint SecondEnd           { get => secondEnd;             }
        public List<Point> Verticies    { get => verticies;             }
    }
}
