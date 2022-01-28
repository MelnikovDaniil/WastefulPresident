using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class GuideStep : MonoBehaviour
{
    public Human humanToSelect;

    private void Start()
    {
        if (humanToSelect != null)
        {
            transform.parent = humanToSelect.transform;
            transform.localPosition = Vector2.zero;
        }
    }

    public UnityEvent action;

    public void Interact()
    {
        action?.Invoke();
    }
}
