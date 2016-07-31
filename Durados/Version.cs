using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados
{
    public class Version
    {
        public int Number { get; private set; }
        public int Major { get; private set; }
        public int Minor { get; private set; }

        public Version(string versionString)
        {
            Parse(versionString);
        }

        private void Parse(string versionString)
        {
            if (string.IsNullOrEmpty(versionString))
            {
                Number = 1;
                Major = 0;
                Minor = 0;
                return;
            }

            string[] numbersStringArray = versionString.Split('.');
            List<string> numbersString = new List<string>();

            for (int i = 0; i < Math.Min(3, numbersStringArray.Count()); i++)
            {
                numbersString.Add(numbersStringArray[i]);
            }
            int number = 1;
            if (!int.TryParse(numbersString[0], out number))
            {
                Number = 1;
            }
            else
            {
                Number = number;
            }

            int major = 0;
            if (!int.TryParse(numbersString[1], out major))
            {
                Major = 0;
            }
            else
            {
                Major = major;
            }

            int minor = 0;
            if (!int.TryParse(numbersString[2], out minor))
            {
                Minor = 0;
            }
            else
            {
                Minor = minor;
            }

        }

        public void NextMinor()
        {
            Minor++;
        }

        public void NextMajor()
        {
            Major++;
        }

        
        public void NextNumber()
        {
            Number++;
        }

        

        public override string ToString()
        {
            return Number.ToString() + "." + Major.ToString() + "." + Minor.ToString();
        }
    }
}
