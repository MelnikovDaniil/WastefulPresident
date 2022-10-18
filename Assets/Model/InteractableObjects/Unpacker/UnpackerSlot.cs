using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnpackerSlot : InteractableObject
{
    public Pipe pipe = new Pipe();
    public Vector2 boxDetectPosition;
    public Vector2 boxDetectSize;
    public LayerMask itemMask;
    public float pressureCheckPeriod = 0.5f;

    private LineRenderer pipeLine = new LineRenderer();

    private void Start()
    {
        CreatePipe();
        StartCoroutine(CheckBoxRoutine());
    }

    public override void StartInteraction(ICharacterVisitor visitor)
    {
        var item = visitor.GetItem();
        if (item is Box)
        {
            SoundManager.PlaySoundWithDelay("BatteryInput", 0.8f);
            visitor.PutItem();
            base.StartInteraction(visitor);
        }
        else
        {
            visitor.FinishVisiting();
        }
    }

    public override void SuccessInteraction(ICharacterVisitor visitor)
    {
        var box = visitor.GetItem() as Box;
        visitor.RemoveItem();
        Unpack(box);
    }

    private void CreatePipe()
    {
        var pipePrefab = Resources.Load<LineRenderer>("pipe");
        pipeLine = Instantiate(pipePrefab, transform);
        pipeLine.positionCount = pipe.points.Count;
        pipeLine.SetPositions(pipe.points.ToArray());
    }

    private void Unpack(Box box)
    {
        box.transform.parent = transform;
        box.transform.localPosition = Vector3.zero;
        box.transform.localRotation = Quaternion.identity;
        box.Hold();
        if (box.isStorePrefab)
        {
            var unpackedObj = Instantiate(box.storedObject, pipe.pipeOutput.GetOutputPosition(), Quaternion.identity);
            if (unpackedObj.TryGetComponent(out Character charecter))
            {
                ControllerManager.Instance.AddCharacter(charecter);
            }
        }
        else
        {
            box.storedObject.gameObject.SetActive(true);
            box.storedObject.gameObject.transform.position = pipe.pipeOutput.GetOutputPosition();
        }
        Destroy(box.gameObject);
    }

    private IEnumerator CheckBoxRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(pressureCheckPeriod);
            var collier = Physics2D.OverlapBox((Vector2)transform.position + boxDetectPosition, boxDetectSize, 0, itemMask);
            if (collier && collier.TryGetComponent(out Box box))
            {
                Unpack(box);
            }
        }
    }


    protected void OnDrawGizmos()
    {
        for (var i = 0; i < pipe.points.Count - 1; i++)
        {
            Gizmos.DrawLine(
                pipe.points[i] + transform.position,
                pipe.points[i + 1] + transform.position);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((Vector2)transform.position + boxDetectPosition, boxDetectSize);
    }
}
