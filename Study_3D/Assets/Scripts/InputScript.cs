using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputScript : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] Animator AniCon_Pico;

    // Update is called once per frame
    void Update()
    {
        // 입력
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        Vector3 moveDirection = new Vector3(moveX, 0, moveZ).normalized;

        // 이동
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // 회전
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        // 공격
        Input.GetMouseButtonDown(0);// GetMouseButtonDown(0) : 마우스 왼쪽 버튼을 눌렀을 때 True를 반환합니다.
        
        bool isAttack = Input.GetMouseButtonDown(0);// GetMouseButtonDown(0) : 마우스 왼쪽 버튼을 눌렀을 때 True를 반환합니다.

        AniCon_Pico.SetBool("ISATTACK", isAttack);

        // 점프
        Input.GetKeyDown(KeyCode.Space);
        
        bool isJump = Input.GetKeyDown(KeyCode.Space);

        AniCon_Pico.SetBool("ISJUMP", isJump);

        //댄스
        Input.GetKeyDown(KeyCode.T);

        bool isTwerk = Input.GetKeyDown(KeyCode.T);
        AniCon_Pico.SetBool("ISTWERK", isTwerk);





        // 애니메이터
        bool isWalk = 0 < moveDirection.magnitude;
        // moveDirection.magnitude : 백터의 길이를 반환합니다.
        // 입력 값을 받으면 백터의 길이가 0보다 커지면서 True를 반환합니다.
        AniCon_Pico.SetBool("ISWALK", isWalk);
        // anicon_PicoChan이라는 애니메이터를 담을 변수를 생성합니다.
        // Bool 타입의 Parameter를 생성하였기에 SetBool함수를 사용합니다.


    }
}
