using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class Web
{
    private MonoBehaviour _mbi;
    public Web(MonoBehaviour mbi)
    {
        _mbi = mbi;
    }

    public void Send(string param)
    {
        Dictionary<string, string> dicHeader = new Dictionary<string, string>();
        dicHeader.Add("Content-Type", "application/json");

        byte[] rawDatas = Encoding.UTF8.GetBytes(param);
        var request = new UnityWebRequest("http://192.168.0.2/store", "POST");
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(rawDatas);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.timeout = 10;
        request.redirectLimit = 0;

        Debug.Log(message: "send url : " + request.url);
        
        foreach (var pair in dicHeader)
        {
            request.SetRequestHeader(pair.Key, pair.Value);
        }

        _mbi.StartCoroutine(WaitForRequest(request));
    }

    private IEnumerator WaitForRequest(UnityWebRequest www)
    {
        yield return www.SendWebRequest();

        //while (www.isDone == false || (www.isDone == true && www.isNetworkError == true))
        if ((www.isHttpError == true && (www.responseCode == 404 || www.responseCode == 500)) || www.isNetworkError == true)
        {
            Debug.LogErrorFormat("[Network] {0} Error Code : {1}", www.uri, www.responseCode);
            www.Dispose();
            www = null;
            yield break;
        }

        // 응답 정보를 넣는다.
        Debug.Log(www.downloadHandler.text);

        // 통신 해제
        www.Dispose();
        www = null;
        //_retryPacket.Reset();
    }
}
