using HeroApp.Domain.Contracts;
using Moq;

namespace HeroApp.Tests
{
    internal class HeroMockBuilder
    {
        private readonly Mock<IHero> _heroMock;

        public HeroMockBuilder()
        {
            _heroMock = new Mock<IHero>();
        }

        public HeroMockBuilder WithHealth(int health)
        {
            _heroMock.SetupGet(hero => hero.Health).Returns(health);
            return this;
        }

        public Mock<IHero> Build()
        {
            return _heroMock;
        }

        public IHero BuildObject()
        {
            return _heroMock.Object;
        }
    }
}