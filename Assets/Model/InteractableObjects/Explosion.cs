using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Model.InteractableObjects
{
    public class Explosion : MonoBehaviour
    {
        private PointEffector2D pointEffector;
        private CircleCollider2D collider;
        private ParticleSystem particles;

        private void Awake()
        {
            pointEffector = GetComponent<PointEffector2D>();
            collider = GetComponent<CircleCollider2D>();
            particles = GetComponent<ParticleSystem>();
        }

        public void Expload(float force, float radius, float explotionTime = 0.2f)
        {
            particles.Play();
            pointEffector.forceMagnitude = force;
            transform.localScale *= radius;
            Destroy(gameObject, explotionTime);
        }
    }
}
