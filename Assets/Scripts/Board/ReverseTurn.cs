using UnityEngine;
using UnityEngine.UI;

public class ReverseTurn : MonoBehaviour
{
    public Image Arrows;

    public void Show(bool isVisible)
    {
        Arrows.enabled = isVisible;
    }
}
