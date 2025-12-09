using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssetsLoad : MonoBehaviour
{
    int row = 8;//ABCDEFG
    int col = 7;//0-7
    public Vector3 firstPos;
    float rowSize;
    float colSize;
    float rowStep;
    float colStep;
    DoCSVSQL doCSV;
    static Vector3 vc_firstJIguiP0s;
    //public static Vector3 get_vc_firstJIguiP0s
    //{
    //    get
    //    {
    //        return vc_firstJIguiP0s;
    //    }
         
    //}
    // Start is called before the first frame update
    void Start()
    {
        LoadJiGui();
    }

    
    Vector3 getPos(int r,int c)
     {
        Vector3 p ;
        p = new Vector3(-r*rowStep, 0, c*colStep);
        p = p + firstPos;
        if (r == 0 && c == 0)
        {
            vc_firstJIguiP0s = p;
        }
        return p;
      }
    /// <summary>
    /// 
    /// </summary>
    void LoadJiGui()
    {
        //服务器 r l u 
        rowSize = 22f;
        colSize = 8.6f;
        rowStep = rowSize / (row + 1);
        colStep = colSize / (col + 1) - 0.2f;
        firstPos = new Vector3(11f - rowStep, -0.3f, -4f + 1.5f * colStep);
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                GameObject jigui = Instantiate(Resources.Load("Prefabs/jigui")) as GameObject;
                jigui.name = "jigui" + "_" + i + "_" + j;
                jigui.transform.position = getPos(i, j);
                //jigui.transform.Rotate(Vector3.up * 180 * (i % 2 == 0 ? 0 : 1));
                char c = (char)('H' - i);
                var textComp = jigui.transform.Find("PosID").gameObject.GetComponent<TextMesh>();
                if (textComp)
                    textComp.text = c + (j + 1).ToString();
                //textComp.text = c + (j + 1).ToString();GetComponent(TextMesh).text = "Hello World";
            }
        }

    }
}
