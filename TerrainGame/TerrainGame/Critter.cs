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

        public Critter()
        {
            x = 0;
            y = 0;
            critterType = CritterType.Diver;
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

        public float X
        {
            get { return x; }
            set { x = (int)value; }
        }
        public float Y
        {
            get { return y; }
            set { y = (int)value; }
        }

        internal void Update(Terrain terrain)
        {
            if (critterType == CritterType.Diver)
            {
                GoLow(terrain);
            }
            else if (critterType == CritterType.Climber)
            {
                GoHigh(terrain);
            }

            if (Game1.rand.Next(100) < 1)
            {
                critterType = (CritterType)Game1.rand.Next(2);
            }
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
                        if (y > 0 && terrain.AltitudeNorthOf(x, y) < lowest && !terrain.IsOccupied(x, y - 1))
                        {
                            lowest = terrain.AltitudeNorthOf(x, y);
                            yMove = -1;
                            xMove = 0;
                        }
                        break;
                    case 1:
                        if (x > 0 && terrain.AltitudeWestOf(x, y) < lowest && !terrain.IsOccupied(x - 1, y))
                        {
                            lowest = terrain.AltitudeWestOf(x, y);
                            xMove = -1;
                            yMove = 0;
                        }
                        break;
                    case 2:
                        if (y < terrain.Height - 1 && terrain.AltitudeSouthOf(x, y) < lowest && !terrain.IsOccupied(x, y + 1))
                        {
                            lowest = terrain.AltitudeSouthOf(x, y);
                            yMove = 1;
                            xMove = 0;
                        }
                        break;
                    case 3:
                        if (x < terrain.Width - 1 && terrain.AltitudeEastOf(x, y) < lowest && !terrain.IsOccupied(x + 1, y))
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
    }
}
