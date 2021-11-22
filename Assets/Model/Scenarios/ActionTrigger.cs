using UnityEngine;
using UnityEngine.Events;

public class ActionTrigger : MonoBehaviour
{
    [SerializeField]
    public UnityEvent unityEvent;
    public bool allowForPresident = true;
    public bool allowForSecurity = false;
    public bool singleRun;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((allowForPresident && collision.gameObject.GetComponent<Character>())
            || (allowForSecurity && collision.gameObject.GetComponent<Agent>()))
        {
            unityEvent?.Invoke();
            if (singleRun)
            {
                unityEvent.RemoveAllListeners();
            }
        }
    }
}
