using HoloToolkit.Unity.InputModule;
using UniRx;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(HandGesture))]
public class AirTap : MonoBehaviour {
    public Camera HololensCamara;
    public VideoPlayer videoPlayer;
    private Object[] videoClips;
    private int index = 0;
    private HandGesture handGesture;

    void OnEnable() {
        InputManager.Instance.AddGlobalListener(gameObject);
        handGesture = GetComponent<HandGesture>();
        handGesture.OnAirTap
            .Subscribe(_ => {
                Play();
                Debug.Log("air tap");
            });

        handGesture.OnAirDoubleTap
            .Subscribe(_ => {
                ChangeVideo();
                Debug.Log("double tap");
            });
    }

    void Start() {
        videoClips = Resources.LoadAll("Movies", typeof(VideoClip));
        //AirTapを検出したとき、OnInputClickedが呼ばれる。
    }

    void Update() { }


    public void Play() {
        if (videoPlayer.isPlaying) {
            videoPlayer.Pause();
        } else {
            videoPlayer.Play();
        }
    }

    public void ChangeVideo() {
        videoPlayer.Stop();
        // resourceの切り替え
        videoPlayer.clip = videoClips[index % 2] as VideoClip;
        index++;
    }
}