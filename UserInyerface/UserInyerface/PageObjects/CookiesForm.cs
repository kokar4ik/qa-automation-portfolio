using Aquality.Selenium.Elements.Interfaces;
using Aquality.Selenium.Forms;
using OpenQA.Selenium;

namespace UserInyerface.PageObjects
{
    public class CookiesForm : Form
    {
        public CookiesForm() : base(By.CssSelector(".cookies"), "Cookies Banner")
        {
        }

        private IButton AcceptCookiesButton => ElementFactory.GetButton(By.XPath("//*[text()='Not really, no']"), "Accept Cookies");

        public void AcceptCookies()
        {
            AcceptCookiesButton.Click();
        }
    }
}