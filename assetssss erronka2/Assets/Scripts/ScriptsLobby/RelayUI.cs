using TMPro;
using UnityEngine;

public class RelayUI : MonoBehaviour
{
    [SerializeField] private TMP_Text joinCodeText;
    [SerializeField] private TMP_InputField joinCodeInput;

    public async void Create()
    {
        string code = await RelayManager.Instance.CreateRelayAndStartHost();
        if (joinCodeText != null)
            joinCodeText.text = string.IsNullOrEmpty(code) ? "ERROR" : code;
    }

    public async void Join()
    {
        string code = joinCodeInput != null ? joinCodeInput.text : "";
        bool ok = await RelayManager.Instance.JoinRelayAndStartClient(code);

        if (!ok && joinCodeText != null)
            joinCodeText.text = "JOIN FAILED";
    }
}
