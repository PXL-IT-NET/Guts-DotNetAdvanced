using System.Windows.Threading;
using NUnit.Framework;

namespace Exercise5.Tests
{
    [SetUpFixture]
    public class Setup
    {
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }
    }
}