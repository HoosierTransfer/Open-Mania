using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class UI : MonoBehaviour
{

    public GameObject menuButton;
    public GameObject scrollView;
    // Start is called before the first frame update
    void Start()
    {
        int totalSongs = 0;
        List<List<string>> diffs = new List<List<string>>();
        List<int> diffCount = new List<int>();
        string[]songs = Directory.GetDirectories("Assets/StreamingAssets/Songs");
        for (int i = 0; i < songs.Length; i++) {
            totalSongs++;
            diffs.Add(Directory.GetFiles(songs[i]).ToList());
            diffCount.Add(diffs[i].Count);
            Debug.Log(diffCount[i]);
        }
        for (int i = 0; i < songs.Length; i++) {
            for(int j = 0; j < diffCount[i]; j++) {
                if(diffs[i][j].EndsWith(".osu")) {
                    GameObject newButton = Instantiate(menuButton, scrollView.transform);
                    newButton.GetComponentInChildren<Text>().text = diffs[i][j].Substring(28);
                    Debug.Log(diffs[i][j].Substring(13));
                    newButton.transform.localPosition = new Vector3(0, (-25 * (i * j))-35);
                    newButton.GetComponent<Button>().onClick.AddListener(()=> buttonPressed("Assets/StreamingAssets/Songs" + newButton.GetComponentInChildren<Text>().text));
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void buttonPressed(string song) {
        string[] sourceDir = song.Split('\\');
        Debug.Log(sourceDir.Length);

        sceneData.mapDir = song;
        sceneData.osuFile = sourceDir[sourceDir.Length - 1];
        Debug.Log(sceneData.osuFile);
        SceneManager.LoadScene(0);
    }
}
