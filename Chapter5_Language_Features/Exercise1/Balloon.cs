using System.Drawing;

namespace Exercise1
{
    public class Balloon
    {
        public Color Color { get; }
        public int Size { get; }

        public Balloon(Color color, int size)
        {
            Color = color;
            Size = size;
        }
    }
}