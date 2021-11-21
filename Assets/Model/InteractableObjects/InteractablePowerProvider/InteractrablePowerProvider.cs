using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public abstract class InteractrablePowerProvider : InteractableObject
{
    public bool isActive;
    public List<Wire> wires = new List<Wire>();
    public Color wireColor;

    private List<LineRenderer> wireLines = new List<LineRenderer>();

    public void Start()
    {
        UpdateState();
        CreateWires();
    }

    public override void SuccessInteraction(IVisitor visitor)
    {
        isActive = !isActive;
        TurnEnergy();
        UpdateState();
    }

    public abstract void UpdateState();

    public void CreateWires()
    {
        var wirePrefab = AssetDatabase.LoadAssetAtPath<LineRenderer>("Assets/Prefab/InteractableObjects/PowerProvider/wire.prefab");
        foreach (var wire in wires)
        {
            var spawnedWire = Instantiate(wirePrefab, transform);
            wireLines.Add(spawnedWire);
            spawnedWire.startColor = wireColor;
            spawnedWire.endColor = wireColor;
            spawnedWire.positionCount = wire.points.Count;
            spawnedWire.SetPositions(wire.points.ToArray());
        }
    }

    private void TurnEnergy()
    {
        foreach (var wire in wires)
        {
            wire.powerConsumer.TurnEnergy();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = wireColor;
        foreach (var wire in wires)
        {
            for (var i = 0; i < wire.points.Count - 1; i++)
            {
                Gizmos.DrawLine(
                    wire.points[i] + transform.position,
                    wire.points[i + 1] + transform.position);
            }
        }
    }
}
