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

    public AudioSource EnterMiniGameSound;

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
            }
            Dialog.ShowText("Welcome! Get the most stars and coins to win!", ChooseCharacter);
        }
        else
        {
            UpdateFromSaveData(saveData);
            Rankings.ShowRankings(DoTurn, saveData.LastWinningCharacter, false);
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
            Move(character);
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
        charStat.Reload(character, place);

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
        else if (space.Type == CircleType.Star)
        {
            Question.Show("Get this star for 20 coins?", character.IsPlayer ? AIChoice.None : character.Coins >= 20 ? AIChoice.Yes : AIChoice.No, BuyStar);
        }
        else if (space.Type == CircleType.Item)
        {
            Question.Show("Steal someone's star for 40 coins?", character.IsPlayer ? AIChoice.None : character.Coins >= 40 ? AIChoice.Yes : AIChoice.No, StealStar);
        }
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
                Dialog.ShowText($"5 turns left, spaces are worth double!", ShowCharacterStart);
                return;
            }
        }

        ShowCharacterStart();
    }

    private void ShowCharacterStart()
    {
        Dialog.ShowText($"{Characters[CharIndex].Type}, start!", SetUpDice);
    }

    private void SetUpDice()
    {
        Character character = Characters[CharIndex];
        Vector3 characterPosition = character.transform.position;
        GameObject die = Dice[(int)character.Type - 1].gameObject;

        die.transform.position = new Vector3(characterPosition.x, die.transform.position.y, characterPosition.z);
        die.SetActive(true);
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
            spaces.Enqueue(Spaces[originalPos]);
            character.Position--;
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

    private void StealStar(bool buy)
    {
        Character character = GetCurrentCharacter();

        if (buy)
        {
            if (character.Coins < 40)
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
                    character.ChangeCoins(-40, true);
                    StartCoroutine(AddStolenStar(stealCharacter));
                }
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

        stealCharacter.Stars--;
        Dialog.ShowText($"You stole a star from {stealCharacter.Type}!", AddStar);
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
            character.WalkTowards(character.Destinations);
        }
    }

    private void ShowRankings()
    {
        Rankings.ShowRankings(ContinueTurn, CharacterType.Unknown, false);
    }

    private void LoadMiniGame()
    {
        SaveController.Save(Characters, Turn);
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
        List<string> miniGames = new List<string> { "Lava Jump", "Bumper Ball" };
        miniGames.Remove(LastMiniGame);
        SceneManager.LoadSceneAsync(miniGames[Random.Range(0, miniGames.Count)]);
    }

    private void UpdateFromSaveData(SaveData saveData)
    {
        foreach (Dice die in Dice)
        {
            die.gameObject.SetActive(false);
        }

        Turn = saveData.Turn;
        LastMiniGame = saveData.LastMiniGame;
        foreach (Character character in Characters)
        {
            var savedCharacter = saveData.Characters.Find(c => c.Type == character.Type);
            character.transform.position = savedCharacter.WorldPosition;
            character.Position = savedCharacter.BoardPosition;
            character.Coins = savedCharacter.Coins;
            character.Stars = savedCharacter.Stars;
            character.IsPlayer = savedCharacter.IsPlayer;
            character.Roll = savedCharacter.RollPosition;
        }

        Characters = Characters.OrderBy(c => c.Roll).ToList();

        LoadAllCharacterStats(false);
    }
}