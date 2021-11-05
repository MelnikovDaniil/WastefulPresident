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

    private void Start()
    {
        var x = 2f;
        foreach (var security in securities)
        {
            security.presidentStopDistance = x;
            x += securityDistacnceGap;
        }

        StartCoroutine(FollowPreidentRoutine());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SetUpTarget();
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
            character.SendOrder();
            var worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetSecurity.target = worldPosition;
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
