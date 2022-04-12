using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{

    public GameObject menuButton;
    public GameObject scrollView;
    // Start is called before the first frame update
    void Start()
    {
        string[] songs = Directory.GetDirectories("Assets/Songs");
        for (int i = 0; i < songs.Length; i++) {
            GameObject newButton = Instantiate(menuButton, scrollView.transform);
            newButton.GetComponentInChildren<Text>().text = songs[i].Substring(13);
            newButton.transform.localPosition = new Vector3(0, (-55 * i)-35);
            newButton.GetComponent<Button>().onClick.AddListener(()=> buttonPressed("Assets/Songs" + newButton.GetComponentInChildren<Text>().text));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void buttonPressed(string song) {
        SceneManager.LoadScene(0);
    }
}
