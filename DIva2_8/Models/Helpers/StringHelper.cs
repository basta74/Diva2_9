using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diva2Web.Models.Helpers
{
    public static class StringHelper
    {
        public static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// z telefonu necha jen cislice a odebere predvolbu (+420, 00420, 420)
        /// </summary>
        /// <returns>9mistne cislo, nebo 0 kdyz cislo neni platne</returns>
        public static int GetPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                return 0;
            }

            var digits = new string(phone.Where(c => c >= '0' && c <= '9').ToArray());

            if (digits.StartsWith("00"))
            {
                digits = digits.Substring(2);
            }

            if (digits.Length == 12 && digits.StartsWith("420"))
            {
                digits = digits.Substring(3);
            }

            if (digits.Length != 9)
            {
                return 0;
            }

            int.TryParse(digits, out int ph);
            return ph;
        }
    }
}
