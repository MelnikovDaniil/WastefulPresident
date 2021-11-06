using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StickerManager : MonoBehaviour
{
    public static StickerManager Instance;
    public List<StickerView> stickers;
    public StickerView stickerPrefab;
    public Transform stickerPlace;

    public void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        stickers = new List<StickerView>();
    }

    public void AddSticker(string name, string description)
    {
        var sticker = Instantiate(stickerPrefab, stickerPlace);
        sticker.stickerName = name;
        sticker.description.text = description;
    }

    public void CloseSticker(string name)
    {
        var sticker = stickers.FirstOrDefault(x => x.stickerName == name);
        if (sticker != null)
        {
            stickers.Remove(sticker);
            Destroy(sticker.gameObject);
        }
        else
        {
            Debug.LogWarning($"Sticker {name} not found!");
        }
    }
}
