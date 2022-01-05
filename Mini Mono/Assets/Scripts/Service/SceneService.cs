using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneService : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private float sceneTime;
    [SerializeField] private bool changeScene;

    // Start is called before the first frame update
    void Start()
    {
        CallChangeScene();
    }
    public void CallChangeScene()
    {
        changeScene = true;
        StartCoroutine(ChangeScene());
    }
    IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(sceneTime);
        SceneManager.LoadScene(sceneName);
    }
}
