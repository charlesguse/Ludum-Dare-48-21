using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RunEscape
{
    class Player : GameObject
    {
        const float JUMP_MAX = 0.7f;
        const float JUMP_APEX_MAX = 0.1f;

        PlayerAction currentAction;
        string DrawAction
        {
            get 
            {
                switch (currentAction)
	            {
		            case PlayerAction.Stopped:
                        return "\\s";
                    case PlayerAction.Running:
                        return "\\r";
                    case PlayerAction.Jumping:
                        return "\\j";
                    case PlayerAction.Falling:
                        return "\\f";
                    default:
                        return "\\d";
                }
            }
        }

        bool previousEscapeWasPressed;
        public SpriteFont font;
        private float jumpTimer;
        private float jumpApexTimer;
        private PlayerAction previousAction;
        public int Speed { get; set; }
        public int Bottom { get; set; }

        WordManager WordManager { get; set; }

        public Player(Vector2 startingPosition, WordManager wordManager, int bottom)
        {
            currentAction = PlayerAction.Stopped;
            Speed = 0;
            this.Bottom = bottom;
            Position = startingPosition;
            Position.X += 1;
            this.WordManager = wordManager;
        }

        public void LoadContent(ContentManager content)
        {
            this.Texture = content.Load<Texture2D>("ObjectBackground");
            this.font = content.Load<SpriteFont>("Word");

            Vector2 textSize = this.font.MeasureString(DrawAction);

            Position.Y -= textSize.Y;

            Collision = new Rectangle();

            UpdateCollisionRect();
        }

        private void UpdateCollisionRect()
        {
            var currentString = string.Format("{0}", DrawAction);

            Vector2 textSize = this.font.MeasureString(currentString);

            Collision.X = (int)this.Position.X;
            Collision.Y = (int)this.Position.Y;
            Collision.Width = (int)textSize.X;
            Collision.Height = (int)textSize.Y;
        }

        public override void Update(GameTime gameTime)
        {
            HandleInput(gameTime);

            if (currentAction == PlayerAction.Running || currentAction == PlayerAction.Jumping)
                Speed = 5;
            else if (currentAction == PlayerAction.Stopped)
                Speed = 0;

            if (currentAction == PlayerAction.Running || currentAction == PlayerAction.Falling)
            {
                Position.Y += 10;
                currentAction = PlayerAction.Falling;
            }

            UpdateCollisionRect();
            CheckCollision();
            UpdateCollisionRect();

            if (Position.Y > Bottom)
            {
                Alive = false;
                Speed = 0;
            }
        }

        public void CheckCollision()
        {
            foreach (var word in WordManager.StreamingWords)
            {
                if (word.Collision.Intersects(this.Collision))
                {
                    if (word.Position.X < this.Position.X + this.Collision.Width && word.Position.Y < this.Position.Y)
                    {
                        //this.Position.X = word.Position.X - this.Collision.Width;
                        //currentAction = PlayerAction.Falling;
                        //Speed = 0;
                    }
                    else if (word.Position.Y < this.Position.Y + this.Collision.Height)
                    {
                        this.Position.Y = word.Position.Y - this.Collision.Height;
                        jumpTimer = 0.0f;
                        if (currentAction != PlayerAction.Stopped)
                            currentAction = PlayerAction.Running;
                    }

                    break;
                }
            }
        }

        // This code is UGLY. But Meh, it gets the job done (barely).
        public void HandleInput(GameTime gameTime)
        {
            bool jumpHigher = false;
            KeyboardState keyState = Keyboard.GetState();

            bool currentEscapePress = keyState.IsKeyDown(Keys.Escape);

            if (currentEscapePress && !previousEscapeWasPressed)
            {
                if (currentAction == PlayerAction.Stopped)
                    currentAction = PlayerAction.Running;
                else if (currentAction == PlayerAction.Running)
                {
                    currentAction = PlayerAction.Jumping;
                    jumpHigher = true;
                    jumpTimer = 0.0f;
                    jumpApexTimer = 0.0f;
                }
            }
            else if (currentEscapePress && previousEscapeWasPressed && currentAction == PlayerAction.Jumping)
                jumpHigher = true;

            if (currentAction == PlayerAction.Running && keyState.IsKeyDown(Keys.Space))
                currentAction = PlayerAction.Stopped;

            if (!currentEscapePress)
            {
                previousEscapeWasPressed = false;
                if (previousAction == PlayerAction.Stopped && currentAction == PlayerAction.Running)
                    previousAction = currentAction;
            }

            if (jumpHigher)
                StartOrContinueJump(gameTime);
            else if (!jumpHigher && currentAction == PlayerAction.Jumping)
                StopJumping(gameTime);

            previousEscapeWasPressed = keyState.IsKeyDown(Keys.Escape);
        }

        private void StopJumping(GameTime gameTime)
        {
            jumpTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (jumpApexTimer < JUMP_APEX_MAX)
            {
                jumpApexTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                Position.Y -= 3;
            }
            else
                Position.Y += 10;
        }

        private void StartOrContinueJump(GameTime gameTime)
        {
            // Maybe I need sleep, but jump looks like a weird word now. Just look at it...
            if (jumpTimer < JUMP_MAX && jumpApexTimer == 0.0f)
            {
                jumpTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                Position.Y -= 8;
            }
            else
                StopJumping(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Collision, Color.BurlyWood);
            spriteBatch.DrawString(font, DrawAction, Position, Color.Black);
        }
    }
}
