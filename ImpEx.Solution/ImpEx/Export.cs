using Newtonsoft.Json;
using ObjectFlattener;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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
        public static async Task<string> SaveAsCSVAsync<T>(IEnumerable<T> data, string fileName, bool overWriteExistingFile = false)
        {
            string completeFileName = GetFileName(fileName, ".csv");
            DeleteExistingFileIfWanted(completeFileName, overWriteExistingFile);
            await SaveAsTextAsync(data, completeFileName, ", ");
            return completeFileName;
        }
        public static async Task<string> SaveAsTSVAsync<T>(IEnumerable<T> data, string fileName, bool overWriteExistingFile = false)
        {
            string completeFileName = GetFileName(fileName, ".tsv");
            DeleteExistingFileIfWanted(completeFileName, overWriteExistingFile);
            await SaveAsTextAsync(data, completeFileName, "\t");
            return completeFileName;
        }

        #region helpers

        private static string GetFileName(string fileName, string fileNameSuffix)
        {
            if (!fileName.Split('.').Length.Equals(fileNameSuffix))
                fileName += fileNameSuffix;

            return fileName;
        }
        private static void DeleteExistingFileIfWanted(string fileName, bool overWriteExistingFile)
        {
            if (File.Exists(fileName) && overWriteExistingFile)
                File.Delete(fileName);
            else if (File.Exists(fileName) && !overWriteExistingFile)
                throw new ArgumentException($"{fileName} already exists.");
        }
        private static async Task SaveAsTextAsync<T>(IEnumerable<T> data, string fileName, string separator = ", ")
        {
            var lastCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

            try
            {
                var flattenedData = data.FlattenToObjects();

                string result = string.Join(separator, flattenedData);
                await File.AppendAllTextAsync(fileName, result);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = lastCulture;
            }       
        }

        #endregion
    }
}
