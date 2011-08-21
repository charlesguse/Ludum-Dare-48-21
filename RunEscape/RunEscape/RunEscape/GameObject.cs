using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RunEscape
{
    abstract class GameObject
    {
        public Vector2 Position;
        public Rectangle Collision;
        public Texture2D Texture;

        public bool Alive = true;

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
