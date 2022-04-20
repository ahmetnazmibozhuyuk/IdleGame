using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IdleGame.Managers;

namespace IdleGame
{
    public class Generator : MonoBehaviour, IInteractable
    {
        [SerializeField] private GameObject objectToSpawn;

        [SerializeField] private List<ObjectData> objectDataList = new List<ObjectData>();

        [SerializeField] private int unlockAmount;

        private int counter;

        private float localX = 1, localY = 0, localZ = 0;

        public bool IsGiving { get; set; }

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
            for (int i = 0; i < 40; i++)
            {
                objectDataList.Add(new ObjectData(new Vector3(localX+transform.position.x,
                    localY+transform.position.y, 
                    localZ+transform.position.z)));

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
            if (counter < 40)
            {
                var temp = Instantiate(objectToSpawn);
                objectDataList[counter].ObjectHeld = temp;
                objectDataList[counter].ObjectHeld.transform.position = objectDataList[counter].ObjectPosition;
                counter++;
                yield return new WaitForSeconds(0.1f);
                StartCoroutine(SpawnObject());
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
                StartCoroutine(SpawnObject());
            }

        }
        public void TakeObject(GameObject givenObj, Transform parent)
        {
            unlockAmount--;
            Destroy(givenObj, 0.2f);
            if (unlockAmount <= 0)
            {
                IsGiving = true;
                StartCoroutine(SpawnObject());
            }

            return;
        }
        public GameObject GiveObject()
        {
            if (counter <= 0)
            {
                Debug.Log("not enough cubes");
                return null;
            }
            counter--;
            var temp = objectDataList[counter].ObjectHeld;
            objectDataList[counter].ObjectHeld = null;
            return temp;
        }

        public void Interact()
        {
            throw new System.NotImplementedException();
        }
    }
}
