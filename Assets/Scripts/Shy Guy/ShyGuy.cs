using System.Collections;
using UnityEngine;

public class ShyGuy : MonoBehaviour
{
    public GameObject FlagA;
    public GameObject FlagB;

    public FlagType Flag;
    public float WaitForSeconds;
    public bool ShowingTwoFlags;

    public void ShowChosenFlag(FlagType flagType)
    {
        Flag = flagType;

        if (Random.Range(0, 2) == 1 && WaitForSeconds <= 2.75f)
        {
            ShowingTwoFlags = true;
            FlagA.SetActive(true);
            FlagB.SetActive(true);

            StartCoroutine(HideBadFlag(flagType));
        }

        if (Flag == FlagType.A)
        {
            FlagA.SetActive(true);
        }
        else
        {
            FlagB.SetActive(true);
        }
    }

    public void HideFlag()
    {
        Flag = FlagType.None;
        FlagA.SetActive(false);
        FlagB.SetActive(false);

    }

    private IEnumerator HideBadFlag(FlagType flag)
    {
        yield return new WaitForSeconds(Mathf.Min(0.5f, WaitForSeconds/2));

        Invoke("ShowingTwoFlagsFalse", 0.25f);
        if (Flag == FlagType.A)
        {
            FlagB.SetActive(false);
        }
        else
        {
            FlagA.SetActive(false);
        }
    }

    private void ShowingTwoFlagsFalse()
    {
        ShowingTwoFlags = false;
    }
}

public enum FlagType
{
    None = 0,
    A = 1,
    B = 2
}
