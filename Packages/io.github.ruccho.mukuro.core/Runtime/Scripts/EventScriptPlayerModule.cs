using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mukuro
{
    public abstract class EventScriptPlayerModule : MonoBehaviour
    {
        /// <summary>
        /// EventScriptPlayerの初期化処理、Moduleの登録処理が終了した際に呼ばれる。
        /// </summary>
        public virtual void OnInitialized()
        {}
        
        /// <summary>
        /// イベントスクリプトの再生が開始する直前に呼ばれる。
        /// </summary>
        /// <param name="context"></param>
        public virtual void OnPlay(EventExecutionContext context)
        {
        }

        /// <summary>
        /// イベントスクリプトの再生中にエラーが発生し中断されたときに呼ばれる。
        /// </summary>
        /// <param name="context"></param>
        public virtual void OnError(EventExecutionContext context)
        {
        }

        /// <summary>
        /// イベントスクリプトの再生が正常に終了したときに呼ばれる。
        /// </summary>
        /// <param name="context"></param>
        public virtual void OnFinished(EventExecutionContext context)
        {
        }
    }
}