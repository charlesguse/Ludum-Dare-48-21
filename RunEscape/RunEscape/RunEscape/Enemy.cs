using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace RunEscape
{
    class Enemy : GameObject
    {
        public Enemy(Vector2 drawPosition)
        {
            Position = drawPosition;
        }

        public void LoadContent(ContentManager content)
        {
            this.Texture = content.Load<Texture2D>("Enemy");
        }

        public override void Update(GameTime gameTime)
        { }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }
    }
}
