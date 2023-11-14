using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagareBrian
{

    public class BB_DefensiveSkill : BB_Skills
    {
        [SerializeField] protected float _TimeToBeInvisibleForEnnemy;
        public virtual float InvicibleFrame => _TimeToBeInvisibleForEnnemy;

      

    }

}
