using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

// UGS Core + Authentication
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Authentication;

// Unity Player Accounts (Login con Unity)
using Unity.Services.Authentication.PlayerAccounts;

public class LoginManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text sessionStatusText;

    public Button anonimoButton;
    public Button unityButton;
    public Button signOutButton;

    [Header("Anon Popup UI")]
    public GameObject anonPanel;
    public TMP_InputField anonNameInput;
    public Button anonOkButton;
    public Button anonCancelButton;

    [Header("Scene")]
    [Tooltip("Nombre EXACTO de la escena a cargar al iniciar sesión (debe estar en Build Settings).")]
    public string lobbySceneName = "Lobby";

    private bool servicesReady = false;

    private async void Awake()
    {
        // Hook de botones
        if (anonOkButton) anonOkButton.onClick.AddListener(OnAnonOk);
        if (anonCancelButton) anonCancelButton.onClick.AddListener(HideAnonPanel);

        if (anonimoButton) anonimoButton.onClick.AddListener(OnClickAnonimo);
        if (unityButton) unityButton.onClick.AddListener(OnClickUnity);
        if (signOutButton) signOutButton.onClick.AddListener(OnClickSignOut);

        if (anonPanel) anonPanel.SetActive(false);

        // Inicializa Unity Services (UGS)
        try
        {
            // (Opcional) Environment; si no lo usas, puedes borrar esta línea.
            var options = new InitializationOptions().SetEnvironmentName("production");

            await UnityServices.InitializeAsync(options);

            servicesReady = true;

            // Eventos de Authentication
            AuthenticationService.Instance.SignedIn += RefreshUI;
            AuthenticationService.Instance.SignedOut += RefreshUI;
            AuthenticationService.Instance.Expired += RefreshUI;
        }
        catch (Exception e)
        {
            Debug.LogWarning("No se pudieron inicializar Unity Services: " + e.Message);
            servicesReady = false;
        }

        RefreshUI();
    }

    private void RefreshUI()
    {
        var type = SessionData.GetSessionType();
        var name = SessionData.GetPlayerName();

        bool hasSession = type != SessionType.None;

        if (sessionStatusText)
        {
            if (!hasSession)
            {
                sessionStatusText.text = "Sesión no iniciada";
            }
            else
            {
                string who = type == SessionType.Anonymous ? "ANÓNIMO" : "UNITY";
                string namePart = string.IsNullOrWhiteSpace(name) ? "" : $" ({name})";
                sessionStatusText.text = $"Sesión iniciada con {who}{namePart}";
            }
        }

        if (signOutButton) signOutButton.interactable = hasSession;

        // Opcional: bloquear login si ya hay sesión
        if (anonimoButton) anonimoButton.interactable = !hasSession;
        if (unityButton) unityButton.interactable = !hasSession;
    }

    // ---------- ANÓNIMO ----------
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
            chosen = "Player";

        // Guardamos sesión local
        SessionData.SetSession(SessionType.Anonymous, chosen);

        HideAnonPanel();
        RefreshUI();

        LoadLobby();
    }

    // ---------- UNITY ----------
    private async void OnClickUnity()
    {
        if (SessionData.GetSessionType() != SessionType.None) return;

        if (!servicesReady)
        {
            Debug.LogWarning("Unity Services no está listo. No se puede hacer login con Unity.");
            return;
        }

        try
        {
            // 1) Inicia flujo Unity Player Accounts (abre login)
            await PlayerAccountService.Instance.StartSignInAsync();

            // 2) Token -> UGS Authentication
            var accessToken = PlayerAccountService.Instance.AccessToken;

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                Debug.LogWarning("AccessToken vacío. Revisa Client ID / Player Accounts / Dashboard.");
                return;
            }

            await AuthenticationService.Instance.SignInWithUnityAsync(accessToken);

            // 3) Guardar sesión local (puedes mejorar el nombre más adelante)
            string unityName = "UnityUser";
            SessionData.SetSession(SessionType.Unity, unityName);

            RefreshUI();
            LoadLobby();
        }
        catch (Exception e)
        {
            Debug.LogWarning("Login Unity falló: " + e.Message);
        }
    }

    // ---------- SIGN OUT ----------
    private void OnClickSignOut()
    {
        if (SessionData.GetSessionType() == SessionType.None) return;

        // Limpia tu sesión local
        SessionData.ClearSession();

        // Cierra sesión en UGS si aplica
        if (servicesReady)
        {
            try
            {
                if (AuthenticationService.Instance.IsSignedIn)
                    AuthenticationService.Instance.SignOut();

                // Limpia token guardado (evita que se restaure sola)
                AuthenticationService.Instance.ClearSessionToken();
            }
            catch (Exception e)
            {
                Debug.LogWarning("SignOut UGS falló: " + e.Message);
            }
        }

        RefreshUI();
    }

    // ---------- HELPERS ----------
    private void LoadLobby()
    {
        if (string.IsNullOrWhiteSpace(lobbySceneName))
        {
            Debug.LogWarning("lobbySceneName está vacío. No puedo cargar la escena.");
            return;
        }

        SceneManager.LoadScene(lobbySceneName);
    }
}
