using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class StartScreenScroll : ScrollRect
{
    public event Action OnScrollFinished;
    public float movementTime { get; set; } = 0.2f;
    public float scrollSwitchPosition { get; set; } = 0.85f;

    private float currentMovementTime;
    private float firstPosition;
    private float secondPosition;

    private void Update()
    {
        if (currentMovementTime > 0)
        {
            currentMovementTime -= Time.deltaTime;
            verticalNormalizedPosition = firstPosition + (1.0f - currentMovementTime / movementTime) * (secondPosition - firstPosition);
            if (currentMovementTime <= 0)
            {
                verticalNormalizedPosition = secondPosition;
                if (secondPosition == 0)
                {
                    OnScrollFinished?.Invoke();
                }
            }
            velocity = Vector2.zero;
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        velocity = Vector2.zero;
        currentMovementTime = movementTime;
        firstPosition = verticalNormalizedPosition;
        Debug.Log("Scroll value is = " + verticalNormalizedPosition);
        //velocity = Vector2.zero;
        if (verticalNormalizedPosition < scrollSwitchPosition)
        {
            secondPosition = 0;
        }
        else
        {
            secondPosition = 1;
        }
        
    }

}
