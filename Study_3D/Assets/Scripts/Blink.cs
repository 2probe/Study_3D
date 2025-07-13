using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : MonoBehaviour
{
    [SerializeField] GameObject Cube;



    private void OnEnable() // 오브젝트가 켜질때마다
    {
        transform.position = new Vector3(Random.Range(0f, 10f), 0f, Random.Range(0f, 10f)); // 랜덤한 위치로 이동    

    }
}
