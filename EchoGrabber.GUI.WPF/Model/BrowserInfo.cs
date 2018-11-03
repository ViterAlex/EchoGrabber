using System.Drawing;

namespace EchoGrabber.GUI.WPF.Model
{
    public class BrowserInfo
    {
        public Icon Icon { get; private set; }
        public string Name { get; private set; }
        public string StartupPath { get; private set; }
        public bool IsDefault { get; private set; }
        public BrowserInfo(string name, string startupPath, Icon icon, bool isDefault)
        {
            Name = name;
            StartupPath = startupPath;
            Icon = icon;
            IsDefault = isDefault;
        }

        public override string ToString()
        {
            return $"{Name}: {StartupPath}";
        }
    }
}