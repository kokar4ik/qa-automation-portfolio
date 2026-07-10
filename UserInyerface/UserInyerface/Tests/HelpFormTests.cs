using UserInyerface.Constants;
using UserInyerface.PageObjects;

namespace UserInyerface.Tests
{
    public class HelpFormTests : BaseTest
    {
        [Test]
        public void TestHideHelpForm()
        {
            var homePage = new HomePage();
            Assert.IsTrue(homePage.State.WaitForDisplayed(), $"{homePage.Name} не открылась");
            homePage.ClickNextPageLink();

            var helpForm = new HelpForm();
            Assert.IsTrue(helpForm.State.WaitForDisplayed(), $"{helpForm.Name} не появилась на экране");

            helpForm.SendToBottom();
            helpForm.WaitUntilMovementStopped();

            Assert.IsTrue(helpForm.IsFormHidden(), $"{helpForm.Name} не получила статус '{CssClasses.IsHidden}' после перемещения вниз");
        }
    }
}
