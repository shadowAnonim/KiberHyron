using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiberHyron.Data
{
    public class RoleGame
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public uint Color { get; set; }
        public ulong Master { get; set; }
    }
}
