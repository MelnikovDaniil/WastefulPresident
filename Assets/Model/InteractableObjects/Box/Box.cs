using UnityEngine;

public class Box : Item
{
    public bool isStorePrefab = true;
    public BoxedObject storedObject;
    public SpriteRenderer icon;

    public new void Start()
    {
        base.Start();
        if (isStorePrefab && storedObject != null)
        {
            icon.enabled = true;
            icon.sprite = storedObject.boxIcon;
        }
        else
        {
            icon.enabled = false;
        }
    }
}
