using System;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace OpenWindowsKeyViewer
{
    class OpenWindowsKeyViewer
    {
        private const string Charset = "BCDFGHJKMPQRTVWXY2346789";
        
        static void Main(string[] args)
        {
            var regKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            var digitalProductId = (byte[]) regKey?.GetValue("DigitalProductId");
            var productName = (string) regKey?.GetValue("ProductName");
            var windowsKeyBytes = digitalProductId?.Skip(52).Take(15).ToArray();
            var windowsKey = DecodeWindowsKey(windowsKeyBytes);
            Console.WriteLine("{0}: {1}", productName, windowsKey);
            Console.ReadLine();
        }
        
        
        private static string DecodeWindowsKey(byte[] bytes) {
            var win8Byte = (bytes[14] / 6) & 1;
            bytes[14] = (byte) ((bytes[14] & 0xf7 | win8Byte & 2) * 4);
            var windowsKey = new StringBuilder();
            var position = 0;

            for (var digit = 24; digit >= 0; digit--)
            {
                position = 0;
                for (var idx = 14; idx >= 0; idx--)
                {
                    position = position * 256 + bytes[idx];
                    bytes[idx] = (byte) (position / 24);
                    position %= 24;
                }
                windowsKey.Insert(0, Charset[position]);
            }
            
            return windowsKey
                .Remove(0, 1)
                .Insert(position, "N")
                .Insert(20, "-")
                .Insert(15, "-")
                .Insert(10, "-")
                .Insert(5, "-")
                .ToString();
        }
    }
}