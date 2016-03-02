using System;
using System.IO;
using System.Linq;

namespace Kopiarka.Classes
{
    static class PathNormalizer
    {
        public static string Normalize(string path)
        {
            return Path.GetFullPath(new Uri(path).LocalPath)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

        public static string CleanQuotes(string path)
        {
            return new string(path.Where(c => c != '"').ToArray());
        }
    }
}
