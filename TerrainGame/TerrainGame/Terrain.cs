using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace TerrainGame
{
    class Terrain
    {
        private uint[] terrainHeightMap;
        private Critter[] occupiedBy;
        private List<Critter> allTheCritters;
        private int width;
        private int height;

        public uint[] TerrainHeightMap
        {
            get
            {
                return terrainHeightMap;
            }
        }

        public int Width
        {
            get
            {
                return width;
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
        }

        public Terrain()
        {
            width = 0;
            height = 0;
            terrainHeightMap = new uint[0];
            occupiedBy = new Critter[0];
            allTheCritters = new List<Critter>();
        }

        public Terrain(int w, int h)
        {
            width = w;
            height = h;
            terrainHeightMap = new uint[w * h];
            occupiedBy = new Critter[w * h];
            allTheCritters = new List<Critter>();
        }

        internal Texture2D ToAbgrTexture(GraphicsDevice device)
        {
            Texture2D texture = new Texture2D(device, width, height);
            texture.SetData(OpaqueTerrain());
            return texture;
        }

        /// <summary>
        /// Returns the height map with 0xFF000000 added to each location
        /// in order to set the alpha channel to opaque.
        /// </summary>
        private uint[] OpaqueTerrain()
        {
            uint[] oT = new uint[terrainHeightMap.Count()];
            for (int i = 0; i < oT.Count(); i++)
            {
                oT[i] = terrainHeightMap[i] + 0xFF000000;
            }
            return oT;
        }

        internal void Randomize()
        {
            for (int i = 0; i < terrainHeightMap.Count(); i++)
            {
                //terrainHeightMap[i] = (uint)Game1.rand.Next(255); //Red terrain.
                terrainHeightMap[i] = (uint)Game1.rand.Next(255) * 256; //For green terrain.
                //terrainHeightMap[i] = (uint)Game1.rand.Next(255) * 256 * 256; //For blue terrain. 
            }
        }

        internal uint Altitude(int x, int y)
        {
            return terrainHeightMap[x + y * width];
        }

        /// <summary>
        /// Returns the height/altitude of the location north of the supplied x, y coordinate.
        /// If the coordinate is in the top row of the map, uint.MaxValue is returned.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        internal uint AltitudeNorthOf(int x, int y)
        {
            if (y - 1 >= 0)
            {
                return Altitude(x, y - 1);
            }
            else
            {
                return uint.MaxValue;
            }
        }
        
        /// <summary>
        /// Returns the height/altitude of the location south of the supplied x, y coordinate.
        /// If the coordinate is in the bottom row of the map, uint.MaxValue is returned.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        internal uint AltitudeSouthOf(int x, int y)
        {
            if (y + 1 < height)
            {
                return Altitude(x, y + 1);
            }
            else
            {
                return uint.MaxValue;
            }
        }
        
        /// <summary>
        /// Returns the height/altitude of the location west of the supplied x, y coordinate.
        /// If the coordinate is in the leftmost column of the map, uint.MaxValue is returned.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        internal uint AltitudeWestOf(int x, int y)
        {
            if (x - 1 >= 0)
            {
                return Altitude(x - 1, y);
            }
            else
            {
                return uint.MaxValue;
            }
        }
        
        /// <summary>
        /// Returns the height/altitude of the location east of the supplied x, y coordinate.
        /// If the coordinate is in the rightmost column of the map, uint.MaxValue is returned.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        internal uint AltitudeEastOf(int x, int y)
        {
            if (x + 1 < width)
            {
                return Altitude(x + 1, y);
            }
            else
            {
                return uint.MaxValue;
            }
        }

        /// <summary>
        /// Smoothes the terrain.
        /// Better smoothing than SmoothLinearly(), but much slower.
        /// </summary>
        internal void SmoothRandomly()
        {
            List<int> unSmoothedLocations = new List<int>();
            for (int i = 0; i < terrainHeightMap.Count(); i++)
            {
                unSmoothedLocations.Add(i);
            }

            while (unSmoothedLocations.Count > 0)
            {
                int randInt = Game1.rand.Next(unSmoothedLocations.Count);
                int locationToSmooth = unSmoothedLocations[randInt];
                unSmoothedLocations.RemoveAt(randInt);
                AverageFromNeighbors(locationToSmooth);
            }
        }

        /// <summary>
        /// Smoothes the terrain.
        /// Not as smooth, but much faster than SmoothRandomly().
        /// </summary>
        internal void SmoothLinearly()
        {
            for (int i = 0; i < terrainHeightMap.Count(); i++)
            {
                AverageFromNeighbors(i);
            }
        }

        /// <summary>
        /// Sets the supplied location to the average of its neighbors to the north, south, west, and east.
        /// </summary>
        /// <param name="locationToSmooth">The location on the map as an int (x + y * width).</param>
        private void AverageFromNeighbors(int locationToSmooth)
        {
            int neighborsFound = 0;
            uint sum = 0;
            if (locationToSmooth / width > 0)
            {
                sum += terrainHeightMap[locationToSmooth - width];
                neighborsFound++;
            }
            if (locationToSmooth / width < height - 1)
            {
                sum += terrainHeightMap[locationToSmooth + width];
                neighborsFound++;
            }
            if (locationToSmooth % width > 0)
            {
                sum += terrainHeightMap[locationToSmooth - 1];
                neighborsFound++;
            }
            if (locationToSmooth % width < width - 1)
            {
                sum += terrainHeightMap[locationToSmooth + 1];
                neighborsFound++;
            }
            terrainHeightMap[locationToSmooth] = (uint)(sum / neighborsFound);
        }

        /// <summary>
        /// Sets the x, y coordinate supplied as being occupied by the critter supplied.
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <param name="critter">The critter that will occupy this location.</param>
        internal void Occupy(int x, int y, Critter critter)
        {
            occupiedBy[x + y * width] = critter;
        }

        /// <summary>
        /// Clears the x, y coordinate supplied of its occupant.
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        internal void Vacate(int x, int y)
        {
            occupiedBy[x + y * width] = null;
        }

        /// <summary>
        /// Check if the supplied coordinate is occupied.
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <returns></returns>
        internal bool IsOccupied(int x, int y)
        {
            return occupiedBy[x + y * width] != null;
        }

        /// <summary>
        /// Gets rid of all the critters.
        /// </summary>
        internal void ClearOccupants()
        {
            occupiedBy = new Critter[width * height];
            allTheCritters.Clear();
        }

        /// <summary>
        /// Creates a critter and places it at a random, unoccupied coordinate.
        /// </summary>
        internal void AddNewRandomCritter()
        {
            Critter c = new Critter();
            bool success = false;
            do
            {
                int x = Game1.rand.Next(width);
                int y = Game1.rand.Next(height);
                if (!IsOccupied(x, y))
                {
                    c.X = x;
                    c.Y = y;
                    Occupy(x, y, c);
                    allTheCritters.Add(c);
                    success = true;
                }
            } while (!success);
        }

        internal void Update()
        {
            foreach (Critter c in allTheCritters.ToArray())
            {
                c.Update(this);
            }
        }

        public IEnumerable<Critter> AllTheCritters { get { return allTheCritters; } }

        internal bool IsOccupiedNorthOf(int x, int y)
        {
            if (y == 0) return true;
            return IsOccupied(x, y - 1);
        }
        internal bool IsOccupiedSouthOf(int x, int y)
        {
            if (y == height - 1) return true;
            return IsOccupied(x, y + 1);
        }
        internal bool IsOccupiedWestOf(int x, int y)
        {
            if (x == 0) return true;
            return IsOccupied(x - 1, y);
        }
        internal bool IsOccupiedEastOf(int x, int y)
        {
            if (x == width - 1) return true;
            return IsOccupied(x + 1, y);
        }

        internal void AddCritter(int x, int y, Critter critter)
        {
            critter.X = x;
            critter.Y = y;
            Occupy(x, y, critter);
            allTheCritters.Add(critter);
        }

        internal void RemoveCritter(Critter critter)
        {
            Vacate(critter.X, critter.Y);
            allTheCritters.Remove(critter);
        }
    }
}
