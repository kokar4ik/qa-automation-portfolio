using Aquality.Selenium.Elements.Interfaces;
using Aquality.Selenium.Forms;
using OpenQA.Selenium;

namespace UserInyerface.PageObjects.GameCards
{
    public class LoginCard : Form
    {
        private static readonly By DomainOptionsLocator = By.XPath("//div[contains(@class, 'dropdown__list')]//div");

        private readonly ITextBox password;
        private readonly ITextBox emailPrefix;
        private readonly ITextBox emailDomain;
        private readonly IButton dropdownOpener;
        private readonly ICheckBox termsCheckBox;
        private readonly IButton next;

        public LoginCard() : base(By.ClassName("login-form"), "Login Card")
        {
            password = ElementFactory.GetTextBox(By.XPath("//input[@placeholder='Choose Password']"), "Password");
            emailPrefix = ElementFactory.GetTextBox(By.XPath("//input[@placeholder='Your email']"), "Email Prefix");
            emailDomain = ElementFactory.GetTextBox(By.XPath("//input[@placeholder='Domain']"), "Email Domain");
            dropdownOpener = ElementFactory.GetButton(By.ClassName("dropdown__opener"), "Dropdown Opener");
            termsCheckBox = ElementFactory.GetCheckBox(By.XPath("//span[contains(@class, 'checkbox__box')]"), "Accept Terms");
            next = ElementFactory.GetButton(By.XPath("//a[text()='Next']"), "Next");
        }

        public void EnterPassword(string value)
        {
            password.ClearAndType(value);
        }

        public void EnterEmailPrefix(string value)
        {
            emailPrefix.ClearAndType(value);
        }

        public void EnterEmailDomain(string value)
        {
            emailDomain.ClearAndType(value);
        }

        public void SelectRandomDomain()
        {
            dropdownOpener.Click();
            var domainOptions = ElementFactory.FindElements<IButton>(DomainOptionsLocator, "Domain Option");
            var randomIndex = Random.Shared.Next(domainOptions.Count);
            domainOptions[randomIndex].Click();
        }

        public void AcceptTerms()
        {
            termsCheckBox.Check();
        }

        public void ClickNext()
        {
            next.Click();
        }
    }
}