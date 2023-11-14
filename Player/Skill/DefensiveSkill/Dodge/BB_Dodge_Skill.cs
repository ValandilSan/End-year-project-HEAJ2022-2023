using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BagareBrian
{
    [CreateAssetMenu(menuName = "ScriptableObject/BB_ScriptableObject/Skills/DodgeSkill")]
    public class BB_Dodge_Skill : BB_DefensiveSkill
    {
       
        [SerializeField] private float _DistanceOfDodge;
        [SerializeField] private ParticleSystem _Particle;
        [SerializeField] private float _TimeToDestoryParticles;
     

        public override void SkillEffect(Transform player, Transform start, Glo_Entities entities)
        {

            Debug.Log("here");
          
          ParticleSystem istance = Instantiate(_Particle,start.position,Quaternion.identity);
            GameObject instanceGameobject = istance.gameObject;
            Destroy(instanceGameobject,_TimeToDestoryParticles) ;
           
        }


    }
}

