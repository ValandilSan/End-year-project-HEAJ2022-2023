using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagareBrian
{
    public class BB_DoorAudio : MonoBehaviour
    {
        [SerializeField] private AudioSource _AudioSource;
        [SerializeField] private List<AudioClip> _AudioList;


        public void LaunchASong()
        {
          
        }
        private void OnEnable()
        {
            _AudioSource.clip = _AudioList[Random.Range(0, _AudioList.Count) ];
            _AudioSource.Play();
           
        }
    }
}

