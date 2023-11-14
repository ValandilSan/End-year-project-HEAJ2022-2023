using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BagareBrian
{
    public class BB_ProjectilPrefab : MonoBehaviour
    {
        [SerializeField] private float _Speed;
        [SerializeField] private float _Size;
        [SerializeField] private float _SecundsToDestroy;
        [SerializeField] ParticleSystem _BloodParticules;

        [SerializeField] private float _SpeedOfTheDeform;
        [SerializeField] private float _TimeBeforeDestroy; //Match With The Duration Of The ParticulesSystems
        [SerializeField] private MeshRenderer _MeshRenderer;
        [SerializeField] private Collider _Collider;
        [SerializeField] private List<AudioClip> _ListAudio;
        [SerializeField] private AudioSource _AudioSource;

        private Material _material;
        private float _ChronoBeforeDestroy;
        private float _Damage;
        private Glo_Entities _Entities;
        private bool _IsActiveTheDeform;
        private void Start()
        {
            transform.localScale = new Vector3(_Size, _Size, _Size);
            _material = GetComponent<MeshRenderer>().material;
            _material.SetFloat("_ZDeform", 0);
        }

        public void giveInformation(float damage, Glo_Entities entities)
        {
            _Damage = damage;
            _Entities = entities;   
            _IsActiveTheDeform = true;
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Enemy")
            {

                Debug.Log("TouhcElseThanEnnemy");
                Glo_ITakeDamage Ennemy = other.GetComponentInParent<Glo_ITakeDamage>();

                if (Ennemy != null)
                {
                    _AudioSource.clip = _ListAudio[Random.Range(0, _ListAudio.Count)];
                    _AudioSource.Play();
                    _Speed = 0;
                    _BloodParticules.Play();
                    _MeshRenderer.enabled = false;
                    _Collider.enabled = false;
                    _IsActiveTheDeform = false;
                    Destroy(gameObject, _TimeBeforeDestroy);
                    Ennemy.GetShot(_Damage, _Entities);
                }
              //  Destroy(gameObject);
            }
            if (other.tag == "Wall")
            {
                _AudioSource.clip = _ListAudio[Random.Range(0, _ListAudio.Count)];
                _AudioSource.Play();
                _Speed = 0;
                _BloodParticules.Play();
                _MeshRenderer.enabled = false;
                _Collider.enabled = false;
                _IsActiveTheDeform = false;
                Destroy(gameObject, _TimeBeforeDestroy);
                //Destroy(gameObject);
            }


        }

        private void Update()
        {

            transform.Translate(Vector3.forward * _Speed * Time.deltaTime);
             
            
            if (_IsActiveTheDeform)
            {
                float currentValue = _material.GetFloat("_ZDeform");
                currentValue = Mathf.Clamp(currentValue += Time.deltaTime * _SpeedOfTheDeform, 0, 1);
                _material.SetFloat("_ZDeform", currentValue);
            }
        }



    }
}

