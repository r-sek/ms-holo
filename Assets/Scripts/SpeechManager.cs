using UnityEngine;
using UnityEngine.SceneManagement;

public class SpeechManager : MonoBehaviour {
    public void Back() {
        SceneManager.LoadScene("MainView");
    }
}