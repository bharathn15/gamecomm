using Google;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GoogleSigninHandler : MonoBehaviour
{

    [SerializeField] RawImage profileImage;
    [SerializeField] TMP_Text displayName;
    [SerializeField] TMP_Text mail;
    [SerializeField] TMP_Text status;

    [SerializeField] string webClientId = "<color=blue>Web client ID</color>";

    [SerializeField] Button signInButton;
    [SerializeField] Button signOutButton;


    GoogleSignInConfiguration googleSignInConfiguration;
    bool _userSignedIn;

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
        Task<GoogleSignInUser> task = (Task<GoogleSignInUser>)GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
        if (task.Status == TaskStatus.Faulted)
        {
            status.text = $"<color=red>{task.Status}</color>";
        }
        else if (task.Status == TaskStatus.Canceled)
        {
            status.text = $"<color=red>{task.Status}</color>";
        }
        else
        {
            status.text = $"<color=green>{task.Status}</color>";
        }

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
        displayName.text = string.Empty;
        mail.text = string.Empty;
        profileImage.texture = null;
        signInButton.gameObject.SetActive(true);
        signOutButton.gameObject.SetActive(false);
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
                    _userSignedIn = true;
                    
                }
            }
        }
        else if (task.IsCanceled)
        {
            status.text = $"<color=red>cancelled</color>";
        }
        else
        {

            displayName.text = task.Result.DisplayName;
            mail.text = task.Result.Email;
            status.text = $"<color=green>successfull login</color>";
            
            StartCoroutine(GetProfilePic(task.Result.ImageUrl.ToString()));

            
            /*status.text =   $"Given name                    {task.Result.GivenName}" +
                            $"Display name                  {task.Result.DisplayName}" +
                            $"Email                         {task.Result.Email}" +
                            $"User Id                       {task.Result.UserId}" +
                            $"ID Token                      {task.Result.IdToken}";*/
        }
    }


    IEnumerator GetProfilePic(string url)
    {
        using(UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
        {
            yield return webRequest.SendWebRequest();

            if(webRequest.result != UnityWebRequest.Result.Success)
            {
                status.text += "\nFailed to load the texture from the URL";
            }
            else
            {
                profileImage.texture = DownloadHandlerTexture.GetContent(webRequest);
                signInButton.gameObject.SetActive(false);
                signOutButton.gameObject.SetActive(true);
            }
            
            
        }
    }
}
