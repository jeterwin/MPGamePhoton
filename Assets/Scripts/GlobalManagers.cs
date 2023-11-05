using UnityEngine;

public class GlobalManagers : MonoBehaviour
{
    public static GlobalManagers Instance;
    public GamePoolingManager PoolingManager;
    public GameManager GameManager;
    [field: SerializeField] public NetworkRunnerController NetworkRunnerController { get; private set; }

    [SerializeField] private GameObject parentObj;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(parentObj);
        }
    }
}
