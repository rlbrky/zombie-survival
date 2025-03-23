using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpCardManager : MonoBehaviour
{
    public static PowerUpCardManager instance { get; private set; }
    
    [Header("EFFECT VALUES")]
    public float maxHP;
    public float speedMultiplier;
    public float damageMultiplier;
    public float critChanceMultiplier;
    public float critDamageMultiplier;
    public float attackSpeedMultiplier;
    public float xpGainMultiplier;
    public float reloadSpeedMultiplier;

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

    private void Start()
    {
        foreach(var card in ExpUI_Manager.instance.powerUpCards)
        {
            var cardSC = card.GetComponent<PowerUpCardSC>();
            cardSC.speedMultiplier = speedMultiplier;
            cardSC.damageMultiplier = damageMultiplier;
            cardSC.critChanceMultiplier = critChanceMultiplier;
            cardSC.critDamageMultiplier = critDamageMultiplier;
            cardSC.attackSpeedMultiplier = attackSpeedMultiplier;
            cardSC.reloadSpeedMultiplier = reloadSpeedMultiplier;
            cardSC.maxHP = maxHP;
            
            cardSC.xpGainMultiplier = xpGainMultiplier;
        }

    }

    public void CreateCards_LevelUp()
    {
        var script1 = ExpUI_Manager.instance.powerUpCards[0].GetComponent<PowerUpCardSC>();
        script1.SetRandomWeapon();
        script1.gameObject.SetActive(true);
        for(int i = 1; i < 3; i++)//First card will always be a weapon, so the other 2 should be upgrades.
        {
            var script = ExpUI_Manager.instance.powerUpCards[i].GetComponent<PowerUpCardSC>();
            script.SetUpCard();
            script.gameObject.SetActive(true);
        }
    }
}
