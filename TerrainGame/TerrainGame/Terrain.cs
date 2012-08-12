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

        /// <summary>
        /// Generates a gradient height map. Red on the horizontal and green on the vertical.
        /// </summary>
        /// <param name="w">The width of the map</param>
        /// <param name="h">The height (y-axis) of the height map</param>
        internal void GenerateRedGreen(int w, int h)
        {
            width = w;
            height = h;
            terrainHeightMap = new uint[width * height];
            for (uint i = 0; i < height; i++)
            {
                for (uint j = 0; j < width; j++)
                {
                    terrainHeightMap[i * width + j] = i + (j * 256) + 0xFF000000;
                }
                //terrainData[i].A = 255;
                //terrainData[i].R = (byte)(i / 256);
                //terrainData[i].G = (byte)(i % 256);
                //terrainData[i].B = 0;
            }
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
                //terrainHeightMap[i] = (uint)Game1.rand.Next(255);
                terrainHeightMap[i] = (uint)Game1.rand.Next(255) * 256; //For green terrain.
                //terrainHeightMap[i] = (uint)Game1.rand.Next(255) * 256 * 256; //For blue terrain. 
            }
        }

        internal uint Altitude(int x, int y)
        {
            return terrainHeightMap[x + y * width];
        }

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

        internal void SmoothRandomly() //Better smoothing than SmoothLinearly(), but much slower.
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

        internal void SmoothLinearly() //Not as smooth, but much faster than SmoothRandomly().
        {
            for (int i = 0; i < terrainHeightMap.Count(); i++)
            {
                AverageFromNeighbors(i);
            }
        }

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

        internal void Occupy(int x, int y, Critter critter)
        {
            occupiedBy[x + y * width] = critter;
        }

        internal void Vacate(int x, int y)
        {
            occupiedBy[x + y * width] = null;
        }

        internal bool IsOccupied(int x, int y)
        {
            return occupiedBy[x + y * width] != null;
        }

        internal void ClearOccupants()
        {
            occupiedBy = new Critter[width * height];
            allTheCritters.Clear();
        }

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
    }
}
