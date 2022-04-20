using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleGame.Helper
{

    public class Helper : MonoBehaviour
    {
        [SerializeField] private Transform collectTransform;
        [SerializeField] private Transform stockpileTransform;

        [SerializeField] private int carryCapacity;

        private bool canCollect;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
