using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EESMap_3D.Model
{
    public enum EESEntityType { Node = 0, Substation = 1, Switch = 3, Line, Unknown = 4}
    public class EESEntity
    {
        private double x;
        private double y;
        public EESEntity(ulong ID, string Name, double X, double Y, EESEntityType type)
        {
            this.ID = ID;
            this.Name = Name;
            this.x = X;
            this.y = Y;
            this.Type = type;
        }
        public EESEntity(EESEntity entity)
        {
            this.ID = entity.ID;
            this.Name = entity.Name;
            this.x = entity.X;
            this.y = entity.Y;
            this.Type = entity.Type;
        }
        public EESEntity()
        {
            this.ID = 0;
            this.Name = "INVALID";
            this.x = 0;
            this.y = 0;
            this.Type = EESEntityType.Unknown;
        }
        public ulong ID { get; }
        public string Name { get; }
        public double X { get => x; }
        public double Y { get => y; }
        public EESEntityType Type { get; }
        public void ReformatCoords(double longitude, double latitude)
        {
            x = longitude;
            y = latitude;
        }
        public override string ToString()
        {
            return $"Type:\t{Enum.GetName(typeof(EESEntityType), Type)}{Environment.NewLine}" +
                   $"ID:\t{ID}{Environment.NewLine}" +
                   $"Name:\t{Name}{Environment.NewLine}" +
                   $"X:\t{X}{Environment.NewLine}" +
                   $"Y:\t{Y}";
        }
    }
}
