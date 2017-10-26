using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Linq;
using UnityEngine;
using System.IO;

public class DownloadImages : MonoBehaviour {
    //private string path = "Assets/Resources/textures/ai.jpg";
    // Use this for initialization
    // index
    private string cae = "";

    private const string SERVER_URL = "http://superkuma.net/textures/";
    private const string PICTURE_URL = "http://superkuma.net/DB";
    private List<string> messages;

    void Start() {
        StartCoroutine(GetFile());
    }

    // Update is called once per frame
    void Update() {
    }

    public Texture2D readByBinary(byte[] bytes) {
        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(bytes);
        return texture;
    }

    IEnumerator GetFile() {
        messages = new List<string>();
        var img = new List<string>();
        using (var www = new UnityWebRequest(PICTURE_URL)) {
            yield return www.Send();
            if (www.isNetworkError) {
            }
            else {
                var jo = new JSONObject(www.downloadHandler.text);
                for (var i = 0; i < jo.Count; i++) {
                    var json = jo[i];
                    var jmsg = json.GetField("message");
                    var jimg = json.GetField("file");
                    messages.Add(jmsg.str);
                    img.Add(jimg.str);
                }
            }
        }
        
        for (var i = 0; i < img.Count; i++) {
            var path = SERVER_URL + img[i];
            
            using (var www = UnityWebRequest.Get(path)) {
                yield return www.Send();
                if (www.isNetworkError) {
                }
                else {
                    cae = Application.temporaryCachePath;
                    File.WriteAllBytes(cae + "/" + i + ".png", www.downloadHandler.data);
                }
            }
        }
    }

    public string getMessage(int i) {
        return messages[i];
    }

    public int getMaxrange() {
        return messages.Count();
    }
}