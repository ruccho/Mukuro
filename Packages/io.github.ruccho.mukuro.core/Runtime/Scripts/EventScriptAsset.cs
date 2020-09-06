using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mukuro
{
    [CreateAssetMenu(fileName = "Event00000", menuName = "Mukuro/CommandScriptAsset")]
    public class EventScriptAsset : ScriptableObject
    {
        [SerializeField] private EventScript script = default;
        [SerializeField] private string id;
        public string Id => id;

        public EventScript Script => script;

        public void SetUniqueId()
        {
            id = Guid.NewGuid().ToString();
        }

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(id))
            {
                SetUniqueId();
            }
        }
        
    }
}