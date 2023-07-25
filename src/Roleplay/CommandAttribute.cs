using System;

namespace Roleplay
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        public readonly string Command;
        public readonly string HelpText;

        public CommandAttribute(string command, string helpText = "")
        {
            Command = command;
            HelpText = helpText;
        }

        public string[] Aliases { get; set; }
        public bool GreedyArg { get; set; }   
    }
}