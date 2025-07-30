using UnityEngine;

namespace Code.CMSSystem.Common
{
    public class SFXTag : ICmsComponentDefinition
    {
        public string sfx_id;
        public float Cooldown = 0.1f;
        public bool VaryPitch;
    }

    public class MusicTag : ICmsComponentDefinition
    {
        public AudioClip clip;
    }

    public class AmbientTag : ICmsComponentDefinition
    {
        public AudioClip clip;
    }
}