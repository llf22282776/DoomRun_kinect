using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;
using Windows.Data;

public class PoseAna : MonoBehaviour {
    public  const int  POSE_TRUN_LEFT = 0;
    public  const int POSE_TRUN_RIGHT = 1;
    public  const int POSE_JUMP = 2;
    public  const int POSE_SQUAT= 3;
    public  const int POSE_LEFTDOWN = 4;
    public  const int POSE_RIGHTDOWN = 5;
    public  const int POSE_NONE = 6;
    public  const int STATUS_POSE_JUMP_AND_SQUART = 7;
    public const int  STATUS_POSE_DOWN = 8;
    public const int  STATUS_POSE_TURN = 9;

    public static Dictionary<int, string> poseString = new Dictionary<int, string>() {
        {0 ,"POSE_TRUN_LEFT"},
        { 1,"POSE_TRUN_RIGHT"},
        { 2,"POSE_JUMP"},
        { 3,"POSE_SQUAT"},
        { 4,"POSE_LEFTDOWN"},
        { 5,"POSE_RIGHTDOWN"},
        { 6,"POSE_NONE" }
    };

    public Body lastBody;//上一个身体
    public  Dictionary<JointType, Windows.Kinect.Joint> lastBodyjoints=null;
    public ulong lastTrackID;//跟踪编号
    public int lastPose=PoseAna.POSE_NONE;//上一个姿势
    public int lastPose_DOWN = POSE_NONE;
    public int lastPose_JUMP_AND_SQUART = POSE_NONE;
    public int lastPose_TURN = POSE_NONE;
    public  static PoseAna poseAna;

