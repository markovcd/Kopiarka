using System;
using System.IO;

namespace Kopiarka.Classes
{
    public enum Period { Daily, Monthly }

    class SheetInfo
    {
        public string Pattern { get; private set; }
        public Period Period { get; private set; }
        public string Password { get; private set; }

        public SheetInfo(string pattern, Period period, string password = "")
        {
            Pattern = pattern;
            Period = period;
            Password = password;
        }

        public string ConstructPath(string rootPath)
        {
            return PathNormalizer.Normalize(rootPath + Path.DirectorySeparatorChar + PathNormalizer.CleanQuotes(Pattern));
        }

        public string ConstructPath(string rootPath, DateTime date)
        {
            return PathNormalizer.Normalize(rootPath + Path.DirectorySeparatorChar + date.ToString(Pattern));
        }
    }
}
