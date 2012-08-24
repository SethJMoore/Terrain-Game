using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TerrainGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        const int SCALE = 4;
        int updateFrequency;

        public static Random rand;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        int mapWidth, mapHeight;
        Terrain terrain;
        Texture2D terrainTexture;

        Texture2D critterTexture1;
        Texture2D critterTexture2;

        Texture2D riderTexture;

        KeyboardState previousKeyboardState;

        uint turnNumber;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            turnNumber = 0;
            rand = new Random();
            mapWidth = 200;
            mapHeight = 200;
            terrain = new Terrain(mapWidth, mapHeight);
            terrain.Randomize();
            for (int i = 0; i < 200; i++)
            {
                terrain.SmoothLinearly();
                //terrain.SmoothRandomly();
            }
            terrainTexture = terrain.ToAbgrTexture(graphics.GraphicsDevice);


            graphics.PreferredBackBufferHeight = mapHeight * SCALE;
            graphics.PreferredBackBufferWidth = mapWidth * SCALE;
            graphics.ApplyChanges();
            this.IsMouseVisible = true;

            updateFrequency = 1; // 1 is fastest. Higher is slower.

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            Color[] cT = new Color[1] { Color.Blue };
            critterTexture1 = new Texture2D(GraphicsDevice, 1, 1);
            critterTexture1.SetData(cT);
            cT[0] = Color.Red;
            critterTexture2 = new Texture2D(GraphicsDevice, 1, 1);
            critterTexture2.SetData(cT);

            cT[0] = Color.White;
            riderTexture = new Texture2D(GraphicsDevice, 1, 1);
            riderTexture.SetData(cT);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            if (currentKeyboardState.IsKeyDown(Keys.Space) && previousKeyboardState.IsKeyUp(Keys.Space)) //Space randomizes the map.
            {
                terrain.Randomize();
                terrainTexture = terrain.ToAbgrTexture(GraphicsDevice);
            }

            if (currentKeyboardState.IsKeyDown(Keys.S)) //S for Smooth.
            {
                //terrain.SmoothRandomly();
                terrain.SmoothLinearly();
                terrainTexture = terrain.ToAbgrTexture(GraphicsDevice);
            }

            if (currentKeyboardState.IsKeyDown(Keys.V) && previousKeyboardState.IsKeyUp(Keys.V)) //V pastes a new critter to a random location.
            {
                terrain.AddNewRandomCritter();
                //Critter c = new Critter();
                //c.PlaceRandomly(terrain);
                //critters.Add(c);
            }

            if (currentKeyboardState.IsKeyDown(Keys.OemPlus) && previousKeyboardState.IsKeyUp(Keys.OemPlus)) //Plus key increases speed of game.
            {
                if (updateFrequency > 1)
                {
                    updateFrequency--;
                }
            }

            if (currentKeyboardState.IsKeyDown(Keys.OemMinus) && previousKeyboardState.IsKeyUp(Keys.OemMinus)) //Minus key decreases speed of game.
            {
                updateFrequency++;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Up) && previousKeyboardState.IsKeyUp(Keys.Up)) //Up key changes mount to the critter to the north.
            {
                if (terrain.OccupiedByNorthOf(terrain.TheRider.X, terrain.TheRider.Y) != null)
                {
                    terrain.TheRider.MountCritter(terrain.OccupiedByNorthOf(terrain.TheRider.X, terrain.TheRider.Y));
                }
            }

            if (currentKeyboardState.IsKeyDown(Keys.Down) && previousKeyboardState.IsKeyUp(Keys.Down)) //Down key changes mount to the critter to the south.
            {
                if (terrain.OccupiedBySouthOf(terrain.TheRider.X, terrain.TheRider.Y) != null)
                {
                    terrain.TheRider.MountCritter(terrain.OccupiedBySouthOf(terrain.TheRider.X, terrain.TheRider.Y));
                }
            }

            if (currentKeyboardState.IsKeyDown(Keys.Left) && previousKeyboardState.IsKeyUp(Keys.Left)) //Left key changes mount to the critter to the west.
            {
                if (terrain.OccupiedByWestOf(terrain.TheRider.X, terrain.TheRider.Y) != null)
                {
                    terrain.TheRider.MountCritter(terrain.OccupiedByWestOf(terrain.TheRider.X, terrain.TheRider.Y));
                }
            }

            if (currentKeyboardState.IsKeyDown(Keys.Right) && previousKeyboardState.IsKeyUp(Keys.Right)) //Right key changes mount to the critter to the east.
            {
                if (terrain.OccupiedByEastOf(terrain.TheRider.X, terrain.TheRider.Y) != null)
                {
                    terrain.TheRider.MountCritter(terrain.OccupiedByEastOf(terrain.TheRider.X, terrain.TheRider.Y));
                }
            }

            if (currentKeyboardState.IsKeyDown(Keys.P) && previousKeyboardState.IsKeyUp(Keys.P)) //P populates the terrain with 100 new randomly placed critters.
            {
                for (int i = 0; i < 100; i++)
                {
                    terrain.AddNewRandomCritter();
                }
            }

            if (currentKeyboardState.IsKeyDown(Keys.Escape) && previousKeyboardState.IsKeyUp(Keys.Escape)) //Escape randomizes the map and gets rid of all the critters.
            {
                terrain.Randomize();
                terrainTexture = terrain.ToAbgrTexture(graphics.GraphicsDevice);

                terrain.ClearOccupants();
            }


            if (turnNumber % updateFrequency == 0)
            {
                terrain.Update();

                //Erosion of the terrain every so many turns.
                if (turnNumber / updateFrequency % 100 == 0)
                {
                    terrain.SmoothLinearly();
                    terrainTexture = terrain.ToAbgrTexture(GraphicsDevice);
                }
            }
            turnNumber++;

            previousKeyboardState = currentKeyboardState;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            //spriteBatch.Draw(terrainTexture, new Vector2(0, 0), Color.White); //One pixel per location.
            spriteBatch.Draw(terrainTexture, new Vector2(0, 0), null, Color.White, 0, new Vector2(0, 0),new Vector2(SCALE,SCALE), SpriteEffects.None,1);
            foreach (Critter c in terrain.AllTheCritters)
            {
                if (c.GetCritterType == Critter.CritterType.Climber)
                {
                    //spriteBatch.Draw(critterTexture1, new Vector2(c.X, c.Y), Color.White); //One pixel per critter.
                    spriteBatch.Draw(critterTexture1, new Vector2(c.X * SCALE, c.Y * SCALE), null, Color.White, 0, new Vector2(0, 0), new Vector2(SCALE, SCALE), SpriteEffects.None, 1);
                }
                else
                {
                    //spriteBatch.Draw(critterTexture2, new Vector2(c.X, c.Y), Color.White); //One pixel per critter.
                    spriteBatch.Draw(critterTexture2, new Vector2(c.X * SCALE, c.Y * SCALE), null, Color.White, 0, new Vector2(0, 0), new Vector2(SCALE, SCALE), SpriteEffects.None, 1);
                }
            }

            //spriteBatch.Draw(riderTexture, new Vector2(terrain.TheRider.X * 2, terrain.TheRider.Y * 2), Color.White); //One pixel for the rider.
            spriteBatch.Draw(riderTexture, new Vector2(terrain.TheRider.X * SCALE, terrain.TheRider.Y * SCALE), null, Color.White, 0, new Vector2(0, 0), new Vector2(SCALE, SCALE), SpriteEffects.None, 1);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
