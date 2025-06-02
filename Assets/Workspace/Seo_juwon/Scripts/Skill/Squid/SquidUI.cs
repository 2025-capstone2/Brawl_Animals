using UnityEngine;
using System.Collections;

public class SquidUI : MonoBehaviour
{
    public static SquidUI Instance;

    [SerializeField] private CanvasGroup blindPanel;

    private void Awake()
    {
        Instance = this;

        // ✅ UI 오브젝트가 꺼져있으면 코루틴 작동안 하므로 강제로 켜준다
        if (!gameObject.activeSelf)
        {
            Debug.LogWarning("[SquidUI] 비활성화 상태에서 Awake → 자동 활성화");
            gameObject.SetActive(true);
        }

        // UI는 기본적으로 투명하게 시작
        blindPanel.alpha = 0f;
        blindPanel.blocksRaycasts = false;
    }

    public void StartBlind()
    {
        Debug.Log("[SquidUI] StartBlind() 호출됨");

        // 코루틴 전에 오브젝트 켜져 있는지 한 번 더 방어
        if (!gameObject.activeInHierarchy)
        {
            Debug.LogWarning("[SquidUI] 비활성 상태라 StartCoroutine 실행 안됨 → 강제 활성화");
            gameObject.SetActive(true);
        }

        StartCoroutine(BlindRoutine());
    }

    private IEnumerator BlindRoutine()
    {
        blindPanel.alpha = 1f;
        blindPanel.blocksRaycasts = true;

        yield return new WaitForSeconds(1f); // 1초 유지

        float time = 0f;
        float duration = 1f;

        while (time < duration)
        {
            time += Time.deltaTime;
            blindPanel.alpha = Mathf.Lerp(1f, 0f, time / duration);
            yield return null;
        }

        blindPanel.alpha = 0f;
        blindPanel.blocksRaycasts = false;
    }
}