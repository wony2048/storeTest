using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mainframework : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration
          .Builder()
          .RequestServerAuthCode(false)
          .RequestIdToken()
          .Build();
        //커스텀된 정보로 GPGS 초기화
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        //GPGS 시작.
        PlayGamesPlatform.Activate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 구글 토큰 받아옴
    public string GetTokens()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            // 유저 토큰 받기 첫번째 방법
            string _IDtoken = PlayGamesPlatform.Instance.GetIdToken();
            // 두번째 방법
            // string _IDtoken = ((PlayGamesLocalUser)Social.localUser).GetIdToken();
            return _IDtoken;
        }
        else
        {
            Debug.Log("접속되어있지 않습니다. PlayGamesPlatform.Instance.localUser.authenticated :  fail");
            return null;
        }
    }
}
