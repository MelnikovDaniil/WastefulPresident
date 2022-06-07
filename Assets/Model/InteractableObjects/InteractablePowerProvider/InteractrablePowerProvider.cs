using System.Collections.Generic;
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

    public override void SuccessInteraction(ICharacterVisitor visitor)
    {
        isActive = !isActive;
        TurnEnergy();
        UpdateState();
    }

    public abstract void UpdateState();

    public void CreateWires()
    {
        var wirePrefab = Resources.Load<LineRenderer>("wire");
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

    protected void TurnEnergy()
    {
        foreach (var wire in wires)
        {
            wire.powerConsumer.TurnEnergy();
        }
    }

    protected void OnDrawGizmos()
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
