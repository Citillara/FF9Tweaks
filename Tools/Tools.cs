using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF9Tweaks.Tools
{
    internal class Tools
    {
        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:X2}", b);
            return hex.ToString();
        }

        public static string UintArrayToString(uint[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 8);
            foreach (uint b in ba)
                hex.AppendFormat("{0:X8}", b);
            return hex.ToString();
        }
    }
}
