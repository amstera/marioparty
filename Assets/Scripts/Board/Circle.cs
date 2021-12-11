using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour
{
    public CircleType Type;
    public List<CharacterType> OnSpace = new List<CharacterType>();
}

public enum CircleType
{
    Positive = 0,
    Negative = 1,
    Star = 2,
    Boo = 3,
    Event = 4,
    Item = 5
}
