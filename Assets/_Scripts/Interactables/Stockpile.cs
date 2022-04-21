using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using IdleGame.Managers;

namespace IdleGame.Interactable
{
    public class Stockpile : MonoBehaviour, IInteractable
    {
        private List<Vector3> ObjectDataList = new List<Vector3>();

        [SerializeField] private List<GameObject> heldObjectList;

        [SerializeField] private int stockpileCapacity;

        private float localX = 1, localY = 0, localZ = 0; 

        private int counter;

        public bool IsGiving { get ; set ; }
        public bool FullCapacity { get; set; }

        private void Awake()
        {
            IsGiving = false;
        }
        private void Start()
        {
            InitializePositions();
        }
        private void InitializePositions()
        {
            for (int i = 0; i < stockpileCapacity; i++)
            {
                ObjectDataList.Add(new Vector3(localX+transform.position.x,
                    localY+transform.position.y, localZ+transform.position.z));

                if (localX > 2)
                {
                    localY++;
                    localX = 0;
                }
                if (localY > 1)
                {
                    localZ++;
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
                heldObjectList.Add(givenObj);
                givenObj.transform.rotation = Quaternion.Euler(0, 0, 0);
                givenObj.transform.DOMove(ObjectDataList[counter], 0.5f);
                counter++;
                GameManager.instance.AddBoxToStockpile();
            }
            else
            {
                FullCapacity = true;
                Debug.Log("Stockpile capacity is reached at "+counter+" counter index.");
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
            var temp = heldObjectList[counter];
            heldObjectList.RemoveAt(counter);
            return temp;
        }
    }
}
