using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration.Json;
using Discord.Commands;
using Discord;
using KiberHyron.Log;

namespace KiberHyron
{
    public class Program
    {
        private DiscordSocketClient _client;

        // Program entry point
        public static Task Main(string[] args) => new Program().MainAsync();


        public async Task MainAsync()
        {
            var config = new ConfigurationBuilder()
            // this will be used more later on
            .SetBasePath(AppContext.BaseDirectory)
            // I chose using YML files for my config data as I am familiar with them
            .AddJsonFile("config.json")
            .Build();

            using IHost host = Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
            services
            // Add the configuration to the registered services
            .AddSingleton(config)
            // Add the DiscordSocketClient, along with specifying the GatewayIntents and user caching
            .AddSingleton(x => new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.All,
                LogGatewayIntentWarnings = true,
                AlwaysDownloadUsers = true,
                LogLevel = LogSeverity.Debug
            }))
            // Adding console logging
            .AddTransient<ConsoleLogger>()
            // Used for slash commands and their registration with Discord
            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
            // Required to subscribe to the various client events used in conjunction with Interactions
            .AddSingleton<InteractionHandler>()
            // Adding the prefix Command Service
            .AddSingleton(x => new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Debug,
                DefaultRunMode = Discord.Commands.RunMode.Async
            })))
            .Build();

            await RunAsync(host);
        }

        public async Task RunAsync(IHost host)
        {
            using IServiceScope serviceScope = host.Services.CreateScope();
            IServiceProvider provider = serviceScope.ServiceProvider;

            var commands = provider.GetRequiredService<InteractionService>();
            _client = provider.GetRequiredService<DiscordSocketClient>();
            var config = provider.GetRequiredService<IConfigurationRoot>();

            await provider.GetRequiredService<InteractionHandler>().InitializeAsync();

            // Subscribe to client log events
            _client.Log += _ => provider.GetRequiredService<ConsoleLogger>().Log(_);
            // Subscribe to slash command log events
            commands.Log += _ => provider.GetRequiredService<ConsoleLogger>().Log(_);

            _client.Ready += async () =>
            {
                await commands.RegisterCommandsGloballyAsync(true);
            };


            await _client.LoginAsync(Discord.TokenType.Bot, config["token"]);
            await _client.StartAsync();

            await Task.Delay(-1);
        }
    }
}