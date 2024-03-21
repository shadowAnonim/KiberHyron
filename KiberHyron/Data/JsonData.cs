using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KiberHyron.Data
{
    public abstract class JsonData
    {
        public static T GetAllData<T>() where T : JsonData
        {
            using (FileStream fs = new FileStream($"Data/{typeof(T).Name}.json", FileMode.OpenOrCreate))
            {
                T data = JsonSerializer.Deserialize<T>(fs);
                return data;
            }
        }

        public static void WriteNewData<T>(T data) where T : JsonData
        {
            using (FileStream fs = new FileStream($"Data/{typeof(T).Name}.json", FileMode.OpenOrCreate))
            {
                JsonSerializer.SerializeAsync<T>(fs, data);
                Console.WriteLine($"Data has been saved to file: {$"Data/{typeof(T).Name}.json"}");
            }
        }
    }
}
