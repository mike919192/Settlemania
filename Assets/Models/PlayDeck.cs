using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayDeck : Deck
{    
    public PlayDeck(int numEachPlayCard)
    {
        var tempCards = new List<ICard>();

        for (int i = 0; i < numEachPlayCard; i++)
        {
            tempCards.Add(new PlayCard(PlayCard.PlayType.Militia));
        }

        for (int i = 0; i < numEachPlayCard; i++)
        {
            tempCards.Add(new PlayCard(PlayCard.PlayType.TradingPost));
        }

        for (int i = 0; i < numEachPlayCard; i++)
        {
            tempCards.Add(new PlayCard(PlayCard.PlayType.Farm));
        }

        for (int i = 0; i < numEachPlayCard; i++)
        {
            tempCards.Add(new PlayCard(PlayCard.PlayType.CityWall));
        }

        for (int i = 0; i < numEachPlayCard; i++)
        {
            tempCards.Add(new PlayCard(PlayCard.PlayType.Bandits));
        }

        for (int i = 0; i < numEachPlayCard; i++)
        {
            tempCards.Add(new PlayCard(PlayCard.PlayType.Raiders));
        }

        _cards = base.shuffleDeck(tempCards);
    }

    public PlayDeck(int numEachPlayCard, int[] indexArray)
    {
        var tempCards = new List<ICard>();

        for (int i = 0; i < numEachPlayCard; i++)
        {
            tempCards.Add(new PlayCard(PlayCard.PlayType.Militia));
        }

        for (int i = 0; i < numEachPlayCard; i++)
        {
            tempCards.Add(new PlayCard(PlayCard.PlayType.TradingPost));
        }

        for (int i = 0; i < numEachPlayCard; i++)
        {
            tempCards.Add(new PlayCard(PlayCard.PlayType.Farm));
        }

        for (int i = 0; i < numEachPlayCard; i++)
        {
            tempCards.Add(new PlayCard(PlayCard.PlayType.CityWall));
        }

        for (int i = 0; i < numEachPlayCard; i++)
        {
            tempCards.Add(new PlayCard(PlayCard.PlayType.Bandits));
        }

        for (int i = 0; i < numEachPlayCard; i++)
        {
            tempCards.Add(new PlayCard(PlayCard.PlayType.Raiders));
        }

        _cards = new List<ICard>();
        for (int i = 0; i < indexArray.Length; i++)
        {
            _cards.Add(tempCards[indexArray[i]]);
        }
    }
}
