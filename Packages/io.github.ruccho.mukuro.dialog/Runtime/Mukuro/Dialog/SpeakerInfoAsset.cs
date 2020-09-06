using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mukuro.Dialog
{
    public abstract class SpeakerInfoAsset : ScriptableObject
    {
        public abstract ISpeakerInfo SpeakerInfo { get; }
    }
}