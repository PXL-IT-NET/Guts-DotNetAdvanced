using System.Threading;

namespace Exercise3.ChefAggregate
{
    public class DummyChefActions : IChefActions
    {
        private readonly int _secondsNeededToBakeABurger;
        private readonly int _secondsNeededToTakeABreather;

        public DummyChefActions(int secondsNeededToBakeABurger, int secondsNeededToTakeABreather)
        {
            _secondsNeededToBakeABurger = secondsNeededToBakeABurger;
            _secondsNeededToTakeABreather = secondsNeededToTakeABreather;
        }

        public void CookBurger()
        {
            Thread.Sleep(_secondsNeededToBakeABurger * 1000);
        }

        public void TakeABreather()
        {
            Thread.Sleep(_secondsNeededToTakeABreather * 1000);
        }
    }
}