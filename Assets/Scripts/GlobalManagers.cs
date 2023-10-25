using UnityEngine;

public class GlobalManagers : MonoBehaviour
{
    public static GlobalManagers Instance;
    [field: SerializeField] public NetworkRunnerController networkRunnerController { get; private set; }

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
