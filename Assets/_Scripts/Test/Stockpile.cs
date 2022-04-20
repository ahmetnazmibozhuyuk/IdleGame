using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using IdleGame.Managers;

namespace IdleGame
{
    public class Stockpile : MonoBehaviour, IInteractable
    {
        private PlayerBackpack _leftPlaceholder;
        private Generator _reservoir;

        private List<Vector3> ObjectDataList = new List<Vector3>();

        [SerializeField] private List<GameObject> heldObjectList;

        private float localX = 6, localY = 5, localZ = 5; 

        private int counter;

        public bool IsGiving { get ; set ; }

        private void Awake()
        {
            IsGiving = false;
            _leftPlaceholder = GetComponent<PlayerBackpack>();
            _reservoir = GetComponent<Generator>();
        }
        private void Start()
        {
            InitializePositions();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                GetCubeFromReservoir();
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                GetCubeFromRightPlaceholder();
            }
        }
        private void InitializePositions()
        {
            for (int i = 0; i < 40; i++)
            {
                ObjectDataList.Add(new Vector3(localX, localY, localZ));

                if (localX > 8)
                {
                    localY++;
                    localX = 5;
                }
                if (localY > 6)
                {
                    localZ++;
                    localY = 5;
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
        private void GetCubeFromReservoir()
        {
            TakeObject(_reservoir.GiveObject(),null);
        }
        private void GetCubeFromRightPlaceholder()
        {
            if (counter >= 40)
            {
                Debug.Log("no capacity");
                return;
            }
            TakeObject(_leftPlaceholder.GiveObject(),null);
        }

        public void Interact()
        {
            throw new System.NotImplementedException();
        }
    }
}
