using Aquality.Selenium.Browsers;
using Aquality.Selenium.Elements.Interfaces;
using Aquality.Selenium.Forms;
using OpenQA.Selenium;
using UserInyerface.Constants;

namespace UserInyerface.PageObjects
{
    public class HelpForm : Form
    {
        private const int StableChecksRequired = 3;

        private static readonly By RootHelpFormLocator = By.XPath(
            "//div[contains(@class, 'help-form') and not(contains(@class, '_container'))]");

        private readonly IButton sendToBottomButton;
        private readonly ILabel rootHelpForm;

        public HelpForm() : base(By.ClassName("help-form__container"), "Help Form")
        {
            sendToBottomButton = ElementFactory.GetButton(
                By.XPath("//*[contains(@class, 'send-to-bottom-button')]"), "Send To Bottom");
            rootHelpForm = ElementFactory.GetLabel(RootHelpFormLocator, "Root Help Form");
        }

        public void SendToBottom()
        {
            sendToBottomButton.Click();
        }

        public void WaitUntilMovementStopped(int timeoutSeconds = 15)
        {
            var previousPosition = GetVerticalPosition();
            var stableChecks = 0;

            ConditionalWait.WaitForTrue(
                () =>
                {
                    var currentPosition = GetVerticalPosition();

                    if (Math.Abs(currentPosition - previousPosition) < 1)
                    {
                        stableChecks++;
                        if (stableChecks >= StableChecksRequired)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        stableChecks = 0;
                        previousPosition = currentPosition;
                    }

                    return false;
                },
                TimeSpan.FromSeconds(timeoutSeconds),
                TimeSpan.FromMilliseconds(200),
                $"{Name} не остановилась за {timeoutSeconds} секунд.");
        }

        public bool IsFormHidden()
        {
            var classAttribute = rootHelpForm.GetAttribute(HtmlAttributes.Class);
            return classAttribute != null && classAttribute.Contains(CssClasses.IsHidden);
        }

        private double GetVerticalPosition()
        {
            var webElement = AqualityServices.Browser.Driver.FindElement(RootHelpFormLocator);
            return webElement.Location.Y;
        }
    }
}