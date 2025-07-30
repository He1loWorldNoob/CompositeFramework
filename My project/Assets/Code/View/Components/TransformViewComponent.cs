public class TransformViewComponent : ViewComponent
{
    private TransformComponent _transform;

    protected override void OnStateInitialized()
    {
        _transform = State.Get<TransformComponent>();
        UpdateTransform();
    }
    
    private void Update()
    {
        UpdateTransform();
    }
    
    private void UpdateTransform()
    {
        if(_transform ==null) return;
        transform.position = _transform.position;
        transform.localScale = _transform.scale;
    }
}