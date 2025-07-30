using System;
using UnityEngine;

[Serializable]
public class TagSprite : ICmsComponentDefinition
{
    public Sprite sprite;
}


public class SpriteRendererTag : ViewComponent
{
    public SpriteRenderer spriteRenderer;

    protected override void OnStateInitialized()
    {
        if(State.Model.Is(out TagSprite tagSprite))
            spriteRenderer.sprite = tagSprite.sprite;
    }
}