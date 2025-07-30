using UnityEngine;

[DefaultExecutionOrder(-100)]
public class GameRunner : MonoBehaviour
{
    [SerializeField] private GameBootstraper bootstraper;
    private void Awake()
    {
        if (!FindAnyObjectByType<GameBootstraper>())
        {
            Instantiate(bootstraper);
        }
        Destroy(gameObject);

    }
}