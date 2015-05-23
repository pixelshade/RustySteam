using System;
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

    public AudioClip RunAudioClip;
    public AudioClip JumpAudioClip;

    private bool _jump;
    private bool _esc;
    private bool _grounded;
    private Rigidbody _rigidbody;
    private Player _player;
    private AudioSource[] _audioSources;
    private bool _dontLock;
    private NetworkView _networkView;



    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _player = GetComponent<Player>();
        _audioSources = GetComponents<AudioSource>();
        _networkView = GetComponent<NetworkView>();

        _rigidbody.freezeRotation = true;
        _rigidbody.useGravity = false;
        _rigidbody.isKinematic = false;

        _dontLock = false;
    }

    void Update()
    {
        _jump = _jump || Input.GetButtonDown("Jump");
        _esc = _esc || Input.GetButtonDown("Cancel");

        HandleSounds();
      
        
        if (!_dontLock) { 
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void HandleSounds()
    {
        if (!_networkView.isMine) return;
        if (_grounded && _jump)
        {
            if (!_audioSources[0].isPlaying)
            {
                _audioSources[0].clip = JumpAudioClip;
                _audioSources[0].Play();
            }
        }

        if (_grounded && (Math.Abs(Input.GetAxis("Vertical")) + Math.Abs(Input.GetAxis("Horizontal"))) > 0.05)
        {
            if (!_audioSources[1].isPlaying)
            {
                _audioSources[1].clip = RunAudioClip;
                _audioSources[1].loop = true;
                _audioSources[1].Play();
            }

        }
        else
        {
            if (_audioSources[1].clip == RunAudioClip && _audioSources[1].isPlaying) _audioSources[1].Stop();
        } 
    }

    void FixedUpdate()
    {
        if (_esc)
        {
           LevelController.Get().LeaveGameAndGoToMainMenu();
            _dontLock = true;
            _esc = false;
        }

        if (GetComponent<NetworkView>().isMine)
        {
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
        }
    }

    void OnCollisionStay()
    {
        _grounded = true;
    }

}
