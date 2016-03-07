using SM.DataAccess;
using SM.Dto;
using System;
using System.Linq.Expressions;
namespace SM.DTOs.Maps
{
    internal static class CountryLocaleCodeMaps
    {
        internal static Func<CountryLocaleCodeDto, CountryLocaleCode> mapToRepo()
        {
            return m => new CountryLocaleCode
            {
                LocaleCode = m.LocaleCode,
                CountryCode = m.CountryCode,
                Preference = m.Preference

                //Country Country { get; set; }
            };
        }

        internal static Expression<Func<CountryLocaleCode, CountryLocaleCodeDto>> mapFromRepo()
        {
            return m => new CountryLocaleCodeDto
            {
                LocaleCode = m.LocaleCode,
                CountryCode = m.CountryCode,
                Preference = m.Preference

                //Country Country { get; set; }
            };
        }
    }
}
