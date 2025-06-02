using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 개발자: 이예린
/// 
/// 타이틀 로직 관련 기능 관리하는 메서드
/// </summary>
public class TitleManager : MonoBehaviour
{
    [SerializeField] string roomSceneName;

    #region Unity Event
    private void Start()
    {
        SoundManager.Instance.PlayBGM(bgmClip.title);     // 타이틀 BGM 실행 
    }
    #endregion

    #region Go To Game Room
    /// <summary>
    /// 게임 룸 씬으로 이동하는 메서드(Develop용)
    /// </summary>
    public void GoToGameScene()
    {
        SceneManager.LoadScene(roomSceneName);
    }
    #endregion
}