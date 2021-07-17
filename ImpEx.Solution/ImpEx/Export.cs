using Newtonsoft.Json;
using ObjectFlattener;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImpEx
{
    public class Export
    {
        public static async Task SaveAsBinaryAsync<T>(T data, string fileName, FileMode writeMode = FileMode.CreateNew)
        {
            await Task.Run(() =>
            {
                using (Stream stream = new FileStream(fileName, writeMode))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(stream, data);
                    stream.Close();
                }
            });
        }
        public static async Task SaveAsJsonAsync<T>(T data, string fileName, bool overWriteExistingFile = false)
        {
            DeleteExistingFileIfWanted(fileName, overWriteExistingFile);

            string json = JsonConvert.SerializeObject(data);
            await File.AppendAllTextAsync(fileName, json);
        }
        public static async Task<string> SaveAsCSVAsync<T>(IEnumerable<T> data, string fileName, int columns, bool overWriteExistingFile = false)
        {
            string completeFileName = GetFileName(fileName, ".csv");
            DeleteExistingFileIfWanted(completeFileName, overWriteExistingFile);
            await SaveAsTextAsync(data, completeFileName, columns, ", ");
            return completeFileName;
        }
        public static async Task<string> SaveAsTSVAsync<T>(IEnumerable<T> data, string fileName, int columns, bool overWriteExistingFile = false)
        {
            string completeFileName = GetFileName(fileName, ".tsv");
            DeleteExistingFileIfWanted(completeFileName, overWriteExistingFile);
            await SaveAsTextAsync(data, completeFileName, columns, "\t");
            return completeFileName;
        }

        #region helpers

        private static string GetFileName(string fileName, string wantedFileNameSuffix)
        {
            string currentSuffix = '.' + fileName.Split('.').Last();

            if (!currentSuffix.Equals(wantedFileNameSuffix))
                fileName += wantedFileNameSuffix;

            return fileName;
        }
        private static void DeleteExistingFileIfWanted(string fileName, bool overWriteExistingFile)
        {
            if (File.Exists(fileName) && overWriteExistingFile)
                File.Delete(fileName);
            else if (File.Exists(fileName) && !overWriteExistingFile)
                throw new ArgumentException($"{fileName} already exists.");
        }
        private static async Task SaveAsTextAsync<T>(IEnumerable<T> data, string fileName, int columns, string separator = ", ")
        {
            var lastCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

            try
            {
                var flattenedData = data.FlattenToObjects().ToList();
                int flattenedDataCount = flattenedData.Count();

                StringBuilder sb = new StringBuilder();

                for (int i = 1; i <= flattenedDataCount; i++)
                {
                    sb.Append(flattenedData.ElementAt(i - 1).ToString());

                    // Add line skip after each n-th (= 'columns-th') item to create multiple rows
                    if (i % columns == 0)
                        sb.Append("\n");
                    // else add a separator.
                    else
                        sb.Append(separator);
                }

                // Remove last comma
                sb.Remove(sb.Length -1, 1);

                await File.AppendAllTextAsync(fileName, sb.ToString());
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = lastCulture;
            }       
        }

        #endregion
    }
}
