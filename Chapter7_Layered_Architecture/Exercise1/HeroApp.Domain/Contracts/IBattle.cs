namespace HeroApp.Domain.Contracts
{
    public interface IBattle
    {
        IHero Fighter1 { get; }
        IHero Fighter2 { get; }
        bool IsOver { get; }
        void FightRound();
    }
}