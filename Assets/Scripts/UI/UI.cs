using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour, ISaveManager
{
    [Header("End Screen")]
    [SerializeField]
    private FadeScreen_UI fadeScreen;

    [SerializeField]
    private GameObject endText;

    [SerializeField]
    private GameObject restartButton;

    [Space]
    [SerializeField]
    private GameObject characterUI;

    [SerializeField]
    private GameObject skillTreeUI;

    [SerializeField]
    private GameObject craftUI;

    [SerializeField]
    private GameObject optionsUI;

    [SerializeField]
    private GameObject inGameUi;

    [SerializeField]
    private VolumeSlider_UI[] volumeSlidersSettings;

    public SkillToolTip_UI skillToolTip;
    public ItemToolTip_UI itemToolTip;
    public StatToolTip_UI statToolTip;
    public CraftWindow_UI craftWindow;

    private void Awake()
    {
        SwitchTo(skillTreeUI);
        fadeScreen.gameObject.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        SwitchTo(inGameUi);

        itemToolTip.gameObject.SetActive(false);
        statToolTip.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            SwitchWithKeyTo(characterUI);

        if (Input.GetKeyDown(KeyCode.B))
            SwitchWithKeyTo(craftUI);

        if (Input.GetKeyDown(KeyCode.K))
            SwitchWithKeyTo(skillTreeUI);

        if (Input.GetKeyDown(KeyCode.O))
            SwitchWithKeyTo(optionsUI);
    }

    public void SwitchTo(GameObject _menu)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            bool isFadeScreen = transform.GetChild(i).GetComponent<FadeScreen_UI>() != null;
            if (isFadeScreen == false)
                transform.GetChild(i).gameObject.SetActive(false);
        }

        if (_menu != null)
        {
            if (AudioManager.instance != null)
                AudioManager.instance.PlaySoundEffect(48, null);
            _menu.SetActive(true);
        }

        if (GameManager.instance != null)
        {
            if (_menu == inGameUi)
                GameManager.instance.PauseGame(false);
            else
                GameManager.instance.PauseGame(true);
        }
    }

    public void SwitchWithKeyTo(GameObject _menu)
    {
        if (_menu != null && _menu.activeSelf)
        {
            _menu.SetActive(false);
            CheckForInGameUI();
            return;
        }

        SwitchTo(_menu);
    }

    private void CheckForInGameUI()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (
                transform.GetChild(i).gameObject.activeSelf
                && transform.GetChild(i).GetComponent<FadeScreen_UI>() == null
            )
                return;
        }

        SwitchTo(inGameUi);
    }

    public void SwitchOnEndScreen()
    {
        fadeScreen.FadeOut();
        StartCoroutine(EndScreenCoroutine());
    }

    IEnumerator EndScreenCoroutine()
    {
        yield return new WaitForSeconds(1.5f);
        endText.SetActive(true);
        yield return new WaitForSeconds(1.3f);
        restartButton.SetActive(true);
    }

    public void RestartGameButton() => GameManager.instance.RestartScene();

    public void LoadData(GameData _data)
    {
        foreach (KeyValuePair<string, float> pair in _data.volumeSettings)
        {
            foreach (VolumeSlider_UI item in volumeSlidersSettings)
            {
                if (item.parametr == pair.Key)
                    item.LoadSlider(pair.Value);
            }
        }
    }

    public void SaveData(ref GameData _data)
    {
        _data.volumeSettings.Clear();

        foreach (VolumeSlider_UI item in volumeSlidersSettings)
        {
            _data.volumeSettings.Add(item.parametr, item.slider.value);
        }
    }
}
