using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandle
{
    private string dataDirPath = "";
    private string dataFileName = "";

    public FileDataHandle(string _dataDirPath, string _dataFileName)
    {
        dataDirPath = _dataDirPath;
        dataFileName = _dataFileName;
    }

    public void Save(GameData _data)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            string json = JsonUtility.ToJson(_data, true);
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(json);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving file: " + fullPath + "\n" + e.Message);
        }
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GameData loadData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                loadData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error loading file: " + fullPath + "\n" + e.Message);
            }
        }
        return loadData;
    }
}
