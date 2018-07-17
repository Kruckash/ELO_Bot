namespace ELO.Modules.Admin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using ELO.Discord.Context;
    using ELO.Discord.Extensions;
    using ELO.Discord.Preconditions;
    using ELO.Handlers;
    using ELO.Models;
    using ELO.Models.Parser;

    using global::Discord;
    using global::Discord.Commands;
    using global::Discord.WebSocket;

    [CustomPermissions(true)]
    [Summary("Direct user stats modifications")]
    public class Stats : Base
    {
        [Command("ModifyPoints")]
        [Summary("Add or subtract points from a user")]
        public Task ModifyPointsAsync(SocketGuildUser user, int pointsToAddOrSubtract)
        {
            return ModifyAsync(new List<SocketGuildUser> { user }, ScoreType.point, pointsToAddOrSubtract);
        }

        [Command("ModifyPoints")]
        [Summary("Add or subtract points from the given user(s)")]
        public Task ModifyPointsAsync(int pointsToAddOrSubtract, params SocketGuildUser[] users)
        {
            return ModifyAsync(users.ToList(), ScoreType.point, pointsToAddOrSubtract);
        }

        [Command("SetPoints")]
        [Summary("Set the points of a user")]
        public Task SetPointsAsync(SocketGuildUser user, int points)
        {
            return SetAsync(new List<SocketGuildUser> { user }, ScoreType.point, points);
        }

        [Command("SetPoints")]
        [Summary("Set the points of the given user(s)")]
        public Task SetPointsAsync(int points, params SocketGuildUser[] users)
        {
            return SetAsync(users.ToList(), ScoreType.point, points);
        }

        [Command("ModifyKills")]
        [Summary("Add or subtract kills from a user")]
        public Task ModifyKillsAsync(SocketGuildUser user, int killsToAddOrSubtract)
        {
            return ModifyAsync(new List<SocketGuildUser> { user }, ScoreType.kill, killsToAddOrSubtract);
        }

        [Command("ModifyKills")]
        [Summary("Add or subtract kills from the given user(s)")]
        public Task ModifyKillsAsync(int killsToAddOrSubtract, params SocketGuildUser[] users)
        {
            return ModifyAsync(users.ToList(), ScoreType.kill, killsToAddOrSubtract);
        }

        [Command("SetKills")]
        [Summary("Set the kills of a user")]
        public Task SetKillsAsync(SocketGuildUser user, int kills)
        {
            return SetAsync(new List<SocketGuildUser> { user }, ScoreType.kill, kills);
        }

        [Command("SetKills")]
        [Summary("Set the kills of the given user(s)")]
        public Task SetKillsAsync(int kills, params SocketGuildUser[] users)
        {
            return SetAsync(users.ToList(), ScoreType.kill, kills);
        }

        [Command("ModifyDeaths")]
        [Summary("Add or subtract Deaths from a user")]
        public Task ModifyDeathsAsync(SocketGuildUser user, int deathsToAddOrSubtract)
        {
            return ModifyAsync(new List<SocketGuildUser> { user }, ScoreType.death, deathsToAddOrSubtract);
        }

        [Command("ModifyDeaths")]
        [Summary("Add or subtract Deaths from the given user(s)")]
        public Task ModifyDeathsAsync(int deathsToAddOrSubtract, params SocketGuildUser[] users)
        {
            return ModifyAsync(users.ToList(), ScoreType.death, deathsToAddOrSubtract);
        }

        [Command("SetDeaths")]
        [Summary("Set the Deaths of a user")]
        public Task SetDeathsAsync(SocketGuildUser user, int deaths)
        {
            return SetAsync(new List<SocketGuildUser> { user }, ScoreType.death, deaths);
        }

        [Command("SetDeaths")]
        [Summary("Set the Deaths of the given user(s)")]
        public Task SetDeathsAsync(int deaths, params SocketGuildUser[] users)
        {
            return SetAsync(users.ToList(), ScoreType.death, deaths);
        }

        [Command("ModifyWins")]
        [Summary("Add or subtract Wins from a user")]
        public Task ModifyWinsAsync(SocketGuildUser user, int winsToAddOrSubtract)
        {
            return ModifyAsync(new List<SocketGuildUser> { user }, ScoreType.win, winsToAddOrSubtract);
        }

        [Command("ModifyWins")]
        [Summary("Add or subtract Wins from the given user(s)")]
        public Task ModifyWinsAsync(int winsToAddOrSubtract, params SocketGuildUser[] users)
        {
            return ModifyAsync(users.ToList(), ScoreType.win, winsToAddOrSubtract);
        }

        [Command("SetWins")]
        [Summary("Set the Wins of a user")]
        public Task SetWinsAsync(SocketGuildUser user, int wins)
        {
            return SetAsync(new List<SocketGuildUser> { user }, ScoreType.win, wins);
        }

        [Command("SetWins")]
        [Summary("Set the Wins of the given user(s)")]
        public Task SetWinsAsync(int wins, params SocketGuildUser[] users)
        {
            return SetAsync(users.ToList(), ScoreType.win, wins);
        }

        [Command("ModifyLosses")]
        [Summary("Add or subtract Losses from a user")]
        public Task ModifyLossesAsync(SocketGuildUser user, int lossesToAddOrSubtract)
        {
            return ModifyAsync(new List<SocketGuildUser> { user }, ScoreType.loss, lossesToAddOrSubtract);
        }

        [Command("ModifyLosses")]
        [Summary("Add or subtract Losses from the given user(s)")]
        public Task ModifyLossesAsync(int lossesToAddOrSubtract, params SocketGuildUser[] users)
        {
            return ModifyAsync(users.ToList(), ScoreType.loss, lossesToAddOrSubtract);
        }

        [Command("SetLosses")]
        [Summary("Set the Losses of a user")]
        public Task SetLossesAsync(SocketGuildUser user, int losses)
        {
            return SetAsync(new List<SocketGuildUser> { user }, ScoreType.loss, losses);
        }

        [Command("SetLosses")]
        [Summary("Set the Losses of the given user(s)")]
        public Task SetLossesAsync(int losses, params SocketGuildUser[] users)
        {
            return SetAsync(users.ToList(), ScoreType.loss, losses);
        }

        public Task SetAsync(List<SocketGuildUser> users, ScoreType type, int modifier)
        {
            var sb = new StringBuilder();
            foreach (var user in users)
            {
                var eUser = Context.Server.Users.FirstOrDefault(x => x.UserID == user.Id);
                if (eUser == null)
                {
                    sb.AppendLine("User is not registered");
                    continue;
                }

                int finalValue;
                switch (type)
                {
                    case ScoreType.win:
                        eUser.Stats.Wins = modifier;
                        finalValue = eUser.Stats.Wins;
                        break;
                    case ScoreType.loss:
                        eUser.Stats.Losses = modifier;
                        finalValue = eUser.Stats.Losses;
                        break;
                    case ScoreType.draw:
                        eUser.Stats.Draws = modifier;
                        finalValue = eUser.Stats.Draws;
                        break;
                    case ScoreType.kill:
                        eUser.Stats.Kills = modifier;
                        finalValue = eUser.Stats.Kills;
                        break;
                    case ScoreType.death:
                        eUser.Stats.Deaths = modifier;
                        finalValue = eUser.Stats.Deaths;
                        break;
                    case ScoreType.point:
                        eUser.Stats.Points = modifier;
                        finalValue = eUser.Stats.Points;
                        var nick = Task.Run(() => UserManagement.UserRenameAsync(Context, eUser));
                        var role = Task.Run(() => UserManagement.UpdateUserRanksAsync(Context, eUser));
                        break;
                    default:
                        throw new InvalidOperationException("Unable to modify stats with provided type");
                }

                sb.AppendLine($"{user.Mention} {type}'s set to: {finalValue}");
            }
            Context.Server.Save();
            return SimpleEmbedAsync(sb.ToString());
        }

        public Task ModifyAsync(List<SocketGuildUser> users, ScoreType type, int modifier)
        {
            var sb = new StringBuilder();
            foreach (var user in users)
            {
                var eUser = Context.Server.Users.FirstOrDefault(x => x.UserID == user.Id);
                if (eUser == null)
                {
                    sb.AppendLine("User is not registered");
                    continue;
                }

                int finalValue;
                switch (type)
                {
                    case ScoreType.win:
                        eUser.Stats.Wins += modifier;
                        finalValue = eUser.Stats.Wins;
                        break;
                    case ScoreType.loss:
                        eUser.Stats.Losses += modifier;
                        finalValue = eUser.Stats.Losses;
                        break;
                    case ScoreType.draw:
                        eUser.Stats.Draws += modifier;
                        finalValue = eUser.Stats.Draws;
                        break;
                    case ScoreType.kill:
                        eUser.Stats.Kills += modifier;
                        finalValue = eUser.Stats.Kills;
                        break;
                    case ScoreType.death:
                        eUser.Stats.Deaths += modifier;
                        finalValue = eUser.Stats.Deaths;
                        break;
                    case ScoreType.point:
                        eUser.Stats.Points += modifier;
                        finalValue = eUser.Stats.Points;
                        var nick = Task.Run(() => UserManagement.UserRenameAsync(Context, eUser));
                        var role = Task.Run(() => UserManagement.UpdateUserRanksAsync(Context, eUser));
                        break;
                    default:
                        throw new InvalidOperationException("Unable to modify stats with provided type");
                }

                sb.AppendLine($"{user.Mention} {type}'s modified: {finalValue}");
            }

            Context.Server.Save();
            return SimpleEmbedAsync(sb.ToString());
        }

        public enum ScoreType
        {
            win,
            loss,
            draw,
            kill,
            death,
            point
        }
   
        // Could literally just use the leaderboard to get the users...
        /// <summary>
        ///     server owner? Admin only command, shows all registered users.
        /// </summary>
        /// <returns></returns>
        [Command("ListUsers")]
        [Alias("UserList")]
        [Summary("Returns all registered users")]
        public Task ListUsersAsync()
        {
            var userList = Context.Server.Users.OrderByDescending(u => u.Stats.Points).Select(
                u =>
                {
                    var name = Context.Guild.GetUser(u.UserID)?.Mention ?? $"[{u.UserID}]";
                    var userPoints= $"{u.Stats.Points.ToString().PadRight(10)}\u200B";
                    return $"`{userPoints}` - {name}";
                }).ToList();

            return SimpleBlueEmbedAsync($"`Points  User      \u200B `\n{string.Join("\n", userList)}");
        }

        /* Meeeh... old ScoreboardReset code
        /// <summary>
        ///     server owner only command, resets all user scores on the scoreboard.
        /// </summary>
        /// <returns></returns>
        [Command("ScoreboardReset", RunMode = RunMode.Async)]
        [Summary("ScoreboardReset")]
        [Remarks("Reset Points, Wins and Losses for all users in the server")]
        //public async Task Reset(Context context, GuildModel server)
        public async Task ResetScore() //Context context
        {
            var server = Servers.ServerList.First(x => x.ServerId == Context.Guild.Id);
            
            await ReplyAsync("Working...");

            var reset = server.UserList.ToList();
            foreach (var user in reset)
            {
                if (user.Points != server.registerpoints || user.Wins != 0 || user.Losses != 0)
                {
                    user.Points = server.registerpoints;
                    user.Wins = 0;
                    user.Losses = 0;
                }
            }
            server.UserList = reset;

            await ReplyAsync("Leaderboard Reset Complete!\n" +
                             "NOTE: Names and ranks will be reset over the next few minutes.\n" +
                             $"EST time = {(double)reset.Count * 6 / 60 / 60} hours");
            var i = 0;
            var completion = await ReplyAsync($"{i}/{reset.Count} completed");
            var botposition = ((SocketGuild)Context.Guild).Users.First(x => x.Id == Context.Client.CurrentUser.Id)
                .Roles
                .OrderByDescending(x => x.Position).First().Position;
            foreach (var user in reset)
            {
                try
                {
                    i++;
                    var us = Context.Guild.GetUser(user.UserId);
                      //old: await Context.Guild.GetUserAsync(user.UserId);
                    var nick = us.Nickname ?? "";
                    if (!nick.Contains(Context.Server.Settings.Registration.NameFormat) &&
                    //old if (!nick.Contains(Globals.GetNamePrefix(server, user.UserId, true)) &&
                        Context.Guild.OwnerId != us.Id &&
                        ((SocketGuildUser)us).Roles.OrderByDescending(x => x.Position).First().Position < botposition)
                    {
                        try
                        {
                            await us.ModifyAsync(x =>
                            {
                                x.Nickname = Context.Server.Settings.Registration.NameFormat + $" {user.Username}";
                                //old: x.Nickname = Globals.GetNamePrefix(server, user.UserId) + $" {user.Username}";
                            });
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }

                    }

                    await Task.Delay(2500);

                    await us.RemoveRolesAsync(server.Ranks.Select(x => Context.Guild.GetRole(x.RoleId)));
                    await Task.Delay(1000);
                    if (server.Ranks.Count(x => x.Points < user.Points) > 0)
                    {
                        var rank = server.Ranks.Where(x => x.Points < user.Points).OrderByDescending(x => x.Points).First();
                        await us.AddRoleAsync(Context.Guild.GetRole(rank.RoleId));
                    }
                    var i1 = i;
                    await completion.ModifyAsync(x => x.Content = $"{i1}/{reset.Count} completed");

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            await ReplyAsync("Reset complete.");
        }
        */


    }
}
