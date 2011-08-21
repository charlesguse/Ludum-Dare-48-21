using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace RunEscape
{
    class WordManager
    {
        int SPACE_GAP = 60;
        int NEWLINE_GAP = 200;

        public Vector2 StartingPosition;
        Vector2 levelInformationPosition;

        public Queue<Word> StreamingWords;
        Queue<LevelWordPair> UpcomingWords;
        Queue<Birds> Birds;

        public int Score { get; set; }

        private int width;
        private int height;
        private SpriteFont font;
        private Texture2D texture;

        public WordManager(int width, int height)
        {
            this.width = width;
            this.height = height;

            StartingPosition = new Vector2(width / 8, height * 4 / 5);
            levelInformationPosition = new Vector2(width / 2, height / 8);

            StreamingWords = new Queue<Word>();
            UpcomingWords = new Queue<LevelWordPair>();
            Birds = new Queue<Birds>();
        }

        public void LoadContent(ContentManager content)
        {
            this.texture = content.Load<Texture2D>("ObjectBackground");
            this.font = content.Load<SpriteFont>("Word");

            string[] filePaths = Directory.GetFiles(".", "*.cs");
            var levels = new Dictionary<String, Queue<string>>();


            foreach (var path in filePaths)
            {
                string text = System.IO.File.ReadAllText(path);
                string level = path.Split('\\').Last();
                AddWordsToQueue(text, level);
            }

            if (StreamingWords.Count == 0)
            {

                var startingWords = WordifyLine(StartingPosition);
                foreach (var word in startingWords)
                    StreamingWords.Enqueue(word);
            }
        }

        public void AddWordsToQueue(string words, string level)
        {
            var newLineDeliminatedWords = words.Split(new Char[] {'\n', '\r'});

            foreach (var line in newLineDeliminatedWords)
            {
                var spaceDeliminatedWords = line.Split(' ');

                bool comment = false;
                bool wordsAdded = false;
                foreach (var word in spaceDeliminatedWords)
                {
                    var trimmedWord = word.Trim();
                    if (trimmedWord.Length > 0)
                    {
                        if (trimmedWord.Length >= 2)
                        {
                            // This ignores the possibilty that a comment could be up against code. Oh well.
                            if (trimmedWord[0] == '/' && trimmedWord[1] == '/')
                                comment = true;
                        }
                        UpcomingWords.Enqueue(new LevelWordPair(level, trimmedWord, comment));
                        UpcomingWords.Enqueue(new LevelWordPair(level, " ", comment));
                        wordsAdded = true;
                    }
                }
                if (wordsAdded)
                    UpcomingWords.Enqueue(new LevelWordPair(level, "\n", comment));
            }
        }

        public List<Word> WordifyLine(Vector2 position)
        {
            List<Word> newWords = new List<Word>();
            LevelWordPair nextWord;

            if (UpcomingWords.Count == 0)
                return newWords;

            do
            {
                nextWord = UpcomingWords.Dequeue();
                position.X += SPACE_GAP;
            }
            while (nextWord.Word == " " && UpcomingWords.Count != 0);

            while (nextWord.Word != "\n" && UpcomingWords.Count != 0)
            {
                var newWord = new Word(nextWord, position, font, texture);
                position.X += newWord.Collision.Width;
                newWords.Add(newWord);

                do
                {
                    nextWord = UpcomingWords.Dequeue();
                    position.X += SPACE_GAP;
                }
                while (nextWord.Word == " " && UpcomingWords.Count != 0);
            }

            return newWords;
        }

        public void Update(GameTime gameTime, int speed)
        {
            foreach (var word in StreamingWords)
            {
                word.CurrentSpeed = speed;
                word.Update(gameTime);
            }

            foreach (var bird in Birds)
            {
                bird.CurrentSpeed = speed;
                bird.Update(gameTime);
            }

            AddNewWords();
            
            RemoveDeadWords();
            RemoveDeadBirds();
        }

        private void AddNewWords()
        {
            var position = StartingPosition;
            Word lastWord = null;

            if (StreamingWords.Count > 0)
            {
                lastWord = StreamingWords.Last<Word>();
                position = lastWord.Position;
            }

            int space = height - (int)position.Y;

            if (space > 0)
            {
                var newPosition = position;
                if (lastWord != null)
                    newPosition.X += NEWLINE_GAP + lastWord.Collision.Width;
                var newWords = WordifyLine(newPosition);

                foreach (var word in newWords)
                {
                    StreamingWords.Enqueue(word);
                    if (RandomProvider.Random.Next(5) < 2)
                        Birds.Enqueue(new Birds(word));
                }
            }
        }

        public void RemoveDeadWords()
        {
            while (StreamingWords.Count > 0 && !StreamingWords.Peek().Alive)
            {
                StreamingWords.Dequeue();
                Score++;
            }
        }

        public void RemoveDeadBirds()
        {
            while (Birds.Count > 0 && !Birds.Peek().Alive)
            {
                Birds.Dequeue();
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            DrawLevelInformation(gameTime, spriteBatch);
            DrawScoreInformation(gameTime, spriteBatch);

            foreach (var word in StreamingWords)
            {
                if (word.Position.X > width)
                    break;
                word.Draw(gameTime, spriteBatch);
            }
            foreach (var bird in Birds)
            {
                bird.Draw(gameTime, spriteBatch);
            }

        }

        public void DrawLevelInformation(GameTime gameTime, SpriteBatch spriteBatch)
        {
            string level;
            if (StreamingWords.Count > 0)
                level = StreamingWords.Peek().WordToDisplay.Level;
            else
                level = "Congratulations, you win!";

            var positionToDraw = levelInformationPosition;
            positionToDraw.X -= font.MeasureString(level).X / 2;

            spriteBatch.DrawString(font, level, positionToDraw, Color.White);
        }

        public void DrawScoreInformation(GameTime gameTime, SpriteBatch spriteBatch)
        {
            string score = string.Format("{0}", Score);

            var positionToDraw = new Vector2(width, 0);
            positionToDraw.X -= font.MeasureString(score).X + 10;

            spriteBatch.DrawString(font, score, positionToDraw, Color.White);
        }
    }
}
//Congratulations!_You_have_made_it_to_the_end_of_the_game._If_you_actually_made_it_this_far,_I_salute_you._I_haven't_even_tried_to_make_it_through_all_of_the_code._Make_sure_to_jump_so_that_the_Ludum_Dare_monster_will_eat_the_rest_of_the_words._JUMP!