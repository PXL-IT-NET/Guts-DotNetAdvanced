using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Exercise2
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //TODO: use the 'CreateGames' method to get a list of 7 games
        }

        private List<Game> CreateGames()
        {
            var games = new List<Game>
            {
                new Game
                {
                    GameId = 1,
                    Name = "GTA V",
                    ReleaseDate = DateTime.Parse("10/01/2013", System.Globalization.CultureInfo.InvariantCulture),
                    Type = "Adventure",
                    Description =
                        "Dit is een spel waarbij een speler allerlei handelingen kan doen zoals rennen, zwemmen, autorijden om het spel te navigeren.Hoe kan je de game uitspelen ? Door alle missies te halen en niet gepakt te worden door politie.",
                    Rating = 7.5,
                    IsUnder18 = true
                },
                new Game
                {
                    GameId = 2,
                    Name = "Call of Duty:Infinite Warfare",
                    ReleaseDate = DateTime.Parse("12/01/2016", System.Globalization.CultureInfo.InvariantCulture),
                    Type = "First-Person Shooter",
                    Description =
                        "Het is een game waarbij je missies uitvoert en heeft een zombiemodus. Je speelt dit spel op veel verschillende plekken en schiet vaak vanuit de cockpit in plaats van tijdens het lopen.",
                    Rating = 8.7,
                    IsUnder18 = false
                },
                new Game
                {
                    GameId = 3,
                    Name = "Battlefield",
                    ReleaseDate = DateTime.Parse("12/01/2015", System.Globalization.CultureInfo.InvariantCulture),
                    Type = "First-Person Shooter",
                    Description =
                        "Spel dat zich afspeelt in de eerste wereldoorlog, waarin er verschillende missies zoals tankmissies en het vliegen van oude vliegtuigen bestaan.",
                    Rating = 6.9,
                    IsUnder18 = true
                },
                new Game
                {
                    GameId = 4,
                    Name = "Resident Evil 7",
                    ReleaseDate = DateTime.Parse("12/01/2015", System.Globalization.CultureInfo.InvariantCulture),
                    Type = "Survival Horror",
                    Description =
                        "Binnen dit spel ben je een doodnormale jongen genaamd Ethan. Zijn vriendin is drie jaar terug verdwenen, maar laat plotseling weer iets van zich horen. Tijdens dit spel ontrafel je raadsels en ga je gevechten aan. ",
                    Rating = 6.3,
                    IsUnder18 = false
                },
                new Game
                {
                    GameId = 5,
                    Name = "FIFA Ultimate Team",
                    ReleaseDate = DateTime.Parse("09/01/2017", System.Globalization.CultureInfo.InvariantCulture),
                    Type = "Sport",
                    Description =
                        "Binnen dit spel kun je jouw eigen FIFA club creëren door je eigen team samen te stellen met spelers die je met FIFA coins koopt. ",
                    Rating = 8.2,
                    IsUnder18 = true
                },
                new Game
                {
                    GameId = 6,
                    Name = "Fortnite: Save the world",
                    ReleaseDate = DateTime.Parse("09/25/2017", System.Globalization.CultureInfo.InvariantCulture),
                    Type = "Third-Person Shooter",
                    Description =
                        "Men dient monsters dood te schieten. Het spel speelt zich af in de hedendaagse wereld, waar het plotselinge verschijnen van een wereldwijde storm ertoe leidt dat 98% van de wereldbevolking verdwijnt, en zombie-achtige wezens opstaan ​​om de rest aan te vallen.",
                    Rating = 9.7,
                    IsUnder18 = true
                },
                new Game
                {
                    GameId = 7,
                    Name = "Minecraft",
                    ReleaseDate = DateTime.Parse("07/01/2011", System.Globalization.CultureInfo.InvariantCulture),
                    Type = "Adventure",
                    Description =
                        "Bij dit spel kan men in een uitgestrekt gebied kubusvormige objecten plaatsen op een rooster. Het spel bouwt automatisch een omgeving, met grondstoffen, tegenstanders, dieren, tunnelsystemen, bergformaties en meren. De speler heeft een eigen avatar, waarmee hij de omgeving actief kan aanpassen.",
                    Rating = 9.4,
                    IsUnder18 = true
                }
            };
            return games;
        }
    }
}
