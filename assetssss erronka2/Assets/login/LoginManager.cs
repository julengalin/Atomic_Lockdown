using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;

public class LoginManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text sessionStatusText;
    public Button anonimoButton;
    public Button unityButton;
    public Button signOutButton;

    [Header("Botón Escena Pruebas")]
    public Button escenaPruebasButton;

    [Header("Anon Popup UI")]
    public GameObject anonPanel;
    public TMP_InputField anonNameInput;
    public Button anonOkButton;
    public Button anonCancelButton;

    [Header("Scenes")]
    public string lobbySceneName = "Lobby";
    public string escenaPruebasName = "EscenaPruebasMecanicas";

    private bool servicesReady = false;
    private bool unityLoginInProgress = false;

    private async void Awake()
    {
        if (anonOkButton) anonOkButton.onClick.AddListener(OnAnonOk);
        if (anonCancelButton) anonCancelButton.onClick.AddListener(HideAnonPanel);

        if (anonimoButton) anonimoButton.onClick.AddListener(OnClickAnonimo);
        if (unityButton) unityButton.onClick.AddListener(OnClickUnity);
        if (signOutButton) signOutButton.onClick.AddListener(OnClickSignOut);
        
        if (escenaPruebasButton)
            escenaPruebasButton.onClick.AddListener(LoadEscenaPruebas);

        if (anonPanel) anonPanel.SetActive(false);

        try
        {
            await UnityServices.InitializeAsync();
            servicesReady = true;

            AuthenticationService.Instance.SignedIn += RefreshUI;
            AuthenticationService.Instance.SignedOut += RefreshUI;
            AuthenticationService.Instance.Expired += RefreshUI;

            PlayerAccountService.Instance.SignedIn += OnPlayerAccountsSignedIn;
        }
        catch (Exception e)
        {
            Debug.LogError("Unity zerbitzuak ezin izan dira hasieratu:\n" + e);
            servicesReady = false;
        }

        RefreshUI();
    }

    private void OnDestroy()
    {
        if (servicesReady)
        {
            PlayerAccountService.Instance.SignedIn -= OnPlayerAccountsSignedIn;
        }
    }

    // ---------------- UI STATE ----------------
    private void RefreshUI()
    {
        var type = SessionData.GetSessionType();
        var name = SessionData.GetPlayerName();

        bool hasSession = type != SessionType.None;

        if (sessionStatusText)
        {
            if (!hasSession)
                sessionStatusText.text = "Saioa hasi gabe";
            else
            {
                string who = type == SessionType.Anonymous ? "ANONIMOA" : "UNITY";
                string namePart = string.IsNullOrWhiteSpace(name) ? "" : $" ({name})";
                sessionStatusText.text = $"Saioa hasita: {who}{namePart}";
            }
        }

        if (signOutButton)
            signOutButton.interactable = hasSession;

        if (anonimoButton)
            anonimoButton.interactable = true;

        if (unityButton)
            unityButton.interactable = !unityLoginInProgress;
    }

    // ---------------- ANÓNIMO ----------------
    private void OnClickAnonimo()
    {
        if (SessionData.GetSessionType() != SessionType.None) return;
        ShowAnonPanel();
    }

    private void ShowAnonPanel()
    {
        if (!anonPanel) return;
        anonPanel.SetActive(true);

        if (anonNameInput)
        {
            anonNameInput.text = "";
            anonNameInput.ActivateInputField();
        }
    }

    private void HideAnonPanel()
    {
        if (!anonPanel) return;
        anonPanel.SetActive(false);
    }

    private void OnAnonOk()
    {
        string chosen = anonNameInput ? anonNameInput.text : "";
        chosen = (chosen ?? "").Trim();

        if (string.IsNullOrWhiteSpace(chosen))
            chosen = "Jokalaria";

        SessionData.SetSession(SessionType.Anonymous, chosen);

        HideAnonPanel();
        RefreshUI();
        LoadLobby();
    }

    // ---------------- UNITY PLAYER ACCOUNTS ----------------
    private async void OnClickUnity()
    {
        if (SessionData.GetSessionType() != SessionType.None) return;

        if (!servicesReady)
        {
            Debug.LogWarning("Unity zerbitzuak ez daude prest. Ezin da saioa hasi.");
            return;
        }

        try
        {
            unityLoginInProgress = true;
            RefreshUI();

            await PlayerAccountService.Instance.StartSignInAsync();
        }
        catch (Exception e)
        {
            unityLoginInProgress = false;
            RefreshUI();
            Debug.LogError("Saioa hastean errorea:\n" + e);
        }
    }

    private async void OnPlayerAccountsSignedIn()
    {
        try
        {
            string accessToken = PlayerAccountService.Instance.AccessToken;

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                unityLoginInProgress = false;
                RefreshUI();
                Debug.LogError("AccessToken hutsik dago. Egiaztatu konfigurazioa.");
                return;
            }

            await AuthenticationService.Instance.SignInWithUnityAsync(accessToken);

            SessionData.SetSession(SessionType.Unity, "Unity erabiltzailea");

            unityLoginInProgress = false;
            RefreshUI();
            LoadLobby();
        }
        catch (Exception e)
        {
            unityLoginInProgress = false;
            RefreshUI();
            Debug.LogError("Unity saioa hastean errorea:\n" + e);
        }
    }

    // ---------------- SIGN OUT ----------------
    private void OnClickSignOut()
    {
        if (SessionData.GetSessionType() == SessionType.None) return;

        SessionData.ClearSession();

        if (servicesReady)
        {
            try
            {
                if (AuthenticationService.Instance.IsSignedIn)
                    AuthenticationService.Instance.SignOut();

                AuthenticationService.Instance.ClearSessionToken();
            }
            catch (Exception e)
            {
                Debug.LogError("Saioa ixtean errorea:\n" + e);
            }
        }

        unityLoginInProgress = false;
        RefreshUI();
    }

    // ---------------- LOAD SCENES ----------------
    private void LoadLobby()
    {
        if (string.IsNullOrWhiteSpace(lobbySceneName))
        {
            Debug.LogWarning("lobbySceneName hutsik dago.");
            return;
        }

        SceneManager.LoadScene(lobbySceneName);
    }

    private void LoadEscenaPruebas()
    {
        if (string.IsNullOrWhiteSpace(escenaPruebasName))
        {
            Debug.LogWarning("escenaPruebasName hutsik dago.");
            return;
        }

        SceneManager.LoadScene(escenaPruebasName);
    }
}