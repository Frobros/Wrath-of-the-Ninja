using System;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMath : MonoBehaviour
{
    public static Vector2 Normalize2D(Vector2 vector)
    {
        return vector / vector.magnitude;
    }

    internal static Vector3 PointwiseDivide(Vector3 vector, Vector3 dividedBy)
    {
        return new Vector3(
            vector.x / dividedBy.x,
            vector.y / dividedBy.y,
            vector.z / dividedBy.z
        );
    }

    internal static Vector3 PointwiseMultiply(Vector3 vector, Vector3 multiplyBy)
    {
        return new Vector3(
            vector.x * multiplyBy.x,
            vector.y * multiplyBy.y,
            vector.z * multiplyBy.z
        );
    }

    internal static Vector2 AngleSplit(Vector2 position1, Vector2 position2)
    {
        return Vector2.zero;
    }

    internal static Vector3 RotateTowards(Vector3 current, Vector3 around, float theta)
    {
        return current * Mathf.Cos(theta) + (Vector3.Cross(around, current) * Mathf.Sin(theta) + around * (Vector3.Dot(around, current)) * (1F - Mathf.Cos(theta)));
    }   

    internal static int StringToInt(string s)
    {
        int result = 0;
        for (int i = s.Length; i > 0; i--)
        {
            int value = (int) Char.GetNumericValue(s.ToCharArray()[i-1]);
            result += value * (int) Mathf.Pow(10F, s.Length - i);
        }
        return result;
    }

    internal static Vector2 NormalVector2D(Vector2 vector)
    {
        return new Vector2(-vector.y, vector.x);
    }

    internal static T[] RemoveArrayElement<T>(T[] array, T elementToRemove)
    {
        T[] result = new T[array.Length - 1];
        bool offset = false;
        for(int i = 0; i < array.Length - 1; i++)
        {
            if (array[i] == null || array[i].Equals(elementToRemove))
            {
                Debug.Log("Removed Array Element " + i);
                offset = true;
            } else
            {
                result[i] = array[offset ? i + 1 : i];
            }
        }
        return result;
    }

    internal static T[] AddArrayElement<T>(T[] array, T elementToAdd)
    {
        T[] result = new T[array.Length + 1];
        for (int i = 0; i < array.Length; i++)
           result[i] = array[i];

        result[array.Length] = elementToAdd;
        return result;
    }

    internal static Vector3 ProjectVectorOntoVector(Vector3 a, Vector3 b)
    {
        return Vector3.Dot(a,Vector3.Normalize(b)) * Vector3.Dot(a,a) * Vector3.Normalize(b);
    }

    internal static bool isValueInRange(float value, float from, float to)
    {
        if (from < to)
        {
            return from <= value && value <= to;
        }
        else
        {
            return to <= value && value <= from;
        }
    }

    internal static bool PointInBound(Vector2 point, Vector2 boundCorner1, Vector2 boundCorner2)
    {
        return isValueInRange(point.x, boundCorner1.x, boundCorner2.x)
            && isValueInRange(point.y, boundCorner1.y, boundCorner2.y);
    }

    internal static Vector3 VectorFromAngle (float angle)
    {
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
    {

        Vector3 lineVec3 = linePoint2 - linePoint1;
        Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
        Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

        float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

        //is coplanar, and not parrallel
        if (Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
        {
            float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
            intersection = linePoint1 + (lineVec1 * s);
            return true;
        }
        else
        {
            intersection = Vector3.zero;
            return false;
        }
    }

    internal static bool PointACloserToPointC(Vector2 pointA, Vector2 pointB, Vector2 pointC)
    {
        return (pointA - pointC).sqrMagnitude < (pointB - pointC).sqrMagnitude;
    }
}
