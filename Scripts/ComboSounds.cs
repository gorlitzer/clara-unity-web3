 using System.Collections;
 using UnityEngine;
 
 public class ComboSounds : MonoBehaviour {
     
    public AudioClip[] audioClipArray;
    private AudioSource source;
    private AudioClip shootClip;

     void Start () {
         source = gameObject.GetComponent<AudioSource> ();
         int index = Random.Range(0, audioClipArray.Length);
         shootClip = audioClipArray[index];
         source.clip = shootClip;
         source.Play();
     }
}