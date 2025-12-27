using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Testparabola : MonoBehaviour
{
    [SerializeField] private float power;
    [SerializeField] private Transform target;
    [SerializeField] private GameObject projectile;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 diff = target.transform.position - transform.position;
            float x = new Vector3(diff.x, 0.0f, diff.z).magnitude; // 수평 거리
            float y = diff.y; // 높이 차이
            float v = power;
            float g = Mathf.Abs(Physics.gravity.y);

            float? theta = CalcTheta(v, g, x, y);

            if (theta.HasValue)
            {
                // 1. 수평 방향 벡터 (Y축은 0)
                Vector3 planarDirection = new Vector3(diff.x, 0, diff.z).normalized;

                // 2. 계산된 각도를 벡터로 변환 (공식: x=cos, y=sin)
                // 수평 방향으로 cos만큼, 수직(Up) 방향으로 sin만큼 힘을 배분
                Vector3 launchVelocity = (planarDirection * Mathf.Cos(theta.Value) + Vector3.up * Mathf.Sin(theta.Value)) * v;

                GameObject game = Instantiate(projectile, transform.position, Quaternion.identity);
                Rigidbody rigid = game.GetComponent<Rigidbody>();
                rigid.velocity = launchVelocity;
            }
        }
    }

    protected float? CalcTheta(float v, float g, float x, float y)
    {
        float v2 = v * v;
        float v4 = v2 * v2;

        // 판별식 계산
        float discriminant = v4 - g * (g * x * x + 2 * y * v2);

        if (discriminant < 0)
        {
            Debug.LogWarning("궤적 범위에 넘어 갑니다. (도달 불가능)");
            return null;
        }

        // 저각(Low Ball)을 구하려면 아래처럼 - 연산자를 사용합니다.
        float tanTheta = (v2 - Mathf.Sqrt(discriminant)) / (g * x);
        return Mathf.Atan(tanTheta);
    }

    private float getTheata(float v, float y, float x)
    {
        float g = -Physics.gravity.y;
        float ptan = v * v - MathF.Sqrt(MathF.Pow(v, 4) - g * (2 * y * v * v + g * x * x));

        return MathF.Atan(ptan / (g * x));
    }
}
