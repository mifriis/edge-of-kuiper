﻿using System;

namespace Kuiper.Systems
{
    public class CommandAttribute : Attribute
    {
        public CommandAttribute(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace", nameof(name));
            }

            Name = name;
        }

        public string Name { get; }
    }
}