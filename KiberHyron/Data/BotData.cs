using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiberHyron.Data
{
    public class BotData
    {
        public List<RoleGame> Games { get; set; } = new List<RoleGame>();

        public static BotData GetAllData()
        {
            using (FileStream fs = new FileStream("Data/GamesData.json", FileMode.OpenOrCreate))
            {
                BotData gamesData = JsonSerializer.Deserialize<BotData>(fs);
                return gamesData;
            }
        }

        public static void WriteNewData(BotData data)
        {
            using (FileStream fs = new FileStream("Data/GamesData.json", FileMode.OpenOrCreate))
            {
                JsonSerializer.SerializeAsync<BotData>(fs, data);
                Console.WriteLine("Data has been saved to file");
            }
        }

        public static List<RoleGame> GetAllGames()
        {
            return BotData.GetAllData().Games;
        }
    }
}
