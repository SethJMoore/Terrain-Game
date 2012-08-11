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
        public static Random rand;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        int mapWidth, mapHeight;
        Terrain terrain;
        Texture2D terrainTexture;

        //Critter critter;
        List<Critter> critters;
        Texture2D critterTexture1;
        Texture2D critterTexture2;

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
            rand = new Random();
            mapWidth = 300;
            mapHeight = 300;
            //terrain = new Terrain();
            //terrain.GenerateRedGreen(mapWidth, mapHeight);
            terrain = new Terrain(mapWidth, mapHeight);
            terrain.Randomize();
            for (int i = 0; i < 200; i++)
            {
                terrain.SmoothLinearly();
            }
            terrainTexture = terrain.ToAbgrTexture(graphics.GraphicsDevice);

            critters = new List<Critter>();
            critters.Add(new Critter());
            critters[0].PlaceRandomly(terrain);

            graphics.PreferredBackBufferHeight = mapHeight * 2;
            graphics.PreferredBackBufferWidth = mapWidth * 2;
            graphics.ApplyChanges();
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                terrain.Randomize();
                terrainTexture = terrain.ToAbgrTexture(GraphicsDevice);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.S)) //S for Smooth.
            {
                //terrain.SmoothRandomly();
                terrain.SmoothLinearly();
                terrainTexture = terrain.ToAbgrTexture(GraphicsDevice);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.C))
            {
                critters[0].PlaceRandomly(terrain);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.V))
            {
                Critter c = new Critter();
                c.PlaceRandomly(terrain);
                critters.Add(c);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.P))
            {
                for (int i = 0; i < 100; i++)
                {
                    Critter c = new Critter();
                    c.PlaceRandomly(terrain);
                    critters.Add(c);
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                terrain.Randomize();
                terrainTexture = terrain.ToAbgrTexture(graphics.GraphicsDevice);

                critters.Clear();
                terrain.ClearOccupants();
            }

            foreach (Critter c in critters)
            {
                c.Update(terrain);
            }
            //critters[0].GoLow(terrain);

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
            spriteBatch.Draw(terrainTexture, new Vector2(0, 0), null, Color.White, 0, new Vector2(0, 0),new Vector2(2,2), SpriteEffects.None,1);
            foreach (Critter c in critters)
            {
                if (c.GetCritterType == Critter.CritterType.Climber)
                {
                    //spriteBatch.Draw(critterTexture1, new Vector2(c.X, c.Y), Color.White); //One pixel per critter.
                    spriteBatch.Draw(critterTexture1, new Vector2(c.X * 2, c.Y * 2), null, Color.White, 0, new Vector2(0, 0), new Vector2(2, 2), SpriteEffects.None, 1);
                }
                else
                {
                    //spriteBatch.Draw(critterTexture2, new Vector2(c.X, c.Y), Color.White); //One pixel per critter.
                    spriteBatch.Draw(critterTexture2, new Vector2(c.X * 2, c.Y * 2), null, Color.White, 0, new Vector2(0, 0), new Vector2(2, 2), SpriteEffects.None, 1);
                }
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
