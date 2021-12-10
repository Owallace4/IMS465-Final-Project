using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetPalette : MonoBehaviour
{
    // enemy prefabs 
    public GameObject[] enemyPrefabs = new GameObject[2];

    public enum EnemyList { MushroomMan, FlyingEye };
    public EnemyList enemyList;
   
    // item prefabs 
    public GameObject[] itemPrefabs = new GameObject[2];

    public enum ItemList { LifeEnergyBig, LifeEnergySmall }; 
    
}
