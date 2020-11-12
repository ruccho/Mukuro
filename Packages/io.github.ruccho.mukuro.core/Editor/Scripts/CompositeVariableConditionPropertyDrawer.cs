using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mukuro.Editors
{
    [CustomPropertyDrawer(typeof(CompositeVariableCondition))]
    public class CompositeVariableConditionPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();

            var listRoot = new VisualElement();
            
            var countField = new PropertyField(property.FindPropertyRelative("conditionCount"));
            countField.RegisterCallback(new EventCallback<ChangeEvent<int>>(evt =>
                {
                    BuildConditionList(listRoot, property);
                }));
            
            BuildConditionList(listRoot, property);

            root.Add(countField);
            root.Add(listRoot);

            return root;

        }

        private void BuildConditionList(VisualElement listRoot, SerializedProperty prop)
        {
            //Debug.Log("Build");
            listRoot.Clear();

            var countProp = prop.FindPropertyRelative("conditionCount");

            for (int i = 0; i < countProp.intValue; i++)
            {
                var item = prop.FindPropertyRelative($"condition{i}");
                listRoot.Add(new PropertyField(item));
            }
            
            listRoot.Bind(prop.serializedObject);
        }
    }
}