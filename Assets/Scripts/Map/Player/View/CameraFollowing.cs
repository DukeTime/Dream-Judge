using DefaultNamespace;
using UnityEngine;
using Zenject;


namespace Map.Player.View
{
    public class CameraFollowing : MonoBehaviour
    {
        private PlayerConfig _playerConfig;
        
        private Transform _target;
        private float _followSpeed => _playerConfig.followSpeed;
        private float _deadZoneRadius => _playerConfig.deadZoneRadius;
        private bool _useLerp => _playerConfig.useLerp;
        private Vector3 _offset;

        [Inject]
        public void Construct(PlayerConfig playerConfig, PlayerController playerController)
        {
            _playerConfig = playerConfig;
            
            _target = playerController.gameObject.transform;
            // _followSpeed = playerConfig.followSpeed;
            // _deadZoneRadius = playerConfig.deadZoneRadius;
            // _useLerp = playerConfig.useLerp;
        }
        
        private void Start()
        {
            _offset = transform.position - _target.position;
        }

        private void LateUpdate()
        {
            Vector3 targetPosition = _target.position + _offset;
            Vector3 direction = targetPosition - transform.position;
            float distance = direction.magnitude;

            // Если цель вне мертвой зоны
            if (distance > _deadZoneRadius)
            {
                if (_useLerp)
                {
                    // Плавное приближение с Lerp
                    transform.position = Vector3.Lerp(
                        transform.position,
                        targetPosition,
                        _followSpeed * Time.deltaTime
                    );
                }
                else
                {
                    // Плавное движение с SmoothDamp (более естественное ускорение/торможение)
                    transform.position = Vector3.MoveTowards(
                        transform.position,
                        targetPosition,
                        _followSpeed * Time.deltaTime
                    );
                }
            }
        }
    }
}