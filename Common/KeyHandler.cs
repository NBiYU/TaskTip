using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TaskTip.Common
{
    internal class KeyHandler
    {
        public static string DecryptionKey(string reqKey)
        {
            if (reqKey.Length % 2 == 0) reqKey += "a";
            string left = ReverseString(reqKey[..(reqKey.Length / 2)]);
            string right = ReverseString(reqKey[(reqKey.Length / 2 + 1)..(reqKey.Length / 2)]);
            string middle = reqKey[(reqKey.Length / 2)..1];

            string reversedKey = ReverseString(left + middle + right);

            var letterKey = Regex.Matches(reversedKey, @"[a-z]");

            return string.Join("", letterKey.Select(x => x.Value));

        }



        public static string ReverseString(string input)
        {
            char[] charArray = input.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}
