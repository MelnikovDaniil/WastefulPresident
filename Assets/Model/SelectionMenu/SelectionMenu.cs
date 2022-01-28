using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SelectionMenu : BaseManager
{
    public static SelectionMenu Instance;
    public static bool isSelecting;

    public UnityEvent OnSelection;

    public Canvas canvas;
    public float itemGap = 30;
    public SelectionMenuItem selectionMenuItemPrefab;
    public Transform menuCanvas;
    public Vector2 padding = new Vector2(4.5f, 1f);
    public float scalingByOrthiginicSize = 5f;

    [NonSerialized]
    public List<SelectionMenuItem> selectionItems;

    private Vector3 startLoacalScale;

    private void Awake()
    {
        Instance = this;
        startLoacalScale = menuCanvas.localScale;
        selectionItems = new List<SelectionMenuItem>();
    }

    private void Update()
    {
        menuCanvas.localScale = startLoacalScale * Camera.main.orthographicSize / scalingByOrthiginicSize * Camera.main.aspect;
    }

    public override void LoadManager()
    {
        Clear();
        selectionItems = new List<SelectionMenuItem>();
        canvas.worldCamera = Camera.main;
    }

    public void Show(InteractableObject interactableObject)
    {
        isSelecting = true;
        selectionItems.ForEach(x => x.gameObject.SetActive(false));
        selectionItems.ForEach(x => x.human.ShowColor());

        var menuPosition = new Vector2(
            Mathf.Clamp(interactableObject.transform.position.x, -Camera.main.orthographicSize * Camera.main.aspect + padding.x, Camera.main.orthographicSize * Camera.main.aspect - padding.x),
            Mathf.Clamp(interactableObject.transform.position.y, -Camera.main.orthographicSize + padding.y, Camera.main.orthographicSize - padding.y));
        menuCanvas.position = menuPosition;

        var activeItems = selectionItems
            .Where(x => 
            (x.human.humanState == HumanState.Waiting || x.human.humanState == HumanState.Follow || x.human.humanState == HumanState.Walking)
            && ((interactableObject.forCharacter && x.human.GetComponent<Character>())
                || (interactableObject.forAgent && x.human.GetComponent<Agent>())));

        var currentItemAngel = (activeItems.Count() - 1) * itemGap / 2 * -1;
        foreach (var item in activeItems)
        {
            item.transform.localEulerAngles = new Vector3(0, 0, currentItemAngel);
            item.faceIcon.transform.rotation = Quaternion.identity;
            currentItemAngel += itemGap;

            item.button.onClick.RemoveAllListeners();
            item.button.onClick.AddListener(() =>
            {
                OnSelection?.Invoke();
                ControllerManager.Instance.SendForInteraction(item.human, interactableObject);
                Hide();
            });
            item.gameObject.SetActive(true);
        }
    }

    public void SetUpNextCharacter(Human humanToInteract)
    {
        selectionItems.Where(x => x.human != humanToInteract).ToList()
            .ForEach(x => {
                x.button.interactable = false;
                x.faceIcon.color = x.button.colors.disabledColor;
            });
    }

    public void Hide()
    {
        isSelecting = false;
        foreach (var item in selectionItems)
        {
            item.faceIcon.color = Color.white;
            item.button.interactable = true;
            item.gameObject.SetActive(false);
            item.human.HideColor();
        }
    }

    private void Clear()
    {
        foreach (var item in selectionItems)
        {
            Destroy(item.gameObject);
        }
    }

    public void AddItem(Human human)
    {
        var selectionItem = Instantiate(selectionMenuItemPrefab, menuCanvas);
        selectionItem.faceIcon.sprite = human.icon;
        selectionItem.colorIcon.color = human.characterColor.color;
        selectionItem.human = human;
        selectionItem.gameObject.SetActive(false);
        selectionItems.Add(selectionItem);
    }
}
