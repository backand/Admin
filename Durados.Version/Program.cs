using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Durados.Version
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Count() != 2)
            {
                throw new DuradosInvalidArgumentException();

            }
            string filename = args[0];
            string subVersionStr = args[1];
            int subVersionInt = 0;

            Version version = null;

            if (!Int32.TryParse(subVersionStr, out subVersionInt))
            {
                throw new DuradosInvalidArgumentException();

            }
            else
            {
                if (subVersionInt < 1 || subVersionInt > 4)
                {
                    throw new DuradosInvalidArgumentException();

                }
                if (!File.Exists(filename))
                {
                    throw new DuradosInvalidArgumentException("The file not " + filename + " was found. ");
                }
            }

            TextReader tr = new StreamReader(filename);

            // read a line of text
            string versionStr = tr.ReadLine();


            version = new Version(versionStr);


            // close the stream
            tr.Close();

            version.Advance(subVersionInt);

            // create a writer and open the file
            TextWriter tw = new StreamWriter(filename);

            // write a line of text to the file
            tw.WriteLine(version.ToString());

            // close the stream
            tw.Close();
        }
    }

    public class DuradosInvalidArgumentException : Exception
    {
        public DuradosInvalidArgumentException()
            : this(string.Empty)
        {
        }
        
        public DuradosInvalidArgumentException(string message)
            : base(message + "Please provide the version file name as the first argument and the version sub number to advance (4-javascript, 3-minor, 2-major, 1-version)")
        {
        }
    }

    public class Version
    {
        public int Number { get; private set; }
        public int Major { get; private set; }
        public int Minor { get; private set; }
        public int JavaScript { get; private set; }

        public Version(string versionString)
        {
            Parse(versionString);
        }

        private void Parse(string versionString)
        {
            string[] numbersString = versionString.Split('.');

            if (numbersString.Count() != 4)
            {
                throw new DuradosInvalidArgumentException("Could not read the version: " + versionString + ". ");
            }

            int number = 0;
            if (!int.TryParse(numbersString[0], out number))
            {
                throw new DuradosInvalidArgumentException("Could not read the first version number: " + versionString[0] + ". ");
            }
            else
            {
                Number = number;
            }

            int major = 0;
            if (!int.TryParse(numbersString[1], out major))
            {
                throw new DuradosInvalidArgumentException("Could not read the second version number: " + versionString[1] + ". ");
            }
            else
            {
                Major = major;
            }

            int minor = 0;
            if (!int.TryParse(numbersString[2], out minor))
            {
                throw new DuradosInvalidArgumentException("Could not read the third version number: " + versionString[2] + ". ");
            }
            else
            {
                Minor = minor;
            }

            int javaScript = 0;
            if (!int.TryParse(numbersString[3], out javaScript))
            {
                throw new DuradosInvalidArgumentException("Could not read the fourth version number: " + versionString[3] + ". ");
            }
            else
            {
                JavaScript = javaScript;
            }
        }
        public void Advance(int subVersion)
        {
            switch (subVersion)
            {
                case 1:
                    Number++;
                    return;
                case 2:
                    Major++;
                    return;
                case 3:
                    Minor++;
                    return;
                case 4:
                    JavaScript++;
                    return;
                default:
                    return;
            }
        }

        public override string ToString()
        {
            return Number.ToString() + "." + Major.ToString() + "." + Minor.ToString() + "." + JavaScript.ToString();
        }
    }
}
