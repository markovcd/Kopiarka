using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using System.Collections.Generic;

namespace Kopiarka.Classes
{
	public class SheetCopier : BackgroundWorker
    {
        public SheetCopier(DateTime from, DateTime to, string source, string confFile, string dest, bool updateDate, bool overwrite)
        {
        	WorkerReportsProgress = true;
        	WorkerSupportsCancellation = true; 
        	DoWork += delegate(object sender, DoWorkEventArgs e) { Copy(from, to, source, confFile, dest, updateDate, overwrite, e); };
		
        }
		
		private void Copy(DateTime from, DateTime to, string source, string confFile, string dest, bool updateDate, bool overwrite, DoWorkEventArgs e)
        {
            var settings = new SheetInfoSettings(source + Path.DirectorySeparatorChar + confFile);
            var days = new DateEnumerable(Period.Daily, from, to).ToList();
            var months = new DateEnumerable(Period.Monthly, from, to).ToList();
            var total = days.Count*settings.Count(s => s.Period == Period.Daily) +
                        months.Count*settings.Count(s => s.Period == Period.Monthly);

            var i = 0;
            try
            {
            	Copy(months, settings.Where(s => s.Period == Period.Monthly), source, dest, updateDate, overwrite, e, total, ref i);
                Copy(days, settings.Where(s => s.Period == Period.Daily), source, dest, updateDate, overwrite, e, total, ref i);
                
            }
            catch (Exception ex)
            {
                e.Cancel = true;
            	MessageBox.Show(ex.Message, e.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
		
		private void Copy(IEnumerable<DateTime> dates, IEnumerable<SheetInfo> settings, string source, string dest, bool updateDate, bool overwrite, DoWorkEventArgs e, int total, ref int i)
		{
			foreach (var date in dates)
            {
                if (CancellationPending) 
	            {
	            	e.Cancel = true;
	            	break;
	            }
				
				
				foreach (var sheetInfo in settings)
                {
                    if (CancellationPending) 
		            {
		            	e.Cancel = true;
		            	break;
		            }
                	
                	Copy(source, dest, sheetInfo, date, updateDate, overwrite);
                    i++;
                }
				ReportProgress(i*100/total);
            }
		}

        private static void Copy(string source, string dest, SheetInfo sheetInfo, DateTime date, bool updateDate = false, bool overwrite = false)
        {
            var sourcePath = sheetInfo.ConstructPath(source);
            var destPath = sheetInfo.ConstructPath(dest, date);
            Directory.CreateDirectory(Path.GetDirectoryName(destPath));
            File.Copy(sourcePath, destPath, overwrite);

            if (updateDate) 
            	SheetWriter.UpdateDate(date, sheetInfo.Period, destPath, sheetInfo.Password);
        }
       
    }
}
