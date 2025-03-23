using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] public GameObject prefabToSpawn;

    [Header("Variables")]
    [SerializeField] private float spawnRate = 1f;
    
    private float spawnCounter = 0;

    private float curMaxHealth = 12;
    private float curDamage = 10;
    private float curSpeed = 5;
    
    // Update is called once per frame
    void Update()
    {
        if(MassSpawnManager.instance.waveSpawnCount < MassSpawnManager.instance.waveMaxSpawns)
        {
            spawnCounter += Time.deltaTime;

            if (spawnCounter > spawnRate)
            {
                spawnCounter = 0;
                var spawnedEnemy = Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
                ApplyStats(spawnedEnemy.GetComponent<IEnemyAI>());
                MassSpawnManager.instance.waveSpawnCount++;
            }
        }
    }

    private void ApplyStats(IEnemyAI _enemy)
    {
        _enemy.maxHealth = curMaxHealth;
        _enemy.damage = curDamage;
        _enemy.speed = curSpeed;
    }
    
    public void UpdateEnemyStats()
    {
        curMaxHealth *= MassSpawnManager.instance.enemyHealthScale;
        curDamage *= MassSpawnManager.instance.enemyDamageScale;
        curSpeed *= MassSpawnManager.instance.enemySpeedScale;
    }
}
