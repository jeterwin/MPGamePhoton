using UnityEngine;
using Fusion;
using TMPro;
public class ChatController : NetworkBehaviour
{
    public Transform textMessagesGrid { get; private set; }

    [SerializeField] private TMP_InputField textInputField;
    [SerializeField] private NetworkPrefabRef textPrefab = NetworkPrefabRef.Empty;

    public override void Spawned()
    {
        textMessagesGrid = GlobalManagers.Instance.GameManager.MessageGrid;

        var isLocalPlayer = Object.InputAuthority == Runner.LocalPlayer;

        gameObject.SetActive(isLocalPlayer);

        if(isLocalPlayer)
        {
            textInputField.onSubmit.AddListener(onFieldSubmit);
        }
    }

    public void onFieldSubmit(string arg0)
    {
        if(string.IsNullOrEmpty(arg0)) { return; }

        RPC_CreateMessage(arg0);
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_CreateMessage(string text)
    {
        NetworkObject textObject = Runner.Spawn(textPrefab, Vector3.zero, Quaternion.identity, Runner.LocalPlayer); 
        RPC_SetMessageChild(textObject, text);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_SetMessageChild(NetworkObject textPrefab, string text)
    {
        textPrefab.transform.SetParent(textMessagesGrid);
        textPrefab.GetComponent<TextMeshProUGUI>().text = text;
    }
}
