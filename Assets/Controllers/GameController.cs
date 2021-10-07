using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Networking;
using SimpleJSON;
using System.Security.Cryptography;

public class GameController : MonoBehaviour
{
    TerrainArea playerTerrains;
    TerrainArea aiTerrains;
    PlayHand playerHand;
    PlayHand aiHand;

    PlayArea playerArea;
    PlayArea aiArea;

    ScorePile playerScorePile;
    ScorePile aiScorePile;

    AIStrat aiStrat;

    const int firstRoundTurns = 5;
    const int drawFirstRound = 8;

    const int secondRoundTurns = 3;
    const int drawSecondRound = 2;

    public Sprite CliffSmallSprite;
    public Sprite RiverSmallSprite;
    public Sprite FieldSmallSprite;

    public Sprite MilitiaSmallSprite;
    public Sprite TradingPostSmallSprite;
    public Sprite FarmSmallSprite;

    public Sprite CityWallsSmallSprite;
    public Sprite BanditsSmallSprite;
    public Sprite RaidersSmallSprite;

    public MainCard OriginalCard;

    private int turnCounter;
    private int roundCounter;
    private int turnsInCurrentRound;

    private List<MainCard> playerTerrainsGraphics;
    private List<MainCard> aiTerrainsGraphics;
    private List<MainCard> playerHandGraphics;
    private List<MainCard> aiHandGraphics;
    private List<MainCard> playerAreaGraphics;
    private List<MainCard> aiAreaGraphics;

    private List<MainCard> playerScorePileGraphics;
    private List<MainCard> aiScorePileGraphics;

    Action<MainCard> cbCardPlayedFromHand;

    private PlayDeck pDeck;
    private TerrainDeck tDeck;

    GameObject scoreTextAI;
    GameObject scoreTextPlayer;

    GameObject cardTitleDisplay;
    GameObject cardDescriptionDisplay;

    GameObject commandDisplay;
    GameObject turnDisplay;
    GameObject roundDisplay;

    GameObject nextButton;
    ButtonScript nextButtonScript;

    GameObject playDeckGraphic;
    DeckScript playDeckScript;

    GameObject terrainDeckGraphic;
    DeckScript terrainDeckScript;

    string userID;
    string serverAddress = "http://mikerbloom1.asuscomm.com:8123/";

    GameData previousData;
    GameData currentData;

    int[] terrainDeckShuffleIndex;
    int[] playDeckShuffleIndex;

    bool player1;
    bool pollingIsRunning;

    class GameData
    {
        public int turn = 1;
        public int round = 1;
        public bool reveal = false;
        public int playerPlayCard = -1;
        public int opponentPlayCard = -1;
        public int playerRevealCard = -1;
        public int opponentRevealCard = -1;
    }

    private static readonly RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();

    private string GenerateUniqueID(int length)
    {
        string allowedChars = @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        string buildString = "";

        System.Random rand = new System.Random();

        for (int i = 0; i < length; i++)
        {
            int randomIndex = (int)(rand.NextDouble() * allowedChars.Length);

            buildString += allowedChars.Substring(randomIndex, 1);
        }

        return buildString;
    }

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 30;
        previousData = new GameData();
        currentData = new GameData();

        scoreTextAI = GameObject.Find("ScoreTextAI");
        scoreTextAI.SetActive(false);
        scoreTextPlayer = GameObject.Find("ScoreTextPlayer");
        scoreTextPlayer.SetActive(false);

        cardTitleDisplay = GameObject.Find("CardTitleDisplay");
        cardTitleDisplay.SetActive(false);
        cardDescriptionDisplay = GameObject.Find("CardDescriptionDisplay");
        cardDescriptionDisplay.SetActive(false);

        commandDisplay = GameObject.Find("CommandDisplay");
        turnDisplay = GameObject.Find("TurnDisplay");
        roundDisplay = GameObject.Find("RoundDisplay");

        nextButton = GameObject.Find("NextButton");
        nextButtonScript = nextButton.GetComponent<ButtonScript>();
        nextButton.SetActive(false);

        playDeckGraphic = GameObject.Find("PlayDeck");
        playDeckScript = playDeckGraphic.GetComponent<DeckScript>();

        terrainDeckGraphic = GameObject.Find("TerrainDeck");
        terrainDeckScript = terrainDeckGraphic.GetComponent<DeckScript>();

        userID = GenerateUniqueID(10);

