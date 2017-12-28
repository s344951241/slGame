using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundMgr : MonoBehaviour {

    public const int MaxAudioCount = 10;
    public const string ResourcePath = "";
    public static SoundMgr _instance;

    public Dictionary<string, int> _soundDictionary = new Dictionary<string, int>();
    public AudioSource _bgmSource;
    public AudioSource _lastSource;

    public void Awake()
    {
        if (SoundMgr._instance == null)
        {
            SoundMgr._instance = this;
            Object.DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (SoundMgr._instance != this)
            {
                Object.Destroy(gameObject);
            }
        }
    }

    public void bgmPause()
    {
        if (_bgmSource != null)
        {
            _bgmSource.Pause();
        }
    }
    public void soundPause()
    {
        if (_lastSource != null)
            _lastSource.Pause();
    }
    public void soundAllPause()
    {
        AudioSource[] array = Object.FindObjectsOfType<AudioSource>();
        if (array != null && array.Length > 0)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i].Pause();
            }
        }
    }
    public void bgmPlay(string audioName, float volume = 1, bool isLoop = true)
    {
        ResourceManager.Instance.DownLoadBundle(URLConst.GetSound(audioName), delegate(object obj)
        {
            Resource resource = ResourceManager.Instance.GetResource(URLConst.GetSound(audioName));
            AudioClip audioClip = resource.AudioClip;
            bgmPlayStart(audioClip, audioName, volume, isLoop);
            ResourceManager.Instance.DestoryResource(resource.BundlePath, false, true);
        }, null, ResourceManager.SOUND_PRIORITY);
    }
    public void soundPlay(string audioName, float volume = 1)
    { 
        ResourceManager.Instance.DownLoadBundle(URLConst.GetSound(audioName),delegate(object obj){
            Resource resource  = ResourceManager.Instance.GetResource(URLConst.GetSound(audioName));
            AudioClip audioClip = resource.AudioClip;
            soundPlayStart(audioClip,audioName,volume);
            ResourceManager.Instance.DestoryResource(resource.BundlePath,false,false);
        },null,ResourceManager.SOUND_PRIORITY);  
    }
    private void bgmPlayStart(AudioClip bgmSound, string bgmName, float volume = 1, bool isLoop = true)
    {
        bgmStop();
        if (!string.IsNullOrEmpty(bgmName) && bgmSound != null)
        {
            playBGMAudioClip(bgmSound, volume, isLoop, "");
        }
    }
    private void soundPlayStart(AudioClip audio, string name, float volume = 1)
    {
        if (_soundDictionary.ContainsKey(name))
        {
            if (_soundDictionary[name] <= 10 && audio != null)
            {
                StartCoroutine(playClipEnd(audio, name));
                playClip(audio, volume,false,"");
                int num = _soundDictionary[name];
                _soundDictionary[name] = num + 1;
            }
        }
        else
        {
            _soundDictionary.Add(name, 1);
            if(audio!=null)
            {
                StartCoroutine(playClipEnd(audio,name));
                playClip(audio, volume,false, "");
                int num = _soundDictionary[name];
                _soundDictionary[name] = num+1;
            }
        }
    }
    public void bgmStop()
    {
        if (_bgmSource != null && _bgmSource.gameObject)
        {
            Object.Destroy(_bgmSource.gameObject);
            _bgmSource = null;
        }
    }

    public void soundStop(string audioName)
    {
        GameObject gameObject = transform.Find(audioName).gameObject;
        if (gameObject != null)
        {
            Object.Destroy(gameObject);
        }
    }
    public void bgmSetVolume(float volume)
    {
        if (_bgmSource != null)
        {
            _bgmSource.volume = volume;
        }
    }
    private void playBGMAudioClip(AudioClip audioClip,float volume = 1,bool isLoop = true,string name = ""){
        if(audioClip==null)
            return;
        GameObject obj = new GameObject(string.IsNullOrEmpty(name) ? "BGMSound" : name);
        obj.transform.parent = transform;
        AudioSource audioSource = obj.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.loop = isLoop;
        audioSource.pitch = 1;
        audioSource.Play();
        _bgmSource = audioSource;
    }
    private void playClip(AudioClip audioClip,float volume = 1,bool isLoop = true,string name = ""){
        if (audioClip == null)
            return;
        GameObject gameObject = new GameObject(string.IsNullOrEmpty(name) ? audioClip.name : name);
        gameObject.transform.parent = transform;
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        StartCoroutine(playClipEndDestroy(audioClip, gameObject));
        audioSource.pitch = 1;
        audioSource.volume = volume;
        audioSource.clip = audioClip;
        audioSource.Play();
        _lastSource = audioSource;
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private IEnumerator playClipEnd(AudioClip audioClip, string audioName)
    {
        if (audioClip != null)
        {
            yield return new WaitForSeconds(audioClip.length * Time.timeScale);
            _soundDictionary[audioName]--;
            if (_soundDictionary[audioName] <= 0)
            {
                _soundDictionary.Remove(audioName);
            }
        }
        yield break;
    }

    private IEnumerator playClipEndDestroy(AudioClip audioClip, GameObject obj)
    {
        if (obj == null || audioClip == null)
            yield break;
        else
        {
            yield return new WaitForSeconds(audioClip.length*Time.timeScale);
            Destroy(obj);
        }
    }
}