    void Start () {
        //清空上次
        clear();
        poseAna = this;
	}
    public static PoseAna getPoseAna() {

        return poseAna;

    }
    void clear() {
        //清空记录
      //  lastPositionJoints.Clear();//清空
        lastPose = PoseAna.POSE_NONE;//没有动作
        lastPose_DOWN = POSE_NONE;
        lastPose_JUMP_AND_SQUART = POSE_NONE;
        lastPose_TURN = POSE_NONE;
        lastTrackID = 0;
        lastBody = null;
        lastBodyjoints = null;
    } 
    public int ActionFromKinect() {//从kinect中获得
                                   /*
                                    要点：
                                    1.跟踪id如果这次没有在Body里面，就吧跟踪id 设置成body里面第一个被跟踪的
                                    */
       PositionManager posManager=  PositionManager.getPositionManger();
        if (posManager == null || posManager.isOpenKinect() == false) {
            return POSE_NONE;
        }// return POSE_NONE;//没有
        //打开了，现在读关节坐标，然后判断，动作
        Body[] bodys= posManager.getBodys();//获得body数组

        if (bodys == null || bodys[0] == null) {
           // Debug.Log("nonono!!"+" "+(bodys == null) +" "+"body's length:");
            return POSE_NONE; }
        bool hasLast = false;
        int index = 0;
        ulong thisTrackID=0;
        Body thisTrackBody=null;
      //  Debug.Log("bodys.length:" + bodys.Length);
        foreach (Body bodyData in bodys) {
            if (bodyData.IsTracked ) {
                if (index == 0) {  thisTrackID = bodyData.TrackingId; thisTrackBody = bodyData; index++; }//记录第一个跟踪的身体
                if (bodyData.TrackingId == lastTrackID) {  thisTrackBody = bodyData;hasLast = true;break; }//找到了上次监测的身体    
            }//加进来
        }
        if (hasLast == false) {//没有上一个,表示第一次
            if  ( thisTrackBody== null ) {
                clear();
                return POSE_NONE;

            }

            if (canBeBody(thisTrackBody) == false) {
                
                clear();
                return POSE_NONE;
            }
         //   Debug.Log("new!");
            clear();
            //保存最基础的坐标
            lastBodyjoints = new Dictionary<JointType, Windows.Kinect.Joint>();
            lastBodyjoints.Clear();
            foreach ( KeyValuePair<JointType, Windows.Kinect.Joint> keyAndValue in thisTrackBody.Joints) {
                Windows.Kinect.Joint joi = new Windows.Kinect.Joint();
                CameraSpacePoint cameraSpacePoint = new CameraSpacePoint();
                cameraSpacePoint.X = keyAndValue.Value.Position.X;
                cameraSpacePoint.Y = keyAndValue.Value.Position.Y;
                cameraSpacePoint.Z = keyAndValue.Value.Position.Z;
                joi.Position = cameraSpacePoint;

                lastBodyjoints.Add(keyAndValue.Key,joi);


            }
          // lastBody =thisTrackBody;
            lastTrackID = thisTrackID; //放置一个初始数据
            return POSE_NONE;//啥都没动作

        }
        else {//有上一个，
              //判断是什么动作发生
            int thisPose=-1;
            thisPose = whatAction(lastBodyjoints,thisTrackBody);
            // Debug.Log("action:"+thisPose);
            //更新上次的姿势
            //更新上次的lastBody,不更新
            if (lastPose != thisPose) {
                //新姿势不同于上次的姿势
                lastPose = thisPose;
                return thisPose;
            }
            else {
                //新姿势和上次一样
                //判断是不是可连续的
                if (lastPose == POSE_LEFTDOWN || lastPose ==POSE_RIGHTDOWN) {
                    lastPose = thisPose;
                    return thisPose;
                }
                return POSE_NONE;
            }
            //返回动作编号
        }
    }
    public int getActionFromKinect()
    {
                return ActionFromKinect();
    }
	// Update is called once per frame
	void Update () {
        
		
	}
    public Dictionary<int,int> getActionFromKinect_sync() {
        PositionManager posManager = PositionManager.getPositionManger();
        Dictionary<int, int> dic = new Dictionary<int, int>();
        int[] stautsList = { STATUS_POSE_DOWN, STATUS_POSE_JUMP_AND_SQUART, STATUS_POSE_TURN };
        for (int i = 0; i < 3; i++) { dic.Add(stautsList[i], POSE_NONE); }//先行填充，按需返回
        if (posManager == null || posManager.isOpenKinect() == false)
        {
            clear();
            return dic;
        }//没有
        //打开了，现在读关节坐标，然后判断，动作
        Body[] bodys = posManager.getBodys();//获得body数组

        if (bodys == null || bodys[0] == null)
        {
            clear();
            return dic;
        }
        bool hasLast = false;
        int index = 0;
        ulong thisTrackID = 0;
        Body thisTrackBody = null;
        //  Debug.Log("bodys.length:" + bodys.Length);
        foreach (Body bodyData in bodys)
        {
            if (bodyData.IsTracked)
            {
                if (index == 0) { thisTrackID = bodyData.TrackingId; thisTrackBody = bodyData; index++; }//记录第一个跟踪的身体
                if (bodyData.TrackingId == lastTrackID) { thisTrackBody = bodyData; hasLast = true; break; }//找到了上次监测的身体    
            }//加进来
        }
        if (hasLast == false)
        {//没有上一个,表示第一次
            if (thisTrackBody == null)
            {
                clear();
                return dic;
              
            }
            if (canBeBody(thisTrackBody) == false)
            {
                clear();
                return dic;
            }
            clear();
            //保存最基础的坐标
            lastBodyjoints = new Dictionary<JointType, Windows.Kinect.Joint>();
            lastBodyjoints.Clear();
            foreach (KeyValuePair<JointType, Windows.Kinect.Joint> keyAndValue in thisTrackBody.Joints)
            {
                Windows.Kinect.Joint joi = new Windows.Kinect.Joint();
                CameraSpacePoint cameraSpacePoint = new CameraSpacePoint();
                cameraSpacePoint.X = keyAndValue.Value.Position.X;
                cameraSpacePoint.Y = keyAndValue.Value.Position.Y;
                cameraSpacePoint.Z = keyAndValue.Value.Position.Z;
                joi.Position = cameraSpacePoint;
                lastBodyjoints.Add(keyAndValue.Key, joi);


            }
            lastTrackID = thisTrackID; //放置一个初始数据        
            return dic;
        }
        else {
            //有先前的身体,抽取出来一个字典
            return ActionsFromKinect(thisTrackBody, thisTrackID);
        }
    }
    public Dictionary<int, int> ActionsFromKinect(  Body thisTrackBody, ulong thisTrackID) {
        Dictionary<int, int> dic = new Dictionary<int, int>();
        int[] stautsList = { STATUS_POSE_DOWN, STATUS_POSE_JUMP_AND_SQUART, STATUS_POSE_TURN };
        for (int i = 0; i < 3; i++) { dic.Add(stautsList[i], POSE_NONE); }//先行填充，按需返回
        dic[STATUS_POSE_DOWN] = ActionFromKinect(STATUS_POSE_DOWN,thisTrackBody,thisTrackID);
        dic[STATUS_POSE_JUMP_AND_SQUART] = ActionFromKinect(STATUS_POSE_JUMP_AND_SQUART, thisTrackBody, thisTrackID);
        dic[STATUS_POSE_TURN] = ActionFromKinect(STATUS_POSE_TURN, thisTrackBody, thisTrackID);
        return dic;
    }
    public int ActionFromKinect(int status, Body thisTrackBody, ulong thisTrackID)
    {
        //有上一个，
        //判断是什么动作发生
        int thisPose = -1;
        thisPose = whatAction(status, lastBodyjoints, thisTrackBody);
        //更新上次的姿势
        //更新上次的lastBody,不更新
        if (status == STATUS_POSE_JUMP_AND_SQUART)
        {//跳蹲动作
            if (lastPose_JUMP_AND_SQUART != thisPose)
            {
                //新姿势不同于上次的姿势
                lastPose_JUMP_AND_SQUART = thisPose;
                return thisPose;
            }
            else
            {
                return POSE_NONE;
            }
        }
        else if (status == STATUS_POSE_DOWN)
        {//倾斜
            //新姿势不同于上次的姿势
                lastPose_DOWN = thisPose;
                return thisPose;
            
           
        }
        else if (status == STATUS_POSE_TURN)//转身
        {
            if (lastPose_TURN != thisPose)
            {
                //新姿势不同于上次的姿势
                lastPose_TURN = thisPose;
                return thisPose;
            }
            else
            {
                return POSE_NONE;
            }
        }
        else return POSE_NONE;
        //返回动作编号
    }
    private int whatAction(Dictionary<Windows.Kinect.JointType, Windows.Kinect.Joint> last, Body thisTime) {
        Dictionary<Windows.Kinect.JointType, Windows.Kinect.Joint> lastJoints = last; //上次的
        Dictionary<Windows.Kinect.JointType, Windows.Kinect.Joint> thisTimeJoints = thisTime.Joints;//这次的
        float jump_space = 0.15f;
        float squat_space = 0.25f;
        float right_Down = 0.1f;
        float right_Turn = 0.1f;

        //检测跳
      //  Debug.Log("new:  KneeLeft.y:" + thisTimeJoints[Windows.Kinect.JointType.KneeLeft].Position.Y + "  " + "KneeRight.y:" + thisTimeJoints[Windows.Kinect.JointType.KneeRight].Position.Y);
      //  Debug.Log("old:  KneeLeft.y:" + lastJoints[Windows.Kinect.JointType.KneeLeft].Position.Y + "  " + "KneeRight.y:" + lastJoints[Windows.Kinect.JointType.KneeRight].Position.Y);

        if ((thisTimeJoints[Windows.Kinect.JointType.HipLeft].Position.Y - lastJoints[Windows.Kinect.JointType.HipLeft].Position.Y > jump_space)
            && (thisTimeJoints[Windows.Kinect.JointType.HipRight].Position.Y - lastJoints[Windows.Kinect.JointType.HipRight].Position.Y > jump_space)
            && (lastPose != POSE_SQUAT) 
            )
        {//上一次姿势必须不能是下蹲,也不能是跳

            return POSE_JUMP;
        }
        //检测下蹲
        else if (
          
            (thisTimeJoints[Windows.Kinect.JointType.HipLeft].Position.Y - lastJoints[Windows.Kinect.JointType.HipLeft].Position.Y < -squat_space)
            && (thisTimeJoints[Windows.Kinect.JointType.HipRight].Position.Y - lastJoints[Windows.Kinect.JointType.HipRight].Position.Y < -squat_space)
             && (lastPose != POSE_JUMP)
            ) {
           // Debug.Log("distance"+ (thisTimeJoints[Windows.Kinect.JointType.KneeLeft].Position.Y - lastJoints[Windows.Kinect.JointType.KneeLeft].Position.Y));
            //现在的y-之前的y小于0.1
            return POSE_SQUAT;

        }
        //检测向左倾斜 
        else if (
             (thisTimeJoints[Windows.Kinect.JointType.ShoulderRight].Position.Y - thisTimeJoints[Windows.Kinect.JointType.ShoulderLeft].Position.Y > right_Down)
          //  && (thisTimeJoints[Windows.Kinect.JointType.ShoulderLeft].Position.Y - thisTimeJoints[Windows.Kinect.JointType.ShoulderLeft].Position.Y < -right_Down)
             && (lastPose != POSE_RIGHTDOWN)
            ) {
            return POSE_LEFTDOWN;
        }
        //检测向右倾斜 
        else if (
             (thisTimeJoints[Windows.Kinect.JointType.ShoulderRight].Position.Y - thisTimeJoints[Windows.Kinect.JointType.ShoulderLeft].Position.Y <- right_Down)
         //   && (thisTimeJoints[Windows.Kinect.JointType.ShoulderLeft].Position.Y - thisTimeJoints[Windows.Kinect.JointType.ShoulderLeft].Position.Y > right_Down)
            && (lastPose != POSE_LEFTDOWN) 
            )
        {
            return POSE_RIGHTDOWN;
        }
        //检测向左转
        else if (
           (thisTimeJoints[Windows.Kinect.JointType.ShoulderRight].Position.Z - thisTimeJoints[Windows.Kinect.JointType.ShoulderLeft].Position.Z <- right_Turn)
        //  && (thisTimeJoints[Windows.Kinect.JointType.ShoulderLeft].Position.Z - thisTimeJoints[Windows.Kinect.JointType.ShoulderLeft].Position.Z < -right_Turn)
           && (lastPose != POSE_TRUN_RIGHT)
          )
        {
            return POSE_TRUN_LEFT;
        }
        //检测向右转
        else if (
            (thisTimeJoints[Windows.Kinect.JointType.ShoulderRight].Position.Z - thisTimeJoints[Windows.Kinect.JointType.ShoulderLeft].Position.Z > right_Turn)
       //   && (thisTimeJoints[Windows.Kinect.JointType.ShoulderLeft].Position.Z - thisTimeJoints[Windows.Kinect.JointType.ShoulderLeft].Position.Z > right_Turn)
          && (lastPose != POSE_TRUN_LEFT) 
          )
        {
            return POSE_TRUN_RIGHT;
        }


        return POSE_NONE;


    }
    private int whatAction(int status, Dictionary<Windows.Kinect.JointType, Windows.Kinect.Joint> last, Body thisTime)
    {
        Dictionary<Windows.Kinect.JointType, Windows.Kinect.Joint> lastJoints = last; //上次的
        Dictionary<Windows.Kinect.JointType, Windows.Kinect.Joint> thisTimeJoints = thisTime.Joints;//这次的
        float jump_space = 0.15f;
        float squat_space = 0.25f;
        float right_Down = 0.1f;
        float right_Turn = 0.1f;
        //检测跳
        if ((thisTimeJoints[Windows.Kinect.JointType.HipLeft].Position.Y - lastJoints[Windows.Kinect.JointType.HipLeft].Position.Y > jump_space)
            && (thisTimeJoints[Windows.Kinect.JointType.HipRight].Position.Y - lastJoints[Windows.Kinect.JointType.HipRight].Position.Y > jump_space)
            && (lastPose != POSE_SQUAT)
            && (status == STATUS_POSE_JUMP_AND_SQUART)
            )
        {//上一次姿势必须不能是下蹲,也不能是跳

            return POSE_JUMP;
        }
        //检测下蹲
        else if (

            (thisTimeJoints[Windows.Kinect.JointType.HipLeft].Position.Y - lastJoints[Windows.Kinect.JointType.HipLeft].Position.Y < -squat_space)
            && (thisTimeJoints[Windows.Kinect.JointType.HipRight].Position.Y - lastJoints[Windows.Kinect.JointType.HipRight].Position.Y < -squat_space)
             && (lastPose != POSE_JUMP)
              && (status == STATUS_POSE_JUMP_AND_SQUART)
            )
        {
            //现在的y-之前的y小于0.1
            return POSE_SQUAT;

        }
        //检测向左倾斜 
        else if (
             (thisTimeJoints[Windows.Kinect.JointType.ShoulderRight].Position.Y - thisTimeJoints[Windows.Kinect.JointType.ShoulderLeft].Position.Y > right_Down)
             && (lastPose != POSE_RIGHTDOWN)
              && (status == STATUS_POSE_DOWN)
            )
        {
            return POSE_LEFTDOWN;
        }
        //检测向右倾斜 
        else if (
             (thisTimeJoints[Windows.Kinect.JointType.ShoulderRight].Position.Y - thisTimeJoints[Windows.Kinect.JointType.ShoulderLeft].Position.Y < -right_Down)
            && (lastPose != POSE_LEFTDOWN)
            && (status == STATUS_POSE_DOWN)
            )
        {
            return POSE_RIGHTDOWN;
        }
        //检测向左转
        else if (
           (thisTimeJoints[Windows.Kinect.JointType.ShoulderRight].Position.Z - thisTimeJoints[Windows.Kinect.JointType.ShoulderLeft].Position.Z < -right_Turn)
           && (lastPose != POSE_TRUN_RIGHT)
           && (status == STATUS_POSE_TURN)
          )
        {
            return POSE_TRUN_LEFT;
        }
        //检测向右转
        else if (
            (thisTimeJoints[Windows.Kinect.JointType.ShoulderRight].Position.Z - thisTimeJoints[Windows.Kinect.JointType.ShoulderLeft].Position.Z > right_Turn)
          && (lastPose != POSE_TRUN_LEFT)
           && (status == STATUS_POSE_TURN)
          )
        {
            return POSE_TRUN_RIGHT;
        }
        return POSE_NONE;
    }
    public bool canBeBody(Body body) {
        Dictionary<Windows.Kinect.JointType, Windows.Kinect.Joint> joints = body.Joints;
        float spaces_allowed = 0.05f;
        if (((joints[Windows.Kinect.JointType.ShoulderLeft].Position.Z - joints[Windows.Kinect.JointType.ShoulderRight].Position.Z <= spaces_allowed) &&
            (joints[Windows.Kinect.JointType.ShoulderLeft].Position.Z > joints[Windows.Kinect.JointType.ShoulderRight].Position.Z) 
            )
           || ((joints[Windows.Kinect.JointType.ShoulderRight].Position.Z - joints[Windows.Kinect.JointType.ShoulderLeft].Position.Z <= spaces_allowed) &&
            (joints[Windows.Kinect.JointType.ShoulderRight].Position.Z > joints[Windows.Kinect.JointType.ShoulderLeft].Position.Z)
           ) 
           ) {//两个肩错不是很大
            return true;
        }
        return false;
    }

    private void OnApplicationQuit()
    {
        clear();
        poseAna = null;
    }
    public bool PauseOrStart (CameraSpacePoint pos1,CameraSpacePoint pos2){
        //吧手伸直就行,这里判断的是，腕关节在肩关节的阈值内
        //腕关节的z要比肩小，Y要和肩在阈值内
        //pos1,肩关节
        //pos2,腕关节
        float Z_space = 0.4f;
        float Y_space = 0.05f;
        if (pos2.Z<pos1.Z-Z_space
            && 
            ( pos2.Y <= pos1.Y+Y_space
            && pos2.Y >= pos1.Y-Y_space
            )
           ) {

            return true;
        }
        return false;
    }
    
}
