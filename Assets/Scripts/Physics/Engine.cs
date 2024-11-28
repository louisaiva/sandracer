using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class Engine : MonoBehaviour
{
    // composants
    private Rigidbody rb;

    // variables du moteur
    public bool isOn; // moteur allumé ou éteint
    // public float fuel_consumption; // consommation d'essence par seconde
    public Vector3 direction; // direction (locale) vers laquelle le moteur pousse
    
    // POWER MANAGEMENT
    public float power; // puissance maximale du moteur
    public float percentage; // pourcentage de puissance actuel
    public Vector3 acceleration; // accélération du moteur


    // variables graphiques
    public GameObject fire; // feu du moteur


    // START
    void Start()
    {
        // on récupère les composants
        rb = GetComponent<Rigidbody>();
        fire = transform.Find("fire").gameObject;

        // on initialise les variables
        isOn = false;
        percentage = 0f;
        acceleration = Vector3.zero;

        // on eteint le moteur
        TurnOff();
    }

    // UPDATE
    void Update()
    {
        // on change l'alpha du material de la flamme en fonction du pourcentage de puissance
        fire.GetComponent<Renderer>().material.SetFloat("_alpha", percentage);

        // on update l'accélération
        UpdateAcceleration();
    }
    void FixedUpdate()
    {
        // on vérifie qu'on est allumé
        if (!isOn) { return; }

        // on sature le pourcentage de puissance
        // percentage = Mathf.Clamp(percentage, 0f, 1f);
        if (percentage == 0f) { return; }

        // on applique la force du moteur
        rb.AddRelativeForce(direction * power * percentage, ForceMode.Force);

        // debug
        Vector3 globalDirection = transform.TransformDirection(direction);
        Debug.DrawRay(transform.position + Vector3.up*1.4f, globalDirection * percentage, Color.red);

    }

    void UpdateAcceleration()
    {
        // update the acceleration with the rigidbody velocity
        acceleration = rb.linearVelocity - acceleration;
    }

    // ENGINE FUNCTIONS
    public void TurnOn()
    {
        isOn = true;
        percentage = 1f;
    }
    public void TurnOff()
    {
        isOn = false;
        percentage = 0f;
    }

    public void SetPower(float thrust)
    {
        // Convert thrust to percentage based on max power
        float percentage = Mathf.Clamp01(thrust / power);
        SetPercentage(percentage);
    }
    public void SetPercentage(float percentage)
    {
        this.percentage = percentage;
        Mathf.Clamp01(this.percentage);
    }    
    public void AddPercentage(float dp)
    {
        SetPercentage(percentage + dp);
    }
}