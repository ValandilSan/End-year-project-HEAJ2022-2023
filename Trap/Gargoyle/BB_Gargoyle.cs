using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace BagareBrian
{
    public class BB_Gargoyle : BB_TrapObserver
    {
        [Header("ColliderforFireDamage")]
        protected BoxCollider _GargoyleCollider;
        [SerializeField] private float _StartCollider, _EndCollider;
        [SerializeField] private float _SpeedOfThecollider;
        private Vector3 _StartPosition;
        private Vector3 _EndPosition;
        private bool _IsTheFirsTime;

        [Header("Audio")]
        [SerializeField] protected AudioSource _AudioSource;
        [SerializeField] protected List<AudioClip> _AudioClip;
        [Header("Gargoyles")]
        [SerializeField] protected GameObject _Gargoyles;
        [SerializeField] protected bool _IsGargoyleEnigma;
        [SerializeField] protected float _SpeedVFXGargoyle;
        [SerializeField] protected ParticleSystem _GargoyleParticles;
        [SerializeField] protected float _YOffsetForFireParticules;
        [SerializeField] protected ParticleSystem _FireParticles;
        [SerializeField] protected float _DurationOfTheParticules; // Need to Egalize With The Time of the littleParticules - the time of the flames
       

        private ParticleSystem _PrefabParticules;
        private bool _IsVFXActive;
        private Material _GargoylesMaterial;

        [Header("FlameThrower")]
        [SerializeField  ][Range(0, 1)] protected float _TimeToRelaunchAnim;
        [SerializeField] protected GameObject _FlameThrower;
        [SerializeField] protected float _FireSpeed;

        private Material _FlameMaterial;
      
        private bool _IsLaunchHell;
        private bool _IsTheLastfire;

        private Coroutine _LaunchDamages;
        protected bool _IsActive;
        public virtual bool IsGargoyleEnigma => _IsGargoyleEnigma;
        private void Start()
        {
            _Entities = this.GetComponent<Glo_Entities>();
            _IsActive = true;
            _IsTheLastfire = false;
            _FlameMaterial = this._FlameThrower.GetComponent<MeshRenderer>().material;
            
            _GargoylesMaterial = this._Gargoyles.GetComponent<MeshRenderer>().material;
            
            _IsVFXActive = false;
            _FlameMaterial.SetFloat("_ManualTime", 0);
            _GargoylesMaterial.SetFloat("_Activation", 0);
            _GargoyleCollider = this.GetComponent<BoxCollider>();
            _StartPosition = new Vector3(_GargoyleCollider.center.x, _GargoyleCollider.center.y, _StartCollider);
            _EndPosition = new Vector3(_GargoyleCollider.center.x, _GargoyleCollider.center.y, _EndCollider);
            _GargoyleCollider.center = _StartPosition;
        }

        #region Trigger
        private void OnTriggerEnter(Collider other)
        {
            if (!_IsActive)
            {
                if (other.tag == "Player")
                {
                    Glo_ITakeDamage playerDamage = other.GetComponentInParent<Glo_ITakeDamage>();
                    if (playerDamage != null)
                    {
                        if (_PrefabParticules == null)
                        {
                            Vector3 newPosition = new Vector3(other.transform.position.x, other.transform.position.y + _YOffsetForFireParticules, other.transform.position.z);
                           _PrefabParticules= Instantiate(_FireParticles, newPosition, Quaternion.identity, other.transform);
                           
                        }
                       
                        playerDamage.GetShot(FireDamage, Entities);
                        Debug.Log(playerDamage);
                    }
                }

            }

        }

        private void OnTriggerStay(Collider other)
        {

            if (!_IsActive)
            {
                if (other.tag == "Player")
                {
                    if (_PrefabParticules == null)
                    {
                        Vector3 newPosition = new Vector3(other.transform.position.x, other.transform.position.y + _YOffsetForFireParticules, other.transform.position.z);
                        _PrefabParticules = Instantiate(_FireParticles, newPosition, Quaternion.identity, other.transform);

                    }
                    if (_LaunchDamages == null)
                    {
                       
                        _LaunchDamages = StartCoroutine(DamagePerSecunds(other));
                    }
                    else
                    {
                        return;
                    }



                }

            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {


                if (_LaunchDamages != null && _IsActive)
                {
                    StopCoroutine(_LaunchDamages);
                    _LaunchDamages = null;
                }
                else
                {
                    return;
                }


            }

        }


        #endregion


        #region Coroutine
        IEnumerator DamagePerSecunds(Collider other)
        {

            yield return new WaitForSecondsRealtime(5f);
            Glo_ITakeDamage playerDamage = other.GetComponentInParent<Glo_ITakeDamage>();
            if (playerDamage != null)
            {
                playerDamage.GetShot(FireDamage, Entities);
                Debug.Log("Do Damage");
            }
            _LaunchDamages = null;
        }
        #endregion

        #region FlameThrower
        private void FireHell()
        {
            float currentFrame = _FlameMaterial.GetFloat("_ManualTime");
            currentFrame = Mathf.Clamp(currentFrame += Time.deltaTime * _FireSpeed, 0, 1);
            _FlameMaterial.SetFloat("_ManualTime", currentFrame);
           
            _GargoyleCollider.center = Vector3.Lerp(_GargoyleCollider.center, _EndPosition, Time.deltaTime * _SpeedOfThecollider);

            if (currentFrame >= _TimeToRelaunchAnim)
            {
                _AudioSource.clip = _AudioClip[Random.Range(0, _AudioClip.Count)];
                _AudioSource.Play();
                _FlameMaterial.SetFloat("_ManualTime", currentFrame - currentFrame);
                _GargoyleCollider.center = _StartPosition;
                if (_IsTheLastfire)
                {
                    _IsLaunchHell = false;
                }
            }
        }

      
        #endregion
        #region GargoylesVFX

        private void UpdateVFXGargoyle()
        {

            if (_IsVFXActive)
            {
                float currentValue = _GargoylesMaterial.GetFloat("_Activation");
                if (currentValue == 1)
                {
                    if (_IsTheFirsTime)
                    {
                        _IsTheFirsTime = false;
                        _AudioSource.clip = _AudioClip[Random.Range(0, _AudioClip.Count)];
                        _AudioSource.Play();
                        
                    }
                    return;
                }
                currentValue = Mathf.Clamp(currentValue += Time.deltaTime * _SpeedVFXGargoyle, 0, 1);
                _GargoylesMaterial.SetFloat("_Activation", currentValue);
            }

            if (!_IsVFXActive)
            {
                float currentValue = _GargoylesMaterial.GetFloat("_Activation");
                if (currentValue == 0)
                {
                   if (!_IsTheFirsTime)
                    {
                        _IsTheFirsTime = true;
                    }
                    return;
                }
                currentValue = Mathf.Clamp(currentValue -= Time.deltaTime * _SpeedVFXGargoyle, 0, 1);
                _GargoylesMaterial.SetFloat("_Activation", currentValue);
            }

        }
        #endregion
        #region Event
        private void OnEnable()
        {
            PressurePlateActive += ActiveFire;
            PressurePlateReactive += ReactiveTheTrap;
            GargoyleEventVFX += GargoyleVFX;
            BB_EnigmaManager.LoseEnigma += LoseEnigma;

        }



        private void OnDisable()
        {
            PressurePlateActive -= ActiveFire;
            PressurePlateReactive -= ReactiveTheTrap;
            GargoyleEventVFX -= GargoyleVFX;
            BB_EnigmaManager.LoseEnigma -= LoseEnigma;
        }


        #region Gargoyle Enigma
        private void LoseEnigma()
        {
            if (IsGargoyleEnigma)
            {
                if (_FlameThrower != null)
                {

                }
               
            }
        }
        #endregion

        #region GargoyleTrap

        #region ActiveVFX Gargoyles
        private void GargoyleVFX(float index, bool playerIn)
        {

            if (_IndexTrap == index && playerIn)
            {
                _IsVFXActive = true;
                _GargoyleParticles.Play();
            }
            else
            {
                _GargoyleParticles.Stop();
                _IsVFXActive = false;
            }
        }

        #endregion


        #region Pressureplate
        private void ActiveFire(float index)
        {

            if (!IsGargoyleEnigma && _IndexTrap == index)
            {
                if (_FlameThrower != null)
                {
                    _IsTheLastfire = false;
                    _IsLaunchHell = true;
                }

                _IsActive = false;
            }
            else
            {

            }


        }
        private void ReactiveTheTrap(float index)
        {

            if (!IsGargoyleEnigma && _IndexTrap == index)
            {
                if (_FlameThrower != null)
                {
                    _IsTheLastfire = true;
                }


                _IsActive = true;
            }
            else
            {

            }
        }
        #endregion



        #endregion


        #endregion
        private void Update()
        {
            if (_IsLaunchHell)
            {
                FireHell();
            }
            UpdateVFXGargoyle();

            if ( _PrefabParticules !=null && _PrefabParticules.isStopped )
            {
                Destroy(_PrefabParticules.gameObject,_DurationOfTheParticules);
                _PrefabParticules=null;
            }
        }
    }
}

