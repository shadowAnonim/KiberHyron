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
        public static T GetAllData<T>() where T : JsonData, new()
        {
            using (FileStream fs = new FileStream($"Data/{typeof(T).Name}.json", FileMode.OpenOrCreate))
            {
                try
                {
                    T data = JsonSerializer.Deserialize<T>(fs);
                    return data;
                }
                catch (Exception ex) 
                {
                    return new T();
                }
            }
        }

        public static void WriteNewData<T>(T data) where T : JsonData
        {
            try
            {
                using (FileStream fs = new FileStream($"Data/{typeof(T).Name}.json", FileMode.Create))
                {
                    JsonSerializer.SerializeAsync<T>(fs, data);
                    Console.WriteLine($"Data has been saved to file: {$"Data/{typeof(T).Name}.json"}");
                }
            }
            catch
            {
                Console.WriteLine("ogo");
            }
        }
    }
}
