using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialMenu : SingletonGameMenu<TutorialMenu>
{
    [System.Serializable]
    struct TutorialPage
    {
        public Sprite image;
        public LocalizedString description;
    }

    [SerializeField]
    TutorialPage[] _pages = new TutorialPage[0];
    [SerializeField]
    Button _nextPageButton = null;
    [SerializeField]
    Button _prevPageButton = null;
    [SerializeField]
    Button _closeButton = null;

    [SerializeField]
    Image _tutorialPanel = null;
    [SerializeField]
    Text _tutorialText = null;

    event Action _tutorialFInishEvent;
    public event Action TutorialFInishEvent 
    {
        add => _tutorialFInishEvent += value;
        remove => _tutorialFInishEvent -= value;
    }

    int _curPage = 0;

    private int GetCurPage()
    {
        return _curPage;
    }

    private void SetCurPage(int value)
    {
        Debug.Assert(value >= 0 && value < _pages.Length);

        _curPage = value;

        if(_curPage == 0)
        {
            _prevPageButton.gameObject.SetActive(false);
        }
        if(_curPage == _pages.Length - 1)
        {
            _nextPageButton.gameObject.SetActive(false);
        }
        if (_curPage >= 0 && _curPage < _pages.Length - 1)
        {
            _nextPageButton.gameObject.SetActive(true);
        }
        if (_curPage > 0 && _curPage < _pages.Length)
        {
            _prevPageButton.gameObject.SetActive(true);
        }

        _tutorialPanel.sprite = _pages[_curPage].image;
        _tutorialText.text = _pages[_curPage].description;

        if (_unskippable)
        {
            _visited[_curPage] = true;
            if (AllVisited())
            {
                PlayerPrefs.SetInt(GameConsts.k_PlayerPrefTutorialViewed, 1);
                _closeButton.gameObject.SetActive(true);
                _unskippable = false;
            }
        }
    }

    bool[] _visited = null;
    bool _unskippable = false;
    

    bool _valid = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        _valid = _pages != null && _pages.Length > 0 && _nextPageButton != null && _prevPageButton != null && _tutorialPanel != null && _closeButton != null;
        Debug.Assert(_valid);

        _nextPageButton.onClick.AddListener(NextPage);
        _prevPageButton.onClick.AddListener(PrevPage);
        _closeButton.onClick.AddListener(OnBackPressed);

        ResetTutorial();
    }

    bool AllVisited()
    {
        foreach(bool val in _visited)
        {
            if (!val) return false;
        }
        return true;
    }

    void NextPage()
    {
        Debug.Assert(_valid && _pages.Length >= 2, this);
        SetCurPage(GetCurPage() + 1);
    }

    void PrevPage()
    {
        Debug.Assert(_valid && _pages.Length >= 2, this);
        SetCurPage(GetCurPage() - 1);
    }

    void ResetTutorial()
    {
        int tutorialViewed = PlayerPrefs.GetInt(GameConsts.k_PlayerPrefTutorialViewed, 0);
        if (tutorialViewed == 0)
        {
            _unskippable = true;
            _visited = new bool[_pages.Length];
            _closeButton.gameObject.SetActive(false);
        }
        else
        {
            _closeButton.gameObject.SetActive(true);
        }
        SetCurPage(0);
    }

    public override bool CanClose()
    {
        return _unskippable ? false : true;
    }

    public override void OnEnterMenu()
    {
        base.OnEnterMenu();
        ResetTutorial();
    }

    public override void OnLeaveMenu()
    {
        base.OnLeaveMenu();
        _tutorialFInishEvent?.Invoke();
    }
}
