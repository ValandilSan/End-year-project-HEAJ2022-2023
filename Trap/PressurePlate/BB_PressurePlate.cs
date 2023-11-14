using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BagareBrian
{
    public class BB_PressurePlate : BB_TrapObserver
    {
       
        [Header("Pressure Plate Parameters")]
        [SerializeField] protected float _SpeedOfThePlate;
        [SerializeField] protected float _PourcentToReachToActiveTrap;
        [SerializeField] protected float _PourcentToReactiveTrap;
        [SerializeField] private AudioSource _AudioSource;
        [SerializeField] private List<AudioClip> _AudioClipList;


        private Vector3 _OriginalPosition;
        private Vector3 _Currentposition;
        private Vector3 _PressedPosition;


       


        private bool _IsPressed = false;
        [SerializeField] private bool _IsPlayerIn = false;


        private bool _IsActiveOrNot = false;

        private void Awake()
        {
            
            _OriginalPosition = this.transform.position;

            _PressedPosition = new Vector3(transform.position.x, transform.position.y - transform.localScale.y, transform.position.z);
            _Currentposition = _OriginalPosition;
        }



        #region Collider

        private void OnTriggerEnter(Collider other)
        {

            _IsPlayerIn = true;
            ActiveGargoyleVFX(Index, _IsPlayerIn);
        }
        private void OnTriggerExit(Collider other)
        {

            _IsPlayerIn = false;
            ActiveGargoyleVFX(Index, _IsPlayerIn);
        }
        #endregion



        #region Displacement
        private void GoUp()
        {
            float distanceBetweenInPourcent = (((_Currentposition.y - _PressedPosition.y) / (_OriginalPosition.y - _PressedPosition.y)) * 100);

            if (_IsPlayerIn)
            {
                _Currentposition = this.transform.position;

                transform.position = Vector3.Lerp(_Currentposition, _PressedPosition, _SpeedOfThePlate * Time.deltaTime);

                if (distanceBetweenInPourcent <= _PourcentToReachToActiveTrap && !_IsPressed)
                {
                    _IsPressed = true;
                    WhatTrapToActive(_IndexTrap);
                    Debug.Log("LauncTheTrap");
                }


            }
            if (!_IsPlayerIn)
            {
                _Currentposition = this.transform.position;
                transform.position = Vector3.Lerp(_Currentposition, _OriginalPosition, _SpeedOfThePlate * Time.deltaTime);


                if (distanceBetweenInPourcent > _PourcentToReactiveTrap && _IsPressed)
                {
                    _IsPressed = false;
                    ReactiveTrap(_IndexTrap);
                    Debug.Log("ReactiveTrap");
                }


            }


        }
        #endregion


        #region Override For Event

        public override void WhatTrapToActive(float Index)
        {

            base.WhatTrapToActive(Index);
        }

        public override void ReactiveTrap(float Index)
        {
            base.ReactiveTrap(Index);
        }

        public override void ActiveGargoyleVFX(float Index, bool PlayerIn)
        {

            base.ActiveGargoyleVFX(Index, PlayerIn);
        }
        #endregion

        private void Update()
        {
            GoUp();


            if (_IsPlayerIn && !_IsActiveOrNot)
            {
                _IsActiveOrNot = true;
                _AudioSource.clip = _AudioClipList[ Random.Range(0, _AudioClipList.Count)];
                _AudioSource.Play();
                ActiveGargoyleVFX(Index, _IsActiveOrNot);

            }
            if (!_IsPlayerIn && _IsActiveOrNot)
            {
                _IsActiveOrNot = false;
                
                _AudioSource.Stop();
                ActiveGargoyleVFX(Index, _IsActiveOrNot);
            }

        }


    }
}

