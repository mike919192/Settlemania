using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class MainCard : MonoBehaviour
{
    [SerializeField] private GameObject Card_Back;
    public bool VisibleToPlayer;

    private bool _faceDown;
    public bool FaceDown
    {
        get { return _faceDown; }
        set
        {
            _faceDown = value;
            if (FaceDown == false)
            {
                Card_Back.SetActive(false);
            }
            else
            {
                if (VisibleToPlayer == true)
                {
                    Card_Back.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 200.0f / 255.0f);
                }
                else
                {
                    Card_Back.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                }
            }
        }
    }
    public bool Selectable;

    public Action<MainCard> CardSelected;

    public GameObject CardTitleDisplay;
    public GameObject CardDescriptionDisplay;

    public string CardTitle;
    public string CardDescription;

    private Vector3 goToPosition;
    private bool goTo;

    public void MoveToPosition(Vector3 Position)
    {
        goToPosition = Position;
        goTo = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (goTo == true)
        {
            transform.Translate((goToPosition - transform.position) * 10.0f * Time.deltaTime);

            if (Vector3.Magnitude(goToPosition - transform.position) < 0.01f)
            {
                goTo = false;
                transform.position = goToPosition;
            }
        }
    }

    public void OnMouseEnter()
    {
        if (FaceDown == true)
        {
            if (VisibleToPlayer && Card_Back.activeSelf)
            {
                Card_Back.SetActive(false);
            }
        }

        //cheat mode if commented
        if (VisibleToPlayer || FaceDown == false)
        {
            if (CardTitle != null && CardTitleDisplay != null)
            { 
                CardTitleDisplay.SetActive(true);
                var cardTitleDisplayObject = CardTitleDisplay.GetComponent<Text>();
                cardTitleDisplayObject.text = CardTitle;
            }
            if (CardDescription != null && CardDescriptionDisplay != null)
            {
                CardDescriptionDisplay.SetActive(true);
                var cardDescriptionDisplayObject = CardDescriptionDisplay.GetComponent<Text>();
                cardDescriptionDisplayObject.text = CardDescription;
            }
        }

    }

    public void OnMouseExit()
    {
        if (FaceDown == true)
        {
            if (Card_Back.activeSelf == false)
            {
                Card_Back.SetActive(true);
            }
        }

        if (CardTitleDisplay != null)
        {
            CardTitleDisplay.SetActive(false);
            var cardTitleDisplayObject = CardTitleDisplay.GetComponent<Text>();
            cardTitleDisplayObject.text = "";
        }

        if (CardDescriptionDisplay != null)
        {
            CardDescriptionDisplay.SetActive(false);
            var cardDescriptionDisplayObject = CardDescriptionDisplay.GetComponent<Text>();
            cardDescriptionDisplayObject.text = "";
        }
    }

    public void OnMouseDown()
    {
        if (Selectable == true)
        {
            CardSelected(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (FaceDown == false)
        {
            Card_Back.SetActive(false);
        }
        else
        {
            if (VisibleToPlayer == true)
            {
                Card_Back.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 200.0f / 255.0f);
            }
            else
            {
                Card_Back.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
        }
    }

    public void DestroyMe()
    {
        Destroy(this);
    }
}
