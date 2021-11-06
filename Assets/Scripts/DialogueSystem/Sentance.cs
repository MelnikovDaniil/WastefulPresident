﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Sentance
{
    [TextArea]
    public string text;

    public Emotion emotion;

    public UnityEvent action;
}
