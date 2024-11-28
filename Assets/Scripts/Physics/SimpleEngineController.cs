using UnityEngine;

public class SimpleEngineController : MonoBehaviour
{

    // composants
    protected Engine engine;

    // inputs
    public KeyCode key; // touche pour allumer/éteindre le moteur

    // percentage management
    [Range(0, 1)] public float default_percentage = 0.5f;

    // START
    protected virtual void Start()
    {
        // on récupère les composants
        engine = GetComponent<Engine>();
    }

    // UPDATE
    void Update()
    {
        UpdateOnOff();
    }

    
    protected void UpdateOnOff()
    {
        if (Input.GetKeyDown(key))
        {
            if (engine.isOn)
            {
                engine.TurnOff();
            }
            else
            {
                engine.TurnOn();
                engine.SetPercentage(default_percentage);
            }
        }
    }
}