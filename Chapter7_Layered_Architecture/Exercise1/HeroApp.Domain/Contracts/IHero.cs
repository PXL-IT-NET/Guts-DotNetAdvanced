namespace HeroApp.Domain.Contracts
{
    public interface IHero
    {
        string Name { get; }
        int Strength { get; }
        float SuperModeLikeliness { get; }
        int Health { get; }
        void Attack(IHero opponent);
        void DefendAgainstAttack(int attackStrength);
    }
}