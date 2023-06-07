using System;
using System.Threading;
using System.Windows.Forms;

/// <summary>
/// Global Util
/// </summary>
public class GU
{
    /// <summary>
    /// Delay 함수
    /// </summary>
    /// <param name="iTime">밀리초 단위 설정</param>
    /// <returns></returns>
    public static DateTime Delay(int iTime)
    {

        DateTime dtNow = DateTime.Now;
        TimeSpan tSpan = new TimeSpan(0, 0, 0, 0, iTime);
        DateTime dtTar = dtNow.Add(tSpan);

        Application.DoEvents();
        while (dtTar >= dtNow)
        {
            Application.DoEvents();
            dtNow = DateTime.Now;
            Thread.Sleep(1);
        }

        return DateTime.Now;
    }

    /// <summary>
    /// 10진수 -> 16진수
    /// </summary>
    /// <param name="iSrc">10진수</param>
    /// <returns></returns>
    public static string ToHex(int iSrc)
    {
        string strHex = iSrc.ToString("X");
        if (strHex.Length % 2 != 0)
            strHex = String.Format("0{0}", strHex);

        return strHex;
    }

    /// <summary>
    /// 16진수 -> 10진수
    /// </summary>
    /// <param name="sSrc">16진수</param>
    /// <returns>Int32</returns>
    public static int ToDec(string sSrc)
    {
        return Convert.ToInt32(sSrc, 16);
    }

    /// <summary>
    /// 16진수 -> 10진수
    /// </summary>
    /// <param name="sSrc">16진수</param>
    /// <returns>Int64</returns>
    public static long ToLong(string sSrc)
    {
        return Convert.ToInt64(sSrc, 16);
    }

    /// <summary>
    /// 소수점 제거 함수
    /// </summary>
    /// <param name="dSrc"></param>
    /// <param name="iIdx"></param>
    /// <returns></returns>
    public static double Truncate(double dSrc, int iIdx)
    {
        double dRet;
        dRet = dSrc * Math.Pow(10.0, iIdx);
        dRet = Math.Truncate(dRet);
        dRet = dRet / Math.Pow(10.0, iIdx);
        return dRet;
    }

    //201225 jhc
    /// <summary>
    /// 두 정수형 변수값 교환 함수 
    /// </summary>
    /// <param name="n1"></param>
    /// <param name="n2"></param>
    public static void Swap(ref int x, ref int y)
    {
        int tmp = x;
        x = y;
        y = tmp;
    }
    /// <summary>
    /// 두 double 형 변수값 교환 함수
    /// </summary>
    /// <param name="d1"></param>
    /// <param name="d2"></param>
    public static void Swap(ref double x, ref double y)
    {
        double tmp = x;
        x = y;
        y = tmp;
    }
    /// <summary>
    /// 두 정수형 변수값을 비교하여 작은 순서로 정렬되도록 값 교환 (작은 값이 첫번째 인자 값에 할당됨)
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public static void Arrange(ref int x, ref int y)
    {
        if (x > y)
        {
            Swap(ref x, ref y);
        }
    }
    /// <summary>
    /// 두 실수형 변수값을 비교하여 작은 순서로 정렬되도록 값 교환 (작은 값이 첫번째 인자 값에 할당됨)
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public static void Arrange(ref double x, ref double y)
    {
        if (x > y)
        {
            Swap(ref x, ref y);
        }
    }
    //..
}
