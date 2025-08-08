using System;
using DefaultNamespace;
using Map.Player;
using UnityEngine;
using Zenject;


[System.Serializable]
public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MovementInput { get; private set; }
    public bool IsRunning { get; private set; }
    public bool InteractPressed { get; private set; }

    
    
    private GlobalConfig _config;
    private bool TestMode => _config.TestMode;

    [Inject]
    public void Construct(GlobalConfig globalConfig)
    {
        // _testMode = globalConfig.TestMode;
        _config = globalConfig;
    }

    private void Start()
    {
    }

    private void Update()
    {
        MovementInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        IsRunning = Input.GetKey(KeyCode.LeftShift) && TestMode;
        InteractPressed = Input.GetKeyDown(KeyCode.E);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        GameObject go = other.gameObject;
        switch (go.tag)
        {
            case "NPC":
                
                break;
        }
    }
}