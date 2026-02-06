using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    [Header("Scene")]
    public string sceneAfterLogin = "Lobby";
    public string sceneAfterSignOut = "login"; // pon aquí tu escena login

    [Header("Sign Out")]
    public bool clearSessionOnSignOut = true;

    private bool hasLoadedScene = false;

    private async void Awake()
    {
        await InitServicesIfNeeded();

        // Evita duplicar el evento si recargas escena
        PlayerAccountService.Instance.SignedIn -= SignInOrLinkWithUnity;
        PlayerAccountService.Instance.SignedIn += SignInOrLinkWithUnity;
    }

    private async Task InitServicesIfNeeded()
    {
        if (UnityServices.State == ServicesInitializationState.Initialized)
            return;

        if (UnityServices.State == ServicesInitializationState.Uninitialized)
        {
            Debug.Log("Services Initializing");
            await UnityServices.InitializeAsync();
        }
    }

    private void LoadGameSceneOnce()
    {
        if (hasLoadedScene) return;
        hasLoadedScene = true;
        SceneManager.LoadScene(sceneAfterLogin);
    }

    async void SignInOrLinkWithUnity()
    {
        try
        {
            await InitServicesIfNeeded();

            // 1) No autenticado -> login con Unity Player Account
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                Debug.Log("Signing up with Unity Player Account...");
                await AuthenticationService.Instance.SignInWithUnityAsync(
                    PlayerAccountService.Instance.AccessToken
                );
                Debug.Log("Successfully signed up with Unity Player Account");
                LoadGameSceneOnce();
                return;
            }

            // 2) Autenticado pero no linkeado -> link
            if (!HasUnityID())
            {
                Debug.Log("Linking anonymous account to Unity...");
                await AuthenticationService.Instance.LinkWithUnityAsync(
                    PlayerAccountService.Instance.AccessToken
                );
                Debug.Log("Successfully linked anonymous account!");
                LoadGameSceneOnce();
                return;
            }

            // 3) Ya está ok
            Debug.Log("Player is already signed in to their Unity Player Account");
            LoadGameSceneOnce();
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    private bool HasUnityID()
    {
        return AuthenticationService.Instance.PlayerInfo.GetUnityId() != null;
    }

    async void Start()
    {
        await InitServicesIfNeeded();

        // Si ya estás logueado, no repitas
        if (AuthenticationService.Instance.IsSignedIn)
        {
            Debug.Log("Already signed in, skipping auto sign-in.");
            return;
        }

        // Si hay sesión previa, intentamos re-loguear
        if (!AuthenticationService.Instance.SessionTokenExists)
        {
            Debug.Log("Session Token not found");
            return;
        }

        try
        {
            Debug.Log("Returning player signing in...");
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            Debug.Log("Returning player signed in!");
            LoadGameSceneOnce();
        }
        catch (RequestFailedException e)
        {
            Debug.LogException(e);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    // BOTÓN: ANONIMO
    public async void StartAnonymousSignIn()
    {
        await InitServicesIfNeeded();

        // Evita el error: "The player is already signed in"
        if (AuthenticationService.Instance.IsSignedIn)
        {
            Debug.Log("Already signed in -> going next scene.");
            LoadGameSceneOnce();
            return;
        }

        await SignUpAnonymouslyAsync();
    }

    private async Task SignUpAnonymouslyAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Sign in anonymously succeeded!");
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

            LoadGameSceneOnce();
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    // BOTÓN: UNITY
    public async void StartUnitySignInAsync()
    {
        await InitServicesIfNeeded();

        if (PlayerAccountService.Instance.IsSignedIn)
        {
            SignInWithUnity();
            return;
        }

        try
        {
            await PlayerAccountService.Instance.StartSignInAsync();
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    async void SignInWithUnity()
    {
        try
        {
            await InitServicesIfNeeded();

            // Si ya estás logueado, no repitas
            if (AuthenticationService.Instance.IsSignedIn)
            {
                Debug.Log("Already signed in -> going next scene.");
                LoadGameSceneOnce();
                return;
            }

            await AuthenticationService.Instance.SignInWithUnityAsync(
                PlayerAccountService.Instance.AccessToken
            );

            LoadGameSceneOnce();
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    // ✅ BOTÓN: SIGN OUT
    public async void SignOut()
    {
        try
        {
            await InitServicesIfNeeded();

            if (AuthenticationService.Instance.IsSignedIn)
            {
                AuthenticationService.Instance.SignOut();
                Debug.Log("Authentication SignOut OK.");
            }
            else
            {
                Debug.Log("No Authentication session to sign out.");
            }

            if (clearSessionOnSignOut)
            {
                AuthenticationService.Instance.ClearSessionToken();
                Debug.Log("Session token cleared.");
            }

            hasLoadedScene = false;

            if (!string.IsNullOrEmpty(sceneAfterSignOut))
                SceneManager.LoadScene(sceneAfterSignOut);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }
}
