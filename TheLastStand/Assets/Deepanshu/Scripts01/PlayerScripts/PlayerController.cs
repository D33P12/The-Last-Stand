using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    private Inputs _controls;
    private Vector2 _movement;
    
    [SerializeField] private Rigidbody _rb;
    public float moveSpeed;

    private void Awake()
    {
        _controls = new Inputs();
        
        _controls.PlayerMovement.Movement.performed += ctx => _movement = ctx.ReadValue<Vector2>();
        _controls.PlayerMovement.Movement.canceled += ctx => _movement = Vector2.zero;
    }
    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }
    private void Update()
    {
        Move();
    }
    
    private void Move()
    {
        Vector3 moveDirection = new Vector3(_movement.x, 0, _movement.y).normalized;

        if (moveDirection != Vector3.zero)
        {
            _rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, _rb.linearVelocity.y, moveDirection.z * moveSpeed);

            transform.forward = moveDirection;
        }
        else
        {
            _rb.linearVelocity = new Vector3(0, _rb.linearVelocity.y, 0);
        }
    }

}
