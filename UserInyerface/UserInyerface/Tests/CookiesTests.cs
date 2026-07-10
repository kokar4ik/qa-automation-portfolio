using UserInyerface.PageObjects;

namespace UserInyerface.Tests
{
    public class CookiesTests : BaseTest
    {
        [Test]
        public void TestAcceptCookiesFormCloses()
        {
            var homePage = new HomePage();
            Assert.IsTrue(homePage.State.WaitForDisplayed(), $"{homePage.Name} не открылась");
            homePage.ClickNextPageLink();

            var cookiesForm = new CookiesForm();
            Assert.IsTrue(cookiesForm.State.WaitForDisplayed(), $"{cookiesForm.Name} не появился на экране");

            cookiesForm.AcceptCookies();

            Assert.IsTrue(cookiesForm.State.WaitForNotDisplayed(), $"{cookiesForm.Name} не исчез после клика по кнопке");
        }
    }
}