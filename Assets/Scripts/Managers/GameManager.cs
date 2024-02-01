using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISaveManager
{
    public static GameManager instance;

    [SerializeField]
    private Checkpoint[] checkpoints;

    [SerializeField]
    private string closestCheckpointId;
    private Transform player;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    private void Start()
    {
        checkpoints = FindObjectsOfType<Checkpoint>();

        player = PlayerManager.instance.player.transform;
    }

    public void RestartScene()
    {
        SaveManager.instance.SaveGame();
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void LoadData(GameData _data)
    {
        foreach (KeyValuePair<string, bool> pair in _data.checkpoints)
        {
            foreach (Checkpoint checkpoint in checkpoints)
            {
                if (checkpoint.checkpointId == pair.Key && pair.Value == true)
                {
                    checkpoint.ActivateCheckpoint();
                }
            }
        }

        closestCheckpointId = _data.closestCheckpointId;
        Invoke("PlacePlayerAtClosestCheckpoint", 0.1f);
    }

    private void PlacePlayerAtClosestCheckpoint()
    {
        foreach (Checkpoint checkpoint in checkpoints)
        {
            if (checkpoint.checkpointId == closestCheckpointId)
                PlayerManager.instance.player.transform.position = checkpoint.transform.position;
        }
    }

    public void SaveData(ref GameData _data)
    {
        _data.closestCheckpointId = FindClosestCheckpoint().checkpointId;
        _data.checkpoints.Clear();

        foreach (Checkpoint checkpoint in checkpoints)
        {
            _data.checkpoints.Add(checkpoint.checkpointId, checkpoint.isCheckpointActive);
        }
    }

    private Checkpoint FindClosestCheckpoint()
    {
        float closestDistance = Mathf.Infinity;
        Checkpoint closestCheckpoint = null;

        foreach (Checkpoint checkpoint in checkpoints)
        {
            float distance = Vector2.Distance(
                PlayerManager.instance.player.transform.position,
                checkpoint.transform.position
            );
            if (distance < closestDistance && checkpoint.isCheckpointActive == true)
            {
                closestDistance = distance;
                closestCheckpoint = checkpoint;
            }
        }

        return closestCheckpoint;
    }
}
