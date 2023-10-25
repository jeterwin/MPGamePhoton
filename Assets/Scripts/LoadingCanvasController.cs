using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingCanvasController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Button cancelBtn;

    private NetworkRunnerController networkRunnerController;
    // Start is called before the first frame update
    void Start()
    {
        networkRunnerController = GlobalManagers.Instance.networkRunnerController;
        networkRunnerController.OnStartedRunnerConnection += networkRunnerOnStartedRunnerConnection;
        networkRunnerController.OnPlayerJoinedSucessfully += networkRunnerOnPlayerJoinedSucessfully;

        cancelBtn.onClick.AddListener(networkRunnerController.ShutDownRunner);
        gameObject.SetActive(false);
    }
    private void networkRunnerOnStartedRunnerConnection()
    {
        gameObject.SetActive(true);
        StartCoroutine(Utils.PlayAnimAndSetStateWhenFinished(gameObject, animator, "In"));
    }
    private void networkRunnerOnPlayerJoinedSucessfully()
    {
        StartCoroutine(Utils.PlayAnimAndSetStateWhenFinished(gameObject, animator, "Out", false));
    }
    private void OnDestroy()
    {
        networkRunnerController.OnStartedRunnerConnection -= networkRunnerOnStartedRunnerConnection;
        networkRunnerController.OnPlayerJoinedSucessfully -= networkRunnerOnPlayerJoinedSucessfully;
    }
}
