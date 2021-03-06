﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mukuro.Dialog
{
    [EventCommand("Mukuro Dialog", "メッセージの表示")]
    public class DialogShowMessageCommand : EventCommand
    {
        [SerializeReference] private DialogShowMessageSettings settings = default;

        public override void Execute(EventExecutionContext context, CommandExecutionHandle handle)
        {
            var dialog = context.GetModule<DialogModule>();
            if (dialog == default)
            {
                context.RaiseError(new ScriptExecutionException("DialogSessionが登録されていません。"));
                return;
            }


            dialog.ShowMessage(settings, () => { handle.Complete(); });
        }
    }
}