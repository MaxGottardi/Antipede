using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : Weapon
{
    [Header(nameof(Gun) + " Settings.")]
    //[SerializeField] float LaunchSpeed;

    [SerializeField] GameObject laserLine;

    private LineRenderer lineRenderer;
    // Start is called before the first frame update
    public override Projectile Fire(Vector3 Direction)
    {
        Projectile RayProjectile = InstantiateProjectile();
        RayProjectile.Initialise();
        //RayProjectile.Launch(transform.forward * LaunchSpeed);

        return RayProjectile;
    }

    public void shootLine()
    {
        Instantiate(laserLine, BarrelEndSocket.position, Quaternion.identity);
        //lineRenderer = laserLine.GetComponent<LineRenderer>();

        RaycastHit hit;

        //if (Physics.Raycast(BarrelEndSocket.position, BarrelEndSocket.TransformDirection(Vector3.up), out hit, Mathf.Infinity))
        //{
            //float beamLength = Vector3.Distance(transform.position, hit.point);
            //lineRenderer.SetPosition(1, new Vector3(0f, 0f, beamLength));
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            //Debug.Log("Did Hit");
        //}
        //else
        //{
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            //Debug.Log("Did not Hit");
        //}
    }

    public void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            shootLine();
        }
    }

}
