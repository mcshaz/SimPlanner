using System.Linq;
using System.Web.Http;
using System.Collections.Generic;
using System.Globalization;
using NodaTime.TimeZones;
using SP.Web.Controllers.Helpers;
using System.Threading.Tasks;

namespace SP.Web.Controllers
{
    [Authorize]
    [RoutePrefix("api/utilities")]
    public class UtilitiesController : ApiController
    {
        // Todo: inject via an interface rather than "new" the concrete class

        [HttpGet]
        public IEnumerable<KeyValuePair<string, string>> CultureFormats() {
            return (from c in CultureInfo.GetCultures(CultureTypes.AllCultures)
                    let indx = c.Name.LastIndexOf('-')
                    where  indx != -1 && indx == c.Name.Length-3
                    select new KeyValuePair<string, string>(c.Name, c.DisplayName));
        }

        [HttpGet]
        public IEnumerable<string> TimeZones(string id)
        {
            int dashPos = id.IndexOf('-');
            if (dashPos != -1)
            {
                id = id.Substring(dashPos + 1);
            }
            HashSet<string> returnVar = new HashSet<string>();
            foreach (var t in TzdbDateTimeZoneSource.Default.ZoneLocations)
            {
                if (t.CountryCode == id)
                {
                    var mapZone = TzdbDateTimeZoneSource.Default.WindowsMapping.MapZones.FirstOrDefault(x => x.TzdbIds.Contains(t.ZoneId));
                    if (mapZone != null)
                    {
                        returnVar.Add(mapZone.WindowsId);
                    }
                }
            }
            return returnVar;
        }

        [HttpGet]
        public string CurrencyInfo(string id)
        {
            /*
            var ci = CultureInfo.GetCultureInfo(cultureCode);
            var returnVar = new Dictionary<string, object>();
            returnVar.Add("currencyDecimalDigits", ci.NumberFormat.CurrencyDecimalDigits);
            returnVar.Add("currencyDecimalSeparator", ci.NumberFormat.CurrencyDecimalSeparator);
            returnVar.Add("currencyGroupSeparator", ci.NumberFormat.CurrencyGroupSeparator);
            returnVar.Add("currencyGroupSizes", ci.NumberFormat.CurrencyGroupSizes);
            returnVar.Add("currencySymbol", ci.NumberFormat.CurrencySymbol);
            */
            var ri = new RegionInfo(id);

            return ri.ISOCurrencySymbol;
        }

        [HttpGet, AllowAnonymous]
        public async Task<IEnumerable<SmtpAttempt>> TestEmail()
        {
            return await EmailHelpers.SendTestEmails(Request);
        }
    }
}