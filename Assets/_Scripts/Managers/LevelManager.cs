using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleGame.Managers
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private GameObject[] UnlockablesList = new GameObject[8];

        private int unlockableCounter;
        public void UnlockNext()
        {
            if (UnlockablesList[unlockableCounter] == null) return;
            UnlockablesList[unlockableCounter].SetActive(true);
            unlockableCounter++;
        }

    }
}
