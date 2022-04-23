﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IdleGame.Managers;
using DG.Tweening;

namespace IdleGame.Interactable
{
    //@todo: GENERATOR UNLOCK OLMAK İÇİN SON KÜBÜN GELMESİNİ BEKLESİN
    //@todo: küpleri poolla
    public class Backpack : MonoBehaviour, IInteractable
    {
        [SerializeField] private Transform backpackTransform;

        [SerializeField] private int backpackCapacity;

        [SerializeField] private float gatherRate = 0.1f;

        [SerializeField] private GathererType gathererType;

        private List<ObjectData> _objectDataList = new List<ObjectData>();

        private float _localY;
        private float _objectTransferSpeed = 0.15f;

        public int Counter { get; private set; }

        private bool inTheZone;


        public bool FullCapacity { get; set; }
        public InteractableType Type { get ; set ; }

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

                _objectDataList[Counter].ObjectHeld = givenObj;
                givenObj.transform.rotation = transform.rotation;
                givenObj.transform.SetParent(transform);

                givenObj.transform.DOMove(new Vector3(backpackTransform.position.x,
                    backpackTransform.position.y + _objectDataList[Counter].ObjectPosition.y,
                    backpackTransform.position.z), _objectTransferSpeed);

                StartCoroutine(Co_CorrectCubePosition(givenObj, Counter));
                Counter++;
                GameManager.instance.AddBoxToBackpack();
                if(Counter>=backpackCapacity) FullCapacity = true;
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
                return GameManager.instance.StockpileInstance.GiveObject();
            }
            FullCapacity = false;
            GameManager.instance.RemoveBoxFromBackpack();
            Counter--;
            var temp = _objectDataList[Counter].ObjectHeld;
            temp.transform.SetParent(null);
            _objectDataList[Counter].ObjectHeld = null;
            return temp;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (inTheZone) return;
            if (FullCapacity) return;
            if (other.GetComponent<IInteractable>() == null) return;


            var interactable = other.GetComponent<IInteractable>();
            inTheZone = true;
            Debug.Log("interacted object is " + interactable);
            if (interactable.Type == InteractableType.Stockpile || interactable.Type == InteractableType.Generator)
            {
                StartCoroutine(Co_GetCubeFrom(interactable));
            }
            else
            {
                StartCoroutine(Co_SendCubeTo(interactable));
            }
        }
        private void OnTriggerExit(Collider other)
        {
            inTheZone = false;
        }
        private IEnumerator Co_GetCubeFrom(IInteractable interactable)
        {
            if (inTheZone && !FullCapacity) // şimdilik while döngüsü kullan
            {
                TakeObject(interactable.GiveObject(), transform); 

                yield return new WaitForSeconds(gatherRate);
                StartCoroutine(Co_GetCubeFrom(interactable)); //@todo: sürekli coroutine çağırma mümkünse mevcut çağrılan coroutine içinde devam et veya update içinde timerla hallet
            }
            else
            {
                yield break;
            }
        }
        private IEnumerator Co_SendCubeTo(IInteractable interactable)
        {
            if (inTheZone)
            {
                if (interactable.FullCapacity)
                {
                    yield return new WaitForSeconds(gatherRate);
                    StartCoroutine(Co_SendCubeTo(interactable));
                }
                else
                {
                    interactable.TakeObject(GiveObject(), null);

                    yield return new WaitForSeconds(gatherRate);
                    StartCoroutine(Co_SendCubeTo(interactable));
                }

            }
            else
            {
                yield break;
            }
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
        //public bool IsGiving { get; set; }
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