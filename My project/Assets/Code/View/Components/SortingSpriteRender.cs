using System;
using UnityEngine;

public class SortingSpriteRender : ViewComponent
{
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        sr.sortingOrder = -(int)transform.position.z;
    }
}