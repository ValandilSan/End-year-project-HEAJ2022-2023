using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagareBrian
{
    public class BB_Sounds : MonoBehaviour
    {
        [SerializeField] private AudioSource _AudioSource;

        [SerializeField] private AudioClip _AudioClip;



        private void Start()
        {
            _AudioSource.clip = _AudioClip;
            _AudioSource.Play();
        }





    }

  
}

