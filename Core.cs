using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public static class Raycast
{
    public static Vector3 ScreenToGroundPos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            return hit.point;
        }
        return Vector3.zero;
    }
    public static string ScreenToGroundName()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            return hit.transform.name;
        }
        return null;
    }
    public static GameObject ScreenToGroundObj()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            return hit.transform.gameObject;
        }
        return null;
    }
    public static Transform ScreenToGroundTransform()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            return hit.transform;
        }
        return null;
    }
}
public static class LookAt
{
    public static void Position(Transform tf, Vector3 position)
    {
        tf.LookAt(position, Vector3.up);
    }
    public static void PositionSmooth(Transform tf, Vector3 position, float speed, float accelerationFactor)
    {
        Vector3 lookDirection = position - tf.position;
        lookDirection.Normalize();

        float acceleration = 1 + Vector3.Distance(tf.position, position) * accelerationFactor;

        tf.rotation = Quaternion.Slerp(tf.rotation, Quaternion.LookRotation(lookDirection), speed*acceleration * Time.deltaTime);
    }
}
public static class Zoom
{
    public static Vector3 gotoPos = Camera.main.transform.position;
    public static void InOut(Transform tf, float zoomAmount, float minRange, float maxRange)
    {
        float axisVal = Input.GetAxis("Mouse ScrollWheel");
        if (axisVal != 0)
        {
            Vector3 gotoDir = tf.position - Camera.main.transform.position;
            float distance = gotoDir.magnitude;
            gotoDir.Normalize();

            float newDistance = distance + axisVal * zoomAmount;
            newDistance = Mathf.Clamp(newDistance, minRange, maxRange);
            gotoPos = tf.position - gotoDir * newDistance;
        }
    }
}
public static class Cam
{
    public static async UniTaskVoid CamFollow()
    {
        while (true)
        {
            await UniTask.DelayFrame(1);
            Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, Zoom.gotoPos, Time.deltaTime*7f);
        }
    }
}
public static class Job
{
    private static List<Action> ActionList = new List<Action>();
    public static void AddJobToQueue(Action act)
    {
        ActionList.Add(act);
    }
    public static void RemoveJobFromQueue(Action act)
    {
        ActionList.Remove(act);
    }
    public static void ClearAllJobs()
    {
        ActionList.Clear();
    }
    public static async UniTaskVoid ExecuteAllJobs(int miliseconds)
    {
        foreach (Action act in ActionList)
        {
            act();
            await UniTask.Delay(miliseconds);
        }
    }
}
