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
            if (mount != null)
            {
                myMount = mount;
                x = myMount.X;
                y = myMount.Y;
            }
            else
            {
                x = 0;
                y = 0;
            }
        }

        internal void Update(Terrain terrain)
        {
            if (myMount != null && myMount.Alive)
            {
                x = myMount.X;
                y = myMount.Y;
            }
            else
            {
                MountCritter(terrain.RandomCritter());
            }
        }
    }
}
