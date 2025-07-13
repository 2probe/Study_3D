using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    [SerializeField] GameObject Cube;

    // Start is called before the first frame update
    void Start()
    {
        //transform.position += new Vector3(2, 0, 3); // 오브젝트 이동
        //float distance = Vector3.Distance(transform.position, Vector3.zero);
        // Vector3.zero : Vector3(0,0,0)와 동일합니다.
        // Vector3.Dsitance(A,B) : A와 B의 사이의 거리를 반환합니다. 
        //Debug.Log($"거리는 : {distance}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
