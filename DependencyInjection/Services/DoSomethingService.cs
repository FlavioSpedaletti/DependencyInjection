using System.Diagnostics;

namespace DependencyInjection.Services
{
    public class DoSomethingService : IDoSomethingService
    {
        private readonly string _configA;

        public DoSomethingService(string configA)
        {
            _configA = configA;
        }

        public async Task Foo()
        {
            await Task.Delay(1000);

            Debug.WriteLine(_configA);
        }
    }
}
