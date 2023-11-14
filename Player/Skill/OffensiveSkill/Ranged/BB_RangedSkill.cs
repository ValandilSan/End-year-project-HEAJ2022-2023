using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


namespace BagareBrian
{
    [CreateAssetMenu(menuName = "ScriptableObject/BB_ScriptableObject/Skills/RangedSkill")]
   

    public class BB_RangedSkill : BB_OffensiveSkill
    {
        [SerializeField] private GameObject _projectile;
     

        public override void SkillEffect(Transform player, Transform start, Glo_Entities PlayerEntities)
        {
            
          GameObject Prefab = Instantiate(_projectile, start.position, player.rotation);
            BB_ProjectilPrefab  PrefabScript = Prefab.GetComponent<BB_ProjectilPrefab>();
            PrefabScript.giveInformation(_DamageDone, PlayerEntities);
          
        }








    }
}

