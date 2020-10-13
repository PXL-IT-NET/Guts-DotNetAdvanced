namespace HeroApp.Domain.Contracts
{
    public interface IBattleFactory
    {
        IBattle CreateNewBattle(IHero fighter1, IHero fighter2);
    }
}