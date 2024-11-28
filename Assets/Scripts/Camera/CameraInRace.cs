using UnityEngine;

public class CameraInRace : MonoBehaviour
{
    
    public Transform target;
    // public Vector3 offset = new Vector3(0, 0, -10);
    // public float speed = 1.0f;

    private Vector3 velocity;
    [SerializeField] private float timeOffset = 0.2f;
    [SerializeField] private float Ytranslation = 20f;
    [SerializeField] private float Ztranslation = 20f;
    [SerializeField] private float Xtranslation = 0f;

    [SerializeField] private float lookAtZ = 0f;
    [SerializeField] private float lookAtY = 0f;
    void LateUpdate()
    {
        if (target != null)
        {

            Vector3 targetPosition = target.position;
            targetPosition = target.position + target.forward * Ztranslation + target.up * Ytranslation + target.right * Xtranslation;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, timeOffset);

            // on fait tourner la camera pour qu'elle regarde la tete du joueur
            Vector3 lookAt = target.position + target.up * lookAtY + transform.forward * lookAtZ;
            transform.LookAt(lookAt);
        }
    }

}