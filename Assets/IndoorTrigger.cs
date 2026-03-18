using UnityEngine;

public class IndoorTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("IndoorTrigger -> Player ENTERED indoor area");

            if (ChewsManager.Instance != null)
                ChewsManager.Instance.SetIndoors(true);
            else
                Debug.LogError("IndoorTrigger: ChewsManager.Instance is NULL");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("IndoorTrigger -> Player EXITED indoor area");

            if (ChewsManager.Instance != null)
                ChewsManager.Instance.SetIndoors(false);
            else
                Debug.LogError("IndoorTrigger: ChewsManager.Instance is NULL");
        }
    }
}
