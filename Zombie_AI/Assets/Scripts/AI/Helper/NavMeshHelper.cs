using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.AI
{
    class NavMeshHelper
    {
        public enum Areas
        {
            Walkable = 1,
            Jump = 4,
            Area2 = 8,
            Area3 = 16,
            Area4 = 32,
            Area5 = 64,
            Area6 = 128

        }
    }
}
