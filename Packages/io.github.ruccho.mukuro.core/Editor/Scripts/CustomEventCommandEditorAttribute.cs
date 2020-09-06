using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mukuro.Editors
{
    public class CustomEventCommandEditorAttribute : Attribute
    {
        public CustomEventCommandEditorAttribute(Type type)
        {
            this.CommandType = type;
        }

        public Type CommandType { get; }
        
    }
}