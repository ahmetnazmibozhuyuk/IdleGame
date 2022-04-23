using UnityEngine;
using IdleGame.Interactable;

namespace IdleGame.Helper
{
    [RequireComponent(typeof(Rigidbody), typeof(Animator), typeof(Backpack))]
    public class Helper : MonoBehaviour
    {
        [SerializeField] private Transform collectTransform;
        [SerializeField] private Transform stockpileTransform;

        [SerializeField] private float speed = 1;
        [SerializeField] private float turnRate = 1;

        [SerializeField] private int carryCapacity;

        private Transform _currentTarget;

        private Vector3 _direction;
        private Vector3 _turnDirection;

        private Rigidbody _rigidbody;
        private Animator _animator;
        private Backpack _helperBackpack;

        private float _timer = 1;
        private float _elapsedTime;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
            _helperBackpack = GetComponent<Backpack>();
        }
        private void OnEnable()
        {
            _currentTarget = collectTransform;
        }
        private void Update()
        {
            _direction = (_currentTarget.position - transform.position).normalized;

            AssignJob();
        }
        private void FixedUpdate()
        {
            AssignMovement();
        }
        private void AssignMovement()
        {
            _turnDirection = new Vector3(_direction.x, 0, _direction.z);
            _rigidbody.MovePosition(transform.position + (speed * Time.deltaTime * _direction));
            if (Vector2.Distance(transform.position, _currentTarget.position) > 0.2f)
                _animator.SetBool("IsMoving", true);
            else
                _animator.SetBool("IsMoving", false);
            if (_turnDirection != Vector3.zero)
                _rigidbody.MoveRotation(Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(_turnDirection), Time.deltaTime * turnRate * 100));
        }
        private void AssignJob()
        {
            _elapsedTime += Time.deltaTime;
            if (_elapsedTime > _timer)
            {
                _elapsedTime = 0;
                MoveToGenerator();
                MoveToStockpile();
            }
        }

        private void MoveToStockpile()
        {

            if (_helperBackpack.FullCapacity)
            {
                _currentTarget = stockpileTransform;
            }
        }
        private void MoveToGenerator()
        {
            if (_helperBackpack.Counter <= 0)
            {
                _currentTarget = collectTransform;
            }
        }


    }
}
