using Aquality.Selenium.Elements.Interfaces;
using Aquality.Selenium.Forms;
using OpenQA.Selenium;

namespace UserInyerface.PageObjects
{
    public class HomePage : Form
    {
        public HomePage() : base(By.XPath("//*[@class='logo']"), "Home Page")
        {
        }

        private ILink NextPage => ElementFactory.GetLink(
            By.XPath("//*[contains(text(), 'HERE')]"), "Next Page");

        public void ClickNextPageLink()
        {
            NextPage.Click();
        }
    }
}
