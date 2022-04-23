using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using IdleGame.Managers;

namespace IdleGame.Interactable
{
    public class Stockpile : MonoBehaviour, IInteractable
    {
        public bool FullCapacity { get; set; }
        public InteractableType Type { get; set; }

        [SerializeField] private int stockpileCapacity;

        private List<ObjectData> _objectDataList = new List<ObjectData>();

        private float _localX = 1, _localY = 0.5f, _localZ = 5;

        private int _counter;

        private void Awake()
        {
            Type = InteractableType.Stockpile;
        }
        private void Start()
        {
            InitializePositions();
        }
        private void InitializePositions()
        {
            for (int i = 0; i < stockpileCapacity; i++)
            {
                _objectDataList.Add(new ObjectData(new Vector3(_localX + transform.position.x,
                    _localY + transform.position.y, _localZ + transform.position.z)));

                if (_localX > 2)
                {
                    _localY++;
                    _localX = 0;
                }
                if (_localY > 2)
                {
                    _localZ--;
                    _localY = 0.5f;
                }
                _localX++;
            }
        }
        public void TakeObject(GameObject givenObj, Transform parent)
        {
            if (_counter < stockpileCapacity)
            {
                if (givenObj == null) return;
                _objectDataList[_counter].ObjectHeld = givenObj;
                givenObj.transform.rotation = Quaternion.Euler(0, 0, 0);
                givenObj.transform.DOMove(_objectDataList[_counter].ObjectPosition, 0.5f);
                _counter++;
                GameManager.instance.AddBoxToStockpile();
                if (_counter >= stockpileCapacity)
                {
                    FullCapacity = true;
                }
            }
        }
        public GameObject GiveObject()
        {
            FullCapacity = false;
            if (_counter <= 0)
            {
                return null;
            }
            _counter--;
            GameManager.instance.RemoveBoxFromStockpile();

            var temp = _objectDataList[_counter].ObjectHeld;
            _objectDataList[_counter].ObjectHeld = null;
            return temp;
        }
    }
}
