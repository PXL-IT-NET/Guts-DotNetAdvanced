using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Exercise2
{
    public class Game
    {
        public int GameId { get; set; }
        public string Name { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public double Rating { get; set; }
        public bool IsUnder18 { get; set; }
    }
}
