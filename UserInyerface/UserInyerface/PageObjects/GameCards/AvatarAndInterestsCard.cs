using Aquality.Selenium.Elements.Interfaces;
using Aquality.Selenium.Forms;
using OpenQA.Selenium;

namespace UserInyerface.PageObjects.GameCards
{
    public class AvatarAndInterestsCard : Form
    {
        private static readonly By InterestLabelsLocator = By.XPath(
            "//label[starts-with(@for, 'interest_') and not(contains(@for, 'selectall'))]");

        private readonly IButton upload;
        private readonly ILabel unselectAll;
        private readonly IButton next;

        public AvatarAndInterestsCard() : base(By.ClassName("avatar-and-interests"), "Avatar And Interests Card")
        {
            upload = ElementFactory.GetButton(By.XPath("//*[text()='upload']"), "Upload");
            unselectAll = ElementFactory.GetLabel(By.CssSelector("[for='interest_unselectall']"), "Unselect All");
            next = ElementFactory.GetButton(By.XPath("//button[text()='Next']"), "Next");
        }

        public void ClickUpload()
        {
            upload.Click();
        }

        public void UnselectAllInterests()
        {
            unselectAll.Click();
        }

        public IList<ILabel> GetInterestLabels()
        {
            return ElementFactory.FindElements<ILabel>(InterestLabelsLocator, "Interest Labels");
        }

        public void ClickInterest(ILabel interest)
        {
            interest.Click();
        }

        public void ClickNext()
        {
            next.Click();
        }
    }
}