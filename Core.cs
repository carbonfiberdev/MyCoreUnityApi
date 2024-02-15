using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
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

            Camera.main.transform.position = tf.position - gotoDir * newDistance;
        }
    }
    public static void InOutSmooth(Transform tf, float zoomAmount, float minRange, float maxRange, float smoothness)
    {
        float axisVal = Input.GetAxis("Mouse ScrollWheel");
        if (axisVal == 0) return;

        Func<UniTask> taskToRun = () => InOutSmoothAsync(tf, zoomAmount, minRange, maxRange, smoothness, axisVal);
        Tasks.AddTaskToPool(taskToRun);
        //Tasks.RunAllTasksOneByOne();
    }
    public static async UniTask InOutSmoothAsync(Transform tf, float zoomAmount, float minRange, float maxRange, float smoothness, float axisVal)
    {
        Vector3 gotoDir = tf.position - Camera.main.transform.position;
        float distance = gotoDir.magnitude;
        gotoDir.Normalize();

        float newDistance = distance + axisVal * zoomAmount * 2f;
        newDistance = Mathf.Clamp(newDistance, minRange, maxRange);

        Vector3 targetPosition = tf.position - gotoDir * newDistance;

        while (Vector3.Distance(Camera.main.transform.position, targetPosition) > 0.03f)
        {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetPosition, smoothness * Time.deltaTime);
            await UniTask.DelayFrame(1);
        }
    }
}
public static class Tasks
{
    public static List<Func<UniTask>> tasksToRun = new List<Func<UniTask>>();
    static int maxTaskToRun = 5;
    static bool runningTasks = false;
    public static async UniTask RunAllTasksOneByOne()
    {
        if (runningTasks || tasksToRun.Count == 0) return;

        runningTasks = true;

        while (tasksToRun.Count > 0)
        {
            Func<UniTask> task = tasksToRun[0];
            await task.Invoke();
            tasksToRun.RemoveAt(0);
        }

        runningTasks = false;
    }
    public static void AddTaskToPool(Func<UniTask> task)
    {
        if (tasksToRun.Count >= maxTaskToRun) return;
        tasksToRun.Add(task);

        if (!runningTasks)
        {
            RunAllTasksOneByOne().Forget();
        }
    }
}
