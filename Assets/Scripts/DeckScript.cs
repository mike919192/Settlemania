using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        var cardTitleDisplayObject = CardTitleDisplay.GetComponent<TextMesh>();
        cardTitleDisplayObject.text = DeckTitle;

        CardDescriptionDisplay.SetActive(true);
        var cardDescriptionDisplayObject = CardDescriptionDisplay.GetComponent<TextMesh>();
        cardDescriptionDisplayObject.text = deck.CardCount.ToString() + " cards";
    }

    public void OnMouseExit()
    {
        CardTitleDisplay.SetActive(false);
        var cardTitleDisplayObject = CardTitleDisplay.GetComponent<TextMesh>();
        cardTitleDisplayObject.text = "";

        CardDescriptionDisplay.SetActive(false);
        var cardDescriptionDisplayObject = CardDescriptionDisplay.GetComponent<TextMesh>();
        cardDescriptionDisplayObject.text = "";
    }

}
