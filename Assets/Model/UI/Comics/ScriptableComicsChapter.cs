using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Comics/Chapter", order = 1)]
public class ScriptableComicsChapter : ScriptableObject
{
    public Sprite comicsButtonSprite;
    public List<ComicsPage> comicsPages;
}
