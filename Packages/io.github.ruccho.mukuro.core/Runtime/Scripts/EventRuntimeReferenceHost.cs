using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Mukuro
{
    public class EventRuntimeReferenceHost : MonoBehaviour, IExposedPropertyTable
    {
        public EventScriptAsset TargetAsset
        {
            set => TargetAssetId = value.Id;
        }

        [SerializeField] public string TargetAssetId;

        
        [SerializeField] private List<EventRuntimeReference> references = new List<EventRuntimeReference>();

        private void Start()
        {
            EventRuntimeReferenceHostRegistry.Register(this);
        }

        public void SetReferenceValue(PropertyName id, Object value)
        {
            var reference = references.FirstOrDefault(r => r.Id == id);
            if (reference == default)
            {
                references.Add(new EventRuntimeReference(id, value));
            }
            else
            {
                reference.Value = value;
            }
        }

        public Object GetReferenceValue(PropertyName id, out bool idValid)
        {
            var reference = references.FirstOrDefault(r => r.Id == id);
            if (reference == default)
            {
                idValid = false;
                return null;
            }
            else
            {
                idValid = true;
                return reference.Value;
            }
        }

        public void ClearReferenceValue(PropertyName id)
        {
            var reference = references.FirstOrDefault(r => r.Id == id);
            if (reference != default)
            {
                references.Remove(reference);
            }
        }
    }

    public static class EventRuntimeReferenceHostRegistry
    {
        private static Dictionary<Scene, List<EventRuntimeReferenceHost>> registry = new Dictionary<Scene, List<EventRuntimeReferenceHost>>();

        private static bool isInitialized = false;
        private static void EnsureInitialized()
        {
            if (isInitialized) return;
            SceneManager.sceneLoaded += (s, m) => AddSceneEntry(s);
            SceneManager.sceneUnloaded += RemoveSceneEntry;
            isInitialized = true;
        }

        static EventRuntimeReferenceHostRegistry()
        {
            EnsureInitialized();
        }

        private static void AddSceneEntry(Scene scene)
        {
            if (!registry.ContainsKey(scene))
            {
                registry.Add(scene, new List<EventRuntimeReferenceHost>());
            }
            else
            {
                throw new InvalidOperationException("Scene is already loaded");
            }
            
        }

        private static void RemoveSceneEntry(Scene scene)
        {
            if (registry.ContainsKey(scene))
            {
                registry.Remove(scene);
            }
        }
        
        public static void Register(EventRuntimeReferenceHost host)
        {
            EnsureInitialized();
            var scene = host.gameObject.scene;
            if(!registry.ContainsKey(scene)) AddSceneEntry(scene);
            var list = registry[scene];
            list.Add(host);
        }

        public static EventRuntimeReferenceHost Get(Scene scene, EventScriptAsset target)
        {
            if(!registry.ContainsKey(scene)) AddSceneEntry(scene);
            var list = registry[scene];
            return list.FirstOrDefault(item => item.TargetAssetId == target.Id);
        }
    }

    [Serializable]
    public class EventRuntimeReference
    {
        [SerializeField] public PropertyName Id;
        [SerializeField] public Object Value;

        public EventRuntimeReference(PropertyName id, Object value)
        {
            Id = id;
            Value = value;
        }
    }
}