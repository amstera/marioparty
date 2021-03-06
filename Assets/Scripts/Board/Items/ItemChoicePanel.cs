using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ItemChoicePanel : MonoBehaviour
{
    public List<ItemChoice> ItemChoices;
    public Text Description;

    private int _choiceIndex;
    private Character _character;
    private ItemsPanel _itemsPanel;
    private Action _callback;
    private bool _choseItem;
    private int _turnsLeft;

    public AudioSource SelectAS;

    void Update()
    {
        if (_character.IsPlayer)
        {
            if (_choiceIndex < _character.Items.Count - 1 && (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)))
            {
                ItemChoices[_choiceIndex].Select(false);
                _choiceIndex++;
                ItemChoices[_choiceIndex].Select(true);

                UpdateDescription();

                SelectAS.Play();
            }
            else if (_choiceIndex > 0 && (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)))
            {
                ItemChoices[_choiceIndex].Select(false);
                _choiceIndex--;
                ItemChoices[_choiceIndex].Select(true);

                UpdateDescription();

                SelectAS.Play();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _character.UseItem(ItemChoices[_choiceIndex].Type);
                gameObject.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _callback?.Invoke();
                gameObject.SetActive(false);
            }
        }
        else if (!_choseItem)
        {
            ItemChoices[_choiceIndex].Select(false);
            int itemChoice;
            if (_character.Coins >= 20 && _character.Items.Any(c => c == ItemType.GoldenPipe))
            {
                itemChoice = ItemChoices.FindIndex(i => i.Type == ItemType.GoldenPipe);
            }
            else if (_turnsLeft < 5)
            {
                var userItems = _itemsPanel.ItemSelections.FindAll(i => _character.Items.Any(ui => ui == i.Type));
                if (_character.Coins < 20 && userItems.Count > 1)
                {
                    userItems.RemoveAll(u => u.Type == ItemType.GoldenPipe);
                }
                itemChoice = ItemChoices.FindIndex(i => i.Type == userItems.OrderByDescending(i => i.Cost).First().Type);
            }
            else
            {
                itemChoice = UnityEngine.Random.Range(0, _character.Items.Count);
            }
            _choiceIndex = itemChoice;
            ItemChoices[_choiceIndex].Select(true);

            UpdateDescription();

            if (_choiceIndex > 0)
            {
                SelectAS.Play();
            }

            _choseItem = true;
            Invoke("ChooseItem", 0.75f);
        }
    }

    public void ShowPanel(Character character, ItemsPanel itemsPanel, Action callback, int turnsLeft)
    {
        _character = character;
        _callback = callback;
        _itemsPanel = itemsPanel;
        _turnsLeft = turnsLeft;

        for (int i = 0; i < ItemChoices.Count; i++)
        {
            if (i + 1 > character.Items.Count)
            {
                ItemChoices[i].gameObject.SetActive(false);
                continue;
            }

            ItemChoices[i].gameObject.SetActive(true);
            var characterItem = character.Items[i];
            var matchingItem = itemsPanel.ItemSelections.Find(i => i.Type == characterItem);
            ItemChoices[i].Set(matchingItem.ItemNameText.text, matchingItem.ItemImage, matchingItem.Type);
            ItemChoices[i].Select(i == 0);
        }

        UpdateDescription();
        gameObject.SetActive(true);
    }

    private void ChooseItem()
    {
        _character.UseItem(ItemChoices[_choiceIndex].Type);
        _choseItem = false;
        gameObject.SetActive(false);
    }

    private void UpdateDescription()
    {
        var matchingItem = _itemsPanel.ItemSelections.Find(i => i.Type == ItemChoices[_choiceIndex].Type);
        Description.text = matchingItem.Description;
    }
}
