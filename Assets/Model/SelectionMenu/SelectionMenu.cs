using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionMenu : MonoBehaviour
{
    public static SelectionMenu Instance;
    public static bool isSelecting;
    public float itemGap = 30;
    public SelectionMenuItem selectionMenuItemPrefab;
    public Transform menuCanvas;

    [NonSerialized]
    public List<SelectionMenuItem> selectionItems;


    private void Awake()
    {
        Instance = this;
        selectionItems = new List<SelectionMenuItem>();
    }

    public void Show(Vector2 position)
    {
        isSelecting = true;
        selectionItems.ForEach(x => x.gameObject.SetActive(false));
        selectionItems.ForEach(x => x.human.characterColor.gameObject.SetActive(true));
        menuCanvas.position = position;
        var activeItems = selectionItems.Where(x => x.human.humanState == HumanState.Waiting || x.human.humanState == HumanState.Follow);

        var currentItemAngel = (activeItems.Count() - 1) * itemGap / 2 * -1;
        foreach (var item in activeItems)
        {
            item.transform.localEulerAngles = new Vector3(0, 0, currentItemAngel);
            currentItemAngel += itemGap;

            item.button.onClick.RemoveAllListeners();
            item.button.onClick.AddListener(() =>
            {
                item.human.SetTarget(position);
                Hide();
            });
            item.gameObject.SetActive(true);
        }
    }

    public void Hide()
    {
        isSelecting = false;
        foreach (var item in selectionItems)
        {
            item.gameObject.SetActive(false);
            item.human.characterColor.gameObject.SetActive(false);
        }
    }

    public void AddItem(Human human)
    {
        var selectionItem = Instantiate(selectionMenuItemPrefab, menuCanvas);
        selectionItem.colorIcon.color = human.characterColor.color;
        selectionItem.human = human;
        selectionItem.gameObject.SetActive(false);
        selectionItems.Add(selectionItem);
    }
}
