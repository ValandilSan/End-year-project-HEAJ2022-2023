using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagareBrian
{
    public class BB_EnigmaOptimizer : MonoBehaviour
    {
        [SerializeField] private AnimationClip _BreakBallAnimation;
        [SerializeField] private AudioSource _AudioSource;
        [SerializeField] private List<AudioClip> _AudioClip;
        private bool _IsStatic;
        // Start is called before the first frame update
        void Start()
        {
            _IsStatic = false;
       
        }
        private void OnEnable()
        {
            _AudioSource.clip = _AudioClip[Random.Range(0, _AudioClip.Count)];
            _AudioSource.Play();
        }
        public void BecomeStatic()
        {
            if (!_IsStatic)
            {
                _IsStatic = true;
                gameObject.isStatic = _IsStatic;
            }
           
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}

