
using BagareBrian;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BagareBrian
{
    [ExecuteInEditMode]
    public class BB_EnnemyProjectil : MonoBehaviour
    {
        private Glo_BezierCurveGenerator _Curve;
        private Glo_ExtendedBezierCurve _ExtendedCurve;
        private Material _GroundMaterial;
        private Transform _GroundTransform;
        private float _RotationSpeed;
        private float _CurrentTime;
        private float _SpeedOfTheTorch;
        private float _YoffsetForTheGround;
        [SerializeField] private float _Waittime;
        [SerializeField] private AudioSource _AudioSource;
        [SerializeField] private List<AudioClip> _Audioclip;
        private GameObject _FireGround;
        private GameObject _AimGround;
        private bool _IsStop = false;
        private bool _Isstartanim = false;
        private BB_EnnemyRanged _MainScript;
        private void Start()
        {
          
            _Isstartanim = true;
           
            gameObject.transform.LookAt(_GroundTransform);
            _AudioSource.clip = _Audioclip[Random.Range(0, _Audioclip.Count)];
            _AudioSource.Play();
        }
     
        public void NeedInformation(Material planeMaterial, Glo_BezierCurveGenerator curve, Glo_ExtendedBezierCurve extendedCurve,
            Transform lookAt, float speedOfTheRotation, GameObject prefabFireGround, GameObject prefabAimGround, float yOffsetForTheFireGround, BB_EnnemyRanged mainScript)
        {
            _Curve = curve;
            _ExtendedCurve = extendedCurve;
            _GroundMaterial = planeMaterial;
            _GroundTransform = lookAt;
            _RotationSpeed = speedOfTheRotation;
            _FireGround = prefabFireGround;
            _AimGround = prefabAimGround;
            _YoffsetForTheGround = yOffsetForTheFireGround;
            _CurrentTime = 0;
            _MainScript = mainScript;
        }

        private void WhenITouchTheGround(float valueToSet)
        {
          
            _GroundMaterial.SetFloat("_SizeInnerCircle", valueToSet);
        }
        public IEnumerator Wait()
        {
            yield return new WaitForSecondsRealtime(_Waittime);
            _Isstartanim = true;
        }
        // Script of Delnaye to following a curve with a modification.
        public void canIGo()
        {
            if (_IsStop)
            {
                gameObject.transform.position = _ExtendedCurve.GetPosition(1);
               
                return;
            }

            if (_Curve == null) return;

           
            float currentAlpha = Mathf.Clamp(_CurrentTime += Time.deltaTime, 0, 1);
            WhenITouchTheGround(currentAlpha);


            if (_ExtendedCurve)
            {
                gameObject.transform.position = _ExtendedCurve.GetPosition(currentAlpha);

                gameObject.transform.Rotate(Vector3.right, _RotationSpeed);


            }
            else
            {
                gameObject.transform.position = _Curve.GetPosition(currentAlpha);
                gameObject.transform.Rotate(Vector3.right, _RotationSpeed);

            }
            if (currentAlpha >= 1)
            {
                
                GameObject prefabFire = Instantiate(_FireGround);
                BB_EnnemyFireGround scriptprefab = prefabFire.GetComponent<BB_EnnemyFireGround>();
                scriptprefab.GiveMeInformation(_MainScript);
                prefabFire.transform.position = new Vector3(_AimGround.transform.position.x, _AimGround.transform.position.y + _YoffsetForTheGround, _AimGround.transform.position.z);
                prefabFire.SetActive(true);
                Destroy(_AimGround, 1f);
                Destroy(this.gameObject, 1f);
                _Isstartanim = false;

            }
        }


        private void Update()
        {
            
            if (_Isstartanim)
            {
                canIGo();
            }


        }
    }

}

