using UnityEngine;
using System.Collections;


public class FPSRigidController : MonoBehaviour
{
    public Transform LookTransform;
    public Vector3 Gravity = Vector3.down * 9.81f;
    public float RotationRate = 0.1f;
    public float Velocity = 8;
    public float GroundControl = 1.0f;
    public float AirControl = 0.2f;
    public float JumpVelocity = 5;
    public float GroundHeight = 1.1f;
    private bool _jump;
    private bool _esc;
    private bool _grounded;
    private Rigidbody _rigidbody;
    private Player _player;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _player = GetComponent<Player>();

        _rigidbody.freezeRotation = true;
        _rigidbody.useGravity = false;
        _rigidbody.isKinematic = false;
        

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        _jump = _jump || Input.GetButtonDown("Jump");
        _esc = _esc || Input.GetButtonDown("Cancel");
    }

    void FixedUpdate()
    {
        if (_esc)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (Network.isServer || Network.isClient)
            {
                if (Network.isServer) MasterServer.UnregisterHost();
                Network.Disconnect();
                Application.LoadLevel(Consts.MainMenuScene);
            }
            _esc = false;
        }

        //if (GetComponent<NetworkView>().isMine)
        //{
            if (!_player.Stunned && !_player.Dead) { 
            // Cast a ray towards the ground to see if the Walker is grounded
                //bool grounded = Physics.Raycast(transform.position, Gravity.normalized, GroundHeight);

                // Rotate the body to stay upright
                Vector3 gravityForward = Vector3.Cross(Gravity, transform.right);
                Quaternion targetRotation = Quaternion.LookRotation(gravityForward, -Gravity);
                _rigidbody.rotation = Quaternion.Lerp(_rigidbody.rotation, targetRotation, RotationRate);

                // Add velocity change for movement on the local horizontal plane
                Vector3 forward = Vector3.Cross(transform.up, -LookTransform.right).normalized;
                Vector3 right = Vector3.Cross(transform.up, LookTransform.forward).normalized;
                Vector3 targetVelocity = (forward * Input.GetAxis("Vertical") + right * Input.GetAxis("Horizontal")) * Velocity;
                Vector3 localVelocity = transform.InverseTransformDirection(_rigidbody.velocity);
                Vector3 velocityChange = transform.InverseTransformDirection(targetVelocity) - localVelocity;

                // The velocity change is clamped to the control velocity
                // The vertical component is either removed or set to result in the absolute jump velocity
                velocityChange = Vector3.ClampMagnitude(velocityChange, _grounded ? GroundControl : AirControl);
                velocityChange.y = _jump && _grounded ? -localVelocity.y + JumpVelocity : 0;
                velocityChange = transform.TransformDirection(velocityChange);
                _rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

                // Add gravity
                _rigidbody.AddForce(Gravity * _rigidbody.mass);

                _jump = false;
                _grounded = false;
            }
        //}
    }

    void OnCollisionStay()
    {
        _grounded = true;
    }

}
