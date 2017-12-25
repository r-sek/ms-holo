using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

public class DownloadImages : MonoBehaviour {
    private const string SERVER_URL = "http://superkuma.net/storage/textures/";
    private const string API_SERVER_URL = "http://superkuma.net/api/json";
    private List<Love> loves;
    private int count;
    private SpriteRenderer spriteRenderer;
    private TextMesh messageTextMesh;
    private List<string> cacheImages;
    private string tempDir = "";
    private Texture2D texture;
    private Sprite sprite;
    private string jsontext = "";
    private HandGesture handGesture;

    void Start() {
        count = 0;
        loves = new List<Love>();
        texture = new Texture2D(0, 0);
        tempDir = Application.temporaryCachePath;
        cacheImages = new List<string>();
        spriteRenderer = GameObject.FindGameObjectWithTag("target").GetComponent<SpriteRenderer>();
        messageTextMesh = GameObject.FindGameObjectWithTag("message").GetComponent<TextMesh>();
        handGesture = GetComponent<HandGesture>();
        handGesture.OnSwipeRight.ThrottleFirst(TimeSpan.FromMilliseconds(800)).Subscribe(s => {
            Debug.Log("右");
            Texturechange(1);
        });
        handGesture.OnSwipeLeft.ThrottleFirst(TimeSpan.FromMilliseconds(800)).Subscribe(s => {
            Debug.Log("左");
            Texturechange(-1);
        });


        StartCoroutine(GetApi());

    }

// Update is called once per frame
    void Update() {
    }

    IEnumerator GetApi() {
        using (var www = UnityWebRequest.Get(API_SERVER_URL)) {
            yield return www.SendWebRequest();
            Debug.Log(www.responseCode);
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
                    .Subscribe(love => { StartCoroutine(Download(love)); });
            }
        }
        Texturechange(0);
    }

    IEnumerator Download(Love love) {
        var url = SERVER_URL + love.MediaName;
        var path = tempDir + "/" + love.MediaName;
        loves.Add(love);
        cacheImages.Add(path);
        Debug.unityLogger.Log("loves", url);
        Debug.unityLogger.Log("loves", loves.Count);
        Debug.unityLogger.Log("loves", cacheImages.Count);
        if (!File.Exists(path))
        {
            using (var image = UnityWebRequest.Get(url))
            {

                yield return image.SendWebRequest();

                Debug.Log(image.responseCode);
                if (image.responseCode != 200 && string.IsNullOrEmpty(image.error))
                {
                    Debug.Log("error");
                }
                else
                {
                    Debug.unityLogger.Log("pathpath", "pathpath");
                    File.WriteAllBytes(path, image.downloadHandler.data);
                }
            }
        }
    }


    public void Texturechange(int index) {
        Debug.unityLogger.Log("count", count);
        Debug.unityLogger.Log("cacheimages", cacheImages.Count);
        Debug.unityLogger.Log("loves", loves.Count);
        if (cacheImages.Count == 0) return;
        count += index;
        if (count > cacheImages.Count-1) {
            count = cacheImages.Count - 1;
            return;
        } else if (count < 0) {
            count = 0;
            return;
        }
        texture.LoadImage(Utilities.LoadbinaryBytes(cacheImages[count]));
        var x = -spriteRenderer.bounds.center.x / spriteRenderer.bounds.size.x + 0.5f;
        var y = -spriteRenderer.bounds.center.y / spriteRenderer.bounds.size.y + 0.5f;
        sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(x, y));
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