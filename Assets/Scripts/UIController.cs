using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private LoadingCanvasController loadingCanvas;

    [SerializeField] private TMP_InputField createRoomNameInputField;
    [SerializeField] private TMP_InputField joinRoomNameInputField;

    [SerializeField] private Button createRoomBtn;
    [SerializeField] private Button joinRoomBtn;
    [SerializeField] private Button joinRandomRoomBtn;
    private void Awake()
    {
        createRoomBtn.onClick.AddListener(() => CreateRoom(GameMode.Host, createRoomNameInputField.text));
        joinRoomBtn.onClick.AddListener(() => CreateRoom(GameMode.Client, joinRoomNameInputField.text));
        joinRandomRoomBtn.onClick.AddListener(JoinRandomRoom);

        Instantiate(loadingCanvas);
    }
    public void JoinRandomRoom()
    {
        GlobalManagers.Instance.networkRunnerController.StartGame(GameMode.AutoHostOrClient, string.Empty);
    }
    public void CreateRoom(GameMode mode, string field)
    {
        if(field.Length >= 2)
        {
            GlobalManagers.Instance.networkRunnerController.StartGame(mode, field);
        }
    }
}
