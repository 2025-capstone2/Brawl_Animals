using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 개발자: 이예린
/// Fusion 네트워크 세션(Room)을 생성하고 시작하는 역할을 담당하는 매니저 클래스.
/// - 매칭(Matching) 결과를 전달받아 세션을 초기화 및 생성한다.
/// - SessionManager는 오직 세션 연결만을 책임진다.
/// </summary>
public class SessionManager : MonoBehaviour
{
    #region Runner Management
    [Header("Network Runner")]
    [SerializeField]
    [Tooltip("미리 연결해둔 NetworkRunner Prefab을 할당")]
    private NetworkRunner runnerPrefab; // 네트워크 기능을 담당할 runner
    [SerializeField] 
    private InputHandler inputHandler;
    [SerializeField]
    private NetworkRunner _runner;  // 런타임에 Instantiate로 생성해 사용하는 실제 NetworkRunner 인스턴스

    [Header("Session Info")]
    [SerializeField]
    private string _currentSessionName; // 현재 세션 이름
    [SerializeField]
    private bool _isHost;   // 호스트 여부
    #endregion

    #region Unity Event
    private void Start()
    {
        StartSession();
    }
    #endregion

    #region Session Logic
    [ContextMenu("StartSession")]
    /// <summary>
    /// 매칭 및 세션 연결을 시작하는 메서드.
    /// 매칭 결과에 따라 Host 또는 Client로 세션에 연결함.
    /// </summary>
    private async void StartSession()
    {
        Debug.Log("StartMatching");
        await StartSessionInternal();  // 세션 연결
    }

    /// <summary>
    /// 매칭 결과를 기반으로 세션 연결을 시작하는 메서드.
    /// 세션 연결 후, Host가 수동으로 게임 씬을 로드한다.
    /// </summary>
    /// <returns></returns>
    private async Task StartSessionInternal()
    {
        // 방 이름이 설정되어 있지 않으면 에러 출력
        if (string.IsNullOrEmpty(_currentSessionName))
        {
            Debug.LogError("Session의 이름이 설정되지 않았습니다.");
            return;
        }
        Debug.Log("세션 연결 시작");

        _runner.ProvideInput = true;
        if (inputHandler != null)
            _runner.AddCallbacks(inputHandler);
        else
            Debug.LogError("InputHandler가 연결되어 있지 않습니다.");
        // 세션 연결 시작
        if (_isHost)
        {
            // 현재 활성화된 씬의 build index를 SceneRef로 변환하여 지정 (Fusion이 씬을 재로드하도록 유도)
            var sceneRef = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
            // Host인 경우 방 생성
            await _runner.StartGame(new StartGameArgs()
            {
                // Host 여부에 따라 모드 결정
                GameMode = _isHost ? GameMode.Host : GameMode.Client,
                // 세션(방) 이름 설정
                SessionName = _currentSessionName,
                // 기본 씬 매니저 추가
                // 씬 로딩 및 동기화를 자동으로 지원해줌
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
                // Scene을 null이 아닌 실제 현재 씬으로 명시 (씬 내 NetworkObject 자동 등록 유도)
                Scene = sceneRef,
                PlayerCount = 4
            });
        }
        else
        {
            // Client인 경우 방 조인 시도 (대기 포함)
            bool success = await TryJoinSessionWithRetry(_currentSessionName, maxRetry: 5, retryDelayMs: 1000);

            if (!success)
            {
                Debug.LogError("SessionManager - 세션 조인 실패 (모든 재시도 실패)");
                // TODO... 실패 처리 (로비로 복귀 등)
            }
        }
    }

    /// <summary>
    /// 세션 조인 시도를 일정 횟수 재시도하는 메서드
    /// </summary>
    private async Task<bool> TryJoinSessionWithRetry(string sessionName, int maxRetry = 5, int retryDelayMs = 1000)
    {
        for (int attempt = 1; attempt <= maxRetry; attempt++)
        {
            Debug.Log($"SessionManager - 세션 조인 시도 {attempt}/{maxRetry}...");

            var result = await _runner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.Client,
                SessionName = sessionName,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
                Scene = null,
            });

            if (result.Ok)
            {
                Debug.Log("SessionManager - 세션 조인 성공!");
                return true;
            }
            else
            {
                Debug.LogWarning($"SessionManager - 세션 조인 실패... {retryDelayMs}ms 후 재시도 예정");
                await Task.Delay(retryDelayMs);
            }
        }

        return false;
    }
    #endregion

    /// <summary>
    /// 고유한 방 이름을 생성하는 메서드
    /// GUID를 사용하여 이름 충돌 없이 유일한 SessionName을 생성
    /// 이를 통해 방 이름 충돌을 방지하고, 매칭마다 독립적인 방을 생성 가능
    /// </summary>
    /// <returns>랜덤으로 생성된 방 이름(string)</returns>
    /*private string GenerateRandomRoomName()
    {
        return "Room_" + System.Guid.NewGuid().ToString();
    }*/
}