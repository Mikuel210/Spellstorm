using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : Singleton<GameManager> {

    public int Points { get; private set; }
    
    [SerializeField] private List<UpgradeSO> fireUpgrades;
    [SerializeField] private List<UpgradeSO> iceUpgrades;
    [SerializeField] private List<UpgradeSO> lightningUpgrades;
    public Dictionary<Spell, List<UpgradeSO>> SpellUpgrades { get; private set; } = new();
    
    [field: Space, SerializeField] public List<Wave> Waves { get; private set; }
    
    [Serializable]
    public class Burst
    {
        public GameObject prefab;
        public int amount = 1;
        public float delay = 1;
        public float initialDelay;

        public float GetDuration() => initialDelay + delay * amount;
    }

    [Serializable]
    public class Wave
    {
        public List<Burst> bursts;
        
        public float GetDuration() => bursts.Max(e => e.GetDuration());
    }
    
    public int CurrentWave { get; private set; }
    public bool Spawning { get; private set; }

    public float Time { get; private set; }
    public float Interval { get; private set; } = 10;
    public float CurrentWaveDuration { get; private set; }

    void Awake() {
        SpellUpgrades.Add(PlayerController.Instance.FireSpell, fireUpgrades);
        SpellUpgrades.Add(PlayerController.Instance.IceSpell, iceUpgrades);
        SpellUpgrades.Add(PlayerController.Instance.LightningSpell, lightningUpgrades);
    }

    private bool _won;
    
    void Update() {
        UpdateWaves();

        if (CurrentWave == Waves.Count
            && GameObject.FindGameObjectsWithTag("Enemy").Length == 0
            && Time > 1) {

            _won = true;
            UIManager.Instance.Win();
        }
            
    }

    void UpdateWaves() {
        if (_won) return;
        
        Time += UnityEngine.Time.deltaTime;

        if (Interval + CurrentWaveDuration > Time) return;
        
        Spawning = true;
        
        CurrentWave++;
        Wave currentWave = Waves[CurrentWave - 1];
        StartCoroutine(SpawnWave(currentWave));
        
        CurrentWaveDuration = currentWave.GetDuration();
        Time = 0;
    }
    
    private IEnumerator SpawnWave(Wave wave) {
        float distance = 20f;
        
        foreach (Burst burst in wave.bursts)
            StartCoroutine(SpawnBurst(burst, Vector2.zero, distance));

        yield return new WaitForSeconds(wave.GetDuration());

        Spawning = false;
    }
    
    private IEnumerator SpawnBurst(Burst burst, Vector2 center, float distance)
    {
        yield return new WaitForSeconds(burst.initialDelay);
        
        for (int i = 0; i < burst.amount; i++)
        {
            int angle = UnityEngine.Random.Range(0, 360);
            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * distance;
            float y = Mathf.Sin(angle * Mathf.Deg2Rad) * distance;

            Vector3 spawnPosition = new Vector2(x, y) + center;
            
            Instantiate(burst.prefab, spawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(burst.delay);
        }
    }
    
    public void EarnPoints(int points) => Points += points;
    
    public void SpendPoints(int points) => Points -= points;

}
