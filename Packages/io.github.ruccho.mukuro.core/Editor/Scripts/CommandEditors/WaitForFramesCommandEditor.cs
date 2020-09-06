﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mukuro.Editors
{
    [CustomEventCommandEditor(typeof(WaitForFramesCommand))]
    public class WaitForFramesCommandEditor : EventCommandEditor
    {
        private static Texture2D Icon { get; set; }

        private static void EnsureIconLoaded()
        {
            if (Icon == null)
                Icon = AssetDatabase.LoadAssetAtPath<Texture2D>(
                    "Packages/io.github.ruccho.mukuro.core/Editor/Scripts/CommandEditors/Icons/Wait.png");
        }

        public override Texture2D GetIcon()
        {
            EnsureIconLoaded();
            return Icon;
        }
        public WaitForFramesCommandEditor(CommandItem commandItem, VisualElement customDetailRoot) : base(commandItem, customDetailRoot)
        {
        }

        public override string GetLabelText()
        {
            return $"{CommandItem.CommandProperty.GetProperty().FindPropertyRelative("frames").intValue}フレームウェイト";
        }

        public override Color? GetLabelColor()
        {
            if (ColorUtility.TryParseHtmlString("#a5bbe2", out Color c))
            {
                return c;
            }

            return null;
        }
    }
}