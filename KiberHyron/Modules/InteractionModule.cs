using Discord;
using Discord.Interactions;
using KiberHyron.Log;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiberHyron
{
    public class InteractionModule : InteractionModuleBase<SocketInteractionContext>
    {
        public InteractionService Commands { get; set; }
        private static Logger _logger;

        public InteractionModule(ConsoleLogger logger)
        {
            _logger = logger;
        }

        [SlashCommand("создать_игру", "Создаёт новую игру и позволяет настроить её параметры")]
        public async Task Create()
        {
            // New LogMessage created to pass desired info to the console using the existing Discord.Net LogMessage parameters
            await _logger.Log(new LogMessage(LogSeverity.Info, "InteractionModule : Create", $"User: {Context.User.Username}, Command: создать_игру", null));
            // Respond to the user
            await RespondAsync("Пока не умею :(");
        }

        
    }
}
