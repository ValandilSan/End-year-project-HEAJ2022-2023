using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BB_MeleeCollider : MonoBehaviour
{
    private float _Damage;
    private Glo_Entities _Entities;


  public void GiveMeInformation(float damage, Glo_Entities entities)
    {
        _Damage = damage;
        _Entities = entities;   
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            Debug.Log("TouhcElseThanEnnemy");
            Glo_ITakeDamage Ennemy = other.GetComponentInParent<Glo_ITakeDamage>();

            if (Ennemy != null)
            {

                Ennemy.GetShot(_Damage, _Entities);
            }
            // Destroy(gameObject);
            /* if (_EnemyColliderList != null)
             {
                 foreach (var item in _EnemyColliderList)
                 {
                     Debug.Log("Nombre de passage");
                     if (other == item)
                     {
                         return;
                     }

                 }
             }

             _EnemyColliderList.Add(other);*/

        }
    }
}
