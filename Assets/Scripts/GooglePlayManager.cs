using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GooglePlayManager : MonoBehaviour
{
    public Text myLog;
    public Text myName;
    public RawImage myImage;
    private bool isWaitingForAuth = false;

    public Button btnGold;
    public Button btnLogin;
    public Button btnLogout;
    public IAPManager iap;
    public Transform helper;
    public Transform orignal;
    public Transform dest;
    public Transform observer;
    public Vector3 pos;
    public Vector3 rot;

    private void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        myLog.text = "Ready...";

        // 구글 게임서비스 활성화(초기화)
        PlayGamesPlatform.InitializeInstance(new PlayGamesClientConfiguration.Builder().Build());
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        btnLogin.onClick.AddListener(OnLogin);
        btnLogout.onClick.AddListener(OnLogOut);
        pos = (helper.position - orignal.position);
        dest.position = (pos + orignal.position) + orignal.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        // 자동 로그인
        //OnLogin();

        btnGold.onClick.AddListener(() =>
        {
            pos = (helper.position - observer.position);
            observer.position = (pos + observer.position) + observer.position;
            return;
            iap.BuyConsumable();
        });
    }

    public void SetLog(string log)
    {
        myLog.text = log;
    }

    // 자동 로그인
    private void OnLogin()
    {
        observer.position = dest.position - pos;
        return;
        SetLog("...");
        if (isWaitingForAuth) return;

        if (Social.localUser.authenticated)
        {
            SetLog(string.Format("id: {0}", Social.localUser.id));
            myName.text = Social.localUser.userName;
            StartCoroutine(UserPictureLoad());
            return;
        }

        // 로그인 되어 있지 않다면
        SetLog("Authenticating...");
        isWaitingForAuth = true;

        // 로그인 인증 처리과정
        Social.localUser.Authenticate((success)=>
        {
            if (success)
            {
                SetLog(string.Format("id: {0}", Social.localUser.id));
                myName.text = string.Format("id: {0} name : {1}", Social.localUser.id, Social.localUser.userName);
                StartCoroutine(UserPictureLoad());
            }
            else
            {
                SetLog("Login fail");
            }
        });
    }

    private void OnLogOut()
    {
        dest.position = pos + orignal.position;
        return;
        PlayGamesPlatform.Activate().SignOut();
        SetLog("LogOut...");
        myName.text = "";
        isWaitingForAuth = false;
        DestroyImmediate(myImage.texture);
        myImage.texture = null;
    }

    IEnumerator UserPictureLoad()
    {
        SetLog("image Loading...");

        Texture2D tex = Social.localUser.image;

        while(tex == null)
        {
            tex = Social.localUser.image;
            yield return null;
        }

        myImage.texture = tex;
        SetLog("image Created");
    }
}
