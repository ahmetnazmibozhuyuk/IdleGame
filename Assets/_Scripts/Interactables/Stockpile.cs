using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using IdleGame.Managers;

namespace IdleGame.Interactable
{
    public class Stockpile : MonoBehaviour, IInteractable
    {
        private List<ObjectData> _objectDataList = new List<ObjectData>();

        [SerializeField] private int stockpileCapacity;

        private float localX = 1, localY = 0, localZ = 5;

        private int counter;


        public bool FullCapacity { get; set; }
        public InteractableType Type { get; set; }

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
                _objectDataList.Add(new ObjectData(new Vector3(localX + transform.position.x,
                    localY + transform.position.y, localZ + transform.position.z)));

                if (localX > 2)
                {
                    localY++;
                    localX = 0;
                }
                if (localY > 2)
                {
                    localZ--;
                    localY = 0;
                }
                localX++;
            }
        }
        public void TakeObject(GameObject givenObj, Transform parent)
        {
            if (counter < stockpileCapacity)
            {
                if (givenObj == null) return;
                _objectDataList[counter].ObjectHeld = givenObj;
                givenObj.transform.rotation = Quaternion.Euler(0, 0, 0);
                givenObj.transform.DOMove(_objectDataList[counter].ObjectPosition, 0.5f);
                counter++;
                GameManager.instance.AddBoxToStockpile();
                if (counter >= stockpileCapacity)
                {
                    FullCapacity = true;
                }
            }
        }
        public GameObject GiveObject()
        {
            FullCapacity = false;
            if (counter <= 0)
            {
                return null;
            }
            counter--;
            GameManager.instance.RemoveBoxFromStockpile();

            var temp = _objectDataList[counter].ObjectHeld;
            _objectDataList[counter].ObjectHeld = null;
            return temp;
        }
    }
}
