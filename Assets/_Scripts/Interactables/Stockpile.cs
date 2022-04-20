using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using IdleGame.Managers;

namespace IdleGame
{
    public class Stockpile : MonoBehaviour, IInteractable
    {
        private List<Vector3> ObjectDataList = new List<Vector3>();

        [SerializeField] private List<GameObject> heldObjectList;

        private float localX = 1, localY = 0, localZ = 0; 

        private int counter;

        public bool IsGiving { get ; set ; }

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
            for (int i = 0; i < 40; i++)
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
            if (counter < 40)
            {
                if (givenObj == null) return;
                heldObjectList.Add(givenObj);
                givenObj.transform.rotation = Quaternion.Euler(0, 0, 0);
                givenObj.transform.DOMove(ObjectDataList[counter], 0.5f);
                counter++;
                GameManager.instance.AddBoxToStockpile();
            }
        }
        public GameObject GiveObject()
        {
            if (counter <= 0)
            {
                Debug.Log("not enough cubes");
                return null;
            }
            counter--;
            var temp = heldObjectList[counter];
            heldObjectList.RemoveAt(counter);
            return temp;
        }

        public void Interact()
        {
            throw new System.NotImplementedException();
        }
    }
}
