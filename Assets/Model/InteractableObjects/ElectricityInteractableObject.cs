using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ElectricityInteractableObject : InteractableObject
{
    public GameObject closedDoor;
    public GameObject openDoor;

    public override void Interect()
    {
        closedDoor.SetActive(false);
        openDoor.SetActive(true);
        StickerManager.Instance.CloseSticker("DoorOpen");
        Destroy(gameObject);
    }
}
