using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagareBrian
{
    [CreateAssetMenu(menuName = "ScriptableObject/BB_ScriptableObject/Skills/HealSkill")]
    public class BB_HealSkill : BB_OffensiveSkill
    {
       
        [SerializeField] protected private float _LifeStealHeal;
        [SerializeField] protected  private float _MaxDistanceForTheRay;
        [SerializeField] protected  private float _RadiusToKnowIfITouch;


        public float MaxDistanceForTheRay => _MaxDistanceForTheRay;
        public float RadiusToKnowIfITouch => _RadiusToKnowIfITouch; 
        public float LifeStealheal => _LifeStealHeal;

  
    }
}

