using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

public class DownloadTextures : MonoBehaviour {
    private const string SERVER_URL = "http://superkuma.net/storage/textures/";
    private const string API_SERVER_URL = "http://superkuma.net/api/json";
    private HandGesture handGesture;
    private string jsontext = "";
    private List<Love> loves;
    private SpriteRenderer spriteRenderer;
    private TextMesh messageTextMesh;

    private int count = 0;

    // Use this for initialization
    void Start() {
        loves = new List<Love>();
        spriteRenderer = GameObject.FindGameObjectWithTag("target").GetComponent<SpriteRenderer>();
        messageTextMesh = GameObject.FindGameObjectWithTag("message").GetComponent<TextMesh>();
        handGesture = GetComponent<HandGesture>();
        handGesture.OnSwipeRight.ThrottleFirst(TimeSpan.FromMilliseconds(800)).Subscribe(s => {
            Debug.Log("右");
            TextureChange(1);
        });
        handGesture.OnSwipeLeft.ThrottleFirst(TimeSpan.FromMilliseconds(800)).Subscribe(s => {
            Debug.Log("左");
            TextureChange(-1);
        });
        StartCoroutine(GetJson());
    }

    IEnumerator GetJson() {
        using (var www = UnityWebRequest.Get(API_SERVER_URL)) {
            yield return www.SendWebRequest();
            if (www.responseCode != 200 && string.IsNullOrEmpty(www.error)) {
                Debug.Log("error");
            } else {
                Debug.Log(www.downloadHandler.text);
                jsontext = www.downloadHandler.text;
                Observable.Create<string>(observer => {
                        observer.OnNext(jsontext);
                        observer.OnCompleted();
                        return Disposable.Empty;
                    }).Select(text => new JSONObject(text)).SelectMany(jsonList => jsonList.list).Select(CreateLove)
                    .Where(love => love.MediaType != Love.MediaTypeEnum.NONE)
                    .Subscribe(love => {
                        loves.Add(love);
                        TextureChange(0);
                    });
            }
        }
    }

    public void TextureChange(int index) {
        StartCoroutine(GetImage(index));
    }

    IEnumerator GetImage(int index) {
        count += index;

        if (count < 0) {
            count = 0;
        }
        if (count >= loves.Count) {
            count = loves.Count - 1;
        }

        var url = SERVER_URL + loves[count].MediaName;

        using (var www = UnityWebRequest.Get(url)) {
            yield return www.SendWebRequest();

            if (www.responseCode != 200 && string.IsNullOrEmpty(www.error)) {
                Debug.Log("error");
            } else {
//                sprite =
//                    Utilities.GetSpriteFromTexture2D(Utilities.GetTexture2DFromBytes(www.downloadHandler.data));
                var texture = new Texture2D(0,0);
                texture.LoadImage(www.downloadHandler.data);
                var sprite = Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),new Vector2(0.5f,0.5f) );
                spriteRenderer.sprite = sprite;
                spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                var sizeX = sprite.bounds.size.x;
                var sizeY = sprite.bounds.size.y;
                var scaleX = 1.0f / sizeX;
                var scaleY = 1.0f / sizeY;
                var scale = scaleX > scaleY ? scaleX : scaleY;
                GameObject.FindGameObjectWithTag("target").transform.localScale = new Vector3(scale, scale, 1.0f);
                messageTextMesh.text = loves[count].Message;
            }
        }
    }

    public Love CreateLove(JSONObject json) {
        var id = json.GetField("id").str;
        var username = json.GetField("name").str;
        var message = json.GetField("message").str;
        var mediaName = json.GetField("media_name").str;
        var mediaType = json.GetField("media_type").str;

        var love = new Love(id, username, message, mediaName, mediaType);
        return love;
    }
}