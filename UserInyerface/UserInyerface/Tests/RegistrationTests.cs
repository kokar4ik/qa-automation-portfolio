using UserInyerface.PageObjects;
using UserInyerface.PageObjects.GameCards;
using UserInyerface.Steps;
using UserInyerface.TestData;
using UserInyerface.Utils;

namespace UserInyerface.Tests
{
    public class RegistrationTests : BaseTest
    {
        [Test]
        public void TestRegistrationFirstCardFlow()
        {
            var homePage = new HomePage();
            Assert.IsTrue(homePage.State.WaitForDisplayed(), $"{homePage.Name} не открылась");
            homePage.ClickNextPageLink();

            var loginCard = new LoginCard();
            Assert.IsTrue(loginCard.State.WaitForDisplayed(), $"{loginCard.Name} не появилась на экране");

            var registrationData = TestDataProvider.Data.Registration;
            var emailPrefix = Randomizer.GenerateRandomString(registrationData.EmailPrefixLength);
            var emailDomain = Randomizer.GenerateRandomString(registrationData.EmailDomainLength);
            var password = Randomizer.GenerateValidPassword(emailPrefix[0]);

            var registrationSteps = new RegistrationSteps();
            registrationSteps.CompleteLoginAndAvatarCards(
                password,
                emailPrefix,
                emailDomain,
                TestDataProvider.GetAvatarFilePath(),
                registrationData.InterestsCount);

            var personalCard = new PersonalDetailsCard();
            Assert.IsTrue(personalCard.State.WaitForDisplayed(), $"{personalCard.Name} не появилась");
        }
    }
}