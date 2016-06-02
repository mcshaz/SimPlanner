using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.DTOs.ProcessBreezeRequests
{
    public static class ISO3166
    {
        /// <summary>
        /// Obtain ISO3166-1 Country based on its alpha3 code.
        /// </summary>
        /// <param name="alpha3"></param>
        /// <returns></returns>
        public static ISO3166Country FromAlpha3(string alpha3)
        {
            return GetCollection().FirstOrDefault(p => p.Alpha3 == alpha3);
        }

        public static ISO3166Country FromAlpha2(string alpha2)
        {
            return GetCollection().FirstOrDefault(p => p.Alpha2 == alpha2);
        }

        #region Build Collection
        private static IEnumerable<ISO3166Country> GetCollection()
        {
            // This collection built from Wikipedia entry on ISO3166-1 on 9th Feb 2016
            yield return new ISO3166Country("Afghanistan", "AF", "AFG", 4);
            yield return new ISO3166Country("Åland Islands", "AX", "ALA", 248);
            yield return new ISO3166Country("Albania", "AL", "ALB", 8);
            yield return new ISO3166Country("Algeria", "DZ", "DZA", 12);
            yield return new ISO3166Country("American Samoa", "AS", "ASM", 16);
            yield return new ISO3166Country("Andorra", "AD", "AND", 20);
            yield return new ISO3166Country("Angola", "AO", "AGO", 24);
            yield return new ISO3166Country("Anguilla", "AI", "AIA", 660);
            yield return new ISO3166Country("Antarctica", "AQ", "ATA", 10);
            yield return new ISO3166Country("Antigua and Barbuda", "AG", "ATG", 28);
            yield return new ISO3166Country("Argentina", "AR", "ARG", 32);
            yield return new ISO3166Country("Armenia", "AM", "ARM", 51);
            yield return new ISO3166Country("Aruba", "AW", "ABW", 533);
            yield return new ISO3166Country("Australia", "AU", "AUS", 36);
            yield return new ISO3166Country("Austria", "AT", "AUT", 40);
            yield return new ISO3166Country("Azerbaijan", "AZ", "AZE", 31);
            yield return new ISO3166Country("Bahamas", "BS", "BHS", 44);
            yield return new ISO3166Country("Bahrain", "BH", "BHR", 48);
            yield return new ISO3166Country("Bangladesh", "BD", "BGD", 50);
            yield return new ISO3166Country("Barbados", "BB", "BRB", 52);
            yield return new ISO3166Country("Belarus", "BY", "BLR", 112);
            yield return new ISO3166Country("Belgium", "BE", "BEL", 56);
            yield return new ISO3166Country("Belize", "BZ", "BLZ", 84);
            yield return new ISO3166Country("Benin", "BJ", "BEN", 204);
            yield return new ISO3166Country("Bermuda", "BM", "BMU", 60);
            yield return new ISO3166Country("Bhutan", "BT", "BTN", 64);
            yield return new ISO3166Country("Bolivia (Plurinational State of)", "BO", "BOL", 68);
            yield return new ISO3166Country("Bonaire, Sint Eustatius and Saba", "BQ", "BES", 535);
            yield return new ISO3166Country("Bosnia and Herzegovina", "BA", "BIH", 70);
            yield return new ISO3166Country("Botswana", "BW", "BWA", 72);
            yield return new ISO3166Country("Bouvet Island", "BV", "BVT", 74);
            yield return new ISO3166Country("Brazil", "BR", "BRA", 76);
            yield return new ISO3166Country("British Indian Ocean Territory", "IO", "IOT", 86);
            yield return new ISO3166Country("Brunei Darussalam", "BN", "BRN", 96);
            yield return new ISO3166Country("Bulgaria", "BG", "BGR", 100);
            yield return new ISO3166Country("Burkina Faso", "BF", "BFA", 854);
            yield return new ISO3166Country("Burundi", "BI", "BDI", 108);
            yield return new ISO3166Country("Cabo Verde", "CV", "CPV", 132);
            yield return new ISO3166Country("Cambodia", "KH", "KHM", 116);
            yield return new ISO3166Country("Cameroon", "CM", "CMR", 120);
            yield return new ISO3166Country("Canada", "CA", "CAN", 124);
            yield return new ISO3166Country("Cayman Islands", "KY", "CYM", 136);
            yield return new ISO3166Country("Central African Republic", "CF", "CAF", 140);
            yield return new ISO3166Country("Chad", "TD", "TCD", 148);
            yield return new ISO3166Country("Chile", "CL", "CHL", 152);
            yield return new ISO3166Country("China", "CN", "CHN", 156);
            yield return new ISO3166Country("Christmas Island", "CX", "CXR", 162);
            yield return new ISO3166Country("Cocos (Keeling) Islands", "CC", "CCK", 166);
            yield return new ISO3166Country("Colombia", "CO", "COL", 170);
            yield return new ISO3166Country("Comoros", "KM", "COM", 174);
            yield return new ISO3166Country("Congo", "CG", "COG", 178);
            yield return new ISO3166Country("Congo (Democratic Republic of the)", "CD", "COD", 180);
            yield return new ISO3166Country("Cook Islands", "CK", "COK", 184);
            yield return new ISO3166Country("Costa Rica", "CR", "CRI", 188);
            yield return new ISO3166Country("Côte d'Ivoire", "CI", "CIV", 384);
            yield return new ISO3166Country("Croatia", "HR", "HRV", 191);
            yield return new ISO3166Country("Cuba", "CU", "CUB", 192);
            yield return new ISO3166Country("Curaçao", "CW", "CUW", 531);
            yield return new ISO3166Country("Cyprus", "CY", "CYP", 196);
            yield return new ISO3166Country("Czech Republic", "CZ", "CZE", 203);
            yield return new ISO3166Country("Denmark", "DK", "DNK", 208);
            yield return new ISO3166Country("Djibouti", "DJ", "DJI", 262);
            yield return new ISO3166Country("Dominica", "DM", "DMA", 212);
            yield return new ISO3166Country("Dominican Republic", "DO", "DOM", 214);
            yield return new ISO3166Country("Ecuador", "EC", "ECU", 218);
            yield return new ISO3166Country("Egypt", "EG", "EGY", 818);
            yield return new ISO3166Country("El Salvador", "SV", "SLV", 222);
            yield return new ISO3166Country("Equatorial Guinea", "GQ", "GNQ", 226);
            yield return new ISO3166Country("Eritrea", "ER", "ERI", 232);
            yield return new ISO3166Country("Estonia", "EE", "EST", 233);
            yield return new ISO3166Country("Ethiopia", "ET", "ETH", 231);
            yield return new ISO3166Country("Falkland Islands (Malvinas)", "FK", "FLK", 238);
            yield return new ISO3166Country("Faroe Islands", "FO", "FRO", 234);
            yield return new ISO3166Country("Fiji", "FJ", "FJI", 242);
            yield return new ISO3166Country("Finland", "FI", "FIN", 246);
            yield return new ISO3166Country("France", "FR", "FRA", 250);
            yield return new ISO3166Country("French Guiana", "GF", "GUF", 254);
            yield return new ISO3166Country("French Polynesia", "PF", "PYF", 258);
            yield return new ISO3166Country("French Southern Territories", "TF", "ATF", 260);
            yield return new ISO3166Country("Gabon", "GA", "GAB", 266);
            yield return new ISO3166Country("Gambia", "GM", "GMB", 270);
            yield return new ISO3166Country("Georgia", "GE", "GEO", 268);
            yield return new ISO3166Country("Germany", "DE", "DEU", 276);
            yield return new ISO3166Country("Ghana", "GH", "GHA", 288);
            yield return new ISO3166Country("Gibraltar", "GI", "GIB", 292);
            yield return new ISO3166Country("Greece", "GR", "GRC", 300);
            yield return new ISO3166Country("Greenland", "GL", "GRL", 304);
            yield return new ISO3166Country("Grenada", "GD", "GRD", 308);
            yield return new ISO3166Country("Guadeloupe", "GP", "GLP", 312);
            yield return new ISO3166Country("Guam", "GU", "GUM", 316);
            yield return new ISO3166Country("Guatemala", "GT", "GTM", 320);
            yield return new ISO3166Country("Guernsey", "GG", "GGY", 831);
            yield return new ISO3166Country("Guinea", "GN", "GIN", 324);
            yield return new ISO3166Country("Guinea-Bissau", "GW", "GNB", 624);
            yield return new ISO3166Country("Guyana", "GY", "GUY", 328);
            yield return new ISO3166Country("Haiti", "HT", "HTI", 332);
            yield return new ISO3166Country("Heard Island and McDonald Islands", "HM", "HMD", 334);
            yield return new ISO3166Country("Holy See", "VA", "VAT", 336);
            yield return new ISO3166Country("Honduras", "HN", "HND", 340);
            yield return new ISO3166Country("Hong Kong", "HK", "HKG", 344);
            yield return new ISO3166Country("Hungary", "HU", "HUN", 348);
            yield return new ISO3166Country("Iceland", "IS", "ISL", 352);
            yield return new ISO3166Country("India", "IN", "IND", 356);
            yield return new ISO3166Country("Indonesia", "ID", "IDN", 360);
            yield return new ISO3166Country("Iran (Islamic Republic of)", "IR", "IRN", 364);
            yield return new ISO3166Country("Iraq", "IQ", "IRQ", 368);
            yield return new ISO3166Country("Ireland", "IE", "IRL", 372);
            yield return new ISO3166Country("Isle of Man", "IM", "IMN", 833);
            yield return new ISO3166Country("Israel", "IL", "ISR", 376);
            yield return new ISO3166Country("Italy", "IT", "ITA", 380);
            yield return new ISO3166Country("Jamaica", "JM", "JAM", 388);
            yield return new ISO3166Country("Japan", "JP", "JPN", 392);
            yield return new ISO3166Country("Jersey", "JE", "JEY", 832);
            yield return new ISO3166Country("Jordan", "JO", "JOR", 400);
            yield return new ISO3166Country("Kazakhstan", "KZ", "KAZ", 398);
            yield return new ISO3166Country("Kenya", "KE", "KEN", 404);
            yield return new ISO3166Country("Kiribati", "KI", "KIR", 296);
            yield return new ISO3166Country("Korea (Democratic People's Republic of)", "KP", "PRK", 408);
            yield return new ISO3166Country("Korea (Republic of)", "KR", "KOR", 410);
            yield return new ISO3166Country("Kuwait", "KW", "KWT", 414);
            yield return new ISO3166Country("Kyrgyzstan", "KG", "KGZ", 417);
            yield return new ISO3166Country("Lao People's Democratic Republic", "LA", "LAO", 418);
            yield return new ISO3166Country("Latvia", "LV", "LVA", 428);
            yield return new ISO3166Country("Lebanon", "LB", "LBN", 422);
            yield return new ISO3166Country("Lesotho", "LS", "LSO", 426);
            yield return new ISO3166Country("Liberia", "LR", "LBR", 430);
            yield return new ISO3166Country("Libya", "LY", "LBY", 434);
            yield return new ISO3166Country("Liechtenstein", "LI", "LIE", 438);
            yield return new ISO3166Country("Lithuania", "LT", "LTU", 440);
            yield return new ISO3166Country("Luxembourg", "LU", "LUX", 442);
            yield return new ISO3166Country("Macao", "MO", "MAC", 446);
            yield return new ISO3166Country("Macedonia (the former Yugoslav Republic of)", "MK", "MKD", 807);
            yield return new ISO3166Country("Madagascar", "MG", "MDG", 450);
            yield return new ISO3166Country("Malawi", "MW", "MWI", 454);
            yield return new ISO3166Country("Malaysia", "MY", "MYS", 458);
            yield return new ISO3166Country("Maldives", "MV", "MDV", 462);
            yield return new ISO3166Country("Mali", "ML", "MLI", 466);
            yield return new ISO3166Country("Malta", "MT", "MLT", 470);
            yield return new ISO3166Country("Marshall Islands", "MH", "MHL", 584);
            yield return new ISO3166Country("Martinique", "MQ", "MTQ", 474);
            yield return new ISO3166Country("Mauritania", "MR", "MRT", 478);
            yield return new ISO3166Country("Mauritius", "MU", "MUS", 480);
            yield return new ISO3166Country("Mayotte", "YT", "MYT", 175);
            yield return new ISO3166Country("Mexico", "MX", "MEX", 484);
            yield return new ISO3166Country("Micronesia (Federated States of)", "FM", "FSM", 583);
            yield return new ISO3166Country("Moldova (Republic of)", "MD", "MDA", 498);
            yield return new ISO3166Country("Monaco", "MC", "MCO", 492);
            yield return new ISO3166Country("Mongolia", "MN", "MNG", 496);
            yield return new ISO3166Country("Montenegro", "ME", "MNE", 499);
            yield return new ISO3166Country("Montserrat", "MS", "MSR", 500);
            yield return new ISO3166Country("Morocco", "MA", "MAR", 504);
            yield return new ISO3166Country("Mozambique", "MZ", "MOZ", 508);
            yield return new ISO3166Country("Myanmar", "MM", "MMR", 104);
            yield return new ISO3166Country("Namibia", "NA", "NAM", 516);
            yield return new ISO3166Country("Nauru", "NR", "NRU", 520);
            yield return new ISO3166Country("Nepal", "NP", "NPL", 524);
            yield return new ISO3166Country("Netherlands", "NL", "NLD", 528);
            yield return new ISO3166Country("New Caledonia", "NC", "NCL", 540);
            yield return new ISO3166Country("New Zealand", "NZ", "NZL", 554);
            yield return new ISO3166Country("Nicaragua", "NI", "NIC", 558);
            yield return new ISO3166Country("Niger", "NE", "NER", 562);
            yield return new ISO3166Country("Nigeria", "NG", "NGA", 566);
            yield return new ISO3166Country("Niue", "NU", "NIU", 570);
            yield return new ISO3166Country("Norfolk Island", "NF", "NFK", 574);
            yield return new ISO3166Country("Northern Mariana Islands", "MP", "MNP", 580);
            yield return new ISO3166Country("Norway", "NO", "NOR", 578);
            yield return new ISO3166Country("Oman", "OM", "OMN", 512);
            yield return new ISO3166Country("Pakistan", "PK", "PAK", 586);
            yield return new ISO3166Country("Palau", "PW", "PLW", 585);
            yield return new ISO3166Country("Palestine, State of", "PS", "PSE", 275);
            yield return new ISO3166Country("Panama", "PA", "PAN", 591);
            yield return new ISO3166Country("Papua New Guinea", "PG", "PNG", 598);
            yield return new ISO3166Country("Paraguay", "PY", "PRY", 600);
            yield return new ISO3166Country("Peru", "PE", "PER", 604);
            yield return new ISO3166Country("Philippines", "PH", "PHL", 608);
            yield return new ISO3166Country("Pitcairn", "PN", "PCN", 612);
            yield return new ISO3166Country("Poland", "PL", "POL", 616);
            yield return new ISO3166Country("Portugal", "PT", "PRT", 620);
            yield return new ISO3166Country("Puerto Rico", "PR", "PRI", 630);
            yield return new ISO3166Country("Qatar", "QA", "QAT", 634);
            yield return new ISO3166Country("Réunion", "RE", "REU", 638);
            yield return new ISO3166Country("Romania", "RO", "ROU", 642);
            yield return new ISO3166Country("Russian Federation", "RU", "RUS", 643);
            yield return new ISO3166Country("Rwanda", "RW", "RWA", 646);
            yield return new ISO3166Country("Saint Barthélemy", "BL", "BLM", 652);
            yield return new ISO3166Country("Saint Helena, Ascension and Tristan da Cunha", "SH", "SHN", 654);
            yield return new ISO3166Country("Saint Kitts and Nevis", "KN", "KNA", 659);
            yield return new ISO3166Country("Saint Lucia", "LC", "LCA", 662);
            yield return new ISO3166Country("Saint Martin (French part)", "MF", "MAF", 663);
            yield return new ISO3166Country("Saint Pierre and Miquelon", "PM", "SPM", 666);
            yield return new ISO3166Country("Saint Vincent and the Grenadines", "VC", "VCT", 670);
            yield return new ISO3166Country("Samoa", "WS", "WSM", 882);
            yield return new ISO3166Country("San Marino", "SP", "SMR", 674);
            yield return new ISO3166Country("Sao Tome and Principe", "ST", "STP", 678);
            yield return new ISO3166Country("Saudi Arabia", "SA", "SAU", 682);
            yield return new ISO3166Country("Senegal", "SN", "SEN", 686);
            yield return new ISO3166Country("Serbia", "RS", "SRB", 688);
            yield return new ISO3166Country("Seychelles", "SC", "SYC", 690);
            yield return new ISO3166Country("Sierra Leone", "SL", "SLE", 694);
            yield return new ISO3166Country("Singapore", "SG", "SGP", 702);
            yield return new ISO3166Country("Sint Maarten (Dutch part)", "SX", "SXM", 534);
            yield return new ISO3166Country("Slovakia", "SK", "SVK", 703);
            yield return new ISO3166Country("Slovenia", "SI", "SVN", 705);
            yield return new ISO3166Country("Solomon Islands", "SB", "SLB", 90);
            yield return new ISO3166Country("Somalia", "SO", "SOM", 706);
            yield return new ISO3166Country("South Africa", "ZA", "ZAF", 710);
            yield return new ISO3166Country("South Georgia and the South Sandwich Islands", "GS", "SGS", 239);
            yield return new ISO3166Country("South Sudan", "SS", "SSD", 728);
            yield return new ISO3166Country("Spain", "ES", "ESP", 724);
            yield return new ISO3166Country("Sri Lanka", "LK", "LKA", 144);
            yield return new ISO3166Country("Sudan", "SD", "SDN", 729);
            yield return new ISO3166Country("Suriname", "SR", "SUR", 740);
            yield return new ISO3166Country("Svalbard and Jan Mayen", "SJ", "SJM", 744);
            yield return new ISO3166Country("Swaziland", "SZ", "SWZ", 748);
            yield return new ISO3166Country("Sweden", "SE", "SWE", 752);
            yield return new ISO3166Country("Switzerland", "CH", "CHE", 756);
            yield return new ISO3166Country("Syrian Arab Republic", "SY", "SYR", 760);
            yield return new ISO3166Country("Taiwan, Province of China[a]", "TW", "TWN", 158);
            yield return new ISO3166Country("Tajikistan", "TJ", "TJK", 762);
            yield return new ISO3166Country("Tanzania, United Republic of", "TZ", "TZA", 834);
            yield return new ISO3166Country("Thailand", "TH", "THA", 764);
            yield return new ISO3166Country("Timor-Leste", "TL", "TLS", 626);
            yield return new ISO3166Country("Togo", "TG", "TGO", 768);
            yield return new ISO3166Country("Tokelau", "TK", "TKL", 772);
            yield return new ISO3166Country("Tonga", "TO", "TON", 776);
            yield return new ISO3166Country("Trinidad and Tobago", "TT", "TTO", 780);
            yield return new ISO3166Country("Tunisia", "TN", "TUN", 788);
            yield return new ISO3166Country("Turkey", "TR", "TUR", 792);
            yield return new ISO3166Country("Turkmenistan", "TM", "TKM", 795);
            yield return new ISO3166Country("Turks and Caicos Islands", "TC", "TCA", 796);
            yield return new ISO3166Country("Tuvalu", "TV", "TUV", 798);
            yield return new ISO3166Country("Uganda", "UG", "UGA", 800);
            yield return new ISO3166Country("Ukraine", "UA", "UKR", 804);
            yield return new ISO3166Country("United Arab Emirates", "AE", "ARE", 784);
            yield return new ISO3166Country("United Kingdom of Great Britain and Northern Ireland", "GB", "GBR", 826);
            yield return new ISO3166Country("United States of America", "US", "USA", 840);
            yield return new ISO3166Country("United States Minor Outlying Islands", "UM", "UMI", 581);
            yield return new ISO3166Country("Uruguay", "UY", "URY", 858);
            yield return new ISO3166Country("Uzbekistan", "UZ", "UZB", 860);
            yield return new ISO3166Country("Vanuatu", "VU", "VUT", 548);
            yield return new ISO3166Country("Venezuela (Bolivarian Republic of)", "VE", "VEN", 862);
            yield return new ISO3166Country("Viet Nam", "VN", "VNM", 704);
            yield return new ISO3166Country("Virgin Islands (British)", "VG", "VGB", 92);
            yield return new ISO3166Country("Virgin Islands (U.S.)", "VI", "VIR", 850);
            yield return new ISO3166Country("Wallis and Futuna", "WF", "WLF", 876);
            yield return new ISO3166Country("Western Sahara", "EH", "ESH", 732);
            yield return new ISO3166Country("Yemen", "YE", "YEM", 887);
            yield return new ISO3166Country("Zambia", "ZM", "ZMB", 894);
            yield return new ISO3166Country("Zimbabwe", "ZW", "ZWE", 716);
        }
        #endregion
    }

    /// <summary>
    /// Representation of an ISO3166-1 Country
    /// </summary>
    public class ISO3166Country
    {
        public ISO3166Country(string name, string alpha2, string alpha3, int numericCode)
        {
            Name = name;
            Alpha2 = alpha2;
            Alpha3 = alpha3;
            NumericCode = numericCode;
        }

        public string Name { get; private set; }

        public string Alpha2 { get; private set; }

        public string Alpha3 { get; private set; }

        public int NumericCode { get; private set; }
    }
}
