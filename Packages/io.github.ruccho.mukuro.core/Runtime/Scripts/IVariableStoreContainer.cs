using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mukuro
{
    public interface IVariableStoreContainer
    {
        IVariableStore TemporaryVariables { get; }
        IVariableStore EventVariables { get; }
    }
    
    
    public class VariableStoreContainer : IVariableStoreContainer
    {
        public VariableStoreContainer(IVariableStore temporaryVariables, IVariableStore eventVariables)
        {
            TemporaryVariables = temporaryVariables;
            EventVariables = eventVariables;
        }

        public IVariableStore TemporaryVariables { get; }
        public IVariableStore EventVariables { get; }
        
    }
}