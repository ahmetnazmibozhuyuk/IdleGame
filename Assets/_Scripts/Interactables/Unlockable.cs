using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using IdleGame.Managers;

namespace IdleGame.Interactable
{
    public class Unlockable : MonoBehaviour, IInteractable
    {
        public bool FullCapacity { get; set; }
        public InteractableType Type { get ; set; }

        [SerializeField] private List<GameObject> buildingToUnlock = new List<GameObject>();

        [SerializeField] private TextMeshProUGUI unlockAmountText;

        [SerializeField] private GameObject lockedText;

        [SerializeField] private Transform objectMovePoint;
        [SerializeField] private int unlockAmount;

        private void Awake()
        {
            Type = InteractableType.Unlockable;
        }
        private void Start()
        {
            unlockAmountText.SetText(unlockAmount + " more to unlock!");
        }
        public GameObject GiveObject()
        {
            Debug.Log("An object is taken from the unlockable");
            return null;
        }
        public void TakeObject(GameObject givenObj, Transform parent)
        {
            if (givenObj == null) return;
            if (FullCapacity) return;
            givenObj.transform.DORotateQuaternion(objectMovePoint.rotation, 0.5f);
            givenObj.transform.DOMove(objectMovePoint.position, 0.5f).OnComplete(() => 
            {
                givenObj.transform.localScale = new Vector3(0, 0, 0);
                ObjectPool.Despawn(givenObj);
            });
            unlockAmount--;
            unlockAmountText.SetText(unlockAmount + " more to unlock!");
            if (unlockAmount <= 0)
            {
                FullCapacity = true;
                UnlockBuilding();
            }
        }
        private void UnlockBuilding()
        {
            GameManager.instance.Unlocked();
            UnlockPart(0);
            unlockAmountText.SetText("This building is unlocked!");
            lockedText.SetActive(false);
        }
        private void UnlockPart(int index)
        {
            buildingToUnlock[index].SetActive(true);
            buildingToUnlock[index].transform.DOScale(1, 0.3f).SetEase(Ease.OutSine).OnComplete(() =>
            {
                index++;
                if(index<buildingToUnlock.Count && buildingToUnlock[index] != null)
                UnlockPart(index);
            });
        }
    }
}
