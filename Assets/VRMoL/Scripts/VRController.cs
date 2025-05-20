using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VRController : MonoBehaviour
{
    [SerializeField] private XRController controller;
    [SerializeField] private LineRenderer teleportLine;
    [SerializeField] private LayerMask teleportLayerMask;
    
    private bool isTeleporting = false;
    private Vector3 teleportPoint;
    
    private void Start()
    {
        // テレポート用のLineRendererを設定
        if (teleportLine == null)
        {
            teleportLine = gameObject.AddComponent<LineRenderer>();
            teleportLine.startWidth = 0.02f;
            teleportLine.endWidth = 0.02f;
            teleportLine.material = new Material(Shader.Find("Sprites/Default"));
            teleportLine.startColor = Color.blue;
            teleportLine.endColor = Color.blue;
            teleportLine.enabled = false;
        }
    }
    
    private void Update()
    {
        // テレポートの処理
        if (controller.activateAction.action.ReadValue<float>() > 0.1f)
        {
            if (!isTeleporting)
            {
                isTeleporting = true;
                teleportLine.enabled = true;
            }
            
            // レイキャストでテレポート位置を確認
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 100f, teleportLayerMask))
            {
                teleportPoint = hit.point;
                teleportLine.SetPosition(0, transform.position);
                teleportLine.SetPosition(1, teleportPoint);
            }
        }
        else if (isTeleporting)
        {
            isTeleporting = false;
            teleportLine.enabled = false;
            
            // テレポート実行
            if (teleportPoint != Vector3.zero)
            {
                Teleport();
            }
        }
    }
    
    private void Teleport()
    {
        // カメラリグの位置を更新
        Vector3 difference = Camera.main.transform.position - transform.position;
        difference.y = 0;
        Camera.main.transform.parent.position = teleportPoint - difference;
        
        // ログに記録
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GetComponent<Logger>().LogEvent("Teleport");
        }
    }
} 