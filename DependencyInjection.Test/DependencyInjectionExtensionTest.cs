using DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DependencyInjection.Test
{
    public class DependencyInjectionExtensionTest
    {
        private readonly ServiceCollection _subject;
        private readonly IServiceProvider _serviceProvider;
        private readonly List<string> _ignoredInterfaces;

        public DependencyInjectionExtensionTest()
        {
            _subject = new ServiceCollection();

            var inMemorySettings = new Dictionary<string, string>
            {
                { "ConfigB", "dummy" }
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _subject.RegisterDI(configuration);
            _serviceProvider = _subject.BuildServiceProvider();

            _ignoredInterfaces = new List<string>();
        }

        [Fact]
        public void DependencyInject_Valid()
        {
            bool flagContinue = false;

            do
            {
                flagContinue = false;

                try
                {
                    var solutionName = GetSolutionName();

                    var types = AppDomain.CurrentDomain.GetAssemblies()
                                    .SelectMany(s => s.GetTypes())
                                    .Where(s => s.FullName.Contains(solutionName) &&
                                    !s.FullName.ToLower().Contains("test") &&
                                    !_ignoredInterfaces.Contains(s.Name) &&
                                    s.IsInterface).ToList();

                    foreach (var type in types)
                    {
                        if (_serviceProvider.GetService(type) == null)
                            throw new Exception($"Não foi possível injetar a dependência {type.FullName.Substring(type.FullName.LastIndexOf('.') + 1)}");
                    }
                }
                catch (Exception ex)
                {
                    if (ex is ReflectionTypeLoadException)
                    {
                        flagContinue = true;
                        continue;
                    }

                    throw;
                }
            }
            while (flagContinue);
        }

        private string GetSolutionName()
        {
            var currentDirPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            while (currentDirPath != null)
            {
                var fileInCurrentDir = Directory.GetFiles(currentDirPath).Select(f => f.Split(Path.DirectorySeparatorChar).Last()).ToArray();
                var solutionFileName = fileInCurrentDir.SingleOrDefault(f => f.EndsWith(".sln", StringComparison.InvariantCultureIgnoreCase));
                if (solutionFileName != null)
                    return solutionFileName.Replace(".sln", string.Empty);

                currentDirPath = Directory.GetParent(currentDirPath)?.FullName;
            }

            throw new FileNotFoundException("Cannot find solution file path");
        }
    }
}