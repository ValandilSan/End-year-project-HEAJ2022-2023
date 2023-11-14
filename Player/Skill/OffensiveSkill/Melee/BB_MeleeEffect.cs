using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;


namespace BagareBrian
{
    public class BB_MeleeEffect : MonoBehaviour
    {
        [SerializeField] private float _ApparitionSpeedOfTheSword, _ApparitionSpeedOfTheSquichy,_ApparitionSpeedOfTheSlash;
       
        [SerializeField] private Material _Slash, _Sword, _SwordSquishi;
        [SerializeField] private BB_MeleeCollider _MeleeSwords;
        [SerializeField] private Collider _SwordCollider;
        private Collider _EnnemyCollider;
        private List<Collider> _EnemyColliderList;
        private Glo_Entities _Entities;
        private float _Damage;
        private float _SpeedAttack;

        [SerializeField] private bool _IsAppear ;
        private bool _IsSlashVFX;
        private void Start()
        {
            // _Sword = GetComponent<MeshRenderer>().material;
            _Sword.SetFloat("_Dissolve", 0);
            _SwordSquishi.SetFloat("_Dissolve", 0);
            _Slash.SetFloat("_Dissolve", 0);
            _SwordCollider.enabled = false;
            _IsAppear = false;
            _IsSlashVFX = false;
            _EnemyColliderList = new List<Collider>();
            StartCoroutine(GivePauseBeforeGiveInformation());
         
        }
        IEnumerator GivePauseBeforeGiveInformation()
        {
            yield return new WaitForSecondsRealtime(1f);
            _ApparitionSpeedOfTheSword *= _SpeedAttack;
            _ApparitionSpeedOfTheSquichy *= _SpeedAttack;
          _ApparitionSpeedOfTheSlash *= _SpeedAttack;
        }
        
        public void ActiveVFXSword()
        {
            _IsAppear = true;
        }
        public void DisableVFXSword()
        {
            _IsAppear=false;
        }
        public void LaunchSlash()
        {
            _IsSlashVFX = true; 
        }
        public void DisableSlash()
        {
            _IsSlashVFX = false;
        }
        public void ActiveDisableCollider()
        {
            _SwordCollider.enabled = !_SwordCollider.enabled;
            if (_SwordCollider.enabled == false)
            {
               // LaunchApparitionSword();
                _EnemyColliderList.Clear();
                Debug.Log("DisableCollider");
            }

        }
        public void NeedInformation(float damage, Glo_Entities entities, float speedAttack)
        {
            _Damage = damage;
            _Entities = entities;
            _MeleeSwords.GiveMeInformation(damage, entities);
            _SpeedAttack = speedAttack;
        }

       // private void OnTriggerEnter(Collider other)
       /* {
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

                _EnemyColliderList.Add(other);
              
            }
        }*/
        private void AppearDisaperSword()
        {
            if (_IsAppear)
            {
                float dissolveVFXValue = _Sword.GetFloat("_Dissolve");
                dissolveVFXValue = Mathf.Clamp(dissolveVFXValue += Time.deltaTime * _ApparitionSpeedOfTheSword*_SpeedAttack, 0, 1);
                _Sword.SetFloat("_Dissolve", dissolveVFXValue);

                float dissolveSquishiValue = _SwordSquishi.GetFloat("_Dissolve");
                dissolveSquishiValue = Mathf.Clamp(dissolveSquishiValue += Time.deltaTime * _ApparitionSpeedOfTheSquichy * _SpeedAttack, 0, 1);
               _SwordSquishi.SetFloat("_Dissolve", dissolveSquishiValue);
            }

            if (!_IsAppear)
            {
                float dissolveVFXValue = _Sword.GetFloat("_Dissolve");
                dissolveVFXValue = Mathf.Clamp(dissolveVFXValue -= Time.deltaTime * _ApparitionSpeedOfTheSword * _SpeedAttack, 0, 1);
                _Sword.SetFloat("_Dissolve", dissolveVFXValue);

                float dissolveSquishiValue = _SwordSquishi.GetFloat("_Dissolve");
                dissolveSquishiValue = Mathf.Clamp(dissolveSquishiValue -= Time.deltaTime * _ApparitionSpeedOfTheSquichy * _SpeedAttack, 0, 1);
                _SwordSquishi.SetFloat("_Dissolve", dissolveSquishiValue);
            }
        }


        private void ActiveSlash()
        {
            if (_IsSlashVFX)
            {
                float dissolveValue = _Slash.GetFloat("_Dissolve");
                dissolveValue = Mathf.Clamp(dissolveValue += Time.deltaTime * _ApparitionSpeedOfTheSword * _SpeedAttack, 0, 1);
                _Slash.SetFloat("_Dissolve", dissolveValue);
            }

            if (!_IsSlashVFX)
            {
                float dissolveValue = _Slash.GetFloat("_Dissolve");
                dissolveValue = Mathf.Clamp(dissolveValue -= Time.deltaTime * _ApparitionSpeedOfTheSword * _SpeedAttack, 0, 1);
                _Slash.SetFloat("_Dissolve", dissolveValue);
            }
        }
        private void Update()
        {
            AppearDisaperSword();
            ActiveSlash();
        }
    }
}

