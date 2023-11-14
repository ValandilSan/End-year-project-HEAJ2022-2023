using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagareBrian
{
    public class BB_TrapObserver : Glo_Traps
    {
        [Header(" Index")]
        [SerializeField] protected float _IndexTrap;

        [Header("Parameters")]
        protected Glo_Entities _Entities;
        [SerializeField] private float _FireDamagePerSecunds;


        public delegate void TrapEvent(float Index);
        public static event TrapEvent PressurePlateActive;
        public static event TrapEvent PressurePlateReactive;


        public delegate void PressurePlateEvent(float Index, bool PlayerIn);
        public static event PressurePlateEvent GargoyleEventVFX;
        public virtual float Index => _IndexTrap;
        public virtual float FireDamage => _FireDamagePerSecunds;
        public virtual Glo_Entities Entities => _Entities;

        public virtual void WhatTrapToActive(float Index)
        {
            PressurePlateActive?.Invoke(Index);

        }

        public virtual void ReactiveTrap(float Index)
        {
            PressurePlateReactive?.Invoke(Index);
        }

        public virtual void ActiveGargoyleVFX(float Index, bool PlayerIn)
        {

            GargoyleEventVFX?.Invoke(Index, PlayerIn);
        }


    }
}

