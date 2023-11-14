using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BagareBrian
{
    public class BB_GateObserver : MonoBehaviour
    {
        [SerializeField] private float _DoorIndex;
        [SerializeField] private bool _IsThisEnigmaDoor;
        [SerializeField] private List<Animator> _NumberOfFence;
        [SerializeField] private AudioSource _Source;
        [SerializeField] private List<AudioClip> _AudioList;
        private bool _IsUp;
        private void Start()
        {
            _IsUp = false;
         
        }

        private void WinEnigma()

        {
            if (this._IsThisEnigmaDoor)
            {
                _IsUp = !_IsUp;
                _Source.clip = _AudioList[Random.Range(0, _AudioList.Count) + 1];
                _Source.Play();
                OpenGate(_IsUp);
            }

        }

        private void OnEnable()
        {
            BB_LeverObserver.LeverEventPullForGate += PullForOpenGate;
            BB_EnigmaManager.WinEnigma += WinEnigma;
        }
        private void OnDisable()
        {
            BB_LeverObserver.LeverEventPullForGate -= PullForOpenGate;
            BB_EnigmaManager.WinEnigma -= WinEnigma;
        }
        private void PullForOpenGate(float Index)
        {

            if (Index == _DoorIndex && !_IsThisEnigmaDoor)
            {
                _IsUp = !_IsUp;
                _Source.clip = _AudioList[Random.Range(0, _AudioList.Count) ];
                _Source.Play(); 
                OpenGate(_IsUp);
                
            }
            else
            {

                Debug.Log("Gate Is Close");
                return;
            }



        }



        private void OpenGate(bool open)
        {

            Debug.Log("OpenGate");

            foreach (var item in _NumberOfFence)
            {
                item.SetBool("Down", open);
            }
         
        }


    }

}
