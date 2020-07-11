using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    //Horizontal
    public enum MovementTypeH { walk, slide, NumberOfTypes }; //keep NumberOfTypes at the end
    public MovementTypeH movementTypeH;
    float HorzForce = 0;

    public float walkSpeedMin, walkSpeedMax;
    public float slideSpeedMin, slideSpeedMax;

    public float canMoveLeftChance = 0.75f;
    public bool canMoveLeft = true;
    public float canMoveInAirChance = 0.75f;
    public bool canMoveInAir = true;

    //Vertical
    public enum MovementTypeV { jump, jetpack, NumberOfTypes }; //keep NumberOfTypes at the end    
    public MovementTypeV movementTypeV;

    float VertForce = 0;
    public float gravityMin, gravityMax;
    [HideInInspector] public bool grounded = false;
    private float jumpCooldown = 0.1f;
    [HideInInspector] public float jumpCooldownClock = 0f;

    public float jumpForceMin, jumpForceMax;
    public float jetpackForceMin, jetpackForceMax;

    //Click Action
    public enum ActionType { shoot, teleport, NumberOfTypes }
    public ActionType mb1ActionType;
    public ActionType mb2ActionType;

    public float actionCooldownMin, actionCooldownMax;

    public float action1Cooldown;
    public float action2Cooldown;
    public float action1CooldownClock = 0;
    public float action2CooldownClock = 0;

    public float bulletSpeedMin, bulletSpeedMax;
    float bulletSpeed;
    public GameObject bullet;

    //Misc
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GenerateAttributes();        
    }

    void GenerateAttributes()
    {
        //Set movement types
        movementTypeH = (MovementTypeH)Random.Range(0, (int)MovementTypeH.NumberOfTypes);
        movementTypeV = (MovementTypeV)Random.Range(0, (int)MovementTypeV.NumberOfTypes);

        //Attributes
        canMoveLeft = Random.value < canMoveLeftChance;
        canMoveInAir = Random.value < canMoveInAirChance;

        rb.gravityScale = Random.Range(gravityMin, gravityMax);

        //Set Horizontal Movement
        if (movementTypeH == MovementTypeH.walk)
            HorzForce = Random.Range(walkSpeedMin, walkSpeedMax);
        if (movementTypeH == MovementTypeH.slide)
        {
            HorzForce = Random.Range(slideSpeedMin, slideSpeedMax);            
        }
        //HorzForce = HorzForce * rb.gravityScale;
        if (HorzForce < 3.5 || HorzForce / rb.gravityScale < 3.5) //Must be able to move in air because can't move on ground.
            canMoveInAir = true;


        //Set Vertical Movement
        if (movementTypeV == MovementTypeV.jump)
            VertForce = Random.Range(jumpForceMin, jumpForceMax);
        if (movementTypeV == MovementTypeV.jetpack)
            VertForce = Random.Range(jetpackForceMin, jetpackForceMax);
        VertForce = VertForce * rb.gravityScale;

        //Click Action Types
        mb1ActionType = (ActionType)Random.Range(0, (int)ActionType.NumberOfTypes);
        mb2ActionType = (ActionType)Random.Range(0, (int)ActionType.NumberOfTypes);

        action1Cooldown = Random.Range(actionCooldownMin, actionCooldownMax);
        action2Cooldown = Random.Range(actionCooldownMin, actionCooldownMax);

        bulletSpeed = Random.Range(bulletSpeedMin, bulletSpeedMax);
    }

    // Update is called once per frame
    void Update()
    {
        jumpCooldownClock -= Time.deltaTime;
        action1CooldownClock -= Time.deltaTime;
        action2CooldownClock -= Time.deltaTime;

        if (Input.GetButtonDown("Cancel"))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        if (Input.GetButtonDown("Submit"))
            GenerateAttributes();

        if (Input.GetAxis("Horizontal") > 0) //right
        {
            if (true)
            {
                if (movementTypeH == MovementTypeH.walk)
                    Walk(Input.GetAxis("Horizontal"));
                if (movementTypeH == MovementTypeH.slide)
                    Slide(Input.GetAxis("Horizontal"));
            }
        }
        if (Input.GetAxis("Horizontal") < 0) //left
        {
            if (canMoveLeft)
            {
                if (movementTypeH == MovementTypeH.walk)
                    Walk(Input.GetAxis("Horizontal"));
                if (movementTypeH == MovementTypeH.slide)
                    Slide(Input.GetAxis("Horizontal"));
            }                
        }
        if (Input.GetButtonDown("Jump")) //jump
        {
            if (movementTypeV == MovementTypeV.jump)
                Jump();
        }
        if (Input.GetAxis("Vertical") > 0) //up
        { 
            if (movementTypeV == MovementTypeV.jetpack)
                Jetpack(); 
        }
        if (Input.GetButtonDown("Fire1"))
        {
            if (ChooseAction(mb1ActionType, action1CooldownClock))
            {
                action1CooldownClock = action1Cooldown;
            }
        }
        if (Input.GetButtonDown("Fire2"))
        {
            if (ChooseAction(mb2ActionType, action2CooldownClock))
            {
                action2CooldownClock = action2Cooldown;
            }
        }
    }

    bool ChooseAction(ActionType action, float cooldownClock) //activates action. Returns true if it is able, and false if it is not.
    {
        if (cooldownClock > 0)
            return false;
        if (action == ActionType.shoot)
        {            
            Shoot();
        }
        else if (action == ActionType.teleport)
        {
            Teleport();
        }
        return true;
    }

    void Walk(float dir)
    {
        if (canMoveInAir || grounded)
            rb.velocity = new Vector2(HorzForce * dir, rb.velocity.y);
    }

    void Slide(float dir)
    {
        if (canMoveInAir || grounded)
            rb.AddForce(new Vector2(HorzForce * dir, 0));
    }

    void Jump()
    {
        if (grounded && jumpCooldownClock <= 0)
        {
            jumpCooldownClock = jumpCooldown;

            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0, VertForce));
        }
    }

    void Jetpack()
    {
        rb.AddForce(new Vector2(0, VertForce));
    }

    void Shoot()
    {
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int dir = 1;
        if (worldPosition.x < transform.position.x)
            dir = -1;
        GameObject newBullet = Instantiate(bullet, this.transform.position, new Quaternion());
        newBullet.GetComponent<Bullet>().speed = bulletSpeed * dir;
    }

    void Teleport()
    {
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        this.transform.position = worldPosition;
    }
}
