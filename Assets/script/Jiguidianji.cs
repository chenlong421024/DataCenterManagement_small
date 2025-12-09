using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jiguidianji : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //添加消息 机柜点击
        Messenger.AddListener(GameEvent.OpenDoor,OpenDoor);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {//计算摄像机和机柜的距离

            Vector3 mousePss = Input.mousePosition;
            //将屏幕位置转换为射线
            /*Ray ray = Camera.main.ScreenPointToRay(mousePss);
            RaycastHit hitInfo;
            bool isCast = Physics.Raycast(ray, out hitInfo);
            Debug.Log(isCast);
            if (isCast)
            {
                Debug.Log(hitInfo.collider.gameObject.name); 
                //gameObject.SetActive(false);
            }*/
            OpenDoor();
        }
    }
    void OpenDoor()
    {
        float dis = Vector3.Distance(this.transform.position, Camera.main.transform.position);
        //Debug.Log(this.transform.position);
        // Debug.Log(Camera.main.transform.position);
        // Debug.Log(dis);
        //transform.Find("backdoor").gameObject.SetActive();
        if (dis < 5)
        {
            transform.Find("backdoor").gameObject.SetActive(false);
            transform.Find("frontdoor").gameObject.SetActive(false);
            //transform.Find("PosID").gameObject.SetActive(true);
        }
        else
        {
            transform.Find("backdoor").gameObject.SetActive(true);
            transform.Find("frontdoor").gameObject.SetActive(true);
            //transform.Find("PosID").gameObject.SetActive(false);
        }
    }
}
