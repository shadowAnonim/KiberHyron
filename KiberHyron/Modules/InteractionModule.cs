using Discord;
using Discord.Interactions;
using KiberHyron.Data;
using KiberHyron.Log;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        #region commands
        [SlashCommand("создать_игру", "Создаёт новую игру и позволяет настроить её параметры")]
        public async Task Create()
        {
            // New LogMessage created to pass desired info to the console using the existing Discord.Net LogMessage parameters
            await _logger.Log(new LogMessage(LogSeverity.Info, "InteractionModule : Create", $"User: {Context.User.Username}, Command: создать_игру", null));
            // Respond to the user
            await RespondWithModalAsync<GameModal>("newGameModal");
        }
        [SlashCommand("показать_все_игры", "Отображает список всех существующих игр")]
        public async Task ShowAllGames()
        {
            List<RoleGame> games = BotData.GetAllGames();
            if (games.Count == 0)
            {
                await RespondAsync("Пока что нет ни одной игры");
                return;
            }
            Embed[] embeds = new Embed[games.Count];
            for (int i = 0; i < games.Count; i++) 
            {
                var game = games[i];
                embeds[i] = new EmbedBuilder()
                    .WithColor(new Color(game.Color))
                    .WithDescription(game.Description)
                    .WithTitle(game.Name)
                    .WithUrl(game.Link)
                    .Build();
            }
            // New LogMessage created to pass desired info to the console using the existing Discord.Net LogMessage parameters
            await _logger.Log(new LogMessage(LogSeverity.Info, "InteractionModule : ShowAllGames", $"User: {Context.User.Username}, Command: показать_все_игры", null));
            // Respond to the user
            await RespondAsync(embeds: embeds);
        }
        #endregion
        #region interactions
        [ModalInteraction("newGameModal")]
        public async Task HandleNewGameModal(GameModal modal)
        {
            if (!uint.TryParse(modal.Color, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var color) && modal.Color != "")
            {
                await RespondAsync("Поле \"Цвет\" содержит значение в неправильном формате. Попробуйте ещё раз", ephemeral: true);
                return;
            }
            if (!(Uri.TryCreate(modal.Link, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)))
            {
                await RespondAsync("Поле \"Ссылка\" содержит значение в неправильном формате. Попробуйте ещё раз", ephemeral: true);
                return;
            }
            BotData data = BotData.GetAllData();
            Random rand = new Random();
            data.Games.Add(new RoleGame()
            {
                Color = modal.Color == "" ? new Color(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255)).RawValue : color,
                Description = modal.Description,
                Link = modal.Link,
                Master = Context.User.Id,
                Name = modal.Name
            });
            BotData.WriteNewData(data);
            await RespondAsync($"Создана игра: {modal.Name}");
        }
        #endregion
    }
}
