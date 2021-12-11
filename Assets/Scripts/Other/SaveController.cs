using System.Collections.Generic;
using UnityEngine;

public class SaveController
{
    public static string SaveDataKey = "SaveData";

    public static void Save(List<Character> characters, List<Circle> spaces, int turn, bool isBoardReversed)
    {
        List<CompressedCharacter> compressedCharacters =  new List<CompressedCharacter>();
        int index = 0;
        foreach (var character in characters)
        {
            compressedCharacters.Add
            (
                new CompressedCharacter
                {
                    Type = character.Type,
                    WorldPosition = character.transform.position,
                    BoardPosition = character.Position,
                    Coins = character.Coins,
                    Stars = character.Stars,
                    RollPosition = index, 
                    IsPlayer = character.IsPlayer
                }
            );
            index++;
        }

        List<CompressedCircle> compressedSpaces = new List<CompressedCircle>();
        foreach (Circle space in spaces)
        {
            compressedSpaces.Add(
                new CompressedCircle
                {
                    OnSpace = space.OnSpace
                }
            );
        }

        SaveData saveData = new SaveData
        {
            Characters = compressedCharacters,
            Turn = turn,
            BoardReversed = isBoardReversed,
            Spaces = compressedSpaces
        };

        Save(saveData);
    }

    public static void Save(SaveData saveData)
    {
        var saveJson = JsonUtility.ToJson(saveData);

        PlayerPrefs.SetString(SaveDataKey, saveJson);
        PlayerPrefs.Save();
    }

    public static SaveData Load()
    {
        if (!PlayerPrefs.HasKey(SaveDataKey))
        {
            return null;
        }

        var saveJson = PlayerPrefs.GetString(SaveDataKey);
        if (string.IsNullOrEmpty(saveJson))
        {
            return null;
        }

        var saveData = JsonUtility.FromJson<SaveData>(saveJson);
        if (saveData.Characters == null || saveData.Characters.Count == 0)
        {
            return null;
        }

        return saveData;
    }
}
