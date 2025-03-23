using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAIMelee : MonoBehaviour, IEnemyAI
{
    [Header("General")]
    [SerializeField] private float _speed;
    [SerializeField] private Slider healthBar;
    [SerializeField] private ParticleSystem deathEffect;
    [SerializeField] private TextMeshProUGUI damageNumbers;
    [SerializeField] private NavMeshAgent agent;

    [Header("Combat")]
    public float attackRange;
    public float attackCD;
    [SerializeField] private GameObject attackHitbox;
    [SerializeField] private ShiftColorForDMG _damageTakeEffect;
    
    [Header("Death")]
    [SerializeField] private GameObject riggedVersion;
    [SerializeField] private GameObject zombieMesh;
    [SerializeField] private Rigidbody riggedVersionRB;
    [SerializeField] private float forceToApply = 40f;

    [HideInInspector]
    public float currentHealth;

    private Collider _collider;
    private Animator animator;
    private Rigidbody rb;

    private Vector3 damageNumberStartPos;
    private Vector3 textSpeed = new Vector3(0, 3f, 0);
    private bool isAlive;
    private float attackCounter = 0;
    private int movementHash;
    private float _damage;
    private float _maxHealth;
    private int choosingPathFrame;
    private float _projectileSpeed;

    public float damage { get => _damage; set => _damage = value; }
    public float maxHealth { get => _maxHealth; set => _maxHealth = value; }
    public float health { get => currentHealth; set => currentHealth = value; }
    public float speed { get => _speed; set => _speed = value; }
    private void Start()
    {
        movementHash = Animator.StringToHash("isMoving");
        attackHitbox.SetActive(false);
        currentHealth = maxHealth;
        isAlive = true;
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        _collider = GetComponent<Collider>();
        damageNumberStartPos = damageNumbers.transform.localPosition;
        damageNumbers.gameObject.SetActive(false);
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
        choosingPathFrame = Random.Range(10, 31);
        agent.speed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive)
            return;

        UpdateDamageTextPos();

        if(Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), PlayerSC.instance.transform.position) <= attackRange && attackCounter >= attackCD)
        {
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
            animator.SetBool(movementHash, false);
            attackCounter = 0f;
            animator.SetTrigger("Attack");
            return;
        }
        else
        {
            agent.isStopped = false;
            animator.SetBool(movementHash, true);
            //transform.LookAt(new Vector3(PlayerSC.instance.transform.position.x, transform.position.y, PlayerSC.instance.transform.position.z));
            if (Time.frameCount % choosingPathFrame == 0) //Set Destination every x frames.
                agent.SetDestination(PlayerSC.instance.transform.position);
        }
        attackCounter += Time.deltaTime;
    }

    public void AttackAnimEvent()
    {
        attackHitbox.SetActive(true);
    }

    public void EndAttackAnimEvent()
    {
        attackHitbox.SetActive(false);
    }

    public void GetHit(float incDamage, float explosionForce)
    {
        //_damageTakeEffect.StartShiftingColor();
        if(PlayerSC.instance.hasKnockback)
        {
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
            rb.AddExplosionForce(explosionForce, transform.forward, 1);
            StartCoroutine(ResetSpeed());
        }

        currentHealth -= incDamage;
        healthBar.value = currentHealth;
        damageNumbers.gameObject.SetActive(true);
        damageNumbers.transform.localPosition = damageNumberStartPos;
        if(incDamage == PlayerSC.instance.damage * PlayerSC.instance.critDamageMultiplier)
        {
            damageNumbers.text = "<color=#F6440B>" + incDamage.ToString("0.0") + "</color>";
            PlayerSC.instance.critHitSFX.Play();
        }
        else
            damageNumbers.text = incDamage.ToString("0.0");
        StartCoroutine(CloseUICoroutine());
        if(currentHealth <= 0 && isAlive)
        {
            Destroy(rb);
            isAlive = false;
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            agent.enabled = false;
            healthBar.gameObject.SetActive(false);
            Destroy(_collider);
            StartCoroutine(DeathCoroutine());
        }
    }

    private void UpdateDamageTextPos()
    {
        damageNumbers.transform.position += textSpeed * Time.deltaTime;
    }

    IEnumerator DeathCoroutine()
    {
        Destroy(attackHitbox);
        //animator.SetTrigger("Die");
        Destroy(_collider);
        Destroy(animator);
        Destroy(zombieMesh);
        _damageTakeEffect.enabled = false;
        Destroy(agent);
        Destroy(rb);
        riggedVersion.SetActive(true);
        //yield return new WaitForSeconds(0.2f);
        riggedVersionRB.AddForce(riggedVersion.transform.forward * forceToApply, ForceMode.Impulse);
        deathEffect.Play();
        yield return new WaitForSeconds(2f);
        GameManager.instance.UpdateWave();
        for(int i = 0; i < Random.Range(1, 6); i++) //Adds exp randomly.
        {
            PlayerSC.instance.exp += GameManager.instance.expValue;
        }
        ExpUI_Manager.instance.characterExpUI.value = PlayerSC.instance.exp;
        ExpUI_Manager.instance.playerText.text = PlayerSC.instance.exp.ToString("0") + " / " + PlayerSC.instance.expThreshold;
        if (PlayerSC.instance.exp >= PlayerSC.instance.expThreshold)
        {
            PlayerSC.instance.LevelUP();
        }
        Destroy(gameObject);
    }

    IEnumerator CloseUICoroutine()
    {
        yield return new WaitForSeconds(1f);
        damageNumbers.gameObject.SetActive(false);
    }

    IEnumerator ResetSpeed()
    {
        yield return new WaitForSeconds(1f);
        agent.isStopped = false;
        agent.speed = _speed;
    }
}
