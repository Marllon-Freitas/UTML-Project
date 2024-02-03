using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Options_UI : MonoBehaviour
{
    [SerializeField]
    private string sceneName = "MainMenu";

    [SerializeField]
    private GameObject backToMainMenuButton;

    [SerializeField]
    FadeScreen_UI fadeScreen;

    public void BackToMainMenu()
    {
        StartCoroutine(LoadSceneWithFadeIn(1.5f));
    }

    public IEnumerator LoadSceneWithFadeIn(float _delay)
    {
        fadeScreen.FadeIn();
        yield return new WaitForSeconds(_delay);
        SceneManager.LoadScene(sceneName);
    }
}
