using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckScript : MonoBehaviour
{
    public GameObject CardTitleDisplay;
    public GameObject CardDescriptionDisplay;

    public Deck deck;

    public string DeckTitle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMouseEnter()
    {
        CardTitleDisplay.SetActive(true);
        var cardTitleDisplayObject = CardTitleDisplay.GetComponent<Text>();
        cardTitleDisplayObject.text = DeckTitle;

        CardDescriptionDisplay.SetActive(true);
        var cardDescriptionDisplayObject = CardDescriptionDisplay.GetComponent<Text>();
        cardDescriptionDisplayObject.text = deck.CardCount.ToString() + " cards";
    }

    public void OnMouseExit()
    {
        CardTitleDisplay.SetActive(false);
        var cardTitleDisplayObject = CardTitleDisplay.GetComponent<Text>();
        cardTitleDisplayObject.text = "";

        CardDescriptionDisplay.SetActive(false);
        var cardDescriptionDisplayObject = CardDescriptionDisplay.GetComponent<Text>();
        cardDescriptionDisplayObject.text = "";
    }

}
