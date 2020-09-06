using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mukuro.Dialog
{
    [EventCommand("Mukuro Dialog", "メッセージの表示")]
    public class DialogShowMessageCommand : EventCommand
    {
        [SerializeField] private SpeakerInfoAsset speaker = default;
        [SerializeField] private string face = default;
        [SerializeField] private string message = default;
        
        [SerializeField] private bool allowSpeedUp = true;
        [SerializeField] private bool allowSkipping = true;
        
        public override void Execute(EventExecutionContext context, CommandExecutionHandle handle)
        {
            var dialog = context.GetModule<DialogModule>();
            if (dialog == default)
            {
                context.RaiseError(new ScriptExecutionException("DialogSessionが登録されていません。"));
                return;
            }
            
            
            dialog.ShowMessage(new DialogShowMessageSettings(speaker?.SpeakerInfo, face, message, allowSpeedUp, allowSkipping), () =>
            {
                handle.Complete();
            });
        }
    }

    [Serializable]
    public class DialogShowMessageSettings
    {
        public DialogShowMessageSettings(ISpeakerInfo speakerInfo, string face, string message, bool allowSpeedUp, bool allowSkipping)
        {
            SpeakerInfo = speakerInfo;
            Face = face;
            Message = message;
            AllowSpeedUp = allowSpeedUp;
            AllowSkipping = allowSkipping;
        }

        public ISpeakerInfo SpeakerInfo { get; }    
        public string Face { get; }

        public string Message { get; }

        public bool AllowSpeedUp { get; }
        public bool AllowSkipping { get; }
    }
}