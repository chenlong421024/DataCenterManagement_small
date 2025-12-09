using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt
{
    Vector3 firstPos;//8.6 -1.26 -2.52      jigui x 1.227  z 0.815
    Vector3 lastPos;//-8.64 0.69 2.64
    public LookAt()
    {
        firstPos = new Vector3(-8.64f, -1.26f, -2.52f);//a1-1u   - - -
        lastPos = new Vector3(8.6f, 0.69f, 2.64f);//h7-40u + + +
    }
    public Vector3 getPos(string r, string c)//r = A01  c= 20u
    {
        float fx, fy, fz;
        char cx = r[0];//(char)r.Substring(0, 1);//a03
        char cz1 = '0', cz2 = '0';
        if (r.Length == 2)
            cz1 = r[1];//(char)r.Substring(0, 1);
        else if (r.Length == 3)
        {
            cz1 = r[1];
            cz2 = r[2];
        }
        int iy = 0;
        float xsetp = (lastPos.x - firstPos.x) / 7;
        float ysetp = (lastPos.y - firstPos.y) / 39;
        float zsetp = (lastPos.z - firstPos.z) / 6;
        if (cx >= 'a' && cx <= 'h')
        {
            fx = firstPos.x + (cx - 'a') * xsetp;
        }
        else
        {
            fx = firstPos.x + (cx - 'A') * xsetp;
        }

        string str = c.Split('-')[0];//40u 34 7u
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] == 'U' || str[i] == 'u')
                break;
            iy = iy * 10 + (str[i] - '0');
        }
        fy = firstPos.y + iy * ysetp;

        char cz = (cz1 != '0') ? cz1 : cz2;
        fz = firstPos.z + (cz - '1') * zsetp;//03

        return new Vector3(fx, fy, fz);
    }
}
