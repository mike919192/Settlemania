using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayHand
{
    private List<PlayCard> _cards;
    private bool _visibleToPlayer;

    public List<PlayCard> Cards
    {
        get
        {
            return _cards;
        }
    }

    public PlayHand(bool VisibleToPlayer)
    {
        _cards = new List<PlayCard>();

        _visibleToPlayer = VisibleToPlayer;
    }

    public override string ToString()
    {
        string output = "";

        if (_visibleToPlayer == true)
        {
            for (int i = 0; i < _cards.Count; i++)
            {
                output += "(" + (i + 1) + ")" + _cards[i] + ", ";
            }
        }
        else
        {
            output = _cards.Count + " cards";
        }

        return output;
    }

    public PlayCard SelectCard(int i)
    {
        var card = _cards[i];

        _cards.RemoveAt(i);

        return card;
    }

    public PlayCard SelectCard(PlayCard card)
    {
        _cards.Remove(card);

        return card;
    }
}
