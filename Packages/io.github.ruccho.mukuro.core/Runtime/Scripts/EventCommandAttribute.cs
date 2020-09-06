using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Mukuro
{
    public class EventCommandAttribute : Attribute
    {
        public EventCommandAttribute(string category, string displayName)
        {
            Category = category;
            DisplayName = displayName;
        }

        public string Category { get; }
        public string DisplayName { get; }
        
    }
}