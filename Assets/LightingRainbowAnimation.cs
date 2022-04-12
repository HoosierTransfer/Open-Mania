using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightingRainbowAnimation : MonoBehaviour
{
    /* Number of frames in between color changes in Light. Private variable editable from Unity editor. 
     * If colorChangeInterval is set to n, the color of Light will wait n frame(s) to change the color. 
     */
    [SerializeField]
    int colorChangeInterval = 0;

    Light mainLight; // Variable for storing data type of Light (component) 

    // Variable of byte type data for setting Color32 value
    byte red = 0;
    byte green = 0;
    byte blue = 0;
    byte alpha = 255;

    int frameCount = 0; // Variable used to pause color changes in Light for the frame count specified with colorChangeInterval

    private void Awake()
    {
        mainLight = GetComponent<Light>(); // Reference to the game object's Light component 
        red = 255;
        mainLight.color = new Color32(red, green, blue, alpha); // Default color of Light set to RGB(255, 0, 0) 
        
    }

    // Update is called once per frame
    void Update()
    {
        DoTimedRainbowLightingAnim();
    }

    private void DoTimedRainbowLightingAnim() // Function for calling AnimateLightingInRainbow() in certain frames
    {
        if (colorChangeInterval == 0)
        {
            AnimateLightingInRainbow();
        }
        else if (colorChangeInterval != 0)
        {
            frameCount++; // count how many frames it has been since the last change in Light's color

            if (frameCount == colorChangeInterval)
            {
                AnimateLightingInRainbow();
                frameCount = 0;
            }
        }
    }

    private void AnimateLightingInRainbow() // Function for changing the color of Light 
    {
        // if - else statement that calculates RGB value for the color change 
        if (red == 0 && green == 0 && blue == 0)
            red = 255;
        else if (red == 0 && green < 255 && blue == 255)
            green++;
        else if (red == 0 && green == 255 && blue > 0)
            blue--;
        else if (red == 255 && green == 0 && blue < 255)
            blue++;
        else if (red == 255 && green > 0 && blue == 0)
            green--;
        else if (red > 0 && green == 0 && blue == 255)
            red--;
        else if (red < 255 && green == 255 && blue == 0)
            red++;

        mainLight.color = new Color32(red, green, blue, alpha); // Update the RGB value of the color of Light with the RGB value calculated from the above if-else statements
         // Log the new RGB value in the console
    }
}