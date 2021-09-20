using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    public void OnMouseEnter()
    {
        if (FaceDown == true)
        {
            if (VisibleToPlayer && Card_Back.activeSelf)
            {
                Card_Back.SetActive(false);
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
