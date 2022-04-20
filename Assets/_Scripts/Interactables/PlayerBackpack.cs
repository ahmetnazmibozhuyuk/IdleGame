using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IdleGame.Managers;
using DG.Tweening;

namespace IdleGame
{
    public class PlayerBackpack : MonoBehaviour, IInteractable
    {
        [SerializeField] private List<ObjectData> objectDataList = new List<ObjectData>();

        [SerializeField] private GameObject backpack;

        private float localX = 1, localY, localZ;
        private int counter;

        private bool inTheZone;

        public bool IsGiving { get ; set ; }

        private void Start()
        {
            InitializePositions();
        }
        private void InitializePositions()
        {
            for (int i = 0; i < 40; i++)
            {
                objectDataList.Add(new ObjectData(new Vector3(localX, localY, localZ)));
                //if (localX > 3)
                //{
                //    localY++;
                //    localX = 0;
                //}
                //if (localY > 1)
                //{
                //    localZ++;
                //    localY = 0;
                //}
                localY++;
            }
        }
        public void TakeObject(GameObject givenObj, Transform parent)
        {
            if (counter < 40)
            {
                if (givenObj == null) return;

                objectDataList[counter].ObjectHeld = givenObj;
                givenObj.transform.rotation = transform.rotation;
                givenObj.transform.SetParent(transform);
                int temp = counter;
                givenObj.transform.DOMove(new Vector3(backpack.transform.position.x,
                    backpack.transform.position.y + objectDataList[counter].ObjectPosition.y,
                    backpack.transform.position.z), 0.15f);
                StartCoroutine(Co_CorrectCubePosition(givenObj, temp));
                counter++;
                GameManager.instance.UpdateUI();
            }
        }
        private IEnumerator Co_CorrectCubePosition(GameObject go, int position)
        {
            yield return new WaitForSeconds(0.15f);
            go.transform.position = new Vector3(backpack.transform.position.x,
                    backpack.transform.position.y + objectDataList[position].ObjectPosition.y,
                    backpack.transform.position.z);
        }

        public GameObject GiveObject()
        {
            if (counter <= 0)
            {
                Debug.Log("not enough cubes from left");
                return null;
            }
            counter--;
            var temp = objectDataList[counter].ObjectHeld;
            temp.transform.SetParent(null);
            objectDataList[counter].ObjectHeld = null;
            GameManager.instance.UpdateUI();
            return temp;

        }
        //private void GetCubeFromReservoir()
        //{
        //    TakeObject(_reservoir.GiveObject(), GameManager.instance.Player.transform);
        //}
        //private void GetCubeFromRightPlaceholder()
        //{
        //    TakeObject(_rightPlaceholder.GiveObject(), GameManager.instance.Player.transform);
        //}
        private void OnTriggerEnter(Collider other)
        {

            if (other.GetComponent<IInteractable>() == null) return;


            if (inTheZone) return;
            var interactable = other.GetComponent<IInteractable>();
            inTheZone = true;
            Debug.Log("trigger enter "+inTheZone);
            if (interactable.IsGiving)
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
            if (inTheZone)
            {
                TakeObject(interactable.GiveObject(), transform);

                yield return new WaitForSeconds(0.3f);
                StartCoroutine(Co_GetCubeFrom(interactable));
            }
            else
            {
                Debug.Log("left the zone");
                yield break;
            }
        }
        private IEnumerator Co_SendCubeTo(IInteractable interactable)
        {
            if (inTheZone)
            {
                interactable.TakeObject(GiveObject(),null);

                yield return new WaitForSeconds(0.3f);
                StartCoroutine(Co_SendCubeTo(interactable));
            }
            else
            {
                Debug.Log("left the zone");
                yield break;
            }
        }

        public void Interact()
        {
            throw new System.NotImplementedException();
        }
    }

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

        public bool IsGiving { get; set; }
        public void Interact();
    }
}
