using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace EFCoreConfigProvider
{
    public class EFCoreConfigSource<T> : IConfigurationSource where T : DbContext
    {
        private readonly Action<DbContextOptionsBuilder> _optionsAction;
        private readonly Func<T, IDictionary<string, string>> _loadValuesFromDbAction;

        public EFCoreConfigSource(Action<DbContextOptionsBuilder> optionsAction,
            Func<T, IDictionary<string, string>> loadValuesFromDbAction)
        {
            _optionsAction = optionsAction;
            _loadValuesFromDbAction = loadValuesFromDbAction;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new EFCoreConfigProvider<T>(_optionsAction, _loadValuesFromDbAction, builder);
        }
    }
}
