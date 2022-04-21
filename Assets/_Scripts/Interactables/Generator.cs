using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IdleGame.Managers;
using DG.Tweening;

namespace IdleGame.Interactable
{
    public class Generator : MonoBehaviour, IInteractable
    {
        [SerializeField] private GameObject objectToSpawn;

        [SerializeField] private List<ObjectData> objectDataList = new List<ObjectData>();

        [SerializeField] private int maxCapacity;
        [SerializeField] private int unlockAmount;

        [SerializeField] private float spawnRate;


        private int counter;

        private float localX = 1, localY = 0, localZ = 0;

        public bool IsGiving { get; set; }
        public bool FullCapacity { get; set; }

        private void Awake()
        {
            if (unlockAmount > 0) IsGiving = false;
            else IsGiving = true;

            InitializePositions();
        }
        private void Start()
        {
            StartCoroutine(SpawnObject());

        }
        private void InitializePositions()
        {
            for (int i = 0; i < maxCapacity; i++)
            {
                objectDataList.Add(new ObjectData(new Vector3(localX + transform.position.x,
                    localY + transform.position.y,
                    localZ + transform.position.z)));

                if (localX > 3)
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
        private IEnumerator SpawnObject()
        {
            if (!IsGiving) yield break;
            if (counter < maxCapacity)
            {
                var temp = Instantiate(objectToSpawn);
                objectDataList[counter].ObjectHeld = temp;
                objectDataList[counter].ObjectHeld.transform.position = objectDataList[counter].ObjectPosition;
                counter++;
                yield return new WaitForSeconds(spawnRate);
                StartCoroutine(SpawnObject());
            }
            else
            {
                yield return new WaitForSeconds(spawnRate);
                StartCoroutine(SpawnObject());
            }

        }

        public void TakeObject(GameObject givenObj, Transform parent)
        {
            if (IsGiving) return;
            if (givenObj == null) return;
            //heldObjectList.Add(givenObj);
            givenObj.transform.rotation = Quaternion.Euler(0, 0, 0);
            givenObj.transform.DOMove(transform.position, 0.5f);
            unlockAmount--;
            Destroy(givenObj, 0.6f);
            if (unlockAmount <= 0)
            {
                IsGiving = true;
                GameManager.instance.Unlocked();
                StartCoroutine(SpawnObject());
            }
        }

        public GameObject GiveObject()
        {
            if (counter <= 0)
            {
                //Debug.Log("not enough cubes");
                return null;
            }
            counter--;
            var temp = objectDataList[counter].ObjectHeld;
            objectDataList[counter].ObjectHeld = null;
            return temp;
        }

    }
}
