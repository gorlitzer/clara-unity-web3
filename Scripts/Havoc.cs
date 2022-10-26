using UnityEngine;

public class Havoc : MonoBehaviour {

    public float delay = 0.25f;

    void Update() 
    {
        Destroy(gameObject, delay);
    }
}