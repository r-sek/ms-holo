using UnityEngine;
using UnityEngine.SceneManagement;

public class SpeechManager : MonoBehaviour {

    public void GoMain() {
        SceneManager.LoadScene("MainView");
    }

    public void Warp() {
        SceneManager.LoadScene("SphereScene");
    }   
}