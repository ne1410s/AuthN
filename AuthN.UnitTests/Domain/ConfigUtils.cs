using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace AuthN.UnitTests.Domain
{
    /// <summary>
    /// Utility methods for working with <see cref="IConfiguration"/>.
    /// </summary>
    public static class ConfigUtils
    {
        /// <summary>
        /// Generates a stub configuration instance from provided values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>Stub configuration.</returns>
        public static IConfiguration Stub(
            this IEnumerable<KeyValuePair<string, string>> values)
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(values)
                .Build();
        }
    }
}
