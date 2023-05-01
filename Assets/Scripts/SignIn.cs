using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using System.Threading.Tasks;
using Firebase.Extensions;

public class SignIn : MonoBehaviour
{
    void Start()
    {
        PlayGamesPlatform.Instance.Authenticate((success) =>
        {
            if (success == SignInStatus.Success)
            {
                PlayGamesPlatform.Instance.RequestServerSideAccess(false, (authCode) =>
                {
                    Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
                    Firebase.Auth.Credential credential =
                        Firebase.Auth.PlayGamesAuthProvider.GetCredential(authCode);
                    auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
                    {
                        if (task.IsCanceled)
                        {
                            Debug.LogFormat("Firebase auth cancel");
                            return;
                        }

                        if (task.IsFaulted)
                        {
                            Debug.LogFormat("Firebase auth fault");
                            return;
                        }

                        Firebase.Auth.FirebaseUser newUser = task.Result;
                        Debug.LogFormat("User signed in successfully: {0} ({1})",
                            newUser.DisplayName, newUser.UserId);
                    });
                });
            }
            else
            {
                Debug.LogFormat("Google play service auth error");
            }
        });
    }
}