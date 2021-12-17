using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public List<Character> Characters;
    public List<Circle> Spaces;
    public int CharIndex;
    public int ReloadIndex;
    public int Turn = -1;
    public int MaxTurns = 10;
    public string LastMiniGame;
    public bool IsBoardReversed;
    public CameraMove Cam;

    public List<CharacterStat> CharacterStats;
    public Dialog Dialog;
    public Question Question;
    public TurnText TurnText;
    public CurrentTurnText CurrentTurnText;
    public StarText StarText;
    public Rankings Rankings;
    public List<Dice> Dice;
    public GameObject Coin;
    public FadePanel FadePanel;
    public EventText EventText;
    public ItemsPanel ItemsPanel;
    public ItemChoicePanel ItemChoicePanel;
    public Pipe GoldenPipe;
    public LuckySpace LuckySpace;

    public AudioSource EnterMiniGameSound;
    public AudioSource MusicAS;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        var saveData = SaveController.Load();
        if (saveData == null || saveData.Turn == 0)
        {
            if (saveData != null)
            {
                foreach (Character character in Characters)
                {
                    var savedCharacter = saveData.Characters.Find(c => c.Type == character.Type);
                    character.IsPlayer = savedCharacter.IsPlayer;
                }
                MaxTurns = saveData.TotalTurns;
            }
            Cam.PanBoard(ShowOpeningText);
        }
        else
        {
            UpdateFromSaveData(saveData);
            Rankings.ShowRankings(DoTurn, saveData.LastWinningCharacters, false);
        }
    }

    public void CharacterLanded(Character character)
    {
        if (Turn == -1)
        {
            NextCharacter();
        }
        else
        {
            if (character.UsedItem == ItemType.DoubleDice)
            {
                character.UsedItem = ItemType.None;
                Dialog.ShowText($"{character.Type} rolls again with Double Dice", SetUpDoubleDice);
            }
            else
            {
                Move(character);
            }
        }
    }

    public void NextCharacter()
    {
        if (Turn >= 0)
        {
            return;
        }
        if (CharIndex == Characters.Count - 1)
        {
            Characters = Characters.OrderByDescending(c => c.Roll).ToList();
            CharIndex = 0;
            LoadAllCharacterStats(true);
        }
        else
        {
            CharIndex++;
            ChooseCharacter();
        }
    }

    public void LoadAllCharacterStats(bool reloadDelay)
    {
        if (ReloadIndex == Characters.Count)
        {
            ReloadIndex = 0;
            if (Turn == -1)
            {
                DoTurn();
            }
            return;
        }

        var charStat = CharacterStats[ReloadIndex];
        var character = Characters[ReloadIndex];
        int place = GetPlace(character.Type);
        charStat.Reload(character, ItemsPanel, place);

        if (!charStat.gameObject.activeSelf)
        {
            charStat.gameObject.SetActive(true);
        }

        ReloadIndex++;
        if (reloadDelay)
        {
            Invoke("LoadAllCharacterStatsDelay", 0.5f);
        }
        else
        {
            LoadAllCharacterStats(false);
        }
    }

    public void ReachedSpace(Character character, Circle space)
    {
        //hide die
        Dice[(int)character.Type - 1].gameObject.SetActive(false);

        //make space do its action
        if (space.Type == CircleType.Positive || space.Type == CircleType.Negative)
        {
            bool isDouble = (Turn + 1) >= MaxTurns - 5;
            var coins = space.Type == CircleType.Positive ? 3 : -3;
            if (isDouble)
            {
                coins *= 2;
            }
            character.ChangeCoins(coins, false);
            Invoke("NextTurn", 1f);
        }
        else if (space.Type == CircleType.Event)
        {
            IsBoardReversed = !IsBoardReversed;
            ReverseBoard();
            EventText.Show(NextTurn);
        }
        else if (space.Type == CircleType.Lucky)
        {
            LuckySpace.Show(character, ItemsPanel, Dialog, ReloadStatsAndContinueTurn, Turn == MaxTurns - 1);
        }
        else // if continue moving
        {
            if (space.Type == CircleType.Star)
            {
                Question.Show("Get this star for 20 coins?", character.IsPlayer ? AIChoice.None : character.Coins >= 20 ? AIChoice.First : AIChoice.Second, "Buy (20 coins)", "Never mind",  BuyStar);
            }
            else if (space.Type == CircleType.Boo)
            {
                Question.Show("He he he! What would like to steal?", character.IsPlayer ? AIChoice.None : character.Coins >= 35 && Characters.Any(c => c.Type != character.Type && c.Stars > 0) ? AIChoice.Second : AIChoice.First, "Coins (Free)", "Star (35 coins)", StealStar);
            }
            else if (space.Type == CircleType.Item)
            {
                if (Turn == MaxTurns - 1)
                {
                    Dialog.ShowText("You can't buy an item on the last turn", ContinueTurn);
                }
                else
                {
                    Question.Show("Would you like to buy an item?", character.IsPlayer ? AIChoice.None : character.Coins >= 3 && character.Items.Count < 3 ? AIChoice.First : AIChoice.Second, "See items", "No thanks", SeeItems);
                }
            }
        }

        //if another player is at this space
        foreach (CharacterType charType in space.OnSpace)
        {
            Characters.Find(c => c.Type == charType).Hide();
        }

        space.OnSpace.Add(character.Type);
    }

    public Character GetCurrentCharacter()
    {
        if (Turn == -1)
        {
            return null;
        }

        return Characters[CharIndex];
    }


    public int GetPlace(CharacterType type)
    {
        var orderedCharacters = Characters.OrderByDescending(c => c.Stars).ThenByDescending(c => c.Coins).ToList();
        return GetPlace(type, orderedCharacters);
    }

    public static int GetPlace(CharacterType type, List<Character> orderedCharacters)
    {
        int rank = 1;
        int currentStars = orderedCharacters[0].Stars;
        int currentCoins = orderedCharacters[0].Coins;
        int amountSinceLastIncrement = 0;
        foreach (Character character in orderedCharacters)
        {
            if (character.Stars != currentStars)
            {
                currentStars = character.Stars;
                currentCoins = character.Coins;
                rank += amountSinceLastIncrement;
                amountSinceLastIncrement = 1;
            }
            else if (character.Coins != currentCoins)
            {
                currentCoins = character.Coins;
                rank += amountSinceLastIncrement;
                amountSinceLastIncrement = 1;
            }
            else
            {
                amountSinceLastIncrement++;
            }

            if (character.Type == type)
            {
                return rank;
            }
        }

        return 0;
    }

    public void UseItem(Character character)
    {
        LoadAllCharacterStats(false);

        var itemType = character.UsedItem;
        var matchingItem = ItemsPanel.ItemSelections.Find(i => i.Type == itemType);
        Dialog.ShowText($"{character.Type} used a {matchingItem.ItemNameText.text}!", CallItem);
    }

    private void CallItem()
    {
        var character = GetCurrentCharacter();
        switch (character.UsedItem)
        {
            case ItemType.Mushroom:
            case ItemType.DoubleDice:
                SetUpDice();
                return;
            case ItemType.WarpBlock:
                WarpCharacter(character);
                return;
            case ItemType.GoldenPipe:
                UseGoldenPipe(character);
                return;
        }
    }

    private void WarpCharacter(Character character)
    {
        var otherCharacters = Characters.FindAll(c => c.Type != character.Type && c.Position != character.Position).OrderBy(c => DistanceToStar(c)).ToList();
        if (otherCharacters.Count == 0)
        {
            character.UsedItem = ItemType.None;
            character.Items.Add(ItemType.WarpBlock);
            Dialog.ShowText($"There's no one to warp with! The item wasn't used", SetUpDice);
            return;
        }

        Character randomCharacter = otherCharacters.First();
        var randomCharacterPosition = randomCharacter.transform.position;

        randomCharacter.transform.position = character.transform.position + Vector3.up;
        character.transform.position = randomCharacterPosition + Vector3.up;

        Spaces[character.Position].OnSpace.Add(randomCharacter.Type);
        Spaces[randomCharacter.Position].OnSpace.Remove(randomCharacter.Type);

        foreach (CharacterType charType in Spaces[randomCharacter.Position].OnSpace)
        {
            Characters.Find(c => c.Type == charType).Hide();
        }

        Spaces[randomCharacter.Position].OnSpace.Add(character.Type);
        Spaces[character.Position].OnSpace.Remove(character.Type);

        var randomCharacterSpacePosition = randomCharacter.Position;
        randomCharacter.Position = character.Position;
        character.Position = randomCharacterSpacePosition;

        randomCharacter.gameObject.SetActive(true);

        character.UsedItem = ItemType.None;

        character.PlayHappySound();
        randomCharacter.PlaySadSound();

        Dialog.ShowText($"{character.Type} and {randomCharacter.Type} swapped places!", SetUpDice);
    }

    private void UseGoldenPipe(Character character)
    {
        character.UsedItem = ItemType.None;

        var goldenPipe = Instantiate<Pipe>(GoldenPipe, character.transform.position, Quaternion.identity);
        goldenPipe.Show(GoldenPipeRaised);
    }

    private void GoldenPipeRaised()
    {
        var character = GetCurrentCharacter();
        character.gameObject.SetActive(false);

        var starSpace = Spaces[Spaces.FindIndex(s => s.Type == CircleType.Star) - 1];
        var goldenPipe = Instantiate<Pipe>(GoldenPipe, starSpace.transform.position, Quaternion.identity);
        goldenPipe.Show(RevealCharacterFromGoldenPipe);

        foreach (CharacterType charType in starSpace.OnSpace)
        {
            Characters.Find(c => c.Type == charType).Hide();
        }

        Spaces[character.Position].OnSpace.Remove(character.Type);
    }

    private void RevealCharacterFromGoldenPipe()
    {
        var character = GetCurrentCharacter();
        character.gameObject.SetActive(true);

        var starSpace = Spaces.FindIndex(s => s.Type == CircleType.Star);
        character.Position = starSpace - 1;

        var newSpace = Spaces[character.Position];
        character.transform.position = new Vector3(newSpace.transform.position.x, character.transform.position.y, newSpace.transform.position.z);

        newSpace.OnSpace.Add(character.Type);

        Invoke("SetUpDice", 0.5f);
    }

    private void ChooseCharacter()
    {
        Character character = Characters[CharIndex];
        if (Characters[CharIndex].IsPlayer)
        {
            Dialog.ShowText("Press space to hit the dice block", CharacterJump);
        }
        else
        {
            character.CanJump = true;
            character.Jump();
        }
    }

    private void LoadAllCharacterStatsDelay()
    {
        LoadAllCharacterStats(true);
    }

    private void DoTurn()
    {
        if (Turn == -1)
        {
            Dialog.ShowText($"{Characters[CharIndex].Type} goes first!", AddCoinsText);
            return;
        }

        if (Turn >= MaxTurns)
        {
            FadePanel.FadeOut();
            Invoke("LoadEndGame", 1f);
            return;
        }

        if (CharIndex == 0)
        {
            TurnText.DisplayTurn(Turn + 1);
            CurrentTurnText.Display(Turn + 1, MaxTurns);
            if ((Turn + 1) == MaxTurns - 5)
            {
                Dialog.ShowText($"Only 5 turns left! Spaces are worth double!", ShowCharacterStart);
                return;
            }
        }

        ShowCharacterStart();
    }

    private void ShowCharacterStart()
    {
        //Show yourself and hide other character at your space
        var currentChar = Characters[CharIndex];
        if (currentChar.Position >= 0)
        {
            foreach (var charType in Spaces[currentChar.Position].OnSpace)
            {
                var character = Characters.Find(c => c.Type == charType);
                if (character.Type == currentChar.Type)
                {
                    character.Show();
                }
                else
                {
                    character.Hide();
                }
            }
        }

        Question.Show($"{currentChar.Type}, it's your turn!", currentChar.IsPlayer ? AIChoice.None : ShouldUseItem(currentChar) ? AIChoice.Second : AIChoice.First, "Roll Dice", "Use Item", SetUpMove, 0.25f);
    }

    private void SetUpMove(bool setUpDice)
    {
        if (setUpDice)
        {
            SetUpDice();
        }
        else
        {
            Character character = GetCurrentCharacter();
            if (character.Items.Count == 0)
            {
                Dialog.ShowText("You don't have any items to use", SetUpDice);
                return;
            }

            ItemChoicePanel.ShowPanel(character, ItemsPanel, SetUpDice, MaxTurns - Turn);
        }
    }

    private bool ShouldUseItem(Character character)
    {
        if (character.Items.Count == 0)
        {
            return false;
        }

        if (character.Items.All(i => i == ItemType.GoldenPipe) && character.Coins < 20)
        {
            return false;
        }

        if (character.Items.All(i => i == ItemType.WarpBlock) && DistanceToStar(character) < 6)
        {
            return false;
        }

        if (Turn + character.Items.Count >= MaxTurns)
        {
            return true;
        }

        return Random.Range(0, 3) == 1;
    }

    private int DistanceToStar(Character character)
    {
        int posOfStar = Spaces.FindIndex(s => s.Type == CircleType.Star);
        int charPos = character.Position;

        if (posOfStar > charPos)
        {
            return posOfStar - charPos;
        }

        return Spaces.Count - charPos + posOfStar;
    }

    private void SetUpDice()
    {
        SetUpDice(false);
    }

    private void SetUpDoubleDice()
    {
        SetUpDice(true);
    }

    private void SetUpDice(bool isDouble)
    {
        Character character = Characters[CharIndex];
        Vector3 characterPosition = character.transform.position;
        Dice singleDie = Dice[(int)character.Type - 1];

        if (isDouble)
        {
            singleDie.gameObject.SetActive(false);
        }

        singleDie.IsDoubleDice = isDouble;
        singleDie.transform.position = new Vector3(characterPosition.x, singleDie.transform.position.y, characterPosition.z);
        singleDie.gameObject.SetActive(true);
        ChooseCharacter();
    }

    private void Move(Character character)
    {
        int roll = character.Roll;
        int originalPos = character.Position;
        character.Position = (originalPos + roll) % Spaces.Count;

        Queue<Circle> spaces = new Queue<Circle>();
        if (Turn == 0)
        {
            originalPos++;
            spaces.Enqueue(Spaces[originalPos]);
        }
        else
        {
            //show another character on your space if you leave it
            var startingSpace = Spaces[originalPos];
            startingSpace.OnSpace.Remove(character.Type);
            if (startingSpace.OnSpace.Any())
            {
                Characters.Find(c => c.Type == startingSpace.OnSpace.First()).Show();
            }
        }

        if (originalPos + 1 < Spaces.Count)
        {
            for (int i = originalPos + 1; i <= (character.Position < originalPos ? Spaces.Count - 1 : character.Position); i++)
            {
                spaces.Enqueue(Spaces[i]);
            }
        }
        if (character.Position < originalPos)
        {
            for (int i = 0; i <= character.Position; i++)
            {
                spaces.Enqueue(Spaces[i]);
            }
        }

        //walk towards this space
        character.WalkTowards(spaces);
    }

    private void AddCoinsText()
    {
        Dialog.ShowText("Each player starts with 5 coins", AddStartingCoins);
    }

    private void AddStartingCoins()
    {
        Turn++;
        foreach (Dice die in Dice)
        {
            die.gameObject.SetActive(false);
        }
        foreach (Character character in Characters)
        {
            Instantiate(Coin, character.gameObject.transform.position + Vector3.up * 3, Quaternion.identity);
        }
        Invoke("DoTurn", 1.5f);
    }

    private void NextTurn()
    {
        //update char index and turn
        CharIndex++;
        if (CharIndex >= Characters.Count)
        {
            CharIndex--;
            Turn++;
            Invoke("LoadMiniGame", 0.5f);
            return;
        }

        DoTurn();
    }

    private void CharacterJump()
    {
        Character character = Characters[CharIndex];
        character.CanJump = true;
        character.Jump();
    }

    private void BuyStar(bool buy)
    {
        Character character = GetCurrentCharacter();

        if (buy)
        {
            if (character.Coins < 20)
            {
                Dialog.ShowText("You don't have enough coins to get this star!", ContinueTurn);
            }
            else
            {
                character.ChangeCoins(-20, true);
                Invoke("AddStar", 1);
            }
        }
        else
        {
            ContinueTurn();
        }
    }

    private void StealStar(bool getCoins)
    {
        Character character = GetCurrentCharacter();

        if (getCoins)
        {
            Character stealCharacter = Characters.FindAll(c => c != character).OrderByDescending(c => c.Coins).FirstOrDefault();
            if (stealCharacter.Coins == 0)
            {
                Dialog.ShowText("There's nobody to steal coins from!", ContinueTurn);
            }
            else
            {
                StartCoroutine(AddStolenCoins(stealCharacter));
            }
        }
        else
        {
            if (character.Coins < 35)
            {
                Dialog.ShowText("You don't have enough coins to steal a star!", ContinueTurn);
            }
            else
            {
                Character stealCharacter = Characters.FindAll(c => c.Stars > 0 && c != character).OrderByDescending(c => c.Stars).FirstOrDefault();
                if (stealCharacter == null)
                {
                    Dialog.ShowText("There's nobody to steal a star from!", ContinueTurn);
                }
                else
                {
                    character.ChangeCoins(-35, true);
                    StartCoroutine(AddStolenStar(stealCharacter));
                }
            }
        }
    }

    private void SeeItems(bool see)
    {
        if (see)
        {
            bool upgradedItems = Turn >= MaxTurns / 2;
            var character = GetCurrentCharacter();
            if (character.IsPlayer)
            {
                if (character.Items.Count >= 3)
                {
                    Dialog.ShowText("You can only have 3 items at a time!", ContinueTurn);
                }
                else
                {
                    ItemsPanel.Show(character, upgradedItems, ReloadStatsAndContinueTurn);
                }
            }
            else
            {
                var possibleItems = ItemsPanel.GetItems(upgradedItems).FindAll(i => i.Cost <= character.Coins);
                ItemSelection selectedItem;
                var goldenPipe = possibleItems.Find(i => i.Type == ItemType.GoldenPipe);
                if (goldenPipe != null)
                {
                    selectedItem = goldenPipe;
                }
                else
                {
                    selectedItem = possibleItems[Random.Range(0, possibleItems.Count)];
                }

                character.ChangeCoins(-selectedItem.Cost, true);
                character.AddItem(selectedItem);

                FindObjectOfType<Dialog>().ShowText($"{character.Type} got a {selectedItem.ItemNameText.text}!", ReloadStatsAndContinueTurn);
            }
        }
        else
        {
            ContinueTurn();
        }
    }

    private void AddStar()
    {
        Character character = GetCurrentCharacter();

        character.ChangeStars(1);
        StarText.Show(character.transform.position + Vector3.up);
        Invoke("ShowRankings", 1.5f);
    }

    private IEnumerator AddStolenStar(Character stealCharacter)
    {
        yield return new WaitForSeconds(1);

        stealCharacter.ChangeStars(-1);
        Dialog.ShowText($"You stole a star from {stealCharacter.Type}!", AddStar);
    }

    private IEnumerator AddStolenCoins(Character stealCharacter)
    {
        yield return new WaitForSeconds(1);

        int coinsToSteal = Random.Range(1, stealCharacter.Coins / 3);
        stealCharacter.ChangeCoins(-coinsToSteal, false);

        Character character = GetCurrentCharacter();
        character.ChangeCoins(coinsToSteal, false);

        Dialog.ShowText($"You stole {coinsToSteal} coin{(coinsToSteal == 1 ? "" : "s")} from {stealCharacter.Type}!", ContinueTurn);
    }

    private void ContinueTurn()
    {
        Character character = GetCurrentCharacter();

        if (character.Destinations.Count == 0)
        {
            NextTurn();
        }
        else
        {
            foreach(Circle space in Spaces)
            {
                if (space.OnSpace.Contains(character.Type))
                {
                    space.OnSpace.Remove(character.Type);
                    if (space.OnSpace.Any())
                    {
                        Characters.Find(c => c.Type == space.OnSpace.First()).Show();
                    }
                }
            }

            character.WalkTowards(character.Destinations);
        }
    }

    private void ReloadStatsAndContinueTurn()
    {
        LoadAllCharacterStats(false);
        ContinueTurn();
    }

    private void ShowRankings()
    {
        Rankings.ShowRankings(ContinueTurn, new List<CharacterType> { CharacterType.Unknown }, false);
    }

    private void LoadMiniGame()
    {
        SaveController.Save(Characters, Spaces, Turn, MaxTurns, IsBoardReversed);
        EnterMiniGameSound.Play();
        FadePanel.FadeOut();
        Invoke("LoadChosenMiniGame", 1.5f);
    }

    private void LoadEndGame()
    {
        SceneManager.LoadSceneAsync("EndGame");
    }

    private void LoadChosenMiniGame()
    {
        List<string> miniGames = new List<string> { "Lava Jump", "Bumper Ball", "Shy Guy", "Air Hockey" };
        miniGames.Remove(LastMiniGame);
        SceneManager.LoadSceneAsync(miniGames[Random.Range(0, miniGames.Count)]);
    }

    private void ShowOpeningText()
    {
        Dialog.ShowText("Welcome! Get the most stars and coins to win!", ChooseCharacter);
    }

    private void ReverseBoard()
    {
        Spaces.Reverse();
        foreach (Character c in Characters)
        {
            c.Position = Spaces.Count - 1 - c.Position;
        }
    }

    private void UpdateFromSaveData(SaveData saveData)
    {
        foreach (Dice die in Dice)
        {
            die.gameObject.SetActive(false);
        }

        Turn = saveData.Turn;
        LastMiniGame = saveData.LastMiniGame;
        IsBoardReversed = saveData.BoardReversed;
        MaxTurns = saveData.TotalTurns;
        if (IsBoardReversed)
        {
            ReverseBoard();
        }

        foreach (Character character in Characters)
        {
            var savedCharacter = saveData.Characters.Find(c => c.Type == character.Type);
            character.transform.position = savedCharacter.WorldPosition;
            character.Position = savedCharacter.BoardPosition;
            character.Coins = savedCharacter.Coins;
            character.Stars = savedCharacter.Stars;
            character.IsPlayer = savedCharacter.IsPlayer;
            character.Roll = savedCharacter.RollPosition;
            character.Items = savedCharacter.Items;
            character.MiniGamesWon = savedCharacter.MiniGamesWon;
            character.SpacesWalked = savedCharacter.SpacesWalked;
        }

        for (int i = 0; i < Spaces.Count; i++)
        {
            Spaces[i].OnSpace = saveData.Spaces[i].OnSpace;
            if (Spaces[i].OnSpace.Count > 1)
            {
                for (int j = 1; j < Spaces[i].OnSpace.Count; j++)
                {
                    CharacterType onSpaceCharType = Spaces[i].OnSpace[j];
                    Characters.Find(c => c.Type == onSpaceCharType).Hide();
                }
            }
        }

        Characters = Characters.OrderBy(c => c.Roll).ToList();

        LoadAllCharacterStats(false);
    }
}