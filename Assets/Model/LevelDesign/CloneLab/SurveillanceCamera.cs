using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurveillanceCamera : MonoBehaviour
{
    public Transform rotatableBlock;
    public float rotationSpeed = 100;
    public LayerMask obstacleMask;

    private President president;
    private int cameraSide;
    private void Start()
    {
        president = FindObjectOfType<President>();
        cameraSide = Math.Sign(transform.localScale.x);
    }

    private void Update()
    {
        var obstacle = Physics2D.Linecast(rotatableBlock.position, president.transform.position, obstacleMask);
        if (obstacle.collider == null)
        {
            var direction = president.transform.position - rotatableBlock.position;
            var rotation = Quaternion.LookRotation(rotatableBlock.forward, direction) 
                * Quaternion.Euler(0, 0, 90 * cameraSide);

            rotatableBlock.rotation = Quaternion.RotateTowards(rotatableBlock.rotation, rotation, rotationSpeed * Time.deltaTime);
        }
    }
}