        StartCoroutine(SetUserID(serverAddress, userID));
    }

    void ProcessGameData(string rawResponse)
    {
        JSONNode node = JSON.Parse(rawResponse);

        currentData.turn = node["turn"];
        currentData.round = node["round"];
        currentData.reveal = node["reveal"];

        if (userID == node["player1UID"])
        {
            currentData.playerPlayCard = node["player1PlayCard"];
            currentData.playerRevealCard = node["player1RevealCard"];
            currentData.opponentPlayCard = node["player2PlayCard"];
            currentData.opponentRevealCard = node["player2RevealCard"];
        }
        else
        {
            currentData.playerPlayCard = node["player2PlayCard"];
            currentData.playerRevealCard = node["player2RevealCard"];
            currentData.opponentPlayCard = node["player1PlayCard"];
            currentData.opponentRevealCard = node["player1RevealCard"];
        }

        
        

        //go from play to reveal
        if (currentData.turn == previousData.turn && currentData.reveal != previousData.reveal)
        {
            GoToRevealPhase();
        }

        //go from reveal to next turn play
        if (currentData.turn != previousData.turn && currentData.round == previousData.round)
        {
            GoToNextTurn();
        }

        if (currentData.round > previousData.round)
        {
            SetUpNextRound();
        }

        if (currentData.round < previousData.round)
        {
            RestartGame();
        }

        //check if opponent played a card
        if (currentData.opponentPlayCard >= 0 && currentData.opponentPlayCard != previousData.opponentPlayCard)
        {
            OpponentPlayCardFromHand(currentData.opponentPlayCard);
        }

        //check if opponent revealed a card
        if (currentData.opponentRevealCard >= 0 && currentData.opponentRevealCard != previousData.opponentRevealCard)
        {
            OpponentRevealACard(currentData.opponentRevealCard);
        }

        previousData.turn = currentData.turn;
        previousData.round = currentData.round;
        previousData.reveal = currentData.reveal;
        previousData.playerPlayCard = currentData.playerPlayCard;
        previousData.opponentPlayCard = currentData.opponentPlayCard;
        previousData.playerRevealCard = currentData.playerRevealCard;
        previousData.opponentRevealCard = currentData.opponentRevealCard;
    }

    void SendGameData(string address, string userID)
    {
        UnityWebRequest www;
        if (player1)
            www = UnityWebRequest.Get(serverAddress +
                "sendGameData/" + 
                userID + "/" + 
                currentData.turn + "/" + 
                currentData.round + "/" + 
                currentData.reveal + "/" + 
                currentData.playerPlayCard + "/" + 
                currentData.playerRevealCard + "/" + 
                currentData.opponentPlayCard + "/" + 
                currentData.opponentRevealCard);
        else
            www = UnityWebRequest.Get(serverAddress +
                "sendGameData/" +
                userID + "/" +
                currentData.turn + "/" +
                currentData.round + "/" +
                currentData.reveal + "/" +
                currentData.opponentPlayCard + "/" +
                currentData.opponentRevealCard + "/" +
                currentData.playerPlayCard + "/" +
                currentData.playerRevealCard);

        www.SendWebRequest();
    }

    IEnumerator PollWebData(string address, string userID)
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            UnityWebRequest www = UnityWebRequest.Get(address + "gamedata/" + userID);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Something went wrong" + www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                ProcessGameData(www.downloadHandler.text);
                SendGameData(address, userID);
            }
        }
    }

    IEnumerator SetUserID(string address, string userID)
    {
        UnityWebRequest www = UnityWebRequest.Get(address + "setUserID/" + userID);

        yield return www.SendWebRequest();

        JSONNode node = JSON.Parse(www.downloadHandler.text);

        if (userID == node["player1UID"])
        {
            player1 = true;
        }
        else if (userID == node["player2UID"])
        {
            player1 = false;
        }
        else
        {
            //some problem here
            //try setting UID again
            StartCoroutine(SetUserID(serverAddress, userID));

            yield break;
        }

        StartCoroutine(GetDeckData(serverAddress));
    }

    IEnumerator GetDeckData(string address)
    {
        UnityWebRequest www = UnityWebRequest.Get(address + "deckData/");

        yield return www.SendWebRequest();

        JSONNode node = JSON.Parse(www.downloadHandler.text);

        terrainDeckShuffleIndex = node["terrainDeck"].Children.Select(t => t[0].AsInt).ToArray();
        playDeckShuffleIndex = node["playDeck"].Children.Select(t => t[0].AsInt).ToArray();

        SetUpGameStart();
    }

    void SetPlayCard(string address, int playCard)
    {
        UnityWebRequest www = UnityWebRequest.Get(address + "setPlayCard/" + userID + "/" + playCard);

        www.SendWebRequest();
    }

    void SetRevealCard(string address, int revealCard)
    {
        UnityWebRequest www = UnityWebRequest.Get(address + "setRevealCard/" + userID + "/" + revealCard);

        www.SendWebRequest();
    }

    void SetUpGameStart()
    {
        if (pollingIsRunning == false)
        {
            StartCoroutine(PollWebData(serverAddress, userID));
            pollingIsRunning = true;
        }

        playerTerrainsGraphics = new List<MainCard>();
        aiTerrainsGraphics = new List<MainCard>();
        playerHandGraphics = new List<MainCard>();
        aiHandGraphics = new List<MainCard>();
        playerAreaGraphics = new List<MainCard>();
        aiAreaGraphics = new List<MainCard>();
        playerScorePileGraphics = new List<MainCard>();
        aiScorePileGraphics = new List<MainCard>();

        turnCounter = 0;
        roundCounter = 0;
        turnsInCurrentRound = firstRoundTurns;

        var turnDisplayText = turnDisplay.GetComponent<TextMesh>();
        turnDisplayText.text = "Turn " + "1/" + turnsInCurrentRound;

        var roundDisplayText = roundDisplay.GetComponent<TextMesh>();
        roundDisplayText.text = "Round 1/2";

        var commandDisplayText = commandDisplay.GetComponent<TextMesh>();
        commandDisplayText.text = "Play card from your hand";

        aiStrat = new AIStrat();

        tDeck = new TerrainDeck(5, terrainDeckShuffleIndex);
        terrainDeckScript.deck = tDeck;

        playerTerrains = new TerrainArea(true);
        aiTerrains = new TerrainArea(false);

        if (player1)
        {
            playerTerrains.Cards.AddRange(tDeck.DrawCards(3).Cast<TerrainCard>().ToList());
            aiTerrains.Cards.AddRange(tDeck.DrawCards(3).Cast<TerrainCard>().ToList());
        }
        else
        {
            aiTerrains.Cards.AddRange(tDeck.DrawCards(3).Cast<TerrainCard>().ToList());
            playerTerrains.Cards.AddRange(tDeck.DrawCards(3).Cast<TerrainCard>().ToList());            
        }

        InitTerrain(playerTerrains, true);
        InitTerrain(aiTerrains, false);

        pDeck = new PlayDeck(5, playDeckShuffleIndex);
        playDeckScript.deck = pDeck;

        playerHand = new PlayHand(true);
        aiHand = new PlayHand(false);

        if(player1)
        {
            playerHand.Cards.AddRange(pDeck.DrawCards(drawFirstRound).Cast<PlayCard>().ToList());
            aiHand.Cards.AddRange(pDeck.DrawCards(drawFirstRound).Cast<PlayCard>().ToList());
        }
        else
        {
            aiHand.Cards.AddRange(pDeck.DrawCards(drawFirstRound).Cast<PlayCard>().ToList());
            playerHand.Cards.AddRange(pDeck.DrawCards(drawFirstRound).Cast<PlayCard>().ToList());
        }        

        InitHand(playerHand, true);
        InitHand(aiHand, false);

        playerArea = new PlayArea(true);
        aiArea = new PlayArea(false);

        playerScorePile = new ScorePile();
        aiScorePile = new ScorePile();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpaceCardsInHand(List<MainCard> handGraphics, bool player)
    {
        float yValue;
        if (player)
            yValue = -4f;
        else
            yValue = 4f;

        //use this method for more than 4 cards
        if (handGraphics.Count > 6)
        {
            var xStep = 12.0f / (handGraphics.Count - 1);
            for (int i = 0; i < handGraphics.Count; i++)
            {
                handGraphics[i].MoveToPosition(new Vector3(-6 + (xStep * i), yValue, i / 100.0f));
            }
        }

        //user this for 4 or less
        else
        {
            var xStep = 2.0f;
            for (int i = 0; i < handGraphics.Count; i++)
            {
                handGraphics[i].MoveToPosition(new Vector3((i * xStep) - ((handGraphics.Count-1) / 2.0f * xStep), yValue, i / 100.0f));
            }
        }
    }

    void InitHand(PlayHand hand, bool player)
    {
        for (int i = 0; i < hand.Cards.Count; i++)
        {
            MainCard card;
            if (player)
            {
                if (playerHandGraphics.Count <= i)
                {
                    card = Instantiate(OriginalCard) as MainCard;
                    playerHandGraphics.Add(card);

                    //start at play card deck
                    card.transform.position = new Vector3(8.0f, 1.0f, 0.0f);

                    card.FaceDown = false;
                    card.VisibleToPlayer = true;
                    card.Selectable = true;
                    card.CardSelected = PlayCardFromHand;
                    SetPlayCardSprite(card, hand.Cards[i]);
                }
                else
                {
                    card = playerHandGraphics[i];
                }

                card.name = "PlayerHand" + (i + 1);
            }
            
            else
            {
                if (aiHandGraphics.Count <= i)
                {
                    card = Instantiate(OriginalCard) as MainCard;
                    aiHandGraphics.Add(card);

                    //start at play card deck
                    card.transform.position = new Vector3(8.0f, 1.0f, 0.0f);

                    card.FaceDown = true;
                    card.VisibleToPlayer = false;
                    card.Selectable = false;
                    SetPlayCardSprite(card, hand.Cards[i]);
                }
                else
                {
                    card = aiHandGraphics[i];
                }

                card.name = "AiHand" + (i + 1);
            }
            
        }

        SpaceCardsInHand(playerHandGraphics, true);
        SpaceCardsInHand(aiHandGraphics, false);
    }

    void SetPlayCardSprite(MainCard card, PlayCard playCard)
    {
        SpriteRenderer hand_sr = card.GetComponent<SpriteRenderer>();
        card.CardTitleDisplay = cardTitleDisplay;
        card.CardDescriptionDisplay = cardDescriptionDisplay;

        if (playCard.Type == PlayCard.PlayType.Militia)
        {
            hand_sr.sprite = MilitiaSmallSprite;
            card.CardTitle = "Militia";
            card.CardDescription = "1 point + 1 for each cliff";
        }
        else if (playCard.Type == PlayCard.PlayType.TradingPost)
        {
            hand_sr.sprite = TradingPostSmallSprite;
            card.CardTitle = "Trading Post";
            card.CardDescription = "1 point + 1 for each river";
        }
        else if (playCard.Type == PlayCard.PlayType.Farm)
        {
            hand_sr.sprite = FarmSmallSprite;
            card.CardTitle = "Farm";
            card.CardDescription = "1 point + 1 for each field";
        }
        else if (playCard.Type == PlayCard.PlayType.CityWall)
        {
            hand_sr.sprite = CityWallsSmallSprite;
            card.CardTitle = "City Wall";
            card.CardDescription = "1 point and counters 1 militia";
        }
        else if (playCard.Type == PlayCard.PlayType.Bandits)
        {
            hand_sr.sprite = BanditsSmallSprite;
            card.CardTitle = "Bandits";
            card.CardDescription = "1 point and counters 1 trading post";
        }
        else
        {
            hand_sr.sprite = RaidersSmallSprite;
            card.CardTitle = "Raiders";
            card.CardDescription = "1 point and counters 1 farm";
        }
    }

    void InitTerrain(TerrainArea terrain, bool player)
    {
        for (int i = 0; i < terrain.Cards.Count; i++)
        {
            MainCard card = Instantiate(OriginalCard) as MainCard;
            //start at play card deck
            card.transform.position = new Vector3(8.0f, -1.0f, 0.0f);
            if (player)
            {
                card.name = "PlayerTerrain" + (i + 1);
                card.MoveToPosition(new Vector3(-2 + 2 * i, -2.5f, 1));
                card.FaceDown = true;
                card.VisibleToPlayer = true;
                playerTerrainsGraphics.Add(card);
            }
            else
            {
                card.name = "AITerrain" + (i + 1);
                card.MoveToPosition(new Vector3(-2 + 2 * i, 2.5f, 1));
                card.FaceDown = true;
                card.VisibleToPlayer = false;
                card.CardSelected = RevealACard;
                aiTerrainsGraphics.Add(card);
            }

            card.CardTitleDisplay = cardTitleDisplay;

            SpriteRenderer terrain_sr = card.GetComponent<SpriteRenderer>();

            if (((TerrainCard)terrain.Cards[i]).Type == TerrainCard.TerrainType.Cliff)
            {
                terrain_sr.sprite = CliffSmallSprite;
                card.CardTitle = "Cliff";
            }
            else if (((TerrainCard)terrain.Cards[i]).Type == TerrainCard.TerrainType.River)
            {
                terrain_sr.sprite = RiverSmallSprite;
                card.CardTitle = "River";
            }
            else
            {
                terrain_sr.sprite = FieldSmallSprite;
                card.CardTitle = "Field";
            }
        }
    }

    public void GoToRevealPhase()
    {
        if (turnCounter < turnsInCurrentRound - 1)
        {
            //make aiArea and aiTerrain selectable
            foreach (var cardGraphic in aiAreaGraphics)
                cardGraphic.Selectable = true;

            foreach (var cardGraphic in aiTerrainsGraphics)
                cardGraphic.Selectable = true;

            var commandDisplayText = commandDisplay.GetComponent<TextMesh>();
            commandDisplayText.text = "Select opponent card to reveal";
        }
        else
        {
            //reveal all play cards
            for (int j = 0; j < playerArea.Cards.Count; j++)
            {
                playerArea.Cards[j].Reveal();
                playerAreaGraphics[j].FaceDown = false;
                aiArea.Cards[j].Reveal();
                aiAreaGraphics[j].FaceDown = false;
            }

            if (roundCounter == 1)
            {
                //reveal all terrains
                for (int j = 0; j < playerTerrains.Cards.Count; j++)
                {
                    playerTerrains.Cards[j].Reveal();
                    playerTerrainsGraphics[j].FaceDown = false;
                    aiTerrains.Cards[j].Reveal();
                    aiTerrainsGraphics[j].FaceDown = false;
                }
            }

            nextButtonScript.ButtonClicked = Proceed;
            nextButton.SetActive(true);
            var nextButtonText = nextButton.GetComponentInChildren(typeof(TextMesh));
            ((TextMesh)nextButtonText).text = "Next";

            var commandDisplayText = commandDisplay.GetComponent<TextMesh>();
            commandDisplayText.text = "Click next to apply counters";
        }
    }

    public void OpponentPlayCardFromHand(int aiIndex)
    {
        //ai plays card
        //var aiIndex = aiStrat.GetNextPlayPick(aiHand, aiTerrains, aiArea, playerArea, playerTerrains, (turnsInCurrentRound - 1) - turnCounter);

        //get card reference
        var aiCard = aiHandGraphics[aiIndex];

        //move card to play area
        aiCard.MoveToPosition(new Vector3(-6 + (1.75f * turnCounter), 1f, 0));

        //move reference in list
        aiHandGraphics.Remove(aiCard);
        aiAreaGraphics.Add(aiCard);
        aiCard.CardSelected = RevealACard;

        //respace cards
        SpaceCardsInHand(aiHandGraphics, false);

        //move card in data
        aiArea.Cards.Add(aiHand.SelectCard(aiIndex));
    }

    public void PlayCardFromHand(MainCard card)
    {
        //get index of selected card
        int index = playerHandGraphics.IndexOf(card);

        //tell server what you played
        SetPlayCard(serverAddress, index);

        // move card to play area
        card.MoveToPosition(new Vector3(-6 + (1.75f * turnCounter), -1f, 0));
        card.FaceDown = true;
        card.Selectable = false;

        //move reference in list
        playerHandGraphics.Remove(card);
        playerAreaGraphics.Add(card);

        //respace cards
        SpaceCardsInHand(playerHandGraphics, true);

        //move card in data
        playerArea.Cards.Add(playerHand.SelectCard(index));

        //display waiting text
        var commandDisplayText = commandDisplay.GetComponent<TextMesh>();
        commandDisplayText.text = "Waiting for opponent";

        //make player hand not selectable
        foreach (var cardGraphic in playerHandGraphics)
            cardGraphic.Selectable = false;
    }

    public void GoToNextTurn()
    {
        //make player hand selectable
        foreach (var cardGraphic in playerHandGraphics)
            cardGraphic.Selectable = true;

        turnCounter++;
        var turnDisplayText = turnDisplay.GetComponent<TextMesh>();
        turnDisplayText.text = "Turn " + (turnCounter + 1) + "/" + turnsInCurrentRound;

        var commandDisplayText = commandDisplay.GetComponent<TextMesh>();
        commandDisplayText.text = "Play card from your hand";
    }

    public void OpponentRevealACard(int revealIndex)
    {
        //ai reveals a card
        //var revealIndex = aiStrat.GetNextRevealPick(playerArea, playerTerrains);

        //reveal the card in data and in graphics
        if (revealIndex < playerArea.Cards.Count)
        {
            playerArea.Cards[revealIndex].Reveal();
            playerAreaGraphics[revealIndex].FaceDown = false;
        }
        else
        {
            playerTerrains.Cards[revealIndex - playerArea.Cards.Count].Reveal();
            playerTerrainsGraphics[revealIndex - playerArea.Cards.Count].FaceDown = false;
        }
    }

    public void RevealACard(MainCard card)
    {
        int index;
        //get index of selected card
        if(aiTerrainsGraphics.Contains(card))
        {
            //get index of selected card
            index = aiTerrainsGraphics.IndexOf(card);

            // reveal the card in the data
            aiTerrains.Cards[index].Reveal();

            index += aiAreaGraphics.Count;
        }
        else //it was from the play area
        {
            //get index of selected card
            index = aiAreaGraphics.IndexOf(card);

            //reveal the card in the data
            aiArea.Cards[index].Reveal();
        }

        SetRevealCard(serverAddress, index);

        //reveal the graphics
        card.FaceDown = false;

        //make aiArea and aiTerrain not selectable
        foreach (var cardGraphic in aiAreaGraphics)
            cardGraphic.Selectable = false;

        foreach (var cardGraphic in aiTerrainsGraphics)
            cardGraphic.Selectable = false;

        //display waiting text
        var commandDisplayText = commandDisplay.GetComponent<TextMesh>();
        commandDisplayText.text = "Waiting for opponent";
    }

    public void applyCounters(PlayArea area1, PlayArea area2, List<MainCard> area2Graphics)
    {
        //loop player area and find any counters
        for (int i = 0; i < area1.Cards.Count; i++)
        {
            if ((((PlayCard)area1.Cards[i]).Type == PlayCard.PlayType.CityWall) ||
                (((PlayCard)area1.Cards[i]).Type == PlayCard.PlayType.Bandits) ||
                (((PlayCard)area1.Cards[i]).Type == PlayCard.PlayType.Raiders))
            {
                //when counter found then loop ai area for the card it counters
                for (int j = 0; j < area2.Cards.Count; j++)
                {
                    //if you find it remove it
                    if ((((PlayCard)area1.Cards[i]).Type == PlayCard.PlayType.CityWall) && (((PlayCard)area2.Cards[j]).Type == PlayCard.PlayType.Militia))
                    {
                        area2.Cards.RemoveAt(j);
                        var card = area2Graphics[j];
                        area2Graphics.RemoveAt(j);
                        Destroy(card.gameObject);
                        break;
                    }
                    if ((((PlayCard)area1.Cards[i]).Type == PlayCard.PlayType.Bandits) && (((PlayCard)area2.Cards[j]).Type == PlayCard.PlayType.TradingPost))
                    {
                        area2.Cards.RemoveAt(j);
                        var card = area2Graphics[j];
                        area2Graphics.RemoveAt(j);
                        Destroy(card.gameObject);
                        break;
                    }
                    if ((((PlayCard)area1.Cards[i]).Type == PlayCard.PlayType.Raiders) && (((PlayCard)area2.Cards[j]).Type == PlayCard.PlayType.Farm))
                    {
                        area2.Cards.RemoveAt(j);
                        var card = area2Graphics[j];
                        area2Graphics.RemoveAt(j);
                        Destroy(card.gameObject);
                        break;
                    }
                }
            }
        }
    }

    public void SendReady(ButtonScript button)
    {
        UnityWebRequest www = UnityWebRequest.Get(serverAddress + "setReady/" + userID);

        www.SendWebRequest();

        var commandDisplayText = commandDisplay.GetComponent<TextMesh>();
        commandDisplayText.text = "Waiting for opponent";
    }

    public void Proceed(ButtonScript button)
    {
        applyCounters(playerArea, aiArea, aiAreaGraphics);
        applyCounters(aiArea, playerArea, playerAreaGraphics);

        if (roundCounter == 0)
        {
            nextButtonScript.ButtonClicked = SendReady;
            var commandDisplayText = commandDisplay.GetComponent<TextMesh>();
            commandDisplayText.text = "Click next to start next round";
        }
        else
        {
            nextButtonScript.ButtonClicked = FinishGame;
            var commandDisplayText = commandDisplay.GetComponent<TextMesh>();
            commandDisplayText.text = "Click next to calculate score";
        }
    }

    public void SetUpNextRound()
    {
        turnCounter = 0;
        roundCounter = 1;
        turnsInCurrentRound = secondRoundTurns;

        var turnDisplayText = turnDisplay.GetComponent<TextMesh>();
        turnDisplayText.text = "Turn " + "1/" + turnsInCurrentRound;

        var roundDisplayText = roundDisplay.GetComponent<TextMesh>();
        roundDisplayText.text = "Round 2/2";

        var commandDisplayText = commandDisplay.GetComponent<TextMesh>();
        commandDisplayText.text = "Play card from your hand";

        nextButton.SetActive(false);

        //move card data to score pile
        aiScorePile.Cards.AddRange(aiArea.Cards.Cast<PlayCard>().ToList());
        aiArea.Cards.Clear();

        playerScorePile.Cards.AddRange(playerArea.Cards.Cast<PlayCard>().ToList());
        playerArea.Cards.Clear();

        //clear play area graphics
        aiScorePileGraphics.AddRange(aiAreaGraphics);
        aiAreaGraphics.RemoveRange(0, aiAreaGraphics.Count);

        foreach(var cardGraphic in aiScorePileGraphics)
        {
            cardGraphic.MoveToPosition(new Vector3(-10f, 1f, 0));
        }

        playerScorePileGraphics.AddRange(playerAreaGraphics);
        playerAreaGraphics.RemoveRange(0, playerAreaGraphics.Count);
        
        foreach(var cardGraphic in playerScorePileGraphics)
        {
            cardGraphic.MoveToPosition(new Vector3(-10f, -1f, 0));
        }

        //draw cards for second round
        if (player1)
        {
            playerHand.Cards.AddRange(pDeck.DrawCards(drawSecondRound).Cast<PlayCard>().ToList());
            aiHand.Cards.AddRange(pDeck.DrawCards(drawSecondRound).Cast<PlayCard>().ToList());
        }
        else
        {
            aiHand.Cards.AddRange(pDeck.DrawCards(drawSecondRound).Cast<PlayCard>().ToList());
            playerHand.Cards.AddRange(pDeck.DrawCards(drawSecondRound).Cast<PlayCard>().ToList());
        }

        //refresh hands
        //foreach (var graphic in aiHandGraphics)
        //    Destroy(graphic.gameObject);
        //aiHandGraphics = new List<MainCard>();

        InitHand(aiHand, false);

        //foreach (var graphic in playerHandGraphics)
        //    Destroy(graphic.gameObject);
        //playerHandGraphics = new List<MainCard>();

        InitHand(playerHand, true);

        //make player hand selectable
        foreach (var cardGraphic in playerHandGraphics)
            cardGraphic.Selectable = true;
    }

    public void FinishGame(ButtonScript button)
    {
        turnCounter = 0;
        roundCounter = 2;

        //move card data to score pile
        aiScorePile.Cards.AddRange(aiArea.Cards.Cast<PlayCard>().ToList());
        aiArea.Cards.Clear();

        playerScorePile.Cards.AddRange(playerArea.Cards.Cast<PlayCard>().ToList());
        playerArea.Cards.Clear();

        //clear play area graphics
        aiScorePileGraphics.AddRange(aiAreaGraphics);
        aiAreaGraphics.RemoveRange(0, aiAreaGraphics.Count);

        playerScorePileGraphics.AddRange(playerAreaGraphics);
        playerAreaGraphics.RemoveRange(0, playerAreaGraphics.Count);

        //show score pile and scores
        //ShowScorePile(aiScorePile, false);
        //ShowScorePile(playerScorePile, true);

        for (int i = 0; i < aiScorePileGraphics.Count; i++)
        {
            aiScorePileGraphics[i].MoveToPosition(new Vector3(-6 + (1.75f * i), 1f, i/100));            
        }

        for (int i = 0; i < playerScorePileGraphics.Count; i++)
        {
            playerScorePileGraphics[i].MoveToPosition(new Vector3(-6 + (1.75f * i), -1f, i/100));
        }

        //var scoreTextAI = GameObject.Find("ScoreTextAI");
        //var scoreTextPlayer = GameObject.Find("ScoreTextPlayer");

        var scoreTextAIObject = scoreTextAI.GetComponent<TextMesh>();
        scoreTextAIObject.text = "Points: " + aiScorePile.CountScore(aiTerrains).ToString();
        scoreTextAI.SetActive(true);

        var scoreTextPlayerObject = scoreTextPlayer.GetComponent<TextMesh>();
        scoreTextPlayerObject.text = "Points: " + playerScorePile.CountScore(playerTerrains).ToString();
        scoreTextPlayer.SetActive(true);

        var commandDisplayText = commandDisplay.GetComponent<TextMesh>();
        commandDisplayText.text = "Click restart to play again";

        var nextButtonText = nextButton.GetComponentInChildren(typeof(TextMesh));
        ((TextMesh)nextButtonText).text = "Restart";
        nextButtonScript.ButtonClicked = SendReady;
    }

    public void RestartGame()
    {
        nextButton.SetActive(false);
        scoreTextAI.SetActive(false);
        scoreTextPlayer.SetActive(false);

        aiScorePile.Cards.Clear();
        playerScorePile.Cards.Clear();

        //clear play area graphics
        foreach (var graphic in aiAreaGraphics)
            Destroy(graphic.gameObject);
        aiAreaGraphics = new List<MainCard>();

        foreach (var graphic in playerAreaGraphics)
            Destroy(graphic.gameObject);
        playerAreaGraphics = new List<MainCard>();

        //clear terrain area graphics
        foreach (var graphic in aiTerrainsGraphics)
            Destroy(graphic.gameObject);
        aiTerrainsGraphics = new List<MainCard>();

        foreach (var graphic in playerTerrainsGraphics)
            Destroy(graphic.gameObject);
        playerTerrainsGraphics = new List<MainCard>();

        //clear hand area graphics
        foreach (var graphic in aiHandGraphics)
            Destroy(graphic.gameObject);
        aiHandGraphics = new List<MainCard>();

        foreach (var graphic in playerHandGraphics)
            Destroy(graphic.gameObject);
        playerHandGraphics = new List<MainCard>();

        //clear score pile
        foreach (var graphic in aiScorePileGraphics)
            Destroy(graphic.gameObject);
        aiScorePileGraphics = new List<MainCard>();

        foreach (var graphic in playerScorePileGraphics)
            Destroy(graphic.gameObject);
        playerScorePileGraphics = new List<MainCard>();

        StartCoroutine(GetDeckData(serverAddress));
        //SetUpGameStart();
    }

    public void ShowScorePile(ScorePile pile, bool player)
    {
        for (int i = 0; i < pile.Cards.Count; i++)
        {
            MainCard card = Instantiate(OriginalCard) as MainCard;
            if (player)
            {
                card.name = "PlayerScorePile" + (i + 1);
                card.transform.position = new Vector3(-6 + (1.75f * i), -1f, 0);
                card.FaceDown = false;
                card.VisibleToPlayer = true;
                playerAreaGraphics.Add(card);
            }
            else
            {
                card.name = "AIScorePile" + (i + 1);
                card.transform.position = new Vector3(-6 + (1.75f * i), 1f, 0);
                card.FaceDown = false;
                card.VisibleToPlayer = true;
                aiAreaGraphics.Add(card);
            }

            SpriteRenderer pile_sr = card.GetComponent<SpriteRenderer>();
            card.CardTitleDisplay = cardTitleDisplay;
            card.CardDescriptionDisplay = cardDescriptionDisplay;

            if (pile.Cards[i].Type == PlayCard.PlayType.Militia)
            {
                pile_sr.sprite = MilitiaSmallSprite;
                card.CardTitle = "Militia";
                card.CardDescription = "1 point + 1 for each cliff";
            }
            else if (pile.Cards[i].Type == PlayCard.PlayType.TradingPost)
            {
                pile_sr.sprite = TradingPostSmallSprite;
                card.CardTitle = "Trading Post";
                card.CardDescription = "1 point + 1 for each river";
            }
            else if (pile.Cards[i].Type == PlayCard.PlayType.Farm)
            {
                pile_sr.sprite = FarmSmallSprite;
                card.CardTitle = "Farm";
                card.CardDescription = "1 point + 1 for each field";
            }
            else if (pile.Cards[i].Type == PlayCard.PlayType.CityWall)
            {
                pile_sr.sprite = CityWallsSmallSprite;
                card.CardTitle = "City Wall";
                card.CardDescription = "1 point and counters 1 militia";
            }
            else if (pile.Cards[i].Type == PlayCard.PlayType.Bandits)
            {
                pile_sr.sprite = BanditsSmallSprite;
                card.CardTitle = "Bandits";
                card.CardDescription = "1 point and counters 1 trading post";
            }
            else
            {
                pile_sr.sprite = RaidersSmallSprite;
                card.CardTitle = "Raiders";
                card.CardDescription = "1 point and counters 1 farm";
            }
        }
    }
}
