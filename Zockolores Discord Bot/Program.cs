using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
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
        private ConfigJson Cfgjson { get; set; }
        private CommandsNextModule Commands { get; set; }

        private static void Main(string[] args)
        {
            var prog = new Program();
            prog.RunBotAsync().GetAwaiter().GetResult();
        }

        private async Task RunBotAsync()
        {
            var sr = new StreamReader(File.OpenRead("config.json"));
            var json = await sr.ReadToEndAsync();
            Cfgjson = JsonConvert.DeserializeObject<ConfigJson>(json);

            Bot = new DiscordClient(new DiscordConfiguration
            {
                Token = Cfgjson.Token,
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

            var ccfg = new CommandsNextConfiguration
            {
                StringPrefix = Cfgjson.CommandPrefix,
                EnableMentionPrefix = true
            };
            Commands = Bot.UseCommandsNext(ccfg);

            Commands.CommandExecuted += Commands_CommandExecuted;
            Commands.CommandErrored += Commands_CommandErrored;

            Commands.RegisterCommands<Commands>();

            await Bot.ConnectAsync();
            await Task.Delay(-1);
        }

        private async Task Commands_CommandErrored(CommandErrorEventArgs e)
        {
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Error, "Zokolores Bot", $"{e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"}", DateTime.Now);

            if (e.Exception is ChecksFailedException ex)
            {
                var emoji = DiscordEmoji.FromName(e.Context.Client, ":no_entry:");
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Access denied",
                    Description = $"{emoji} You do not have the permissions required to execute this command.",
                    Color = new DiscordColor(0xFF0000)
                };
                await e.Context.RespondAsync("", embed: embed);
            }
        }

        private Task Commands_CommandExecuted(CommandExecutionEventArgs e)
        {
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Info, "Zockolores Bot", $"{e.Context.User.Username} successfully executed '{e.Command.QualifiedName}'", DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Bot_ClientError(ClientErrorEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Error, "Zockolores Bot", $"Exception occured: {e.Exception.GetType()}: {e.Exception.Message}", DateTime.Now);
            return Task.CompletedTask;
        }
    }
}
