using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public List<CompressedCharacter> Characters;
    public int Turn;
    public int TotalTurns;
    public string LastMiniGame;
    public int[] MiniGameFrequency;
    public bool BoardReversed;
    public List<CompressedCircle> Spaces;
    public List<CharacterType> LastWinningCharacters;
}

[Serializable]
public class CompressedCharacter
{
    public CharacterType Type;
    public Vector3 WorldPosition;
    public int BoardPosition;
    public int Coins;
    public int Stars;
    public int RollPosition;
    public bool IsPlayer;
    public int MiniGamesWon;
    public int SpacesWalked;
    public List<ItemType> Items;
}

[Serializable]
public class CompressedCircle
{
    public List<CharacterType> OnSpace = new List<CharacterType>();
}

