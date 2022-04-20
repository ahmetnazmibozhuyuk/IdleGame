using UnityEngine;
using IdleGame.Managers;

namespace IdleGame.Control
{
    public class CamFollow : MonoBehaviour
    {
        private void Update()
        {
            transform.position = GameManager.instance.Player.transform.position;
        }
    }
}
