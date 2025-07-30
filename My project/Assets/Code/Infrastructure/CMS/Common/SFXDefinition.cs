using System.Collections.Generic;
using UnityEngine;

namespace Code.CMSSystem.Common
{
    public class SFXArray : ICmsComponentDefinition
    {
        public List<AudioClip> files = new List<AudioClip>();
        public float volume = 1f;
    }
}
