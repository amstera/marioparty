using System;
using System.Collections.Generic;
using UnityEngine;

public class LuckySpace : MonoBehaviour
{
    public List<LuckySpaceOption> LuckySpaces;

    private Action _callback;
    private Character _character;
    private int _chosenIndex;
    private int _showingIndex;
    private int _rotatingSelectionCount;
    private List<ItemSelection> _itemSelections;
    private Dialog _dialog;

    public AudioSource SelectionAS;

    public void Show(Character character, ItemsPanel itemsPanel, Dialog dialog, Action callback, bool isLastTurn)
    {
        foreach (var luckySpace in LuckySpaces)
        {
            luckySpace.Select(false);
        }

        gameObject.SetActive(true);

        _character = character;
        _callback = callback;
        _itemSelections = itemsPanel.ItemSelections;
        _dialog = dialog;

        _chosenIndex = UnityEngine.Random.Range(0, isLastTurn || character.Items.Count >= 3 ? 3 : LuckySpaces.Count);
        if (LuckySpaces[_chosenIndex].Type == LuckySpaceOptionType.GoldenPipe)
        {
            if (UnityEngine.Random.Range(0, 2) == 1) //make it hard to get golden pipe
            {
                _chosenIndex = UnityEngine.Random.Range(0, LuckySpaces.Count - 1); //take another random choice that isn't golden pipe
            }
        }

        _rotatingSelectionCount = 0;
        _showingIndex = 0;

        Invoke("DoRotatingSelection", 0.25f);
    }

    private void DoRotatingSelection()
    {
        LuckySpaces[_showingIndex].Select(false);
        _showingIndex = (_showingIndex + 1) % LuckySpaces.Count;
        LuckySpaces[_showingIndex].Select(true);

        _rotatingSelectionCount++;

        if (_rotatingSelectionCount > 15 && _showingIndex == _chosenIndex)
        {
            SelectionAS.pitch = 1.25f;
            SelectionAS.Play();

            Invoke("GiveReward", 0.5f);
        }
        else
        {
            SelectionAS.pitch = 1f;
            SelectionAS.Play();

            Invoke("DoRotatingSelection", _rotatingSelectionCount < 10 ? 0.125f : 0.35f);
        }
    }

    private void GiveReward()
    {
        ItemSelection itemSelection;
        switch (LuckySpaces[_chosenIndex].Type)
        {
            case LuckySpaceOptionType.Coins5:
                _character.ChangeCoins(5, false);
                _dialog.ShowText($"{_character.Type} got 5 coins!", _callback);
                break;
            case LuckySpaceOptionType.Coins10:
                _character.ChangeCoins(10, false);
                _dialog.ShowText($"{_character.Type} got 10 coins!", _callback);
                break;
            case LuckySpaceOptionType.Coins15:
                _character.ChangeCoins(15, false);
                _dialog.ShowText($"{_character.Type} got 15 coins!", _callback);
                break;
            case LuckySpaceOptionType.Mushroom:
                itemSelection = _itemSelections.Find(i => i.Type == ItemType.Mushroom);
                _character.AddItem(itemSelection);
                _dialog.ShowText($"{_character.Type} got a Mushroom!", _callback);
                break;
            case LuckySpaceOptionType.DoubleDice:
                itemSelection = _itemSelections.Find(i => i.Type == ItemType.DoubleDice);
                _character.AddItem(itemSelection);
                _dialog.ShowText($"{_character.Type} got a Double Dice!", _callback);
                break;
            case LuckySpaceOptionType.GoldenPipe:
                itemSelection = _itemSelections.Find(i => i.Type == ItemType.GoldenPipe);
                _character.AddItem(itemSelection);
                _dialog.ShowText($"{_character.Type} got a Golden Pipe!", _callback);
                break;
        }

        gameObject.SetActive(false);
    }
}
