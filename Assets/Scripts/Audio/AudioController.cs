using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClipSetSO clips;

    public AudioClipSO currentPlayingClip;

    public FloatReference speedUpDuration;

    public bool isSpeeding;

    [Header("Speed Up Info")]
    public FloatReference maxPitch;
    public FloatReference speedUpThreshold;

    private static string BGMNormalName = "BGM_Normal";


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        BGMSpeedUP();  
    }

    public void PlayNormalBGM()
    {
        PlayClipWithName(BGMNormalName);
        audioSource.loop = true;
    }

    private void BGMFade()
    {
        
    }

    private void BGMSlowDown()
    {

    }

    private void BGMSpeedUP()
    {
        if (GameObject.Find("BlackHole").GetComponent<BlackHoleController>().absorbNum >= speedUpThreshold)
        {
            isSpeeding = true;
            //StartCoroutine(BGMPitchTunning());
        }
    }

    IEnumerator BGMPitchTunning(float targetPitch)
    {
        float timeElapsed = 0;

        while (timeElapsed < speedUpDuration)
        {
            audioSource.pitch = Mathf.Lerp(1f, targetPitch, timeElapsed / speedUpDuration);
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        audioSource.pitch = targetPitch;
        
    }

    private void BGMRaise()
    {

    }

    public void PlayClipWithName(string clipName)
    {
        AudioClipSO soundClip = clips.FindClipByName(clipName); 
        if (soundClip)
        {
            currentPlayingClip = soundClip;
            audioSource.clip = soundClip.clip;
            audioSource.volume = soundClip.volume;
            audioSource.Play();
        }
    }

    public void PlayCrashSE()
    {
        PlayClipWithName("SE_Crash");
    }

    public void PlaySuckinSE()
    {
        PlayClipWithName("SE_Suckin");
    }

        
    public void PlayParkSE()
    {
        PlayClipWithName("SE_Park");
    }


}
