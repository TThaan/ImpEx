using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace ImpEx
{
    public class Export
    {
        public static async Task SaveAsJsonAsync<T>(T data, string fileName, bool overWriteExistingFile = false)
        {
            if (File.Exists(fileName) && overWriteExistingFile)
                File.Delete(fileName);
            else if (File.Exists(fileName) && !overWriteExistingFile)
                throw new ArgumentException($"{fileName} already exists.");

            string json = JsonConvert.SerializeObject(data);
            await File.AppendAllTextAsync(fileName, json);
        }
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
        public static async Task SaveAsCSVAsync<T>(IEnumerable<T> data, string fileName, bool overWriteExistingFile = false)
        {
            if (File.Exists(fileName) && overWriteExistingFile)
                File.Delete(fileName);
            else if (File.Exists(fileName) && !overWriteExistingFile)
                throw new ArgumentException($"{fileName} already exists.");

            string csv = data.FromCollectionToString(",");
            await File.AppendAllTextAsync(fileName, csv);
        }public static async Task SaveAsTSVAsync<T>(IEnumerable<T> data, string fileName, bool overWriteExistingFile = false)
        {
            if (File.Exists(fileName) && overWriteExistingFile)
                File.Delete(fileName);
            else if (File.Exists(fileName) && !overWriteExistingFile)
                throw new ArgumentException($"{fileName} already exists.");

            string tsv = data.FromCollectionToString("\t");
            await File.AppendAllTextAsync(fileName, tsv);
        }
    }
}
