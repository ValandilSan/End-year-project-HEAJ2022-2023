using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace BagareBrian
{
    public class BB_EnigmaAsset : MonoBehaviour
    {



        


        [SerializeField] private Material _AssetMaterial;
        [SerializeField] private List<AudioClip> _AudioClipList;
        [SerializeField] private AudioSource _AudioSource;


        [Header("displacement / Rotation")]
        [SerializeField] private int _RotationWhenLeverPulled;
        [SerializeField] private float _SpeedRotation;
        [SerializeField] private float _RotationMarge;

        private float _SuccedRotation;
        private BB_EnigmaManager _Script;

        private List<float> _ListOfRotationShuffle = new List<float>();
        private List<float> _Index = new List<float>();

        private Quaternion _SuccedQuaternionRotation;
        private Quaternion _StartQuaternionRotation;
        private Quaternion _EndQuaternionRotation;


        private int _WhatRotation;
        private float _RotationToHave;

       
        private bool _Issucced;
        private bool _IsRotate;
        private bool _IsShuffle;
        private bool _IsActiveDistincSign;



        private void Awake()
        {
            _Script = GetComponentInParent<BB_EnigmaManager>();
            _Script.AddObject(this);
            _Issucced = false;
            _SuccedRotation = transform.eulerAngles.y;

            _AssetMaterial.SetFloat("_Intensity", 0);

        }

        private void Start()
        {
            _SuccedQuaternionRotation = Quaternion.Euler(0, _SuccedRotation, 0);
            _RotationToHave = transform.rotation.y;
            _StartQuaternionRotation = gameObject.transform.rotation;
        

        }

        #region Events
        public void GivemeandIndex(float index)
        {
            _Index.Add(index);
          
        }
        private void EnigmaLose()
        {
            _IsActiveDistincSign = false;
          
        }
        private void LaunchVFX(float Index, Color MaterialColor)
        {

            foreach (var item in _Index)
            {
                if (item == Index)
                {
                    _AssetMaterial.SetColor("_FresnelColor", MaterialColor);
                    _IsActiveDistincSign = true;

                }
            }
        }

        private void DisableVFX(float Index)
        {
            foreach (var item in _Index)
            {
                if (item == Index)
                {
                    _IsActiveDistincSign = false;
                }
            }
        }
        private void OnEnable()
        {
            BB_LeverObserver.LeverEventForEnigma += TurnAssets;
            BB_EnigmaManager.LoseEnigma += EnigmaLose;
            BB_LeverObserver.LeverEventForAssetsEnigmaToLaunchVFX += LaunchVFX;
           
            BB_LeverObserver.LeverEventForAssetsEnigmaToDisableVFX += DisableVFX;
        }



        private void OnDisable()
        {
            BB_LeverObserver.LeverEventForEnigma -= TurnAssets;
            BB_EnigmaManager.LoseEnigma -= EnigmaLose;
            BB_LeverObserver.LeverEventForAssetsEnigmaToLaunchVFX -= LaunchVFX;
            BB_LeverObserver.LeverEventForAssetsEnigmaToDisableVFX -= DisableVFX;
        }
        #endregion

        #region Rotation Of TheAssets


        private void TurnAssets(float Index)
        {
            foreach (var item in _Index)
            {
                if (!_IsRotate && item == Index)
                {
                    _IsRotate = true;
                    _AudioSource.clip = _AudioClipList[Random.Range(0, _AudioClipList.Count)];
                    _AudioSource.Play();
                    NextRotation(_RotationWhenLeverPulled);




                    return;
                }

            }
           
            _Script.CountOnTheLastTry();
        }

        private void NextRotation(float angle)
        {
            _StartQuaternionRotation = transform.rotation;
            Quaternion rotationToDo = Quaternion.Euler(0, angle, 0);
            _EndQuaternionRotation = _StartQuaternionRotation * rotationToDo;
        }
        
        private void ChecktheSucced(float angle)
        {
            if (angle == _SuccedQuaternionRotation.y && !_Issucced)
            {
                _Issucced = true;
                
                _Script.IsSucced(_Issucced);
                return;
            }

           
        }
        private void Rotation()
        {
            if (_IsRotate)
            {
                if (_Issucced)
                {
                    _Issucced = false;
                  
                    _Script.IsSucced(_Issucced);
                }
               
                transform.rotation = Quaternion.Lerp(transform.rotation, _EndQuaternionRotation, Time.deltaTime * _SpeedRotation);
                float gapAngle = Quaternion.Angle(transform.rotation, _EndQuaternionRotation);

                if (gapAngle <= _RotationMarge)
                {

                    transform.rotation = _EndQuaternionRotation;
                    Quaternion angleEulersY = Quaternion.Euler(0, transform.eulerAngles.y, 0);

                    ChecktheSucced(angleEulersY.y);


                    _IsRotate = false;
                    if (!_IsRotate)
                    {
                        _Script.CountOnTheLastTry();
                    }
                   
                }

            }

            if (_IsShuffle)
            {
                if (_Issucced)
                {
                    _Issucced = false;
                  
                    _Script.IsSucced(_Issucced);
                }

                transform.rotation = Quaternion.Lerp(transform.rotation, _EndQuaternionRotation, Time.deltaTime * _SpeedRotation);

                float gapAngle = Quaternion.Angle(transform.rotation, _EndQuaternionRotation);

                if (gapAngle <= _RotationMarge)
                {
                    
                    transform.rotation = _EndQuaternionRotation;
                    if (_WhatRotation == _ListOfRotationShuffle.Count - 1)
                    {
                        
                        _StartQuaternionRotation = transform.rotation;
                        _WhatRotation = 0;
                        _ListOfRotationShuffle.Clear();
                        Quaternion angleEulersY = Quaternion.Euler(0, transform.eulerAngles.y, 0);
                        ChecktheSucced(angleEulersY.y);
                        _IsShuffle = false;
                        if (!_IsShuffle)
                        {
                            _Script.CountOnTheLastTry();
                        }
                        return;

                    }

                    _WhatRotation++;
                    NextRotation(_ListOfRotationShuffle[_WhatRotation]);
                }

            }

        }

        #endregion



        #region ShuffleAssets
        public void ShuffleAssets(int numberMaxOfRotation)
        {
            int numberOfrotation = Random.Range(1, numberMaxOfRotation);
            
            List<float> rotation = new List<float>();
            rotation.Add(0);
            rotation.Add(_RotationWhenLeverPulled);
            rotation.Add(_RotationWhenLeverPulled * 2);
            rotation.Add(_RotationWhenLeverPulled * 3);

            for (int i = 0; i < numberOfrotation; i++)
            {
                int howRotate = Random.Range(0, rotation.Count);

                _ListOfRotationShuffle.Add(rotation[howRotate]);

              
            }
            if (numberOfrotation != 0)
            {
                _WhatRotation = 0;
                NextRotation(_ListOfRotationShuffle[_WhatRotation]);
                _IsShuffle = true;
            }


        }

        #endregion


        private void DownAssetsVFX(float posOrNeg)
        {
            float currentValue = this._AssetMaterial.GetFloat("_Intensity");
            currentValue = Mathf.Clamp(currentValue += Time.deltaTime * posOrNeg, 0, 1);
            this._AssetMaterial.SetFloat("_Intensity", currentValue);
        }
        private void Update()
        {

            Rotation();

            if (_IsActiveDistincSign)
            {
                DownAssetsVFX(1);
            }
            if (!_IsActiveDistincSign)
            {
                DownAssetsVFX(-1);
            }
          
        }

    }
}

