using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private CharacterController _controller;
    [SerializeField]
    private float _moveSpeed = 5f;
    [SerializeField]
    private float _shootDelay = 2f;
    [SerializeField]
    private float _jumpSpeed = 3.5f;
    [SerializeField]
    private float _gravity =-9.81f;
    [SerializeField]
    private float _doubleJumpMultiply = 0.5f;
    [SerializeField]
    private Transform debugHitPointTrasform;
    [SerializeField]
    private LayerMask hookShotMask;
    [SerializeField]
    private GameObject weapon;
    [SerializeField]
    private GameObject shootEffect;
    [SerializeField]
    private GameObject playerBody;
    [SerializeField]
    private GameObject playerSpwnPoint;
    [SerializeField]
    private GameObject ui;
    [SerializeField]
    private float dashCooldown;
    [SerializeField]
    private float hookShootCooldown;

    public Transform groudCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    bool isGrounded;
    public float dashDuration;
    public float dashSpeed;
    public Rigidbody rb;
    
    private bool isDash=false;
    private bool isHookshooted = false;
    private Camera cam;
    private bool _canDoubleJump=false;
    private PlayerState state;
    private Vector3 hookShotPosition;
    private float shootStart;
    private bool isShoot;
    private float shootClock;
    private float dashStart;
    private float dashClock;
    private float hookShotClock;
    private GUIHandler uiHandler;
    


    //public GameObject[] debugEnemies;

    Vector3 velocity;
  

    // Start is called before the first frame update
    void Start()
    {
        _controller = this.GetComponent<CharacterController>();
        cam = transform.Find("Camera").GetComponent<Camera>();
        uiHandler = ui.GetComponent<GUIHandler>();
        state = PlayerState.Normal;
       
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            default:
            case PlayerState.Normal:
                handleShootClock();
                handleDashClock();
                handlehookShotClock();
                handlePLayerMovement();
                handleHookshotStart();
                handleshoot();    
                break;

            case PlayerState.HookShotFlyingPLayer:
                handleShootClock();
                handleDashClock();
                handleHookshotMovement();
                handleshoot();
                break;
            case PlayerState.Climb:
                handleShootClock();
                handleDashClock();
                handlehookShotClock();
                handleClimbMovement();
                break;
            case PlayerState.Dash:
                handlePlayerDash();
                handlehookShotClock();
                break;
        }
      
        
    }

    private void handlePlayerDash()
    {
        //Debug.Log(this.name + " handlePlayerDash");
        if (Time.time < dashStart + dashDuration)
        {
            _controller.Move(cam.transform.forward * dashSpeed * Time.deltaTime);
        }
        else
        {
           
            state = PlayerState.Normal;
            isDash = true;
            dashClock = dashCooldown;
        }   
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triger Enter");
        if (other.CompareTag("DeadGround"))
        {
             _controller.enabled = false;
             transform.position = playerSpwnPoint.transform.position;
             _controller.enabled = true;
             Debug.Log("Spawn");
        }
        if (other.CompareTag("HighJump"))
        {
            Debug.Log("HighJump Juhu");
            velocity.y = Mathf.Sqrt(_jumpSpeed*8f* -2f * _gravity);
            rb.useGravity = false;

        }
    }
    private void handleShootClock()
    {
        if (shootClock != -100)
        {
            if (shootClock > 0)
            {
                shootClock -= Time.deltaTime;
            }
            else
            {
                shootClock = -100;
                isShoot = false;
            }
        }
    }

    private void handleDashClock()
    {
        if (dashClock != -100)
        {
            if (dashClock > 0)
            {
                float amount = dashClock / dashCooldown;
                uiHandler.updatedashBlur(amount);
                dashClock -= Time.deltaTime;
            }
            else
            {
                uiHandler.updatedashBlur(0);
                dashClock = -100;
                isDash = false;
            }
        }
    }

    private void handlehookShotClock()
    {
        if (hookShotClock != -100)
        {
            if (hookShotClock > 0)
            {
                float amount = hookShotClock / hookShootCooldown;
                uiHandler.updateGrapplinghookBlur(amount);
                hookShotClock -= Time.deltaTime;
            }
            else
            {
                uiHandler.updateGrapplinghookBlur(0);
                hookShotClock = -100;
                isHookshooted = false;
            }
        }
    }

    private void handleshoot()
    {
        Debug.Log(this.name + " handleshoot");
        if (Input.GetMouseButtonDown(0))
        {
            if (!isShoot)
            {
                shootEffect.GetComponent<ParticleSystem>().Play();
                if (Physics.Raycast(weapon.transform.position, weapon.transform.forward, out RaycastHit raycastHit))
                {
                    Debug.Log(raycastHit.collider.name);
                    if (raycastHit.collider.gameObject.CompareTag("Enemy"))
                    {
                        Debug.Log("EnemyHit");
                        raycastHit.collider.gameObject.GetComponent<MutantSkalaton>().getScript().getHit(20);
                    }
                }
                isShoot = true;
                shootClock = _shootDelay;
            }
        }
    }

    private void handlePLayerMovement()
    {
        Debug.Log(this.name + " handlePlayerMovement");
        isGrounded = Physics.CheckSphere(groudCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
           // Debug.Log("isGrounded");
            velocity.y = -2f;
            rb.useGravity = true;
            if (!_controller.enabled && !isDash)
            {
                _controller.enabled = true;
            }
        }

        float horizonatlInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");



        Vector3 move = transform.right * horizonatlInput + transform.forward * verticalInput;

        _controller.Move(move * _moveSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Debug.Log("Jump");
            velocity.y = Mathf.Sqrt(_jumpSpeed * -2f * _gravity);
            _canDoubleJump = true;
            rb.useGravity = false;
        }
        else
        {
            if (Input.GetButtonDown("Jump") && _canDoubleJump)
            {
                velocity.y = Mathf.Sqrt(_jumpSpeed * _doubleJumpMultiply * -2f * _gravity);
                _canDoubleJump = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!isDash)
            {
                Debug.Log("Dash");
                dashStart = Time.time;
                state = PlayerState.Dash;
                uiHandler.updatedashBlur(1);
            }
           

        }


         velocity.y += _gravity * Time.deltaTime;
        _controller.Move(velocity * Time.deltaTime);

        
    }

    private void handleHookshotStart()
    {
        if (!isHookshooted)
        {
            Debug.Log(this.name + " handleHookshotStart");
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit raycastHit))
                {
                    if (raycastHit.collider.gameObject.layer == 8 || raycastHit.collider.gameObject.layer == 10)
                    {
                        //hit something
                        debugHitPointTrasform.position = raycastHit.point;
                        hookShotPosition = raycastHit.point;
                        state = PlayerState.HookShotFlyingPLayer;
                    }

                }
            }
        }
    }

    private void handleHookshotMovement()
    {

        float hookshotMinSpeed = 10f;
        float hookshotMaxSpeed = 40f;
        Vector3 hookshotDir = (hookShotPosition - transform.position).normalized;
        float hookshotSpeed =Mathf.Clamp( Vector3.Distance(transform.position, hookShotPosition),hookshotMinSpeed,hookshotMaxSpeed);
        float hookSpeedMultiplier = 2f;

        //Move Character Controller
        _controller.Move(hookshotDir * hookshotSpeed*hookSpeedMultiplier* Time.deltaTime);

        float reachedHookShotpositionDistance = 1.2f;

        if (Vector3.Distance(transform.position, hookShotPosition) < reachedHookShotpositionDistance)
        {
            //velocity.y = transform.position.y;
            //Debug.Log("reachedHookshotPosition");
            //reached Hookshot Position
            state = PlayerState.Normal;
            resetGravityEffect();
            isHookshooted = true;
            hookShotClock = hookShootCooldown;
            uiHandler.updateGrapplinghookBlur(1);
        }

        if (testInputDownHookshot())
        {
            state = PlayerState.Normal;
            resetGravityEffect();
            isHookshooted = true;
            hookShotClock = hookShootCooldown;
            uiHandler.updateGrapplinghookBlur(1);
        }

        if (testInputJump())
        {
            state = PlayerState.Normal;
            //resetGravityEffect();
            float hookshotJumpSpeed = 5f;
             velocity.y = Mathf.Sqrt(hookshotJumpSpeed* -2f * _gravity);
            _canDoubleJump = true;
            rb.useGravity = false;

            isHookshooted = true;
            hookShotClock = hookShootCooldown;
            uiHandler.updateGrapplinghookBlur(1);
        }

    }


    private void handleClimbMovement()
    {
        float horizonatlInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");



        Vector3 move = transform.right * horizonatlInput + transform.up * verticalInput;

        _controller.Move(move * _moveSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump"))
        {
            Debug.Log("ClimbJump");
            //velocity.y = Mathf.Sqrt(_jumpSpeed * -2f * _gravity);
            _controller.enabled = false;
            rb.AddForce(Camera.main.transform.forward * _jumpSpeed*3f, ForceMode.VelocityChange);

        }
    }

    private void resetGravityEffect()
    {
        velocity.y = -2f;
    }

    private bool testInputDownHookshot()
    {
        return Input.GetKeyDown(KeyCode.E);
    }
   

    private bool testInputJump()
    {
        return Input.GetButtonDown("Jump");
    }

    public PlayerState getCurrentPlayerState()
    {
        return state;
    }

    public void setCurrentPlayerState(PlayerState pState)
    {
        state = pState;
    }
    public void disableControllerGravity()
    {
        _gravity = 0f;
    }

    public void enableControllerGravity()
    {
        _gravity = -9.81f;
    }
}
