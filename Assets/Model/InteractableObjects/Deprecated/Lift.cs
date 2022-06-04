using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Lift : DeprecatedInteractableObject
{
    public Vector2 position1;
    public Vector2 position2;
    public int direction;
    public float liftTime;
    public Transform humanPlace;

    public float currentTIme = -1;
    private List<Character> characters;
    private President president;

    private void Awake()
    {
        characters = new List<Character>();
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
        characters = colliders.Where(x => x.gameObject.GetComponent<Character>()).Select(x => x.gameObject.GetComponent<Character>()).ToList();
        characters.ForEach(x =>
        {
            x.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            x.transform.parent = humanPlace;
        });
        president = characters.FirstOrDefault(x => x.GetComponent<President>())?.GetComponent<President>();
        if (president != null)
        {
            president.isLocked = true;
        }
    }

    public void GetOutLift()
    {
        president.isLocked = false;
        characters.ForEach(x =>
        {
            x.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            x.transform.parent = null;
        });

        characters.Clear();
    }
}
