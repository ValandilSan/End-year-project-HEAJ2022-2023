using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagareBrian
{
    public class BB_HealRay : MonoBehaviour
    {
        [Header("Sounds")]
        [SerializeField] private List<AudioClip> _ListAudio;
        [SerializeField] private AudioSource _AudioSource;


        [Header("HealRay")]

        [SerializeField] private Material _HealRayMaterial;
        [SerializeField] private float _MinValuePropertiesRay, _MaxValuePropertiesRay;
        [SerializeField] private float _ApparitionSpeed;
        private float _CurrentValuePropertiesRay;

        [SerializeField] private bool _IsActiveTheRay;

        [Header("HealCurve")]
        [SerializeField] private Material _HealCurveMaterial;
        [SerializeField] private float _MinValuePropertiesCurve, _MaxValuePropertiesCurve;

        private float _CurrentValuePropertiesCurve;
        [SerializeField] private bool _IsActiveTheCurve;

        private void Start()
        {
            _HealRayMaterial.SetFloat("_Fill", _MinValuePropertiesRay);
            _HealCurveMaterial.SetFloat("_Fill", _MaxValuePropertiesCurve);
            _CurrentValuePropertiesRay = _HealRayMaterial.GetFloat("_Fill");
            _CurrentValuePropertiesCurve = _HealCurveMaterial.GetFloat("_Fill");
            _IsActiveTheRay = false;
            _IsActiveTheCurve = false;
        }

        public void LauncTheHealRay()
        {
            _IsActiveTheRay = true;
        }
        public void DisableTheHealRay()
        {
            _IsActiveTheRay = false;

        }
        public void LaunchTheHealCurve(bool IsActive)
        {
            _IsActiveTheCurve = IsActive;
        }
        private void UpDownAPropertiesValueInTime(float speed, float minimum, float maximum, Material material)
        {
            float currentValue = material.GetFloat("_Fill");
            currentValue = Mathf.Clamp(currentValue += Time.deltaTime * speed, minimum, maximum);
            material.SetFloat("_Fill", currentValue);

        }
        private void UpdateTheRay()
        {
            if (_IsActiveTheRay)
            {
                UpDownAPropertiesValueInTime(_ApparitionSpeed, _MinValuePropertiesRay, _MaxValuePropertiesRay, _HealRayMaterial);
                if (_CurrentValuePropertiesRay != _MaxValuePropertiesRay)
                {
                    _CurrentValuePropertiesRay = _HealRayMaterial.GetFloat("_Fill");
                }

       

            }

            if (!_IsActiveTheRay && _CurrentValuePropertiesRay > _MinValuePropertiesRay)
            {
                UpDownAPropertiesValueInTime(-_ApparitionSpeed, _MinValuePropertiesRay, _MaxValuePropertiesRay, _HealRayMaterial);
                if (_CurrentValuePropertiesRay != _MinValuePropertiesRay)
                {
                    _CurrentValuePropertiesRay = _HealRayMaterial.GetFloat("_Fill");
                }
             
            }
        }


        private void UpdateTheCurve()
        {
            if (_IsActiveTheCurve && _CurrentValuePropertiesRay == _MaxValuePropertiesRay)
            {
                if (_CurrentValuePropertiesCurve == _MaxValuePropertiesCurve)
                {
                    _AudioSource.clip = _ListAudio[Random.Range(0, _ListAudio.Count)];
                    _AudioSource.Play();
                }
                UpDownAPropertiesValueInTime(-_ApparitionSpeed, _MinValuePropertiesCurve, _MaxValuePropertiesCurve, _HealCurveMaterial);
                if (_CurrentValuePropertiesCurve != _MinValuePropertiesCurve)
                {
                    _CurrentValuePropertiesCurve = _HealCurveMaterial.GetFloat("_Fill");
                    if (_CurrentValuePropertiesCurve == _MinValuePropertiesCurve)
                    {
                        _CurrentValuePropertiesCurve = _MaxValuePropertiesCurve;
                        _HealCurveMaterial.SetFloat("_Fill", _CurrentValuePropertiesCurve);
                       
                    }
                }



            }

            if (!_IsActiveTheCurve && _CurrentValuePropertiesCurve != _MaxValuePropertiesCurve)
            {
                UpDownAPropertiesValueInTime(-_ApparitionSpeed, _MinValuePropertiesCurve, _MaxValuePropertiesCurve, _HealCurveMaterial);
                if (_CurrentValuePropertiesCurve != _MinValuePropertiesCurve)
                {
                    _CurrentValuePropertiesCurve = _HealCurveMaterial.GetFloat("_Fill");
                    if (_CurrentValuePropertiesCurve == _MinValuePropertiesCurve)
                    {
                        _CurrentValuePropertiesCurve = _MaxValuePropertiesCurve;
                        _HealCurveMaterial.SetFloat("_Fill", _CurrentValuePropertiesCurve);
                    }
                }

            }
        }
        private void Update()
        {
            UpdateTheRay();
            UpdateTheCurve();
        }
    }
}

