using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class DetectGround : MonoBehaviour
{
  public bool grounded;
  void Start()
  {

  }

  void OnTriggerEnter(Collider collider)
  {
    if (collider.gameObject.CompareTag("Ground")) {
      print("grounded");
      grounded = true;
    }
  }

  void Update()
  {

  }
}
