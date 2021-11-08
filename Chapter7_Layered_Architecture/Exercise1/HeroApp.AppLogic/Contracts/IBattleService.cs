using HeroApp.Domain.Contracts;

namespace HeroApp.AppLogic.Contracts
{
    public interface IBattleService
    {
        IBattle SetupRandomBattle();
    }
}