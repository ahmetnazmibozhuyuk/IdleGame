using UnityEngine;
using IdleGame.Control;
using IdleGame.Interactable;

namespace IdleGame.Managers
{
    [RequireComponent(typeof(UIManager),typeof(LevelManager))]
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        public Controller Player
        {
            get { return player; }
            set { player = value; }
        }
        [SerializeField] private Controller player;

        public Stockpile StockpileInstance
        {
            get { return stockpileInstance; }
            set { stockpileInstance = value; }
        }
        [SerializeField] private Stockpile stockpileInstance;

        public int TotalBoxInStore { get; private set; }

        public int TotalBoxInBackpack { get; private set; }

        private UIManager _uiManager;
        private LevelManager _levelManager;

        private void Awake()
        {
            if(instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;

            _uiManager = GetComponent<UIManager>();
            _levelManager = GetComponent<LevelManager>();
        }
        private void Start()
        {
            _uiManager.SetAmountText();
        }
        public void AddBoxToStockpile()
        {
            TotalBoxInStore++;
            _uiManager.SetAmountText();
        }
        public void AddBoxToBackpack()
        {
            TotalBoxInBackpack++;
            _uiManager.SetAmountText();
        }
        public void RemoveBoxFromBackpack()
        {
            TotalBoxInBackpack--;
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

        public void Unlocked()
        {
            _levelManager.UnlockNext();
        }
    }
}
