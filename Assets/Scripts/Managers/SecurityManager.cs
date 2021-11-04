using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityManager : MonoBehaviour
{
    public List<Security> securities;
    public Security targetSecurity;
    public Character character;

    public float followPeriod = 0.5f;

    public Vector2 presidentStopRange = new Vector2(0.1f, 0.5f);

    private void Start()
    {
        securities.ForEach(x => x.presidentStopDistance = Random.Range(presidentStopRange.x, presidentStopRange.y));
        StartCoroutine(FollowPreidentRoutine());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SetUpTarget();
        }
    }

    public void SetUpTarget()
    {
        var worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetSecurity.target = worldPosition;
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
