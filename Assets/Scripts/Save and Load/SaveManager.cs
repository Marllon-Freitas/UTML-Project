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

    [SerializeField]
    private bool encryptData;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    [ContextMenu("Delete Save File")]
    public void DeleteSaveData()
    {
        dataHandle = new FileDataHandle(Application.persistentDataPath, fileName, encryptData);
        dataHandle.Delete();
    }

    private void Start()
    {
        dataHandle = new FileDataHandle(Application.persistentDataPath, fileName, encryptData);
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
        if (this.gameData == null)
        {
            NewGame();
        }

        foreach (ISaveManager saveManager in saveManagers)
        {
            saveManager.LoadData(gameData);
            Debug.Log("Loaded Curreny: " + gameData.skillTree);
        }
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

    public bool HasSaveData()
    {
        Debug.Log("Checking for save data..." + dataHandle.Load());
        if (dataHandle.Load() != null)
        {
            return true;
        }

        return false;
    }
}
