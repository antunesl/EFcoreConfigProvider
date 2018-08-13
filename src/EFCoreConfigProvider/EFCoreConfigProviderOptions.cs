using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace EFCoreConfigProvider
{
    public class EFCoreConfigProviderOptions<T>
    {
        public Action<DbContextOptionsBuilder> DbContextSetup { get; set; }

        public Func<T, IDictionary<string, string>> LoadValuesFromDbAction { get; set; }
    }
}
