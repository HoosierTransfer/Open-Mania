using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DFJKPressed : MonoBehaviour
{
    //public AudioSource hitSounds;
    private ParticleSystem.Particle[] particleList;
    public ParticleSystem NoteEmiter;

    
    void Start()
    {
        particleList = new ParticleSystem.Particle[NoteEmiter.main.maxParticles];
    }

    private void OnParticleTrigger()
    {
        //if(Input.GetKeyDown(KeyCode.D) == true)
        //{
            //GetComponent<MeshRenderer>().material.color = new Color(230 / 255f, 0f, 38f / 255f);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetKey(gameObject.tag.ToLower()) == true)
        {
            // GetComponent<MeshRenderer>().material.color = new Color(230f / 255f, 0f, 38f / 255f);
            float closestDistance = 10000f;
            int currentParticle = -1;
            int numParticles = NoteEmiter.GetParticles(particleList);
            for(int i = 0; i < numParticles; i++)
            {
                float currentDistance = Vector3.Distance(particleList[i].position, transform.position);
                if(currentDistance < closestDistance)
                {
                    closestDistance = currentDistance;
                    currentParticle = i;
                    
                }
            }
            if (closestDistance < 1f && currentParticle != -1) {
            //Debug.Log(particleList[currentParticle].startLifetime - particleList[currentParticle].remainingLifetime);
                particleList[currentParticle].remainingLifetime = -1;
                NoteEmiter.SetParticles(particleList);
            //hitSounds.Stop();
            //hitSounds.Play();
            }
} 
        //{
            //GetComponent<MeshRenderer>().material.color = new Color(0f, 0f, 0f);
        //}
        
    }
}
