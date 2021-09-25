using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Area
{
    protected List<ICard> _cards;
    protected bool _visibleToPlayer;

    public List<ICard> Cards
    {
        get
        {
            return _cards;
        }
    }

    public int NumRevealedCards
    {
        get
        {
            int numRevealed = 0;

            for (int i = 0; i < _cards.Count; i++)
            {
                if (_cards[i].IsRevealed)
                {
                    numRevealed++;
                }
            }

            return numRevealed;
        }
    }

    public Area(bool VisibleToPlayer)
    {
        _cards = new List<ICard>();
        _visibleToPlayer = VisibleToPlayer;
    }

    public string ToString(int numOffset)
    {
        string output = "";

        for (int i = 0; i < _cards.Count; i++)
        {
            if (_visibleToPlayer == true)
            {
                var faceDown = _cards[i].IsRevealed ? "" : "/Face Down";
                output += "(" + (i + 1 + numOffset) + ")" + _cards[i] + faceDown + ", ";
            }
            else
            {
                if (_cards[i].IsRevealed == true)
                {
                    output += "(" + (i + 1 + numOffset) + ")" + _cards[i] + ", ";
                }
                else
                {
                    output += "(" + (i + 1 + numOffset) + ")Face Down, ";
                }
            }
        }

        return output;
    }

    public int NumOfFaceDown(bool player) //player when true, ai when false
    {
        int count = 0;

        if (player != _visibleToPlayer)
        {
            for (int i = 0; i < _cards.Count; i++)
            {
                if (_cards[i].IsRevealed == false)
                {
                    count++;
                }
            }
        }

        return count;
    }
}

