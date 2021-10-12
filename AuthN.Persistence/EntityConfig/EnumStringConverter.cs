using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AuthN.Persistence.EntityConfig
{
    /// <summary>
    /// Generic enum/string conversion.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    public class EnumStringConverter<TEnum> : ValueConverter<TEnum, string>
        where TEnum : struct, Enum
    {
        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="EnumStringConverter{TEnum}"/> class.
        /// </summary>
        public EnumStringConverter()
            : base(
                  enu => enu.ToString(),
                  str => Enum.Parse<TEnum>(str))
        { }
    }
}
