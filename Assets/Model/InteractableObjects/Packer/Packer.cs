using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Packer : InteractableObject
{
    public Pipe pipe = new Pipe();
    public Box boxPrefab;

    private LineRenderer pipeLine = new LineRenderer();

    private void Start()
    {
        CreatePipe();
    }

    public override void SuccessInteraction(ICharacterVisitor visitor)
    {
        var boxedObject = visitor.GetBoxedObject();
        if (boxedObject)
        {
            boxedObject.gameObject.SetActive(false);
            var box = Instantiate(boxPrefab, pipe.pipeOutput.GetOutputPosition(), Quaternion.identity);
            box.storedObject = boxedObject;
            box.isStorePrefab = false;
        }
    }

    private void CreatePipe()
    {
        var pipePrefab = Resources.Load<LineRenderer>("pipe");
        pipeLine = Instantiate(pipePrefab, transform);
        pipeLine.positionCount = pipe.points.Count;
        pipeLine.SetPositions(pipe.points.ToArray());
    }

    protected void OnDrawGizmos()
    {
        for (var i = 0; i < pipe.points.Count - 1; i++)
        {
            Gizmos.DrawLine(
                pipe.points[i] + transform.position,
                pipe.points[i + 1] + transform.position);
        }
    }
}
