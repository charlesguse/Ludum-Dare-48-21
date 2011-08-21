using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RunEscape
{
    class Word : GameObject
    {
        // OMG! This is the container class to what you are jumping across right now!\
        public int CurrentSpeed { get; set; }
        public SpriteFont Font { get; set; }
        private Color wordColor;
        public LevelWordPair WordToDisplay { get; set; }
        //private Rectangle collision;
        //public Rectangle Collision { get { return collision; } set { collision = value; } }

        public Word(LevelWordPair word, Vector2 startingPosition, SpriteFont font, Texture2D texture)
        {
            WordToDisplay = word;
            WordToDisplay.Keyword = IsWordAKeyword();

            this.Position = startingPosition;

            this.Font = font;
            this.Texture = texture;

            Collision = new Rectangle();
            Vector2 textSize = this.Font.MeasureString(WordToDisplay.Word);

            Collision.X = (int)this.Position.X;
            Collision.Y = (int)this.Position.Y;
            Collision.Width = (int)textSize.X;
            Collision.Height = (int)textSize.Y;

            if (this.WordToDisplay.Comment)
                wordColor = Color.DarkGreen;
            else if (this.WordToDisplay.Keyword)
                wordColor = Color.Blue;
            else
                wordColor = Color.Black;
        }

        public override void Update(GameTime gameTime)
        {
            Position.X -= CurrentSpeed;
            Collision.X = (int)this.Position.X;
            Collision.Y = (int)this.Position.Y;

            if (Position.X + Collision.Width < 0)
                Alive = false;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Collision, Color.White);

            spriteBatch.DrawString(Font, WordToDisplay.Word, Position, wordColor);
        }

        public bool IsWordAKeyword()
        {
            // This won't be perfect, oh well.
            string[] keywords = new string[] {"abstract", "event", "new", "struct",
                                              "as", "explicit", "null", "switch",
                                              "base", "extern", "object", "this",
                                              "bool", "false", "operator", "throw",
                                              "break", "finally", "out", "true",
                                              "byte", "fixed", "override", "try",
                                              "case", "float", "params", "typeof",
                                              "catch", "for", "private", "uint",
                                              "char", "foreach", "protected", "ulong",
                                              "checked", "goto", "public", "unchecked",
                                              "class", "if", "readonly", "unsafe",
                                              "const", "implicit", "ref", "ushort",
                                              "continue", "in", "return", "using",
                                              "decimal", "int", "sbyte", "virtual",
                                              "default", "interface", "sealed", "volatile",
                                              "delegate", "internal", "short", "void",
                                              "do", "is", "sizeof", "while",
                                              "double", "lock", "stackalloc",
                                              "else", "long", "static",
                                              "enum", "namespace", "string"};
            return Array.Exists(keywords, FindWordInKeyords);
        }

        private bool FindWordInKeyords(string keyword)
        {
            return this.WordToDisplay.Word == keyword;
        }
    }
}