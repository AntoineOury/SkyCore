using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Hopping : MonoBehaviour
{
    [SerializeField]
    private Wandering _wandering;

    [SerializeField, Tooltip("We manipulate local position of the model itself so we need a reference to just the model")]
    private GameObject _model;


    [Header("Hopping Parammeters")]

    [SerializeField, Min(0), Tooltip("Min height in Unity units of the Jelly's hop.")]
    private float _minHopHeight = 0.30f;

    [SerializeField, Min(0), Tooltip("Max height in Unity units of the Jelly's hop.")]
    private float _maxHopHeight = 0.76f;

    [SerializeField, Min(0), Tooltip("Max time (in seconds) between the Jelly's hops.")]
    private float _minTimeBetweenHops = 0.5f;

    [SerializeField, Min(0), Tooltip("Min height in Unity units of the Jelly's hop.")]
    private float _maxTimeBetweenHops = 1f;


    [SerializeField]
    private JellyStateMachine _stateMachine;
    [SerializeField]
    private GameEventScriptableObject _wanderEnter;
    [SerializeField]
    private GameEventScriptableObject _wanderUpdate;
    [SerializeField]
    private GameEventScriptableObject _wanderExit;


    private NavMeshAgent _navAgent;
    private Rigidbody _rigidbody;

    private bool _isGrounded = true;

    private float _hopTimer;
    private float _groundedTimer;

    private bool _hopRoutineIsRunning;

    private bool _isFinished = true;

    private GameEventResponses _gameEventResponses = new();

    private void Awake()
    {
        //_wandering.OnChangeDirection += SetIsFinished;
        _gameEventResponses.SetSelectiveResponses(_stateMachine,
            (_wanderEnter, WanderEnter),
            (_wanderUpdate, WanderUpdate),
            (_wanderExit, WanderExit));

        _navAgent = GetComponent<NavMeshAgent>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable() => _gameEventResponses.Register();
    private void OnDisable() => _gameEventResponses.Unregister();

    public void WanderEnter()
    {
        _isFinished = false;
    }

    public void WanderUpdate()
    {
        CheckIfGrounded();

        _hopTimer -= Time.deltaTime;
        if (!_hopRoutineIsRunning && _isGrounded && _hopTimer <= 0 && !_isFinished)
        {
            StartCoroutine(StartHop());
        }
    }

    public void WanderExit()
    {
        _isFinished = true;
    }

    private void CheckIfGrounded()
    {
        // If the jelly's vertical velocity is near zero, increment the groundedTimer.
        if (_rigidbody.velocity.y > -0.1f && _rigidbody.velocity.y < 0.1f)
        {
            _groundedTimer += Time.deltaTime;
        }
        // If the jelly's vertical velocity is moving upward, then reset the grounded timer to 0f.
        else if (_rigidbody.velocity.y >= 0.1f)
        {
            _isGrounded = false;
            _groundedTimer = 0f;
        }
        
        // If the jelly has been on the ground a short time, then set _isGrounded to true.
        if (_groundedTimer >= 0.3f)
        {
            _isGrounded = true;
        }
    }

    private void SetIsFinished(bool newValue)
    {
        Debug.Log("Set: " +  newValue);
        //_isFinished = newValue;
    }

    private IEnumerator StartHop()
    {
        _hopRoutineIsRunning = true;

        _rigidbody.isKinematic = false;

        _navAgent.enabled = false;
        _rigidbody.velocity += GetJumpVelocity();
        _isGrounded = false;


        // We put a quarter second on the timer, as this is being used to disable the code in OnCollisionEnter that detects when the jelly is back on the ground.
        // This short delay is needed, as otherwise the OnCollisionEnter() function will instantly set the _isGrounded variable to true again before the jelly
        // has a chance to jump off the ground.
        _hopTimer = 0.1f;
        while (!_isGrounded)
        {
            yield return null;
        }


        // Reset the hop timer, as it will now be used to handle the delay between hops.
        _hopTimer = Random.Range(_minTimeBetweenHops, _maxTimeBetweenHops);

        _navAgent.enabled = true;
        _rigidbody.isKinematic = true;

        _hopRoutineIsRunning = false;
    }


    private Vector3 GetJumpVelocity()
    {
        // This formula (u*u = 2gS) is calculating the velocity needed to reach the specified jump height.        
        // I derived it from this page at #2 under "Maximum height of a projectile": https://byjus.com/question-answer/how-do-you-determine-the-maximum-height-of-a-projectile/
        // NOTE: I removed v squared and added (2gH) to both sides so u squared is by itself so we can now solve for it.
        //       v squared is the final velocity, which is 0 since the jelly lands at the end of the jump.
        //       u squared is the initial velocity we're solving for.
        //       g is gravity
        //       S is the distance traveled, which is our total jump height.
        float minJumpVelocity = Mathf.Sqrt(2 * -(Physics.gravity.y) * _minHopHeight);
        float maxJumpVelocity = Mathf.Sqrt(2 * -(Physics.gravity.y) * _maxHopHeight);

        Vector3 jumpVelocity = transform.forward * _navAgent.speed;
        jumpVelocity.y = Random.Range(minJumpVelocity, maxJumpVelocity);

        //Debug.Log(jumpVelocity);

        return jumpVelocity;
    }

}

