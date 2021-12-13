using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemsPanel : MonoBehaviour
{
    public List<ItemSelection> ItemSelections;

    private int _curIndex;
    private Action _callback;
    private Character _currentCharacter;

    public void Show(Character character, bool showUpdatedItems, Action callback)
    {
        _callback = callback;
        _currentCharacter = character;

        if (showUpdatedItems)
        {
            ItemSelections[3].gameObject.SetActive(true);
            ItemSelections[2].gameObject.SetActive(false);
        }
        else
        {
            ItemSelections[3].gameObject.SetActive(false);
            ItemSelections[2].gameObject.SetActive(true);
        }

        gameObject.SetActive(true);
    }

    void Update()
    {
        var shownSelections = ItemSelections.FindAll(i => i != null);
        if (_curIndex < 2 && (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)))
        {
            shownSelections[_curIndex].Select(false);
            _curIndex++;
            shownSelections[_curIndex].Select(true);
        }
        else if (_curIndex > 0 && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)))
        {
            shownSelections[_curIndex].Select(false);
            _curIndex--;
            shownSelections[_curIndex].Select(true);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            var selectedItem = shownSelections[_curIndex];
            if (_currentCharacter.Coins < selectedItem.Cost)
            {
                return;
            }

            _currentCharacter.ChangeCoins(-selectedItem.Cost, true);
            _currentCharacter.AddItem(selectedItem);

            FindObjectOfType<Dialog>().ShowText($"You bought {selectedItem.ItemNameText.text}!", _callback);

            gameObject.SetActive(false);
        }
    }
}
