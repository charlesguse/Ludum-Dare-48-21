using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RunEscape
{
    class Birds
    {
        public bool Alive = true;
        List<Bird> birds;
        public int CurrentSpeed { get; set; }

        public Birds(Word word)
        {
            Random random = new Random();
            birds = new List<Bird>();
            for (int i = 0; i * 15 < word.Collision.Width; i++)
            {
                var birdPosition = new Vector2();
                birdPosition.X = word.Position.X + i * 15;
                birdPosition.Y = word.Position.Y;
                birds.Add(new Bird(birdPosition, word.Font));
            }
        }

        public void Update(GameTime gameTime)
        {
            var tempDeadList = new List<Bird>();
            foreach (var bird in birds)
            {
                bird.CurrentSpeed = CurrentSpeed;
                bird.Update(gameTime);
                if (!bird.Alive)
                    tempDeadList.Add(bird);
            }
            foreach (var dead in tempDeadList)
                birds.Remove(dead);

            if (birds.Count == 0)
                Alive = false;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var bird in birds)
                bird.Draw(gameTime, spriteBatch);
        }
    }
}
