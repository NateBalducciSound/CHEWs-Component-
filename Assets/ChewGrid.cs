using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ChewGrid3D : MonoBehaviour
{
    public Transform player;
    public Vector3 originOffset = Vector3.zero; 
    private BoxCollider box;

    void Awake()
    {
        box = GetComponent<BoxCollider>();
        box.isTrigger = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (ChewsManager.Instance == null)
        {
            Debug.LogError("ChewsManager.Instance is NULL");
            return;
        }

        // bottom-left-front corner for ref
        Vector3 bottomLeftFront = transform.TransformPoint(
            box.center - box.size * 0.5f + originOffset
        );

        Vector3 worldSize = Vector3.Scale(box.size, transform.lossyScale);

        // Player offset from origin of grid
        Vector3 delta = other.transform.position - bottomLeftFront;

        // norm one for scaling
        float xNorm = Mathf.Clamp01(delta.x / worldSize.x);
        float yNorm = Mathf.Clamp01(delta.y / worldSize.y);
        float zNorm = Mathf.Clamp01(delta.z / worldSize.z);

        // Feed into CHEWS
        ChewsManager.Instance.SetWindVolume(xNorm);
        ChewsManager.Instance.SetLPFOpen(yNorm);
        ChewsManager.Instance.SetRainVolume(zNorm);
        ChewsManager.Instance.SetDropDensity(zNorm); 

        
        Debug.Log($"Grid → X:{xNorm:F2}, Y:{yNorm:F2}, Z:{zNorm:F2}");
    }
     private void OnTriggerExit(Collider other)
{
    if (!other.CompareTag("Player")) return;

    Debug.Log("ChewGrid3D -> Player EXITED weather grid");

    if (ChewsManager.Instance != null)
    {
        // Turn off grid params
        ChewsManager.Instance.RestoreBaseState();

        // Also mark outdoors just in case 
        ChewsManager.Instance.SetIndoors(false);
    }
    else
    {
        Debug.LogError("ChewsManager.Instance is NULL");
    }
}

}
