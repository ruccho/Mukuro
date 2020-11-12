using System;
using System.Collections;
using System.Collections.Generic;
using Mukuro.Dialog;
using UnityEngine;

namespace Mukuro.Samples
{
    public class TalkStyleDialogProvider : DialogProviderBase
    {
        [SerializeField] private int maxPopCount = 20;
        [SerializeField] private RectTransform popParent = default;
        [SerializeField] private GameObject popMessageL = default;
        [SerializeField] private GameObject popMessageR = default;
        [SerializeField] private GameObject popMenuL = default;
        [SerializeField] private GameObject popMenuR;

        public override string ProviderName => "TalkStyle";

        private ISpeakerInfo speakerL;
        private ISpeakerInfo speakerR;
        private List<MonoBehaviour> Pops = new List<MonoBehaviour>();

        public override void Open(Action onOpened)
        {
            speakerL = null;
            speakerR = null;
            Pops.Clear();
            onOpened?.Invoke();
        }

        public override void Close(Action onClosed)
        {
            onClosed?.Invoke();
        }

        public override void ShowMessage(DialogShowMessageSettings settings, Action onNext)
        {
            ISpeakerInfo s = settings.SpeakerInfo;
            SetSpeaker(s);

            if (string.IsNullOrEmpty(settings.Message))
            {
                onNext?.Invoke();
                return;
            }

            GameObject popObj;
            if (s == speakerL)
            {
                popObj = Instantiate(popMessageL, popParent);
            }
            else
            {
                popObj = Instantiate(popMessageR, popParent);
            }

            var pop = popObj.GetComponent<TalkStyleDialogMessagePop>();

            pop.SetMessage(settings, onNext);

            Pops.Add(pop);
            DestroyOldPops();
        }

        public override void ShowMenu(DialogShowMenuSettings settings, Action<int> onSelected)
        {
            ISpeakerInfo s = settings.SpeakerInfo;
            SetSpeaker(s);

            GameObject popObj;

            popObj = Instantiate(popMenuL, popParent);

            var pop = popObj.GetComponent<TalkStyleDialogMenuPop>();

            pop.Open(settings, onSelected);

            Pops.Add(pop);
            DestroyOldPops();
        }

        private void SetSpeaker(ISpeakerInfo s)
        {
            if (s == speakerL || s == speakerR)
            {
                return;
            }
            else
            {
                if (speakerL == null) speakerL = s;
                else speakerR = s;
            }
        }

        private void DestroyOldPops()
        {
            int destroyCount = Pops.Count - maxPopCount;
            if (destroyCount > 0)
            {
                for (int i = 0; i < destroyCount; i++)
                {
                    Destroy(Pops[i].gameObject);
                }

                Pops.RemoveRange(0, destroyCount);
            }
        }
    }
}