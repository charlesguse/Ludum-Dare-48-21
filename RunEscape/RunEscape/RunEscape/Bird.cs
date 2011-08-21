using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RunEscape
{
    class Bird : GameObject
    // That's right, this game has birds. I drew inspiration from a specific one-button game.
    {
        private int THRESHHOLD = 300;
        private float ANIMATION_TIME = 0.3f;

        int TOTAL_ACTIONS = 2;

        public int CurrentSpeed;
        public SpriteFont font;
        int currentAction = 2;

        // I use the word animation losely here...
        private float animationTime;

        public string DisplayText
        {
            get
            {
                switch (currentAction)
                {
                    case 0:
                        return "W";
                    case 1:
                        return "M";
                    default:
                        return "o";
                }
            }
        }

        public Bird(Vector2 position, SpriteFont font)
        {
            this.Position = position;
            this.Position.Y -= font.MeasureString(DisplayText).Y;
            this.font = font;
        }

        public override void Update(GameTime gameTime)
        {
            if (currentAction == 2)
            {
                this.Position.X -= CurrentSpeed;

                if (this.Position.X < THRESHHOLD)
                {
                    currentAction = RandomProvider.Random.Next(TOTAL_ACTIONS);
                }
            }
            else
            {
                //currentAction = RandomProvider.Random.Next(2);
                Position.Y -= RandomProvider.Random.Next(3, 10);
                Position.X -= RandomProvider.Random.Next(-5, 5);

                animationTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (animationTime > ANIMATION_TIME)
                {
                    animationTime = 0.0f;
                    currentAction = (currentAction + 1) % TOTAL_ACTIONS;
                }
            }

            if (Position.Y < 0)
                Alive = false;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, DisplayText, Position, Color.AntiqueWhite);
        }
    }
}
