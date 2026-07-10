using WindowsInput;
using WindowsInput.Native;

namespace UserInyerface.Utils
{
    public static class FileUtils
    {
        public static void UploadFileViaOsDialog(string absoluteFilePath, int delayBeforeTypingMs = 500)
        {
            var simulator = new InputSimulator();
            simulator.Keyboard.Sleep(delayBeforeTypingMs);
            simulator.Keyboard.TextEntry(absoluteFilePath);
            simulator.Keyboard.KeyPress(VirtualKeyCode.RETURN);
        }
    }
}
