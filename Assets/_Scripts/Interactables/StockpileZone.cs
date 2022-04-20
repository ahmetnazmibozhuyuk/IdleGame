using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IdleGame.Managers;

namespace IdleGame.Interactable
{
    [System.Serializable]
    public class CollectableData
    {
        //pozisyon (vector3), grid pozisyonu (vector3int), tutulan objenin class veya gameobject tut
        public Vector3 pos;
        public Vector3Int gridPos;
        public GameObject collectObj;

        public CollectableData(Vector3 position, Vector3Int gridPosition, GameObject collectGameObjet)
        {
            position = pos;
            gridPosition = gridPos;
            collectGameObjet = collectObj;
        }
        //Start'ta liste oluştururken diğer değerler boş olacak şekilde grid pozisyonları uygun şekilde newlensin.


    }

    [RequireComponent(typeof(Collider))]
    public class StockpileZone : MonoBehaviour
    {
        [SerializeField] int stockpileCapacity;

        [SerializeField] private float fillRate;

        private bool _playerIsInTheZone;


        private int _currentBoxIndex;

        [SerializeField] private GameObject cubesObj;
        [SerializeField] private GameObject[] objectInStockpile = new GameObject[40];


        [SerializeField] private List<CollectableData> colDatList = new List<CollectableData>();

        private int localX, localY, localZ;

        private void Start()
        {
            for(int i = 0; i < objectInStockpile.Length; i++)
            {
                objectInStockpile[i] = cubesObj.transform.GetChild(i).gameObject;           
            }

            for(int i = 0; i < objectInStockpile.Length; i++)
            {
                if(localX < 4)
                {
                    localX++;

                }
                else
                {
                    localX = 0;
                    localY++;
                }
                var temp = new CollectableData(Vector3.zero, new Vector3Int(localX, localY, localZ), null);
                colDatList.Add(temp);

            }

        }

        private void OnTriggerEnter(Collider other)
        {
            _playerIsInTheZone = true;
            StartCoroutine(StoreBox());
        }
        private void OnTriggerExit(Collider other)
        {
            _playerIsInTheZone = false;
        }
        private IEnumerator StoreBox()
        {
            if (!_playerIsInTheZone || GameManager.instance.TotalBoxInStore >= stockpileCapacity || GameManager.instance.Player.CurrentBoxCarried<=0) yield break;

            yield return new WaitForSeconds(fillRate);

            GameManager.instance.AddBoxToStockpile();
            AddBox();
            StartCoroutine(StoreBox());
        }
        private void AddBox()
        {
            if (_currentBoxIndex <= objectInStockpile.Length - 1)
            {
                objectInStockpile[_currentBoxIndex].SetActive(true);
                _currentBoxIndex++;
            }
        }
        private IEnumerator RemoveBox()
        {
            yield return new WaitForSeconds(1);
        }
    }
}
