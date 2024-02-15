using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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
    public static void InOut(Transform tf, float amount, float minRange, float maxRange)
    {
        float axisVal = Input.GetAxis("Mouse ScrollWheel");
        if (axisVal != 0)
        {
            Vector3 gotoDir = tf.position - Camera.main.transform.position;
            float distance = gotoDir.magnitude;
            gotoDir.Normalize();

            float newDistance = distance + axisVal * amount;
            newDistance = Mathf.Clamp(newDistance, minRange, maxRange);

            Camera.main.transform.position = tf.position - gotoDir * newDistance;
        }
    }

}