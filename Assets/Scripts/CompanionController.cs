using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionController : MonoBehaviour
{
    [SerializeField] GameObject targetPlayer;
    [SerializeField] float movementSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float distanceToFollow;
    [SerializeField] AnimationCurve accelerationCurve;

    Rigidbody rb;
    Vector3 movementDirection;
    float timer = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        FollowPlayer();
    }

    public void FollowPlayer()
    {
        Vector3 toPlayer = targetPlayer.transform.position - transform.position;
        movementDirection = new Vector3(toPlayer.x, 0f, toPlayer.z)/*.normalized*/ * movementSpeed;

        // Rota la cara hacia el jugador
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(movementDirection.x, 0f, movementDirection.z)), Time.deltaTime * rotationSpeed); // Mirar de quitar el delta time
        
        if (movementDirection == Vector3.zero)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            timer += Time.deltaTime;
        }

        timer = Mathf.Clamp(timer, 0, accelerationCurve.length);

        // Mueve el rigidBody hacia el jugador
        if (Vector3.Distance(targetPlayer.transform.position, transform.position) >= distanceToFollow)
        {
            rb.velocity = movementDirection * accelerationCurve.Evaluate(timer);
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }
}
