using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace Zockolores
{
    public class Commands
    {
        [Command("ping")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            var emoji = DiscordEmoji.FromName(ctx.Client, ":ping_pong:");
            await ctx.RespondAsync($"{emoji} Pong! Ping: {ctx.Client.Ping}ms");
        }
        [Command("marco")] 
        public async Task Marco(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            await ctx.RespondAsync("Polo !");
        }
        [Command("zklmoments")]
        public async Task ZklMoments(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            var ran = new Random().Next(2);
            string response;
            if (ran == 0)
            {
                response = "Die Jagd nach Karl dem Käfer - oder doch Fred ?\nhttps://clips.twitch.tv/ShortSuspiciousChoughDBstyle";
            }
            else
            {
                var emoji = DiscordEmoji.FromName(ctx.Client, ":gabriel:");
                response = $"Das bin ich! Garbiel Knight {emoji}";
            }
            
            await ctx.RespondAsync(response);
        }
    }
}
