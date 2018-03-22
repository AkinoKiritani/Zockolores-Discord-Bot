using DSharpPlus;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Zockolores
{
    internal class Program
    {
        private DiscordClient Bot { get; set; }
        private ConfigJson _cfgjson;

        private static void Main(string[] args)
        {
            var prog = new Program();
            prog.RunBotAsync().GetAwaiter().GetResult();
        }

        private async Task RunBotAsync()
        {
            var sr = new StreamReader(File.OpenRead("config.json"));
            var json = await sr.ReadToEndAsync();
            _cfgjson = JsonConvert.DeserializeObject<ConfigJson>(json);

            Bot = new DiscordClient(new DiscordConfiguration
            {
                Token = _cfgjson.Token,
                TokenType = TokenType.Bot,

                AutoReconnect = true,
                #if DEBUG
                LogLevel = LogLevel.Debug,
                #else
                LogLevel = LogLevel.Info,
                #endif
                UseInternalLogHandler = true
            });

            Bot.ClientErrored += Bot_ClientError;
            Bot.MessageCreated += Bot_MessageCreated;

            await Bot.ConnectAsync();
            await Task.Delay(-1);
        }

        private Task Bot_MessageCreated(MessageCreateEventArgs e)
        {
            if (e.Message.Content.StartsWith(_cfgjson.CommandPrefix + "ping"))
            {
                e.Message.RespondAsync("pong!");
            }
            return Task.CompletedTask;
        }

        private Task Bot_ClientError(ClientErrorEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Error, "Zockolores Bot", $"Exception occured: {e.Exception.GetType()}: {e.Exception.Message}", DateTime.Now);
            return Task.CompletedTask;
        }
    }
}
