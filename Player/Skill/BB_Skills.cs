using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BagareBrian
{
    public class BB_Skills : ScriptableObject
    {
       
       [SerializeField] protected float _CoolDown;
        [SerializeField] protected float _ManaCost;
        [SerializeField] protected AnimationClip _Animationclip;
        [SerializeField] protected string _NameOfTheSkill;
       
      
        public virtual float coolDown => _CoolDown;
        public virtual float manacost => _ManaCost;
        public virtual float TimeForanimation => _Animationclip.length;
        public virtual string NameOfTheSkill => _NameOfTheSkill;

        public virtual void SkillEffect(Transform Player, Transform begin, Glo_Entities PlayerEntities)
        {

        }


      
    }
}

