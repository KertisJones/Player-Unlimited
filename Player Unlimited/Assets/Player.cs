using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    //Horizontal
    public enum MovementTypeH { walk, slide, NumberOfTypes }; //keep NumberOfTypes at the end
    public MovementTypeH movementTypeH;
    public float HorzForce = 0;

    public float walkSpeedMin, walkSpeedMax;
    public float slideSpeedMin, slideSpeedMax;

    public float canMoveLeftChance = 0.75f;
    public bool canMoveLeft = true;
    public float canMoveInAirChance = 0.75f;
    public bool canMoveInAir = true;

    //Vertical
    public enum MovementTypeV { jump, jetpack, NumberOfTypes }; //keep NumberOfTypes at the end
    public MovementTypeV movementTypeV;
    public float VertForce = 0;
    public float gravityMin, gravityMax;
    [HideInInspector] public bool grounded = false;
    private float jumpCooldown = 0.1f;
    [HideInInspector] public float jumpCooldownClock = 0f;

    public float jumpForceMin, jumpForceMax;
    public float jetpackForceMin, jetpackForceMax;

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
    }

    // Update is called once per frame
    void Update()
    {
        jumpCooldownClock -= Time.deltaTime;
        
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
}
