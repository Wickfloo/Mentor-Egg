using UnityEngine;

public class Temp : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnEnable()
    {
        Debug.Log(gameObject.name + " was just turned ON by: " + UnityEngine.StackTraceUtility.ExtractStackTrace());
    }

    private void OnDisable()
    {
        // This will print to your console the exact moment the spider turns off and what caused it!
        Debug.Log(gameObject.name + " was just turned OFF!");
    }
}
