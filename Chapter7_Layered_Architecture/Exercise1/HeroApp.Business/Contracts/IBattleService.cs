using HeroApp.Domain.Contracts;

namespace HeroApp.Business.Contracts
{
    public interface IBattleService
    {
        IBattle SetupRandomBattle();
    }
}