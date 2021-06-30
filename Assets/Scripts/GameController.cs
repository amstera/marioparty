using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public List<Character> Characters;
    public int CharIndex;
    public int ReloadIndex;
    public bool GameStart;

    public List<CharacterStat> CharacterStats;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        ChooseCharacter();
    }

    public void NextCharacter()
    {
        if (GameStart)
        {

        }
        else if (CharIndex == Characters.Count - 1)
        {
            Characters = Characters.OrderByDescending(c => c.Roll).ToList();
            CharIndex = 0;
            PopulateCharacterStat(true);
        }
        else
        {
            CharIndex++;
            ChooseCharacter();
        }
    }

    private void ChooseCharacter()
    {
        Character character = Characters[CharIndex];
        if (Characters[CharIndex].IsPlayer)
        {
            character.CanJump = true;
        }
        else
        {
            character.CanJump = true;
            character.Jump();
        }
    }

    private void PopulateCharacterStat(bool reloadDelay)
    {
        if (ReloadIndex == Characters.Count)
        {
            ReloadIndex = 0;
            GameStart = true;
            NextCharacter();
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
            Invoke("PopulateCharacterStatDelay", 0.5f);
        }
        else
        {
            PopulateCharacterStat(false);
        }
    }

    private void PopulateCharacterStatDelay()
    {
        PopulateCharacterStat(true);
    }

    private int GetPlace(CharacterType type)
    {
        var orderedCharacters = Characters.OrderByDescending(c => c.Stars).ThenByDescending(c => c.Coins).ToList();
        int rank = 1;
        int currentStars = 0;
        int currentCoins = 0;
        foreach (Character character in orderedCharacters)
        {
            if (character.Type == type)
            {
                return rank;
            }
            if (character.Stars != currentStars)
            {
                currentStars = character.Stars;
                rank++;
            }
            else if  (character.Coins != currentCoins)
            {
                currentCoins = character.Coins;
                rank++;
            }
        }

        return 0;
    }
}
