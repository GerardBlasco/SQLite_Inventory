using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    InputManager inputManager;
    
    [SerializeField] float horizontalSpeed;
    [SerializeField][Range(1f, 2f)] float sprintMultiplier;
    [SerializeField] float rotationSpeed;
    [SerializeField] AnimationCurve accelerationCurve;
    [SerializeField] CanvasGroup canvasGroup;

    private bool isMoving;
    Rigidbody rb;
    Vector3 movementDirection;

    float horizontalXMovement, horizontalZMovement;
    float timer = 0;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        inputManager = InputManager.instance;
    }

    void FixedUpdate()
    {
        OnMovement();
    }

    void OnMovement()
    {
        horizontalXMovement = inputManager.horizontalX_ia.ReadValue<float>();
        horizontalZMovement = inputManager.horizontalZ_ia.ReadValue<float>();

        Vector3 movementInput = new Vector3(horizontalXMovement, 0f, horizontalZMovement);
        isMoving = movementInput.sqrMagnitude > 0.01f;

        movementDirection = movementInput.normalized * horizontalSpeed;
        movementDirection.y = rb.velocity.y;

        if (isMoving) // Si la velocidad no es 0, actualiza la rotacion
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(movementDirection.x, 0f, movementDirection.z)), Time.deltaTime * rotationSpeed); // Mirar de quitar el delta time
        }
        
        if (movementDirection == Vector3.zero)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            timer += Time.deltaTime;
        }

        timer = Mathf.Clamp(timer, 0, accelerationCurve.length); 

        rb.velocity = movementDirection/* * accelerationCurve.Evaluate(timer)*/;

        // Multiplica la velocidad si el boton de sprint esta presionado
        if (inputManager.sprint_ia.IsPressed())
        {
            rb.velocity *= sprintMultiplier;
        }
    }
}
