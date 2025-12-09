using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 视角移动
/// </summary>
public class FollowMouseMove : MonoBehaviour
{
    public float rSpeed = 0.5f;
    //public Transform obj;
    public Vector3 targetPos;
    public float movespeed=2;
    public float dis = 0;
    bool isListServer = false;
    bool isNewPoint = false;
    Vector3 new_point1;
    Vector3 new_point2;
    Vector3 new_point3;
    Vector3 old_point;
    Quaternion qr ;
    //public Vector3 mousePointAt ;
    // Start is called before the first frame update
    void Start()
    {
        new_point1=new_point2=new_point3 = old_point = Camera.main.transform.position;
        //targetPos = Camera.main.transform.position;
        Messenger.AddListener<PointsMes>(GameEvent.MoveCreama, GoToLookAt);
        
    }
    void Awake()
    {
        targetPos = Camera.main.transform.position;
        movespeed = 2;
        qr = Quaternion.Euler(0, -90, 0);
    }
    // Update is called once per frame
    private void FixedUpdate()
    {
        // 获得鼠标当前位置的X和Y
        isListServer = GameObject.Find("Canvas").transform.Find("ScrollView").gameObject.activeSelf;
        if (!isListServer)
        {
            if (Input.GetMouseButton(1))
            {
                float mouseX = Input.GetAxis("Mouse X") * rSpeed;
                float mouseY = Input.GetAxis("Mouse Y") * rSpeed;
                // 鼠标在Y轴上的移动号转为摄像机的上下运动，即是绕着X轴反向旋转
                Camera.main.transform.localRotation = Camera.main.transform.localRotation * Quaternion.Euler(-mouseY, 0, 0);
                //Camera.main.transform.rotation = Camera.main.transform.localRotation * Quaternion.Euler(-mouseY, 0, 0);
                //Camera.main.transform.rotation = Camera.main.transform.localRotation * Quaternion.Euler(-0, mouseX, 0);
                // 鼠标在X轴上的移动转为主角左右的移动，同时带动其子物体摄像机的左右移动
                Camera.main.transform.localRotation = transform.localRotation * Quaternion.Euler(0, mouseX, 0);
                if (transform.localEulerAngles.z != 0)
                {
                    float rotx = transform.localEulerAngles.x;
                    float rotY = transform.localEulerAngles.y;
                    transform.localEulerAngles = new Vector3(rotx, rotY, 0);
                }

            }
            //鼠标滚轮的效果
            //Camera.main.fieldOfView 摄像机的视野
            //Camera.main.orthographicSize 摄像机的正交投影
            //Zoom out
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                if (Camera.main.fieldOfView <= 120)
                    Camera.main.fieldOfView += 2;
                if (Camera.main.orthographicSize <= 10)
                    Camera.main.orthographicSize += 0.5F;
            }
            //Zoom in
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                if (Camera.main.fieldOfView > 2)
                    Camera.main.fieldOfView -= 2;
                if (Camera.main.orthographicSize >= 1)
                    Camera.main.orthographicSize -= 0.5F;
            }
            /*if (Input.GetMouseButtonDown(0))
            {

                Vector3 mousePss = Input.mousePosition;
                //将屏幕位置转换为射线
                Ray ray = Camera.main.ScreenPointToRay(mousePss);
                RaycastHit hitInfo;
                bool isCast = Physics.Raycast(ray, out hitInfo);
                if (isCast)
                {
                    movespeed = 10;
                    targetPos = hitInfo.point;
                    dis = Vector3.Distance(Camera.main.transform.position, targetPos);
                    if (dis > 1.8 && dis < 2)
                        dis = -1;
                }
            }*/
            MoveTo();
        }
        if (isNewPoint)
        {
            moveCream();
        }
        
    }

    //<returns></returns>x -16 16 y 0 16 z -4 4
    /// <summary>
    /// 位置调整
    /// </summary>
    void PosAdjust()
    {
        if (transform.position.x < -14)
            transform.position=new Vector3(-14, transform.position.y, transform.position.z);
        if (transform.position.x > 16)
            transform.position = new Vector3(16, transform.position.y, transform.position.z);
        if (transform.position.y < -2)
            transform.position = new Vector3(transform.position.x, -1, transform.position.z);
        if (transform.position.y > 16)
            transform.position = new Vector3(transform.position.x, 16, transform.position.z);
        if (transform.position.z < -4)
            transform.position = new Vector3(transform.position.x, transform.position.y, -4);
        if (transform.position.z > 4)
            transform.position = new Vector3(transform.position.x, transform.position.y, 4);
    }
    void MoveTo()
    {
        
        if (Input.GetKey(KeyCode.W))
        {
            Camera.main.transform.transform.Translate(new Vector3(0, 0, movespeed * Time.deltaTime));
        }
        //s键后退
        if (Input.GetKey(KeyCode.S))
        {
            Camera.main.transform.Translate(new Vector3(0, 0, -movespeed * Time.deltaTime));
        }
        //a键后退
        if (Input.GetKey(KeyCode.A)|| Input.GetKey(KeyCode.LeftArrow))
        {
            Camera.main.transform.Translate(new Vector3(-movespeed* Time.deltaTime, 0, 0 ));
        }
        //d键后退
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            Camera.main.transform.Translate(new Vector3(movespeed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            if (Camera.main.transform.position.y < 10)
                Camera.main.transform.Translate(new Vector3( 0,movespeed * Time.deltaTime, 0));
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            if(Camera.main.transform.position.y>-2)
            Camera.main.transform.Translate(new Vector3(0,-movespeed * Time.deltaTime,  0));
        }
        // Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, targetPos,movespeed * Time.deltaTime);
        //Debug.Log(Camera.main.transform.position + " :" + Camera.main.transform.position);
        //Debug.Log(movespeed.ToString ());
        PosAdjust();
    }
    void GoToLookAt(PointsMes  pm)
    {
        //Debug.Log("sdddddddddddd");
        //Debug.Log(pm.v_new+"  "+pm.v_old);
        if (Vector3.Distance(pm.v_new, pm.v_old) > 0.1)
        {
            //Camera.main.transform.position.Set(pm.v_new.x, pm.v_new.y,pm.v_new.z);
            //LineRenderer.Instantiate()
            //判断一下z 和 y 的 距离 

            if (Math.Abs(pm.v_new.x - pm.v_old.x) > 1)
            {
                old_point = pm.v_old;
                new_point1 = new Vector3(pm.v_old.x, 2, pm.v_old.z);
                new_point2 = new Vector3(pm.v_new.x, 2, pm.v_new.z);
                new_point3 = pm.v_new;
            }
            else 
            {
                old_point = pm.v_old;
                new_point1 = new Vector3(pm.v_new.x, pm.v_new.y, pm.v_old.z);
                new_point2 = new Vector3(pm.v_new.x, pm.v_new.y, pm.v_new.z);
                new_point3 = pm.v_new;
            }
            Camera.main.transform.rotation = Quaternion.Euler(new Vector3(25, -90, 0));
            isNewPoint = true ;
        }
    }
    void moveCream()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(0))
            isNewPoint = false;//鼠标按下停止
        if (Vector3.Distance(new_point1, old_point) > 0.2)
        {

            //Camera.main.transform.transform.Translate((new_point - old_point)/10f * Time.deltaTime*movespeed*1.5f);
            //old_point = Camera.main.transform.transform.position;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, new_point1, movespeed * Time.deltaTime);
            old_point = Camera.main.transform.transform.position;
        }
        else if (Vector3.Distance(new_point1, new_point2) > 0.2)
        {

            //Camera.main.transform.transform.Translate((new_point - old_point)/10f * Time.deltaTime*movespeed*1.5f);
            //old_point = Camera.main.transform.transform.position;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, new_point2, movespeed * Time.deltaTime);
            old_point= new_point1 = Camera.main.transform.transform.position;
        }
        else if(Vector3.Distance(new_point2, new_point3) > 0.02)
        {

            //Camera.main.transform.transform.Translate((new_point - old_point)/10f * Time.deltaTime*movespeed*1.5f);
            //old_point = Camera.main.transform.transform.position;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, new_point3, movespeed * Time.deltaTime/3f);
            old_point = new_point1=new_point2 = Camera.main.transform.transform.position;
        }
        else if (transform.rotation!= qr)//
        {
           
            //Camera.main.transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
            transform.rotation = Quaternion.Slerp(transform.rotation, qr, 2 * Time.deltaTime);
           
        }
        else
        {
            isNewPoint = false;
            old_point = new_point2 = new_point3 = Camera.main.transform.transform.position;
            Camera.main.transform.transform.LookAt(new_point3);
            Camera.main.transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
            //transform.rotation = Quaternion.Slerp(transform.rotation, qr, 2 * Time.deltaTime);
            Messenger.Broadcast(GameEvent.OpenDoor);
            //Debug.Log(GameEvent.OpenDoor);
        }
    }
}
