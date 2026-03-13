using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private AudioSource musicSource;
    private AudioSource soundSource;
    [SerializeField] private AudioClip bgMusicClip;
    [SerializeField] private AudioClip tapClip;
    private AudioClip levelCompleteClip;
    private AudioClip levelFailClip;

    private bool isSFXOff = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        musicSource = gameObject.AddComponent<AudioSource>();
        soundSource = gameObject.AddComponent<AudioSource>();

        musicSource.playOnAwake = false;
        soundSource.playOnAwake = false;
    }

    private void PlayBgSound(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    private void PlaySFXSound(AudioClip clip)
    {
        if (isSFXOff) return;

        soundSource.clip = clip;
        soundSource.Play();
    }

    public void BgMusicControl(bool isPlay)
    {
        if (!isPlay) musicSource.Stop();
        else musicSource.Play(); 
    }

    public void SFXControl(bool isPlay)
    {
        isSFXOff = !isPlay;
    }

    public void Tap()
    {
        PlaySFXSound(tapClip);
    }

    public void LevelComplete()
    {
        PlaySFXSound(levelCompleteClip);
    }

    public void LevelFail()
    {
        PlaySFXSound(levelFailClip);
    }

    public void BgMusic()
    {
        PlayBgSound(bgMusicClip);
    }

    public void LowVibrate()
    {
        Handheld.Vibrate();
    }

    public static void MediumVibrate()
    {
        VibrateAndroid(80, 120);
    }

    public static void HighVibrate()
    {
        VibrateAndroid(120, 255);
    }

    static void VibrateAndroid(long ms, int amplitude)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject vibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator");

        AndroidJavaClass effectClass = new AndroidJavaClass("android.os.VibrationEffect");
        AndroidJavaObject effect = effectClass.CallStatic<AndroidJavaObject>("createOneShot", ms, amplitude);

        vibrator.Call("vibrate", effect);
#endif
    }
}
