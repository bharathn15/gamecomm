using Google;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class GoogleSigninHandler : MonoBehaviour
{


    [SerializeField] TMP_Text status;

    [SerializeField] string webClientId = "<color=blue>Web client ID</color>";


    GoogleSignInConfiguration googleSignInConfiguration;

    #region Monobehaviour callbacks

    private void Awake()
    {
        googleSignInConfiguration = new GoogleSignInConfiguration
        {
            WebClientId = webClientId,
            RequestAuthCode  = true,
            RequestEmail = true,
            RequestIdToken = true,
            RequestProfile = true,
            
        };
    }

    private void OnEnable()
    {
        GoogleSignIn.Configuration = googleSignInConfiguration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestAuthCode = true;
        GoogleSignIn.Configuration.RequestEmail = true;
        GoogleSignIn.Configuration.RequestIdToken = true;
        GoogleSignIn.Configuration.RequestProfile = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    
    #endregion

    #region Public methods
    public void SignInOnButtonClick()
    {
        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
        
    }

    public void SignInSilentlyOnButtonClick()
    {
        GoogleSignIn.DefaultInstance.SignInSilently()
            .ContinueWith(OnAuthenticationFinished);
    }

    public void SignOutOnButtonClick()
    {
        GoogleSignIn.DefaultInstance.SignOut();
        status.text = string.Empty;
    }
    #endregion

    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            using (IEnumerator<System.Exception> enumerator =
                    task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error =
                            (GoogleSignIn.SignInException)enumerator.Current;
                    status.text = $"<color=red>Got Error: {error.Status} {error.Message}</color>";
                }
                else
                {
                    status.text = $"<color=red>Got Unexpected Exception?!? {task.Exception}</color>";
                }
            }
        }
        else if (task.IsCanceled)
        {
            status.text = $"<color=red>cancelled</color>";
        }
        else
        {
            
            status.text =   $"Given name                    {task.Result.GivenName}" +
                            $"Display name                  {task.Result.DisplayName}" +
                            $"Email                         {task.Result.Email}" +
                            $"User Id                       {task.Result.UserId}" +
                            $"ID Token                      {task.Result.IdToken}";
        }
    }
}
