using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Exercise3
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        private IList<Game> GetDummyGames()
        {
            var games = new List<Game>
            {
                new Game
                {
                    Name = "GTA V",
                    Description =
                        "Dit is een spel waarbij een speler allerlei handelingen kan doen zoals rennen, zwemmen, autorijden om het spel te navigeren.Hoe kan je de game uitspelen ? Door alle missies te halen en niet gepakt te worden door politie.",
                },
                new Game
                {
                    Name = "Call of Duty:Infinite Warfare",
                    Description =
                        "Het is een game waarbij je missies uitvoert en heeft een zombiemodus. Je speelt dit spel op veel verschillende plekken en schiet vaak vanuit de cockpit in plaats van tijdens het lopen.",
                }
            };
            return games;
        }
    }
}
