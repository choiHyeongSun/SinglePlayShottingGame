using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestCapsule : MonoBehaviour
{
    [SerializeField] private CapsuleCollider capsule;
    private RaycastHit hit;
    

    private void FixedUpdate()
    {
     

    }

    private void OnDrawGizmos()
    {
        if (capsule == null)
        {
            return;
        }

        Vector3 center = capsule.center;
        float halfHeight = capsule.height * 0.5f;
        float radius = capsule.radius;

        Vector3 firstPos = center + transform.position + transform.forward * halfHeight;
        Vector3 secondPos = center + transform.position - transform.forward * halfHeight;

        if (Physics.CapsuleCast(firstPos, secondPos, radius, transform.forward, out hit))
        {
            Gizmos.DrawSphere(hit.point + transform.forward * (halfHeight / 2), capsule.radius);
            Gizmos.DrawSphere(hit.point - transform.forward * (halfHeight / 2), capsule.radius);

        }


    }
    private void DrawCapsuleLines(Vector3 start, Vector3 end, float radius)
    {
        Vector3 forward = (end - start).normalized;
        Vector3 up = Vector3.up;
        if (Vector3.Dot(forward, up) > 0.99f) up = Vector3.right; // 방향이 수직일 경우 예외처리

        Vector3 right = Vector3.Cross(forward, up).normalized * radius;
        up = Vector3.Cross(right, forward).normalized * radius;

        // 4개 지점의 직선 연결
        Gizmos.DrawLine(start + right, end + right);
        Gizmos.DrawLine(start - right, end - right);
        Gizmos.DrawLine(start + up, end + up);
        Gizmos.DrawLine(start - up, end - up);
    }

}
