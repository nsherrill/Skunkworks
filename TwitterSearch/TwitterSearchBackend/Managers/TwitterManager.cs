using System;
using System.Collections.Generic;
using System.Text;
using TwitterSearchBackend.Accessors;
using TwitterSearchBackend.Engines;

namespace TwitterSearchBackend.Managers
{
    public interface ITwitterManager : ITwitterSearchServiceBase
    {
        TweetArrayResult Search(string textToSearch);
    }

    public class TwitterManager : TwitterSearchServiceBase, ITwitterManager
    {
        private ITwitterApiAccessor twitterAcc { get; set; }
        private IValidationEngine validationEng { get; set; }

        public void FactoryOverride<T>(T service)
        {
            if (typeof(T) == typeof(ITwitterApiAccessor))
                this.twitterAcc = (ITwitterApiAccessor)service;
            else if (typeof(T) == typeof(IValidationEngine))
                this.validationEng = (IValidationEngine)service;
        }

        public TweetArrayResult Search(string textToSearch)
        {
            ValidationResult validationResult = null;
            TweetArrayResult result = null;

            Init();

            try
            {
                validationResult = validationEng.ValidateSearchText(textToSearch);

                if (validationResult.IsValid)
                {
                    result = twitterAcc.SearchForTweets(textToSearch);
                }
                else
                    result = new TweetArrayResult()
                    {
                        Error = validationResult.Error
                    };
            }
            catch (Exception e)
            {
                Logger.Error(string.Format("Exception caught within manager for [{0}]", textToSearch), e);
                return new TweetArrayResult()
                {
                    Error = ErrorCodes.ExceptionCaught(ErrorTextType.Internal, e)
                };
            }

            return result;
        }

        public override string TestMe(string source = null)
        {
            Init();

            source = this.validationEng.TestMe(source);
            source = this.twitterAcc.TestMe(source);
            return base.TestMe(source);
        }

        #region privates
        private void Init()
        {
            if (validationEng == null)
                validationEng = new ValidationEngine();

            if (twitterAcc == null)
                twitterAcc = new TwitterAPIAccessor();
        }
        #endregion
    }
}
