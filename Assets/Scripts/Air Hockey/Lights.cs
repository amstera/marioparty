using System.Collections.Generic;
using UnityEngine;

public class Lights : MonoBehaviour
{
    public List<LightBulb> LightBulbs;
    public int Score;

    public void AddScore()
    {
        LightBulbs[Score].Activate();
        Score++;
    }
}
