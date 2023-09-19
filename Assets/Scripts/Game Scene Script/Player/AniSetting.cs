using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//제작자 방민호
namespace mino 
{
    public enum PlayerState
    {
        Idle,   //0 가만히 있는 상태 지정할거
        RunForward,   //1 마우스로 클릭해서 이동할때 실행할 애니메이션
        LongAttack, //2 원거리 무기 공격모션 애니메이션
        GunAttackIdle, //3 무기: 총 공격 애니메이션
        BowAttackIdle,  //4 무기 : 활 공격 애니메이션
        ShortAttack,   //5  근접 무기 애니메이션 지정 
        HammerAttackIdle,   //6 무기 : 해머 공격 애니메이션
        Dead, //7 죽었을때
        SpaceMove, //8 스페이스바를 눌렀을 때 나가게 할 애니메이션
        ItemUse, //9 아이템을 사용중일때 실행시킬 애니메이션 
        GetHit //10 맞았을때 나오게할 애니메이션
    }

    public class AniSetting : MonoBehaviour
    {
        
        public Animator ani;

        private static readonly int AniPlayer = Animator.StringToHash("aniPlayer");

        // Start is called before the first frame update
        void Start()
        {
            ani = GetComponent<Animator>();
        }
        //enum 값에 따라서 행동하게 할거임
        public void ChangeAnimation(PlayerState aniNumber)
        {
            //에니메이션의 값을 aniPlayer에서 번호에 따라서 가져오게 할것임!
            ani.SetInteger(AniPlayer, (int)aniNumber);
        }
    }
}



