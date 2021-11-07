using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Lift : InteractableObject
{
    public Vector2 position1;
    public Vector2 position2;
    public int direction;
    public float liftTime;
    public Transform humanPlace;

    public float currentTIme = -1;
    private List<Human> humans;
    private Character character;

    private void Awake()
    {
        humans = new List<Human>();
    }

    public void Update()
    {
        if (currentTIme >= 0 && currentTIme <= liftTime)
        {
            currentTIme += Time.deltaTime * direction;
            transform.position = Vector2.Lerp(position1, position2, currentTIme / liftTime);
            if (currentTIme < 0 || currentTIme > liftTime)
            {
                GetOutLift();
            }
        }
    }

    public void Up()
    {
        direction = 1;
        currentTIme = 0;
    }

    public void Down()
    {
        direction = -1;
        currentTIme = liftTime;
    }

    public override void Interect()
    {
        if (currentTIme < 0)
        {
            GetInLift();
            Up();
        }
        else if (currentTIme > liftTime)
        {
            GetInLift();
            Down();
        }
    }

    public void GetInLift()
    {
        var boxCollider = GetComponent<BoxCollider2D>();
        var colliders = Physics2D.OverlapBoxAll(boxCollider.offset + (Vector2)transform.position, boxCollider.size, 0);
        humans = colliders.Where(x => x.gameObject.GetComponent<Human>()).Select(x => x.gameObject.GetComponent<Human>()).ToList();
        humans.ForEach(x =>
        {
            x.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            x.transform.parent = humanPlace;
        });
        character = humans.FirstOrDefault(x => x.GetComponent<Character>())?.GetComponent<Character>();
        if (character != null)
        {
            character.isLocked = true;
        }
    }

    public void GetOutLift()
    {
        character.isLocked = false;
        humans.ForEach(x =>
        {
            x.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            x.transform.parent = null;
        });

        humans.Clear();
    }
}
