using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateController : MonoBehaviour
{


    //제작한 이유 : bool 형식으로 관리를 했지만 관리가 어려웠던 일이 있었습니다.
    //C++로 개인 프로젝트를 만들던 중 "상태관리를 Smart pointer 처럼 레퍼런스 카운트처럼 만들면 편하겠는데"
    //라는 생각이 들어 상태관리를 Count 하는 형식으로 제작하게 되었습니다.
    private int donMoveCount;
    private int donFireCount;
    private int donRollingCount;
    private int donPickupCount;
    private int donTurnCount;
    private int donChangeWeaponCount;
    
    public bool canMove => donMoveCount == 0;
    public bool canFire => donFireCount == 0;
    public bool canRolling=> donRollingCount == 0;
    public bool canPickup => donPickupCount == 0;
    public bool canTurn => donTurnCount == 0;
    public bool canChangeWeapon => donChangeWeaponCount == 0;

    public void BeginRolling()
    {
        donRollingCount++;
        donMoveCount++;
        donFireCount++;
        donPickupCount++;
        donTurnCount++;
        donChangeWeaponCount++;
    }

    public void EndRolling()
    {
        donRollingCount--;
        donMoveCount--;
        donFireCount--;
        donPickupCount--;
        donTurnCount--;
        donChangeWeaponCount--;
    }

    public void BeginThrow()
    {
        donChangeWeaponCount++;
    }

    public void EndThrow()
    {
        donChangeWeaponCount--;
    }
}
