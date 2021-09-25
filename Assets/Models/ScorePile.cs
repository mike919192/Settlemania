using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorePile
{
    private List<PlayCard> _cards;

    public List<PlayCard> Cards
    {
        get
        {
            return _cards;
        }
    }

    public ScorePile()
    {
        _cards = new List<PlayCard>();
    }

    public int CountScore(TerrainArea terrain)
    {
        int score = 0;

        for (int i = 0; i < _cards.Count; i++)
        {
            if ((_cards[i].Type == PlayCard.PlayType.CityWall) ||
                (_cards[i].Type == PlayCard.PlayType.Bandits) ||
                (_cards[i].Type == PlayCard.PlayType.Raiders))
            {
                //one point for counters
                score += 1;
            }
            else if (_cards[i].Type == PlayCard.PlayType.Militia)
            {
                //one point plus one for each cliff
                score += 1;

                for (int j = 0; j < terrain.Cards.Count; j++)
                {
                    if (((TerrainCard)terrain.Cards[j]).Type == TerrainCard.TerrainType.Cliff)
                    {
                        score += 1;
                    }
                }
            }
            else if (_cards[i].Type == PlayCard.PlayType.TradingPost)
            {
                //one point plus one for each river
                score += 1;

                for (int j = 0; j < terrain.Cards.Count; j++)
                {
                    if (((TerrainCard)terrain.Cards[j]).Type == TerrainCard.TerrainType.River)
                    {
                        score += 1;
                    }
                }
            }
            else if (_cards[i].Type == PlayCard.PlayType.Farm)
            {
                //one point plus one for each field
                score += 1;

                for (int j = 0; j < terrain.Cards.Count; j++)
                {
                    if (((TerrainCard)terrain.Cards[j]).Type == TerrainCard.TerrainType.Field)
                    {
                        score += 1;
                    }
                }
            }
        }

        return score;
    }

    public override string ToString()
    {
        string output = "";

        for (int i = 0; i < _cards.Count; i++)
        {
            output += _cards[i] + ", ";
        }

        return output;
    }
}
