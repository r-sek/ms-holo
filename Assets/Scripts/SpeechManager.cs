using UnityEngine;
using UnityEngine.SceneManagement;

public class SpeechManager : MonoBehaviour {
    public void Back() {
        SceneManager.LoadScene("SphereScene");
    }

    public void GoMain() {
        SceneManager.LoadScene("MainView");
    }
}