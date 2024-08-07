﻿using Discord;
using Discord.Interactions;
using Discord.WebSocket;
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
            // Respond to the user
            await RespondAsync("Список всех игр");
            await ShowGames(Context.Channel, game => true);
            // New LogMessage created to pass desired info to the console using the existing Discord.Net LogMessage parameters
            await _logger.Log(new LogMessage(LogSeverity.Info, "InteractionModule : ShowAllGames", $"User: {Context.User.Username}, Command: показать_все_игры", null));
        }
        #endregion
        #region interactions
        [ModalInteraction("newGameModal")]
        public async Task HandleNewGameModal(GameModal modal)
        {
            if (!uint.TryParse(modal.Color, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var color) && !string.IsNullOrEmpty(modal.Color))
            {
                await RespondAsync("Поле \"Цвет\" содержит значение в неправильном формате. Попробуйте ещё раз", ephemeral: true);
                return;
            }
            if (!(Uri.TryCreate(modal.Link, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)))
            {
                await RespondAsync("Поле \"Ссылка\" содержит значение в неправильном формате. Попробуйте ещё раз", ephemeral: true);
                return;
            }
            GamesData gamesData = GamesData.GetAllData<GamesData>();
            if (gamesData.Games.Count(game => game.Link == modal.Link) > 0)
            {
                await RespondAsync("Эта игра уже зарегистрирована: ");
                await ShowGames(Context.Channel, game => game.Link == modal.Link);
                return;
            }
            if (gamesData.Games.Count(game => game.Name == modal.Name) > 0)
            {
                await RespondAsync("Игра с таким названием уже есть. Выберите другое.", ephemeral: true);
                return;
            }
            if (Context.Guild.Roles.Select(r => r.Name).Contains($"НРИ: {modal.Name}"))
            {
                await RespondAsync("На сервере уже есть роль с таким названием. Выберите другое.", ephemeral: true);
                return;
            }
            Random rand = new Random();
            if (string.IsNullOrEmpty(modal.Color)) color = new Color(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255)).RawValue;
            var role = await Context.Guild.CreateRoleAsync($"НРИ: {modal.Name}", color: color);
            await (Context.User as IGuildUser).AddRoleAsync(role);
            gamesData.Games.Add(new RoleGame()
            {
                Color = color,
                Description = modal.Description,
                Link = modal.Link,
                Master = Context.User.Id,
                Name = modal.Name,
                Role = role.Id
                
            });
            try
            {
                GamesData.WriteNewData<GamesData>(gamesData);
                await RespondAsync($"Игра создана:");
                await ShowGames(Context.Channel, game => game.Name == modal.Name);
                var message = await Context.Channel.SendMessageAsync("Поставьте 👍, чтобы присоединиться к игре");
                MessagesData messagesdata = MessagesData.GetAllData<MessagesData>();
                messagesdata.messages.Add(new ReactableMessage() { Message = message.Id, Game = modal.Name });
                MessagesData.WriteNewData<MessagesData>(messagesdata);
                await message.AddReactionAsync(new Emoji("👍"));
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.ToString());
            }
        }
        #endregion

        private async Task ShowGames(ISocketMessageChannel channel,  Func<RoleGame, bool> filter)
        {
            List<RoleGame> games = GamesData.GetAllData<GamesData>().Games.Where(filter).ToList();
            if (games.Count == 0)
            {
                await channel.SendMessageAsync("Не найдено ни одной игры");
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
            await channel.SendMessageAsync(embeds: embeds);
        }
    }
}
