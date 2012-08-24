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

        public int X
        {
            get
            {
                return x;
            }
        }
        
        public int Y
        {
            get
            {
                return y;
            }
        }

        public Rider()
        {
            x = -1;
            y = -1;
        }
        
        public void MountCritter(Critter mount)
        {
            myMount = mount;
            x = myMount.X;
            y = myMount.Y;
        }

        internal void Update()
        {
            if (myMount != null && myMount.Alive)
            {
                x = myMount.X;
                y = myMount.Y;
            }
            else
            {
                x = 0;
                y = 0;
            }
        }
    }
}
