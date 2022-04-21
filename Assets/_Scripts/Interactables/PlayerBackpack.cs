using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IdleGame.Managers;
using DG.Tweening;

namespace IdleGame.Interactable
{

    //STOCKPILE DOLUYKEN EŞYA BOŞALTMAYA MÜSADE ETME -son eşyayı boşluğa bırakıyor-
    //GENERATORLER UNLOCK OLMAK İÇİN SON KÜBÜN GELMESİNİ BEKLESİN
    //küpleri poolla
    //ui düzenle
    public class PlayerBackpack : MonoBehaviour, IInteractable
    {
        [SerializeField] private List<ObjectData> objectDataList = new List<ObjectData>();

        [SerializeField] private GameObject backpack;

        private float localY = 1;
        private int counter;

        private bool inTheZone;

        public bool IsGiving { get; set; }
        public bool FullCapacity { get; set; }

        public int BackpackCapacity;


        private void Start()
        {
            InitializePositions();
        }
        private void InitializePositions()
        {
            for (int i = 0; i < BackpackCapacity; i++)
            {
                objectDataList.Add(new ObjectData(new Vector3(0, localY, 0)));
                localY++;
            }
        }
        public void TakeObject(GameObject givenObj, Transform parent)
        {
            if (counter < BackpackCapacity)
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
                if(counter>=BackpackCapacity) FullCapacity = true;
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
                return GameManager.instance.StockpileInstance.GiveObject();
            }

            FullCapacity = false;
            counter--;
            var temp = objectDataList[counter].ObjectHeld;
            temp.transform.SetParent(null);
            objectDataList[counter].ObjectHeld = null;
            GameManager.instance.UpdateUI();
            return temp;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (inTheZone) return;
            if (other.GetComponent<IInteractable>() == null) return;


            var interactable = other.GetComponent<IInteractable>();
            inTheZone = true;
            //Debug.Log("trigger enter "+inTheZone);
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
            if (inTheZone && !FullCapacity)
            {
                TakeObject(interactable.GiveObject(), transform);

                yield return new WaitForSeconds(0.3f);
                StartCoroutine(Co_GetCubeFrom(interactable));
            }
            else
            {

                yield break;

            }
        }
        private IEnumerator Co_SendCubeTo(IInteractable interactable)
        {
            if (interactable.IsGiving == true || interactable.FullCapacity) yield break;
            if (inTheZone)
            {
                interactable.TakeObject(GiveObject(), null);

                yield return new WaitForSeconds(0.3f);
                StartCoroutine(Co_SendCubeTo(interactable));
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
        public bool IsGiving { get; set; }
        public bool FullCapacity { get; set; }

    }
}