using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwilioConsole.Accessors;
using TwilioConsole.Engines;
using TwilioConsole.Shared;
using TwilioConsole.Shared.Enums;

namespace TwilioConsole.Managers
{
    public class TwilioConsoleManager
    {
        public void Start()
        {
            Dictionary<long, object> textOptions = new Dictionary<long, object>();
            Dictionary<long, object> languageOptions = new Dictionary<long, object>();

            textOptions.Add((long)MessageType.Greeting, MessageType.Greeting);
            textOptions.Add((long)MessageType.Goodbye, MessageType.Goodbye);
            textOptions.Add((long)MessageType.MealQuestion, MessageType.MealQuestion);
            textOptions.Add((long)MessageType.NewContent, MessageType.NewContent);

            languageOptions.Add((long)Languages.English, Languages.English);
            languageOptions.Add((long)Languages.Spanish, Languages.Spanish);
            languageOptions.Add((long)Languages.French, Languages.French);
            languageOptions.Add((long)Languages.German, Languages.German);

            string result = InteractWithUser(textOptions, languageOptions);
            while (!string.IsNullOrEmpty(result))
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                result = InteractWithUser(textOptions, languageOptions);
            }
        }

        private string InteractWithUser(Dictionary<long, object> textOptions, Dictionary<long, object> languageOptions)
        {
            long textOptionToSend = GetItem("What would you like to send?", textOptions);
            if (textOptionToSend == 0) return null;

            long languageToUse = GetItem("Which language?", languageOptions);
            if (languageToUse == 0) return null;

            var translationEng = new TranslationEngine();
            translationEng.SetLanguage((Languages)languageToUse);
            var formatters = translationEng.GetFormatters((MessageType)textOptionToSend);
            var textToSend = translationEng.GetText((MessageType)textOptionToSend, formatters);

            string destinationNumber = GetItem("What phone number?");
            if (string.IsNullOrEmpty(destinationNumber) || destinationNumber == "0") return null;

            var sendingObject = GenerateSendingObject(textToSend, destinationNumber);
            var twilioAcc = new TwilioAccessor();
            var sendResult = twilioAcc.SendSMS(sendingObject);
            if (sendResult.IsSuccess)
                return "success!";
            return null;
        }

        private NotificationSettings GenerateSendingObject(string textToSend, string destinationNumber)
        {
            return new NotificationSettings()
            {
                Content = textToSend,
                PhoneNumber = destinationNumber,
            };
        }

        private long GetItem(string headerText, Dictionary<long, object> options)
        {
            string result = null;
            long optionSelected = 0;
            do
            {
                Console.WriteLine();
                Console.WriteLine(headerText);
                foreach (var item in options.Keys.OrderBy(k => k))
                    Console.WriteLine($"{item}: {options[item]}");
                Console.WriteLine("0: --quit--");

                result = Console.ReadLine();
                long.TryParse(result, out optionSelected);
            } while (string.IsNullOrEmpty(result) || optionSelected == 0);

            return optionSelected;
        }

        private string GetItem(string headerText)
        {
            string result = null;
            do
            {
                Console.WriteLine();
                Console.WriteLine(headerText);
                Console.WriteLine("0: --quit--");

                result = Console.ReadLine();
            } while (string.IsNullOrEmpty(result));

            return result;
        }
    }
}
