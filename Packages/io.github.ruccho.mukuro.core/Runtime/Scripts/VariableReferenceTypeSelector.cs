using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mukuro
{
    [Serializable]
    public class VariableReferenceTypeSelector
    {
        [SerializeReference] private VariableReference variable = default;
        public VariableReference Variable => variable;
    }
}