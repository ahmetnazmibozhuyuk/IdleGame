using System.Collections;
using UnityEngine;
using IdleGame.Interactable;

namespace IdleGame.Helper
{
    [RequireComponent(typeof(Rigidbody),typeof(Animator),typeof(PlayerBackpack))]
    public class Helper : MonoBehaviour
    {
        [SerializeField] private Transform collectTransform;
        [SerializeField] private Transform stockpileTransform;

        [SerializeField] private float speed = 1;
        [SerializeField] private float turnRate = 1;

        [SerializeField] private int carryCapacity;

        private bool _canCollect;

        private Transform _currentTarget;

        private Vector3 _direction;
        private Vector3 _turnDirection;

        private Rigidbody _rigidbody;
        private Animator _animator;
        private PlayerBackpack _helperBackpack;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
            _helperBackpack = GetComponent<PlayerBackpack>();
        }
        private void OnEnable()
        {
            StartCoroutine(Co_TargetSwitcher());
        }
        private void Update()
        {
            _direction = (_currentTarget.position - transform.position).normalized;
        }
        private void FixedUpdate()
        {

            AssignMovement();

        }
        private void AssignMovement()
        {
            _turnDirection = new Vector3(_direction.x, 0, _direction.z);
            _rigidbody.MovePosition(transform.position + (_direction * speed * Time.deltaTime));
            if (_turnDirection != Vector3.zero)
                _rigidbody.MoveRotation(Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(_turnDirection), Time.deltaTime * turnRate * 100));
        }
        private void MoveToStockpile()
        {

            if (_helperBackpack.FullCapacity)
            {
                _canCollect = false;
            }
        }
        private void MoveToGenerator()
        {

        }
        private IEnumerator Co_TargetSwitcher()
        {
            _currentTarget = collectTransform;
            yield return new WaitForSeconds(6);
            _currentTarget = stockpileTransform;
            yield return new WaitForSeconds(6);
            StartCoroutine(Co_TargetSwitcher());
        }

    }
}
