using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagareBrian
{
    public class BB_LeverObserver : MonoBehaviour
    {


        [SerializeField] protected float _IndexLevelLever;
        [SerializeField] private AnimationClip _AnimationLever;
        [SerializeField] protected bool _IsActivable;
        [SerializeField] protected AudioSource _AudioSource;
        [SerializeField] protected List<AudioClip> _AudioClip;

        protected Animator _LeverAnimation;
        protected bool _IsLeverForEnigma;
        protected Collider _Collider;
        protected Material _LeverMaterial;

        private bool _IsPlayerIn = false;

        protected bool _IsDown = false;

        public delegate void LeverEvent(float Index);
        public static event LeverEvent LeverEventPullForGate;
        public static event LeverEvent LeverEventForEnigma;
        public static event LeverEvent LeverEventForAssetsEnigmaToDisableVFX;

        public delegate void LeverEventVFX(float Index, Color MaterialColor);
        public static event LeverEventVFX LeverEventForAssetsEnigmaToLaunchVFX;

        private bool _AvoidSpammingBouton;

        public virtual float IndexLever => _IndexLevelLever;
        public virtual bool EnigmaLever => _IsLeverForEnigma;
   
        public virtual Animator LeverAnimation => _LeverAnimation;

        public virtual Material LeverMaterial => _LeverMaterial;


        private void Start()
        {
            _Collider = this.GetComponent<Collider>();
            _LeverAnimation = this.gameObject.GetComponent<Animator>();
            if (LeverMaterial != null)
            {
                this.LeverMaterial.SetFloat("_Intensity", 0);
            }



            
            // CanIBeActivable = false;

        }


        #region EventEnigma
        private void OnEnable()
        {
            BB_EnigmaManager.AssetRotate += AssetsRotate;
            BB_EnigmaManager.AssetStopRotate += AssetsStopRotate;
            BB_EnigmaManager.WinEnigma += WinEnigma;
            BB_EnigmaManager.LoseEnigma += LoseEnigma;
            BB_StartAndEndOfTheRoom._MoveCamera += MoveCamera;
        }

        private void MoveCamera()
        {
          
            return;
        }

        private void OnDisable()
        {
            BB_EnigmaManager.AssetRotate -= AssetsRotate;
            BB_EnigmaManager.AssetStopRotate -= AssetsStopRotate;
            BB_EnigmaManager.WinEnigma -= WinEnigma;
            BB_EnigmaManager.LoseEnigma -= LoseEnigma;
            BB_StartAndEndOfTheRoom._MoveCamera -= MoveCamera;
        }

        private void WinEnigma()
        {

            if (this._IsLeverForEnigma)
            {
                this._IsActivable = false;
                this._IsPlayerIn = false;
                this._Collider.enabled = false;
                bool currentValue = LeverAnimation.GetBool("Down");
                if (!currentValue)
                {
                    _IsDown = !_IsDown;
                    LeverAnimation.SetBool("Down", _IsDown);

                }
            }
        }

        IEnumerator WaitToReloadLEver()
        {

            yield return new WaitForSecondsRealtime(_AnimationLever.length);
            this._IsActivable = true;
        }
        private void AssetsStopRotate()
        {
            if (this._IsLeverForEnigma)
            {
                bool currentValue = LeverAnimation.GetBool("Down");
                if (currentValue)
                {
                    _IsDown = !_IsDown;
                    _AudioSource.clip = _AudioClip[Random.Range(0, _AudioClip.Count)];
                    _AudioSource.Play();
                    LeverAnimation.SetBool("Down", _IsDown);

                }
                StartCoroutine(WaitToReloadLEver());

            }

        }
        private void LoseEnigma()
        {
            if (this._IsLeverForEnigma)
            {
                this._IsActivable = false;
            }

        }
        private void AssetsRotate()
        {
            if (this._IsLeverForEnigma)
            {

                this._IsActivable = false;
            }
        }
        #endregion



        public virtual void PulledLeverForWhat(float index, bool leverEnigma)
        {
            _AudioSource.clip = _AudioClip[Random.Range(0, _AudioClip.Count)];
            _AudioSource.Play();
            if (!leverEnigma)
            {
                LeverEventPullForGate?.Invoke(index);
            }
            
            if (leverEnigma)
            {
                LeverEventForEnigma?.Invoke(index);
            }
           
        }


        private void DownTheLeverVFX(float posOrNeg)
        {
            float currentValue = this.LeverMaterial.GetFloat("_Intensity");
            currentValue = Mathf.Clamp(currentValue += Time.deltaTime * posOrNeg, 0, 1);
            this.LeverMaterial.SetFloat("_Intensity", currentValue);
        }
        #region Collider

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _IsPlayerIn = true;

                if (this._IsLeverForEnigma && this._IsActivable )
                {
                    Color materialColor = this.LeverMaterial.GetColor("_ColorFresnel");
                    LeverEventForAssetsEnigmaToLaunchVFX?.Invoke(IndexLever, materialColor);
                }
            }

        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _IsPlayerIn = false;
                if (this._IsLeverForEnigma )
                {

                    LeverEventForAssetsEnigmaToDisableVFX?.Invoke(IndexLever);
                }
            }
        }

        #endregion


        IEnumerator StopSpammingBouton()
        {

            yield return new WaitForSecondsRealtime(1f);
            _AvoidSpammingBouton = false;
        }
        private void Update()
        {
            if (_IsPlayerIn)
            {
                DownTheLeverVFX(1);
               /* if (Input.GetButtonDown("LT")  )
                {
                    if (_IsActivable)
                    {
                        LeverAnimation.SetBool("Down", !_IsDown);
                        _IsDown = !_IsDown;
                        PulledLeverForWhat(IndexLever, EnigmaLever);
                        // CanIBeActivable = false;

                        Debug.Log("You Activate That");
                    }
                    else
                    {
                        Debug.Log("You CantActivate That");
                        return;
                    }
                }*/
                if (Input.GetAxis("RT") !=0.0f && !_AvoidSpammingBouton)
                {
                    if (_IsActivable)
                    {
                        _AvoidSpammingBouton = true;
                        StartCoroutine(StopSpammingBouton());
                        LeverAnimation.SetBool("Down", !_IsDown);
                        _IsDown = !_IsDown;
                        PulledLeverForWhat(IndexLever, EnigmaLever);
                        // CanIBeActivable = false;

                        Debug.Log("You Activate That");
                    }
                    else
                    {
                        Debug.Log("You CantActivate That");
                        return;
                    }
                }


            }
            if (!_IsPlayerIn && _LeverMaterial != null)
            {
                DownTheLeverVFX(-1);
            }
            if (Input.GetButtonDown("RB"))
            {
                Debug.Log("I Launch nothing");
            }

          
          
        }
    }
}

