﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using ELOBOT.Handlers;

namespace ELOBOT.Discord.Preconditions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CheckRegistered : PreconditionAttribute
    {
        /// <summary>
        ///     This will check wether or not a user has permissions to use a command/module
        /// </summary>
        /// <param name="context"></param>
        /// <param name="command"></param>
        /// <param name="services"></param>
        /// <returns></returns>
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var EloContext = new Context.Context(context.Client, context.Message, services);
            return Task.FromResult(EloContext.Elo.User != null ? PreconditionResult.FromSuccess() : PreconditionResult.FromError("User is Not Registered!"));
        }
    }
}
