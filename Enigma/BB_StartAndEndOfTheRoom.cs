using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagareBrian
{
    public class BB_StartAndEndOfTheRoom : MonoBehaviour
    {

        public delegate void CameraMoveEvent();
        public static event CameraMoveEvent _MoveCamera;




        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _MoveCamera?.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _MoveCamera?.Invoke();
            }
        }
    }
}

