using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mukuro.Dialog;
using UnityEngine;

[CreateAssetMenu(menuName = "Mukuro/SpeakerInfo (Regular)", fileName = "Speaker")]
public class RegularSpeakerInfoAsset : SpeakerInfoAsset
{
    [SerializeField] private RegularSpeakerInfo speakerInfo = default;
    public override ISpeakerInfo SpeakerInfo => speakerInfo;
}

[Serializable]
public class RegularSpeakerInfo : ISpeakerInfo
{
    [SerializeField] private string name = default;
    [SerializeField] private string displayName = default;

    [SerializeField] private RegularSpeakerInfoFaceSpriteEntry[] faceSprites = default;

    public string Name => name;
    public string DisplayName => displayName;
    
#pragma warning disable 1998
    public async Task<Sprite> GetFaceSpriteAsync(string face, CancellationToken cancellationToken = default)
#pragma warning restore 1998
    {
        var matched = faceSprites.FirstOrDefault(f => f.Name == face);
        if (matched == default)
        {
            return null;
        }
        else
        {
            return matched.Sprite;
        }
    }

    public IEnumerable<string> Faces => faceSprites.Select(f => f.Name);
}

[Serializable]
public class RegularSpeakerInfoFaceSpriteEntry
{
    [SerializeField] private string name = default;
    [SerializeField] private Sprite sprite = default;

    public string Name => name;
    public Sprite Sprite => sprite;
}

