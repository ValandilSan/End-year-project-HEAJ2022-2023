using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagareBrian
{
    public class BB_EnnemyForkTrigger : MonoBehaviour
    {
        [SerializeField] private BB_EnnemyMelee _MeleeScript;
        private Collider _thisCollider;


        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                _MeleeScript.DoDamage(other);
            }
        }
    }
}

