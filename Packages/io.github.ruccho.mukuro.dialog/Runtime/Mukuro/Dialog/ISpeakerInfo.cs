using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Mukuro.Dialog
{
    public interface ISpeakerInfo
    {
        string Name { get;}
        string DisplayName { get; }
        Task<Sprite> GetFaceSpriteAsync(string face, CancellationToken cancellationToken);
        IEnumerable<string> Faces { get; }
    }
}