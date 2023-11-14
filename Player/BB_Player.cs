using Global;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace BagareBrian
{
    public class BB_Player : Glo_Players
    {
        [Header("Sounds")]
        [SerializeField] private float _PitchInHeal;
        [SerializeField] private AudioSource _AudioSource;
        [SerializeField] private AudioSource _HealAudioSource;
        [SerializeField] private List<AudioClip> _AudioMeleeClip;
        [SerializeField] private List<AudioClip> _AudioRanged;
        [SerializeField] private List<AudioClip> _AudioSummon;
        [SerializeField] private List<AudioClip> _AudioDodge;
        [SerializeField] private List<AudioClip> _AudioDeath;
        [SerializeField] private List<AudioClip> _AudioHeal;
        [SerializeField] private Glo_PlayerData _playerData;
        [SerializeField] private GameObject _PlayerController;
        [SerializeField] private GameObject _BloodObs;

        private Glo_Entities _PlayerEntities;
        [SerializeField] private Camera _CameraFOrGameplayScene;
        private BB_PPSobelTargetAndVignette _PPSobelTargetAndVignette;
        [SerializeField] private float _ActiveSobelAtPourcentHP;
        [SerializeField] private Material _PlayerMaterial;
        private Coroutine _TimeBeforeNewSong;

        [Header("PLayer Stats")]
        [Space(5)]
        [SerializeField] private float _MaxHealt;
        [SerializeField] private float _Speed;
        [SerializeField] private float _SoothUpdateSpeed;
        [SerializeField] private float _SpeedRotation;
        [SerializeField] private float _SpeedSmooth;
        [SerializeField] private float _AttackSpeed;
        [SerializeField] private float _SpeedToRecoverMana;

        private float _CurrentHealt;
        private float _CurrentMana;
        private Vector3 _PreviousPosition;
        private float _KeepSpeed;

        #region
        [Header("Player Skills")]


        [Header("Melee SKills")]
        [Space(5)]
        [SerializeField] private BB_OffensiveSkill _MeleeSkill;
        [SerializeField] private BB_MeleeEffect _MeleeEffect;
        [SerializeField] private GameObject _Sword;
        [SerializeField] private AnimationClip _MeleeAnimation;
        [SerializeField] private ParticleSystem _MeleeSlash;
        private float _CurrentMeleeCooldown;

        [Space(5)]
        [Header("Ranged Skills")]
        [Space(5)]
        [SerializeField] private BB_OffensiveSkill _RangedSkill;
        [SerializeField] private Transform _StartRangedSkill;
        [SerializeField] private BB_RangedLoading _RangedLoading;
        private float _CurrentRangedCooldown;

        [Space(5)]
        [Header("Heal Skills")]
        [Space(5)]
        [SerializeField] private BB_HealSkill _HealSkill;
        [SerializeField] private Transform _HealTransform;
        [SerializeField] private Transform _EndHealCurve;
     
        [SerializeField] private BB_HealRay _HealRay;
        [SerializeField] private float _TimeBeforeDoDamage;

     
        private float _CurrentHealCooldown;
        private Glo_ITakeDamage _HealOnEnnemy;
        private NavMeshAgent _SlowEnnemy;
        private float _KeepValueOfTheSpeedEnnemy;
        private Vector3 _StartPositionOfTheEndHeal;
        private Coroutine _HealCourutine;


        [Space(5)]
        [Header("SummonSKills")]
        [Space(5)]
        [SerializeField] private BB_DefensiveSkill _SummonSkill;
        [SerializeField] private Transform _PositionForSummon;
        private float _CurrentSummonCooldown;


        [Space(5)]
        [Header("DodgeSKills")]
        [Space(5)]
        [SerializeField] private BB_DefensiveSkill _DodgeSkill;
        [SerializeField] private ParticleSystem _Smoke;
        [SerializeField] private Transform _SmokeGenerator;
        [SerializeField] private GameObject _Bat;
        [SerializeField] private float _MultiplySpeedForBat;
        [SerializeField] private float _SpeedOfTransform;
        [SerializeField] private float _AirControlIntensity;
        private float _CurrentDodgeCooldown;
        private bool _IsABat;
        private bool _IsLaunchingDodgeVFX;
        private Material _BatMaterial;

        [Space(5)]
      
        #endregion

        private float m_timeReadOfTheCurve;

        [Header("Animator And Animations")]
        [SerializeField] private AnimationClip _Dead;
        [SerializeField] private GameObject _DeadGround;
        [SerializeField] private float _TimeBeforeLaunchAnIdleBreak;
        [SerializeField] private float _TimeToReturnToIdleNeutral;
        [SerializeField] private Animator _PlayerAnimator;
        [SerializeField] private float _SpeedOfTheLayerWeightForTheHand;

       [SerializeField] private AnimationClip _HealClipForEvent;
        private Material _DeadGroundMat;
        private bool _IsLaunchGroundAnimation;
        private bool _IsGoDownForDeadAnim;
        private bool _IsAttack;
        private Coroutine _IdleBreakCourutine;
        private int _OrbsLayerAnimato;
        private bool _IsDead;
        private bool _IsUseAnotherAttack;
        private void OnEnable()
        {
            _IsLaunchGroundAnimation = false;
            _IsDead = false;
            _IsUseAnotherAttack = false;
            _IsABat = false;
            _PlayerMaterial.SetFloat("_Apparition", 0);
            _PlayerMaterial.SetFloat("_DeathColor", 1);
          
          
            _CurrentHealt = _playerData._playerHealth;
          
           
           _CurrentSummonCooldown = _SummonSkill.coolDown*_playerData._playerInvocationCooldown;
            _CurrentRangedCooldown = _RangedSkill.coolDown* _playerData._playerLongRangeCooldown;
            _CurrentHealCooldown = _HealSkill.coolDown*_playerData._playerHealCooldown;
            _CurrentDodgeCooldown = _DodgeSkill.coolDown* _playerData._playerDashCooldown;

            _DeadGroundMat = _DeadGround.GetComponent<MeshRenderer>().material;
            _BatMaterial = _Bat.GetComponent<MeshRenderer>().material;
            _BatMaterial.SetFloat("_Appear", 0);
            _PlayerAnimator.SetFloat("AttackSpeed", _AttackSpeed);
            _OrbsLayerAnimato = _PlayerAnimator.GetLayerIndex("OrbHand");
            _KeepSpeed = _Speed;
            _StartPositionOfTheEndHeal = _EndHealCurve.position;
            _IsLaunchingDodgeVFX = false;
            _MeleeEffect.NeedInformation(_MeleeSkill.DamageDone, _PlayerEntities, _AttackSpeed);


            if (_CameraFOrGameplayScene.GetComponent<BB_PPSobelTargetAndVignette>() != null)
            {
                _PPSobelTargetAndVignette = _CameraFOrGameplayScene.GetComponent<BB_PPSobelTargetAndVignette>();
            }
            _PlayerEntities = this.GetComponent<Glo_Entities>();
            SwitchState(EPlayerState.Idle);
        }

        enum EPlayerState
        {
            Dead,
            Idle,
            Movement,
            Attack
        }

        private EPlayerState _StatePlayer;

        private void Start()
        {
            _MeleeEffect.NeedInformation(_MeleeSkill.DamageDone, _PlayerEntities, _AttackSpeed);

            /*  _DeadGroundMat = _DeadGround.GetComponent<MeshRenderer>().material;
              _BatMaterial = _Bat.GetComponent<MeshRenderer>().material;
              _BatMaterial.SetFloat("_Appear", 0);
              _PlayerAnimator.SetFloat("AttackSpeed", _AttackSpeed);
              _OrbsLayerAnimato = _PlayerAnimator.GetLayerIndex("OrbHand");
              _KeepSpeed = _Speed;
              _StartPositionOfTheEndHeal = _EndHealCurve.position;
              _IsLaunchingDodgeVFX = false;
              _MeleeEffect.NeedInformation(_MeleeSkill.DamageDone, _PlayerEntities,_AttackSpeed);*/
        }

        #region Movement

        private void Movemement()
        {
           
            if (gameObject != null && _IsDead == false)
            {
                Vector3 direction = new Vector3(Input.GetAxis("LS_H"), 0f, Input.GetAxis("LS_V"));
                if (_IsLaunchingDodgeVFX)
                {
                    transform.Translate((transform.forward * _Speed * Time.deltaTime), Space.World);
                    if (direction.magnitude > 0.10f)
                    {
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction),_AirControlIntensity*  Time.deltaTime);
                    }
                    return;
                }
                if (!_IsAttack)
                {
                    if ((Input.GetAxis("LS_H") != 0 || (Input.GetAxis("LS_V") != 0)))
                    {
                        SwitchState(EPlayerState.Movement);
                    }

                }
                transform.Translate(direction * (_Speed * Time.deltaTime), Space.World);
               

                _PlayerAnimator.SetFloat("Walk_Run", direction.magnitude);




                //Rotation
                if (direction.magnitude > 0.10f)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), _SpeedRotation * Time.deltaTime);
                }




            }


        }

        #endregion

        #region DeadAnimation

        private void AGround()
        {
            _IsLaunchGroundAnimation = true;


        }
        private void GoDown()
        {
            _IsGoDownForDeadAnim = true;
            _PlayerMaterial.SetFloat("_DeathColor", 0);
        }
        private void DGround()
        {
            _IsLaunchGroundAnimation = false;
        }


        #endregion

        #region Skills


        IEnumerator LaunchAttack(float animationLenght, string animationName)
        {
            
            _PlayerAnimator.SetTrigger(animationName);
            _IsUseAnotherAttack = true;
            SwitchState(EPlayerState.Attack); Glo_GameManager._Instance.SpellCast = true;
            yield return new WaitForSeconds((float)animationLenght);

           
            _PlayerAnimator.ResetTrigger(animationName);
            Glo_GameManager._Instance.SpellCast = false;
            SwitchState(EPlayerState.Idle);
            _IsUseAnotherAttack = false;

        }

        private void WhatSkilltoUse()
        {
            if (_IsDead == false && _IsLaunchingDodgeVFX == false)
            {
                if (_IsAttack == false)
                {
                    #region Melee
                    if (Input.GetButtonDown("X")  )
                    {

                        
                        Debug.Log("melee");
                        _AudioSource.clip = _AudioMeleeClip[Random.Range(0, _AudioMeleeClip.Count)];
                        _AudioSource.Play();
                        StartCoroutine(LaunchAttack(_MeleeSkill.TimeForanimation / _AttackSpeed, _MeleeSkill.NameOfTheSkill));
                       
                       

                    }
                    #endregion

                    #region RangedAttack
                    if (Input.GetButtonDown("A") && _CurrentRangedCooldown ==0 && _playerData._PlayerMana>= _RangedSkill.manacost)
                    {
                        //RangedAttack
                        StartCoroutine(LaunchAttack(_RangedSkill.TimeForanimation / _AttackSpeed, _RangedSkill.NameOfTheSkill));

                        _RangedLoading.GrowUpOrbs();

                        _CurrentRangedCooldown = _RangedSkill.coolDown;
                        _AudioSource.clip = _AudioRanged[Random.Range(0, _AudioRanged.Count)];
                        _AudioSource.Play();
                        ModifyMana(-_RangedSkill.manacost);
                    }


                    #endregion

                    #region Summon
                    if (Input.GetButtonDown("Y") && _CurrentSummonCooldown == 0 && _playerData._PlayerMana >= _SummonSkill.manacost)
                    {
                        // Summon

                        StartCoroutine(LaunchAttack(_SummonSkill.TimeForanimation, _SummonSkill.NameOfTheSkill));
                       _CurrentSummonCooldown= _SummonSkill.coolDown;
                        _AudioSource.clip = _AudioSummon[Random.Range(0, _AudioSummon.Count)];
                        _AudioSource.Play();
                       ModifyMana(-_SummonSkill.manacost);

                    }
                    #endregion

                    #region Dodge
                    if (Input.GetButtonDown("RB") && _CurrentDodgeCooldown == 0 )
                    {
                        Glo_GameManager._Instance.SpellCast = true;
                        //StartCoroutine(LaunchAttack(_DodgeSkill.TimeForanimation / _AttackSpeed, _DodgeSkill.NameOfTheSkill));

                        _AudioSource.clip = _AudioDodge[Random.Range(0, _AudioDodge.Count)];
                        _AudioSource.Play();
                        Debug.Log("Dodge");
                        _DodgeSkill.SkillEffect(transform, _SmokeGenerator, _PlayerEntities);
                        _IsLaunchingDodgeVFX = true;
                        _IsABat = true;
                        StartCoroutine(TransformationInBat());

                        _CurrentDodgeCooldown = _DodgeSkill.coolDown;
                    }


                    #endregion
                }

                #region Heal
                if (!_IsUseAnotherAttack && _CurrentHealCooldown == 0  && _playerData._PlayerMana >= _RangedSkill.manacost)
                {
                    if (Input.GetButtonDown("B"))
                    {
                        Glo_GameManager._Instance.SpellCast = true;
                        ModifyMana(-_HealSkill.manacost);
                        AudioClip _current = _AudioHeal[Random.Range(0, _AudioHeal.Count)];

                        _AudioSource.pitch = _PitchInHeal;
                        
                        _AudioSource.clip = _current;
                        _HealAudioSource.clip = _current;
                        _HealAudioSource.pitch = _AudioSource.pitch;
                      //  _AudioSource.loop = true;
                        _AudioSource.Play();
                        
                        _IsAttack = true;
                        
                    }

                    if (Input.GetButton("B"))
                    {
                        if (_StatePlayer != EPlayerState.Attack)
                        {
                            SwitchState(EPlayerState.Attack);
                        }
                        if (_TimeBeforeNewSong == null)
                        {
                          _TimeBeforeNewSong = StartCoroutine(WaitBeforeLaunchANewSong());
                        }
                        ///heal
                        Debug.Log("heal");

                        _PlayerAnimator.SetBool("IsHealing", true);
                        ChangeWeightOfAnim(_OrbsLayerAnimato, -_SpeedOfTheLayerWeightForTheHand, 0);
                       
                        DoITouchEnnemy();


                    }
                    else
                    {
                        ChangeWeightOfAnim(_OrbsLayerAnimato, _SpeedOfTheLayerWeightForTheHand, 1);
                    }

                    if (Input.GetButtonUp("B"))
                    {
                        if (_TimeBeforeNewSong != null)
                        {
                            StopCoroutine(_TimeBeforeNewSong);

                            _TimeBeforeNewSong = null;
                        }
                      
                        _HealAudioSource.Stop();
                        _AudioSource.Stop();
                        _AudioSource.clip= null;
                        _HealAudioSource.clip = null;
                        _AudioSource.pitch = 1;
                        _HealAudioSource.pitch = 1;
                          //  _AudioSource.loop = false;
                        Debug.Log("healStop");
                        _PlayerAnimator.SetBool("IsHealing", false);
                        SwitchState(EPlayerState.Movement);
                        _CurrentHealCooldown = _HealSkill.coolDown;
                        _HealRay.DisableTheHealRay();
                        _HealRay.LaunchTheHealCurve(false);
                        _IsAttack = false;
                        Glo_GameManager._Instance.SpellCast = false;
                        if (_HealCourutine != null)
                        {
                            StopCoroutine(_HealCourutine);
                            _HealCourutine = null;
                        }
                        if (_SlowEnnemy != null)
                        {
                            _SlowEnnemy.speed = _KeepValueOfTheSpeedEnnemy;
                            _SlowEnnemy = null;
                        }
                        //  m_EndHealCurve.position = m_healTransform.position + transform.forward * 20;
                    }
                }
              

                #endregion
            }
        }
        IEnumerator WaitBeforeLaunchANewSong()
        {
            //   AudioClip _current = _AudioHeal[Random.Range(0, _AudioHeal.Count)];
            yield return new WaitForSecondsRealtime((_AudioSource.clip.length / _AudioSource.pitch) / 2); // ToHaveTheMiddleOfTheAnimationTime

            _HealAudioSource.Play();
            //  _AudioSource.pitch = 0.5f;
            yield return new WaitForSecondsRealtime((_AudioSource.clip.length/_AudioSource.pitch)/2);
            _AudioSource.Play();
              _TimeBeforeNewSong = null;
            //_AudioSource.clip = _current;
            //_AudioSource.Play();

        }

        #region EventAnim/ SkillEffect

        #region Ranged
        private void Launchprojectil()
        {
            _RangedLoading.LaunchObs(_RangedSkill.DamageDone, _PlayerEntities);
            _RangedSkill.SkillEffect(transform, _StartRangedSkill, _PlayerEntities);

        }
        #endregion

        #region Summon Bait
        private void BloodPuddle()
        {

            _PositionForSummon.position = this.transform.position;

            //Debug.DrawLine(this.transform.position, this.transform.forward * 20, Color.cyan, 15f);
            _SummonSkill.SkillEffect(this.transform, _PositionForSummon, _PlayerEntities);
            //Debug.DrawLine(_PositionForSummon.position, _PositionForSummon.transform.forward*20, Color.red, 15f);

        }

        private void Apparition()
        {

            // m_summonSkill.SkillEffect(transform, transform);
            StartCoroutine(TimeBaitIsActive());
        }
        IEnumerator TimeBaitIsActive()
        {
            _PlayerController.tag = "Untagged";

            yield return new WaitForSecondsRealtime(_SummonSkill.InvicibleFrame);
            _PlayerController.tag = "Player";
        }
        #endregion

        #region Melee
        public void ActiveCollider()
        {
            // m_meleeSkill.SkillEffect(transform, transform);
            // m_MeleeCollider.enabled = true;
            // _MeleeEffect.ActiveDisableCollider();
            _MeleeEffect.ActiveDisableCollider();

        }
        public void DisableCollider()
        {
            _MeleeEffect.ActiveDisableCollider();
            _MeleeEffect.DisableSlash();
        }
        public void LaunchVFXSword()
        {
            _MeleeEffect.ActiveVFXSword();
        }
        public void DisableVFXSword()
        {

            _MeleeEffect.DisableVFXSword();

        }

        public void ActiveSlash()
        {
            _MeleeEffect.LaunchSlash();
            _MeleeSlash.Play();
        }


        #endregion



        #region Dodge

        private void TransformBat()
        {

            if (_IsLaunchingDodgeVFX)
            {
                if (_IsABat)
                {
                    float currentValue = _PlayerMaterial.GetFloat("_Apparition");
                    if (currentValue >= 1)
                    {
                        _BloodObs.SetActive(false);
                        _Sword.SetActive(false);
                        float CurrentValueForTheBat = _BatMaterial.GetFloat("_Appear");

                        CurrentValueForTheBat = Mathf.Clamp(CurrentValueForTheBat += Time.deltaTime * _SpeedOfTransform, 0, 1);
                        _BatMaterial.SetFloat("_Appear", CurrentValueForTheBat);
                        return;
                    }
                    currentValue = Mathf.Clamp(currentValue += Time.deltaTime * _SpeedOfTransform, 0, 1);
                    _PlayerMaterial.SetFloat("_Apparition", currentValue);

                }

                if (!_IsABat)
                {
                    float currentValue = _BatMaterial.GetFloat("_Appear");
                    if (currentValue <= 0)
                    {
                       
                        float CurrentValueForThePlayer = _PlayerMaterial.GetFloat("_Apparition");

                        CurrentValueForThePlayer = Mathf.Clamp(CurrentValueForThePlayer -= Time.deltaTime * _SpeedOfTransform, 0, 1);
                        _PlayerMaterial.SetFloat("_Apparition", CurrentValueForThePlayer);
                        if (CurrentValueForThePlayer <= 0)
                        {
                            _BloodObs.SetActive(true);
                            _Sword.SetActive(true);
                            _IsLaunchingDodgeVFX = false;
                        }
                        return;
                    }
                    currentValue = Mathf.Clamp(currentValue -= Time.deltaTime * _SpeedOfTransform, 0, 1);
                    _BatMaterial.SetFloat("_Appear", currentValue);
                }
            }


        }

        IEnumerator TransformationInBat()
        {
            _BloodObs.SetActive(false);
            float BeforeSpeed = _Speed;
           
           
            _Speed *= _MultiplySpeedForBat;

            yield return new WaitForSeconds(_DodgeSkill.InvicibleFrame);
            Glo_GameManager._Instance.SpellCast = false;
            _Speed = BeforeSpeed;
            _IsABat = false;
            _DodgeSkill.SkillEffect(transform, _SmokeGenerator, _PlayerEntities);

        }


        #endregion

        #region Heal

        IEnumerator DamagePerSeconds()
        {
            yield return new WaitForSecondsRealtime(_TimeBeforeDoDamage);
            if (_HealOnEnnemy != null)
            {
                _HealOnEnnemy.GetShot(_HealSkill.DamageDone, _PlayerEntities);
                _playerData._playerHealth += _HealSkill.DamageDone;
                _HealCourutine = null;
            }

        }
        private void LaunchRay()
        {
            _HealRay.LauncTheHealRay();
        }

        private void ChangeWeightOfAnim(int layerIndex, float speed, float valueToReach)
        {
            float weightValue = _PlayerAnimator.GetLayerWeight(layerIndex);
            weightValue = Mathf.Clamp(weightValue += Time.deltaTime * speed, 0, valueToReach);
            _PlayerAnimator.SetLayerWeight(layerIndex, weightValue);
           
        }


        private void DoITouchEnnemy()
        {
            // TestScript


            RaycastHit hit;

            _EndHealCurve.position = _HealTransform.position + transform.forward * _HealSkill.MaxDistanceForTheRay;
            Debug.DrawRay(_HealTransform.position, transform.forward * _HealSkill.MaxDistanceForTheRay, Color.red);
            if (Physics.SphereCast(_HealTransform.position, _HealSkill.RadiusToKnowIfITouch, this.transform.forward, out hit, _HealSkill.MaxDistanceForTheRay))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    Debug.Log("Le raycast a touché l'ennemi : " + hit.collider.name);
                    _EndHealCurve.position = hit.transform.position;
                    _HealRay.LaunchTheHealCurve(true);
                    if (_HealOnEnnemy == null)
                    {
                        _HealOnEnnemy = hit.collider.GetComponentInParent<Glo_ITakeDamage>();
                        _SlowEnnemy = hit.collider.GetComponentInParent<NavMeshAgent>();
                        _KeepValueOfTheSpeedEnnemy = _SlowEnnemy.speed;
                        float MINIMUMSPEEDFORENNEMY = 1;
                        _KeepValueOfTheSpeedEnnemy = Mathf.Clamp(_KeepValueOfTheSpeedEnnemy, MINIMUMSPEEDFORENNEMY, _KeepValueOfTheSpeedEnnemy);

                    }
                    if (_HealOnEnnemy != null)
                    {
                        if (_SlowEnnemy.speed != _KeepValueOfTheSpeedEnnemy / 2)
                        {
                            _SlowEnnemy.speed = _SlowEnnemy.speed / 2;
                        }
                        if (_HealCourutine == null)
                        {

                            _HealCourutine = StartCoroutine(DamagePerSeconds());

                        }

                    }



                }
                else
                {

                    _HealRay.LaunchTheHealCurve(false);
                    if (_HealCourutine != null)
                    {
                        StopCoroutine(_HealCourutine);
                        _HealCourutine = null;
                    }
                    if (_SlowEnnemy != null)
                    {
                        _SlowEnnemy.speed = _KeepValueOfTheSpeedEnnemy;
                        _SlowEnnemy = null;
                    }
                    _HealOnEnnemy = null;
                }

            }



        }
        #endregion

        #endregion


        #endregion

        #region Idle/State

        IEnumerator TimeBeforeLaunchIdleBreak()
        {
            while (true)
            {

                yield return new WaitForSecondsRealtime(_TimeBeforeLaunchAnIdleBreak);

                _PlayerAnimator.SetFloat("IdleType", Random.Range(0, 2));
                _PlayerAnimator.SetBool("IsIdleBreak", true);
                yield return new WaitForSecondsRealtime(_TimeBeforeLaunchAnIdleBreak);

                _PlayerAnimator.SetBool("IsIdleBreak", false);

            }
        }




        private void WhatStateThePlayerIs()
        {

            if (_StatePlayer != EPlayerState.Attack)
            {
                if (_Speed < _KeepSpeed)
                {
                    IncreaseSpeed();
                }
            }
            //Debug.Log(m_speed);
            switch (_StatePlayer)
            {
                case EPlayerState.Dead:
                    if (_IsLaunchGroundAnimation)
                    {
                        float currentValue = _DeadGroundMat.GetFloat("_Fill");
                        currentValue = Mathf.Clamp(currentValue += Time.deltaTime, 0, 1);
                        _DeadGroundMat.SetFloat("_Fill", currentValue);




                    }
                    else
                    {
                        float currentValue = _DeadGroundMat.GetFloat("_Fill");
                        currentValue = Mathf.Clamp(currentValue -= Time.deltaTime, 0, 1);
                        _DeadGroundMat.SetFloat("_Fill", currentValue);


                    }

                    if (_IsGoDownForDeadAnim)
                    {
                        float currentValueForGround = _DeadGroundMat.GetFloat("_Fill");
                        float currentValue = _PlayerMaterial.GetFloat("_Apparition");
                        currentValue = Mathf.Clamp(currentValue += Time.deltaTime, 0, 1);
                        _PlayerMaterial.SetFloat("_Apparition", currentValue);
                        if (currentValue >= 1 && currentValueForGround <= 0)
                        {
                            SwitchState(EPlayerState.Idle);
                        }
                    }


                    break;
                case EPlayerState.Idle:

                    break;
                case EPlayerState.Movement:


                    if ((Input.GetAxis("LS_H") == 0 && (Input.GetAxis("LS_V") == 0)))
                    {

                        SwitchState(EPlayerState.Idle);

                    }


                    break;
                case EPlayerState.Attack:


                    if (_Speed > 0)
                    {
                        decreaseSpeed();
                        // m_speed -= Time.deltaTime * m_speed;

                    }
                    // attack = true;
                    break;
                default:
                    break;
            }
        }


        private void decreaseSpeed()
        {

          
            _Speed = Mathf.Clamp(_Speed -= Time.deltaTime * _SoothUpdateSpeed, 0, _KeepSpeed);
        }

        private void IncreaseSpeed()
        {

        

            _Speed = Mathf.Clamp(_Speed += Time.deltaTime * _SoothUpdateSpeed, 0, _KeepSpeed);
        }


        #region Enter/Exit State
        private void EEnterState()
        {
            switch (_StatePlayer)
            {
                case EPlayerState.Dead:
                    _IsDead = true;
                    StopAllCoroutines();
                    _AudioSource.clip = null;
                    _HealAudioSource.clip = null;
                    _AudioSource.clip = _AudioDeath[Random.Range(0, _AudioDeath.Count)];
                    _AudioSource.Play();
                    _IdleBreakCourutine = null;
                    _PlayerController.tag = "Untagged";
                    _PreviousPosition = transform.position;
                    break;



                case EPlayerState.Idle:

                    _PlayerAnimator.SetBool("IsMove", false);

                    if (_IdleBreakCourutine != null)
                    {

                        StopCoroutine(_IdleBreakCourutine);

                        _IdleBreakCourutine = null;


                    }

                    _IdleBreakCourutine = StartCoroutine(TimeBeforeLaunchIdleBreak());

                    break;
                case EPlayerState.Movement:
                    _PlayerAnimator.SetBool("IsMove", true);


                    break;

                case EPlayerState.Attack:
                    m_timeReadOfTheCurve = 0;
                    _IsAttack = true;
                    //m_speed = 0;
                    break;
                default:
                    break;
            }
        }

        void SwitchState(EPlayerState newState)
        {
            EExitState();
            _StatePlayer = newState;
            EEnterState();
        }

        private void EExitState()
        {
            switch (_StatePlayer)
            {
                case EPlayerState.Dead:
                    _PlayerController.tag = "Player";
                    _IsDead = false;
                    _PlayerMaterial.SetFloat("_Apparition", 0);
                    _PlayerMaterial.SetFloat("_DeathColor", 1);
                    //   _playerData._PlayerHealth = _playerData._PlayerMaxHealth/2;
                    // _playerData._playerMana = _playerData._PlayerMaxHealth/2;
                    _MaxHealt = _playerData._PlayerMaxHealth;
                    _PlayerAnimator.SetBool("IsDead", _IsDead);
                    break;



                case EPlayerState.Idle:


                    if (_IdleBreakCourutine != null)
                    {
                        StopCoroutine(_IdleBreakCourutine);

                        _IdleBreakCourutine = null;


                    }
                    _PlayerAnimator.SetBool("IsIdleBreak", false);

                    break;

                case EPlayerState.Movement:

                    break;

                case EPlayerState.Attack:
                    _IsAttack = false;
                    //      m_speed = 0;
                    // m_timeReadOfTheCurve = 1;
                    // m_speed = _KeepSpeed;
                    break;
                default:
                    break;
            }
        }
        #endregion




        #endregion

        #region Health/Cooldown/Damage/

        public void ModifyHealth(float modifier)
        {
            _CurrentHealt += modifier;
            _CurrentHealt = Mathf.Clamp(_CurrentHealt, 0f, _MaxHealt);
            //   UpdateHealthBar();
        }

        public void ModifyMana(float modifier)
        {
            _playerData._playerMana += modifier;
        }

        public void ManaBySeconds()
        {
            if (_playerData._playerMana != _playerData._PlayerMaxMana)
            {
                ModifyMana(Time.deltaTime * _SpeedToRecoverMana);
            }
        }

       
        private void CoolDownForEachSkill()
        {

            if (_CurrentDodgeCooldown > 0)
            {
                _CurrentDodgeCooldown = Mathf.Clamp(_CurrentDodgeCooldown -= Time.deltaTime, 0, _DodgeSkill.coolDown);
               _playerData._PlayerDashCooldown= _CurrentDodgeCooldown / _DodgeSkill.coolDown;   
            }
            if (_CurrentHealCooldown > 0)
            {
                _CurrentHealCooldown = Mathf.Clamp(_CurrentHealCooldown -= Time.deltaTime, 0, _HealSkill.coolDown);
                _playerData._PlayerHealCooldown = _CurrentHealCooldown / _HealSkill.coolDown;
            }

            if (_CurrentMeleeCooldown > 0)
            {
                _CurrentMeleeCooldown = Mathf.Clamp(_CurrentMeleeCooldown -= Time.deltaTime, 0, _MeleeSkill.coolDown);
                _playerData._PlayerCloseRangeCooldown = _CurrentMeleeCooldown / _MeleeSkill.coolDown;

            }
            if (_CurrentRangedCooldown > 0)
            {
                _CurrentRangedCooldown = Mathf.Clamp(_CurrentRangedCooldown -= Time.deltaTime, 0, _RangedSkill.coolDown);
                _playerData._PlayerLongRangeCooldown = _CurrentRangedCooldown / _RangedSkill.coolDown;
            }
            if (_CurrentSummonCooldown > 0)
            {
                _CurrentSummonCooldown = Mathf.Clamp(_CurrentSummonCooldown -= Time.deltaTime, 0, _SummonSkill.coolDown);
                _playerData._PlayerInvocationCooldown = _CurrentSummonCooldown / _SummonSkill.coolDown;
            }


        }



        #endregion

        

        void Update()
        {

            ManaBySeconds();
          
            WhatSkilltoUse();
            WhatStateThePlayerIs();
            TransformBat();
            CoolDownForEachSkill();
            _MaxHealt = _playerData._playerHealth;
            _CurrentMana = _playerData._playerMana;
            if (_MaxHealt < _ActiveSobelAtPourcentHP && _IsDead == false)
            {
                _PPSobelTargetAndVignette.LaunchTheSobel();
            }
            else
            {

               _PPSobelTargetAndVignette.DisableSobel();


            }
            if (_playerData._playerHealth <= 0 && _IsDead == false)
            {
                _playerData._playerHealth = 0;

                _IsDead = true;
                SwitchState(EPlayerState.Dead);
                _PlayerAnimator.SetBool("IsDead", _IsDead);
            }
        }


        private void FixedUpdate()
        {

            Movemement();
        }

        public override void GetShot(float damage, Glo_Entities striker)
        {
            if ((striker is Glo_Entities || striker is Glo_Traps )&& Glo_GameManager._Instance.GodMode ==false)
            {
                Debug.Log("Lolololol");
                if (_IsLaunchingDodgeVFX == false)
                {
                    _PPSobelTargetAndVignette.UpdateVignetteByDamage();
                    _playerData._PlayerHealth -= damage;
                    Debug.Log("vie du joueur" + _playerData._PlayerHealth);
                 /*   if (_playerData._playerHealth > 0 )
                    {
                       
                    }*/
                }


            }
        }
    }
}






///Ancien displacement
/*if ((Input.GetAxis("LS_H") == 0 && (Input.GetAxis("LS_V") == 0)))
{

    SwitchState(EPlayerState.Idle);
   // m_playerAnimator.SetBool("IsMove", false);


    if ( IsIdleBreak == false)
    {

        SwitchState(EPlayerState.IdleBreak);
        /*IsIdleBreak = true;
        StartCoroutine(BreakIdle());
    }

}
else
{
    SwitchState(EPlayerState.Movement);
    /*StopCoroutine(BreakIdle());
    m_playerAnimator.SetBool("IsIdleBreak", false);
    m_playerAnimator.SetBool("IsMove", true);
}*/