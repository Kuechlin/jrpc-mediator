using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.Client
{
    internal static class Utils
    {
        public static bool Question(string text, bool @default = false)
        {
            var defaultText = @default ? "[Y/n]" : "[y/N]";

            Console.WriteLine(text + " " + defaultText);

            var answer = Console.ReadLine();
            if (new[] { "y", "Y", "yes" }.Contains(answer))
            {
                return true;
            }
            else if (new[] { "n", "N", "no" }.Contains(answer))
            {
                return false;
            }
            else return @default;
        }
    }
}
