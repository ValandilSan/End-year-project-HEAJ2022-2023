using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagareBrian
{
    public class BB_RangedLoading : MonoBehaviour
    {

        [Header("Orbs")]
        [SerializeField] private GameObject _LoadingOrbs;
        [SerializeField] private float _ScaleToHave;
        [SerializeField] private float _GrowSpeed;

        [SerializeField] private bool _IsGrowOrbs = false;
        private float _CurrentValueOfTheScale;

        [Header("Curves")]
        [SerializeField] private GameObject _RangedCurve;
        [SerializeField] private Material _CurveMateriel;
        [SerializeField] private float _SpeedCurve;

        private float _StartvalueProperties;
        [SerializeField] private float _EndValuePropertie;
        private float _CurrentValueOfTheProperties;



        private Glo_Entities _Entities;
        private float _damage;

        private void Start()
        {

            _CurrentValueOfTheScale = 0;

            _LoadingOrbs.transform.localScale -= _LoadingOrbs.transform.localScale;

            _StartvalueProperties = _CurveMateriel.GetFloat("_Fill");
            _CurrentValueOfTheProperties = _StartvalueProperties;


        }
        
        public void GrowUpOrbs()
        {
            _IsGrowOrbs = !_IsGrowOrbs;


        }

        public void LaunchObs(float damage, Glo_Entities entities)
        {
           
            _damage = damage;   
            _Entities = entities;   
            _IsGrowOrbs = !_IsGrowOrbs;
            UpdateTheScale(0);
            _CurveMateriel.SetFloat("_Fill", _StartvalueProperties);
            _CurrentValueOfTheProperties = _StartvalueProperties;
        }
        private void UpdateTheScale(float scale)
        {
            _LoadingOrbs.transform.localScale = new Vector3(scale, scale, scale);
        }
        private void Update()
        {

            if (_IsGrowOrbs)
            {
                float size = _LoadingOrbs.transform.localScale.y;
                size = Mathf.Clamp(size += Time.deltaTime * _GrowSpeed, 0, _ScaleToHave);
                UpdateTheScale(size);

                _CurrentValueOfTheProperties = Mathf.Clamp(_CurrentValueOfTheProperties += Time.deltaTime * _SpeedCurve, _StartvalueProperties, _EndValuePropertie);
                _CurveMateriel.SetFloat("_Fill", _CurrentValueOfTheProperties);

            }
            _RangedCurve.GetComponent<LineRenderer>().material.SetFloat("_Fill", 1);



        }
    }
}

