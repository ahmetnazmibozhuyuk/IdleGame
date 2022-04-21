using UnityEngine;
namespace IdleGame.Control
{
    [RequireComponent(typeof(Rigidbody), typeof(Animator))]
    public class Controller : MonoBehaviour
    {
        [SerializeField] private float maxSpeed;
        [SerializeField] private float turnRate;

        private Rigidbody _rigidbody;
        private Animator _animator;

        private Vector3 _hitDownPosition;
        private Vector3 _offset;
        private Vector3 _offsetOnXZ;
        private Vector3 _rotateVector;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
        }
        private void Update()
        {
            SetControl();
        }
        private void FixedUpdate()
        {
            AssignMovement();
        }
        #region Movement Controls
        private void SetControl()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _hitDownPosition = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0))
            {
                _offset = (Input.mousePosition - _hitDownPosition).normalized;
                _offsetOnXZ = new Vector3(_offset.x, _offset.z, _offset.y);

                _animator.SetBool("IsMoving", true);

                if (_offsetOnXZ != Vector3.zero)
                    _rotateVector = _offsetOnXZ;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _offset = Vector3.zero;
                _offsetOnXZ = Vector3.zero;
                _animator.SetBool("IsMoving", false);
            }
        }
        private void AssignMovement()
        {
            _rigidbody.MovePosition(transform.position + maxSpeed * Time.deltaTime * _offsetOnXZ);
            if (_rotateVector != Vector3.zero)
                _rigidbody.MoveRotation(Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(_rotateVector), Time.deltaTime * turnRate * 100));
        }
        #endregion
    }
}

