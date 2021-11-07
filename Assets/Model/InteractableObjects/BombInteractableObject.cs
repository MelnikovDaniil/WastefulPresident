using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Model.InteractableObjects
{
    public class BombInteractableObject : InteractableObject
    {
        public Explosion explosionPrefab;
        public float explotionForce;
        public float explotionRadius;
        
        public override void Interect()
        {
            var exploasion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            exploasion.Expload(explotionForce, explotionRadius);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explotionRadius);
        }
    }
}
