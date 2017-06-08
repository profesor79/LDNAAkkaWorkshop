// -----------------------------------------------------------------------
// <copyright file="MsgCommands.cs" company="Petabridge, LLC">
//      Copyright (C) 2017 - 2017 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------
namespace Petabridge.Cmd.QuickStart
{
    /// <summary>
    ///     Contains all of the custom commands used in the Petabridge.Cmd QuickStart
    /// </summary>
    public static class MsgCommands
    {
        public static readonly CommandDefinition StartSystem =
            new CommandDefinitionBuilder().WithName("ss")
                .WithDescription("Starts the process")
                
                .Build();


        public static readonly CommandPalette Palette = new CommandPalette("LDNA", new[] { StartSystem });
    }
}