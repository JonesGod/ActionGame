using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartLoadingGame : MonoBehaviour
{
    public GameObject loadingScreen;
    public Slider slider;
    public Text loadText;
    
    public void LoadLevel(string sceneName)
    {
        loadingScreen.SetActive(true);
        StartCoroutine(DisplayLoadingScreen(sceneName));
    }
    IEnumerator DisplayLoadingScreen (string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);        

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            
            Debug.Log(progress);

            loadText.text = (progress * 100.0f) + "%";

            slider.value = progress;
            yield return null;
        }
    }
}