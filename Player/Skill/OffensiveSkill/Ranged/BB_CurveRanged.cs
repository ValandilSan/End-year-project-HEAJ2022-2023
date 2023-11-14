using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagareBrian
{ /// <summary>
/// TEST
/// </summary>
    public class BB_CurveRanged : MonoBehaviour
    {
        private GameObject game;
        private LineRenderer lineRenderer;
        [SerializeField] private Material curveMaterial;

        private void Start()
        {



            this.curveMaterial.SetFloat("_Fill", 0);
        }
    }
}

