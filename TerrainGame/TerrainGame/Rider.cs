using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerrainGame
{
    class Rider
    {
        private Critter myMount;
        int x, y;

        public Rider(Critter mount)
        {
            myMount = mount;
            x = myMount.X;
            y = myMount.Y;
        }

        internal void Update()
        {
            if (myMount.Alive)
            {
                x = myMount.X;
                y = myMount.Y;
            }
        }
    }
}
