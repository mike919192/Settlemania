using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStrat
{
    public enum RevealStrat { Play, Terrain, Mixed };
    private RevealStrat _currentRevealStrat;

    public enum PlayStrat { Points, Random };
    private PlayStrat _currentPlayStrat;

    public RevealStrat CurrentRevealStrat
    {
        get
        {
            return _currentRevealStrat;
        }
    }

    public PlayStrat CurrentPlayStrat
    {
        get
        {
            return _currentPlayStrat;
        }
    }

    public AIStrat()
    {
        System.Random rand = new System.Random();

        int randomIndex = (int)(rand.NextDouble() * 3);

        _currentRevealStrat = (RevealStrat)randomIndex;

        randomIndex = (int)(rand.NextDouble() * 1);

        _currentPlayStrat = (PlayStrat)randomIndex;
    }

    public int GetNextRevealPick(PlayArea playArea, TerrainArea terrainArea)
    {
        if (_currentRevealStrat == RevealStrat.Play)
        {
            for (int i = 0; i < playArea.Cards.Count; i++)
            {
                if (playArea.Cards[i].IsRevealed == false)
                {
                    return i;
                }
            }
        }
        else if (_currentRevealStrat == RevealStrat.Terrain)
        {
            System.Random rand;
            int randomIndex;
            List<int> unrevealedIndexes = new List<int>();
            for (int i = 0; i < terrainArea.Cards.Count; i++)
            {
                if (terrainArea.Cards[i].IsRevealed == false)
                {
                    unrevealedIndexes.Add(i);
                }
            }

            if (unrevealedIndexes.Count > 0)
            {
                rand = new System.Random();
                randomIndex = (int)(rand.NextDouble() * unrevealedIndexes.Count);

                return unrevealedIndexes[randomIndex] + playArea.Cards.Count;
            }

            unrevealedIndexes = new List<int>();
            for (int i = 0; i < playArea.Cards.Count; i++)
            {
                if (playArea.Cards[i].IsRevealed == false)
                {
                    unrevealedIndexes.Add(i);
                }
            }

            rand = new System.Random();
            randomIndex = (int)(rand.NextDouble() * unrevealedIndexes.Count);

            return unrevealedIndexes[randomIndex];
        }
        else //mixed
        {
            System.Random rand = new System.Random();
            int randomIndex;
            List<int> unrevealedIndexes = new List<int>();

            for (int i = 0; i < playArea.Cards.Count; i++)
            {
                if (playArea.Cards[i].IsRevealed == false)
                {
                    unrevealedIndexes.Add(i);
                }
            }

            for (int i = 0; i < terrainArea.Cards.Count; i++)
            {
                if (terrainArea.Cards[i].IsRevealed == false)
                {
                    unrevealedIndexes.Add(i + playArea.Cards.Count);
                }
            }

            rand = new System.Random();
            randomIndex = (int)(rand.NextDouble() * unrevealedIndexes.Count);

            return unrevealedIndexes[randomIndex];
        }

        //this should never be reached
        return -1;
    }

    public int GetNextPlayPick(PlayHand aiHand, TerrainArea aiTerrain, PlayArea aiArea, PlayArea playerArea, TerrainArea playerTerrain, int turnsUntilEnd) //0 turns until end is last turn!
    {
        if (_currentPlayStrat == PlayStrat.Points)
        {
            List<double> scoreList = new List<double>();
            scoreList = evalHand(aiHand, aiTerrain, aiArea, playerArea, playerTerrain);

            double nextHighestScore = 0;
            int highestIndex = 0;
            double maxScore = 0;
            int nextHighestIndex = 0;
            for (int i = 0; i < scoreList.Count; i++)
            {
                if (scoreList[i] >= maxScore)
                {
                    nextHighestScore = maxScore;
                    nextHighestIndex = highestIndex;
                    maxScore = scoreList[i];
                    highestIndex = i;
                }
                else if (scoreList[i] >= nextHighestScore)
                {
                    nextHighestScore = scoreList[i];
                    nextHighestIndex = i;
                }
            }

            //int highestIndex = scoreList.FindIndex(t => t == maxScore);
            //int nextHighestIndex = scoreList.FindIndex(t => t == nextHighestScore);

            //on last turn play highest point card
            if (turnsUntilEnd == 0)
                return highestIndex;
            else
                return nextHighestIndex;
        }
        else //random!
        {
            System.Random rand = new System.Random();
            int randomIndex;
            randomIndex = (int)(rand.NextDouble() * aiHand.Cards.Count);

            return randomIndex;
        }
    }

    private List<double> evalHand(PlayHand aiHand, TerrainArea aiTerrain, PlayArea aiArea, PlayArea playerArea, TerrainArea playerTerrain)
    {
        List<double> pointArray = new List<double>();

        //evaluate how many points each card is
        for (int i = 0; i < aiHand.Cards.Count; i++)
        {
            if (aiHand.Cards[i].Type == PlayCard.PlayType.Militia)
            {
                //if there is already a counter on the board avoid playing it
                if (playerArea.NumOfType(PlayCard.PlayType.CityWall, false) > aiArea.NumOfType(PlayCard.PlayType.Militia, false))
                {
                    pointArray.Add(0);
                }
                else
                {
                    //count up the cliffs we have
                    pointArray.Add(aiTerrain.NumOfType(TerrainCard.TerrainType.Cliff, false) + 1);
                }
            }
            else if (aiHand.Cards[i].Type == PlayCard.PlayType.TradingPost)
            {
                //if there is already a counter on the board avoid playing it
                if (playerArea.NumOfType(PlayCard.PlayType.Bandits, false) > aiArea.NumOfType(PlayCard.PlayType.TradingPost, false))
                {
                    pointArray.Add(0);
                }
                else
                {
                    //count up the rivers
                    pointArray.Add(aiTerrain.NumOfType(TerrainCard.TerrainType.River, false) + 1);
                }
            }
            else if (aiHand.Cards[i].Type == PlayCard.PlayType.Farm)
            {
                //if there is already a counter on the board avoid playing it
                if (playerArea.NumOfType(PlayCard.PlayType.Raiders, false) > aiArea.NumOfType(PlayCard.PlayType.Farm, false))
                {
                    pointArray.Add(0);
                }
                else
                {
                    //count up the fields
                    pointArray.Add(aiTerrain.NumOfType(TerrainCard.TerrainType.Field, false) + 1);
                }
            }
            else if (aiHand.Cards[i].Type == PlayCard.PlayType.CityWall)
            {
                //check if we have militia to counter
                if (playerArea.NumOfType(PlayCard.PlayType.Militia, false) > aiArea.NumOfType(PlayCard.PlayType.CityWall, false))
                {
                    pointArray.Add(playerTerrain.NumOfType(TerrainCard.TerrainType.Cliff, false) + 1.0 + (playerTerrain.NumOfFaceDown(false) / 2.0));
                }
                //even if we dont see a militia if we see a land type we might want to counter it
                else if (playerTerrain.NumOfType(TerrainCard.TerrainType.Cliff, false) > 0)
                {
                    pointArray.Add(playerTerrain.NumOfType(TerrainCard.TerrainType.Cliff, false) + 0.5 + (playerTerrain.NumOfFaceDown(false) / 3.0));
                }
                //even if we dont see any land maybe add some weight to face down cards
                else
                {
                    pointArray.Add(1);
                }
            }
            else if (aiHand.Cards[i].Type == PlayCard.PlayType.Bandits)
            {
                //check if we have trading post to counter
                if (playerArea.NumOfType(PlayCard.PlayType.TradingPost, false) > aiArea.NumOfType(PlayCard.PlayType.Bandits, false))
                {
                    pointArray.Add(playerTerrain.NumOfType(TerrainCard.TerrainType.River, false) + 1.0 + (playerTerrain.NumOfFaceDown(false) / 2.0));
                }
                //even if we dont see a trading post if we see a land type we might want to counter it
                else if (playerTerrain.NumOfType(TerrainCard.TerrainType.River, false) > 0)
                {
                    pointArray.Add(playerTerrain.NumOfType(TerrainCard.TerrainType.River, false) + 0.5 + (playerTerrain.NumOfFaceDown(false) / 3.0));
                }
                //even if we dont see any land maybe add some weight to face down cards
                else
                {
                    pointArray.Add(1);
                }
            }
            else //raiders
            {
                //check if we have farm to counter
                if (playerArea.NumOfType(PlayCard.PlayType.Farm, false) > aiArea.NumOfType(PlayCard.PlayType.Raiders, false))
                {
                    pointArray.Add(playerTerrain.NumOfType(TerrainCard.TerrainType.Field, false) + 1.0 + (playerTerrain.NumOfFaceDown(false) / 2.0));
                }
                //even if we dont see a farm if we see a land type we might want to counter it
                else if (playerTerrain.NumOfType(TerrainCard.TerrainType.Field, false) > 0)
                {
                    pointArray.Add(playerTerrain.NumOfType(TerrainCard.TerrainType.Field, false) + 0.5 + (playerTerrain.NumOfFaceDown(false) / 3.0));
                }
                //even if we dont see any land maybe add some weight to face down cards
                else
                {
                    pointArray.Add(1);
                }
            }
        }

        return pointArray;
    }
}
