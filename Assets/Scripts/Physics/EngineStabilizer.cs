using UnityEngine;
using System.Collections.Generic;

public class EngineStabilizer : MonoBehaviour
{
    // list of engines
    [Header("Engines & pod")]
    public List<Engine> engines;
    protected Pod pod;

    [Header("Height Target")]
    public float targetHeight = 15f;

    private PIDController heightPID;

    [Header("PID Controllers")]
    public float heightDamping = 100f;
    public float heightCorrection = 50f;

    private PIDController pitchPID;
    public float pitchDamping = 0f;
    public float pitchCorrection = 0f;
    private PIDController rollPID;
    public float rollDamping = 0f;
    public float rollCorrection = 0f;

    // START
    void Start()
    {
        // assign the multiple PID Controllers
        heightPID = new PIDController();
        pitchPID = new PIDController();
        rollPID = new PIDController();

        // assign the pod
        pod = GetComponent<Pod>();
    }

    // UPDATE
    void FixedUpdate()
    {
        
        // 1 - on trie les moteurs allumés
        List<Engine> onEngines = engines.FindAll(engine => engine.isOn);
        // if (onEngines.Count == 0) { return; } // si aucun moteur n'est allumé on ne fait rien
    

        // 2 - We stabilise the pod at the target height
        float targetY = pod.chassis.position.y - CalculateDistanceFromGround() + targetHeight;
        float totalThrustNeeded = CalculateGlobalThrustToStabilizeAtY(targetY,pod.GetTotalMass());
        float baseThrust = totalThrustNeeded / onEngines.Count;


        // 3 - We stabilise the pod in pitch and roll
        float pitchTorque = CalculatePitchTorque();
        float rollTorque = CalculateRollTorque();
        // Debug.Log("pitchTorque: " + pitchTorque + " rollTorque: " + rollTorque);


        // 4 - We calculate the splitting of the thrust between the engines
        float[] engineThrusts = new float[onEngines.Count];
        float totalThrust = 0f;
        for (int i = 0; i < onEngines.Count; i++)
        {
            string log1 = "engine " + i;
            // Vector3 localOffset = pod.chassis.InverseTransformPoint(onEngines[i].transform.position);
            // log1 += " localOffset: " + localOffset;
            Vector3 localOffset = pod.GetLocalOffsetFromCenterOfMass(onEngines[i].transform.position);
            log1 += " localOffset com: " + pod.GetLocalOffsetFromCenterOfMass(onEngines[i].transform.position);
            // Debug.Log(log1);

            engineThrusts[i] = baseThrust - pitchTorque / localOffset.z + rollTorque / localOffset.x;
            totalThrust += engineThrusts[i];
            // Debug.Log("engine "+i+" z offset: "+localOffset.z+ " x offset: "+localOffset.x + " thrust: " + engineThrusts[i]);
        }
        float thrustNormalizationFactor = totalThrustNeeded / totalThrust;



        // 5 - We apply thrust to each engine
        for (int i = 0; i < onEngines.Count; i++)
        {
            // we apply the normalized thrust so that the total thrust is equal to totalThrustNeeded
            engineThrusts[i] *= thrustNormalizationFactor;
            onEngines[i].SetPower(engineThrusts[i]);
        }

        // DEBUG
        /* float realTotalThrustPowered = 0f;
        foreach (Engine engine in onEngines)
        {
            realTotalThrustPowered += engine.power*engine.percentage;
        }
        if (realTotalThrustPowered != totalThrustNeeded)
        {
            Debug.LogWarning("Thrusts are not equal" + "(realTotalThrustPowered: " + realTotalThrustPowered + " totalThrustNeeded: " + totalThrustNeeded + ")");
        }
        else
        {
            Debug.Log( (realTotalThrustPowered == totalThrustNeeded) ? "Thrusts are equal" : "Thrusts are not equal"
                + "(realTotalThrustPowered: " + realTotalThrustPowered + " totalThrustNeeded: " + totalThrustNeeded + ")");
        } */

    }

    // STABILIZATION FUNCTIONS
    private float CalculateDistanceFromGround()
    {
        // on calcule la hauteur du pod par rapport au sol (layer Terrain)
        RaycastHit hit;
        if (Physics.Raycast(pod.chassis.position, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
        {
            return hit.distance;
        }
        return 0f;
    }
    private float CalculateGlobalThrustToStabilizeAtY(float targetY,float total_weight)
    {
        // on calcule la hauteur du pod par rapport au sol (layer Terrain)
        float currentY = pod.chassis.position.y;

        // Calculate the adjustment needed
        heightPID.SetCoeffs(heightDamping, 0f, heightCorrection);
        float adjustment = heightPID.Calculate(targetY, currentY, Time.fixedDeltaTime);

        // Scale adjustment to engine power limits
        float baselineThrust = total_weight * 9.81f; // Total mass * gravity
        return baselineThrust + adjustment;
    }
    private float CalculatePitchTorque(float target_pitch = 0f)
    {
        // Calculate tilt angles
        Vector3 tiltAngles = pod.chassis.rotation.eulerAngles;
        float pitchError = (tiltAngles.x > 180f) ? tiltAngles.x - 360f : tiltAngles.x;

        // Calculate corrective torques
        pitchPID.SetCoeffs(pitchDamping, 0f, pitchCorrection);
        return pitchPID.Calculate(target_pitch, pitchError, Time.fixedDeltaTime);
    }
    private float CalculateRollTorque(float target_roll = 0f)
    {
        // Calculate tilt angles
        Vector3 tiltAngles = pod.chassis.rotation.eulerAngles;
        float rollError = (tiltAngles.z > 180f) ? tiltAngles.z - 360f : tiltAngles.z;

        // Calculate corrective torques
        rollPID.SetCoeffs(rollDamping, 0f, rollCorrection);
        return rollPID.Calculate(target_roll, rollError, Time.fixedDeltaTime);
    }

}


public class PIDController
{
    public float Kp = 1.0f; // Proportional gain -> freine l'erreur !!
    public float Ki = 0.0f; // Integral gain -> erreur d'offset !!
    public float Kd = 0.0f; // Derivative gain -> freine la variation de l'erreur !!

    public float previousError = 0.0f;
    public float integral = 0.0f;

    public PIDController() { }

    public PIDController(float Kp, float Ki, float Kd)
    {
        this.Kp = Kp;
        this.Ki = Ki;
        this.Kd = Kd;
    }

    public void SetCoeffs(float Kp, float Ki, float Kd)
    {
        this.Kp = Kp;
        this.Ki = Ki;
        this.Kd = Kd;
    }

    public float Calculate(float setpoint, float current, float deltaTime)
    {
        float error = setpoint - current;
        integral += error * deltaTime;
        float derivative = (error - previousError) / deltaTime;

        previousError = error;

        float adjustment = (Kp * error) + (Ki * integral) + (Kd * derivative);
        // Debug.Log("Adjustment: " + adjustment);

        return adjustment;
    }
}
