using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
  public float speed = 0.5f;
  public float jumpForce = 0.5f;
  float horizontalMove;

  SpriteRenderer sprite;
  Rigidbody2D rB;
  Animator anim;

  PlayerInput playerInput;
  InputAction jumpAction;
  InputAction moveAction;
  InputAction attackAction;
  InputAction bumpAction;

  void Start()
  {
    sprite = GetComponent<SpriteRenderer>();
    rB = GetComponent<Rigidbody2D>();
    anim = GetComponent<Animator>();
    anim.StopPlayback();
    playerInput = GetComponent<PlayerInput>();
    jumpAction = playerInput.actions.FindAction("Jump");
    moveAction = playerInput.actions.FindAction("Move");
    attackAction = playerInput.actions.FindAction("Attack");
    bumpAction = playerInput.actions.FindAction("Bump");
  }

  void Update()
  {
    if (!anim.GetBool("Bump")){

    horizontalMovement();
    }

    Jump();

    Punch();

    Bump();

    // if (checkIfGrounded())
    // {
    //   print("grounded");
    // }

    Debug.DrawRay(transform.position, Vector2.down);
  }

  private void horizontalMovement()
  {
    // horizontalMove = Input.GetAxis("Horizontal") * 0.01f;
    horizontalMove = moveAction.ReadValue<Vector2>().x * (Time.deltaTime * speed);

    anim.SetFloat("Speed", Mathf.Abs(horizontalMove));
    anim.SetBool("Grounded", checkIfGrounded());
    // anim.ResetTrigger("Punch");

    if (Mathf.Abs(horizontalMove) != 0)
    {
      transform.position = transform.position + new Vector3(horizontalMove, 0, 0);
    }

    if (horizontalMove < 0)
    {
      sprite.flipX = true;
    }
    else if (horizontalMove > 0)
    {
      sprite.flipX = false;
    }
  }

  bool canDoubleJump = false;
  private void Jump()
  {
    if (jumpAction.WasPressedThisFrame() && checkIfGrounded())
    {
      anim.SetBool("Jump_up", true);
      StartCoroutine(IEJumpTimer());
    }
  }

  void activateJumpForce(float forceMultiplier = 100)
  {
    rB.AddForce(new Vector2(0, forceMultiplier * jumpForce));
    // Debug.Log("Jump force activated");
  }

  IEnumerator IEJumpTimer()
  {
    activateJumpForce();
    canDoubleJump = true;

    StartCoroutine(IEDoubleJumpTimer());

    yield return new WaitForSeconds(0.2f);
    anim.SetBool("Jump_up", false);

    if (checkIfGrounded())
    {
      anim.SetBool("Grounded", true);
    }
    else
    {
      yield return new WaitForSeconds(0.3f);
      anim.SetBool("Grounded", true);
    }
  }

  float doubleJumpWindowDuration = 1.3f;
  IEnumerator IEDoubleJumpTimer()
  {
    yield return null;
    for (float i = 0; i < doubleJumpWindowDuration; i += Time.deltaTime)
    {
      if (jumpAction.WasPressedThisFrame() && canDoubleJump && !checkIfGrounded())
      {
        activateJumpForce(50);
        canDoubleJump = false;
        break;
      }
      yield return null;
    }
  }

  float raycastGroundedDistance = 0.9f;
  bool checkIfGrounded()
  {
    RaycastHit2D raycastGroundedHit = Physics2D.Raycast(
      transform.position, transform.TransformDirection(Vector2.down)
      , raycastGroundedDistance
      , 1 << 6);

    return raycastGroundedHit || rB.linearVelocityY == 0;

  }

  void OnDrawGizmos()
  {
    Gizmos.color = Color.red;
    Gizmos.DrawRay(transform.position, transform.TransformDirection(Vector2.down));
  }

  private void Punch()
  {
    if (attackAction.WasPressedThisFrame())
    {
      anim.SetTrigger("Punch");
      transform.position = transform.position;
    }
  }

  private void Bump()
  {
    if (bumpAction.WasPressedThisFrame())
    {
      anim.SetBool("Bump", true);
      StartCoroutine(IEBumpTimer());
    }
  }

  public float bumpDuration = 0.25f;
  public float bumpForce = 50;
  IEnumerator IEBumpTimer()
  {

    rB.AddForce(new Vector2(sprite.flipX == true ? -bumpForce : bumpForce, 0));
    sprite.flipX = !sprite.flipX;
    yield return new WaitForSeconds(bumpDuration);
    anim.SetBool("Bump", false);
    rB.linearVelocityX = 0;
    sprite.flipX = !sprite.flipX;
  }

}
