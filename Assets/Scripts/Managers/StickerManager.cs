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
        stickers.Add(sticker);
    }

    public void CloseSticker(string stickerName)
    {
        var sticker = stickers.FirstOrDefault(x => x.stickerName == stickerName);
        if (sticker != null)
        {
            stickers.Remove(sticker);
            Destroy(sticker.gameObject);
        }
        else
        {
            Debug.LogWarning($"Sticker {stickerName} not found!");
        }
    }
}
