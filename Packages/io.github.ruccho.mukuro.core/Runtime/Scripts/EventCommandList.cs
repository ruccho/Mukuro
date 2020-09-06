using System;
using System.Collections;
using System.Collections.Generic;
using Mukuro;
using UnityEngine;

namespace Mukuro
{
    [Serializable]
    public class EventCommandList
    {
        [SerializeReference] private List<EventCommand> commands = default;

        public List<EventCommand> Commands => commands;
        
        
    }
}