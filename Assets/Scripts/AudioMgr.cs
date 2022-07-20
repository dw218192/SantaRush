using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class AudioMgr : MonoBehaviour
{
    [SerializeField]
    GameGlobalConfig _config;
    [SerializeField]
    float _transitionTime = 1f;

    AudioSource _src;
    float _volume;
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

        _src = GetComponent<AudioSource>();
        _src.playOnAwake = true;
        _volume = _src.volume;
        PlayDefault();
    }

    IEnumerator _transitionRoutine(float second, AudioClip newClip)
    {
        for(float time = 0; time <= second; time += Time.deltaTime)
        {
            _src.volume = Mathf.Lerp(_volume, 0, time/second);
            yield return null;
        }

        _src.clip = newClip;
        _src.Play();

        for (float time = 0; time <= second; time += Time.deltaTime)
        {
            _src.volume = Mathf.Lerp(0, _volume, time / second);
            yield return null;
        }
        _src.volume = _volume;
    }

    public void Play(AudioClip clip)
    {
        if (_src.clip)
        {
            _last = _src.clip;
            StartCoroutine(_transitionRoutine(_transitionTime, clip));
        }
        else
        {
            _src.clip = clip;
            _src.Play();
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
            _src.clip = _config.MenuBGM;
            _src.Play();
        }
        else
        {
            _src.clip = _config.GameBGM;
            _src.Play();
        }
    }
}
