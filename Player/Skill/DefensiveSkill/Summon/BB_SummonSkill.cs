using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BagareBrian
{
    [CreateAssetMenu(menuName = "ScriptableObject/BB_ScriptableObject/Skills/SummonSkill")]
    public class BB_SummonSkill : BB_DefensiveSkill
    {

        [SerializeField] private GameObject _Bait;
     

        [Header("SKills Information")]
        [SerializeField] private float _DistanceBetweenPlayerAndBait;
        [SerializeField] private float _DistanceInPourcentageToStopAnim;
        [SerializeField] private float _StartPositionOfthEBait;
        [SerializeField] private float _SpeedToGetOutOfTheGround;
        [SerializeField] private float _DistanceWithAWall;
        [SerializeField] private float _YOffset;

        private float _Test;
        private GameObject _CurrentBait;
        public GameObject Bait => _Bait;

        public void DoITouchaWall()
        {

        }
        public override void SkillEffect(Transform player, Transform start, Glo_Entities entities)
        {
            if (_CurrentBait != null)
            {
                Destroy(_CurrentBait);
                _CurrentBait = null;
            }
            Transform offsetForTheBait = start.transform;
            offsetForTheBait.Translate(Vector3.forward * _DistanceBetweenPlayerAndBait);
            Vector3 beginInTheground = new Vector3(offsetForTheBait.position.x, offsetForTheBait.position.y, offsetForTheBait.position.z);
            Vector3 baitEnd = new Vector3(offsetForTheBait.position.x, offsetForTheBait.position.y, offsetForTheBait.position.z);
            player.position = new Vector3(player.position.x, player.position.y + _YOffset, player.position.z);
            RaycastHit hit;
            if (Physics.Raycast(player.position, offsetForTheBait.transform.forward, out hit,_DistanceBetweenPlayerAndBait))
            {
             
              Debug.DrawRay(player.position, offsetForTheBait.transform.forward, Color.green, 15f);
                Debug.DrawRay(player.position, hit.point,Color.red,15f);
              
                Debug.Log("J'ai toucher quelque chose");
                Debug.Log(hit.collider);

                if (hit.collider.tag == ("Wall") || hit.collider.tag == ("CheckPoint") || hit.collider.tag != null) 
                {
              
                    Vector3 spawnPosition = hit.point - offsetForTheBait.forward * _DistanceWithAWall;
                    beginInTheground = new Vector3(spawnPosition.x, offsetForTheBait.position.y, spawnPosition.z );
                    baitEnd = new Vector3(spawnPosition.x, offsetForTheBait.position.y, spawnPosition.z );
                   
                    GameObject Bait = Instantiate(_Bait, beginInTheground, player.rotation);
                    _CurrentBait = Bait;
                    BB_Bait prefabBait = Bait.GetComponent<BB_Bait>();
                    prefabBait.GiveInformation(beginInTheground, baitEnd, _SpeedToGetOutOfTheGround, _DistanceInPourcentageToStopAnim, InvicibleFrame);
                    prefabBait.ActivePhase();
                    return;
                }

          
                
            } else
            {
               GameObject Bait = Instantiate(_Bait, beginInTheground, player.rotation);
                _CurrentBait = Bait;
                BB_Bait prefabBait = Bait.GetComponent<BB_Bait>();
                prefabBait.GiveInformation(beginInTheground, baitEnd, _SpeedToGetOutOfTheGround, _DistanceInPourcentageToStopAnim, InvicibleFrame);
                prefabBait.ActivePhase();
            }

           
            
           
           
           



        }
    }

}


