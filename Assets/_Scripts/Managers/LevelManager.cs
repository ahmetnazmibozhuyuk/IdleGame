using UnityEngine;

namespace IdleGame.Managers
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private GameObject[] UnlockablesList = new GameObject[8];

        private int unlockableCounter;
        public void UnlockNext()
        {
            if (UnlockablesList[unlockableCounter] == null)
            {
                Debug.Log("Max unlock area is reached.");
                return;
            }

            UnlockablesList[unlockableCounter].SetActive(true);
            unlockableCounter++;
        }
    }
}
