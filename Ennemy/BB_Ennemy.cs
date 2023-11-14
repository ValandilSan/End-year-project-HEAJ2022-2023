using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace BagareBrian
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class BB_Ennemy : Glo_Enemies
    {

        [SerializeField] private AudioSource _AudioSource;
       
        [SerializeField] private List<AudioClip> _AudioMeleeClip;
        [SerializeField] private List<AudioClip> _AudioRanged;
        [SerializeField] private List<AudioClip> _AudioDeath;
        [SerializeField] private GameObject _Paysan;
        [Header("EnnemyStats")]

        [SerializeField] private float _ModifySpeed;
        [SerializeField] private float _SpeedRotation;
        [SerializeField] private float _ModifyAcceleration;
        [SerializeField] private float _ModifyStopDistance;
        [SerializeField] private float _MaxHeal;
        [SerializeField] private float _NumberOfRanged;
        [SerializeField] private float _AngleToHaveBeforeAttacking;
        private float _CurrentAngle;
        private float _BasicSpeed;
        private bool _IsDead;


        [Header("Ennemy anims")]
        [SerializeField] private AnimationClip _MeleeAttack;
        [SerializeField] private AnimationClip _LookAroundAnimation;
        [SerializeField] private AnimationClip _IdleStandTorch;
        [SerializeField] private AnimationClip _ThrowTorch;

        private Animator _EnnemyAnimator;

        [Header("EnnemyParameters")]
        [SerializeField] private GameObject _Torch;
        [SerializeField] private float _TimeBetweenToSeeToThrowTorch;
        [SerializeField] private float _DistanceToThrow;

        private NavMeshAgent _Meshagent;
        private EEnnemyState _EnemyState;
        private bool _IsInClosetoMe;
        private bool _IsItTheplayer = false;
        private Transform _Target;

        [Header("AggroZone")]
        [SerializeField] private GameObject _AggroZone;
        private BB_AggroZone _AggroScript;

        [Header("EnnemySkills")]
        [SerializeField] private BB_EnnemyRanged _RangedScript;
        [SerializeField] private GameObject _MeleeGround, _MeleeVFX;
        private float _CurrentHeal;

        private bool _IsHavingatorch;
        private Coroutine _Attackcouroutine;
        private Coroutine _IdleSearchCouroutine;
        private Coroutine _IdlePatrol;
        private Coroutine _ThrowAttack;
        private Coroutine _DoIThrowOrNot;

        [Header("DifferentPath")]
        [SerializeField] private List<Transform> _Position;

        private int _PatrolPoint = 0;
        private int _MaxPatrolPoint;
     
        private void Start()
        {
            _IsDead = false;
            _IsInClosetoMe = false;

            _CurrentHeal = _MaxHeal;

            if (_Position.Count > 0)
            {
                _EnemyState = EEnnemyState.Patrol;
            }
            else
            {
                _EnemyState = EEnnemyState.Idle;
            }


            _Meshagent = this.GetComponent<NavMeshAgent>();
            _Meshagent.stoppingDistance = _ModifyStopDistance;
            _EnnemyAnimator = this.GetComponent<Animator>();
            _MaxPatrolPoint = _Position.Count;
            _AggroScript = _AggroZone.GetComponent<BB_AggroZone>();
            _BasicSpeed = _Meshagent.speed;


            
        }

        #region Detection
        public void IsTargetHere(bool isTargethere, Transform intruders, bool isItThePlayer)
        {
            _IsInClosetoMe = isTargethere;
            _Target = intruders;
            _IsItTheplayer = isItThePlayer;
        }


        #endregion

        #region LaunchDifferentAnimation
        public void LaunchIdleTrigger()
        {

        }


        #endregion

        #region Mouvement

        private void GoOnTheNextPoint()
        {

            if (_Meshagent.remainingDistance <= _Meshagent.stoppingDistance)
            {

                if (_Position.Count == 1)
                {
                    _EnnemyAnimator.SetBool("LookMode", true);
                    return;
                }
                _PatrolPoint++;

                if (_PatrolPoint >= _MaxPatrolPoint)
                {
                    _PatrolPoint = 0;
                }
              
            }
            if (_Position.Count > 1)
            {
                _Meshagent.SetDestination(_Position[_PatrolPoint].position);
            }

            if (_Position.Count == 1)
            {
                _Meshagent.SetDestination(_Position[_PatrolPoint].position);
            }

            //   m_meshagent.destination = m_position[0].position;
        }

        private void RegularPath()
        {
            if (_IsInClosetoMe == true)
            {
                SwitchState(EEnnemyState.Attack);
            }
            if (_Position != null && _IsInClosetoMe == false)
            {
                GoOnTheNextPoint();
            }

        }


        private void GoOnTheTarget()
        {
            if (_Target && _IsInClosetoMe)
            {
                _Meshagent.SetDestination(_Target.position);
             
            }
            if (!_IsInClosetoMe)
            {
                SwitchState(EEnnemyState.Patrol);
            }


        }


        #endregion

        #region Coroutines

        IEnumerator StopForMeleeAttack()
        {
            float currentSpeed = _Meshagent.speed;
            _Meshagent.speed = 0;
            _EnnemyAnimator.SetTrigger("AttackFork");
            _AudioSource.clip = _AudioMeleeClip[Random.Range(0, _AudioMeleeClip.Count)];
            _AudioSource.Play();
            yield return new WaitForSecondsRealtime(_MeleeAttack.length / _EnnemyAnimator.GetFloat("SpeedMelee"));
            _Meshagent.speed = currentSpeed;
            _Attackcouroutine = null;
         //   _Meshagent.speed = Mathf.Clamp(_Meshagent.speed, _BasicSpeed / 2, _BasicSpeed * _ModifySpeed);

        }
        IEnumerator DoILaunchatorch()
        {
           
            yield return new WaitForSecondsRealtime(_TimeBetweenToSeeToThrowTorch);
            float randomFloat = Random.Range(0, 2);
            if (randomFloat == 0)
            {
                _DoIThrowOrNot = null;
                ///nothing;
            }
            else
            {
                _RangedScript.GiveMeInformation(_Target);
                _ThrowAttack = StartCoroutine(StopForLaunchingATorch());
            }
            
        }
        IEnumerator StopForLaunchingATorch()
        {
            float currentSpeed = _Meshagent.speed;
            _Meshagent.speed = 0;
            _AudioSource.clip = _AudioRanged[Random.Range(0, _AudioRanged.Count)];
            _EnnemyAnimator.SetTrigger("ThrowTorch");
            _AudioSource.Play();
            yield return new WaitForSecondsRealtime(_ThrowTorch.length);
            _NumberOfRanged--;
            _Meshagent.speed = currentSpeed;
            _DoIThrowOrNot = null;
            _ThrowAttack = null;
           // _Meshagent.speed = Mathf.Clamp(_Meshagent.speed, _BasicSpeed / 2, _BasicSpeed * _ModifySpeed);
        }
        IEnumerator LaunchIdleSearch()
        {
            float currentSpeed = _Meshagent.speed;
            _Meshagent.speed = 0;
            _EnnemyAnimator.SetBool("LookAround", true);
            yield return new WaitForSecondsRealtime(_LookAroundAnimation.length);
            _EnnemyAnimator.SetBool("LookAround", false);

            _Meshagent.speed = currentSpeed;
            SwitchState(EEnnemyState.Patrol);

        }

        #endregion

        private void RotationOfTheennemy()
        {
            if (_Target != null)
            {
                Vector3 targetDirection = _Target.position - transform.position;
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                _CurrentAngle = Vector3.Angle(targetDirection, transform.forward);

                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _SpeedRotation * Time.deltaTime);

            } 
           

        }

        #region State
        enum EEnnemyState
        {
            Die,
            Idle,
            Searching,
            Patrol,
            Attack

        }

        private void WhatStateTheEnnemyIs()
        {


            {
                switch (_EnemyState)
                {
                    case EEnnemyState.Die:
                        if (_Meshagent.enabled == true)
                        {
                            _Paysan.gameObject.tag = "Untagged";
                            _Meshagent.enabled = false; 

                            
                        }
                        break;
                    case EEnnemyState.Idle:
                        if (_IsInClosetoMe)
                        {
                            SwitchState(EEnnemyState.Attack);


                        }

                        break;

                    case EEnnemyState.Searching:
                        _Meshagent.speed = Mathf.Clamp(_Meshagent.speed, _BasicSpeed / 2, _BasicSpeed * _ModifySpeed);
                        if (_IsInClosetoMe)
                        {
                            SwitchState(EEnnemyState.Attack);


                        }

                        break;
                    case EEnnemyState.Patrol:
                        RegularPath();
                        _Meshagent.speed = Mathf.Clamp(_Meshagent.speed, _BasicSpeed / 2, _BasicSpeed * _ModifySpeed);
                        break;
                    case EEnnemyState.Attack:

                        GoOnTheTarget();

                        if ( _Attackcouroutine != null || _ThrowAttack !=null)
                        {
                            break;

                        }
                        else
                        {
                           
                            if (_Target != null)
                            {
                                RotationOfTheennemy();
                            } else
                            {
                                SwitchState(EEnnemyState.Searching);
                            }
                           
                        }
                        if(_Attackcouroutine == null && _ThrowAttack== null )
                        {
                            _Meshagent.speed = Mathf.Clamp(_Meshagent.speed, _BasicSpeed / 2, _BasicSpeed * _ModifySpeed);
                        }


                        if (_IsItTheplayer)
                        {
                            if (_NumberOfRanged > 0 && _Attackcouroutine == null)
                            {
                                if (_Meshagent.remainingDistance > _DistanceToThrow && _DoIThrowOrNot == null)
                                {
                                    _DoIThrowOrNot = StartCoroutine(DoILaunchatorch());

                                }
                            }

                            if (_CurrentAngle<= _AngleToHaveBeforeAttacking &&  _Meshagent.remainingDistance <= _Meshagent.stoppingDistance)
                            {
                                

                                if (_DoIThrowOrNot != null)
                                {
                                    StopCoroutine(_DoIThrowOrNot);
                                    _DoIThrowOrNot = null;
                                }
                                

                                if (_Attackcouroutine == null && _ThrowAttack == null)
                                {

                                    _Attackcouroutine = StartCoroutine(StopForMeleeAttack());
                                }
                                else
                                {
                                    return;
                                }

                             
                            }

                        }

                        if (!_IsItTheplayer)

                        {
                            if (_DoIThrowOrNot != null)
                            {
                                StopCoroutine(_DoIThrowOrNot);
                                _DoIThrowOrNot = null;
                            }
                            if (_Meshagent.remainingDistance <= _Meshagent.stoppingDistance)
                            {
                                SwitchState(EEnnemyState.Searching);
                                _AggroScript.DestroyTheFakeTarget();

                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }





        #region Enter/Exit State
        private void EEnterState()
        {
            switch (_EnemyState)
            {
                case EEnnemyState.Die:
                    StopAllCoroutines();
                    _SpeedRotation = 0;
                    _Meshagent.stoppingDistance = 0;
                    _Meshagent.acceleration = 0;
                    _Meshagent.speed = 0;
                    _EnnemyAnimator.SetBool("Isdead", true);
                    _AudioSource.clip = null;
                   
                    _AudioSource.clip = _AudioDeath[Random.Range(0, _AudioDeath.Count)];
                    _AudioSource.Play();
                    Destroy(_MeleeGround);
                    Destroy(_MeleeVFX);
                    break;

                case EEnnemyState.Idle:

                    break;
                case EEnnemyState.Searching:
                    _IsInClosetoMe = false;
                    if (_IdleSearchCouroutine == null)
                    {
                        _IdleSearchCouroutine = StartCoroutine(LaunchIdleSearch());


                    }
                    else
                    {
                        return;



                    }
                    break;
                case EEnnemyState.Patrol:


                    break;

                case EEnnemyState.Attack:
                    GoOnTheTarget();
                    _Meshagent.stoppingDistance *= _ModifyStopDistance;
                    _Meshagent.acceleration *= _ModifyAcceleration;
                    _Meshagent.speed *= _ModifySpeed;
                    break;
                default:
                    break;
            }
        }

        void SwitchState(EEnnemyState newState)
        {
            EExitState();
            _EnemyState = newState;
            EEnterState();
        }

        private void EExitState()
        {
            {
                switch (_EnemyState)
                {
                    case EEnnemyState.Die:


                        break;
                    case EEnnemyState.Idle:

                        break;



                    case EEnnemyState.Searching:

                        if (_IdleSearchCouroutine != null)
                        {
                            StopCoroutine(_IdleSearchCouroutine);
                            _IdleSearchCouroutine = null;
                            _EnnemyAnimator.SetBool("LookAround", false);
                            _EnnemyAnimator.SetBool("LookMode", false);
                        }
                        _Meshagent.speed = _BasicSpeed;


                        break;
                    case EEnnemyState.Patrol:
                        _EnnemyAnimator.SetBool("LookMode", false);


                        break;
                    case EEnnemyState.Attack:
                        _Meshagent.acceleration /= _ModifyAcceleration;
                        _Meshagent.speed /= _ModifySpeed;
                        _Meshagent.stoppingDistance /= _ModifyStopDistance;
                        _Target = null;
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        #endregion


        public override void GetShot(float damage, Glo_Entities striker)
        {
            if (striker is Glo_Entities || striker is Glo_Traps)
            {
                if (_IsDead == false)
                {
                    _MaxHeal -= damage;
                    Debug.Log("vie de l'enemy" + _MaxHeal);
                    if ( _EnemyState != EEnnemyState.Attack)
                    {
                        SwitchState(EEnnemyState.Attack);
                       
                        _AggroScript.InstianteNewChase(striker.transform.position,true);
                        _AggroScript.UpgradeRadisuAggro();
                    }
                    if (_MaxHeal <= 0)
                    {
                        SwitchState(EEnnemyState.Die);
                        _AggroScript.ImDead();
                        _IsDead = true;
                    }
                }
               
               
            }
        }
        private void Update()
        {
            if (_IsDead == false)
            {
               
                if (_MaxHeal <= 0)
                {
                    SwitchState(EEnnemyState.Die);
                    _AggroScript.ImDead();
                    _IsDead = true;
                }
            }

            
            WhatStateTheEnnemyIs();

     


        }
    }
}

