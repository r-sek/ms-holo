using System;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(HandGesture))]
public class SwipeGesture : MonoBehaviour {
    private SpriteRenderer spriteRenderer;

    private HandGesture handGesture;

    // Use this for initialization
    void Start() {
        handGesture = GetComponent<HandGesture>();
        handGesture.OnSwipeRight
            .ThrottleFirst(TimeSpan.FromMilliseconds(800))
            .Subscribe(s => { Debug.Log("右"); });
        handGesture.OnSwipeLeft
            .ThrottleFirst(TimeSpan.FromMilliseconds(800))
            .Subscribe(s => { Debug.Log("左"); });
//		handGesture.OnSwipeUp
//			.Subscribe(s=> { Debug.Log("上");});		
//		handGesture.OnSwipeDown
//			.Subscribe(s=> { Debug.Log("下");});
    }

    // Update is called once per frame
    void Update() { }

    public void textureChange() { }
}