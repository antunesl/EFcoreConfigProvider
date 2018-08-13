﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace EFCoreConfigProvider
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddEFCoreConfig<T>(this IConfigurationBuilder builder, Action<DbContextOptionsBuilder> setup, Func<T, IDictionary<string, string>> loadValuesFromDb) where T : DbContext
        {
            if (setup == null)
                throw new ArgumentNullException(nameof(setup));

            if (loadValuesFromDb == null)
                throw new ArgumentNullException(nameof(loadValuesFromDb));

            return builder.Add(new EFCoreConfigSource<T>(setup, loadValuesFromDb));
        }

        public static IConfigurationBuilder AddEFCoreConfig<T>(this IConfigurationBuilder builder, Action<EFCoreConfigProviderOptions<T>> options) where T : DbContext
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            var configProviderOptions = new EFCoreConfigProviderOptions<T>();
            options(configProviderOptions);

            if (configProviderOptions.DbContextSetup == null)
                throw new ArgumentNullException("DbContextSetup");

            if (configProviderOptions.LoadValuesFromDbAction == null)
                throw new ArgumentNullException("LoadValuesFromDbAction");

            return builder.Add(new EFCoreConfigSource<T>(configProviderOptions.DbContextSetup, configProviderOptions.LoadValuesFromDbAction));
        }
    }

    public class EFCoreConfigProviderOptions<T>
    {
        public Action<DbContextOptionsBuilder> DbContextSetup { get; set; }

        public Func<T, IDictionary<string, string>> LoadValuesFromDbAction { get; set; }
    }
}
