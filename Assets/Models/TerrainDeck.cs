using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainDeck
{
    private List<TerrainCard> _cards;

    public TerrainDeck(int numEachTerrains)
    {
        var tempCards = new List<TerrainCard>();

        for (int i = 0; i < numEachTerrains; i++)
        {
            tempCards.Add(new TerrainCard(TerrainCard.TerrainType.Cliff));
        }

        for (int i = 0; i < numEachTerrains; i++)
        {
            tempCards.Add(new TerrainCard(TerrainCard.TerrainType.Field));
        }

        for (int i = 0; i < numEachTerrains; i++)
        {
            tempCards.Add(new TerrainCard(TerrainCard.TerrainType.River));
        }

        _cards = shuffleDeck(tempCards);
    }

    private List<TerrainCard> shuffleDeck(List<TerrainCard> cards)
    {
        System.Random rand = new System.Random();

        List<TerrainCard> shuffledCards = new List<TerrainCard>();

        while (cards.Count > 0)
        {
            int randomIndex = (int)(rand.NextDouble() * cards.Count);

            shuffledCards.Add(cards[randomIndex]);

            cards.RemoveAt(randomIndex);
        }

        return shuffledCards;
    }

    public TerrainCard DrawCard()
    {
        var card = _cards[0];

        _cards.RemoveAt(0);

        return card;
    }

    public List<TerrainCard> DrawCards(int num)
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
