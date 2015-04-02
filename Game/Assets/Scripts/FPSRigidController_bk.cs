using UnityEngine;
using System.Collections;


public class FPSRigidController_bk : MonoBehaviour
{
    public float speed = 10.0f;
    public float gravity = 10.0f;
    public float maxVelocityChange = 10.0f;
    public bool canJump = true;
    public float jumpHeight = 2.0f;
    private bool grounded = false;
    private Rigidbody _rigidbody;
    private Player _player;




    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _player = GetComponent<Player>();

        //_rigidbody.freezeRotation = true;
        //_rigidbody.useGravity = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void FixedUpdate()
    {
        if (Input.GetButton("Cancel"))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Application.LoadLevel(Consts.MainMenuScene);
        }

        if (GetComponent<NetworkView>().isMine)
        {

            if (grounded && !_player.Stunned)
            {
                // Calculate how fast we should be moving
                Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                targetVelocity = transform.TransformDirection(targetVelocity);
                targetVelocity *= speed;

                // Apply a force that attempts to reach our target velocity
                Vector3 velocity = _rigidbody.velocity;
                if (targetVelocity != Vector3.zero)
                {
                    Vector3 velocityChange = (targetVelocity - velocity);
                    velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                    velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                    velocityChange.y = 0;
                    _rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
                }
                // Jump
                if (canJump && Input.GetButton("Jump"))
                {
                    _rigidbody.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
                }
            }
            else if (!_player.Stunned)
            {
                // Calculate how fast we should be moving
                Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                targetVelocity = transform.TransformDirection(targetVelocity);
                targetVelocity *= speed;
                targetVelocity *= 0.4f;

                // Apply a force that attempts to reach our target velocity
                Vector3 velocity = _rigidbody.velocity;
                if (targetVelocity != Vector3.zero)
                {
                    Vector3 velocityChange = (targetVelocity - velocity);
                    velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                    velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                    velocityChange.y = 0;
                    _rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
                }
            }

            // We apply gravity manually for more tuning control
            _rigidbody.AddForce(new Vector3(0, -gravity * _rigidbody.mass, 0));

            grounded = false;
        }
    }

    void OnCollisionStay()
    {
        grounded = true;
    }

    float CalculateJumpVerticalSpeed()
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }

}