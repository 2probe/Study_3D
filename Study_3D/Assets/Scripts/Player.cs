using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Rigidbody rigid;
    [SerializeField] Transform character;
    [SerializeField] Animator anicon;
    [SerializeField] float moveSpeed; // �̵� �ӵ�
    [SerializeField] int attackRange;
    [SerializeField] int attackAngle;

    Vector2 moveInput; // �Է¹��� �̵� ������ ����� ����

    void Update()
    {
        Move();
        Jump();
        Attack();
    }

    void Move()
    {
        // �Է�
        Vector2 rawInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        moveInput.x = Mathf.MoveTowards(moveInput.x, rawInput.x, Time.deltaTime * 10);
        moveInput.y = Mathf.MoveTowards(moveInput.y, rawInput.y, Time.deltaTime * 10);
        float moveValue = moveInput.magnitude;

        // �̵�
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

        // �ִϸ��̼�
        anicon.SetBool("ISWALK", moveValue !=0);
    }

    public float jumpPower; // ������
    public int MaxJumpCount; // �ִ� ���� Ƚ��
    [SerializeField] int nowJumpCount; // ���� ���� Ƚ��

    void Awake()
    {
        nowJumpCount = MaxJumpCount;
    }

    void Jump()
    {
        // Space Ű�� ������ + jumpCount�� 0���� ũ�� => �����Ѵ�.
        if (Input.GetKeyDown(KeyCode.Space) && 0 < nowJumpCount)
        {
            rigid.velocity = Vector3.up * jumpPower;
            nowJumpCount--;
        }

        // [ rigid.velocity.y <= 0 ]
        // �����Ͽ� ��ü�� �ö󰥶��� ������Ʈ�� �������� �ӵ�(velocity)�� 0���� Ŭ ���Դϴ�.
        // �ݴ�� �������� �ӵ��� 0���� �۴ٸ�, �����ϰų� �������� �ִ� ��Ȳ�� ���Դϴ�.

        // [ Physics.Raycast(character.position, Vector3.down, 0.1f, LayerNumber.Ground) ]
        // Raycast�� ������ �ʴ� ��(Ray)�� ������ ���� ��� �κ��� �ľ��մϴ�.

        // Physics.Raycast��
        // - character.position + (Vector3.up * 0.1f) : �������� Collider�� ��ġ�� �ʵ��� 0.1f ��ŭ ���� �����մϴ�.
        // - Vector3.down : �Ʒ� ��������
        // - 0.2f : 0.2f ��ŭ�� ũ�⸸ŭ Ž���Ͽ��� ��
        // - LayerNumber.Ground : ���� ��´ٸ�
        // True�� ��ȯ�մϴ�.

        // �ᱹ �������ų� ������ ���� + ���� ĳ���� �Ÿ��� 0.1f ���ϸ� => ���� Ƚ���� �ʱ�ȭ�մϴ�.
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
                        // ���� �� ���Ϳ��� ����
                        monster.Damaged();
                    }
                }
            }
        }
    }

    // ���� ���� �ð�ȭ (Scene �信���� ����)
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
