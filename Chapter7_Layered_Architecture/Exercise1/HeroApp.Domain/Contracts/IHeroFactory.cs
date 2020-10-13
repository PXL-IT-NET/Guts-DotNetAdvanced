namespace HeroApp.Domain.Contracts
{
    public interface IHeroFactory
    {
        IHero CreateNewHero(string name, int strength, float superModeLikeliness);
    }
}