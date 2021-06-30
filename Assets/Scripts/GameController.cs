using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        int place = Characters.Count - 1 - ReloadIndex;
        var character = Characters[place];
        charStat.Reload(character, place);

        if (!charStat.gameObject.activeSelf)
        {
            charStat.gameObject.SetActive(true);
        }

        ReloadIndex++;
        Invoke("PopulateCharacterStatDelay", reloadDelay ? 0.5f : 0);
    }

    private void PopulateCharacterStatDelay()
    {
        PopulateCharacterStat(true);
    }
}
