using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using System.Threading.Tasks;
public class SignIn : MonoBehaviour
{
    void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available) {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
               var  app = Firebase.FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
            } else {
                UnityEngine.Debug.LogError(System.String.Format(
                    "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
        
        Social.localUser.Authenticate((bool success) => {
            if (success) {
                  PlayGamesPlatform.Instance.RequestServerSideAccess(false, (authCode) =>
                  {
                      Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
                      Firebase.Auth.Credential credential =
                          Firebase.Auth.PlayGamesAuthProvider.GetCredential(authCode);
                      auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
                          if (task.IsCanceled) {
                              return;
                          }
                          if (task.IsFaulted) {
                              return;
                          }

                          Firebase.Auth.FirebaseUser newUser = task.Result;
                          Debug.LogFormat("User signed in successfully: {0} ({1})",
                              newUser.DisplayName, newUser.UserId);
                      });
                  });
            }
        });
    }
}