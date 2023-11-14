using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagareBrian
{
    public class BB_EnigmaLifeBalls : MonoBehaviour
    {
        [SerializeField] private AudioSource _AudioSource;
        [SerializeField] private List<AudioClip> _AudioClip;
        private void OnEnable()
        {
            _AudioSource.clip = _AudioClip[Random.Range(0, _AudioClip.Count)];
            _AudioSource.Play();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Ground" )
            {
                Destroy(gameObject);
            }
            if (other.tag == "Wall")
            {
                return;

            }
            else
            {
                _AudioSource.Stop();
            }
           
        }
    }

}

