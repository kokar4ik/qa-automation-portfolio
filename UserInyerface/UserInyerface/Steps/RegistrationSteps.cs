using UserInyerface.PageObjects.GameCards;
using UserInyerface.Utils;

namespace UserInyerface.Steps
{
    public class RegistrationSteps
    {
        private readonly LoginCard loginCard = new();
        private readonly AvatarAndInterestsCard avatarCard = new();

        public void CompleteLoginAndAvatarCards(
            string password,
            string emailPrefix,
            string emailDomain,
            string avatarPath,
            int interestsCount)
        {
            loginCard.EnterPassword(password);
            loginCard.EnterEmailPrefix(emailPrefix);
            loginCard.EnterEmailDomain(emailDomain);
            loginCard.SelectRandomDomain();
            loginCard.AcceptTerms();
            loginCard.ClickNext();

            avatarCard.State.WaitForDisplayed();

            avatarCard.ClickUpload();
            FileUtils.UploadFileViaOsDialog(avatarPath);

            avatarCard.UnselectAllInterests();
            var random = new Random();
            var selectedInterests = avatarCard.GetInterestLabels()
                .OrderBy(_ => random.Next())
                .Take(interestsCount)
                .ToList();

            foreach (var interest in selectedInterests)
            {
                avatarCard.ClickInterest(interest);
            }

            avatarCard.ClickNext();
        }
    }
}