using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EESMap_3D.Model
{
    public class Switch : EESEntity
    {
        private string status;
        public Switch(ulong ID, string Name, double X, double Y, string status) : base(ID, Name, X, Y, EESEntityType.Switch) { this.status = status; }

        public string Status { get => status; }

        public override string ToString()
        {
            return base.ToString()+Environment.NewLine+ $"Status:\t{status}";
        }
    }
}
