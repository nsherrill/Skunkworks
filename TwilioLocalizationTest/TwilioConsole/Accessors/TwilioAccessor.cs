using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using TwilioConsole.Shared;

namespace TwilioConsole.Accessors
{
    public class TwilioAccessor
    {
        public SMSResult SendSMS(NotificationSettings sendingObject)
        {
            try
            {
                TwilioClient.Init(Configs.SmsAccountSid, Configs.SmsAuthToken);

                var toPhone = sendingObject.PhoneNumber;
                var fromPhone = $"+{Configs.SmsSourcePhoneNumber}";

                var message = MessageResource.Create(
                    to: new PhoneNumber(toPhone),
                    from: new PhoneNumber(fromPhone),
                    body: sendingObject.Content,
                    smartEncoded: false
                    
                    
                );

                return new SMSResult()
                {
                    IsSuccess = true,
                };
            }
            catch (Exception e)
            {
                return new SMSResult()
                {
                    ErrorMessage = e.ToString(),
                };
            }
        }
    }
}
