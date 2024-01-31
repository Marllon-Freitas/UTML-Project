using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    private GameData gameData;
    private List<ISaveManager> saveManagers;
    private FileDataHandle dataHandle;

    [SerializeField]
    private string fileName;

    private void Awake()
    {
        dataHandle = new FileDataHandle(Application.persistentDataPath, fileName);
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    private void Start()
    {
        saveManagers = FindAllSaveManagers();
        LoadGame();
    }

    public void NewGame()
    {
        gameData = new GameData();
    }

    public void SaveGame()
    {
        foreach (ISaveManager saveManager in saveManagers)
        {
            saveManager.SaveData(ref gameData);
        }
        dataHandle.Save(gameData);
    }

    public void LoadGame()
    {
        gameData = dataHandle.Load();
        // Load the game data
        if (this.gameData == null)
        {
            NewGame();
        }

        foreach (ISaveManager saveManager in saveManagers)
        {
            saveManager.LoadData(gameData);
        }
        Debug.Log("Loaded Curreny: " + gameData.currency);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<ISaveManager> FindAllSaveManagers()
    {
        IEnumerable<ISaveManager> saveManagers = FindObjectsOfType<MonoBehaviour>()
            .OfType<ISaveManager>();

        return new List<ISaveManager>(saveManagers);
    }
}
