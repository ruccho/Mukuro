using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mukuro.Editors
{
    [CustomEventCommandEditor(typeof(SkipContainerCommand))]
    public class SkipContainerCommandEditor : EventCommandEditor
    {
        public SkipContainerCommandEditor(CommandItem commandItem, VisualElement customDetailRoot) : base(commandItem, customDetailRoot)
        {
        }
        

        public override void OnCreated()
        {
            CustomDetailRoot.Clear();
            
            var commandProp = CommandItem.CommandProperty;
            
            var containerProp = commandProp.GetChildProperty("container.commands");
            var containerListView = new CommandListView(CommandItem.ParentList,containerProp);
            CustomDetailRoot.Add(containerListView);
        }
    }
}