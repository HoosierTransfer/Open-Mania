using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapSelect : MonoBehaviour
{
    public AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void playClicked()
    {
        audioSource.Play();
    }
}
