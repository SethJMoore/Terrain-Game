using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerrainGame
{
    class Critter
    {
        public enum CritterType
        {
            Diver, Climber
        }

        int x, y; //X and Y cooridinates.
        CritterType critterType;
        int age;
        bool fertile;

        public Critter()
        {
            x = 0;
            y = 0;
            critterType = (CritterType)Game1.rand.Next(2); //Random Critter type.
            age = 0;
            fertile = false;
        }

        internal void PlaceRandomly(Terrain terrain)
        {
            x = Game1.rand.Next(terrain.Width);
            y = Game1.rand.Next(terrain.Height);
        }

        public CritterType GetCritterType
        {
            get
            {
                return critterType;
            }
        }

        public int X
        {
            get { return x; }
            set { x = (int)value; }
        }
        public int Y
        {
            get { return y; }
            set { y = (int)value; }
        }

        internal void Update(Terrain terrain)
        {
            if (critterType == CritterType.Diver)
            {
                if (!fertile)
                {
                    GoLow(terrain);
                }
                else
                {
                    GoHigh(terrain);
                }
            }
            else if (critterType == CritterType.Climber)
            {
                if (!fertile)
                {
                    GoHigh(terrain);
                }
                else
                {
                    GoLow(terrain);
                }
            }

            //if (Game1.rand.NextDouble() < 0.001)
            //if (Game1.rand.NextDouble() < 0.01)
            //{
                //critterType = (CritterType)Game1.rand.Next(2);
            //}

            //if (Game1.rand.NextDouble() < 0.01)
            //{
                //Reproduce(terrain);
            //}

            ToDiePerchance(terrain);

            age++;
            
            if (age == 200)
            {
                fertile = true;
            }
            if (age == 1000)
            {
                fertile = false;
            }
        }

        private void ToDiePerchance(Terrain terrain)
        {
            if (terrain.IsOccupiedNorthOf(x, y) && terrain.IsOccupiedSouthOf(x, y) && terrain.IsOccupiedWestOf(x, y) && terrain.IsOccupiedEastOf(x, y))
            {
                if (Game1.rand.NextDouble() < age * 0.001)
                {
                    Die(terrain);
                }
            }
        }

        private void Die(Terrain terrain)
        {
            terrain.RemoveCritter(this);
        }

        private void Reproduce(Terrain terrain)
        {
            int r = Game1.rand.Next(4);
            switch (r)
            {
                case 0:
                    if (!terrain.IsOccupiedNorthOf(x, y)) terrain.AddCritter(x, y - 1, CloneMe());
                    break;
                case 1:
                    if (!terrain.IsOccupiedSouthOf(x, y)) terrain.AddCritter(x, y + 1, CloneMe());
                    break;
                case 2:
                    if (!terrain.IsOccupiedWestOf(x, y)) terrain.AddCritter(x - 1, y, CloneMe());
                    break;
                case 3:
                    if (!terrain.IsOccupiedEastOf(x, y)) terrain.AddCritter(x + 1, y, CloneMe());
                    break;
            }
        }

        private Critter CloneMe()
        {
            Critter c = new Critter();
            c.SetCritterType = critterType;
            return c;
        }

        private void GoLow(Terrain terrain)
        {
            //uint lowest = terrain.Altitude(x, y); //Stays at lowest point.
            uint lowest = uint.MaxValue; //Goes back and forth between lowest points.
            int xMove = 0;
            int yMove = 0;

            for (int i = 0, j = Game1.rand.Next(4); i < 4; i++)
            {
                switch (j % 4)
                {
                    case 0:
                        //if (y > 0 && terrain.AltitudeNorthOf(x, y) < lowest && !terrain.IsOccupied(x, y - 1))
                        if (y > 0 && terrain.AltitudeNorthOf(x, y) < lowest && !terrain.IsOccupiedNorthOf(x, y))
                        {
                            lowest = terrain.AltitudeNorthOf(x, y);
                            yMove = -1;
                            xMove = 0;
                        }
                        break;
                    case 1:
                        if (x > 0 && terrain.AltitudeWestOf(x, y) < lowest && !terrain.IsOccupiedWestOf(x, y))
                        {
                            lowest = terrain.AltitudeWestOf(x, y);
                            xMove = -1;
                            yMove = 0;
                        }
                        break;
                    case 2:
                        if (y < terrain.Height - 1 && terrain.AltitudeSouthOf(x, y) < lowest && !terrain.IsOccupiedSouthOf(x, y))
                        {
                            lowest = terrain.AltitudeSouthOf(x, y);
                            yMove = 1;
                            xMove = 0;
                        }
                        break;
                    case 3:
                        if (x < terrain.Width - 1 && terrain.AltitudeEastOf(x, y) < lowest && !terrain.IsOccupiedEastOf(x, y))
                        {
                            lowest = terrain.AltitudeEastOf(x, y);
                            xMove = 1;
                            yMove = 0;
                        }
                        break;
                    default:
                        break;
                }
                j++;
            }

            terrain.Vacate(x, y);
            x += xMove;
            y += yMove;
            terrain.Occupy(x, y, this);
        }

        private void GoHigh(Terrain terrain)
        {
            //uint highest = terrain.Altitude(x, y); //Stays at highest point.
            uint highest = 0; //Goes back and forth between highest points.
            int xMove = 0;
            int yMove = 0;

            for (int i = 0, j = Game1.rand.Next(4); i < 4; i++)
            {
                switch (j % 4)
                {
                    case 0:
                        if (y > 0 && terrain.AltitudeNorthOf(x, y) > highest && !terrain.IsOccupied(x, y - 1))
                        {
                            highest = terrain.AltitudeNorthOf(x, y);
                            yMove = -1;
                            xMove = 0;
                        }
                        break;
                    case 1:
                        if (x > 0 && terrain.AltitudeWestOf(x, y) > highest && !terrain.IsOccupied(x - 1, y))
                        {
                            highest = terrain.AltitudeWestOf(x, y);
                            xMove = -1;
                            yMove = 0;
                        }
                        break;
                    case 2:
                        if (y < terrain.Height - 1 && terrain.AltitudeSouthOf(x, y) > highest && !terrain.IsOccupied(x, y + 1))
                        {
                            highest = terrain.AltitudeSouthOf(x, y);
                            yMove = 1;
                            xMove = 0;
                        }
                        break;
                    case 3:
                        if (x < terrain.Width - 1 && terrain.AltitudeEastOf(x, y) > highest && !terrain.IsOccupied(x + 1, y))
                        {
                            highest = terrain.AltitudeEastOf(x, y);
                            xMove = 1;
                            yMove = 0;
                        }
                        break;
                    default:
                        break;
                }
                j++;
            }

            terrain.Vacate(x, y);
            x += xMove;
            y += yMove;
            terrain.Occupy(x, y, this);
        }

        public CritterType SetCritterType { set { critterType = value; } }
    }
}
