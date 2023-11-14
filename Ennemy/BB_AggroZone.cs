using BagareBrian;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagareBrian
{
    public class BB_AggroZone : MonoBehaviour
    {
        private bool _IsTargetHere;
        private bool _Soemonedisapper = true;
        private bool _ReduceAggroZone;
        private Transform _Intruder;
        private BB_Ennemy _Ennemy;
        private SphereCollider _Colldier;
        [SerializeField] private float _UpgradetheAggrozone;
        [SerializeField] private GameObject _LastPoint;
        private float _BaseValueOfTheAggroZone;
        Vector3 _rayDirection;
        private bool _IsDead;


        private GameObject _prefab;
        private void Start()
        {
            _IsDead = false;    
            _Ennemy = GetComponentInParent<BB_Ennemy>();
            _Colldier = GetComponent<SphereCollider>();
            _BaseValueOfTheAggroZone = _Colldier.radius;
            _ReduceAggroZone = false;
        }

        private void ChangeRadiusAggro(float posOrNeg)
        {
            if (_ReduceAggroZone)
            {
                if (_Colldier.radius != _BaseValueOfTheAggroZone)
                {
                    _Colldier.radius = Mathf.Clamp(_Colldier.radius += Time.deltaTime * posOrNeg, _BaseValueOfTheAggroZone, _BaseValueOfTheAggroZone * _UpgradetheAggrozone);
                }

            }

        }
        public void UpgradeRadisuAggro()
        {
            if (_Colldier.radius < _BaseValueOfTheAggroZone * _UpgradetheAggrozone)
            {
                _Colldier.radius = _BaseValueOfTheAggroZone * _UpgradetheAggrozone;
            }
        }
        #region Detection

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {

                RaycastHit hit;
                Vector3 direction = other.transform.position - transform.position;
                float distance = Vector3.Distance(transform.position, other.transform.position);

                direction = new Vector3(direction.x, transform.position.y, direction.z);
                if (Physics.Raycast(transform.position, direction, out hit, distance))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        _ReduceAggroZone = false;

                        _Soemonedisapper = false;
                        Destroy(_prefab);
                        _prefab = null;
                        _Intruder = other.transform;
                        _IsTargetHere = true;
                        _Ennemy.IsTargetHere(_IsTargetHere, _Intruder, true);
                        UpgradeRadisuAggro();
                        
                    }
                    else
                    {
                        Debug.Log("Other");
                    }

                }


            }
        }
        private void OnTriggerStay(Collider other)
        {

            if (other.tag == "Player")
            {
                RaycastHit hit;
                Vector3 direction = other.transform.position - transform.position;
                float distance = Vector3.Distance(transform.position, other.transform.position);
                direction = new Vector3(direction.x, transform.position.y, direction.z);
                if (Physics.Raycast(transform.position, direction, out hit, distance))
                {

                    if (hit.collider.CompareTag("Player"))
                    {
                        _ReduceAggroZone = false;
                        if (_Colldier.radius < _BaseValueOfTheAggroZone * _UpgradetheAggrozone)
                        {
                            _Colldier.radius = _BaseValueOfTheAggroZone * _UpgradetheAggrozone;
                        }
                        Destroy(_prefab);
                        _prefab = null;
                        _Intruder = other.transform;
                        _IsTargetHere = true;
                        _Ennemy.IsTargetHere(_IsTargetHere, _Intruder, true);


                    }


                }

                Debug.DrawRay(transform.position, direction, Color.red);

            }

        }
        private void OnTriggerExit(Collider other)
        {

            if (other.tag == "Player")
            {
                if (_prefab != null)
                {
                    return;
                }
                Vector3 lastPoint = other.transform.position;
             InstianteNewChase(lastPoint,_IsTargetHere);
               

            }
        }
        public void InstianteNewChase(Vector3 lastPoint, bool isTargetHere)
        {
            if (_prefab == null)
            {
                _prefab = Instantiate(_LastPoint, lastPoint, Quaternion.identity);
            }
            else
            {
                Destroy(_prefab);
                _prefab = null;
                _prefab = Instantiate(_LastPoint, lastPoint, Quaternion.identity);
            }
            _Intruder = _prefab.transform;

            _Ennemy.IsTargetHere(isTargetHere, _prefab.transform, false);
            _ReduceAggroZone = true;
        }
        public void ImDead()
        {
            _IsDead = true;
        }
        public void DestroyTheFakeTarget()
        {
            Destroy(_prefab);
            _prefab = null;
        }
        private void Update()
        {
            if (_Intruder == null && !_Soemonedisapper)
            {
                /*  Soemonedisapper = true;

                  IsTargetHere = false;
                  Intruder = null;
                  m_Ennemy.IsTargetHere(IsTargetHere, Intruder);*/
            }
            if (_IsDead)
            {
                if (_Colldier != null)
                {
                    _Colldier.radius -= _Colldier.radius;
                    Destroy(_Colldier);
                }
                return;
            }
            ChangeRadiusAggro(-1);

        }

        #endregion
    }
}

