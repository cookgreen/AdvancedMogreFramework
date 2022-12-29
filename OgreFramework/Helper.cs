using Mogre;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace org.ogre.framework
{
    public static class Helper
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
    
        public static StringVector ToStringVector<T>(this IEnumerable<T> list)
        {
            StringVector stringVector = new StringVector();

            foreach (var item in list)
            {
                stringVector.Add(item.ToString());
            }

            return stringVector;
        }

        public static T Clamp<T>(T val, T minval, T maxval) where T : IComparable
        {
            T temp;
            if (val.CompareTo(maxval) < 0)
            {
                temp = val;
            }
            else
            {
                temp = maxval;
            }
            if (temp.CompareTo(minval) > 0)
            {
                return temp;
            }
            else
            {
                return minval;
            }
        }

        public static MemoryStream DataPtrToStream(DataStreamPtr ptr)
        {
            if (ptr.Size() != 0)
            {
                byte[] buffer = new byte[ptr.Size()];
                unsafe
                {
                    fixed (byte* bufferPtr = &buffer[0])
                    {
                        ptr.Read(bufferPtr, (uint)buffer.Length);
                    }
                }
                MemoryStream memoryStream = new MemoryStream(buffer);
                return memoryStream;
            }
            return null;
        }
    }
}
