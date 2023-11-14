using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagareBrian
{
    [ExecuteInEditMode]
    public class BB_PPGlobalParameters : MonoBehaviour
    {
        //Crédits Gillard
        [SerializeField] private Transform _PlayerTransform;
        void Update()
        {
            if (_PlayerTransform)
            {
                Shader.SetGlobalVector("_PlayerPosition", _PlayerTransform.position);
            }
        }
    }
}

