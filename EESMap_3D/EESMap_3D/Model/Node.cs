using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EESMap_3D.Model
{
    public class Node : EESEntity
    {
        public Node(ulong ID, string Name, double X, double Y) : base(ID, Name, X, Y, EESEntityType.Node) { }
    }
}
