using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IdleGame.Managers;
using DG.Tweening;

namespace IdleGame.Interactable
{
    public class Backpack : MonoBehaviour, IInteractable
    {
        public bool FullCapacity { get; set; }
        public InteractableType Type { get; set; }
        public int Counter { get; private set; }

        [SerializeField] private Transform backpackTransform;

        [SerializeField] private int backpackCapacity;

        [SerializeField] private float gatherRate = 0.1f;

        [SerializeField] private GathererType gathererType;

        private List<ObjectData> _objectDataList = new List<ObjectData>();

        private Animator _animator;

        private float _localY;
        private float _objectTransferSpeed = 0.15f;

        private bool _inTheZone;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }
        private void Start()
        {
            InitializePositions();
        }
        private void InitializePositions()
        {
            for (int i = 0; i < backpackCapacity; i++)
            {
                _localY++;
                _objectDataList.Add(new ObjectData(new Vector3(0, _localY, 0)));
            }
        }
        public void TakeObject(GameObject givenObj, Transform parent)
        {
            if (Counter < backpackCapacity)
            {
                if (givenObj == null) return;
                _animator.SetBool("IsCarrying", true);
                _objectDataList[Counter].ObjectHeld = givenObj;
                givenObj.transform.rotation = transform.rotation;
                givenObj.transform.SetParent(transform);

                givenObj.transform.DOJump(new Vector3(backpackTransform.position.x,
                    backpackTransform.position.y + _objectDataList[Counter].ObjectPosition.y,
                    backpackTransform.position.z),2,1, _objectTransferSpeed);
                StartCoroutine(Co_CorrectCubePosition(givenObj, Counter));
                Counter++;
                if (gathererType == GathererType.Player)
                    GameManager.instance.AddBoxToBackpack();
                if (Counter >= backpackCapacity) FullCapacity = true;
            }
        }
        private IEnumerator Co_CorrectCubePosition(GameObject go, int position)
        {
            yield return new WaitForSeconds(_objectTransferSpeed);
            go.transform.position = new Vector3(backpackTransform.position.x,
                    backpackTransform.position.y + _objectDataList[position].ObjectPosition.y,
                    backpackTransform.position.z);
        } //Coroutine'den kurtul timer'a bağla
        public GameObject GiveObject()
        {
            if (Counter <= 0)
            {
                return null;
            }
            Counter--;
            if (Counter <= 0)
            {
                _animator.SetBool("IsCarrying", false);
            }
            FullCapacity = false;
            if (gathererType == GathererType.Player)
                GameManager.instance.RemoveBoxFromBackpack();

            var temp = _objectDataList[Counter].ObjectHeld;
            temp.transform.SetParent(null);
            _objectDataList[Counter].ObjectHeld = null;
            return temp;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (_inTheZone) return;
            if (other.GetComponent<IInteractable>() == null) return;
            var interactable = other.GetComponent<IInteractable>();
            _inTheZone = true;
            switch (interactable.Type)
            {
                case InteractableType.Generator:
                    StartCoroutine(Co_GetCubeFrom(interactable));
                    break;
                case InteractableType.Stockpile:
                    switch (gathererType)
                    {
                        case GathererType.Player:
                            StartCoroutine(Co_GetCubeFrom(interactable));
                            break;
                        case GathererType.Helper:
                            StartCoroutine(Co_SendCubeTo(interactable));
                            break;
                    }
                    break;
                case InteractableType.Unlockable:
                    StartCoroutine(Co_SendCubeTo(interactable));
                    break;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            _inTheZone = false;
        }
        private IEnumerator Co_GetCubeFrom(IInteractable interactable)
        {
            while (_inTheZone && !FullCapacity)
            {
                TakeObject(interactable.GiveObject(), transform);
                yield return new WaitForSeconds(gatherRate); //@todo: sürekli coroutine çağırma mümkünse mevcut çağrılan coroutine içinde devam et veya update içinde timerla hallet
            }
            yield break;
        }
        private IEnumerator Co_SendCubeTo(IInteractable interactable)
        {
            while (_inTheZone)
            {
                if (interactable.FullCapacity)
                {
                    yield return new WaitForSeconds(gatherRate);
                }
                else
                {
                    interactable.TakeObject(GiveObject(), null);
                    yield return new WaitForSeconds(gatherRate);
                }
            }
            yield break;
        }
    }
}
namespace IdleGame
{
    [System.Serializable]
    public class ObjectData
    {
        public Vector3 ObjectPosition;
        public GameObject ObjectHeld;
        public ObjectData(Vector3 pos, GameObject obj)
        {
            ObjectPosition = pos;
            ObjectHeld = obj;
        }
        public ObjectData(Vector3 pos)
        {
            ObjectPosition = pos;
            ObjectHeld = null;
        }
    }
    public interface IInteractable
    {
        public void TakeObject(GameObject givenObj, Transform parent);
        public GameObject GiveObject();
        public bool FullCapacity { get; set; }
        public InteractableType Type { get; set; }

    }
    public enum InteractableType
    {
        Stockpile = 0, Generator = 1, Unlockable = 2
    }
    public enum GathererType
    {
        Player = 0, Helper = 1
    }
}