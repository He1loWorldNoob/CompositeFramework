using UnityEngine.Rendering;

public class ViewSortingGroup : ViewComponent
{
    public SortingGroup sortingLayer;
    private void Update()
    {
        sortingLayer.sortingOrder = -(int)transform.position.z;
    }
}