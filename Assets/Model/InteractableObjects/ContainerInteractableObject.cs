using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class ContainerInteractableObject : InteractableObject
{
    public List<InventoryItem> inventoryItems;
    public GameObject itemPrefab;
    public float itemAnimationDelay = 0.75f;
    public Vector3 offest = new Vector2(0, 2);
    public UnityEvent onOpenAction;
    public bool singleActivation;
    private bool activated;

    public override void Interect()
    {
        if (!activated || !singleActivation)
        {
            onOpenAction?.Invoke();
            activated = true;
            StartCoroutine(TakingItemsRoutine());
        }
    }

    private IEnumerator TakingItemsRoutine()
    {
        foreach (var item in inventoryItems)
        {
            var createdItem = Instantiate(itemPrefab, transform.position + offest, Quaternion.identity);
            createdItem.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = item.sprite;
            Destroy(createdItem, itemAnimationDelay);
            InventoryManager.Instance.AddItem(item);
            yield return new WaitForSeconds(itemAnimationDelay);
        }

    }
}
