using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour {

    public void NextScene(string levelName) {
        SceneManager.LoadScene(levelName);
    }
}
