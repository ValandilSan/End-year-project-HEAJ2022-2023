using BagareBrian;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagareBrian
{
    public class BB_EnnemyFireGround : MonoBehaviour
    {
        [SerializeField] private float _FireSpeed;
        [SerializeField] private float _RadiusCollider;
        [SerializeField] private float _SpeedRadiusCollider;
        [SerializeField] protected float _YOffsetForFireParticules;
        [SerializeField] protected ParticleSystem _FireParticles;
        [SerializeField] protected float _DurationOfTheParticules; // Need to Egalize With The Time of the littleParticules - the time of the flames
        [SerializeField] private AudioSource _AudioSource;
        [SerializeField] private List<AudioClip> _Audioclip;
        [SerializeField] private ParticleSystem _Sparks;
        private BB_EnnemyRanged _MainEnnemyScript;
        private Material _FireGroudnMaterial;
        private SphereCollider _FireColliderForDamage;
        private ParticleSystem _CurrentSparks;
        private bool _IsFireDamage;
        private ParticleSystem _PrefabParticules;
        private bool _LastHit;
        private void Start()
        {
            _IsFireDamage = true;
            _FireGroudnMaterial = this.GetComponent<MeshRenderer>().material;
            _FireColliderForDamage = this.GetComponent<SphereCollider>();
            _FireColliderForDamage.radius = 0;
            _AudioSource.clip = _Audioclip[Random.Range(0, _Audioclip.Count)];
            _AudioSource.Play();
            _CurrentSparks = Instantiate(_Sparks);
            _CurrentSparks.transform.position = transform.position;
            _CurrentSparks.Play();
            _FireGroudnMaterial.SetFloat("_ManualTime", 0);
            _LastHit = false;
        }
        
        private void upgradeThecollider(float currentframe)
        {
            float currentSize = _FireColliderForDamage.radius;
            currentSize = Mathf.Clamp(currentSize += Time.deltaTime * currentframe * _SpeedRadiusCollider, 0, _RadiusCollider);
            _FireColliderForDamage.radius = currentSize;
        }
        private void LaunchHell()
        {
            float currentFrame = _FireGroudnMaterial.GetFloat("_ManualTime");
            currentFrame = Mathf.Clamp(currentFrame += Time.deltaTime * _FireSpeed, 0, 1);
            _FireGroudnMaterial.SetFloat("_ManualTime", currentFrame);
            upgradeThecollider(currentFrame);

            if (currentFrame >= 1)
            {
                _LastHit = true;

                Destroy(_CurrentSparks.gameObject, 1f);
                Destroy(gameObject, 1f);
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                if (_PrefabParticules == null)
                {
                    Vector3 newPosition = new Vector3(other.transform.position.x, other.transform.position.y + _YOffsetForFireParticules, other.transform.position.z);
                    _PrefabParticules = Instantiate(_FireParticles, newPosition, Quaternion.identity, other.transform);

                }
                _MainEnnemyScript.DoDamage(other, _LastHit);
            }
        }
        public void OnTriggerStay(Collider other)
        {
            if (other.tag == "Player")
            {
                if (_PrefabParticules == null)
                {
                    Vector3 newPosition = new Vector3(other.transform.position.x, other.transform.position.y + _YOffsetForFireParticules, other.transform.position.z);
                    _PrefabParticules = Instantiate(_FireParticles, newPosition, Quaternion.identity, other.transform);

                }
                _MainEnnemyScript.DoDamage(other, _LastHit);
            }

        }
        public void GiveMeInformation(BB_EnnemyRanged mainScript)
        {
            _MainEnnemyScript = mainScript;
        }
        private void Update()
        {
            if (_IsFireDamage)
            {
                LaunchHell();
            }
            if (_PrefabParticules != null && _PrefabParticules.isStopped)
            {
                Destroy(_PrefabParticules.gameObject, _DurationOfTheParticules);
                _PrefabParticules = null;
            }
        }
    }
}

