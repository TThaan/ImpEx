using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace ImpEx
{
    public class Import
    {
        public static async Task<string> LoadAsOriginalFileTextAsync(string fileName)
        {
            return await File.ReadAllTextAsync(fileName);
        }
        public static async Task<T> LoadAsJsonAsync<T>(string fileName)
        {
            var json = await File.ReadAllTextAsync(fileName);
            return JsonConvert.DeserializeObject<T>(json);
        }
        public static async Task<T> LoadAsBinaryAsync<T>(string fileName)
        {            
            return await Task.Run(() =>
            {
                using (Stream stream = File.Open(fileName, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    return (T)bf.Deserialize(stream);
                }
            });
        }
    }
}
