using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public List<CompressedCharacter> Characters;
    public int Turn;
    public string LastMiniGame;
    public bool BoardReversed;
    public List<CompressedCircle> Spaces;
    public CharacterType LastWinningCharacter;
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
}

[Serializable]
public class CompressedCircle
{
    public List<CharacterType> OnSpace = new List<CharacterType>();
}

