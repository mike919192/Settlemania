using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayArea
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

    public PlayArea(bool VisibleToPlayer)
    {
        _cards = new List<PlayCard>();
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

    public int NumOfType(PlayCard.PlayType type, bool player) //player when true, ai when false
    {
        int count = 0;

        if (player == _visibleToPlayer)
        {
            for (int i = 0; i < _cards.Count; i++)
            {
                if (_cards[i].Type == type)
                {
                    count++;
                }
            }
        }
        else
        {
            for (int i = 0; i < _cards.Count; i++)
            {
                if (_cards[i].IsRevealed && _cards[i].Type == type)
                {
                    count++;
                }
            }
        }

        return count;
    }
}
