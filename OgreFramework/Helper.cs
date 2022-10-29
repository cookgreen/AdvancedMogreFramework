using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace org.ogre.framework
{
    public class Helper
    {
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        private static extern int DrawMenuBar(int currentWindow);

        private const int WM_SETICON = 0x80;
        private const int ICON_SMALL = 0;

        public static void SetWindowIcon(Icon icon, IntPtr hwnd)
        {
            if (icon == null || hwnd == null || hwnd == IntPtr.Zero) // check parameters
                return;
            SendMessage(hwnd, WM_SETICON, (IntPtr)ICON_SMALL, (IntPtr)icon.Handle); // Set the icon with SendMessage
            DrawMenuBar((int)hwnd);
        }
    }
}
