
namespace RunEscape
{
    public class LevelWordPair
    {
        public string Level;
        public string Word;
        public bool Comment;

        public LevelWordPair(string level, string word, bool comment)
        {
            this.Level = level;
            this.Word = word;
            this.Comment = comment;
        }
    }
}
