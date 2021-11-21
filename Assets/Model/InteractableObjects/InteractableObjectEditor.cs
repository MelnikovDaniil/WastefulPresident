using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InteractableObject), true)]
public class InteractableObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var interactableObject = (InteractableObject)target;

        if (GUILayout.Button("Interact"))
        {
            interactableObject.StartInteraction(null);
        }
    }
}
