using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mukuro.Editors
{
    public class CommandEditorDomain
    {
        public CommandListView RootCommandListView { get; }
        public CommandItem SelectedItem { get; private set; }
        
        private Action<CommandItem> OnItemSelected { get; }
        
        internal CommandEditorDomain(SerializedProperty rootCommandArray, Action<CommandItem> onItemSelected)
        {
            RootCommandListView = new CommandListView(this, rootCommandArray);
            OnItemSelected = onItemSelected;
        }

        public void SelectItem(CommandItem item)
        {
            if (SelectedItem == item) return;

            SelectedItem?.OnDeselected();

            SelectedItem = item;

            item?.OnSelected();
            
            OnItemSelected?.Invoke(item);
        }


        public CommandItem AddCommandAtSelected(EventCommand command)
        {
            if (SelectedItem != null && SelectedItem.parent != null)
            {
                int index = SelectedItem.GetIndex();
                if (index == -1)
                    throw new InvalidOperationException("Selected Item is not a member of the CommandListView. ");

                return SelectedItem.ParentList.AddCommandAt(index + 1, command);
            }
            else
            {
                return RootCommandListView.AddCommandAt(-1, command);
            }
        }
        

        public void RemoveCommandAtSelected()
        {
            if (SelectedItem != null && SelectedItem.parent != null)
            {
                SelectedItem.ParentList.RemoveCommandAt(SelectedItem.GetIndex());
                SelectItem(null);
            }
        }
        
    }
    


    
}