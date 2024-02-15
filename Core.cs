using Cysharp.Threading.Tasks;
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
    private static bool zooming = false;
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
    public static async UniTaskVoid InOutSmooth(Transform tf, float zoomAmount, float minRange, float maxRange, float smoothness)
    {
        if (zooming) return;

        float axisVal = Input.GetAxis("Mouse ScrollWheel");
        if (axisVal != 0)
        {
            zooming = true;

            Vector3 gotoDir = tf.position - Camera.main.transform.position;
            float distance = gotoDir.magnitude;
            gotoDir.Normalize();

            float newDistance = distance + axisVal * zoomAmount*2f;
            newDistance = Mathf.Clamp(newDistance, minRange, maxRange);

            Vector3 targetPosition = tf.position - gotoDir * newDistance;

            while (Vector3.Distance(Camera.main.transform.position, targetPosition) > 0.03f)
            {
                Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetPosition, smoothness * Time.deltaTime);
                await UniTask.DelayFrame(1);
            }
            zooming = false;
        }
    }
}
