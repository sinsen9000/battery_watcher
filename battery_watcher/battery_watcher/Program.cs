using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace battery_watcher
{
    class Program
    {
        
        static void Main(string[] args)
        {
            Microsoft.VisualBasic.Devices.Audio audio =
                new Microsoft.VisualBasic.Devices.Audio();
            PowerLineStatus pls = SystemInformation.PowerStatus.PowerLineStatus;
            bool battery = false; //バッテリーの状態（True=充電中、False=バッテリー使用中）
            bool battery_80 = false; //バッテリーが80%かどうか
            string current = @"C:\battery_watcher\";
            switch (pls)
            {
                case PowerLineStatus.Offline: //充電状態ではない
                    //バックグラウンドでWAVを再生する
                    audio.Play(current+"no_connecting.wav",
                        Microsoft.VisualBasic.AudioPlayMode.WaitToComplete);
                    battery = false;
                    break;
                case PowerLineStatus.Online: //充電状態
                    audio.Play(current+"connecting.wav",
                        Microsoft.VisualBasic.AudioPlayMode.WaitToComplete);
                    battery = true;
                    break;
            }
            while (true)
            {
                //バッテリーの充電状態を取得する
                BatteryChargeStatus bcs =
                    SystemInformation.PowerStatus.BatteryChargeStatus;
                pls = SystemInformation.PowerStatus.PowerLineStatus;
                //充電中
                if (pls == PowerLineStatus.Online) //バッテリー検知
                {
                    if (battery == false)
                    {
                        audio.Play(current + "battery_to_AC.wav",
                        Microsoft.VisualBasic.AudioPlayMode.WaitToComplete);
                        battery = true;
                    }
                    if ((bcs != BatteryChargeStatus.Unknown) && ((bcs & BatteryChargeStatus.Charging) ==
                        BatteryChargeStatus.Charging))
                    {
                        //バッテリー残量の確認（割合）
                        float blp = SystemInformation.PowerStatus.BatteryLifePercent;
                        if(blp*100 >= 80) //80%（実際は79%の時も）に到達したら通知
                        {
                            if (battery_80 == false)
                            {
                                audio.Play(current+"battery_80.wav",
                                    Microsoft.VisualBasic.AudioPlayMode.WaitToComplete);
                                battery_80 = true;
                            }
                        }
                        else
                        {
                            battery_80 = false;
                        }
                    }
                }
                //バッテリー駆動
                else if(pls == PowerLineStatus.Offline)
                {
                    if (battery)
                    {
                        audio.Play(current+"AC_to_battery.wav",
                            Microsoft.VisualBasic.AudioPlayMode.WaitToComplete);
                        battery = false;
                    }
                }
                Thread.Sleep(500);
            }
        }
    }
}
