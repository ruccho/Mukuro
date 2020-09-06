using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace Mukuro
{
    public struct MukuroUpdate
    {
    }

    public static class MukuroPlayerLoopRegisterer
    {
        [RuntimeInitializeOnLoadMethod]
        private static void RegisterMukuroPlayerLoop()
        {
            using (var modifier = new PlayerLoopModifier())
            {
                modifier.InsertAfter<Update.ScriptRunBehaviourUpdate>(new PlayerLoopSystem()
                {
                    updateDelegate = OnMukuroUpdate,
                    type = typeof(MukuroUpdate)
                });
            }
/*
            var sb = new StringBuilder();
            LogPlayerLoopSystems(PlayerLoop.GetCurrentPlayerLoop(), sb, 0);
            Debug.Log(sb.ToString());
*/
        }

        private static void LogPlayerLoopSystems(PlayerLoopSystem system, StringBuilder sb, int depth)
        {
            if (depth == 0)
            {
                sb.AppendLine("PlayerLoop");
            }
            else if (system.type != null)
            {
                for (int i = 0; i < depth; i++)
                {
                    sb.Append("\t");
                }
                sb.AppendLine(system.type.Name);
            }
            if (system.subSystemList != null)
            {
                depth++;
                foreach (var s in system.subSystemList)
                {
                    LogPlayerLoopSystems(s, sb, depth);
                }
                depth--;
            }
        }

        private static void OnMukuroUpdate()
        {
            foreach(var a in onUpdateActions) a?.Invoke();
        }
        
        private static List<Action> onUpdateActions = new List<Action>();

        public static void AddListenerOnUpdate(Action onUpdate)
        {
            onUpdateActions.Add(onUpdate);
        }
    }
}