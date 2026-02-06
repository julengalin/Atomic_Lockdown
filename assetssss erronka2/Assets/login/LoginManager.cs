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
    public string sceneAfterLogin = "game";
    private bool hasLoadedScene = false;

    private async void Awake()
    {
        if (UnityServices.State == ServicesInitializationState.Uninitialized)
        {
            Debug.Log("Services Initializing");
            await UnityServices.InitializeAsync();
        }

        PlayerAccountService.Instance.SignedIn += SignInOrLinkWithUnity;
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
            // 1. Player is not yet authenticated, signing up with Unity
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

            // 2. Player is authenticated, but does not yet have a Unity ID linked, so let's link
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

            // 3. Player has authentication and a Unity ID
            Debug.Log("Player is already signed in to their Unity Player Account");

            LoadGameSceneOnce();
        }
        catch (RequestFailedException ex)
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
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public async void StartAnonymousSignIn()
    {
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
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }

    public async void StartUnitySignInAsync()
    {
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
    }

    async void SignInWithUnity()
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUnityAsync(
                PlayerAccountService.Instance.AccessToken
            );

            LoadGameSceneOnce();
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }

    private async Task LinkWithUnityAsync(string accessToken)
    {
        try
        {
            await AuthenticationService.Instance.LinkWithUnityAsync(accessToken);
            Debug.Log("Link is successful.");

            LoadGameSceneOnce();
        }
        catch (AuthenticationException ex) when (ex.ErrorCode == AuthenticationErrorCodes.AccountAlreadyLinked)
        {
            Debug.LogError("This user is already linked with another account. Log in instead.");
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }
}
