using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagareBrian
{
    public class BB_DitheringWall : MonoBehaviour
    {
        [SerializeField] private GameObject _WallObject;
        [SerializeField] private float _SpeedDithering;
        private Material _WallMaterial;



        [SerializeField] private bool _IsNeedADithering;
        void Start()
        {
            _IsNeedADithering = false;

            _WallMaterial = _WallObject.GetComponent<MeshRenderer>().material;

        }
        public void ActiveDithering()
        {

            _IsNeedADithering = true;

        }
        public void DisableDithering()
        {
            _IsNeedADithering = false;
        }
        private void NeedADithering()
        {
            if (_IsNeedADithering)
            {
                float currentDithering = _WallMaterial.GetFloat("_Opacity");
                currentDithering = Mathf.Clamp(currentDithering -= Time.deltaTime * _SpeedDithering, 0, 1);
                _WallMaterial.SetFloat("_Opacity", currentDithering);
            }
            else
            {
                float currentDithering = _WallMaterial.GetFloat("_Opacity");
                if (currentDithering == 1)
                {
                    return;
                }
                currentDithering = Mathf.Clamp(currentDithering += Time.deltaTime * _SpeedDithering, 0, 1);
                _WallMaterial.SetFloat("_Opacity", currentDithering);

            }
        }

        // Update is called once per frame
        void Update()
        {
            NeedADithering();
        }
    }
}

