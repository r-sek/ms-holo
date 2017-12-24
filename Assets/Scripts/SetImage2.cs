using System;
using UniRx;
using UnityEngine;

public class SetImage2 : MonoBehaviour {
    private int count;
    private string[] mes = {"幸せ", "大好き！", "ハート", "愛してる", "１周年"};
    public SpriteRenderer spriteRenderer;
    public TextMesh messageTextMesh;

    private HandGesture handGesture;

    // Use this for initialization
    void Start() {
        Texturechange(0);
        handGesture = GetComponent<HandGesture>();
        handGesture.OnSwipeRight
            .ThrottleFirst(TimeSpan.FromMilliseconds(800))
            .Subscribe(s => {
                Debug.Log("右");
                Texturechange(1);
            });
        handGesture.OnSwipeLeft
            .ThrottleFirst(TimeSpan.FromMilliseconds(800))
            .Subscribe(s => {
                Debug.Log("左");
                Texturechange(-1);
            });
    }

    // Update is called once per frame
    void Update() { }

    public void Texturechange(int index) {
        count += index;
        if (count > 4) {
            count = 4;
        } else if (count < 0) {
            count = 0;
        }
        spriteRenderer.sprite = Resources.Load("ImageTarget4/" + count, typeof(Sprite)) as Sprite;
        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        messageTextMesh.text = mes[count];
        count++;
    }
}