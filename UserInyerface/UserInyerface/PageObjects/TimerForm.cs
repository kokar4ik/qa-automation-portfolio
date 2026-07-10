using Aquality.Selenium.Elements.Interfaces;
using Aquality.Selenium.Forms;
using OpenQA.Selenium;

namespace UserInyerface.PageObjects
{
    public class TimerForm : Form
    {
        public TimerForm() : base(By.ClassName("timer"), "Timer")
        {
        }

        private ILabel TimerLabel => ElementFactory.GetLabel(By.ClassName("timer"), "Timer");

        public string GetTimerText()
        {
            return TimerLabel.Text.Trim();
        }
    }
}