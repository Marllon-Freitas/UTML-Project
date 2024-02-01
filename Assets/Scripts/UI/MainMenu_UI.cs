using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu_UI : MonoBehaviour
{
    [SerializeField]
    private string sceneName = "MainScene";

    [SerializeField]
    private GameObject continueButton;

    [SerializeField]
    FadeScreen_UI fadeScreen;

    private void Start()
    {
        if (SaveManager.instance.HasSaveData() == false)
            continueButton.SetActive(false);
    }

    public void ContinueGame()
    {
        StartCoroutine(LoadSceneWithFade(1.5f));
    }

    public void NewGame()
    {
        SaveManager.instance.DeleteSaveData();
        StartCoroutine(LoadSceneWithFade(1.5f));
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        // Application.Quit();
    }

    IEnumerator LoadSceneWithFade(float _delay)
    {
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(_delay);
        SceneManager.LoadScene(sceneName);
    }
}
