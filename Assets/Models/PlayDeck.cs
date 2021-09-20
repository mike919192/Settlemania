using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayDeck
{
    private List<PlayCard> _cards;

    public PlayDeck(int numEachPlayCard)
    {
        var tempCards = new List<PlayCard>();

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

        _cards = shuffleDeck(tempCards);
    }

    private List<PlayCard> shuffleDeck(List<PlayCard> cards)
    {
        System.Random rand = new System.Random();

        List<PlayCard> shuffledCards = new List<PlayCard>();

        while (cards.Count > 0)
        {
            int randomIndex = (int)(rand.NextDouble() * cards.Count);

            shuffledCards.Add(cards[randomIndex]);

            cards.RemoveAt(randomIndex);
        }

        return shuffledCards;
    }

    public PlayCard DrawCard()
    {
        var card = _cards[0];

        _cards.RemoveAt(0);

        return card;
    }

    public List<PlayCard> DrawCards(int num)
    {
        var cards = _cards.GetRange(0, num);

        _cards.RemoveRange(0, num);

        return cards;
    }

    public override string ToString()
    {
        string output = "";

        foreach (var card in _cards)
        {
            output += card.ToString() + ", ";
        }

        return output;
    }
}
