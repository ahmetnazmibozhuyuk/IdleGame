using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace IdleGame.Managers
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI totalBoxInStore;
        [SerializeField] private TextMeshProUGUI carriedBox;

        public void SetAmountText()
        {
            totalBoxInStore.SetText("Total box = " + GameManager.instance.TotalBoxInStore.ToString());
            carriedBox.SetText("Carried box amount = " + GameManager.instance.Player.CurrentBoxCarried);
        }

    }
}
