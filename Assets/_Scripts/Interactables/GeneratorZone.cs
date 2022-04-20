using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using IdleGame.Managers;

namespace IdleGame.Interactable
{
    // Her noktadaki pozisyon vs veriler bir class veya struct ile tutulacak, sayılar yerine bu bilgiler aktarılacak.


    [RequireComponent(typeof(Collider))]
    public class GeneratorZone : MonoBehaviour
    {
        [SerializeField] private float fillRate;
        [SerializeField] private float generateRate;

        [SerializeField] private int boxCapacity;

        [SerializeField] private int unlockAmount;

        private int _depositedBoxAmount; // PlayerPrefs ile kaydet.

        private bool _isUnlocked = false;

        private int _currentBoxAmount;

        private bool _playerIsInTheZone;
        private void Start()
        {
            if (unlockAmount == 0) _isUnlocked = true;

            StartCoroutine(GenerateBox());
        }

        private void OnTriggerEnter(Collider other)
        {
            _playerIsInTheZone = true;
            if (_isUnlocked)
                StartCoroutine(GrabBox());
            else
                StartCoroutine(ConsumeBox());
        }
        private void OnTriggerExit(Collider other)
        {
            _playerIsInTheZone = false;
        }
        private IEnumerator GrabBox()
        {
            // player +1, generator -1
            while (_playerIsInTheZone)
            {
                if(_currentBoxAmount > 0)
                {
                    _currentBoxAmount--;
                    GameManager.instance.AddBoxToPlayer();
                }
                yield return new WaitForSeconds(fillRate);
            }
        }
        private IEnumerator ConsumeBox()
        {
            // player -1, generator +1
            if (!_playerIsInTheZone || _depositedBoxAmount < unlockAmount || GameManager.instance.Player.CurrentBoxCarried <= 0) yield break;

            yield return new WaitForSeconds(fillRate);
            Debug.Log("box consumed");
            _depositedBoxAmount++;
            GameManager.instance.RemoveBoxFromStockpile();
            //AddBox();
            StartCoroutine(ConsumeBox());
        }

        private IEnumerator GenerateBox()
        {
            yield return new WaitForSeconds(generateRate);
            if (!_playerIsInTheZone && _currentBoxAmount < boxCapacity)
            {
                StartCoroutine(GenerateBox());
                _currentBoxAmount++;
            }
            else
            {
                StartCoroutine(GenerateBox());
            }
        }
    }
}
