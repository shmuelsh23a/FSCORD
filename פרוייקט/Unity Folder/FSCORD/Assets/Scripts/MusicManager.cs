using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {
    public AudioSource[] music;
    int randomNumber;
    public AudioSource chosenMusic;
	// Use this for initialization
	void Start ()
    {       

    }
	
	// Update is called once per frame
	void Update ()
    {
	    if (chosenMusic.isPlaying == false)
        {
            PlayMusic();
        }
	}

    void Randomizer()
    {
        randomNumber = Random.Range(0, music.Length);
    }

    void PlayMusic()
    {
        Randomizer();
        chosenMusic.clip = music[randomNumber].clip;
        chosenMusic.Play();
    }
}
