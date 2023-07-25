using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameDirector : MonoBehaviour {
    public static GameDirector Instance;

    [Header("Time")]
    [SerializeField] [Min(1f)] private float time = 60f;
    [SerializeField] private Timer timer;

    [Header("Spawning")]
    [SerializeField] [Min(1f)] private float spawnRadius = 2f;
    [SerializeField] [Min(0f)] private float playerOffset = 3f;
    [SerializeField] [Min(1f)] private float difficulty = 1f;
    [SerializeField] [Min(0f)] private float baseSpawnCooldown = 1f;
    [SerializeField] [Min(0f)] private float spawnCooldownSubtract = 0.5f;
    [SerializeField] [Min(0f)] private int baseEnemyCount = 5;
    [SerializeField] [Min(0f)] private int additionalEnemies = 5;
    [SerializeField] private GameObject enemyPrefab;

    [Header("References")] 
    [SerializeField] private PlayerController player;
    [SerializeField] private Target target;
    [SerializeField] private UIManager uiManager;

    private readonly List<Enemy> _enemies = new();
    private float _startDist;
    private float _currentCooldown;

    private bool _playing;
    
    
    private void Awake() {
        if (Instance == null) Instance = this;
    }

    private void Start() {
        _startDist = DistPlayerToTarget();
    }

    private void Update() {
        if (!_playing) return;
        SpawnEnemies();
    }


    public void BeginCountdown() {
        timer.gameObject.SetActive(true);
        timer.Set(time, _ => Fail());

        _playing = true;
        player.Enable();
    }
    
    private void Fail() {
        uiManager.ShowLooseScreen();
        Finish();
    }

    public void Win() {
        uiManager.ShowWinScreen();
        Finish();
    }

    private void Finish() {
        _playing = false;
        timer.Cancel();
        timer.gameObject.SetActive(false);

        player.Disable();
        player.transform.position = Vector3.zero;

        foreach (Enemy enemy in _enemies) {
            Destroy(enemy.gameObject);
        }
        _enemies.Clear();
    }

    private void SpawnEnemies() {
        float difficultyScale = 1 - DistPlayerToTarget() / _startDist;

        float spawnCooldown = baseSpawnCooldown - difficultyScale * spawnCooldownSubtract;
        if (_currentCooldown < spawnCooldown) {
            _currentCooldown += Time.deltaTime;
            return;
        }
        
        int spawnCap = baseEnemyCount + (int) (difficultyScale * difficulty * additionalEnemies);
        if (_enemies.Count >= spawnCap) return;

        GameObject eObject = Instantiate(enemyPrefab, GetSpawnPosition(), Quaternion.identity);
        _enemies.Add(eObject.GetComponent<Enemy>());
        _currentCooldown = 0f;
    }

    private Vector2 GetSpawnPosition() {
        Vector2 spawnCenter = GetSpawnCenter();
        float x = Random.Range(spawnCenter.x - spawnRadius, spawnCenter.x + spawnRadius);
        float y = Random.Range(spawnCenter.y - spawnRadius, spawnCenter.y + spawnRadius);

        return new Vector2(x, y);
    }

    private Vector2 GetSpawnCenter() {
        Vector2 playerPos = player.transform.position;
        Vector2 toGoal = (Vector2) target.transform.position - playerPos;
        float dist = toGoal.magnitude;
        
        return playerPos + toGoal / dist * Math.Min(dist, playerOffset);
    }

    private float DistPlayerToTarget() {
        return (player.transform.position - target.transform.position).magnitude;
    }

    public void RemoveEnemy(Enemy enemy) {
        _enemies.Remove(enemy);
    }
    

    private void OnDrawGizmos() {
        if (player == null || target == null) return;

        Vector2 spawnCenter = GetSpawnCenter();
        float d = spawnRadius * 2;
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(spawnCenter,  new Vector3(d, d, d));
    }
}
