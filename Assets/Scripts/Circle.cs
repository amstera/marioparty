using UnityEngine;

public class Circle : MonoBehaviour
{
    public CircleType Type;
}

public enum CircleType
{
    Positive = 0,
    Negative = 1,
    Star = 2,
    Steal = 3
}
