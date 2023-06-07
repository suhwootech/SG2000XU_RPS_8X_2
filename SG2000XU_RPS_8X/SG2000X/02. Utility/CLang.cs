using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SG2000X
{
    public class CLang : CStn<CLang>
    {
        private CLang()
        {
              
        }
        public string GetLanguage(string text)
        {
            if(text.Equals("AIR", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "AIR";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "공기";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "气";
            }
            else if(text.Equals("AIR BLOW", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "AIR BLOW";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "AIR BLOW";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "吹气";//
            }  
            else if(text.Equals("AIR KNIFE", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "AIR KNIFE";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "AIR KNIFE";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "器气刀";
            }  
            else if(text.Equals("Align", StringComparison.Ordinal))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Align";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "정렬";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "校准";
            }
            else if (text.Equals("ALIGN", StringComparison.Ordinal))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "ALIGN";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "정렬";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "校准";
            }
            else if(text.Equals("BACKWARD", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "BACKWARD";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "후진";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "向后";
            }
            else if(text.Equals("Barcode", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Barcode";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "바코드";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "条码";
            }
            else if(text.Equals("BOTTOM", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "BOTTOM";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "하단";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "底部";//
            }
            else if(text.Equals("CW", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "CW";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "시계방향";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "正转";//
            }
            else if(text.Equals("CCW", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "CCW";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "반시계방향";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "逆时针";
            }
            else if(text.Equals("CLAMP", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "CLAMP";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "잡다";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "夹子打开";
            }
            else if(text.Equals("CLEANER", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "CLEANER";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "크리너";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "清洁";//清洁
            }
            else if(text.Equals("CONVEYOR", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "CONVEYOR";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "컨베이어";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "传送";//
            }
            else if(text.Equals("Cylinder", StringComparison.Ordinal))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Cylinder";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "실린더";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "气缸";//
            }
            else if (text.Equals("CYLINDER", StringComparison.Ordinal))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "CYLINDER";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "실린더";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "气缸";//
            }
            else if(text.Equals("Depth", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Depth";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "깊이";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "深度";
            }
            else if(text.Equals("Down", StringComparison.Ordinal))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Down";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "아래";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "下降";//

            }
            else if (text.Equals("DOWN", StringComparison.Ordinal))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "DOWN";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "아래";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "下降";//

            }
            else if(text.Equals("DRAIN", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "DRAIN";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "배수";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "排水";//
            }
            else if(text.Equals("Dynamic", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Dynamic";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "다이나믹";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "Dynamic";
            }
            else if(text.Equals("EJECTOR", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "EJECTOR";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "배출기";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "真空释放";//
            }
            else if(text.Equals("Flow Low", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Flow Low";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "흐름 낮음";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "流量低";
            }
            else if(text.Equals("FORWARD", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "FORWARD";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "전진";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "向前";
            }
            else if(text.Equals("FRONT", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "FRONT";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "앞";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "面前";
            }
            else if(text.Equals("Gripper", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Gripper";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "그리퍼";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "夹子";
            }
            else if(text.Equals("Guide", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Guide";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "가이드";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "轨道";
            }
            else if(text.Equals("IONIZER", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "IONIZER";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "이온화장치";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "IONIZER";
            }
            else if(text.Equals("LEFT", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "LEFT";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "좌";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "剩下";
            }
            else if(text.Equals("Lift", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Lift";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "리프트";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "升降机";// 
            }
            else if(text.Equals("MAGAZINE", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "MAGAZINE";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "매가진";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "料盒";
            }
            else if(text.Equals("MGZ", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "MGZ";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "MGZ";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "MGZ";
            }
             else if(text.Equals("MIDDLE", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "MIDDLE";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "중간";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "中间";//
            }
            else if(text.Equals("No Alarm", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "No Alarm";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "알람없음";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "没有警报";
            }
            else if(text.Equals("OCR", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "OCR";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "문자인식";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "OCR";
            }
            else if(text.Equals("OFF", StringComparison.Ordinal))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "OFF";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "OFF";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "关闭";//关闭
            }
            else if (text.Equals("Off", StringComparison.Ordinal))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Off";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "Off";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "关闭";//关闭
            }
            else if(text.Equals("ON", StringComparison.Ordinal))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "ON";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "ON";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "ON";
            }
            else if (text.Equals("On", StringComparison.Ordinal))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "On";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "On";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "On";
            }
            else if(text.Equals("Orientation", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Orientation";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "방향";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "取向";
            }
            if(text.Equals("Overload", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Overload";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "오버로드";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "超载";
            }
            else if(text.Equals("PICKER", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "PICKER";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "피커";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "抓手";//抓手
            }
            else if(text.Equals("POSITION", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "POSITION";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "위치";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "位置";//
            }
            else if(text.Equals("PROBE", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "PROBE";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "프로브";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "探针";//
            }
            
             else if(text.Equals("PUSHER", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "PUSHER";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "푸셔";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "推杆";
            }
            else if(text.Equals("REAR", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "REAR";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "뒤";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "后";
            }
            else if(text.Equals("RIGHT", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "RIGHT";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "우";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "对";
            }
             else if(text.Equals("ROTATE", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "ROTATE";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "회전";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "转为";
            }
            else if(text.Equals("RUN", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "RUN";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "가동";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "跑";
            }
            else if(text.Equals("Skip", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Skip";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "건너뜀";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "跳跃";
            }
            else if(text.Equals("STANDBY", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "STANDBY";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "대기";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "等待";//
            }  
            else if(text.Equals("STOP", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "STOP";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "정지";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "停止";//
            }
             else if(text.Equals("TABLE", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "TABLE";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "테이블";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "平台";//
            }
            // 2021.04.26 lhs Start
            //else if (text.Equals("Target", StringComparison.OrdinalIgnoreCase))
            //{
            //    if ((int)ELang.English == CData.Opt.iSelLan) return "Target";
            //    else if ((int)ELang.Korea == CData.Opt.iSelLan) return "목표";
            //    else if ((int)ELang.China == CData.Opt.iSelLan) return "目标";
            //}
            else if (text.Equals("Target", StringComparison.Ordinal))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Target";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "목표";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "目标";
            }
            else if (text.Equals("TARGET", StringComparison.Ordinal))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "TARGET";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "목표";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "目标";
            }
            // 2021.04.26 lhs End
            else if (text.Equals("Temp High", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Temp High";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "온도 높음";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "温度高";
            }
            else if(text.Equals("Thickness", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Thickness";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "두께";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "厚度";
            }
            else if(text.Equals("TOP", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "TOP";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "상단";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "顶部";//
            }
            else if(text.Equals("TopDown", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "TopDown";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "위에서아래로";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "自顶向下";
            }
            else if(text.Equals("Total", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Total";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "전체";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "总";
            }
            else if(text.Equals("UNCLAMP", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "UNCLAMP";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "놓다";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "剪辑打开";//

            }
            else if(text.Equals("Use", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Use";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "사용";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "使用";
            }
            else if(text.Equals("Up", StringComparison.Ordinal))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Up";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "위";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "上升";//
            }
            else if (text.Equals("UP", StringComparison.Ordinal))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "UP";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "위";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "上升";//
            }
            else if(text.Equals("VACUUM", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "VACUUM";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "진공";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "真空";//
            }
            else if(text.Equals("WATER", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "WATER";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "물";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "器水";//
            }
             else if(text.Equals("WATER KNIFE", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "WATER KNIFE";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "WATER KNIFE";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "器水刀";//器水刀

            }
            return text;
        }
        public string GetConvertMsg(string Msg)
        {
            if(Msg.Equals("Wheel change not complete", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Wheel change not complete";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "휠교체가 완료 되지 않았습니다.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "砂轮更换未完成";
            }
            else if(Msg.Equals("Manual All Home Please", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Manual All Home Please";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "매뉴얼로 전체 홈동작 해주세요.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "请复位";
            }
            else if(Msg.Equals("Lot Open Please", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Lot Open Please";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "Lot 오픈 해 주세요.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "请开批";
            }
            else if(Msg.Equals("GRD Front Door close Please", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "GRD Front Door close Please";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "그라인더 앞쪽 도어를 닫아 주세요";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "请关闭研磨区前玻璃门";
            }
            else if(Msg.Equals("GRD Rear Door close Please", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "GRD Rear Door close Please";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "그라인더 뒷쪽 도어를 닫아 주세요";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "请关闭研磨区后玻璃门";
            }
            else if(Msg.Equals("Onloader Left Door close Please", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Onloader Left Door close Please";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "Onloader 왼쪽 도어를 닫아 주세요";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "请关闭左侧安全门";
            }
            else if(Msg.Equals("Onloader Front Door close Please", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Onloader Front Door close Please";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "Onloader 앞쪽 도어를 닫아 주세요";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "请关闭前方安全门";
            }
            else if(Msg.Equals("Offloader Right Door close Please", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Offloader Right Door close Please";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "Offloader 오른쪽 도어를 닫아 주세요";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "请关闭下料区右侧安全门";
            }
            else if(Msg.Equals("Offloader Rear Door close Please", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Offloader Rear Door close Please";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "Offloader 뒤쪽 도어를 닫아 주세요";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "请关闭下料区后方安全门";
            }
            else if(Msg.Equals("Offloader Front Door close Please", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Offloader Front Door close Please";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "Offloader 앞쪽 도어를 닫아 주세요";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "请关闭下料区前方安全门";
            }
            else if(Msg.Equals("Grinding Cover Check Please", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Grinding Cover Check Please";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "그라인드 커버를 체크해 주세요";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "请检查研磨区盖板";
            }
            else if(Msg.Equals("Check Leak Sensor Please", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check Leak Sensor Please";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "누수센서를 체크해 주세요";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "请检查防漏传感器";
            }
            else if(Msg.Equals("Check DryLeak Sensor Please", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check DryLeak Sensor Please";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "드라이리크 센서를 체크해 주세요";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "请检查排水传感器";
            }
            else if(Msg.Equals("Different the device & BCR Recipe", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Different the device & BCR Recipe";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "디바이스와 바코드 레시피가 다릅니다.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "程序名不同 请确认";
            }
            else if(Msg.Equals("Check Emergency", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check Emergency";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "이머전시를 체크해 주세요";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "检查禁止停止按钮";
            }
            else if(Msg.Equals("Need Warm Up Please", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Need Warm Up Please";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "웜업해 주세요";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "请暖机";
            }
            else if(Msg.Equals("Warm Up Time is 30 minute", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Warm Up Time is 30 minute";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "웜업 시간은 30분입니다.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "须暖机30分钟";
            }
            else if(Msg.Equals("Warm Up Time is 15 minute", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Warm Up Time is 15 minute";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "웜업 시간은 15분입니다.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "须暖机15分钟";
            }
            else if(Msg.Equals("Warm Up Time is 5 ~ 10 minute", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Warm Up Time is 5 ~ 10 minute";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "웜업 시간은 5~10분입니다.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "须暖机5~10分钟";
            }
            else if(Msg.Equals("Check QC Vision Connect", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check QC Vision Connect";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "QC 비전 연결을 확인해 주세요.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "检查QC镜头连接";
            }
            else if(Msg.Equals("QC Vision Can't AutoRun Now", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "QC Vision Can't AutoRun Now";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "QC비전에서 오토런을 할 수 없습니다.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "QC镜头无法运行";
            }
            else if(Msg.Equals("Manual Stop Now, Move the stopped module to the wait position please", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Manual Stop Now, Move the stopped module to the wait position please";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "매뉴얼 정지 상태입니다. 대기 위치로 정지된 모듈을 이동해 주세요.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "操作停止，将已经停止的模块移动到等待位置";
            }
            else if(Msg.Equals("Warm Up OK", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Warm Up OK";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "웜업이 완료 되었습니다.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "暖机完毕";
            }
            else if(Msg.Equals("All Servo On OK", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "All Servo On OK";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "모든 서보에 파워가 공급 되었습니다.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "所有伺服开启完毕";
            }
            else if(Msg.Equals("All Servo Off OK", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "All Servo Off OK";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "모든 서보에 파워가 차단 되었습니다.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "所有伺服关闭完毕";
            }
            else if(Msg.Equals("Reset OK", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Reset OK";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "리셋 완료.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "重启完毕";
            }
            else if(Msg.Equals("On loader home ok", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "On loader home ok";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "온로더 홈동작 완료.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "上料区复位完毕";
            }
            else if(Msg.Equals("On loader place ok", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "On loader place ok";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "온로더 안착 완료.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "上料区放置完毕";
            }
            else if(Msg.Equals("On Loader Push OK", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "On Loader Push OK";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "온로더 푸쉬 완료.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "上料区推料完毕";
            }
            else if(Msg.Equals("In Rail Home OK", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "In Rail Home OK";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "인레일 홈동작 완료.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "进料区复位完毕";
            }
            else if(Msg.Equals("In Rail Move Wait Position OK", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "In Rail Move Wait Position OK";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "인레일 대기 위치로 이동 완료.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "进料区移动到等待位置完毕";
            }
            else if(Msg.Equals("In Rail Align OK", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "In Rail Align OK";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "인레일 얼라인 이동 완료.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "进料区校准完毕";
            }
            else if(Msg.Equals("On loader picker Barcode Check OK", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "On loader picker Barcode Check OK";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "온로더 피커 바코드 확인 완료.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "上料区定位孔检测完毕";
            }
            else if(Msg.Equals("On loader picker Orientation Check OK", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "On loader picker Orientation Check OK";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "온로더 피커 방향 확인 완료.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "进料区基板高度检测完毕";
            }
            else if(Msg.Equals("In Rail DynamicFunction Check OK", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "In Rail DynamicFunction Check OK";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "인레일 다이나믹펑션 확인 완료.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "上料区抓手移动到等待位置完毕";
            }
            else if(Msg.Equals("In Rail DynamicFunction Check OK", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "In Rail DynamicFunction Check OK";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "인레일 다이나믹펑션 확인 완료.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "上料区抓手复位完毕";
            }
            else if(Msg.Equals("On loader picker wait ok", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "On loader picker wait ok";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "온로더 피커 대기 위치 이동 완료.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "上料区抓手抓取于进料区完毕";
            }
            else if(Msg.Equals("On Loader Picker Home OK", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "On Loader Picker Home OK";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "온로더 피커 홈 동작 완료.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "上料区抓手抓取于左侧工作台完毕";
            }
            //-----------------------------------------------------------------------------------------------
            else if(Msg.Equals("Wrong Passwaord", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Wrong Passwaord";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "비밀번호가 잘못 되었습니다.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "密码错误";
            }
            else if(Msg.Equals("Wrong Confirm Passwaord", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Wrong Confirm Passwaord";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "비밀번호를 확인해 주세요.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "确认密码错误";
            }
            else if(Msg.Equals("Wheel change not complete", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Wheel change not complete";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "휠 교체가 완료 되지 않았습니다..";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "更换砂轮未完成";
            }
            else if(Msg.Equals("Do you want shut Down?", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Do you want shut Down?";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "프로그램을 종료 하시겠습니까?";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "是否想关闭?";
            }
            else if(Msg.Equals("Upper select Level", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Upper select Level";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "상위 레벨로 변경후 다시 시도 해주세요";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "更高的等级权限";
            }
            else if(Msg.Equals("Upper select Engineer", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Upper select Engineer";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "엔지니어 모드로 전환해 주세요";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "更高的工程师权限";
            }
            else if(Msg.Equals("Lot End Now, But Different In/Out Strip Count. So Check Strip Count", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Lot End Now, But Different In/Out Strip Count. So Check Strip Count";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "투입한 스트립 겟수와 배출한 스트립 겟수가 맞지 않습니다. 확인해 주세요";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "批次已经结束，进/出料数量不符，请确认";
            }
            else if(Msg.Equals("Lot End", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Lot End";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "Lot 종료";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "批次结束";
            }
            else if(Msg.Equals("Lot End", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Lot End";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "Lot 종료";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "批次结束";
            }
            else if(Msg.Equals("Magazin Or Strip Remove Please", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Magazin Or Strip Remove Please";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "매거진과 스트립을 제거해  주세요";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "料盒或产品请移除";
            }
            else if(Msg.Equals("Qc Vision Can't Change Device File", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Qc Vision Can't Change Device File";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "QC 비전에서 디바이스 파일을 변경 못했습니다.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "Qc 镜头系统无法更换程序名";
            }
            else if(Msg.Equals("No Input Tool Name", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "No Input Tool Name";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "툴 이름을 입력해 주세요.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "未输入工具信息";
            }
            else if(Msg.Equals("No Input Lot Name", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "No Input Lot Name";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "Lot 이름을 입력해 주세요.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "未输入批次信息";
            }
            else if(Msg.Equals("Current using wheel", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Current using wheel";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "현재 사용중인 휠입니다.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "当前使用砂轮";
            }
            else if(Msg.Equals("Not selected wheel", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Not selected wheel";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "휠을 선택하지 않았습니다.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "未选择砂轮";
            }
            else if(Msg.Equals("Wheel name exist !!!", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Wheel name exist !!!";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "휠 이름이 있습니다.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "砂轮名字存在 !!!";
            }
            else if(Msg.Equals("Wheel name same !!!", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Wheel name same !!!";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "같은 이름의 휠이 있습니다.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "砂轮名字重复 !!!";
            }
            else if(Msg.Equals("Not selected wheel", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Not selected wheel";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "휠이 선택되지 않았습니다.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "未选择砂轮 !!!";
            }
            //999999999999999999999999999999999
            else if(Msg.Equals("Check Dry Z Axis", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check Dry Z Axis";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "휠이 선택되지 않았습니다.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "检查甩干区 Z 向 !!!";
            }
            else if(Msg.Equals("Check Dry R Axis", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check Dry R Axis";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "휠이 선택되지 않았습니다.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "检查甩干区 R 向";
            }
            else if(Msg.Equals("Check OffLoader Magazin", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check OffLoader Magazin";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "휠이 선택되지 않았습니다.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "检查下料区料盒";
            }
            else if(Msg.Equals("Check OffLoader Z Axis Pos", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check OffLoader Z Axis Pos";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "휠이 선택되지 않았습니다.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "检查下料区 Z 向位置";
            }
            else if(Msg.Equals("Check OffLoader Y Axis Pos", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check OffLoader Z Axis Pos";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "휠이 선택되지 않았습니다.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "检查下料区 Y 向位置";
            }
            else if(Msg.Equals("Check Dry Z Axis Pos", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check Dry Z Axis Pos";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "휠이 선택되지 않았습니다.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "检查甩干区 Z 向位置";
            }
            else if(Msg.Equals("Check Offloader Picker Z Axis", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check Offloader Picker Z Axis";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "휠이 선택되지 않았습니다.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "检查下料区抓手 Z 向";
            }
            else if(Msg.Equals("Check Offloader Picker X Axis Pos", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check Offloader Picker X Axis Pos";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "휠이 선택되지 않았습니다.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "检查下料区抓手 X 向位置";
            }
            else if(Msg.Equals("Check Onloader Picker X Axis Pos", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check Onloader Picker X Axis Pos";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "휠이 선택되지 않았습니다.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "检查上料区抓手 X 向位置";
            }
            else if(Msg.Equals("Check Offloader Picker Z Axis", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check Offloader Picker Z Axis";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "휠이 선택되지 않았습니다.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "检查下料区抓手 Z 向";
            }
            else if(Msg.Equals("Check Right Table Probe", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check Right Table Probe";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "휠이 선택되지 않았습니다.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "检查Right工作台探针";
            }
            else if(Msg.Equals("Check Right Table Z Axis", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check Right Table Z Axis";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "휠이 선택되지 않았습니다.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "检查Right工作台Z 向";
            }
            else if(Msg.Equals("Check OffLoader Picker Z Axis", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check OffLoader Picker Z Axis";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "휠이 선택되지 않았습니다.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "检查下料区抓手 Z 向";
            }
            else if(Msg.Equals("Check OnLoader Picker Z Axis", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check OnLoader Picker Z Axis";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "휠이 선택되지 않았습니다.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "检查上料区抓手 Z 向";
            }
            else if(Msg.Equals("Check Left Table Z Axis", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check Left Table Z Axis";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "휠이 선택되지 않았습니다.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "检查左侧工作台Z 向";
            }
            else if(Msg.Equals("Check Left Table Probe", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check Left Table Probe";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "휠이 선택되지 않았습니다.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "检查左侧工作台探针";
            }
            else if(Msg.Equals("Check Left Table Z Axis", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check Left Table Z Axis";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "휠이 선택되지 않았습니다.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "检查左侧工作台Z 向";
            }
            else if(Msg.Equals("Check OffLoader Picker Z Axis", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check OffLoader Picker Z Axis";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "휠이 선택되지 않았습니다.";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "检查下料区抓手 Z 向";
            }
            else if(Msg.Equals("Check OnLoader Picker Z Axis", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check OnLoader Picker Z Axis";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "온로더 피커 Z축을 확인해 주세요";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "检查上料区抓手 Z 向";
            }
            else if(Msg.Equals("Check Left Table Z Axis", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check Left Table Z Axis";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "왼쪽 테이블 Z축을 확인해 주세요";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "检查左侧工作台Z 向";
            }
            else if(Msg.Equals("Check Onloader Picker X Axis Pos", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check Onloader Picker X Axis Pos";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "온로더 피커 X축 위치를 확인해 주세요";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "检查上料区抓手 X 向位置";
            }
            else if(Msg.Equals("Check Onloader Picker X Axis Pos", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check Onloader Picker X Axis Pos";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "检查上料区抓手 X 向位置";
            }
            else if(Msg.Equals("Check Onloader Picker X Axis Pos", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check Onloader Picker X Axis Pos";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "检查上料区抓手 X 向位置";
            }
            else if(Msg.Equals("Check Onloader Picker Z Axis", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check Onloader Picker Z Axis";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "检查上料区抓手 Z 向";
            }
            else if(Msg.Equals("Check Onloader Picker", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check Onloader Picker";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "检查上料区抓手";
            }
            else if(Msg.Equals("Do you want table grinding start?", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Do you want table grinding start?";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "是否进行工作台研磨?";
            }
            else if(Msg.Equals("Do you want table inspection start?", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Do you want table inspection start?";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "是否进行工作台测高?";
            }
            else if(Msg.Equals("Option data save complete", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Option data save complete";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "Option data save完毕";
            }
            else if(Msg.Equals("Right Z-Axis Pos is very Low. Move able Pos. Please", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Right Z-Axis Pos is very Low. Move able Pos. Please";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "右侧 Z-向位置过低，请移动位置";
            }
            else if(Msg.Equals("Left Z-Axis Pos is very Low. Move able Pos. Please", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Left Z-Axis Pos is very Low. Move able Pos. Please";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "左侧 Z-向位置过低，请移动位置";
            }
            else if(Msg.Equals("Do you want magazin place?", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Do you want magazin place?";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "是否进行料盒放置?";
            }
            else if(Msg.Equals("During Before Manual Run now, So wait finish run", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "During Before Manual Run now, So wait finish run";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "请等待当至前运行结束";
            }
            else if(Msg.Equals("Do you want magazin pick up?", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Do you want magazin pick up?";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "是否进行料盒抓起?";
            }
            else if(Msg.Equals("The machine is in Run. First stop Please", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "The machine is in Run. First stop Please";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "设备运行中，清先停止运行";
            }
            else if(Msg.Equals("Check Wheel dressing", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check Wheel dressing";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "确认磨刀";
            }
            else if(Msg.Equals("Check Wheel measure", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check Wheel measure";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "确认砂轮高度";
            }
            else if(Msg.Equals("Check Right wheel parameter change", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check Right wheel parameter change";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "确认右侧砂轮参数变化";
            }
            else if(Msg.Equals("Check left wheel parameter change", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check left wheel parameter change";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "确认左侧砂轮参数变化";
            }
            else if(Msg.Equals("Check wheel Parameter View", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check wheel Parameter View";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "确认砂轮参数";
            }
            else if(Msg.Equals("Check wheel change complete", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check wheel change complete";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "确认更换砂轮完毕";
            }
            else if(Msg.Equals("Check wheel change pos", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check wheel change pos";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "确认更换砂轮位置";
            }
            else if(Msg.Equals("Before Wheel measure", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Before Wheel measure";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "砂轮测高前";
            }
            else if(Msg.Equals("Select Wheel File", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Select Wheel File";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "选择砂轮文档";
            }
            else if(Msg.Equals("During Before Manual Run now, So wait finish run", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "During Before Manual Run now, So wait finish run";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "请等待当至前运行结束";
            }else if(Msg.Equals("Different the device & BCR Recipe", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Different the device & BCR Recipe";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "程序名不同 请确认";
            }
            else if(Msg.Equals("Grinding Cover Check Please", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Grinding Cover Check Please";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "请检查研磨区盖板";
            }
            else if(Msg.Equals("One More Check Strip Jig. Please", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "One More Check Strip Jig. Please";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "请再次确认夹具是否移除";
            }
            else if(Msg.Equals("Right Table Skip Now!", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Right Table Skip Now!";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "右侧工作台屏蔽中!";
            }
            else if(Msg.Equals("Left Table Skip Now!", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Left Table Skip Now!";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "左侧工作台屏蔽中!";
            }
            else if(Msg.Equals("One more check strip on each Table. Please", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "One more check strip on each Table. Please";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "请再一次确认工作台表面是否有产品";
            }
            else if(Msg.Equals("Need Warm Up Please", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Need Warm Up Please";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "请暖机";
            }
            else if(Msg.Equals("Strip exist on the table. Please check", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Strip exist on the table. Please check";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "请检查是否产品在工作台上";
            }
            else if(Msg.Equals("During Before Manual Run now, So wait finish run", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "During Before Manual Run now, So wait finish run";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "请等待当至前运行结束";
            }
            else if(Msg.Equals("Check Dry Z Axis", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check Dry Z Axis";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "检查甩干区 Z 向";
            }
            else if(Msg.Equals("Check Dry R Axis", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check Dry R Axis";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "检查甩干区 R 向";
            }
            else if(Msg.Equals("Check OffLoader Magazin", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check OffLoader Magazin";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "检查下料区料盒";
            }
            else if(Msg.Equals("Check OffLoader Z Axis Pos", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check OffLoader Z Axis Pos";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "检查下料区 Z 向位置";
            }
            else if(Msg.Equals("Check OffLoader Y Axis Pos", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check OffLoader Y Axis Pos";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "检查下料区 Y 向位置";
            }
            else if(Msg.Equals("Check Dry Z Axis Pos", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check Dry Z Axis Pos";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "检查甩干区 Z 向位置";
            }
            else if(Msg.Equals("Check Offloader Picker Z Axis", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check Offloader Picker Z Axis";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "检查下料区抓手 Z 向";
            }
            else if(Msg.Equals("Check Offloader Picker X Axis Pos", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check Offloader Picker X Axis Pos";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "检查下料区抓手 X 向位置";
            }
            else if(Msg.Equals("Check Onloader Picker X Axis Pos", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check Onloader Picker X Axis Pos";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "检查上料区抓手 X 向位置";
            }
            else if(Msg.Equals("Check Offloader Picker Z Axis", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check Offloader Picker Z Axis";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "检查下料区抓手 Z 向";
            }
            else if(Msg.Equals("Check Right Table Probe", StringComparison.OrdinalIgnoreCase))
            {
                if      ((int)ELang.English == CData.Opt.iSelLan) return "Check Right Table Probe";
                else if ((int)ELang.Korea   == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China   == CData.Opt.iSelLan) return "检查Right工作台探针";
            }
            else if (Msg.Equals("On Loader Picker Pick In Rail OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "On Loader Picker Pick In Rail OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "上料区抓手放置于左侧工作台完毕";
            }
            else if (Msg.Equals("On Loader Picker Pick Left Table OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "On Loader Picker Pick Left Table OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "上料区抓手放置于右侧工作台完毕";
            }
            else if (Msg.Equals("On Loader Picker Place Left Table OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "On Loader Picker Place Left Table OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "上料区抓手传送完毕";
            }
            else if (Msg.Equals("On Loader Picker Place Right Table OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "On Loader Picker Place Right Table OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "上料区抓手传送完毕";
            }
            else if (Msg.Equals("On Loader Picker Conversion OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "On Loader Picker Conversion OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "两侧研磨区移动到等待位置完毕";
            }
            else if (Msg.Equals("On Loader Picker Conversion OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "On Loader Picker Conversion OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "两侧研磨区复位完毕";
            }
            else if (Msg.Equals("Grind Dual Move Wait Position OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Grind Dual Move Wait Position OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "左侧研磨区复位完毕";
            }
            else if (Msg.Equals("Grind Dual Home OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Grind Dual Home OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "左侧研磨区移动到等待位置完毕";
            }
            else if (Msg.Equals("Grind Left Home OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Grind Left Home OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "左侧研磨区砂轮高度测量完毕";
            }
            else if (Msg.Equals("Grind Left Move Wait Position OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Grind Left Move Wait Position OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "左侧研磨区磨刀石高度测量完毕";
            }
            else if (Msg.Equals("Grind Left Wheel Hight Measure OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Grind Left Wheel Hight Measure OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "左侧研磨区工作台高度测量完毕";
            }
            else if (Msg.Equals("Grind Left Dresser Thickness Measure OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Grind Left Dresser Thickness Measure OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "左侧研磨区磨刀完毕";
            }
            else if (Msg.Equals("Grind Left Table Thickness Measure OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Grind Left Table Thickness Measure OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "左侧研磨区手动研磨完毕";
            }
            else if (Msg.Equals("Grind Left Dressing OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Grind Left Dressing OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "左侧研磨区产品高度测量完毕";
            }
            else if (Msg.Equals("Grind Left Manual Grinding OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Grind Left Manual Grinding OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "左侧研磨区产品高度测量完毕";
            }
            else if (Msg.Equals("Grind Left Strip Thickness Measure OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Grind Left Strip Thickness Measure OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "左侧研磨区水刀清洁完毕";
            }
            else if (Msg.Equals("Grind Left Strip Thickness Measure One OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Grind Left Strip Thickness Measure One OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "左侧研磨区探针测试完毕";
            }
            else if (Msg.Equals("Grind Left Water Knife OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Grind Left Water Knife OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "左侧研磨区工作台研磨完毕";
            }
            else if (Msg.Equals("Grind Left Probe Test Measure OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Grind Left Probe Test Measure OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "右侧研磨区复位完毕";
            }
            else if (Msg.Equals("Grind Left Table Grinding OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Grind Left Table Grinding OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "右侧研磨区移动到等待位置完毕";
            }
            else if (Msg.Equals("Grind Right Home OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Grind Right Home OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "右侧研磨区砂轮高度测量完毕";
            }
            else if (Msg.Equals("Grind Right Move Wait Position OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Grind Right Move Wait Position OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "右侧研磨区磨刀石高度测量完毕";
            }
            else if (Msg.Equals("Grind Right Wheel Height Measure OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Grind Right Wheel Height Measure OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "右侧研磨区工作台高度测量完毕";
            }
            else if (Msg.Equals("Grind Right Dresser Thickness Measure OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Grind Right Dresser Thickness Measure OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "右侧研磨区磨刀完毕";
            }
            else if (Msg.Equals("Grind Right Table Thickness Measure OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Grind Right Table Thickness Measure OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "右侧研磨区手动研磨完毕";
            }
            else if (Msg.Equals("Grind Right Dressing OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Grind Right Dressing OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "右侧研磨区产品高度测量完毕";
            }
            else if (Msg.Equals("Grind Right Manual Grinding OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Grind Right Manual Grinding OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "右侧研磨区产品高度测量完毕";
            }
            else if (Msg.Equals("Grind Right Strip Thickness Measure OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Grind Right Strip Thickness Measure OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "右侧研磨区水刀清洁完毕";
            }
            else if (Msg.Equals("Grind Right Strip Thickness Measure One OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Grind Right Strip Thickness Measure One OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "右侧研磨区探针测试完毕";
            }
            else if (Msg.Equals("Grind Right Water Knife OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Grind Right Water Knife OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "右侧研磨区工作台研磨完毕";
            }
            else if (Msg.Equals("Grind Right Probe Test Measure OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Grind Right Probe Test Measure OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "下料区抓手复位完毕";
            }
            else if (Msg.Equals("Grind Right Table Grinding OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Grind Right Table Grinding OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "下料区抓手移动到等待位置完毕";
            }
            else if (Msg.Equals("Off Loader Picker Home OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Off Loader Picker Home OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "下料区抓手于左侧工作台抓取完毕";
            }
            else if (Msg.Equals("Off Loader Picker Move Wait Position OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Off Loader Picker Move Wait Position OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "下料区抓手于右侧工作台抓取完毕";
            }
            else if (Msg.Equals("Off Loader Picker Left Table Pick OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Off Loader Picker Left Table Pick OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "下料区抓手甩干区放置完毕";
            }
            else if (Msg.Equals("Off Loader Picker Right Table Pick OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Off Loader Picker Right Table Pick OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "下料区抓手抓手清洁完毕";
            }
            else if (Msg.Equals("Off Loader Picker Dry Place OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Off Loader Picker Dry Place OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "下料区抓手产品清洁完毕";
            }
            else if (Msg.Equals("Off Loader Picker Picker Cleaning OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Off Loader Picker Picker Cleaning OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "下料区抓手传送完毕";
            }
            else if (Msg.Equals("Off Loader Picker Strip Cleaning OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Off Loader Picker Strip Cleaning OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "甩干区复位完毕";
            }
            else if (Msg.Equals("Off Loader Picker Conversion OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Off Loader Picker Conversion OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "甩干区移动到等待位置完毕";
            }
            else if (Msg.Equals("Dry Home OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Dry Home OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "甩干区感应器检查完毕";
            }
            else if (Msg.Equals("Dry Move Wait Position OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Dry Move Wait Position OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "甩干区产品送出完毕";
            }
            else if (Msg.Equals("Dry Check Safty Sensor OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Dry Check Safty Sensor OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "甩干区产品送出完毕";
            }
            else if (Msg.Equals("Dry Strip Out OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Dry Strip Out OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "甩干区运行完毕";
            }
            else if (Msg.Equals("Dry Work OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Dry Work OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "下料区移动到等待位置完毕";
            }
            else if (Msg.Equals("Off Loader Move Wait Position OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Off Loader Move Wait Position OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "下料区复位完毕";
            }
            else if (Msg.Equals("Off Loader Home OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Off Loader Home OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "下料区上方传送区抓取";
            }
            else if (Msg.Equals("Off Loader Top Conveyor Pick OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Off Loader Top Conveyor Pick OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "下料区上方传送区放置完毕";
            }
            else if (Msg.Equals("Off Loader Top Conveyor Place OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Off Loader Top Conveyor Place OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "下料区上方传送区抓取完毕";
            }
            else if (Msg.Equals("Off Loader Top Conveyor Receive OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Off Loader Top Conveyor Receive OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "下料区下方传送区抓取完毕";
            }
            else if (Msg.Equals("Off Loader Bottom Conveyor Pick OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Off Loader Bottom Conveyor Pick OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "下料区下方传送区抓取完毕";
            }
            else if (Msg.Equals("Off Loader Bottom Conveyor Pick OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Off Loader Bottom Conveyor Pick OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "下料区下方传送区抓取完毕";
            }
            else if (Msg.Equals("Off Loader Bottom Conveyor Place OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Off Loader Bottom Conveyor Place OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "下料区下方传送区放置完毕"; 
            }
            else if (Msg.Equals("Off Loader Bottom Conveyor Receive OK", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Off Loader Bottom Conveyor Receive OK";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "下料区下方传送区抓取完毕";
            }
            else if (Msg.Equals("Strip exist on the table. Please check", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Strip exist on the table. Please check";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "请检查是否产品在工作台上";
            }
            else if (Msg.Equals("Need Warm Up Please", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Need Warm Up Please";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "请暖机";
            }
            else if (Msg.Equals("One more check strip on each Table. Please", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "One more check strip on each Table. Please";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "请再一次确认工作台表面是否有产品";
            }
            else if (Msg.Equals("One more check strip on each Table. Please", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "One more check strip on each Table. Please";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "请再一次确认工作台表面是否有产品";
            }
            else if (Msg.Equals("One more check strip on each Table. Please", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "One more check strip on each Table. Please";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "请再一次确认工作台表面是否有产品";
            }
            else if (Msg.Equals("Left Table Skip Now!", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Left Table Skip Now!";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "请再一次确认工作台表面是否有产品工作台屏蔽中!";
            }
            else if (Msg.Equals("Right Table Skip Now!", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Right Table Skip Now!";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "右侧工作台屏蔽中!";
            }
            else if (Msg.Equals("One More Check Strip Jig. Please", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "One More Check Strip Jig. Please";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "请再次确认夹具是否移除";
            }
            else if (Msg.Equals("Grinding Cover Check Please", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Grinding Cover Check Please";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "请检查研磨区盖板";
            }
            else if (Msg.Equals("Different the device & BCR Recipe", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Different the device & BCR Recipe";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "程序名不同 请确认";
            }
            else if (Msg.Equals("During Before Manual Run now, So wait finish run.", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "During Before Manual Run now, So wait finish run.";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "请等待当至前运行结束.";
            }
            else if (Msg.Equals("Can't Both Select Skip Table", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Can't Both Select Skip Table";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "无法同时屏蔽两个工作平台";
            }
            else if (Msg.Equals("Magazin Or Strip Remove Please", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Magazin Or Strip Remove Please";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "料盒或产品请移除";
            }
            else if (Msg.Equals("Do you want LOT end?", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Do you want LOT end?";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "是否需要结束当前批次?";
            }
            else if (Msg.Equals("First Lot Cancel and after Save", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "First Lot Cancel and after Save";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "先取消后再保存";
            }
            else if (Msg.Equals("Device file save complete!!", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Device file save complete!!";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "程序保存完毕!!";
            }
            else if (Msg.Equals("Group name exist !!!", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Group name exist !!!";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "组别名字已经存在 !!!";
            }
            else if (Msg.Equals("Not selected group", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Not selected group";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "未选组别";
            }
            else if (Msg.Equals("Group name exist !!!", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Group name exist !!!";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "组别名字已经存在 !!!";
            }
            else if (Msg.Equals("Not selected group", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Not selected group";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "未选组别";
            }
            /*
            else if (Msg.Equals("Do you want delete " + m_sGrp + " group?", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)eLanguage.English == CData.Opt.iSelLan) return "Do you want delete " + m_sGrp + " group?";
                else if ((int)eLanguage.Korea == CData.Opt.iSelLan) return "";
                else if ((int)eLanguage.China == CData.Opt.iSelLan) return "你想删除 " + m_sGrp + " 组?";
            }
            */
            else if (Msg.Equals("Not selected group", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Not selected group";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "未选组别";
            }
            else if (Msg.Equals("Device name exist !!!", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Device name exist !!!";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "程序名存在 !!!";
            }
            else if (Msg.Equals("Device file create success", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Device file create success";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "程序名创建成功";
            }
            else if (Msg.Equals("Not selected group", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Not selected group";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "未选组别";
            }
            else if (Msg.Equals("Not selected device", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Not selected device";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "未选程序";
            }
            else if (Msg.Equals("Device name same !!!", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Device name same !!!";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "程序名相同 !!!";
            }
            else if (Msg.Equals("Device name exist !!!", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Device name exist !!!";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "程序名存在 !!!";
            }
            else if (Msg.Equals("Not selected group", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Not selected group";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "未选组别";
            }
            else if (Msg.Equals("Not selected device", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Not selected device";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "未选程序";
            }
            /*
            else if (Msg.Equals("Do you want delete " + m_sDev + " device?", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)eLanguage.English == CData.Opt.iSelLan) return "Do you want delete " + m_sDev + " device?";
                else if ((int)eLanguage.Korea == CData.Opt.iSelLan) return "";
                else if ((int)eLanguage.China == CData.Opt.iSelLan) return "你想删除 " + m_sDev + " 程序?";
            }
            */
            else if (Msg.Equals("Not selected device", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Not selected device";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "未选程序";
            }
            else if (Msg.Equals("Check Onloader Picker", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Check Onloader Picker";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "检查上料区抓手";
            }
            else if (Msg.Equals("Check Onloader Picker Z Axis", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Check Onloader Picker Z Axis";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "检查上料区抓手 Z 向";
            }
            else if (Msg.Equals("Check Offloader Picker X Axis Pos", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Check Offloader Picker X Axis Pos";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "检查下料区抓手 X 向位置";
            }
            else if (Msg.Equals("Check Onloader Picker X Axis Pos", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Check Onloader Picker X Axis Pos";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "检查上料区抓手 X 向位置";
            }
            else if (Msg.Equals("Check Onloader Picker X Axis Pos", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Check Onloader Picker X Axis Pos";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "检查上料区抓手 X 向位置";
            }
            else if (Msg.Equals("Check Onloader Picker X Axis Pos", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Check Onloader Picker X Axis Pos";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "检查上料区抓手 X 向位置";
            }
            else if (Msg.Equals("Check Onloader Picker X Axis Pos", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Check Onloader Picker X Axis Pos";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "检查上料区抓手 X 向位置";
            }
            else if (Msg.Equals("Check Left Table Z Axis", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Check Left Table Z Axis";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "检查左侧工作台Z 向";
            }
            else if (Msg.Equals("Check OnLoader Picker Z Axis", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Check OnLoader Picker Z Axis";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "检查上料区抓手 Z 向";
            }
            else if (Msg.Equals("Check OffLoader Picker Z Axis", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Check OffLoader Picker Z Axis";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "检查下料区抓手 Z 向";
            }
            else if (Msg.Equals("Check Left Table Z Axis", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Check Left Table Z Axis";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "检查左侧工作台Z 向";
            }
            else if (Msg.Equals("Check Left Table Probe", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Check Left Table Probe";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "检查左侧工作台探针";
            }
            else if (Msg.Equals("Check Left Table Z Axis", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Check Left Table Z Axis";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "检查左侧工作台Z 向";
            }
            else if (Msg.Equals("Check OnLoader Picker Z Axis", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Check OnLoader Picker Z Axis";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "检查上料区抓手 Z 向";
            }
            else if (Msg.Equals("Check OffLoader Picker Z Axis", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Check OffLoader Picker Z Axis";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "检查下料区抓手 Z 向";
            }
            else if (Msg.Equals("Check Right Table Z Axis", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Check Right Table Z Axis";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "检查Right工作台Z 向";
            }
            else if (Msg.Equals("Check Right Table Probe", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Check Right Table Probe";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "检查Right工作台探针";
            }
            else if (Msg.Equals("Check Offloader Picker Z Axis", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Check Offloader Picker Z Axis";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "检查下料区抓手 Z 向";
            }
            else if (Msg.Equals("Check Onloader Picker X Axis Pos", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Check Onloader Picker X Axis Pos";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "检查上料区抓手 X 向位置";
            }
            else if (Msg.Equals("Check Offloader Picker X Axis Pos", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Check Onloader Picker X Axis Pos";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "检查下料区抓手 X 向位置";
            }
            else if (Msg.Equals("Check Offloader Picker X Axis Pos", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Check Onloader Picker X Axis Pos";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "检查下料区抓手 X 向位置";
            }
            else if (Msg.Equals("Check Offloader Picker X Axis Pos", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Check Onloader Picker X Axis Pos";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "检查下料区抓手 X 向位置";
            }
            else if (Msg.Equals("Check Offloader Picker X Axis Pos", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Check Onloader Picker X Axis Pos";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "检查下料区抓手 X 向位置";
            }
            else if (Msg.Equals("Check Offloader Picker Z Axis", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Check Offloader Picker Z Axis";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "检查下料区抓手 Z 向";
            }
            else if (Msg.Equals("Check Dry Z Axis Pos", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Check Dry Z Axis Pos";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "检查甩干区 Z 向位置";
            }
            else if (Msg.Equals("Check OffLoader Y Axis Pos", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Check OffLoader Y Axis Pos";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "检查下料区 Y 向位置";
            }
            else if (Msg.Equals("Check OffLoader Z Axis Pos", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Check OffLoader Z Axis Pos";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "检查下料区 Z 向位置";
            }
            else if (Msg.Equals("Check OffLoader Magazin", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Check OffLoader Magazin";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "检查下料区料盒";
            }
            else if (Msg.Equals("Check Dry R Axis", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Check Dry R Axis";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "检查甩干区 R 向";
            }
            else if (Msg.Equals("Check Dry Z Axis", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Check Dry Z Axis";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "检查甩干区 Z 向";
            }
            else if (Msg.Equals("During Before Manual Run now, So wait finish run.", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "During Before Manual Run now, So wait finish run";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "请等待当至前运行结束.";
            }
            else if (Msg.Equals("During Before Manual Run now, So wait finish run.", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "During Before Manual Run now, So wait finish run";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "请等待当至前运行结束.";
            }
            else if (Msg.Equals("During Before Manual Run now, So wait finish run.", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "During Before Manual Run now, So wait finish run";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "请等待当至前运行结束.";
            }
            else if (Msg.Equals("Strip exist on the table. Please check", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Strip exist on the table. Please check";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "请检查是否产品在工作台上";
            }
            else if (Msg.Equals("Need Warm Up Please", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Need Warm Up Please";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "请暖机";
            }
            else if (Msg.Equals("One more check strip on each Table. Please", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "One more check strip on each Table. Please";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "请再一次确认工作台表面是否有产品";
            }
            else if (Msg.Equals("One more check strip on each Table. Please", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "One more check strip on each Table. Please";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "请再一次确认工作台表面是否有产品";
            }
            else if (Msg.Equals("One more check strip on each Table. Please", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "One more check strip on each Table. Please";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "请再一次确认工作台表面是否有产品";
            }
            else if (Msg.Equals("Left Table Skip Now!", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Left Table Skip Now!";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "左侧工作台屏蔽中!";
            }
            else if (Msg.Equals("Right Table Skip Now!", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Right Table Skip Now!";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "右侧工作台屏蔽中!";
            }
            else if (Msg.Equals("One More Check Strip Jig. Please", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "One More Check Strip Jig. Please";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "请再次确认夹具是否移除";
            }
            else if (Msg.Equals("Grinding Cover Check Please", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Grinding Cover Check Please";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "请检查研磨区盖板";
            }
            else if (Msg.Equals("Different the device & BCR Recipe", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Different the device & BCR Recipe";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "程序名不同 请确认";
            }
            else if (Msg.Equals("Different the device & BCR Recipe", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Different the device & BCR Recipe";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "程序名不同 请确认";
            }
            else if (Msg.Equals("During Before Manual Run now, So wait finish run.", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "During Before Manual Run now, So wait finish run";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "请等待当至前运行结束.";
            }
            else if (Msg.Equals("Select Wheel File", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Select Wheel File";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "选择砂轮文档";
            }
            else if (Msg.Equals("Select Wheel File", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Select Wheel File";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "选择砂轮文档";
            }
            else if (Msg.Equals("Before Wheel measure", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Before Wheel measure";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "砂轮测高前";
            }
            else if (Msg.Equals("Before Wheel measure", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Before Wheel measure";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "砂轮测高前";
            }
            else if (Msg.Equals("Check wheel change pos", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Check wheel change pos";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "确认更换砂轮位置";
            }
            else if (Msg.Equals("Check wheel change complete", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Check wheel change complete";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "确认更换砂轮完毕";
            }
            else if (Msg.Equals("Check wheel Parameter View", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Check wheel Parameter View";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "确认砂轮参数";
            }
            else if (Msg.Equals("Check left wheel parameter change", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Check left wheel parameter change";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "确认左侧砂轮参数变化";
            }
            else if (Msg.Equals("Check Right wheel parameter change", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Check Right wheel parameter change";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "确认右侧砂轮参数变化";
            }
            else if (Msg.Equals("Check Wheel measure", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Check Wheel measure";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "确认砂轮高度";
            }
            else if (Msg.Equals("Check Wheel measure", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Check Wheel measure";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "确认砂轮高度";
            }
            else if (Msg.Equals("Check Wheel dressing", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Check Wheel dressing";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "确认磨刀";
            }
            else if (Msg.Equals("Check Wheel dressing", StringComparison.OrdinalIgnoreCase))
            {
                if ((int)ELang.English == CData.Opt.iSelLan) return "Check Wheel dressing";
                else if ((int)ELang.Korea == CData.Opt.iSelLan) return "";
                else if ((int)ELang.China == CData.Opt.iSelLan) return "确认磨刀";
            }           
            return Msg;
        }

        public string GetSeq(ESeq eSeq)
        {
            if (eSeq == ESeq.GRL_Wait || eSeq == ESeq.GRR_Wait)
            { return "Wait"; }
            else if (eSeq == ESeq.GRL_Ready || eSeq == ESeq.GRR_Ready)
            { return "Ready"; }
            else if (eSeq == ESeq.GRL_WaitPick || eSeq == ESeq.GRR_WaitPick)
            { return "Wait Picker"; }
            else if (eSeq == ESeq.GRL_Dressing || eSeq == ESeq.GRR_Dressing)
            { return "Dressing"; }
            else if (eSeq == ESeq.GRL_Grinding || eSeq == ESeq.GRR_Grinding)
            { return "Grinding"; }
            else if (eSeq == ESeq.GRL_WaterKnife || eSeq == ESeq.GRR_WaterKnife)
            { return "Water Knife"; }
            else if (eSeq == ESeq.GRL_Home || eSeq == ESeq.GRR_Home)
            { return "Homing"; }
            else if (eSeq == ESeq.GRL_Table_Measure || eSeq == ESeq.GRR_Table_Measure)
            { return "Measure Table"; }
            else if (eSeq == ESeq.GRL_Strip_Measure || eSeq == ESeq.GRR_Strip_Measure)
            { return "Measure Strip"; }
            else if (eSeq == ESeq.GRL_Strip_Measureone || eSeq == ESeq.GRR_Strip_Measureone)
            { return "Measure One Point"; }
            else if (eSeq == ESeq.GRL_Dresser_Measure || eSeq == ESeq.GRR_Dresser_Measure)
            { return "Measure Dresser"; }
            else if (eSeq == ESeq.GRL_Wheel_Measure || eSeq == ESeq.GRR_Wheel_Measure)
            { return "Measure Wheel"; }
            else if (eSeq == ESeq.GRL_Table_Grinding || eSeq == ESeq.GRR_Table_Grinding)
            { return "Table Grinding"; }
            else if (eSeq == ESeq.GRL_Table_Grinding_Wheel_Measure || eSeq == ESeq.GRR_Table_Grinding_Wheel_Measure)
            { return "Tbl Measure Wheel"; }
            else if (eSeq == ESeq.GRL_Table_Grinding_Table_Measure || eSeq == ESeq.GRR_Table_Grinding_Table_Measure)
            { return "Tbl Measure Table"; }
            else if (eSeq == ESeq.GRL_Table_Grinding_Work || eSeq == ESeq.GRR_Table_Grinding_Work)
            { return "Table Grinding Work"; }
            else if (eSeq == ESeq.GRL_WorkEnd || eSeq == ESeq.GRR_WorkEnd)
            { return "Work End"; }
            else if (eSeq == ESeq.GRL_WorkEnd || eSeq == ESeq.GRR_WorkEnd)
            { return "Work End"; }
            else if (eSeq == ESeq.GRL_ProbeTest || eSeq == ESeq.GRR_ProbeTest)
            { return "Probe Test"; }
            else if (eSeq == ESeq.GRL_Table_MeasureSix || eSeq == ESeq.GRR_Table_MeasureSix)
            { return "Measure Table Six"; }
            else if (eSeq == ESeq.GRL_Table_Measure_8p || eSeq == ESeq.GRR_Table_Measure_8p)    // 200928 lhs
            { return "Measure Table 8 point"; }
            else if (eSeq == ESeq.GRL_AfterMeaStrip || eSeq == ESeq.GRR_AfterMeaStrip)
            { return "Measure After"; }
            else if (eSeq == ESeq.GRL_Table_Clean || eSeq == ESeq.GRR_Table_Clean)
            { return "Table Cleaning"; }
            else if (eSeq == ESeq.GRL_Manual_GrdBcr || eSeq == ESeq.GRR_Manual_GrdBcr)
            { return "Manual Grind BCR"; }
            else if (eSeq == ESeq.GRL_Manual_Bcr || eSeq == ESeq.GRR_Manual_Bcr)
            { return "Manual BCR"; }
            else
            { return eSeq.ToString(); }
        }
    }

}
