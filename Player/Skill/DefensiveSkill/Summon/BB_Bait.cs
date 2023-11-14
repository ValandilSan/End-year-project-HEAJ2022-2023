using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

namespace BagareBrian
{
    public class BB_Bait : MonoBehaviour
    {
        [SerializeField] private float _SpeedOfTheGround;

        [SerializeField] private GameObject _Ground;
        private Material _MatGround;
        private float _GroundFill;

        [SerializeField] private GameObject _Bait;
        [SerializeField] private float _SpeedOfTheBait;

        [SerializeField] private Material _MatBait;
        private float _BaitFill;



        private bool _IsPuddleBloodAppear = false;
        private bool _IsBaitAppear = false;
        private bool _IsTimeToDisapper = false;

        private float _DurationOfTheBait;
        private float _SpeedToOutTheGround;
        private bool _NotReady = true;
        private Vector3 _PositionToReach;
        private float _OriginalValue;
        private float _MagnitudeOfTheTranslate;


        private void Start()
        {

            _MatGround = _Ground.GetComponent<MeshRenderer>().material;
            
            _BaitFill = 0;
            _GroundFill = 0;
           // _MatGround.SetFloat("_Fading", 1);
            _MatGround.SetFloat("_Fill", 0);
            _MatBait.SetFloat("_Fill", 0);
            
        }


        public void GiveInformation(Vector3 positionOfTheBait, Vector3 positionToReach, float speed, float pourcentageToReachToStopTheTranslate, float duration)
        {
            transform.position = positionOfTheBait;
            _PositionToReach = positionToReach;
            _SpeedToOutTheGround = speed;
            _OriginalValue = transform.position.y;
            _MagnitudeOfTheTranslate = pourcentageToReachToStopTheTranslate;
            _DurationOfTheBait = duration;
         
        }

        public void OutOfTheGround()
        {
            if (_NotReady)
            {
                float distanceBetweenInPourcent = Mathf.Abs(((_PositionToReach.y - transform.position.y) / (_OriginalValue)) * 100);
                if (distanceBetweenInPourcent <= _MagnitudeOfTheTranslate)
                {
                    transform.position = _PositionToReach;
                    _NotReady = false;
                    return;
                }
                transform.position = Vector3.Lerp(transform.position, _PositionToReach, _SpeedToOutTheGround * Time.deltaTime);

            }
        }


        public void ActivePhase()
        {
            if (!_IsPuddleBloodAppear)
            {
                _IsPuddleBloodAppear = !_IsPuddleBloodAppear;
                StartCoroutine(WaitToFill());
                return;
            }
            if (_IsPuddleBloodAppear && !_IsBaitAppear)
            {
                _IsPuddleBloodAppear = !_IsPuddleBloodAppear;
                _IsBaitAppear = !_IsBaitAppear;
                StartCoroutine(Disapper());
            }
        }


        //Temps entre les deux anims;
        IEnumerator WaitToFill()
        {

            yield return new WaitForSecondsRealtime(0.8f);
            ActivePhase();
        }
        IEnumerator Disapper()
        {
            yield return new WaitForSecondsRealtime(_DurationOfTheBait);
            _IsTimeToDisapper = !_IsTimeToDisapper;
            _IsBaitAppear = !_IsBaitAppear;
        }

        public void ApparitionPuddleAndBait()
        {
            if (_IsTimeToDisapper)
            {
                if (_GroundFill >= 1)
                {
                    float fading = _MatGround.GetFloat("_Fill");
                    fading = Mathf.Clamp(fading -= Time.deltaTime, 0, 1);
                    _MatGround.SetFloat("_Fill", fading);
                    if (fading <= 0)
                    {
                        Destroy(gameObject);
                    }
                    return;
                }
                _BaitFill = Mathf.Clamp(_BaitFill -= Time.deltaTime * _SpeedOfTheBait, 0, 1);
                _MatBait.SetFloat("_Fill", _BaitFill);

                _GroundFill = Mathf.Clamp(_GroundFill += Time.deltaTime * _SpeedOfTheBait, 0, 1);
                _MatGround.SetFloat("_Fill", _GroundFill);
                return;
            }
            if (_IsBaitAppear)
            {
                _BaitFill = Mathf.Clamp(_BaitFill += Time.deltaTime * _SpeedOfTheBait, 0, 1);
                _MatBait.SetFloat("_Fill", _BaitFill);

                _GroundFill = Mathf.Clamp(_GroundFill -= Time.deltaTime * _SpeedOfTheBait, 0, 1);
                _MatGround.SetFloat("_Fill", _GroundFill);
            }
            if (_GroundFill < 1 && _IsPuddleBloodAppear)
            {
                _GroundFill = Mathf.Clamp(_GroundFill += Time.deltaTime * _SpeedOfTheGround, 0, 1);
                _MatGround.SetFloat("_Fill", _GroundFill);
            }


        }



        private void Update()
        {
            ApparitionPuddleAndBait();

           
        }
    }

}
