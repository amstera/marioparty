using UnityEngine;

public class ShyGuy : MonoBehaviour
{
    public GameObject FlagA;
    public GameObject FlagB;

    public FlagType Flag;

    public void ShowChosenFlag(FlagType flagType)
    {
        Flag = flagType;

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
}

public enum FlagType
{
    None = 0,
    A = 1,
    B = 2
}
