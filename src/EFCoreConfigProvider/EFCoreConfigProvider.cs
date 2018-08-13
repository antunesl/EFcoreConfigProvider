using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

namespace EFCoreConfigProvider
{
    public class EFCoreConfigProvider<T> : ConfigurationProvider where T : DbContext
    {
        private readonly string _reloadConfigDirectoryPath;
        private readonly Func<T, IDictionary<string, string>> _loadValuesFromDb;
        private readonly JsonConfigurationSource _jsonConfigurationSource;

        Action<DbContextOptionsBuilder> OptionsAction { get; }


        public EFCoreConfigProvider(Action<DbContextOptionsBuilder> optionsAction, Func<T, IDictionary<string, string>> loadValuesFromDbAction, IConfigurationBuilder builder)
        {
            OptionsAction = optionsAction;
            _loadValuesFromDb = loadValuesFromDbAction;

            var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
            _reloadConfigDirectoryPath = new FileInfo(location.AbsolutePath).Directory.FullName;

            var reloadConfigFileName = "reloadConfig.json";
            var reloadConfigFilePath = Path.Combine(_reloadConfigDirectoryPath, reloadConfigFileName);
            if (!File.Exists(reloadConfigFilePath))
            {
                File.WriteAllText(reloadConfigFilePath, "{}");
            }

            _jsonConfigurationSource = new JsonConfigurationSource
            {
                FileProvider = builder.GetFileProvider(),
                Path = reloadConfigFileName,
                ReloadOnChange = true
            };
            _jsonConfigurationSource.Build(builder);

            if (_jsonConfigurationSource.ReloadOnChange && _jsonConfigurationSource.FileProvider != null)
            {
                ChangeToken.OnChange(
                () => _jsonConfigurationSource.FileProvider.Watch(_jsonConfigurationSource.Path),
                () =>
                {
                    Thread.Sleep(_jsonConfigurationSource.ReloadDelay);
                    Load();
                });
            }
        }

        public override void Load()
        {
            var builder = new DbContextOptionsBuilder<T>();
            OptionsAction(builder);

            if (Activator.CreateInstance(typeof(T), builder.Options) is T dbContextInstance)
                Data = _loadValuesFromDb(dbContextInstance);
        }
    }
}
