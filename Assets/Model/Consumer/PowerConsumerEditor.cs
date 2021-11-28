using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(PowerConsumer), true)]
public class PowerConsumerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var powerConsumer = (PowerConsumer)target;

        if (GUILayout.Button("Turn energy"))
        {
            powerConsumer.TurnEnergy();
        }
    }
}
#endif
