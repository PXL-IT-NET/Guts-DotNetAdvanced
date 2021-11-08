using System.Collections.Generic;
using HeroApp.Domain.Contracts;

namespace HeroApp.AppLogic.Contracts
{
    public interface IHeroRepository
    {
        IReadOnlyList<IHero> GetAll();
    }
}
