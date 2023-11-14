using BagareBrian;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagareBrian
{
    public class BB_EnnemyMelee : MonoBehaviour
    {
        [Header("Animation")]
        [SerializeField] private AnimationClip _MeleeFork;
        [SerializeField] private float _SpeedAnimMelee;
        private Animator _EnnemyAnimator;
        [Header("TouchVFX")]
        [SerializeField] private ParticleSystem _TouchParticles;

        [Header("Fork")]
        [SerializeField] private GameObject _Fork;
        [SerializeField] private GameObject _ForkCone;
        [SerializeField] private float _SpeedForkVFX;
        [SerializeField] private float _SpeedDisplacementVFX;
        [SerializeField] private float _ForkDamage;
        private Collider _ForkCollider;
        private Animator _ForkAnimator;
        private Material _ForkMaterial;
        private bool _IsLaunchForkVfx = false;

        [Header("Ground")]
        [SerializeField] private GameObject _GroundVFXFork;
        [SerializeField] private float _SpeedVFX;
        private Material _GroundMaterial;
        private bool _IsLaunchGroundVFX = false;

        private Glo_Entities _Entities;
        private float _Currentvalueofthematerial;

        private void Start()
        {

            _ForkAnimator = _Fork.GetComponent<Animator>();
            _ForkMaterial = _ForkCone.GetComponent<MeshRenderer>().material;
            _EnnemyAnimator = this.gameObject.GetComponent<Animator>();
            _ForkCollider = _ForkCone.GetComponent<Collider>();
            _GroundMaterial = _GroundVFXFork.GetComponent<MeshRenderer>().material;
            _ForkCollider.enabled = false;
            _GroundMaterial.SetFloat("_OffsetY", 0);
            _ForkMaterial.SetFloat("_activation", 0);
            _ForkMaterial.SetFloat("_Dispplacement", 0);
            _GroundVFXFork.SetActive(false);
            _EnnemyAnimator.SetFloat("SpeedMelee", _SpeedAnimMelee);
            _Entities = this.GetComponent<BB_Ennemy>().GetComponent<Glo_Entities>();
        }

        #region EventAnimation

        private void LaunchAttack()
        {
            _ForkAnimator.SetBool("Attack", true);
            _GroundVFXFork.SetActive(true);
        }
        private void DisableAttack()
        {
            _GroundVFXFork.SetActive(false);
            _ForkAnimator.SetBool("Attack", false);
        }
        private void StartForkVFX()
        {
            _IsLaunchForkVfx = true;


        }
        private void StartGroundVFX()
        {

            _IsLaunchGroundVFX = true;
        }

        private void AnnulationVfx()
        {
            _IsLaunchGroundVFX = false;
            _IsLaunchForkVfx = false;
            _ForkCollider.enabled = false;
        }

        private void TouchVFX()
        {
            _TouchParticles.Play();
            _ForkCollider.enabled = true;

        }
        #endregion

        public void DoDamage(Collider other)
        {
            Glo_ITakeDamage playerDamage = other.GetComponentInParent<Glo_ITakeDamage>();
            if (playerDamage != null)
            {
                playerDamage.GetShot(_ForkDamage, _Entities);
                Debug.Log(playerDamage);
            }
        }


        #region UpdateVFX
        private void groundvfx()
        {
            if (_IsLaunchGroundVFX)
            {

                float currentValue = _GroundMaterial.GetFloat("_OffsetY");
                currentValue = Mathf.Clamp(currentValue += Time.deltaTime * _SpeedVFX * _SpeedAnimMelee, 0, 1);
                _GroundMaterial.SetFloat("_OffsetY", currentValue);

            }
            else
            {
                float currentValue = _GroundMaterial.GetFloat("_OffsetY");
                currentValue = Mathf.Clamp(currentValue -= Time.deltaTime * _SpeedVFX * 2 * _SpeedAnimMelee, 0, 1);
                _GroundMaterial.SetFloat("_OffsetY", currentValue);
            }
        }

        private void forkVfx()
        {
            if (_IsLaunchForkVfx)
            {

                float currentValue = _ForkMaterial.GetFloat("_activation");
                currentValue = Mathf.Clamp(currentValue += Time.deltaTime * _SpeedForkVFX * _SpeedAnimMelee, 0, 1);
                _ForkMaterial.SetFloat("_activation", currentValue);

                float currentValueDisplacement = _ForkMaterial.GetFloat("_Dispplacement");
                currentValueDisplacement = Mathf.Clamp(currentValueDisplacement += Time.deltaTime * _SpeedDisplacementVFX * _SpeedAnimMelee, 0, 1);
                _ForkMaterial.SetFloat("_Dispplacement", currentValueDisplacement);



            }
            else
            {
                float currentValue = _ForkMaterial.GetFloat("_activation");
                currentValue = Mathf.Clamp(currentValue -= Time.deltaTime * _SpeedForkVFX * 2 * _SpeedAnimMelee, 0, 1);
                _ForkMaterial.SetFloat("_activation", currentValue);
                _ForkMaterial.SetFloat("_Dispplacement", 0);
            }
        }
        #endregion
        private void Update()
        {
            groundvfx();
            forkVfx();

        }
    }
}

