using UserInyerface.PageObjects;
using UserInyerface.TestData;

namespace UserInyerface.Tests
{
    public class TimerTests : BaseTest
    {
        [Test]
        public void TestTimerStartsFromZero()
        {
            var homePage = new HomePage();
            Assert.IsTrue(homePage.State.WaitForDisplayed(), $"{homePage.Name} не открылась");
            homePage.ClickNextPageLink();

            var timerForm = new TimerForm();
            Assert.IsTrue(timerForm.State.WaitForDisplayed(), $"{timerForm.Name} не появился на экране");

            var expectedTimerValue = TestDataProvider.Data.Timer.InitialValue;
            var actualTimerText = timerForm.GetTimerText();

            Assert.That(actualTimerText, Is.EqualTo(expectedTimerValue),
                $"Ожидалось, что таймер начнется с '{expectedTimerValue}', но было: {actualTimerText}");
        }
    }
}
