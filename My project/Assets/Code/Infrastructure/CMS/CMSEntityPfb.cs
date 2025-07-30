using UnityEngine;

namespace Code.Infrastructure.CMSSystem
{
    public class CMSEntityPfb : MonoBehaviour
    {
        public CmsEntity entityState;
        
        public Sprite GetSprite()
        {
            if (entityState.Is(out TagSprite sprite))
            {
                return sprite.sprite;
            }
            return null;
        }
    }
}