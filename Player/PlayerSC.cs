using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;
using System;
using Cinemachine;
using TMPro;

public class PlayerSC : MonoBehaviour
{
    public static PlayerSC instance {  get; private set; }
    [Header("Necessary Objs")] 
    public ParticleSystem muzzleFlash;
    public Transform weaponHoldPoint;
    public GameObject currentWeapon;
    public GameObject camLookTarget;
    public GameObject riggedVersion;
    public GameObject playerMesh;
    public Rigidbody riggedVersionRB;
    public GameObject playerCanvas;
    public Collider _collider;
    public Slider healthbar;
    public ShiftColorForDMG takeDamageEffect;
    public AudioSource getHitBody;
    public AudioSource getHitVocal;
    public AudioSource critHitSFX;
    
    private Animator animator;

    [Header("Attacking")]
    public AudioSource firingSFX;
    public AudioSource reloadSFX;
    public CinemachineImpulseSource firingImpulseSource;

    [Header("Ammo")]
    [SerializeField] private Transform firingPoint;
    [SerializeField] public GameObject _bullet;

    private float firingCounter = 0f;
    
    [Header("Stats")]
    public float maxHealth;
    public float damage;
    public float critChance = 0.1f;
    public float critDamageMultiplier = 1.5f;
    public float speed = 5f;
    public float knockbackForce;
    
    [Header("Powerup Stats")]
    public float exp; //Controls character level.
    public float expThreshold = 200f; //Will scale.

    public bool isLifeStealing;
    public bool hasKnockback;

    [Header("Weapon System")] 
    public List<Weapon> weapons;
    public float fireRate = 0.7f;
    public float reloadTime;
    public int maxAmmo;
    private int _currentAmmo;
    private bool _isReloading;
    
    [Header("Weapon UI")]
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private Slider reloadProgressSlider;
    
    //For animations
    private int movementHash;
    //private int movementHashY;

    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public float health;
    [HideInInspector] public bool isDead;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        movementHash = Animator.StringToHash("isMoving");
        //movementHashY = Animator.StringToHash("movY");
        health = maxHealth;
        rb = GetComponent<Rigidbody>();
        exp = 0;

        healthbar.maxValue = maxHealth;
        healthbar.value = health;

        Time.timeScale = 1;
        _currentAmmo = maxAmmo;
        reloadProgressSlider.gameObject.SetActive(false);
        ApplyWeaponStats(0); //Player always starts the game with glock.
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;
        //Movement();
        firingCounter += Time.deltaTime;
        ammoText.text = _currentAmmo.ToString();
    }

    public void Attack()
    {
        if (_isReloading) return;
        
        if (firingCounter >= fireRate && _currentAmmo > 0)
        {
            firingCounter = 0f;
            var bullet = Instantiate(_bullet, firingPoint.position, Quaternion.identity);
            bullet.transform.forward = transform.forward;
            _currentAmmo--;
            muzzleFlash.Play();
            firingSFX.Play();
            firingImpulseSource.GenerateImpulse();
        }
        else if (_currentAmmo <= 0)
        {
            StartCoroutine(ReloadRoutine());
        }
    }
    
    public void Movement(Vector2 movementInput)
    {
        if (movementInput != Vector2.zero)
            animator.SetBool(movementHash, true);
        else
            animator.SetBool(movementHash, false);

        Vector3 result = new Vector3(movementInput.x, 0, movementInput.y);

        rb.velocity = result * speed;
    }

    public void LevelUP()
    {
        Time.timeScale = 0f;
        exp = 0;
        expThreshold += 150f;
        PowerUpCardManager.instance.CreateCards_LevelUp();
        maxHealth *= 1.1f; //Gain 10 percent max health. But don't heal.
        healthbar.maxValue = maxHealth;
        healthbar.value = health;
        //health += maxHealth / 10;
        ExpUI_Manager.instance.characterExpUI.maxValue = expThreshold;
        ExpUI_Manager.instance.playerText.text = exp + " / " + expThreshold;
    }

    public void TakeDamage(float incDamage)
    {
        if(!isDead)
        {
            getHitBody.Play();
            getHitVocal.Play();
            takeDamageEffect.StartShiftingColor();
            health -= incDamage;
            healthbar.value = health;
            if (health <= 0f)
            {
                Die();
            }
        }
    }

    public void Die()
    {
        //Play SFX and maybe VFX.
        if (PlayerTouchInputs.instance != null)
        {
            PlayerTouchInputs.instance.DisableTouchStuff();
            PlayerTouchInputs.instance.enabled = false;
        }
        Destroy(takeDamageEffect);
        Destroy(playerCanvas);
        isDead = true;
        Destroy(_collider);
        Destroy(animator);
        Destroy(playerMesh);
        riggedVersion.SetActive(true);
        riggedVersionRB.AddForce(transform.forward * 14, ForceMode.Impulse);
        Destroy(rb);
        StartCoroutine(DeathUICoroutine());
    }

    public void ApplyWeaponStats(int weaponHash)
    {
        if (weaponHash < 2) //Use pistol anims
        {
            animator.SetLayerWeight(0, 1);
            animator.SetLayerWeight(1, 0);
        }
        else //Use rifle anims
        {
            animator.SetLayerWeight(0, 0);
            animator.SetLayerWeight(1, 1);
        }
        Weapon weaponObj = weapons[weaponHash];
        Destroy(currentWeapon); //Destroy previous weapon and apply new one below.
        currentWeapon = Instantiate(weaponObj.weaponPrefab, weaponHoldPoint.position, Quaternion.identity, weaponHoldPoint);
        currentWeapon.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        currentWeapon.transform.localRotation = Quaternion.Euler(0, 0, 0);
        if (weaponHash == 4) //Check if it is shotgun
            hasKnockback = true;
        else
            hasKnockback = false;
        //Set new stats from the new weapon.
        damage = weaponObj.damage;
        fireRate = weaponObj.fireRate;
        reloadTime = weaponObj.reloadTime;
        maxAmmo = weaponObj.ammo;
        _currentAmmo = weaponObj.ammo;
        firingSFX.clip = weaponObj.fireSFX;
        reloadSFX.clip = weaponObj.reloadSFX;
        
        _bullet = weaponObj.ammoPrefab; //Change the bullet it fires.
    }
    
    IEnumerator ReloadRoutine()
    {
        reloadSFX.Play();
        reloadProgressSlider.gameObject.SetActive(true);
        _isReloading = true;
        float timer = 0f;
        reloadProgressSlider.maxValue = reloadTime;
        reloadProgressSlider.value = 0;
        while (timer < reloadTime)
        {
            timer += Time.deltaTime;
            reloadProgressSlider.value = timer;
            yield return null;
        }
        
        _isReloading = false;
        _currentAmmo = maxAmmo;
        reloadProgressSlider.gameObject.SetActive(false);
    }
    
    IEnumerator DeathUICoroutine()
    {
        yield return new WaitForSeconds(1.5f);
        Time.timeScale = 0;
        AdManager.instance.ShowInterstitialAd();
        ExpUI_Manager.instance.ActivateDeathUI();
    }
}
