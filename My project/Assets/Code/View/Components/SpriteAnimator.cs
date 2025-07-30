using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class SpriteSheetTag : ICmsComponentDefinition
{
    public List<Sprite> sprites;
    public float FPS = 10;
}

public class SpriteAnimator : ViewComponent
{
    public SpriteRenderer spriteRenderer;

    private SpriteSheetTag _spriteSheetTag;
    private Coroutine _animationCoroutine;

    protected override void OnStateInitialized()
    {
        if (State.Model.Is(out SpriteSheetTag sheetTag))
        {
            _spriteSheetTag = sheetTag;
            Play();
        }
    }
    
    public void Play()
    {
        Stop();
        _animationCoroutine = StartCoroutine(PlayAnimation());

    }
    public void Stop()
    {
        if (_animationCoroutine == null) return;
        StopCoroutine(_animationCoroutine);
        _animationCoroutine = null;
    }

    private IEnumerator PlayAnimation()
    {
        int index = 0;
        float delay = 1f / _spriteSheetTag.FPS;
        var spriteList = _spriteSheetTag.sprites;
        while (index < spriteList.Count)
        {
            spriteRenderer.sprite = spriteList[index];
            index = (index + 1) % spriteList.Count;
            yield return new WaitForSeconds(delay);
        }
    }


}