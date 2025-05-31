using Fusion;
using Fusion.Sockets;
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

        // NetworkRunner 프리팹을 인스턴스화하여 런타임에 사용할 실제 Runner 생성
        _runner = Instantiate(runnerPrefab);
        _runner.gameObject.name = "==[Runner]==";
        Debug.Log("Runner 생성 완료");

        // 씬 동기화 및 네트워크 오브젝트 자동 등록을 위한 SceneManager 컴포넌트 추가
        // Fusion은 Runner 오브젝트에 SceneManager가 붙어 있어야 StartGame 동작 시 자동으로 씬을 관리함
        var sceneManager = _runner.gameObject.AddComponent<NetworkSceneManagerDefault>();

        // 세션 연결 시작
        if (_isHost)
        {
            Debug.Log("호스트");
            // 현재 활성화된 씬의 build index를 SceneRef로 변환하여 지정 (Fusion이 씬을 재로드하도록 유도)
            var sceneRef = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
            // Host인 경우 방 생성
            var result = await _runner.StartGame(new StartGameArgs()
            {
                // Host 여부에 따라 모드 결정
                GameMode = _isHost ? GameMode.Host : GameMode.Client,
                // 세션(방) 이름 설정
                SessionName = _currentSessionName,
                // Runner에 부착된 SceneManager를 StartGameArgs에 지정
                // Fusion이 씬 전환, 네트워크 오브젝트 자동 등록 및 동기화를 수행할 수 있도록 함
                SceneManager = sceneManager,
                // Scene을 null이 아닌 실제 현재 씬으로 명시 (씬 내 NetworkObject 자동 등록 유도)
                Scene = sceneRef,
                PlayerCount = 4,
                Address = NetAddress.Any(),
            });

            if (!result.Ok)
            {
                Debug.LogError($"StartGame 실패 → Ok: {result.Ok}, ShutdownReason: {result.ShutdownReason}, ErrorMessage: {result.ErrorMessage}");
                return;
            }

            Debug.Log("StartGame 성공");
        }
        else
        {
            Debug.Log("클라이언트");
            // Client인 경우 방 조인 시도 (대기 포함)
            bool success = await TryJoinSessionWithRetry(_currentSessionName, maxRetry: 5, retryDelayMs: 1000);

            if (!success)
            {
                Debug.LogError("SessionManager - 세션 조인 실패 (모든 재시도 실패)");
                // TODO... 실패 처리 (로비로 복귀 등)
                return;
            }
            _runner.ProvideInput = true;
            Debug.Log("클라이언트 ProvideInput 설정 완료");
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
                _runner.ProvideInput = true;
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
}