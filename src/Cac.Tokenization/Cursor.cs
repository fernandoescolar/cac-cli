namespace Cac.Tokenization
{
    public class Cursor
    {
        public int Line { get; set; }
        public int Column { get; set; }

        public void Reset()
        {
            Line = 0;
            Column = 0;
        }
    }
}
