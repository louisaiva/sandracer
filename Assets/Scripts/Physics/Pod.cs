using System.Collections.Generic;
using UnityEngine;

public class Pod : MonoBehaviour
{
    
    // string name = "Pod";
    // Chassis
    public Transform chassis;


    public float GetTotalMass()
    {
        // go over all children and add their mass
        float totalMass = 0f;
        foreach (Transform child in transform)
        {
            if (child.GetComponent<Rigidbody>() == null) { continue; }
            totalMass += child.GetComponent<Rigidbody>().mass;
        }
        return totalMass;
    }

    public Vector3 GetLocalCenterOfMass()
    {
        // go over all children and add their mass
        Vector3 centerOfMass = Vector3.zero;
        float totalMass = 0f;
        foreach (Transform child in transform)
        {
            if (child.GetComponent<Rigidbody>() == null) { continue; }
            centerOfMass += child.GetComponent<Rigidbody>().mass * chassis.InverseTransformPoint(child.position);
            totalMass += child.GetComponent<Rigidbody>().mass;
        }
        return centerOfMass / totalMass;
    }

    public Vector3 GetLocalOffsetFromCenterOfMass(Vector3 global_position)
    {
        Vector3 local_center_of_mass = GetLocalCenterOfMass();
        // Debug.Log("local_center_of_mass: " + local_center_of_mass);
        Vector3 local_position = chassis.InverseTransformPoint(global_position);
        return local_position - local_center_of_mass;
    }
}