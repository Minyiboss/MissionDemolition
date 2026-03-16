using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    [Header("Inscribed")]
    public GameObject[] projectilePrefabs;
    public float velocityMult = 10f;
    public GameObject projLinePrefab;
    public Transform leftPoint;
    public Transform rightPoint;
    public LineRenderer leftLine;
    public LineRenderer rightLine;
    public AudioSource launchSound;


    [Header("Dynamic")]
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;

    public GameObject launchPoint;
    void OnMouseEnter()
    {
        //print("Slingshot:OnMouseEnter()");
        launchPoint.SetActive(true);
    }

    void OnMouseExit()
    {
        //print("Slingshot:OnMouseExit()");
        launchPoint.SetActive(false);
    }
    void Update()
    {
        if (!aimingMode) return;
        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);
        Vector3 mouseDelta = mousePos3D - launchPos;
        float maxMagnitude = this.GetComponent<SphereCollider>().radius;
        if (mouseDelta.magnitude > maxMagnitude)
        {
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }
        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;
        leftLine.SetPosition(1, projectile.transform.position);
        rightLine.SetPosition(1, projectile.transform.position);
        if (Input.GetMouseButtonUp(0))
        {
            aimingMode = false;
            Rigidbody projRB = projectile.GetComponent<Rigidbody>();
            projRB.isKinematic = false;
            projRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
            projRB.velocity = -mouseDelta * velocityMult;
            FollowCam.SWITCH_VIEW(FollowCam.eView.slingshot);
            FollowCam.POI = projectile;
            Instantiate<GameObject>(projLinePrefab, projectile.transform);
            leftLine.enabled = false;
            rightLine.enabled = false;
            projectile = null;
            launchSound.Play();
            MissionDemolition.SHOT_FIRED();

        }
    }
    void Awake()
    {
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);
        launchPos = launchPointTrans.position;

        leftLine.positionCount = 2;
        rightLine.positionCount = 2;

        leftLine.SetPosition(0, leftPoint.position);
        rightLine.SetPosition(0, rightPoint.position);

        leftLine.enabled = false;
        rightLine.enabled = false;
    }
    void OnMouseDown()
    {
        aimingMode = true;
        int index = Random.Range(0, projectilePrefabs.Length);
        projectile = Instantiate(projectilePrefabs[index]) as GameObject;
        projectile.transform.position = launchPos;
        projectile.GetComponent<Rigidbody>().isKinematic = true;
        leftLine.enabled = true;
        rightLine.enabled = true;
    }
}
