using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_EnemySpawner : MonoBehaviour
{
    [SerializeField, Tooltip("Type of enemy to spawn.")]
    private GameObject enemyPrefab;

    public GameObject SpawnEnemy()
    {
        return Instantiate(enemyPrefab, transform.position, Quaternion.identity);
    }
}
