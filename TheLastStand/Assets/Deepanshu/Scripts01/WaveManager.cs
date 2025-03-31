using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class WaveManager : MonoBehaviour
{ 
    public static WaveManager Instance { get; private set; }

    [Header("Wave Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int totalEnemiesToSpawn = 10;
    [SerializeField] private int maxEnemiesInScene = 5;

    private int _totalEnemiesSpawned = 0;
    private int _currentAliveEnemies = 0;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI enemyCounterText;

    [Header("Player Reference")]
    [SerializeField] private Transform player;
    [SerializeField] private Camera playerCamera;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            return;
        }
        SpawnInitialEnemies();
        UpdateEnemyCounter();
    }
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    private void SpawnInitialEnemies()
    {
        for (int i = 0; i < maxEnemiesInScene && _totalEnemiesSpawned < totalEnemiesToSpawn; i++)
        {
            SpawnEnemy();
        }
    }
    private void SpawnEnemy()
    {
        if (_totalEnemiesSpawned >= totalEnemiesToSpawn || _currentAliveEnemies >= maxEnemiesInScene) return;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        EnemyBase enemyScript = newEnemy.GetComponent<EnemyBase>();
        if (enemyScript != null)
        {
            enemyScript.SetPlayer(player);
            enemyScript.SetCamera(playerCamera);
            enemyScript.OnDeath += OnEnemyDeath;
        }
        _totalEnemiesSpawned++;
        _currentAliveEnemies++;
        UpdateEnemyCounter();
    }
    public void OnEnemyDeath(EnemyBase enemy)
    {
        _currentAliveEnemies--;
        enemy.OnDeath -= OnEnemyDeath;

        if (_totalEnemiesSpawned < totalEnemiesToSpawn)
        {
            SpawnEnemy();
        }
        else if (_currentAliveEnemies == 0)
        {
            SceneManager.LoadScene(3);
        }
        UpdateEnemyCounter();
    }
    private void UpdateEnemyCounter()
    {
        if (enemyCounterText != null)
        {
            enemyCounterText.text = $"Enemies Remaining: {totalEnemiesToSpawn - _totalEnemiesSpawned + _currentAliveEnemies}";
        }
    }
    public Camera GetPlayerCamera()
    {
        return playerCamera;
    }
}
