using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace BagareBrian
{
    public class BB_DoorBreak : Glo_Enemies
    {
        [SerializeField] private GameObject _DoorToBreak;
        [SerializeField] private float _Durability;

       // [SerializeField] private AudioSource _AudioSource;
       



        public override void GetShot(float damage, Glo_Entities striker)
        {
            if (striker is Glo_Entities || striker is Glo_Traps)
            {
                if (damage > _Durability)
                {
                     _DoorToBreak.SetActive(true);
                
                    Animator animatorPrefabDoor = _DoorToBreak.GetComponent<Animator>();
                   
                    animatorPrefabDoor.SetTrigger("Explosion");
                   Destroy(this.gameObject);
                }


            }
        }
    }

}

