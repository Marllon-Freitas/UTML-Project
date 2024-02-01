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

    [Header("Lost Souls")]
    [SerializeField]
    private GameObject lostSoulsPrefab;
    public int lostSoulsAmount;

    [SerializeField]
    private float lostSoulsXPosition;

    [SerializeField]
    private float lostSoulsYPosition;

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

    public void SaveData(ref GameData _data)
    {
        _data.lostSoulsXPosition = player.position.x;
        _data.lostSoulsYPosition = player.position.y;
        _data.lostSoulsAmount = lostSoulsAmount;

        if (FindClosestCheckpoint() != null)
            _data.closestCheckpointId = FindClosestCheckpoint().checkpointId;
        _data.checkpoints.Clear();

        foreach (Checkpoint checkpoint in checkpoints)
        {
            _data.checkpoints.Add(checkpoint.checkpointId, checkpoint.isCheckpointActive);
        }
    }

    public void LoadData(GameData _data) => StartCoroutine(LoadWithDelay(_data));

    private void LoadLostSouls(GameData _data)
    {
        lostSoulsAmount = _data.lostSoulsAmount;
        lostSoulsXPosition = _data.lostSoulsXPosition;
        lostSoulsYPosition = _data.lostSoulsYPosition;

        if (lostSoulsAmount > 0)
        {
            GameObject lostSouls = Instantiate(
                lostSoulsPrefab,
                new Vector3(lostSoulsXPosition, lostSoulsYPosition),
                Quaternion.identity
            );
            lostSouls.GetComponent<LostSoulsController>().currency = lostSoulsAmount;
        }

        lostSoulsAmount = 0;
    }

    private IEnumerator LoadWithDelay(GameData _data)
    {
        yield return new WaitForSeconds(0.1f);
        LoadCheckpoints(_data);
        PlacePlayerAtClosestCheckpoint(_data);
        LoadLostSouls(_data);
    }

    private void LoadCheckpoints(GameData _data)
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
    }

    private void PlacePlayerAtClosestCheckpoint(GameData _data)
    {
        if (_data.closestCheckpointId == null)
            return;

        closestCheckpointId = _data.closestCheckpointId;

        foreach (Checkpoint checkpoint in checkpoints)
        {
            if (checkpoint.checkpointId == closestCheckpointId)
                player.position = checkpoint.transform.position;
        }
    }

    private Checkpoint FindClosestCheckpoint()
    {
        float closestDistance = Mathf.Infinity;
        Checkpoint closestCheckpoint = null;

        foreach (Checkpoint checkpoint in checkpoints)
        {
            float distance = Vector2.Distance(player.position, checkpoint.transform.position);
            if (distance < closestDistance && checkpoint.isCheckpointActive == true)
            {
                closestDistance = distance;
                closestCheckpoint = checkpoint;
            }
        }

        return closestCheckpoint;
    }
}
