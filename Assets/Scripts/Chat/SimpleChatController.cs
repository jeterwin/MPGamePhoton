using UnityEngine;
using Fusion;
using TMPro;
public class SimpleChatController : NetworkBehaviour
{
    public TextMeshProUGUI textMessagesGrid { get; private set; }

    [SerializeField] private TMP_InputField textInputField;
    [SerializeField] private NetworkPrefabRef textPrefab = NetworkPrefabRef.Empty;

    public override void Spawned()
    {
        //Uncomment this
        //textMessagesGrid = GlobalManagers.Instance.GameManager.MessageGrid;

        bool isLocalPlayer = Object.IsValid == Object.HasInputAuthority;

        gameObject.SetActive(isLocalPlayer);

        if(!isLocalPlayer) { return; }

        textInputField.onSubmit.AddListener(onFieldSubmit);
    }

    private void onFieldSubmit(string arg0)
    {
        if(string.IsNullOrEmpty(arg0)) { return; }

        RpcSendMessage(arg0);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void RpcSendMessage(NetworkString<_64> text)
    {
        textMessagesGrid.text += text + "\n";
    }
}
