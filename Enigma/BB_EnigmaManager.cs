using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagareBrian
{
    public class BB_EnigmaManager : MonoBehaviour
    {
        [Header("Enigma Paramaters")]
        [SerializeField] private int _NumberOfTryForEnigma;
        [SerializeField] private int _TimeBeforeInitializeEnigma;
        [SerializeField] private List<BB_EnigmaAsset> _ListOfAssets = new List<BB_EnigmaAsset>();
        [SerializeField] private int _NumberOfShuffleAssets;
        private bool _IsActiveCameraEnigma = false;

        [Header("Assets Paramaters")]
        [SerializeField] private GameObject _BallToTestIfTheEnigmaSucced;
        [SerializeField] private Transform _GeneratorBalls;
        [SerializeField] private GameObject _GroundEnigma;
        [SerializeField] private Camera _EnigmaCamera;


        [Header("The Number Of Essays")]
        [SerializeField] private Transform _SpawnNewSphere;
        [SerializeField] private GameObject _PrefabSphereBasics;
        [SerializeField] private Animator _DoorForSphere;
        [SerializeField] private AnimationClip _DoorOpen;

        [Header("FinishParameters")]
        [SerializeField] private Material _FinishLine;


        [Header("Optimizer")]
        [SerializeField] private float _MaxNumberOfBallsOnTheGround;
        private bool _IsActiveSuccedVFX;
        private bool _IsActiveFailVFX;

        private float _NumberOfSpawn;
        private Coroutine _InstantiateNewBalls;

      



        public delegate void EnigmaEvent();
        public static event EnigmaEvent WinEnigma;
        public static event EnigmaEvent LoseEnigma;
      
        public static event EnigmaEvent AssetRotate;
        public static event EnigmaEvent AssetStopRotate;

        private float _CurrentTry;
        private float _CurrentSucced;
        private float _CurrentAsset;
     

        private bool _IsEnigmaSucced;
        private bool _IsLastTry;

        private float _NumberOfBall;
        private BagareBrian.BB_EnigmaManager _ThisScript;
        private List<GameObject> _BallsInThescene = new List<GameObject>();
        private void Start()
        {
           
            _ThisScript = GetComponent<BagareBrian.BB_EnigmaManager>();
            _IsEnigmaSucced = false;
           
            StartCoroutine(WaitBeforeLaunchEnigma());
            _BallsInThescene.Capacity = _NumberOfTryForEnigma;
            InstianteNewBall();
        }


        #region Coroutine
        IEnumerator WaitBeforeLaunchEnigma()
        {
            AssetRotate?.Invoke();
            _IsLastTry = false;
            yield return new WaitForSecondsRealtime(_TimeBeforeInitializeEnigma);
            _CurrentAsset = 0;
            _CurrentTry = 0;

            InitilializeEnigma();
        }

        IEnumerator WaitToLaunchNewBalls()
        {
            if (_NumberOfBall != 0)
            {
                _DoorForSphere.SetTrigger("Open");
            }
            

            yield return new WaitForSecondsRealtime(_DoorOpen.length / _DoorForSphere.GetFloat("Multi"));
            _CurrentAsset = 0;
            if (_NumberOfBall != 0)
            {
                GameObject copy = Instantiate(_BallToTestIfTheEnigmaSucced, _GeneratorBalls.position, Quaternion.identity);
                BB_EnigmaBalls copyScript = copy.GetComponent<BB_EnigmaBalls>();
                copyScript.GiveMeInformation(_GroundEnigma, _ThisScript);
            }
            else
            {
                AssetStopRotate?.Invoke();
            }
            _NumberOfBall++;
        }

        IEnumerator WaitCoroutine(float Times)
        {

            yield return new WaitForSeconds(Times);
            Instantiate(_PrefabSphereBasics, _SpawnNewSphere.position, Quaternion.identity);
            _InstantiateNewBalls = null;
        }
        #endregion

        private void InstianteNewBall()
        {
            for (int i = 0; i < _NumberOfTryForEnigma ; i++)
            {

                _InstantiateNewBalls = StartCoroutine(WaitCoroutine(i));





            }

        }
        private void OnEnable()
        {
            BB_LeverObserver.LeverEventForEnigma += NumberOfTry;
            BB_StartAndEndOfTheRoom._MoveCamera += StartAndEndOfTheRoomMoveCamera;
        }

        private void StartAndEndOfTheRoomMoveCamera()
        {
            if (_IsActiveCameraEnigma)
            {
                _IsActiveCameraEnigma = false;
                Glo_HUDManager._Instance.Cameratransform = null;
                foreach (var item in _BallsInThescene)
                {
                    Destroy(item);
                }
                return;
            }
            else
            {
                Glo_HUDManager._Instance.Cameratransform = _EnigmaCamera.transform;
                _IsActiveCameraEnigma = true;
            }
        }

        private void NumberOfTry(float Index)
        {
            AssetRotate?.Invoke();
            _CurrentTry++;

            if (_CurrentTry >= _NumberOfTryForEnigma && !_IsEnigmaSucced)
            {
              
                _IsLastTry = true;
            }
        }

        //Créditer cette partie 
        #region Shuffle
        public List<BB_EnigmaAsset> ShuffleOfFisherYates(List<BB_EnigmaAsset> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);
                BB_EnigmaAsset value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }
        #endregion

        #region Initiliazitation
        public void InitilializeEnigma()
        {

            _ListOfAssets = ShuffleOfFisherYates(_ListOfAssets);
          

            for (int i = 0; i < _ListOfAssets.Count; i++)
            {
                int KeepIndex = i;

                _ListOfAssets[i].GivemeandIndex(i);
                if (i > 0)
                {
                    KeepIndex--;
                    _ListOfAssets[i].GivemeandIndex(KeepIndex);

                }
                if (i < _ListOfAssets.Count - 1)
                {
                    KeepIndex = i;
                    KeepIndex++;
                    _ListOfAssets[i].GivemeandIndex(KeepIndex);

                }

                _ListOfAssets[i].ShuffleAssets(_NumberOfShuffleAssets);
            }
        }
        public void AddObject(BB_EnigmaAsset thisgameobject)
        {
            _ListOfAssets.Add(thisgameobject);

        }
        #endregion

        public void IsSucced(bool IsSucced)
        {

            if (IsSucced)
            {
                _CurrentSucced++;
                if (_CurrentSucced == _ListOfAssets.Count)
                {
                  
                }
                return;
            }

            if (!IsSucced)
            {
               
                return;
            }
        }

        public void AddToTheList(GameObject Sphere)
        {
            if (_BallsInThescene.Count >= _MaxNumberOfBallsOnTheGround)
            {
                Destroy(_BallsInThescene[0]);
                _BallsInThescene.Remove(_BallsInThescene[0]);
                
                
             
            }
            _BallsInThescene.Add(Sphere);
        }

        public void IsSuccedOrNot(bool SuccedOrNot)
        {
            if (SuccedOrNot)
            {
                WinEnigma?.Invoke();
                _IsActiveSuccedVFX = true;
            }
            if (!SuccedOrNot)
            {
                AssetStopRotate?.Invoke();
                _IsActiveFailVFX = true;
             

            }

            if (_IsLastTry)
            {
                if (SuccedOrNot)
                {
                   

                    WinEnigma?.Invoke();
                    return;
                }
                else
                {

                    Debug.Log("YOU Lose");
                    LoseEnigma?.Invoke();
                    StartCoroutine(WaitBeforeLaunchEnigma());
                    InstianteNewBall();
                    _NumberOfBall = 0;
                    foreach (var item in _BallsInThescene)
                    {
                      //  Destroy(item);
                    }
                    
                }
            }
        }
        public void CountOnTheLastTry()
        {
            _CurrentAsset++;

            if (_CurrentAsset == _ListOfAssets.Count)
            {
               StartCoroutine(WaitToLaunchNewBalls());
             

            }


        }
        
        private void ActiveVFXSucced()
        {
            if (_IsActiveSuccedVFX)
            {
                float currentValue = _FinishLine.GetFloat("_Succed");
                currentValue = Mathf.Clamp(currentValue += Time.deltaTime, 0, 1);
                _FinishLine.SetFloat("_Succed", currentValue);
                if (currentValue >= 1)
                {
                    _IsActiveSuccedVFX = false;
                }
            } else
            {
                float currentValue = _FinishLine.GetFloat("_Succed");
                if (currentValue == 0)
                {
                    return;
                }
                currentValue = Mathf.Clamp(currentValue -= Time.deltaTime, 0, 1);
                _FinishLine.SetFloat("_Succed", currentValue);
            }
           
        }
        private void ActiveVFXFail()
        {
            if (_IsActiveFailVFX)
            {
                float currentValue = _FinishLine.GetFloat("_Fail");
                currentValue = Mathf.Clamp(currentValue += Time.deltaTime, 0, 1);
                _FinishLine.SetFloat("_Fail", currentValue);
                if (currentValue >= 1)
                {
                    _IsActiveFailVFX = false;
                }
            }
            else
            {
                float currentValue = _FinishLine.GetFloat("_Fail");
                if (currentValue == 0)
                {
                    return;
                }
                currentValue = Mathf.Clamp(currentValue -= Time.deltaTime, 0, 1);
                _FinishLine.SetFloat("_Fail", currentValue);
            }
          
        }

        private void Update()
        {
          
            


            ActiveVFXFail();
            ActiveVFXSucced();  
        }
    }

}
