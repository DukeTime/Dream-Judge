using DefaultNamespace;
using Map.Player;
using UnityEngine;
using Zenject;


[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rb;
    private PlayerInputHandler _input;
    private PlayerConfig _config;
    
    private float WalkSpeed => _config.walkSpeed;

    [Inject]
    public void Construct(PlayerConfig playerConfig)
    {
        _config = playerConfig;
        // _walkSpeed = playerConfig.walkSpeed;
    }
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _input = GetComponent<PlayerInputHandler>();
    }

    public void Move(bool isRunning)
    {
        float speed = isRunning ? WalkSpeed * 2 : WalkSpeed;
        
        _rb.linearVelocity = _input.MovementInput * speed;
    }

    public void Stop() => _rb.linearVelocity = Vector2.zero;
}