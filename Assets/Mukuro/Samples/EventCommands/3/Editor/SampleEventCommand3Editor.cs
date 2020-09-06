using System.Collections;
using System.Collections.Generic;
using Mukuro.Editors;
using Mukuro.Samples;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mukuro.Samples
{
    [CustomEventCommandEditor(typeof(SampleEventCommand3))]
    public class SampleEventCommand3Editor : EventCommandEditor
    {
        public SampleEventCommand3Editor(CommandItem commandItem, VisualElement customDetailRoot) : base(commandItem, customDetailRoot)
        {
        }

        public override string GetLabelText()
        {
            return "サンプルコマンド3:";
        }

        public override void OnCreated()
        {
            BuildNestedCommandList();
        }

        public override void OnUpdate()
        {
            //
        }
        
        private void BuildNestedCommandList()
        {
            var commandProp = CommandItem.CommandProperty;
            var nestedCommandProp = commandProp.GetChildProperty("commandList.commands");
            CustomDetailRoot.Clear();
            var commandListView = new CommandListView(CommandItem.ParentList,nestedCommandProp);
            
            CustomDetailRoot.Add(commandListView);
            
        }
     
    }
}