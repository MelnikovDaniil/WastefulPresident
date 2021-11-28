using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionMenu : BaseManager
{
    public static SelectionMenu Instance;
    public static bool isSelecting;
    public Canvas canvas;
    public float itemGap = 30;
    public SelectionMenuItem selectionMenuItemPrefab;
    public Transform menuCanvas;

    [NonSerialized]
    public List<SelectionMenuItem> selectionItems;

    private Vector3 startLoacalScale;

    private void Awake()
    {
        Instance = this;
        startLoacalScale = menuCanvas.localScale;
    }

    private void Update()
    {
        menuCanvas.localScale = startLoacalScale * Camera.main.orthographicSize / 9f;
    }

    public override void LoadManager()
    {
        selectionItems = new List<SelectionMenuItem>();
        canvas.worldCamera = Camera.main;
    }

    public void Show(InteractableObject interactableObject)
    {
        isSelecting = true;
        selectionItems.ForEach(x => x.gameObject.SetActive(false));
        selectionItems.ForEach(x => x.human.characterColor.gameObject.SetActive(true));
        menuCanvas.position = interactableObject.transform.position;
        var activeItems = selectionItems
            .Where(x => 
            (x.human.humanState == HumanState.Waiting || x.human.humanState == HumanState.Follow)
            && ((interactableObject.forCharacter && x.human.GetComponent<Character>())
                || (interactableObject.forAgent && x.human.GetComponent<Agent>())));

        var currentItemAngel = (activeItems.Count() - 1) * itemGap / 2 * -1;
        foreach (var item in activeItems)
        {
            item.transform.localEulerAngles = new Vector3(0, 0, currentItemAngel);
            currentItemAngel += itemGap;

            item.button.onClick.RemoveAllListeners();
            item.button.onClick.AddListener(() =>
            {
                var complexPositioning = interactableObject as IComplexPositioning;
                if (complexPositioning != null)
                {
                    var position = complexPositioning.GetPositionForInteraction(item.human);
                    item.human.SetTarget(position);
                }
                else
                {
                    item.human.SetTarget(interactableObject.transform.position);
                }
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
