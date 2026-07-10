using Aquality.Selenium.Browsers;
using YandexDiskApi.Config;
using YandexDiskApi.Forms;
using YandexDiskApi.TestData;

namespace YandexDiskApi.Steps;

public class LoginSteps
{
    public void LoginWithConfiguration()
    {
        var configuration = TestConfiguration.Instance;
        Login(configuration.Login, configuration.Password);
    }

    public void Login(string login, string password)
    {
        var loginForm = new LoginForm();
        var timeouts = TestDataProvider.Instance.Timeouts;

        AqualityServices.Logger.Info("Ввод логина.");
        loginForm.ClickLoginField();
        loginForm.TypeLogin(login);
        loginForm.ClickNext();

        var passwordPageShown = AqualityServices.ConditionalWait.WaitFor(
            loginForm.IsPasswordPageOpened,
            TimeSpan.FromSeconds(timeouts.LoginSeconds),
            TimeSpan.FromSeconds(timeouts.PollingSeconds));

        if (passwordPageShown)
        {
            AqualityServices.Logger.Info("Ввод пароля.");
            loginForm.ClickPasswordField();
            loginForm.TypePassword(password);
            loginForm.ClickNext();
        }
        else
        {
            AqualityServices.Logger.Info("Страница ввода пароля не отображается. Шаг с паролем пропущен.");
        }
    }

    public void DismissFingerprintPromptIfDisplayed()
    {
        var loginForm = new LoginForm();
        if (loginForm.RemindLaterButton.State.IsDisplayed)
        {
            AqualityServices.Logger.Info("Закрытие запроса отпечатка.");
            loginForm.ClickRemindLater();
        }
    }
}
