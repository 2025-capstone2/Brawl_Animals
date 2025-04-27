using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SessionManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("미리 연결해둔 NetworkRunner Prefab을 할당")]
    private NetworkRunner runnerPrefab; // 네트워크 기능을 담당할 runner
    private NetworkRunner _runner;  // 런타임에 Instantiate로 생성해 사용하는 실제 NetworkRunner 인스턴스

    [SerializeField]
    private string _currentSessionName; // 현재 세션 이름
    [SerializeField]
    private bool _isHost;   // 호스트 여부

    /// <summary>
    /// 매칭 및 세션 연결을 시작하는 메서드.
    /// 매칭 결과에 따라 Host 또는 Client로 세션에 연결함.
    /// </summary>
    public async void StartMatching()
    {
        _runner = Instantiate(runnerPrefab);

        await RequestMatching();    // 매칭 요청 및 결과 처리
        await StartSessionAfterMatching();  // 세션 연결
    }

    /// <summary>
    /// 매칭 요청을 처리하는 메서드.
    /// 현재는 매칭 서버 없이 로컬에서 기본 설정을 함.
    /// 이후 서버 연동 예정
    /// </summary>
    /// <returns></returns>
    private async Task RequestMatching()
    {
        // TODO... AI 매칭 요청 보내고 결과 기다리기 - 방 이름과 플레이어가 Host인지 Client인지 여부

        await Task.Delay(1000); // 1초 기다리기 (단위: milliseconds)

        // TODO... 매칭 결과를 _currentSessionName과 _isHost에 반영하기
    }

    /// <summary>
    /// 매칭 결과를 기반으로 세션 연결을 시작하는 메서드.
    /// 세션 연결 후, Host가 수동으로 게임 씬을 로드한다.
    /// </summary>
    /// <returns></returns>
    private async Task StartSessionAfterMatching()
    {
        // 방 이름이 설정되어 있지 않으면 에러 출력
        if (string.IsNullOrEmpty(_currentSessionName))
        {
            Debug.LogError("Session의 이름이 설정되지 않았습니다.");
            return;
        }

        // 세션 연결 시작
        await _runner.StartGame(new StartGameArgs()
        {
            // Host 여부에 따라 모드 결정
            GameMode = _isHost ? GameMode.Host : GameMode.Client,
            // 세션(방) 이름 설정
            SessionName = _currentSessionName,
            Scene = null,
            // 기본 씬 매니저 추가
            // 씬 로딩 및 동기화를 자동으로 지원해줌
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        if (_isHost)    // Host만 게임 플레이 씬을 로딩
            // StartGame 완료 후 수동으로 게임 플레이 씬 로드
            // Single 모드: 기존 씬 제거 후 새 씬 로드
            await _runner.LoadScene("Room_Test_Scene", LoadSceneMode.Single);
    }

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
