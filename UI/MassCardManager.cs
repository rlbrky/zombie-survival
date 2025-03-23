using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassCardManager : MonoBehaviour
{
    public static MassCardManager instance { get; private set; }

    List<PowerUpCardSC> powerUpCards;

    public bool noMoreLifeSteal;

    public List<int> cardValues= new List<int>();

    private readonly int _cardEffectCount = 9; //Max value should be effect value. Currently we have 9 effects so 9 values.
    
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

    void Start()
    {
        for(int i = 0; i < _cardEffectCount; i++)
            cardValues.Add(i);

        powerUpCards = new List<PowerUpCardSC>();
        foreach (var powerUpCard in ExpUI_Manager.instance.powerUpCards)
        {
            powerUpCards.Add(powerUpCard.GetComponent<PowerUpCardSC>());
        }
    }

    public void ResetCards()
    {
        cardValues.Clear();

        for (int i = 0; i < _cardEffectCount; i++)
            cardValues.Add(i);

        if (noMoreLifeSteal)
            cardValues.Remove(2); //Case number of life steal.
    }
}
