using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SecurityManager : MonoBehaviour
{
    public List<Security> securities;
    public Security targetSecurity;
    public Character character;

    public float followPeriod = 0.5f;
    public float securityDistacnceGap = 1;

    private float presidentDistance;

    private void Start()
    {
        presidentDistance = 2f;
        foreach (var security in securities)
        {
            security.presidentStopDistance = presidentDistance;
            presidentDistance += securityDistacnceGap;
        }

        StartCoroutine(FollowPreidentRoutine());
    }

    private void Update()
    {
        if (!DialogueManager.isWorking && !character.isDead && Input.GetMouseButtonDown(0))
        {
            SetUpTarget();
        }
    }
    public void AddSecurities(List<Security> newSecurities)
    {
        this.securities.AddRange(newSecurities);
        foreach (var security in securities)
        {
            security.presidentStopDistance = presidentDistance;
            presidentDistance += securityDistacnceGap;
        }
    }
    public void SetUpNextSecurity()
    {
        targetSecurity = securities.Where(x => x.isDead == false).GetRandomOrDefault();
        if (targetSecurity != null)
        {
            securities.Remove(targetSecurity);
            targetSecurity.OnDeath += () => targetSecurity = null;
        }
    }

    public void SetUpTarget()
    {
        var position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var hits = Physics2D.RaycastAll(position, position);
        var interactableObject = hits
            .Select(x => x.transform.gameObject)
            .FirstOrDefault(x => x.transform.gameObject.GetComponent<InteractableObject>());

        if (interactableObject != null && interactableObject.GetComponent<InteractableObject>().enabled)
        {
            if (targetSecurity == null)
            {
                SetUpNextSecurity();
            }

            if (targetSecurity == null)
            {
                Debug.Log("No any security alive");
            }
            else
            {
                targetSecurity.target = interactableObject.transform.position;
            }
        }
    }

    public IEnumerator FollowPreidentRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(followPeriod);
            FollowPresident();
        }
    }

    public void FollowPresident()
    {
        foreach (var security in securities)
        {
            security.FollowPresedent(character.transform.position);
        }
    }
}
