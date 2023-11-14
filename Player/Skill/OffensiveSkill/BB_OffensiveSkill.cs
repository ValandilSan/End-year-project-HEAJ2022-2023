
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BagareBrian
{

    public class BB_OffensiveSkill : BB_Skills
    {
        [SerializeField] protected float _DamageDone;
       


        public virtual float DamageDone
        {
            get
            {
              
                return _DamageDone;
            }
        }

   


    }

}
