using UnityEngine;

public class EngineAlignmentHandler : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Engine Alignment with plane")]
    public Vector3 engineAlignmentDirection = Vector3.forward;
    public Vector3 planeAlignmentNormal = Vector3.up;
    public float alignmentThreshold = 0.01f;
    public float rotationSpeed = 5f;

    [Header("Engine Alignement with Pod")]
    public Pod pod;
    // public angle


    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (pod == null)
        {
            pod = transform.parent.GetComponent<Pod>();
        }
    }

    void FixedUpdate()
    {

        // 1 - calculate the alignment of the engine with the plane
        Vector3 global_engine_forward = transform.TransformDirection(engineAlignmentDirection);
        float plane_alignment = Vector3.Dot(global_engine_forward, planeAlignmentNormal);
        // Debug.Log("plane_alignment: " + plane_alignment);



        // 2 - calculate the alignment of the engine with the pod

        // Pod's global forward vector
        Vector3 global_pod_forward = pod.chassis.TransformDirection(Vector3.forward);

        // Calculate the alignment of the engine with the pod
        float pod_alignement = Vector3.Dot(global_pod_forward.normalized, global_engine_forward.normalized);
        // Debug.Log("pod_alignement: " + pod_alignement + " global_pod_forward: " + global_pod_forward + " global_engine_forward: " + global_engine_forward);
        
        if (Mathf.Abs(plane_alignment) < alignmentThreshold && pod_alignement > 0f) { return; }



        // 3 - calculate the rotation target of the engine to align it with the plane

        // Calculate the forward engine direction projected onto the plane
        Vector3 engine_forward_XZ = Vector3.ProjectOnPlane(global_engine_forward, planeAlignmentNormal).normalized;
        // Debug.Log("engine_forward_XZ: " + engine_forward_XZ + " planeAlignmentNormal: " + planeAlignmentNormal);

        if (engineAlignmentDirection.z < 0f)
        {
            // todo : not ideal, we should find a better way to handle this
            // we reverse the XZ direction of the engine if the engineAlignmentDirection is negative
            // so that the engine points backward when the engineAlignmentDirection is negative
            engine_forward_XZ = -engine_forward_XZ;
        }

        // Create the target rotation using LookRotation
        Quaternion engine_target_rotation = Quaternion.LookRotation(engine_forward_XZ, planeAlignmentNormal);


        // 4 - calculate the 180° rotation if the engine is not aligned with the pod

        if (pod_alignement < 0f)
        {
            engine_target_rotation = Quaternion.AngleAxis(180, planeAlignmentNormal) * engine_target_rotation;
        }


        // 5 - apply the rotation to the engine
        rb.MoveRotation(Quaternion.Lerp(rb.rotation, engine_target_rotation, Time.fixedDeltaTime * rotationSpeed));


    }

    // Alignement handler
    public void SetForwardAlignment(Vector3 newEngineAlignmentDirection)
    {
        engineAlignmentDirection = newEngineAlignmentDirection;
    }
}
