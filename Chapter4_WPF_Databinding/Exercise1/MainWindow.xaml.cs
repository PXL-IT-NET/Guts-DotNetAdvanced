using System;
using System.Windows;

namespace Exercise1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Game game = new Game
            {
                GameId = 1,
                Name = "GTA V",
                ReleaseDate = DateTime.Parse("01/10/2013", System.Globalization.CultureInfo.InvariantCulture),
                Type = "Action/Adventure",
                Description =
                    "Dit is een spel waarbij een speler allerlei handelingen kan doen zoals rennen, zwemmen, autorijden om het spel te navigeren. " +
                    "Hoe kan je de game uitspelen? " +
                    "Door alle missies te halen en niet gepakt te worden door politie."
            };
        }
    }
}
