using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace BagareBrian
{
    [ExecuteInEditMode, ImageEffectAllowedInSceneView]
    public class BB_PPSobelTargetAndVignette : MonoBehaviour
    {
        [Header("Maetrial")]
        [SerializeField] private Material _PostProcessMaterial;
        [SerializeField] private float _Blend;
        [SerializeField] private float _SpeedSobel;
        [SerializeField] private float _SpeedVignetteOpacity;
        [SerializeField] private bool _IsActiveTheSobel;
       [SerializeField] private bool _IsActiveVignette;

        [Header("ShakerCamera")]
        [SerializeField] private Camera _Camera;
        [SerializeField] private float _ShakeAmount;
        [SerializeField] private float _ShakeDuring;

        [SerializeField] private float _DecreaseTime;

        private float _CurrentShakeTime;
        private bool _IsShaking;
        private Vector3 _OriginalPos;
        private Vector3 _CurrentPosition;
        private bool _IsHavingPos;
       
        protected virtual void Start()
        {

            _PostProcessMaterial.SetFloat("_Activation", 0);
           
            
        }


        private void Shaking()
        {
            if (_IsShaking)
            {

                if (_CurrentShakeTime > 0)
                {
                    _Camera.transform.localPosition = _OriginalPos + Random.insideUnitSphere * _ShakeAmount;
                    _CurrentShakeTime -= Time.deltaTime * _DecreaseTime;
                }
                else
                {
                    _CurrentShakeTime = 0;
                    _Camera.transform.localPosition = _OriginalPos;

                }


            }

        }
        private void DoIHavePosition()
        {
            if (_IsHavingPos)
            {
                return;
            }
            else
            {

                _OriginalPos = _Camera.transform.localPosition;
                _IsHavingPos = true;
            }
        }
        #region Sobel
        public void LaunchTheSobel()
        {
            _IsActiveTheSobel = true;

        }
        public void DisableSobel()
        {
            _IsActiveTheSobel = false;
        }

        public void LaunchVignette()
        {
            _IsActiveVignette = true;
        }
        public void DisableVignette()
        {
            _IsActiveVignette = false;
        }
        private void UpdateTheSobel()
        {

            if (_IsActiveTheSobel)
            {
                DoIHavePosition();
                float currentvalueSobel = _PostProcessMaterial.GetFloat("_Activation");
                currentvalueSobel = Mathf.Clamp(currentvalueSobel += Time.deltaTime * _SpeedSobel, 0, 1);
                _PostProcessMaterial.SetFloat("_Activation", currentvalueSobel);



            
                _IsShaking = true;

            }
            if (!_IsActiveTheSobel)
            {
                float currentvalueSobel = _PostProcessMaterial.GetFloat("_Activation");
                currentvalueSobel = Mathf.Clamp(currentvalueSobel -= Time.deltaTime * _SpeedSobel, 0, 1);
                _PostProcessMaterial.SetFloat("_Activation", currentvalueSobel);

              

                _IsShaking = false;
                _CurrentShakeTime = _ShakeDuring;
                _IsHavingPos = false;

            }
        }
        #endregion

        private void UpdateVignette()
        {
            if (_IsActiveVignette)
            {

                float currentValueVignette = _PostProcessMaterial.GetFloat("_OpacityVignette");
                currentValueVignette = Mathf.Clamp(currentValueVignette += Time.deltaTime * _SpeedVignetteOpacity, 0, 1);
                _PostProcessMaterial.SetFloat("_OpacityVignette", currentValueVignette);

            }
            if (!_IsActiveVignette)
            {
                float currentValueVignette = _PostProcessMaterial.GetFloat("_OpacityVignette");
                currentValueVignette = Mathf.Clamp(currentValueVignette -= Time.deltaTime * _SpeedVignetteOpacity, 0, 1);
                _PostProcessMaterial.SetFloat("_OpacityVignette", currentValueVignette);

            }
        }
        public void UpdateVignetteByDamage()
        {
            if (!_IsActiveVignette)
            {

                _PostProcessMaterial.SetFloat("_OpacityVignette", 1);
            }
        }
        protected virtual void Update()
        {
            UpdateTheSobel();
            UpdateVignette();
            Shaking();
            if (_PostProcessMaterial && _Blend != 0)
            {
                _PostProcessMaterial.SetFloat("_Blend", _Blend);
            }

        }
        // Credits Gillard
        private void OnRenderImage(RenderTexture src, RenderTexture dst)
        {
            if (_PostProcessMaterial && _Blend != 0)
            {
                Graphics.Blit(src, dst, _PostProcessMaterial);
            }
            else
            {
                Graphics.Blit(src, dst);

                if (!_PostProcessMaterial)
                {
                   
                    return;
                }
            }
        }

    }
}

