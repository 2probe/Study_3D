using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Rigidbody rigid;
    [SerializeField] Transform character;
    [SerializeField] Animator anicon;
    [SerializeField] float moveSpeed; // 이동 속도
    [SerializeField] int attackRange;
    [SerializeField] int attackAngle;

    Vector2 moveInput; // 입력받은 이동 방향이 저장될 공간

    void Update()
    {
        Move();
        Jump();
        Attack();
    }

    void Move()
    {
        // 입력
        Vector2 rawInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        moveInput.x = Mathf.MoveTowards(moveInput.x, rawInput.x, Time.deltaTime * 10);
        moveInput.y = Mathf.MoveTowards(moveInput.y, rawInput.y, Time.deltaTime * 10);
        float moveValue = moveInput.magnitude;

        // 이동
        if (moveValue != 0)
        {
            Vector3 inputForward = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
            rigid.MovePosition(transform.position + (inputForward * Time.deltaTime * moveSpeed));

            if (moveInput != Vector2.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(inputForward);
                character.rotation = Quaternion.Slerp(character.rotation, targetRotation, Time.deltaTime * 10f);
            }
        }

        // 애니메이션
        anicon.SetBool("ISWALK", moveValue !=0);
    }

    public float jumpPower; // 점프력
    public int MaxJumpCount; // 최대 점프 횟수
    [SerializeField] int nowJumpCount; // 현재 점프 횟수

    void Awake()
    {
        nowJumpCount = MaxJumpCount;
    }

    void Jump()
    {
        // Space 키가 눌린다 + jumpCount가 0보다 크다 => 점프한다.
        if (Input.GetKeyDown(KeyCode.Space) && 0 < nowJumpCount)
        {
            rigid.velocity = Vector3.up * jumpPower;
            nowJumpCount--;
        }

        // [ rigid.velocity.y <= 0 ]
        // 점프하여 물체가 올라갈때는 오브젝트의 물리적인 속도(velocity)가 0보다 클 것입니다.
        // 반대로 물리적인 속도가 0보다 작다면, 정지하거나 떨어지고 있는 상황일 것입니다.

        // [ Physics.Raycast(character.position, Vector3.down, 0.1f, LayerNumber.Ground) ]
        // Raycast는 보이지 않는 빛(Ray)을 투사해 빛이 닿는 부분을 파악합니다.

        // Physics.Raycast는
        // - character.position + (Vector3.up * 0.1f) : 시작점이 Collider와 겹치지 않도록 0.1f 만큼 위를 설정합니다.
        // - Vector3.down : 아래 방향으로
        // - 0.2f : 0.2f 만큼의 크기만큼 탐지하였을 때
        // - LayerNumber.Ground : 땅이 닿는다면
        // True를 반환합니다.

        // 결국 떨어지거나 정지한 상태 + 땅과 캐릭터 거리가 0.1f 이하면 => 점프 횟수를 초기화합니다.
        if (rigid.velocity.y <= 0 && Physics.Raycast(character.position + (Vector3.up * 0.1f), Vector3.down, 0.2f, LayerMask.GetMask("Ground")))
        {
            nowJumpCount = MaxJumpCount;
        }
    }

    void Attack()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            anicon.SetTrigger("ATTACK");

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);

            foreach (Collider collider in hitColliders)
            {
                Monster monster = collider.GetComponent<Monster>();
                if (monster != null)
                {
                    Vector3 directionToTarget = (monster.transform.position - transform.position).normalized;
                    float dot = Vector3.Dot(transform.forward, directionToTarget);

                    float angleThreshold = Mathf.Cos(attackAngle * 0.5f * Mathf.Deg2Rad);

                    if (dot >= angleThreshold)
                    {
                        // 범위 내 몬스터에게 피해
                        monster.Damaged();
                    }
                }
            }
        }
    }

    // 공격 범위 시각화 (Scene 뷰에서만 보임)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Vector3 forward = transform.forward;
        Quaternion leftRotation = Quaternion.Euler(0, -attackAngle / 2, 0);
        Quaternion rightRotation = Quaternion.Euler(0, attackAngle / 2, 0);

        Vector3 leftDirection = leftRotation * forward;
        Vector3 rightDirection = rightRotation * forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + leftDirection * attackRange);
        Gizmos.DrawLine(transform.position, transform.position + rightDirection * attackRange);
    }
}
