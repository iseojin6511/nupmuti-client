public static class PACKET_TYPE
{
    // Ping
    public const int PING = 1;
    public const int PONG = 2;

    // Room
    public const int ENTER_ROOM = 1001; // 입장하기 버튼, 닉네임 입력란
    public const int LEAVE_ROOM = 1002; // 뒤로 가기 버튼
    public const int CREATE_ROOM = 1004;
    public const int YOU_ARE_HOST = 1005; // 방장 - 게임 시작하기 버튼이 있는 UI
    public const int READY_GAME = 1006; // 시작하기 버튼이 활성화된 UI
    public const int PLAYER_COUNT_CHANGED = 1007; // 참가자 수 업데이트

    // Game
    public const int START_GAME = 1010; // 게임 UI로 넘어감
    public const int END_GAME = 3000; // 완전히 종료됐을 때 필요한 UI

    // Round
    public const int NEW_ROUND = 1019; // 새로운 라운드 시작
    public const int START_ROUND = 1020; // Round1 등등 알아서 구현 ^^
    public const int END_ROUND = 1021; // round 변수 변하면 모든 클라이언트한테 시그널 보내서 round 올라간 UI
    public const int FIRST_ROUND_RULES = 1022; // 1라운드 규칙 알려주는 UI
    public const int SHUFFLE_CARDS = 1023; // 카드 섞는 UI
    public const int DEAL_ONE_CARD = 1024; // 카드를 한장만 나눠주는 애니메이션
    public const int UPDATE_HAND = 1025; // 카드 분배 안내
    public const int YOUR_CARD = 1026; // 무작위로 나에게 분배된 카드를 나타내는 UI
    public const int YOUR_RANK = 1103; // 내 계급 안내
    public const int YOUR_ORDER = 1104; // 내 순서 안내
    public const int ROUND_STARTED = 1105; // 라운드 시작 안내
    public const int DEAL_CARDS = 1106; // 카드 분배 안내
    public const int EXCHANGE_PHASE = 1107; // 카드 교환 단계 안내
    public const int EXCHANGE_INFO = 1108; // 버릴 카드 선택 안내
    public const int EXCHANGE_INFO_2 = 1109; // 버릴 카드 선택 UI
    public const int THROW_SUBMIT = 1110; // 버릴 카드 제출 버튼
    public const int EXCHANGE_DONE = 1111; // 카드 교환 완료 안내
    public const int YOUR_TURN = 1112; // 내 턴임을 알리는 UI
    public const int PLAY_CARD = 1113; // 제출 버튼
    public const int PASS = 1114; // 패스 버튼
    public const int ALL_PASSED = 1115; // 모두 패스했을 때 안내
    public const int END_TURN = 1116; // YOUR_TURN UI 비활성화
    public const int DONE_ROUND = 1028; // 특정 참가자가 카드를 다 내거나, 꼴찌가 됐을 때 안내

    // Exception
    public const int INVALID_CARD = 1030; // 카드 숫자/개수 오류, 제출 성공 안내
    public const int OUT_OF_TURN = 1031; // 현재 턴이 아닐 때 안내
    public const int RETRY_CARD = 1032; // 카드 제출 실패 시 다시 제출 버튼 활성화
    public const int CANT_ENTER_ROOM = 1033; // 방 입장 실패 안내

    // Auth (주석 처리)
    // public const int LOGIN = 4001;
    // public const int CREATE_ACCOUNT = 4000;
}
