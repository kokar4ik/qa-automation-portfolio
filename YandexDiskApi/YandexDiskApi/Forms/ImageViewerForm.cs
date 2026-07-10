using Aquality.Selenium.Browsers;
using Aquality.Selenium.Elements.Interfaces;
using Aquality.Selenium.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace YandexDiskApi.Forms;

public class ImageViewerForm : Form
{
    private static readonly By ImagePreviewLocator = By.CssSelector("img.scalable-preview__image");

    public ImageViewerForm()
        : base(ImagePreviewLocator, "Просмотр изображения")
    {
    }

    private ILabel ImagePreview =>
        ElementFactory.GetLabel(ImagePreviewLocator, "Превью изображения");

    public bool IsPreviewDisplayed() =>
        ImagePreview.State.IsDisplayed;

    public string GetImageSource() =>
        ImagePreview.GetElement().GetDomProperty("src") ?? string.Empty;

    public void Close()
    {
        new Actions(AqualityServices.Browser.Driver).SendKeys(Keys.Escape).Perform();
        AqualityServices.Browser.WaitForPageToLoad();
    }
}
