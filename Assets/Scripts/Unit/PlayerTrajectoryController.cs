using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerTrajectoryController : MonoBehaviour
{
    private Player player;
    private Vector3[] lineRendererPositions;
    private LineRenderer lineRenderer;

    public int count { get; private set; }

    private void Awake()
    {
        lineRenderer = GetComponentInChildren<LineRenderer>();
        count = lineRenderer.positionCount;
        lineRendererPositions = new Vector3[count];
    }

    public void EnableLineRenderer()
    {
        lineRenderer.enabled = true;
    }

    public void SetPosition(int index, Vector3 position)
    {
        if (index >= count || index < 0)
        {
            return;
        }
        lineRendererPositions[index] = position;
    }

    public void ApplyPostions()
    {
        lineRenderer.SetPositions(lineRendererPositions);
    }

    public void DisableLineRenderer()
    {
        for (int i = 0; i < count; i++)
        {
            lineRendererPositions[i] = Vector3.zero;
        }
        lineRenderer.SetPositions(lineRendererPositions);
        lineRenderer.enabled = false;
    }
}
