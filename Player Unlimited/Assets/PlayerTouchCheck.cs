using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTouchCheck : MonoBehaviour {

    private enum CheckType { groundCheck, wallCheck };

    [SerializeField]
    private CheckType checkType= CheckType.groundCheck;

    private Player player;

    public bool isTouching = false;
	
	private void Start ()
    {
        player = GetComponentInParent<Player>();
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "block" || other.tag == "BlockEdge")
        {
            if (checkType == CheckType.groundCheck)
            {

            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Ground")
        {
            if (checkType == CheckType.groundCheck)
            {     
                //otherwise, you are grounded.
                player.grounded = true;
            }
            else if (checkType == CheckType.wallCheck)
            {
                /*
                if (other.GetComponent<block>() != null)
                    if (other.GetComponent<block>().canWallJump && !player.ghostActive)
                    {
                        player.touchingWall = true;
                        player.wallSlideBlock = other.gameObject;
                    }*/
            }
            isTouching = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Ground")
        {
            if (checkType == CheckType.groundCheck)
            {
                player.grounded = false;
            }
            else if (checkType == CheckType.wallCheck)
            {/*
                if (other.GetComponent<block>() != null)
                    if (other.GetComponent<block>().canWallJump)
                    {
                        player.touchingWall = false;
                        player.wallSlideBlock = null;
                    }*/
            }
            isTouching = false;
        }
    }
}
