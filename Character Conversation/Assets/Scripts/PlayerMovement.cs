using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 12f;
    public float gravity = -60f;
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;
    public bool isNearAthena = false;
    public LevelChanger levelChanger;

    Vector3 velocity;
    bool isGrounded;

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -0.5f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if(isNearAthena && Input.GetKeyDown(KeyCode.Space))
        {
            print("LoadingNewScene");
            Cursor.lockState = CursorLockMode.None;
            levelChanger.FadeToLevel(1);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Athena" || !isNearAthena)
        {
            isNearAthena = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Athena" || isNearAthena)
        {
            isNearAthena = false;
        }
    }

}
