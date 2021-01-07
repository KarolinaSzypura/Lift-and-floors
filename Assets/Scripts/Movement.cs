using UnityEngine;

public class Movement : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private float playerSpeed = 5.0f;
    private float playerRotationSpeed = 50.0f;

    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
    }

    void Update()
    {
        Vector3 move = transform.rotation * new Vector3(0f, 0f, Input.GetAxis("Vertical"));
        Vector3 rotate = new Vector3(0, Input.GetAxis("Horizontal"), 0f) * Time.deltaTime * playerRotationSpeed;
        controller.Move(move * Time.deltaTime * playerSpeed);
        
        if (rotate != Vector3.zero)
        {
            transform.eulerAngles += rotate;
        }
        if (controller.isGrounded && playerVelocity.y < 0)
            playerVelocity.y = 0f;
        //else
        //    playerVelocity.y -= 9.81f * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
}
