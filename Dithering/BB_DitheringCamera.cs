
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagareBrian
{
    public class BB_DitheringCamera : MonoBehaviour
    {
        private BB_DitheringWall _CurrentWall;
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Wall")
            {

                if (other.gameObject.GetComponent<BB_DitheringWall>() != null)
                {
                    BB_DitheringWall scriptOther = other.gameObject.GetComponent<BB_DitheringWall>();
                    scriptOther.ActiveDithering();
                }

            }
            else
            {
                return;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Wall")
            {
                if (other.gameObject.GetComponent<BB_DitheringWall>() != null)
                {
                    BB_DitheringWall scriptOther = other.gameObject.GetComponent<BB_DitheringWall>();
                    scriptOther.DisableDithering();
                }
            }
            else
            {
                return;
            }
        }
    }
}

