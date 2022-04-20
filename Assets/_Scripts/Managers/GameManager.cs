using UnityEngine;
using IdleGame.Control;

namespace IdleGame.Managers
{
    [RequireComponent(typeof(UIManager))]
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        public Controller Player
        {
            get { return player; }
            set { player = value; }
        }
        [SerializeField] private Controller player;


        public int TotalBoxInStore { get; private set; }

        private UIManager _uiManager;

        private void Awake()
        {
            if(instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;

            _uiManager = GetComponent<UIManager>();
        }
        private void Start()
        {
            _uiManager.SetAmountText();
        }
        public void AddBoxToStockpile()
        {
            if (Player.CurrentBoxCarried <= 0) return;
            TotalBoxInStore++;
            _uiManager.SetAmountText();
        }
        public void RemoveBoxFromStockpile()
        {
            if (TotalBoxInStore <= 0) return;
            TotalBoxInStore--;
            _uiManager.SetAmountText();
        }
        public void UpdateUI()
        {

            _uiManager.SetAmountText();
        }
    }
}
