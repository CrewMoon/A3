using UnityEngine;

public class HandPositionTracker : MonoBehaviour
{
    public Transform leftHand;
    public Transform rightHand;
    public Transform head;

    public static HandPositionTracker Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public string GetAction()
    {
        if (leftHand && rightHand && head)
        {
            Vector3 worldLeftHandPosition = leftHand.position;
            Vector3 worldRightHandPosition = rightHand.position;
            Vector3 headPosition = head.position;

            Vector3 localLeftHandPosition = head.InverseTransformPoint(worldLeftHandPosition);
            Vector3 localRightHandPosition = head.InverseTransformPoint(worldRightHandPosition);

            string action = CheckAction(localLeftHandPosition, localRightHandPosition, headPosition);
            Debug.Log($"Action: {action}, LeftPos: {localLeftHandPosition}, RightPos: {localRightHandPosition}");
            return action;
        }
        return "No Action";
    }

    public string CheckAction(Vector3 localLeftHandPos, Vector3 localRightHandPos, Vector3 headPos)
    {
        if (headPos.y > 0.4f && headPos.y < 0.7f)
        {
            if ((localLeftHandPos.z > 0.35f || localRightHandPos.z > 0.35f) && localLeftHandPos.x > -0.3f && localRightHandPos.x < 0.3f)
            {
                return "Forward";
            }
            else if (localLeftHandPos.z < -0.3f && localRightHandPos.z < -0.3f && localLeftHandPos.x > -0.5f && localRightHandPos.x < 0.5f)
            {
                return "Back";
            }
            else if (localLeftHandPos.x < -0.6f && localRightHandPos.x < 0.3f)
            {
                return "Left";
            }
            else if (localLeftHandPos.x > -0.3f && localRightHandPos.x > 0.6f)
            {
                return "Right";
            }
        }
        else if (headPos.y < 0.2f)
        {
            return "Squat";
        }
        else if (headPos.y > 0.7f)
        {
            return "Jump";
        }
        
        return "No Action";
    }
}
