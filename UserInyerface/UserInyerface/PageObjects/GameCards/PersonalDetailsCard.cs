using OpenQA.Selenium;
using Aquality.Selenium.Forms;

namespace UserInyerface.PageObjects.GameCards
{
    public class PersonalDetailsCard : Form
    {
        public PersonalDetailsCard() : base(By.ClassName("personal-details"), "PersonalDetailsCard")
        {
        }
    }
}