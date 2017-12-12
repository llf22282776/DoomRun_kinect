using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;

public class PositionManager : MonoBehaviour {
    public KinectSensor kinect;
    public BodyFrameReader bodyFrameReader;//身体框架读入器
    private Body[] bonesData = null;
    private bool opened;
    private static PositionManager positionManager;

    // Use this for initialization
    void Start () {
        startKinect();
	}
	
	// Update is called once per frame
	void Update () {
        //不停的读取kincet，获得骨骼信息，保存在body里面
        if (bodyFrameReader != null)
        {
            var frame = bodyFrameReader.AcquireLatestFrame();
            //Debug.Log("frame == null:" + (frame == null));
            if (frame != null)
            {
                //Debug.Log(" before bonesData == null:" + (bonesData == null));
                if (bonesData == null)
                {
                 //   Debug.Log("already new!!!");
                    bonesData = new Body[kinect.BodyFrameSource.BodyCount];
                }

                frame.GetAndRefreshBodyData(bonesData);
                frame.Dispose();//销毁掉它
                frame = null;
               // bonesData = null;
                
               // Debug.Log("after bonesData == null:" + (bonesData == null));
            }
           
        }




    }
    void OnApplicationQuit()
    {
        stopKinect();
       

    }
    public void stopKinect() {
        if (bodyFrameReader != null) bodyFrameReader.Dispose();
        if (kinect != null && kinect.IsOpen) kinect.Close();
        kinect = null;
        bodyFrameReader = null;
        bonesData = null;
        positionManager = null;

    }
    public void startKinect() {
        stopKinect();//关闭先
        kinect = KinectSensor.GetDefault();//获得默认kinect

        if (kinect != null)
        {
            bodyFrameReader = kinect.BodyFrameSource.OpenReader();
            if (bodyFrameReader == null) opened = false;//体感模式没打开
            else {
                positionManager = this;
                if (kinect.IsOpen == false) kinect.Open();
             //   Debug.Log("opened!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                opened = true;
            }//
        }//打开一个读入流
        else
        {
            opened = false;
        }

    }
    public static PositionManager getPositionManger() { return positionManager; }
    public bool isOpenKinect() { return opened; }
    public Body[] getBodys() { return bonesData; }
}
