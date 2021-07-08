using UnityEngine;
using UnityEngine.UI;

public class FadePanel : MonoBehaviour
{
    public Image Fade;

    private bool _shouldFadeIn;
    private bool _shouldFadeOut;

    void Update()
    {
        if (_shouldFadeOut)
        {
            var color = Fade.color;
            color.a += Time.deltaTime * 5;
            Fade.color = color;

            if (color.a >= 255)
            {
                _shouldFadeOut = false;
            }
        }
        else if (_shouldFadeIn)
        {
            var color = Fade.color;
            color.a -= Time.deltaTime * 5;
            Fade.color = color;

            if (color.a <= 0)
            {
                _shouldFadeIn = false;
            }
        }
    }

    public void FadeIn()
    {
        var color = Fade.color;
        color.a = 255;
        Fade.color = color;

        _shouldFadeIn = true;
    }

    public void FadeOut()
    {
        var color = Fade.color;
        color.a = 0;
        Fade.color = color;

        _shouldFadeOut = true;
    }
}
