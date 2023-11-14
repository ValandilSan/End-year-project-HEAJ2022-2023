using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagareBrian
{
    public class BB_EnigmaBalls : MonoBehaviour
    {

     
        [SerializeField] private GameObject _MeshToExplode;
        [SerializeField] private GameObject _MeshNotToExplode;
        [SerializeField] private AudioSource _AudioSource;
        [SerializeField] private List<AudioClip> _AudioClip;

        private bool _IsSucced;
        private BB_EnigmaManager _EnigmaManager;
        private void Start()
        {
        
            _MeshToExplode.SetActive(false);

        }

        public void GiveMeInformation(GameObject ground, BB_EnigmaManager enigmaScript)
        {
           // _Ground = Ground;
            _EnigmaManager = enigmaScript;
        }

        private void OnEnable()
        {
            _AudioSource.clip = _AudioClip[Random.Range(0, _AudioClip.Count)];
            _AudioSource.Play();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Ground"))
            {
                Vector3 positionToSpawn = new Vector3(transform.position.x, other.transform.position.y, transform.position.z);
                GameObject prefab = Instantiate(_MeshToExplode, positionToSpawn, Quaternion.identity);
                prefab.SetActive(true);
                Animator prefabAnimator = prefab.GetComponent<Animator>();
                prefabAnimator.SetBool("IsExplode", true);
                _EnigmaManager.AddToTheList(prefab);
                _EnigmaManager.IsSuccedOrNot(false);
                Destroy(gameObject);


            }
            if (other.CompareTag("Finish"))
            {
                if (!_IsSucced)
                {
                    _EnigmaManager.IsSuccedOrNot(true);
                    _IsSucced = true;
                }
                return;

            }
        }


    }
}

