namespace MNF
{
    public class NetSetting
    {
        public const int NET_HEAD_SIZE = 12;
        public const ushort NET_PROTOCOL_VER = 12;
        public const float NET_WAIT_TIME = 0.25f;
    };


    public class NetConst
    {
        public const int SIZE_USER_TRASNFORM = 4;
        public const int SIZE_OBJECT_TRASNFORM = 3;
        public const int SIZE_USER_DATA = 10;
        public const int SIZE_ROOM_DATA = 10;
    }

    // 패킷 대분류
    public class HeadClass
    {
        public const byte SOCK_MENU = 11;   // 공용 패킷
        public const byte SOCK_WAIT = 12;   // 대기실 패킷
        public const byte SOCK_ROOM = 13;   // 게임룸 패킷
    };

    // 패킷 소분류
    public class HeadEvent
    {
        public const byte WAIT_LOGIN = 11;  // 로그인

        public const byte ROOM_ENTER = 1;   // 방입장
        public const byte ROOM_EXIT = 2;    // 방퇴장
        public const byte ROOM_MAN_IN = 3;  // 유저 입장
        public const byte ROOM_MAN_OUT = 4; // 유저 퇴장
        public const byte ROOM_BROADCAST = 5;    // JSON데이터 전송
        public const byte ROOM_USER_DATA_UPDATE = 6;    // 룸 유저 데이터 변경
        public const byte ROOM_UPDATE = 7;      // 룸 정보 업데이트 - 0.5초 간격
        public const byte ROOM_USER_MOVE = 8;   // 유저 위치정보 변경 - ROOM_UPDATE에서 반영되서 옴
        public const byte ROOM_USER_MOVE_DIRECT = 9;    // 유저위치정보 변경 - 즉시
        public const byte ROOM_USER_ITEM_UPDATE = 10;   // 유저별 아이템 정보 갱신
        public const byte ROOM_DATA_UPDATE = 11;        // 룸 데이터 업데이트

    };

    public class KW_ERROR
    {
        public const ushort ERROR_SUCCESS = 0;
        public const ushort ERROR_DEFAULT = 1;
        public const ushort ERROR_DB_FAIL = 2;
        public const ushort ERROR_DB_EXEC = 3;
    };
}
