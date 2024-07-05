using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonController : MonoBehaviour
{
    [SerializeField] private List<SlicableTest> throwablesList = new List<SlicableTest>();

    public float rotationSpeed = 1;
    public float BlastPower = 5;

    public GameObject Cannonball;
    public Transform ShotPoint;
    private void Update()
    {
        //float HorizontalRotation = Input.GetAxis("Horizontal");
        //float VericalRotation = Input.GetAxis("Vertical");

        //transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles +
        //    new Vector3(0, HorizontalRotation * rotationSpeed, VericalRotation * rotationSpeed));

        if (Input.GetKeyDown(KeyCode.Space))
        {

            throwablesList[0].TransformToShotPoint(ShotPoint);
            throwablesList[0].TurnOnMesh();
            throwablesList[0].Rb.velocity = ShotPoint.transform.up * BlastPower;


            //GameObject CreatedCannonball = Instantiate(Cannonball, ShotPoint.position, ShotPoint.rotation);
            //CreatedCannonball.GetComponent<Rigidbody>().velocity = ShotPoint.transform.up * BlastPower;
        }
    }


}
