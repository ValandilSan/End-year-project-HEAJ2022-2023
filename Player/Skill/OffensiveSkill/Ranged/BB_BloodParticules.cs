using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagareBrian
{
    public class BB_BloodParticules : MonoBehaviour
    {
        private ParticleCollisionEvent _ParticleCollisionEvent;
        private ParticleSystem _ParticleSystem;

        private Vector3 _Position;
        [SerializeField] private GameObject _Projectil;
        private BB_ProjectilPrefab _ProjectilPrefab;
        private void Start()
        {
            _ProjectilPrefab = _Projectil.GetComponent<BB_ProjectilPrefab>();
            _ParticleSystem = GetComponent<ParticleSystem>();
            _Position = new Vector3(transform.parent.position.x, transform.parent.position.y, transform.parent.position.z);
           

        }


        private void Update()
        {
            transform.position = _Position;
        }

    }
}

