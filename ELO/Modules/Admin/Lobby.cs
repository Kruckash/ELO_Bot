﻿namespace ELO.Modules.Admin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using ELO.Discord.Context;
    using ELO.Discord.Preconditions;
    using ELO.Models;

    using global::Discord;
    using global::Discord.Addons.Interactive;
    using global::Discord.Commands;

    [CustomPermissions(true)]
    public class Lobby : Base
    {
        [Command("CreateLobby")]
        public Task CreateLobbyAsync()
        {
            if (Context.Elo.Lobby != null)
            {
                throw new Exception("Channel is already a lobby");
            }

            var lobby = new GuildModel.Lobby
                            {
                                ChannelID = Context.Channel.Id
                            };

            return InlineReactionReplyAsync(
                new ReactionCallbackData(
                        "",
                        new EmbedBuilder
                            {
                                Description =
                                    "Please react with the amount of players you would like **PER TEAM**"
                            }.Build(),
                        timeout: TimeSpan.FromMinutes(2), timeoutCallback: c => SimpleEmbedAsync("Command Timed Out"))
                    .WithCallback(new Emoji("1\u20e3"), (c, r) => SortModeAsync(lobby, 1))
                    .WithCallback(new Emoji("2\u20e3"), (c, r) => SortModeAsync(lobby, 2))
                    .WithCallback(new Emoji("3\u20e3"), (c, r) => SortModeAsync(lobby, 3))
                    .WithCallback(new Emoji("4\u20e3"), (c, r) => SortModeAsync(lobby, 4))
                    .WithCallback(new Emoji("5\u20e3"), (c, r) => SortModeAsync(lobby, 5))
                    .WithCallback(new Emoji("6\u20e3"), (c, r) => SortModeAsync(lobby, 6))
                    .WithCallback(new Emoji("7\u20e3"), (c, r) => SortModeAsync(lobby, 7))
                    .WithCallback(new Emoji("8\u20e3"), (c, r) => SortModeAsync(lobby, 8))
                    .WithCallback(new Emoji("9\u20e3"), (c, r) => SortModeAsync(lobby, 9)));
        }

        public Task SortModeAsync(GuildModel.Lobby lobby, int teamPlayers)
        {
            lobby.UserLimit = teamPlayers * 2;
            return InlineReactionReplyAsync(
                new ReactionCallbackData(
                        "",
                        new EmbedBuilder
                            {
                                Description =
                                    "Please react with the team sorting mode you would like for this lobby:\n" +
                                    "1\u20e3 `CompleteRandom` __**Completely Random Team sorting**__\n" +
                                    "All teams are chosen completely randomly\n\n" +
                                    "2\u20e3 `Captains` __**Captains Mode**__\n" +
                                    "Two team captains are chosen, they each take turns picking players until teams are both filled.\n\n" +
                                    "3\u20e3 `SortByScore` __**Score Balance Mode**__\n" +
                                    "Players will be automatically selected and teams will be balanced based on player scores"
                            }.Build(),
                        timeout: TimeSpan.FromMinutes(2), timeoutCallback: c => SimpleEmbedAsync("Command Timed Out"))
                    .WithCallback(new Emoji("1\u20e3"), (c, r) => CompleteLobbyCreationAsync(lobby, GuildModel.Lobby._PickMode.CompleteRandom))
                    .WithCallback(new Emoji("2\u20e3"), (c, r) => CompleteLobbyCreationAsync(lobby, GuildModel.Lobby._PickMode.Captains))
                    .WithCallback(new Emoji("3\u20e3"), (c, r) => CompleteLobbyCreationAsync(lobby, GuildModel.Lobby._PickMode.SortByScore)));
        }

        public Task CompleteLobbyCreationAsync(GuildModel.Lobby lobby, GuildModel.Lobby._PickMode pickMode)
        {
            lobby.PickMode = pickMode;
            Context.Server.Lobbies.Add(lobby);
            Context.Server.Save();
            return SimpleEmbedAsync(
                "Success, Lobby has been created.\n" + $"`Size:` {lobby.UserLimit}\n"
                                                      + $"`Team Size:` {lobby.UserLimit / 2}\n"
                                                      + $"`Team Mode:` {(lobby.PickMode == GuildModel.Lobby._PickMode.Captains ? $"Captains => {lobby.CaptainSortMode}" : $"{lobby.PickMode}")}\n"
                                                      + $"`Host Selection Mode:` {lobby.HostSelectionMode}\n\n"
                                                      + $"To Set Description: `{Context.Prefix}LobbyDescription <description>`\n"
                                                      + $"For More info, type `{Context.Prefix}help Lobby`");
        }

        [CheckLobby]
        [Command("RemoveLobby")]
        public Task RemoveLobbyAsync()
        {
            Context.Server.Lobbies.Remove(Context.Elo.Lobby);
            Context.Server.Save();
            return SimpleEmbedAsync("Success, Lobby has been removed.");
        }

        [CheckLobby]
        [Command("ClearQueue")]
        public Task ClearQueueAsync()
        {
            Context.Elo.Lobby.Game = new GuildModel.Lobby.CurrentGame();
            Context.Server.Save();
            return SimpleEmbedAsync("Queue has been cleared");
        }

        [CheckLobby]
        [Command("LobbyDescription")]
        public Task SetDescriptionAsync([Remainder] string description)
        {
            if (description.Length > 200)
            {
                throw new Exception("Lobby description is limited to 200 characters or less.");
            }

            Context.Elo.Lobby.Description = description;
            Context.Server.Save();
            return SimpleEmbedAsync($"Success, Description is now:\n{description}");
        }

        [CheckLobby]
        [Command("LobbySortMode")]
        public Task LobbySortModeAsync(GuildModel.Lobby._PickMode sortMode)
        {
            Context.Elo.Lobby.PickMode = sortMode;
            Context.Server.Save();

            return SimpleEmbedAsync("Success, lobby team sort mode has been modified to:\n" +
                                    $"{sortMode.ToString()}");
        }

        [CheckLobby]
        [Command("LobbySortMode")]
        public Task LobbySortModeAsync()
        {
            return SimpleEmbedAsync($"Please use command `{Context.Prefix}LobbySortMode <mode>` with the selection mode you would like for this lobby:\n" +
                                    "`CompleteRandom` __**Completely Random Team sorting**__\n" +
                                    "All teams are chosen completely randomly\n\n" +
                                    "`Captains` __**Captains Mode**__\n" +
                                    "Two team captains are chosen, they each take turns picking players until teams are both filled.\n\n" +
                                    "`SortByScore` __**Score Balance Mode**__\n" +
                                    "Players will be automatically selected and teams will be balanced based on player scores");
        }

        [CheckLobby]
        [Command("CaptainSortMode")]
        public Task CapSortModeAsync(GuildModel.Lobby.CaptainSort sortMode)
        {
            Context.Elo.Lobby.CaptainSortMode = sortMode;
            Context.Server.Save();

            return SimpleEmbedAsync("Success, captain sort mode has been modified to:\n" +
                                    $"{sortMode.ToString()}");
        }

        [CheckLobby]
        [Command("CaptainSortMode")]
        public Task CapSortModeAsync()
        {
            return SimpleEmbedAsync($"Please use command `{Context.Prefix}CapSortMode <mode>` with the captain selection mode you would like for this lobby:\n" +
                                    "`MostWins` __**Choose Two Players with Highest Wins**__\n" +
                                    "Selects the two players with the highest amount of Wins\n\n" +
                                    "`MostPoints` __**Choose Two Players with Highest Points**__\n" +
                                    "Selects the two players with the highest amount of Points\n\n" +
                                    "`HighestWinLoss` __**Selects the two players with the highest Win/Loss Ratio**__\n" +
                                    "Selects the two players with the highest win/loss ratio\n\n" +
                                    "`Random` __**Random**__\n" +
                                    "Selects Randomly\n\n" +
                                    "`RandomTop4MostPoints` __**Selects Random from top 4 Most Points**__\n" +
                                    "Selects Randomly from the top 4 highest ranked players based on points\n\n" +
                                    "`RandomTop4MostWins` __**Selects Random from top 4 Most Wins**__\n" +
                                    "Selects Randomly from the top 4 highest ranked players based on wins\n\n" +
                                    "`RandomTop4HighestWinLoss` __**Selects Random from top 4 Highest Win/Loss Ratio**__\n" +
                                    "Selects Randomly from the top 4 highest ranked players based on win/loss ratio");
        }

        [CheckLobby]
        [Command("RandomMap")]
        public Task RandomMapAsync()
        {
            Context.Elo.Lobby.RandomMapAnnounce = !Context.Elo.Lobby.RandomMapAnnounce;
            Context.Server.Save();

            return SimpleEmbedAsync($"Random Map on game announcements: {Context.Elo.Lobby.RandomMapAnnounce}");
        }

        [CheckLobby]
        [Command("HostSelectionMode")]
        public Task HostModeAsync(GuildModel.Lobby.HostSelector mode)
        {
            Context.Elo.Lobby.HostSelectionMode = mode;
            Context.Server.Save();

            return SimpleEmbedAsync($"Host selection mode = {mode.ToString()}");
        }

        [CheckLobby]
        [Command("HostSelectionMode")]
        public Task HostModeAsync()
        {
            return SimpleEmbedAsync($"Please use command `{Context.Prefix}HostSelectionMode <mode>` with the host selection mode you would like for this lobby:\n" +
                                    "`MostWins` __**Selects Player with Most Wins**__\n" +
                                    "`MostPoints` __**Selects Player with Most Points**__\n" +
                                    "`HighestWinLoss` __**Selects the Player with the highest Win/Loss Ratio**__\n" +
                                    "`Random` __**Random**__\n");
        }

        [CheckLobby]
        [Command("AddMap")]
        public async Task AddMapAsync([Remainder] string mapName)
        {
            if (!Context.Elo.Lobby.Maps.Contains(mapName))
            {
                Context.Elo.Lobby.Maps.Add(mapName);
                Context.Server.Save();
                await SimpleEmbedAsync("Success, Lobby Map list is now:\n" +
                                       $"{string.Join("\n", Context.Elo.Lobby.Maps)}");
            }
            else
            {
                throw new Exception("Map has already been added to the lobby");
            }
        }

        [CheckLobby]
        [Command("DelMap")]
        public async Task DelMapAsync([Remainder] string mapName)
        {
            if (Context.Elo.Lobby.Maps.Contains(mapName))
            {
                Context.Elo.Lobby.Maps.Remove(mapName);
                Context.Server.Save();
                await SimpleEmbedAsync("Success, Lobby Map list is now:\n" +
                                       $"{string.Join("\n", Context.Elo.Lobby.Maps)}");
            }
            else
            {
                throw new Exception("Map is not in lobby");
            }
        }

        [CheckLobby]
        [Command("AddMaps")]
        public async Task AddMapsAsync([Remainder] string mapList)
        {
            var maps = mapList.Split(",");
            if (!Context.Elo.Lobby.Maps.Any(x => maps.Contains(x)))
            {
                Context.Elo.Lobby.Maps.AddRange(maps);
                Context.Server.Save();
                await SimpleEmbedAsync("Success, Lobby Map list is now:\n" +
                                       $"{string.Join("\n", Context.Elo.Lobby.Maps)}");
            }
            else
            {
                throw new Exception("One of the provided maps is already in the lobby");
            }
        }

        [CheckLobby]
        [Command("ClearMaps")]
        public Task ClearMapsAsync()
        {
            Context.Elo.Lobby.Maps = new List<string>();
            Context.Server.Save();
            return SimpleEmbedAsync("Map List for this lobby has been reset.");
        }
    }
}