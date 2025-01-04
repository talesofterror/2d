using NUnit.Framework.Constraints;
using UnityEngine;

public class BabaMovement : MonoBehaviour
{
  Animator anim;
  SpriteRenderer sprite;
  Vector2 front;
  Vector2 back;
  bool walking = false;
  bool dancing = true;

  public float raycastCheckFrontDistance = 3;
  public float raycastCheckBackDistance = 1;
  public float raycastAlertVisionDistance = 10;

  void Start()
  {
    anim = GetComponent<Animator>();
    sprite = GetComponent<SpriteRenderer>();
  }

  bool wasAlertedFront, wasAlertedBack;
  void Update()
  {
    CalculateFrontAndBack();
    
    if (CheckPlayerDetected()) {
      AlertedBehavior();

    } else {
      IdleBehavior();
    }
  }

  Vector3 alertTargetHeading;
  bool isFacingPlayer;
  void AlertedBehavior () 
  {
    isFacingPlayer = wasAlertedFront;

    if (!isFacingPlayer) {
      sprite.flipX = !sprite.flipX;
    } 

    // check if Player visible
    RaycastHit2D raycastAlertVisionHit = Physics2D.Raycast(
      transform.position, transform.TransformDirection(front)
      , raycastAlertVisionDistance
      , 1 << 3);
    Vector3 raycastHitPoint = new Vector3(raycastAlertVisionHit.point.x, raycastAlertVisionHit.point.y, 0);

    alertTargetHeading = raycastHitPoint - transform.position;

    print("Baba heading normalized: " + alertTargetHeading.normalized);

    transform.position = transform.position + transform.TransformDirection(front) * 0.002f;

    anim.SetBool("Walking", true);

    print("Baba detected the Player!");
  }

  void IdleBehavior() 
  {
    anim.SetBool("Walking", false);
  }

  bool CheckPlayerDetected()
  {
    // check front
    RaycastHit2D raycastFrontHit = Physics2D.Raycast(
      transform.position, transform.TransformDirection(front)
      , raycastCheckFrontDistance
      , 1 << 3);

    // check back
    RaycastHit2D raycastBackHit = Physics2D.Raycast(
      transform.position, transform.TransformDirection(back)
      , raycastCheckBackDistance
      , 1 << 3);
    
    wasAlertedFront = raycastFrontHit;
    wasAlertedBack = raycastBackHit;

    return raycastFrontHit || raycastBackHit;
  }

  void OnDrawGizmos()
  {
    Gizmos.color = Color.blue;
    Gizmos.DrawRay(
      transform.position
      , transform.TransformDirection(front) * raycastCheckFrontDistance);
    Gizmos.color = Color.cyan;
    Gizmos.DrawRay(
      transform.position
      , transform.TransformDirection(back) * raycastCheckBackDistance);
  }
  
  private void CalculateFrontAndBack()
  {
    if (sprite.flipX == false)
    {
      front = Vector2.left;
      back = Vector2.right;
    }
    else
    {
      front = Vector2.right;
      back = Vector2.left;
    }
  }
}


