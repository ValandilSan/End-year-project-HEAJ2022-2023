using BagareBrian;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagareBrian
{
    public class BB_EnnemyRanged : MonoBehaviour
    {
        [SerializeField] private BB_Ennemy _MainScript;

        [Header("Ground¨Paramaters")]
        [SerializeField] private float _XSecondsToDamage;
        [SerializeField] private float _YOffsetForTheAimPLane;
        [SerializeField] private float _GroundDamage;

        private BB_EnnemyRanged _MainRangedScript;
        private Glo_Entities _Entities;
        [Header("Curve")]
        [SerializeField] private Glo_BezierCurveGenerator _Curve;
        [SerializeField] private Glo_ExtendedBezierCurve _ExtendedCurve;
        [SerializeField] private Transform _TangentPosition;

        [SerializeField] private float _PosYOfTheTangent;
        [SerializeField] private GameObject _Endposition;
        private GameObject _Prefab;
        private Material _MaterialAimPlane;
        private Vector3 _NewPositionForTheTangent;

        [Header("Animation")]
        [SerializeField] private AnimationClip _TorchTrow;

        private bool _IsThrowing;

        [Header("Position")]
        [SerializeField] private Transform _StartPosition;
        private Transform _PositionToHave;
        [SerializeField] private float _SpeedOfTheTorch;
        [SerializeField] private float _YoffsetForTheFireGround;

        [Header("Prefab")]

        [SerializeField] private GameObject _TorchTrowPrefab;
        [SerializeField] private GameObject _Aimplane;
        [SerializeField] private GameObject _FireGround;
        private GameObject _AimplanePrefab;

        [Header("VFX")]
        [SerializeField] private float _SpeedOfTheVFXOutCirlce;

        private Coroutine _Damageperseconds;

        private void Start()
        {
          
            StayAtTheMiddle();
            _MainRangedScript = this.GetComponent<BB_EnnemyRanged>();
         
            _Entities = this.GetComponent<BB_Ennemy>().GetComponent<Glo_Entities>();
        }
        public void GiveMeInformation(Transform pLayer)
        {
            _PositionToHave = pLayer;
            if (_Prefab != null)
            {
                Destroy(_Prefab);
                _Prefab = null;
                _Prefab = Instantiate(_Endposition, _PositionToHave.position, Quaternion.identity);

            }
            else
            {
                _Prefab = Instantiate(_Endposition, _PositionToHave.position, Quaternion.identity);
            }


            _Curve.ChangeEndTarget(_PositionToHave.position, _Prefab.transform);
           

        }
        private void StayAtTheMiddle()
        {
            if (_PositionToHave != null)
            {
                _NewPositionForTheTangent = new Vector3((_PositionToHave.position.x + _StartPosition.position.x) / 2, _StartPosition.position.y + _PosYOfTheTangent, (_PositionToHave.position.z + _StartPosition.position.z) / 2);
                _TangentPosition.position = _NewPositionForTheTangent;
            }

        }
        public void DoDamage(Collider other, bool lasthit)
        {
            if (_Damageperseconds == null && !lasthit)
            {
                _Damageperseconds = StartCoroutine(DamagePerSeconds(other));
            }
            else
            {
                return;
            }


        }


        IEnumerator DamagePerSeconds(Collider other)
        {
            yield return new WaitForSecondsRealtime(_XSecondsToDamage);
            if (other != null)
            {
                Glo_ITakeDamage playerDamage = other.GetComponentInParent<Glo_ITakeDamage>();
                if (playerDamage != null)
                {
                    playerDamage.GetShot(_GroundDamage, _Entities);
                    Debug.Log(playerDamage);
                }
            }
           
            _Damageperseconds = null;
        }

        #region EventAnimation

        private void StartThrow()
        {

            _AimplanePrefab = Instantiate(_Aimplane, new Vector3(_PositionToHave.position.x, _PositionToHave.position.y + _YOffsetForTheAimPLane, _PositionToHave.position.z), Quaternion.identity);
            _MaterialAimPlane = _AimplanePrefab.GetComponent<MeshRenderer>().material;
            _MaterialAimPlane.SetFloat("_SizeCircleOut", 0);
            _MaterialAimPlane.SetFloat("_SizeInnerCircle", 0);
          
            _AimplanePrefab.SetActive(true);

            _IsThrowing = true;
        }
        private void LauncProjectil()
        {
            _IsThrowing = false;

            GameObject prefab = Instantiate(_TorchTrowPrefab, _StartPosition.position, Quaternion.identity);
            BB_EnnemyProjectil scriptPrefab = prefab.GetComponent<BB_EnnemyProjectil>();
            scriptPrefab.NeedInformation(_MaterialAimPlane, _Curve, _ExtendedCurve, _PositionToHave, _SpeedOfTheTorch, _FireGround, _AimplanePrefab, _YoffsetForTheFireGround, _MainRangedScript);


        }


        private void Update()
        {

            StayAtTheMiddle();



            if (_IsThrowing)
            {

                float getTheFloat = _MaterialAimPlane.GetFloat("_SizeCircleOut");
                getTheFloat = Mathf.Clamp(getTheFloat += Time.deltaTime * _SpeedOfTheVFXOutCirlce, 0, 1);
                _MaterialAimPlane.SetFloat("_SizeCircleOut", getTheFloat);
            }
        }
        #endregion
    }
}

