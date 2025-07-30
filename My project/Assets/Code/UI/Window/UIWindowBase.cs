using UnityEngine;
using UnityEngine.UI;

public abstract class UIWindowBase : MonoBehaviour
{
    public Button closeButton;
    private void Awake()
    {
        OnAwake(); 
    }
    protected virtual void OnAwake()
    {
        closeButton.onClick.AddListener(Close);
    }

    public virtual void Close()
    {
        Destroy(gameObject);
    }
}