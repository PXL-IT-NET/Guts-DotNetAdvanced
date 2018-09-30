namespace Lottery.Domain
{
    public class DrawNumber
    {
        public Draw Draw { get; set; }
        public int DrawId { get; set; }
        public int Number { get; set; }
        public int? Position { get; set; }
    }
}