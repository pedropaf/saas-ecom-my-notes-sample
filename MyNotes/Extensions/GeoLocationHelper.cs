namespace MyNotes.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using MaxMind.GeoIP2;
    using MaxMind.GeoIP2.Exceptions;

    public static class GeoLocationHelper
    {
        // ReSharper disable once InconsistentNaming
        /// <summary>
        /// Gets the country ISO code from IP.
        /// </summary>
        /// <param name="ipAddress">The ip address.</param>
        /// <returns></returns>
        public static string GetCountryFromIP(string ipAddress)
        {
            string country;
            try
            {
                using (
                    var reader =
                        new DatabaseReader(HttpContext.Current.Server.MapPath("~/App_Data/GeoLite2-Country.mmdb")))
                {
                    var response = reader.Country(ipAddress);
                    country = response.Country.IsoCode;
                }
            }
            catch (AddressNotFoundException ex)
            {
                country = null;
            }
            catch (Exception ex)
            {
                country = null;
            }

            return country;
        }

        /// <summary>
        /// Selects the list countries.
        /// </summary>
        /// <param name="country">The country.</param>
        /// <returns></returns>
        public static List<SelectListItem> SelectListCountries(string country)
        {
            var getCultureInfo = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            var countries =
                getCultureInfo.Select(cultureInfo => new RegionInfo(cultureInfo.LCID))
                    .Select(getRegionInfo => new SelectListItem
                    {
                        Text = getRegionInfo.EnglishName,
                        Value = getRegionInfo.TwoLetterISORegionName,
                        Selected = country == getRegionInfo.TwoLetterISORegionName
                    }).OrderBy(c => c.Text).DistinctBy(i => i.Text).ToList();
            return countries;
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            return source.Where(element => seenKeys.Add(keySelector(element)));
        }
    }
}