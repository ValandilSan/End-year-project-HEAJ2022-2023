using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagareBrian
{
    public class BB_EnigmaLever : BB_LeverObserver
    {
        private void Awake()
        {
            _IsLeverForEnigma = true;
            _LeverMaterial = this.gameObject.GetComponentInChildren<MeshRenderer>().material;
           

        }





        public override void PulledLeverForWhat(float Index, bool LeverEnigma)
        {
            base.PulledLeverForWhat(Index, LeverEnigma);
        }
    }
}

