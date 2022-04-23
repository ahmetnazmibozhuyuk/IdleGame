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

        [SerializeField] private GameObject buildingToUnlock;

        [SerializeField] private TextMeshProUGUI unlockAmountText;

        [SerializeField] private Transform objectMovePoint;
        [SerializeField] private int unlockAmount;


        private void Awake()
        {
            Type = InteractableType.Unlockable;
        }


        public GameObject GiveObject()
        {
            throw new System.NotImplementedException();
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
            if (unlockAmount <= 0)
            {
                UnlockBuilding();
            }
        }
        private void UnlockBuilding()
        {
            GameManager.instance.Unlocked();
            FullCapacity = true;
            buildingToUnlock.SetActive(true);
            buildingToUnlock.transform.DOScale(1, 0.5f).SetEase(Ease.OutBounce);
        }
    }
}
