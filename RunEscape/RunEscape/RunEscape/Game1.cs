//Onto the main game driver!
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RunEscape
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        Player player;
        WordManager wordManager;
        static long TARGET_FRAME_RATE = 60;
        private Enemy enemy;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 640;
            graphics.PreferredBackBufferHeight = 480;
            TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / TARGET_FRAME_RATE);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            font = Content.Load<SpriteFont>("Word");
            wordManager = new WordManager(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            player = new Player(new Vector2(graphics.PreferredBackBufferWidth / 3, wordManager.StartingPosition.Y), wordManager, graphics.PreferredBackBufferHeight);
            enemy = new Enemy(new Vector2(0, graphics.PreferredBackBufferHeight * 2 / 3));
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            player.LoadContent(Content);
            wordManager.LoadContent(Content);
            enemy.LoadContent(Content);
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            // NAAAAAAAAAAAAAH, Memdor, memorating the country side, memorating the peasants.
        }

        // This is where the magic happens ;)
        protected override void Update(GameTime gameTime)
        {
            if (player.Alive)
            {
                player.Update(gameTime);
                wordManager.Update(gameTime, player.Speed);
            }
            else
            {
                KeyboardState keyState = Keyboard.GetState();

                if (keyState.IsKeyDown(Keys.Escape))
                    Restart();
            }

            base.Update(gameTime);
        }

        // This is where you see the magic happen ;)
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightSlateGray);

            spriteBatch.Begin();
            wordManager.Draw(gameTime, spriteBatch);
            enemy.Draw(gameTime, spriteBatch);
            player.Draw(gameTime, spriteBatch);
            if (!player.Alive)
                DrawRestartInstructions();
            spriteBatch.End();
            base.Draw(gameTime);
        }

        public void DrawRestartInstructions()
        {
            string instructions = "Press Escape to Restart";
            Vector2 drawPosition = new Vector2();
            drawPosition.X = graphics.PreferredBackBufferWidth / 2;
            drawPosition.Y = graphics.PreferredBackBufferHeight / 2;

            Vector2 stringSize = font.MeasureString(instructions);

            drawPosition.X -= stringSize.X / 2;
            spriteBatch.DrawString(font, instructions, drawPosition, Color.Black);
        }

        public void Restart()
        {
            Initialize();
            //LoadContent();
        }
    }
}
