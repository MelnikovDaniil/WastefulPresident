using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class Sentance
{
    [TextArea]
    public string text;

    public Emotion emotion;
}
