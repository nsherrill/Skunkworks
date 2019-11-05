using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwilioConsole.Shared.Enums;

namespace TwilioConsole.Engines
{
    public class TranslationEngine
    {
        private Languages DEFAULT_LANGUAGE { get { return Languages.English; } }
        private Dictionary<Languages, string> SUPPORTED_COUNTRY_CODES
        {
            get
            {
                var result = new Dictionary<Languages, string>();
                result.Add(Languages.English, "en-US");
                result.Add(Languages.French, "fr-CA");
                result.Add(Languages.German, "de");
                result.Add(Languages.Spanish, "es-AR");
                return result;
            }
        }

        char spaceCharacter = ' ';
        public void SetLanguage(Languages countryCode)
        {
            if (SUPPORTED_COUNTRY_CODES.Keys.Contains(countryCode))
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(SUPPORTED_COUNTRY_CODES[countryCode]);
                if (countryCode == Languages.English)
                    spaceCharacter = ' ';
                else
                    spaceCharacter = (char)0x00002008;
            }
            else
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(SUPPORTED_COUNTRY_CODES[DEFAULT_LANGUAGE]);
        }

        public string GetText(MessageType messageType, params string[] formatters)
        {
            string result = string.Empty;

            switch (messageType)
            {
                case MessageType.Goodbye:
                    result = Properties.strings.Goodbye;
                    break;
                case MessageType.Greeting:
                    result = Properties.strings.Greeting;
                    break;
                case MessageType.MealQuestion:
                    result = Properties.strings.MealQuestion;
                    break;
                case MessageType.NewContent:
                    result = Properties.strings.NewContent;
                    break;
            }
            if (formatters != null && formatters.Any() && result.Contains("{0}"))
                result = string.Format(result, formatters);

            if (result.Contains(' '))
                result = result.Replace(' ', spaceCharacter);
            else
                result = result + spaceCharacter;

            return result;
        }

        internal string[] GetFormatters(MessageType messageType)
        {
            string[] result = null;

            switch (messageType)
            {
                case MessageType.Goodbye:
                case MessageType.Greeting:
                    result = null;
                    break;
                case MessageType.MealQuestion:
                    result = new string[] { Properties.strings.Breakfast };
                    break;
                case MessageType.NewContent:
                    result = new string[] { Guid.NewGuid().ToString() };
                    break;
            }

            return result;
        }
    }
}
