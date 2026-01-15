using TMPro;
using UnityEngine;

public class RelayUI : MonoBehaviour
{
    [SerializeField] private TMP_Text joinCodeText;
    [SerializeField] private TMP_InputField joinCodeInput;

    public async void Create()
    {
        if (joinCodeText != null)
            joinCodeText.text = "CREATING...";

        string code = await RelayManager.Instance.CreateRelayAndStartHost();

        if (joinCodeText != null)
        {
            if (string.IsNullOrEmpty(code))
                joinCodeText.text = "CREATE FAILED (Relay)";
            else
                joinCodeText.text = $"CODE: {code}";
        }

        Debug.Log($"UI Create result: {code}");
    }

    public async void Join()
    {
        string code = joinCodeInput != null ? joinCodeInput.text : "";

        if (joinCodeText != null)
            joinCodeText.text = "JOINING...";

        Debug.Log($"UI Join pressed with code: {code}");

        bool ok = await RelayManager.Instance.JoinRelayAndStartClient(code);

        if (joinCodeText != null)
        {
            if (ok)
                joinCodeText.text = "JOINING (waiting netcode...)";
            else
                joinCodeText.text = "JOIN FAILED (Relay)";
        }

        Debug.Log($"UI Join result: {ok}");
    }
}
