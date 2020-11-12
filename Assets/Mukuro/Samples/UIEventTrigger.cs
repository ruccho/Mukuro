using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mukuro.Samples
{
    public class UIEventTrigger : MonoBehaviour
    {
        [SerializeField] private EventScriptAsset script = default;
        [SerializeField] private EventScriptPlayer player = default;
        
        public void Play()
        {
            player.Play(new EventPlayingOption(script)
            {
                SceneForRuntimeReference = gameObject.scene
            });
        }
    }
}