using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mukuro.Samples
{
    [EventCommand("サンプルカテゴリ", "サンプルコマンド2: カスタムエディタ")]
    public class SampleEventCommand2 : EventCommand
    {
        [SerializeField] private string sampleString;
        [SerializeField] private ExposedReference<GameObject> targetGameObject;
        
        public override void Execute(EventExecutionContext context, CommandExecutionHandle handle)
        {
            var obj = targetGameObject.Resolve(context.ReferenceResolver);
            if (obj)
            {
                Debug.Log($"シーン参照に成功しました。: {obj.name}", obj);
            }
            else
            {
                Debug.Log($"シーン参照に失敗しました。"); 
            }

            handle.Complete();
        }
    }
}