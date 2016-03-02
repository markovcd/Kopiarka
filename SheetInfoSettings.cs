using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Kopiarka.Classes
{
    class SheetInfoSettings : IEnumerable<SheetInfo>
    {
        private readonly IList<SheetInfo> sheetInfos;

        public SheetInfoSettings(string path)
        {
            sheetInfos = new List<SheetInfo>();

            using (var file = new StreamReader(path))
                while (!file.EndOfStream)
                {
                    var line = file.ReadLine().Split('\t');
                    var password = line.Length == 3 ? line[2] : "";
                    var sheet = new SheetInfo(line[0], (Period)int.Parse(line[1]), password);
                    sheetInfos.Add(sheet);
                }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<SheetInfo> GetEnumerator()
        {
            return sheetInfos.GetEnumerator();
        }
    }
}
