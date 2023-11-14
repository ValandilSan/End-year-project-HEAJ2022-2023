using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BagareBrian
{
    public class BB_GateLever : BB_LeverObserver
    {

        private void Awake()
        {
            _LeverMaterial = this.gameObject.GetComponentInChildren<MeshRenderer>().material;
            _IsLeverForEnigma = false;
            
            _IsActivable = true;
        }



        public override void PulledLeverForWhat(float Index, bool LeverEnigma)
        {
            
            base.PulledLeverForWhat(Index, LeverEnigma);
        }
    }
}

