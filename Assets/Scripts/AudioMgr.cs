using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioMgr : MonoBehaviour
{
    const int k_MaxOneShotSrcs = 5;

    [SerializeField]
    GameGlobalConfig _config;
    [SerializeField]
    float _transitionTime = 1f;
    [SerializeField]
    [Range(0,1)]
    float _volume = 0.5f;

    AudioSource[] _oneshotSrcs;
    AudioSource _musicSrc;
    AudioClip _last;

    void Awake()
    {
        if (!GameConsts.audioMgr)
        {
            GameConsts.audioMgr = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }


        _oneshotSrcs = new AudioSource[k_MaxOneShotSrcs];
        for(int i=0; i<k_MaxOneShotSrcs; ++i)
        {
            _oneshotSrcs[i] = gameObject.AddComponent<AudioSource>();
            _oneshotSrcs[i].playOnAwake = false;
            _oneshotSrcs[i].loop = false;
            _oneshotSrcs[i].volume = _volume;
        }

        _musicSrc = gameObject.AddComponent<AudioSource>();
        _musicSrc.playOnAwake = true;
        _musicSrc.loop = true;
        _musicSrc.volume = _volume;

        PlayDefault();
    }

    IEnumerator _transitionRoutine(float second, AudioClip newClip)
    {
        for(float time = 0; time <= second; time += Time.deltaTime)
        {
            _musicSrc.volume = Mathf.Lerp(_volume, 0, time/second);
            yield return null;
        }

        _musicSrc.clip = newClip;
        _musicSrc.Play();

        for (float time = 0; time <= second; time += Time.deltaTime)
        {
            _musicSrc.volume = Mathf.Lerp(0, _volume, time / second);
            yield return null;
        }
        _musicSrc.volume = _volume;
    }


    public void PlayEffect(AudioClip clip, int slot = 0)
    {
        _oneshotSrcs[slot].Stop();
        _oneshotSrcs[slot].clip = clip;
        _oneshotSrcs[slot].Play();
    }

    public void Play(AudioClip clip)
    {
        if (_musicSrc.clip)
        {
            _last = _musicSrc.clip;
            StartCoroutine(_transitionRoutine(_transitionTime, clip));
        }
        else
        {
            _musicSrc.clip = clip;
            _musicSrc.Play();
        }
    }

    public void PlayLast()
    {
        if(_last)
            Play(_last);
    }

    public void PlayDefault()
    {

        if (SceneManager.GetActiveScene().buildIndex == GameConsts.k_MainMenuSceneIndex)
        {
            _musicSrc.clip = _config.MenuBGM;
            _musicSrc.Play();
        }
        else
        {
            _musicSrc.clip = _config.GameBGM;
            _musicSrc.Play();
        }
    }
}
