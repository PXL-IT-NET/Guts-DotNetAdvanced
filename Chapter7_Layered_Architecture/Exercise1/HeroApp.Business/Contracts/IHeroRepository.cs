using System.Collections.Generic;
using HeroApp.Domain.Contracts;

namespace HeroApp.Business.Contracts
{
    public interface IHeroRepository
    {
        IReadOnlyList<IHero> GetAll();
    }
}
