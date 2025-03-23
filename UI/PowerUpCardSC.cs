using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpCardSC : MonoBehaviour
{
    [Header("Weapon Images")] [SerializeField]
    private List<Image> weaponImages;
    
    [Header("Card Elements")]
    [SerializeField] Image cardHeaderImage;
    [SerializeField] TextMeshProUGUI cardHeaderText;
    [SerializeField] TextMeshProUGUI cardBodyText;
    [SerializeField] Image cardBodyImage;

    private int _cardEffect;
    private float _effectValue;

    public float speedMultiplier;
    public float damageMultiplier;
    public float critChanceMultiplier;
    public float critDamageMultiplier;
    public float attackSpeedMultiplier;
    public float reloadSpeedMultiplier;
    public float maxHP;

    //RARE POWERUPS.
    public float xpGainMultiplier;
    
    private float percentage;
    private int pickedWeaponIndex;

    private void OnEnable()
    {
        if (PlayerTouchInputs.instance != null)
        {
            PlayerTouchInputs.instance.DisableTouchStuff();
        }
    }

    private void OnDisable()
    {
        if (PlayerTouchInputs.instance != null)
        {
            PlayerTouchInputs.instance.EnableTouchStuff();
        }
    }

    public void SetUpCard()
    {
        Color _color;

        if(UnityEngine.ColorUtility.TryParseHtmlString("#640055", out _color))
            cardHeaderText.color = _color;

        int random = Random.Range(0, MassCardManager.instance.cardValues.Count);
        int switchHandler = MassCardManager.instance.cardValues[random];
        MassCardManager.instance.cardValues.Remove(switchHandler);
        switch (switchHandler)
        {
            case 0: //XP
                percentage = (xpGainMultiplier * 100) - 100;
                cardHeaderText.text = "TRAINED SOLDIER!";
                cardBodyText.text = "Get <color=#3f000f>%" + percentage.ToString("0.0") + "</color> more <color=#ADD8E6>xp</color> per drop.";
                _cardEffect = 7;
                _effectValue = xpGainMultiplier;
                break;
            case 1: //Damage
                percentage = (damageMultiplier * 100) - 100;
                cardHeaderText.text = "BRUTE!";
                cardBodyText.text = "Get <color=#3EB49B>%" + percentage.ToString("0.0") + "</color> <color=red>damage.</color>";
                _cardEffect = 1;
                _effectValue = damageMultiplier;
                break;
            case 2: //Life Steal
                cardHeaderText.text = "VAMPIRIC SOLDIER!";
                cardBodyText.text = "<color=red>Gain Life Steal!</color>";
                _cardEffect = 6;
                break;
            case 3: //Speed
                percentage = (speedMultiplier * 100) - 100;
                cardHeaderText.text = "TRAINED RUNNER!";
                cardBodyText.text = "Get <color=#3EB49B>%" + percentage.ToString("0.0") + "</color> <color=#00FFFF>movement speed.</color>";
                _cardEffect = 0;
                _effectValue = speedMultiplier;
                break;
            case 4: //Crit Chance
                percentage = (critChanceMultiplier * 100) - 100;
                cardHeaderText.text = "LUCKY SOLDIER!";
                cardBodyText.text = "Get <color=#3EB49B>%" + percentage.ToString("0.0") + "</color> <color=#FF00FF>crit chance.</color>";
                _cardEffect = 2;
                _effectValue = critChanceMultiplier;
                break;
            case 5: //Crit Damage
                percentage = (critDamageMultiplier * 100) - 100;
                cardHeaderText.text = "ENRAGED SOLDIER!";
                cardBodyText.text = "Get <color=#3EB49B>%" + percentage.ToString("0.0") + "</color> <color=orange>crit damage.</color>";
                _cardEffect = 3;
                _effectValue = critDamageMultiplier;
                break;
            case 6: //Attack Speed
                percentage = (attackSpeedMultiplier * 100) - 100;
                cardHeaderText.text = "ARMS MASTER!";
                cardBodyText.text = "Get <color=#3EB49B>%" + percentage.ToString("0.0") + "</color> <color=yellow>attack speed.</color>";
                _cardEffect = 4;
                _effectValue = attackSpeedMultiplier;
                break;
            case 7: //Max Health
                percentage = (maxHP * 100) - 100;
                cardHeaderText.text = "COMMANDO!";
                cardBodyText.text = "Get <color=#3EB49B>%" + percentage.ToString("0.0") + "</color> <color=green>health.</color> and <color=green>heal</color> for <color=#3EB49B>%10</color>";
                _cardEffect = 5;
                _effectValue = maxHP;
                break;
            case 8: //Reaload Speed
                percentage = (reloadSpeedMultiplier * 100) - 100;
                cardHeaderText.text = "FAST HANDS!";
                cardBodyText.text = "Reload your weapon <color=#1c3cff>%" + percentage.ToString("0.0") + "</color> faster.";
                _cardEffect = 8;
                _effectValue = reloadSpeedMultiplier;
                break;
        }
    }

    public void SetRandomWeapon()
    {
        int random = Random.Range(0, PlayerSC.instance.weapons.Count);
        pickedWeaponIndex = random;
        cardBodyImage.sprite = PlayerSC.instance.weapons[pickedWeaponIndex].image;
        
        switch (pickedWeaponIndex)
        {
            case 0:
                cardHeaderText.text = "Glock";
                break;
            case 1:
                cardHeaderText.text = "Desert Eagle";
                break;
            case 2:
                cardHeaderText.text = "M4";
                break;
            case 3:
                cardHeaderText.text = "FAMAS";
                break;
            case 4:
                cardHeaderText.text = "Shotgun";
                break;
        }
    }

    public void ApplyPickedWeapon()
    {
        MassCardManager.instance.ResetCards();
        Time.timeScale = 1f;
        ExpUI_Manager.instance.characterExpUI.value = 0;
        foreach (var card in ExpUI_Manager.instance.powerUpCards)
            card.gameObject.SetActive(false);
        
        PlayerSC.instance.ApplyWeaponStats(pickedWeaponIndex);
    }
    
    public void ApplyCardEffect()
    {
        MassCardManager.instance.ResetCards();
        Time.timeScale = 1f;
        //PlayerSC.instance.enabled = true;
        ExpUI_Manager.instance.characterExpUI.value = 0;
        foreach (var card in ExpUI_Manager.instance.powerUpCards)
            card.gameObject.SetActive(false);
        switch (_cardEffect)
        {
            case 0: //Movement Speed
                PlayerSC.instance.speed *= _effectValue;
                break;
            case 1: //Damage
                PlayerSC.instance.damage *= damageMultiplier;
                break;
            case 2: //Crit Chance
                PlayerSC.instance.critChance *= critChanceMultiplier;
                break;
            case 3: //Crit Damage
                PlayerSC.instance.critDamageMultiplier *= critDamageMultiplier;
                break;
            case 4: //Attack Speed
                PlayerSC.instance.fireRate /= attackSpeedMultiplier;
                break;
            case 5: //Max Health
                PlayerSC.instance.maxHealth *= _effectValue;
                PlayerSC.instance.health += PlayerSC.instance.maxHealth / 10;
                PlayerSC.instance.healthbar.maxValue = PlayerSC.instance.maxHealth;
                PlayerSC.instance.healthbar.value = PlayerSC.instance.health;
                break;
            case 6: //LIFE STEAL
                PlayerSC.instance.isLifeStealing = true;
                MassCardManager.instance.noMoreLifeSteal = true;
                break;
            case 7: //MORE EXP
                GameManager.instance.expValue *= _effectValue;
                break;
            case 8: //Reload Speed
                PlayerSC.instance.reloadTime /= _effectValue;
                break;
        }
    }
}

//Multi shot can only be picked twice, after that it will no longer spawn.
/*if (MassCardManager.instance.multiShotFirstPicked)
{
    MassCardManager.instance.multiShotEnd = true;
    PlayerSC.instance._bullet = Resources.Load("TRIPLE SHOT") as GameObject;
}
else
{
    MassCardManager.instance.multiShotFirstPicked = true;
    PlayerSC.instance._bullet = Resources.Load("DOUBLE SHOT") as GameObject;
} if(isKnockbackPicked)
{
    PlayerSC.instance.knockbackForce *= _effectValue;
}
else
{
    isKnockbackPicked = true;
    PlayerSC.instance.hasKnockback = true;
} */
