using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class FuwuqiDianji : MonoBehaviour
{
    LookAt la = new LookAt();
    // Start is called before the first frame update
    void Start()
    {
        //UnityAction<BaseEventData> click = new UnityAction<BaseEventData>(MyClick); 
        //EventTrigger.Entry myclick = new EventTrigger.Entry();
        //myclick.eventID = EventTriggerType.PointerClick;
        //myclick.callback.AddListener(click);
        //EventTrigger trigger = gameObject.AddComponent<EventTrigger>(); 
        //trigger.triggers.Add(myclick);
    }

    // Update is called once per frame
    public  void MyClick(BaseEventData eventData)
    {
        PointerEventData e = (PointerEventData)eventData;
        if (e.button == PointerEventData.InputButton.Left)
        {
            //Debug.Log("left");
            //if (Input.GetMouseButton(1))
            string[] strs = this.name.Split('_');
            if (strs.Length < 3)
                return;
            string t_jigui = strs[2];
            string t_u = strs[3];
            Vector3 vc = la.getPos(t_jigui, t_u);
            int row = DoCSVSQL.Instance.GetRow(t_jigui, t_u);
            //Debug.Log(row);
            //tr.LookAt(vc);
            Vector3 v_old = Camera.main.transform.position;
            Vector3 v_new = vc;
            v_new.x += 1;
           // Debug.Log(Vector3.Distance(v_new, v_old));
            Messenger.Broadcast<ShowMes>(GameEvent.Show,new ShowMes(UIMenu.PanelInfo,row));
            Messenger.Broadcast<PointsMes>(GameEvent.MoveCreama, new PointsMes(v_new, v_old));
           // Debug.Log(this.name);
        }
    }
}
