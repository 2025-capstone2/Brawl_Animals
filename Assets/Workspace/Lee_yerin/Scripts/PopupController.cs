using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 개발자: 이예린
/// 
/// 팝업 관련 UI들을 일괄적으로 활성화하거나 비활성화하는 역할을 하는 컨트롤러
/// </summary>
public class PopupController : MonoBehaviour
{
    [Tooltip("활성화할 UI들을 저장한 리스트")]
    [SerializeField] List<GameObject> activationList = new();
    [Tooltip("비활성화할 UI들을 저장한 리스트")]
    [SerializeField] List<GameObject> deactivationList = new();
    [SerializeField] AudioClip buttonClip;

    /// <summary>
    /// 컨트롤러 버튼 눌렸을 때 호출될 메서드
    /// </summary>
    public void PopupControl()
    {
        SoundManager.Instance.PlaySFX(buttonClip);  // 버튼 클릭 사운드 실행

        DeactivateAll();
        ActivateAll();
    }

    /// <summary>
    /// activationList에 포함된 모든 오브젝트를 활성화(SetActive(true))하는 메서드
    /// </summary>
    private void ActivateAll()
    {
        foreach (GameObject activation in activationList)
        {
            activation.SetActive(true);
        }
    }

    /// <summary>
    /// deactivationList에 포함된 모든 오브젝트를 비활성화(SetActive(false))하는 메서드
    /// </summary>
    private void DeactivateAll()
    {
        foreach (GameObject deactivation in deactivationList)
        {
            deactivation.SetActive(false);
        }
    }
}