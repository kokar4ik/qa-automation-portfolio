using Aquality.Selenium.Browsers;
using Aquality.Selenium.Elements.Interfaces;
using Aquality.Selenium.Forms;
using OpenQA.Selenium;
using YandexDiskApi.Constants;
using YandexDiskApi.Utils;

namespace YandexDiskApi.Forms;

public class LoginForm : Form
{
    private static readonly By LoginInputLocator = By.XPath("//input[@aria-label='Логин или email']");
    private static readonly By NextButtonLocator = By.XPath("//*[text()='Далее']");
    private static readonly By PasswordInputLocator = By.XPath("//input[@type='password']");
    private static readonly By RemindLaterButtonLocator = By.XPath("//*[text()='Напомнить позже']");

    public LoginForm()
        : base(LoginInputLocator, "Вход в Яндекс")
    {
    }

    private ITextBox LoginTextBox =>
        ElementFactory.GetTextBox(LoginInputLocator, "Логин");

    private ITextBox PasswordTextBox =>
        ElementFactory.GetTextBox(PasswordInputLocator, "Пароль");

    private IButton NextButton =>
        ElementFactory.GetButton(NextButtonLocator, "Далее");

    public IButton RemindLaterButton =>
        ElementFactory.GetButton(RemindLaterButtonLocator, "Напомнить позже");

    public void ClickLoginField() =>
        LoginTextBox.Click();

    public void TypeLogin(string login) =>
        LoginTextBox.ClearAndType(login);

    public void ClickNext() =>
        NextButton.Click();

    public bool IsPasswordFieldDisplayed() =>
        PasswordTextBox.State.IsDisplayed;

    public void ClickPasswordField() =>
        PasswordTextBox.Click();

    public void TypePassword(string password) =>
        PasswordTextBox.ClearAndType(password);

    public bool IsPasswordPageOpened() =>
        AqualityServices.Browser.CurrentUrl.Contains(UrlFragments.AuthPassword, StringComparison.OrdinalIgnoreCase)
        || PasswordTextBox.State.IsDisplayed;

    public void ClickRemindLater() =>
        RemindLaterButton.Click();
}
