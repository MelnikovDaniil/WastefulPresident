using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Comics/Chapter", order = 1)]
public class ScriptableComicsChapter : ScriptableObject
{
    public List<ComicsPage> comicsPages;
}
