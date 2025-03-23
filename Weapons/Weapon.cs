using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Weapon",menuName = "Weapon")]
public class Weapon : ScriptableObject
{
    public Sprite image;
    public GameObject weaponPrefab;
    public GameObject ammoPrefab;
    public AudioClip fireSFX;
    public AudioClip reloadSFX;
    
    [Header("Stats")] 
    public float damage;
    public float fireRate;
    public float reloadTime;
    public int ammo;
}
