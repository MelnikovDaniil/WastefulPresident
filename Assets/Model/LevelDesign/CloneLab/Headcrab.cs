using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Headcrab : MonoBehaviour
{
    public Vector2 changePosionMinMaxRate = new Vector2(5, 10);
    public float speed = 50;
    public float rotationSpeed = 50f;

    private Animator _animator;
    private Vector2? targetPosition;
    private SMSound sound;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        StartCoroutine(PositionGenerationRouting());
    }

    private void Update()
    {
        if (targetPosition.HasValue)
        {
            var direction = targetPosition.Value - (Vector2)transform.position;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition.Value, speed * Time.deltaTime);
            var rotation = Quaternion.LookRotation(transform.forward, direction);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, targetPosition.Value) < 0.1f)
            {
                sound?.Stop();
                targetPosition = null;
            }
        }
    }

    private void SetNextPosition()
    {
        sound = SoundManager.PlaySound("Headcrab")
            .SetVolume(0.5f);
        var camera = Camera.main;
        var y = Random.Range(-camera.orthographicSize, camera.orthographicSize);
        var x = Random.Range(-camera.orthographicSize, camera.orthographicSize) / Camera.main.aspect;
        targetPosition = new Vector2(x, y);
    }

    private IEnumerator PositionGenerationRouting()
    {
        while (true)
        {
            SetNextPosition();
            yield return new WaitForSeconds(Random.Range(changePosionMinMaxRate.x, changePosionMinMaxRate.y));
        }
    }
}
