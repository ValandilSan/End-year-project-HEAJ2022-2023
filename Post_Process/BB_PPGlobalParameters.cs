using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagareBrian
{
    [ExecuteInEditMode]
    public class BB_PPGlobalParameters : MonoBehaviour
    {
        //Cr�dits Gillard
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

