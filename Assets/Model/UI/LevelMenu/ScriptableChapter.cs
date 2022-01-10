using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Levels/Chapter")]
public class ScriptableChapter : ScriptableObject
{
    public Sprite backgroundSprite;
    public List<string> levelNames;
}
