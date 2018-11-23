﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DirectConnectionPredictControl.CommenTool;

namespace DirectConnectionPredictControl
{
    /// <summary>
    /// OverviewWindow.xaml 的交互逻辑
    /// </summary>
    public partial class OverviewWindow : Window
    {
        /// <summary>
        /// 数据组
        /// </summary>
        private MainDevDataContains container_1 = new MainDevDataContains();
        private MainDevDataContains container_6 = new MainDevDataContains();
        private SliverDataContainer container_2 = new SliverDataContainer();
        private SliverDataContainer container_3 = new SliverDataContainer();
        private SliverDataContainer container_4 = new SliverDataContainer();
        private SliverDataContainer container_5 = new SliverDataContainer();

        /// <summary>
        /// 线程组
        /// </summary>
        private Thread uiThread;


        public event closeWindowHandler CloseWindowEvent;
        public OverviewWindow()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            uiThread = new Thread(UpdateUI);
            uiThread.Start();
        }

        #region bussiness method
        private void UpdateUI()
        {
            while (true)
            {
                this.Dispatcher.Invoke(() =>
                {
                    currentTimeLbl.Content = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    UpdateAO();
                    UpdateDI();
                    UpdateDO();
                    UpdateFA();
                    UpdateWSP();
                });
                Thread.Sleep(Utils.timeInterval);
            }
            
        }

        private void UpdateWSP()
        {
            row_1_column_1_sli.Content = container_1.SlipLvl1 + " / " + container_1.SlipLvl2;
            row_1_column_2_sli.Content = container_2.SlipLvl1 + " / " + container_2.SlipLvl2;
            row_1_column_3_sli.Content = container_3.SlipLvl1 + " / " + container_3.SlipLvl2;
            row_1_column_4_sli.Content = container_4.SlipLvl1 + " / " + container_4.SlipLvl2;
            row_1_column_5_sli.Content = container_5.SlipLvl1 + " / " + container_5.SlipLvl2;
            row_1_column_6_sli.Content = container_6.SlipLvl1 + " / " + container_6.SlipLvl2;

            row_1_column_8_sli.Content = container_1.AccValue1 + " / " + container_1.AccValue2;
            row_1_column_9_sli.Content = container_2.AccValue1 + " / " + container_2.AccValue2;
            row_1_column_10_sli.Content = container_3.AccValue1 + " / " + container_3.AccValue2;
            row_1_column_11_sli.Content = container_4.AccValue1 + " / " + container_4.AccValue2;
            row_1_column_12_sli.Content = container_5.AccValue1 + " / " + container_5.AccValue2;
            row_1_column_13_sli.Content = container_6.AccValue1 + " / " + container_6.AccValue2;

            row_2_column_1_sli.Content = container_1.DcuVolta[0] + " V";
            row_3_column_8_sli.Content = container_1.DcuVolta[3] + " V";
            row_3_column_1_sli.Content = container_1.DcuVolta[1] + " V";
            row_2_column_8_sli.Content = container_1.DcuVolta[2] + " V";
        }

        /// <summary>
        /// 更新主UI模拟量
        /// </summary>
        private void UpdateAO()
        {
            row_1_column_1.Content = Check_Double(container_1.RefSpeed, 1);
            row_1_column_6.Content = Check_Double(container_6.RefSpeed, 6);
            row_2_column_1.Content = Check_Int(container_1.BrakeLevel, 1);
            row_2_column_6.Content = Check_Int(container_6.BrakeLevel, 6);
            row_3_column_1.Content = Check_Int(container_1.TrainBrakeForce, 1);
            row_3_column_6.Content = Check_Int(container_6.TrainBrakeForce, 6);

            //生命信号
            row_4_column_1.Content = container_1.LifeSig;
            row_4_column_2.Content = container_2.LifeSig;
            row_4_column_3.Content = container_3.LifeSig;
            row_4_column_4.Content = container_4.LifeSig;
            row_4_column_5.Content = container_5.LifeSig;
            row_4_column_6.Content = container_6.LifeSig;

            //速度信号
            row_5_column_1.Content = string.Format(Utils.formatN1, container_1.SpeedA1Shaft1);
            row_5_column_2.Content = string.Format(Utils.formatN1, container_2.SpeedShaft1);
            row_5_column_3.Content = string.Format(Utils.formatN1, container_3.SpeedShaft1);
            row_5_column_4.Content = string.Format(Utils.formatN1, container_4.SpeedShaft1);
            row_5_column_5.Content = string.Format(Utils.formatN1, container_5.SpeedShaft1);
            row_5_column_6.Content = string.Format(Utils.formatN1, container_6.SpeedA1Shaft1);
            
            row_6_column_1.Content = string.Format(Utils.formatN1, container_1.SpeedA1Shaft2);
            row_6_column_2.Content = string.Format(Utils.formatN1, container_2.SpeedShaft2);
            row_6_column_3.Content = string.Format(Utils.formatN1, container_3.SpeedShaft2);
            row_6_column_4.Content = string.Format(Utils.formatN1, container_4.SpeedShaft2);
            row_6_column_5.Content = string.Format(Utils.formatN1, container_5.SpeedShaft2);
            row_6_column_6.Content = string.Format(Utils.formatN1, container_6.SpeedA1Shaft2);

            //空气制动目标值
            row_7_column_1.Content = Check(container_1.AbTargetValueAx1, 1) + " / " + Check(container_6.AbTargetValueAx1, 6);
            row_7_column_2.Content = Check(container_1.AbTargetValueAx2, 1) + " / " + Check(container_6.AbTargetValueAx2, 6);
            row_7_column_3.Content = Check(container_1.AbTargetValueAx3, 1) + " / " + Check(container_6.AbTargetValueAx3, 6);
            row_7_column_4.Content = Check(container_1.AbTargetValueAx4, 1) + " / " + Check(container_6.AbTargetValueAx4, 6);
            row_7_column_5.Content = Check(container_1.AbTargetValueAx5, 1) + " / " + Check(container_6.AbTargetValueAx5, 6);
            row_7_column_6.Content = Check(container_1.AbTargetValueAx6, 1) + " / " + Check(container_6.AbTargetValueAx6, 6);

            //1/6架制动风缸压力
            row_8_column_1.Content = container_1.BrakeCylinderSourcePressure;
            row_8_column_2.Content = container_2.BrakeCylinderSourcePressure;
            row_8_column_3.Content = container_3.BrakeCylinderSourcePressure;
            row_8_column_4.Content = container_4.BrakeCylinderSourcePressure;
            row_8_column_5.Content = container_5.BrakeCylinderSourcePressure;
            row_8_column_6.Content = container_6.BrakeCylinderSourcePressure;

            //1/6架空簧压力
            row_9_column_1.Content = container_1.AirSpring1PressureA1Car1;
            row_9_column_2.Content = container_2.AirSpringPressure1;
            row_9_column_3.Content = container_3.AirSpringPressure1;
            row_9_column_4.Content = container_4.AirSpringPressure1;
            row_9_column_5.Content = container_5.AirSpringPressure1;
            row_9_column_6.Content = container_6.AirSpring1PressureA1Car1;

            row_10_column_1.Content = container_1.AirSpring2PressureA1Car1;
            row_10_column_2.Content = container_2.AirSpringPressure2;
            row_10_column_3.Content = container_3.AirSpringPressure2;
            row_10_column_4.Content = container_4.AirSpringPressure2;
            row_10_column_5.Content = container_5.AirSpringPressure2;
            row_10_column_6.Content = container_6.AirSpring2PressureA1Car1;

            //停放压力
            row_11_column_1.Content = container_1.ParkPressureA1;
            row_11_column_3.Content = container_3.ParkPressure;
            row_11_column_5.Content = container_5.ParkPressure;

            //VLD实际
            row_12_column_1.Content = container_1.VldRealPressureAx1;
            row_12_column_2.Content = container_2.VldRealPressure;
            row_12_column_3.Content = container_3.VldRealPressure;
            row_12_column_4.Content = container_4.VldRealPressure;
            row_12_column_5.Content = container_5.VldRealPressure;
            row_12_column_6.Content = container_6.VldRealPressureAx1;

            //1/6架制动缸压力
            row_13_column_1.Content = container_1.Bcp1PressureAx1;
            row_13_column_2.Content = container_2.Bcp1Pressure;
            row_13_column_3.Content = container_3.Bcp1Pressure;
            row_13_column_4.Content = container_4.Bcp1Pressure;
            row_13_column_5.Content = container_5.Bcp1Pressure;
            row_13_column_6.Content = container_6.Bcp1PressureAx1;

            row_14_column_1.Content = container_1.Bcp2PressureAx2;
            row_14_column_2.Content = container_2.Bcp2Pressure;
            row_14_column_3.Content = container_3.Bcp2Pressure;
            row_14_column_4.Content = container_4.Bcp2Pressure;
            row_14_column_5.Content = container_5.Bcp2Pressure;
            row_14_column_6.Content = container_6.Bcp2PressureAx2;

            //VLD设定
            row_15_column_1.Content = container_1.VldPressureSetupAx1;
            row_15_column_2.Content = container_2.VldSetupPressure;
            row_15_column_3.Content = container_3.VldSetupPressure;
            row_15_column_4.Content = container_4.VldSetupPressure;
            row_15_column_5.Content = container_5.VldSetupPressure;
            row_15_column_6.Content = container_6.VldPressureSetupAx1;

            //1/6架载荷
            row_16_column_1.Content = container_1.MassA1;
            row_16_column_2.Content = container_2.MassValue;
            row_16_column_3.Content = container_3.MassValue;
            row_16_column_4.Content = container_4.MassValue;
            row_16_column_5.Content = container_5.MassValue;
            row_16_column_6.Content = container_6.MassA1;

            //架1自检目标设定值
            row_17_column_1.Content = container_1.SelfTestSetup;
            row_17_column_2.Content = container_2.SelfTestSetup;
            row_17_column_3.Content = container_3.SelfTestSetup;
            row_17_column_4.Content = container_4.SelfTestSetup;
            row_17_column_5.Content = container_5.SelfTestSetup;
            row_17_column_6.Content = container_6.SelfTestSetup;

            //VCM & DCU
            row_18_column_1.Content = Check_Int(container_1.VCMLifeSig, 1);
            row_18_column_6.Content = Check_Int(container_6.VCMLifeSig, 6);

            row_19_column_1.Content = Check_Int(container_1.DcuLifeSig[0], 1);
            row_19_column_6.Content = Check_Int(container_6.DcuLifeSig[0], 6);

            row_20_column_1.Content = Check_Int(container_1.DcuLifeSig[1], 1);
            row_20_column_6.Content = Check_Int(container_6.DcuLifeSig[1], 6);

            row_21_column_1.Content = Check_Int(container_1.DcuLifeSig[2], 1);
            row_21_column_6.Content = Check_Int(container_6.DcuLifeSig[2], 6);

            row_22_column_1.Content = Check_Int(container_1.DcuLifeSig[3], 1);
            row_22_column_6.Content = Check_Int(container_6.DcuLifeSig[3], 6);

            row_23_column_1.Content = Check_Int(container_1.DcuEbRealValue[0], 1);
            row_23_column_6.Content = Check_Int(container_6.DcuEbRealValue[0], 6);

            row_24_column_1.Content = Check_Int(container_1.DcuMax[0], 1);
            row_24_column_6.Content = Check_Int(container_6.DcuMax[0], 6);

            row_25_column_1.Content = Check_Int(container_1.DcuEbRealValue[1], 1);
            row_25_column_6.Content = Check_Int(container_6.DcuEbRealValue[1], 6);

            row_26_column_1.Content = Check_Int(container_1.DcuMax[1], 1);
            row_26_column_6.Content = Check_Int(container_6.DcuMax[1], 6);

            row_27_column_1.Content = Check_Int(container_1.DcuEbRealValue[2], 1);
            row_27_column_6.Content = Check_Int(container_6.DcuEbRealValue[2], 6);

            row_28_column_1.Content = Check_Int(container_1.DcuMax[2], 1);
            row_28_column_6.Content = Check_Int(container_6.DcuMax[2], 6);

            row_29_column_1.Content = Check_Int(container_1.DcuEbRealValue[3], 1);
            row_29_column_6.Content = Check_Int(container_6.DcuEbRealValue[3], 6);

            row_30_column_1.Content = Check_Int(container_1.DcuMax[3], 1);
            row_30_column_6.Content = Check_Int(container_6.DcuMax[3], 6);

            //空气制动能力值
            row_31_column_1.Content = Check(container_1.AbCapacity[0], 1) + " / " + Check(container_6.AbCapacity[0], 6);
            row_31_column_2.Content = Check(container_1.AbCapacity[1], 1) + " / " + Check(container_6.AbCapacity[1], 6);
            row_31_column_3.Content = Check(container_1.AbCapacity[2], 1) + " / " + Check(container_6.AbCapacity[2], 6);
            row_31_column_4.Content = Check(container_1.AbCapacity[3], 1) + " / " + Check(container_6.AbCapacity[3], 6);
            row_31_column_5.Content = Check(container_1.AbCapacity[4], 1) + " / " + Check(container_6.AbCapacity[4], 6);
            row_31_column_6.Content = Check(container_1.AbCapacity[5], 1) + " / " + Check(container_6.AbCapacity[5], 6);

            //空气制动实际值
            row_32_column_1.Content = Check(container_1.AbRealValue[0], 1) + " / " + Check(container_6.AbRealValue[0], 6);
            row_32_column_2.Content = Check(container_1.AbRealValue[1], 1) + " / " + Check(container_6.AbRealValue[1], 6);
            row_32_column_3.Content = Check(container_1.AbRealValue[2], 1) + " / " + Check(container_6.AbRealValue[2], 6);
            row_32_column_4.Content = Check(container_1.AbRealValue[3], 1) + " / " + Check(container_6.AbRealValue[3], 6);
            row_32_column_5.Content = Check(container_1.AbRealValue[4], 1) + " / " + Check(container_6.AbRealValue[4], 6);
            row_32_column_6.Content = Check(container_1.AbRealValue[5], 1) + " / " + Check(container_6.AbRealValue[5], 6);

            //软件版本
            row_33_column_1.Content = container_1.SoftwareVersionCPU;
            row_34_column_1.Content = container_1.SoftwareVersionEP;

            row_33_column_2.Content = container_2.SoftwareVersionCPU;
            row_34_column_2.Content = container_2.SoftwareVersionEP;
            row_33_column_3.Content = container_3.SoftwareVersionCPU;
            row_34_column_3.Content = container_3.SoftwareVersionEP;
            row_33_column_4.Content = container_4.SoftwareVersionCPU;
            row_34_column_4.Content = container_4.SoftwareVersionEP;
            row_33_column_5.Content = container_5.SoftwareVersionCPU;
            row_34_column_5.Content = container_5.SoftwareVersionEP;
            row_33_column_6.Content = container_6.SoftwareVersionCPU;
            row_34_column_6.Content = container_6.SoftwareVersionEP;

            //row_34_column_4.Content = container_4.ParkPressure;

            // TC、MP、ME轮径
            row_36_column_1.Content = container_1.Tc1 + " / " + container_6.Tc1;
            row_36_column_3.Content = container_1.Mp1 + " / " + container_6.Mp1;
            row_36_column_5.Content = container_1.M1 + " / " + container_6.M1;
        }

        private void UpdateDI()
        {
            //制动命令
            row_1_column_1_DI.Fill = GetBrush(container_1.BrakeCmd, 1);
            row_1_column_6_DI.Fill = GetBrush(container_6.BrakeCmd, 6);

            //牵引命令
            row_2_column_1_DI.Fill = GetBrush(container_1.DriveCmd, 1);
            row_2_column_6_DI.Fill = GetBrush(container_6.DriveCmd, 6);

            //惰行命令
            row_3_column_1_DI.Fill = GetBrush(container_1.LazyCmd, 1);
            row_3_column_6_DI.Fill = GetBrush(container_6.LazyCmd, 6);

            //快速制动命令
            row_4_column_1_DI.Fill = GetBrush(container_1.FastBrakeCmd, 1);
            row_4_column_6_DI.Fill = GetBrush(container_6.FastBrakeCmd, 6);

            //紧急制动命令
            row_5_column_1_DI.Fill = GetBrush(container_1.EmergencyBrakeCmd, 1);
            row_5_column_6_DI.Fill = GetBrush(container_6.EmergencyBrakeCmd, 6);

            //紧急制动激活
            row_6_column_1_DI.Fill = GetBrush(container_1.EmergencyBrakeActiveA1);
            row_6_column_2_DI.Fill = GetBrush(container_2.EmergencyBrakeActive);
            row_6_column_3_DI.Fill = GetBrush(container_3.EmergencyBrakeActive);
            row_6_column_4_DI.Fill = GetBrush(container_4.EmergencyBrakeActive);
            row_6_column_5_DI.Fill = GetBrush(container_5.EmergencyBrakeActive);
            row_6_column_6_DI.Fill = GetBrush(container_6.EmergencyBrakeActiveA1);

            //空气制动施加
            row_7_column_1_DI.Fill = GetBrush(container_1.AbActive);
            row_7_column_2_DI.Fill = GetBrush(container_2.AbBrakeActive);
            row_7_column_3_DI.Fill = GetBrush(container_3.AbBrakeActive);
            row_7_column_4_DI.Fill = GetBrush(container_4.AbBrakeActive);
            row_7_column_5_DI.Fill = GetBrush(container_5.AbBrakeActive);
            row_7_column_6_DI.Fill = GetBrush(container_6.AbActive);

            //停放制动缓解
            row_8_column_1_DI.Fill = GetBrush(container_1.ParkBreakRealease);
            row_8_column_3_DI.Fill = GetBrush(container_3.ParkBrakeRealease);
            row_8_column_5_DI.Fill = GetBrush(container_5.ParkBrakeRealease);

            //硬线牵引
            row_9_column_1_DI.Fill = GetBrush(container_1.HardDriveCmd, 1);
            row_9_column_6_DI.Fill = GetBrush(container_6.HardDriveCmd, 6);

            //硬线制动
            row_10_column_1_DI.Fill = GetBrush(container_1.HardBrakeCmd, 1);
            row_10_column_6_DI.Fill = GetBrush(container_6.HardBrakeCmd, 6);

            //第二列
            //硬线快速制动
            row_1_column_8_DI.Fill = GetBrush(container_1.HardFastBrakeCmd, 1);
            row_1_column_13_DI.Fill = GetBrush(container_6.HardFastBrakeCmd, 6);

            //硬线紧急制动
            row_2_column_8_DI.Fill = GetBrush(container_1.HardEmergencyBrake, 1);
            row_2_column_13_DI.Fill = GetBrush(container_6.HardEmergencyBrake, 6);

            //硬线紧急牵引
            row_3_column_8_DI.Fill = GetBrush(container_1.HardEmergencyDriveCmd, 1);
            row_3_column_13_DI.Fill = GetBrush(container_6.HardEmergencyDriveCmd, 6);

            //网络牵引
            row_4_column_8_DI.Fill = GetBrush(container_1.NetDriveCmd, 1);
            row_4_column_13_DI.Fill = GetBrush(container_6.NetDriveCmd, 6);


            //网络制动
            row_5_column_8_DI.Fill = GetBrush(container_1.NetBrakeCmd, 1);
            row_5_column_13_DI.Fill = GetBrush(container_6.NetBrakeCmd, 6);

            //保持制动缓解
            row_6_column_8_DI.Fill = GetBrush(container_1.HoldBrakeRealease, 1);
            row_6_column_13_DI.Fill = GetBrush(container_6.HoldBrakeRealease, 6);

            //CAN单元
            row_7_column_8_DI.Fill = GetBrush(container_1.CanUintSelfTestCmd_A, 1);
            row_8_column_8_DI.Fill = GetBrush(container_1.CanUintSelfTestCmd_B, 1);

            row_7_column_13_DI.Fill = GetBrush(container_6.CanUintSelfTestCmd_A, 6);
            row_8_column_13_DI.Fill = GetBrush(container_6.CanUintSelfTestCmd_B, 6);

            //自检
            row_9_column_8_DI.Fill = GetBrush(container_1.SelfTestCmd, 1);
            row_9_column_13_DI.Fill = GetBrush(container_6.SelfTestCmd, 6);

            //紧急制动命令
            row_10_column_8_DI.Fill = GetBrush(container_1.EmergencyBrakeCmd, 1);
            row_10_column_13_DI.Fill = GetBrush(container_6.EmergencyBrakeCmd, 6);

            //网络快速制动
            row_11_column_8_DI.Fill = GetBrush(container_1.NetFastBrakeCmd, 1);
            row_11_column_13_DI.Fill = GetBrush(container_6.NetFastBrakeCmd, 6);
        }

        private void UpdateDO()
        {
            row_1_column_1_DO.Fill = GetBrush(container_1.Mode.Equals(MainDevDataContains.NORMAL_MODE), 1);
            row_1_column_6_DO.Fill = GetBrush(container_6.Mode.Equals(MainDevDataContains.NORMAL_MODE), 6);
            row_2_column_1_DO.Fill = GetBrush(container_1.Mode.Equals(MainDevDataContains.EMERGENCY_DRIVE_MODE), 1);
            row_2_column_6_DO.Fill = GetBrush(container_6.Mode.Equals(MainDevDataContains.EMERGENCY_DRIVE_MODE), 6);
            row_3_column_1_DO.Fill = GetBrush(container_1.Mode.Equals(MainDevDataContains.CALLBACK_MODE), 1);
            row_3_column_6_DO.Fill = GetBrush(container_6.Mode.Equals(MainDevDataContains.CALLBACK_MODE), 6);

            row_4_column_1_DO.Fill = GetBrush(container_1.LazyCmd, 1);
            row_4_column_6_DO.Fill = GetBrush(container_6.LazyCmd, 6);

            row_5_column_1_DO.Fill = GetBrush(container_1.KeepBrakeState, 1);
            row_5_column_6_DO.Fill = GetBrush(container_6.KeepBrakeState, 6);

            row_6_column_1_DO.Fill = GetBrush(container_1.LazyState, 1);
            row_6_column_6_DO.Fill = GetBrush(container_6.LazyState, 6);

            row_7_column_1_DO.Fill = GetBrush(container_1.DriveState, 1);
            row_7_column_6_DO.Fill = GetBrush(container_6.DriveState, 6);

            row_8_column_1_DO.Fill = GetBrush(container_1.NormalBrakeState, 1);
            row_8_column_6_DO.Fill = GetBrush(container_6.NormalBrakeState, 6);

            row_9_column_1_DO.Fill = GetBrush(container_1.EmergencyBrakeState, 1);
            row_9_column_6_DO.Fill = GetBrush(container_6.EmergencyBrakeState, 6);

            row_10_column_1_DO.Fill = GetBrush(container_1.ZeroSpeed, 1);
            row_10_column_6_DO.Fill = GetBrush(container_6.ZeroSpeed, 6);

            row_11_column_1_DO.Fill = GetBrush(container_1.SelfTestFail, 1);
            row_11_column_6_DO.Fill = GetBrush(container_6.SelfTestFail, 6);

            row_12_column_1_DO.Fill = GetBrush(container_1.UnSelfTest24, 1);
            row_12_column_6_DO.Fill = GetBrush(container_6.UnSelfTest24, 6);

            row_13_column_1_DO.Fill = GetBrush(container_1.UnSelfTest26, 1);
            row_13_column_6_DO.Fill = GetBrush(container_6.UnSelfTest26, 6);

            row_14_column_1_DO.Fill = GetBrush(container_1.GateValveState);
            row_14_column_6_DO.Fill = GetBrush(container_6.GateValveState);

            row_15_column_1_DO.Fill = GetBrush(container_1.CanUnitSelfTestOn, 1);
            row_15_column_6_DO.Fill = GetBrush(container_6.CanUnitSelfTestOn, 6);

            row_16_column_1_DO.Fill = GetBrush(container_1.ValveCanEmergencyActive, 1);
            row_16_column_6_DO.Fill = GetBrush(container_6.ValveCanEmergencyActive, 6);

            row_17_column_1_DO.Fill = GetBrush(container_1.CanUintSelfTestOver, 1);
            row_17_column_6_DO.Fill = GetBrush(container_6.CanUintSelfTestOver, 6);

            row_18_column_1_DO.Fill = GetBrush(container_1.TowingMode, 1);
            row_18_column_6_DO.Fill = GetBrush(container_6.TowingMode, 6);

            row_19_column_1_DO.Fill = GetBrush(container_1.ATOMode1, 1);
            row_19_column_6_DO.Fill = GetBrush(container_6.ATOMode1, 6);

            row_20_column_1_DO.Fill = GetBrush(container_1.BrakeLevelEnable, 1);
            row_20_column_6_DO.Fill = GetBrush(container_6.BrakeLevelEnable, 6);

            row_21_column_1_DO.Fill = GetBrush(container_1.DcuEbOK[0], 1);
            row_21_column_6_DO.Fill = GetBrush(container_6.DcuEbOK[0], 6);

            row_22_column_1_DO.Fill = GetBrush(container_1.DcuEbFadeout[0], 1);
            row_22_column_6_DO.Fill = GetBrush(container_6.DcuEbFadeout[0], 6);

            row_23_column_1_DO.Fill = GetBrush(container_1.DcuEbSlip[0], 1);
            row_23_column_6_DO.Fill = GetBrush(container_6.DcuEbSlip[0], 6);

            row_24_column_1_DO.Fill = GetBrush(container_1.DcuEbOK[1], 1);
            row_24_column_6_DO.Fill = GetBrush(container_6.DcuEbOK[1], 6);

            row_25_column_1_DO.Fill = GetBrush(container_1.DcuEbFadeout[1], 1);
            row_25_column_6_DO.Fill = GetBrush(container_6.DcuEbFadeout[1], 6);

            row_26_column_1_DO.Fill = GetBrush(container_1.DcuEbSlip[1], 1);
            row_26_column_6_DO.Fill = GetBrush(container_6.DcuEbSlip[1], 6);

            row_27_column_1_DO.Fill = GetBrush(container_1.DcuEbOK[2], 1);
            row_27_column_6_DO.Fill = GetBrush(container_6.DcuEbOK[2], 6);

            row_28_column_1_DO.Fill = GetBrush(container_1.DcuEbFadeout[2], 1);
            row_28_column_6_DO.Fill = GetBrush(container_6.DcuEbFadeout[2], 6);

            row_29_column_1_DO.Fill = GetBrush(container_1.DcuEbSlip[2], 1);
            row_29_column_6_DO.Fill = GetBrush(container_6.DcuEbSlip[2], 6);

            //第二列
            row_1_column_8_DO.Fill = GetBrush(container_1.VCM_MVBConnectionState);
            row_1_column_9_DO.Fill = GetBrush(container_2.VCM_MVBConnectionState);
            row_1_column_10_DO.Fill = GetBrush(container_3.VCM_MVBConnectionState);
            row_1_column_11_DO.Fill = GetBrush(container_4.VCM_MVBConnectionState);
            row_1_column_12_DO.Fill = GetBrush(container_5.VCM_MVBConnectionState);
            row_1_column_13_DO.Fill = GetBrush(container_6.VCM_MVBConnectionState);

            row_2_column_8_DO.Fill = GetBrush(container_1.SlipA1);
            row_2_column_9_DO.Fill = GetBrush(container_2.Slip);
            row_2_column_10_DO.Fill = GetBrush(container_3.Slip);
            row_2_column_11_DO.Fill = GetBrush(container_4.Slip);
            row_2_column_12_DO.Fill = GetBrush(container_5.Slip);
            row_2_column_13_DO.Fill = GetBrush(container_6.SlipA1);

            row_3_column_8_DO.Fill = GetBrush(container_1.NotZeroSpeed, 1);
            row_3_column_13_DO.Fill = GetBrush(container_6.NotZeroSpeed, 6);

            row_4_column_8_DO.Fill = GetBrush(container_1.BSRLowA11);
            row_4_column_9_DO.Fill = GetBrush(container_2.BSRLow1);
            row_4_column_10_DO.Fill = GetBrush(container_3.BSRLow1);
            row_4_column_11_DO.Fill = GetBrush(container_4.BSRLow1);
            row_4_column_12_DO.Fill = GetBrush(container_5.BSRLow1);
            row_4_column_13_DO.Fill = GetBrush(container_6.BSRLowA11);

            row_5_column_8_DO.Fill = GetBrush(container_1.AbStatuesA1);
            row_5_column_9_DO.Fill = GetBrush(container_2.AbBrakeSatet);
            row_5_column_10_DO.Fill = GetBrush(container_3.AbBrakeSatet);
            row_5_column_11_DO.Fill = GetBrush(container_4.AbBrakeSatet);
            row_5_column_12_DO.Fill = GetBrush(container_5.AbBrakeSatet);
            row_5_column_13_DO.Fill = GetBrush(container_6.AbStatuesA1);
            
            row_6_column_8_DO.Fill = GetBrush(container_1.AirSigValid);
            row_6_column_9_DO.Fill = GetBrush(container_2.AirSigValid);
            row_6_column_10_DO.Fill = GetBrush(container_3.AirSigValid);
            row_6_column_11_DO.Fill = GetBrush(container_4.AirSigValid);
            row_6_column_12_DO.Fill = GetBrush(container_5.AirSigValid);
            row_6_column_13_DO.Fill = GetBrush(container_6.AirSigValid);

            row_7_column_8_DO.Fill = GetBrush(container_1.SelfTestInt, 1);
            row_7_column_13_DO.Fill = GetBrush(container_6.SelfTestInt, 6);

            row_8_column_8_DO.Fill = GetBrush(container_1.SelfTestActive, 1);
            row_8_column_13_DO.Fill = GetBrush(container_6.SelfTestActive, 6);

            row_9_column_8_DO.Fill = GetBrush(container_1.SelfTestSuccess, 1);
            row_9_column_13_DO.Fill = GetBrush(container_6.SelfTestSuccess, 6);

            row_10_column_8_DO.Fill = GetBrush(container_1.EdFadeOut, 1);
            row_10_column_13_DO.Fill = GetBrush(container_6.EdFadeOut, 6);

            row_11_column_8_DO.Fill = GetBrush(container_1.TrainBrakeEnable, 1);
            row_11_column_13_DO.Fill = GetBrush(container_6.TrainBrakeEnable, 6);

            row_12_column_8_DO.Fill = GetBrush(container_1.ZeroSpeed, 1);
            row_12_column_13_DO.Fill = GetBrush(container_6.ZeroSpeed, 6);

            row_13_column_8_DO.Fill = GetBrush(container_1.EdOffB1, 1);
            row_14_column_8_DO.Fill = GetBrush(container_1.EdOffC1, 1);
            row_13_column_13_DO.Fill = GetBrush(container_6.EdOffB1, 6);
            row_14_column_13_DO.Fill = GetBrush(container_6.EdOffC1, 6);

            row_15_column_8_DO.Fill = GetBrush(container_1.WheelInputState, 1);
            row_15_column_13_DO.Fill = GetBrush(container_6.WheelInputState, 6);

            row_16_column_8_DO.Fill = GetBrush(container_1.Ax1SelfTestActive);
            row_16_column_9_DO.Fill = GetBrush(container_2.SelfTestActive);
            row_16_column_10_DO.Fill = GetBrush(container_3.SelfTestActive);
            row_16_column_11_DO.Fill = GetBrush(container_4.SelfTestActive);
            row_16_column_12_DO.Fill = GetBrush(container_5.SelfTestActive);
            row_16_column_13_DO.Fill = GetBrush(container_6.Ax1SelfTestActive);

            row_17_column_8_DO.Fill = GetBrush(container_1.Ax1SelfTestOver);
            row_17_column_9_DO.Fill = GetBrush(container_2.SelfTestOver);
            row_17_column_10_DO.Fill = GetBrush(container_3.SelfTestOver);
            row_17_column_11_DO.Fill = GetBrush(container_4.SelfTestOver);
            row_17_column_12_DO.Fill = GetBrush(container_5.SelfTestOver);
            row_17_column_13_DO.Fill = GetBrush(container_6.Ax1SelfTestOver);
            
            row_18_column_8_DO.Fill = GetBrush(container_1.Ax1SelfTestSuccess);
            row_18_column_9_DO.Fill = GetBrush(container_2.SelfTestSuccess);
            row_18_column_10_DO.Fill = GetBrush(container_3.SelfTestSuccess);
            row_18_column_11_DO.Fill = GetBrush(container_4.SelfTestSuccess);
            row_18_column_12_DO.Fill = GetBrush(container_5.SelfTestSuccess);
            row_18_column_13_DO.Fill = GetBrush(container_6.Ax1SelfTestSuccess);
            
            row_19_column_8_DO.Fill = GetBrush(container_1.Ax1SelfTestFail);
            row_19_column_9_DO.Fill = GetBrush(container_2.SelfTestFail);
            row_19_column_10_DO.Fill = GetBrush(container_3.SelfTestFail);
            row_19_column_11_DO.Fill = GetBrush(container_4.SelfTestFail);
            row_19_column_12_DO.Fill = GetBrush(container_5.SelfTestFail);
            row_19_column_13_DO.Fill = GetBrush(container_6.Ax1SelfTestFail);
            
            row_20_column_8_DO.Fill = GetBrush(container_1.DcuEbOK[3], 1);
            row_20_column_13_DO.Fill = GetBrush(container_6.DcuEbOK[3], 6);

            row_21_column_8_DO.Fill = GetBrush(container_1.DcuEbFadeout[3], 1);
            row_21_column_13_DO.Fill = GetBrush(container_6.DcuEbFadeout[3], 6);

            row_22_column_8_DO.Fill = GetBrush(container_1.DcuEbSlip[3], 1);
            row_22_column_13_DO.Fill = GetBrush(container_6.DcuEbSlip[3], 6);
        }

        private void UpdateFA()
        {
            row_1_column_1_FA.Fill = GetBrush(container_1.BSSRSenorFault);
            row_1_column_2_FA.Fill = GetBrush(container_2.BSSRSenorFault);
            row_1_column_3_FA.Fill = GetBrush(container_3.BSSRSenorFault);
            row_1_column_4_FA.Fill = GetBrush(container_4.BSSRSenorFault);
            row_1_column_5_FA.Fill = GetBrush(container_5.BSSRSenorFault);
            row_1_column_6_FA.Fill = GetBrush(container_6.BSSRSenorFault);

            row_2_column_1_FA.Fill = GetBrush(container_1.AirSpringSenorFault_1);
            row_2_column_2_FA.Fill = GetBrush(container_2.AirSpringSenorFault_1);
            row_2_column_3_FA.Fill = GetBrush(container_3.AirSpringSenorFault_1);
            row_2_column_4_FA.Fill = GetBrush(container_4.AirSpringSenorFault_1);
            row_2_column_5_FA.Fill = GetBrush(container_5.AirSpringSenorFault_1);
            row_2_column_6_FA.Fill = GetBrush(container_6.AirSpringSenorFault_1);

            row_3_column_1_FA.Fill = GetBrush(container_1.AirSpringSenorFault_2);
            row_3_column_2_FA.Fill = GetBrush(container_2.AirSpringSenorFault_2);
            row_3_column_3_FA.Fill = GetBrush(container_3.AirSpringSenorFault_2);
            row_3_column_4_FA.Fill = GetBrush(container_4.AirSpringSenorFault_2);
            row_3_column_5_FA.Fill = GetBrush(container_5.AirSpringSenorFault_2);
            row_3_column_6_FA.Fill = GetBrush(container_6.AirSpringSenorFault_2);

            row_4_column_1_FA.Fill = GetBrush(container_1.ParkCylinderSenorFault);
            row_4_column_2_FA.Fill = GetBrush(container_2.ParkCylinderSenorFault);
            row_4_column_3_FA.Fill = GetBrush(container_3.ParkCylinderSenorFault);
            row_4_column_4_FA.Fill = GetBrush(container_4.MainPipeSensorFault);
            row_4_column_5_FA.Fill = GetBrush(container_5.ParkCylinderSenorFault);
            row_4_column_6_FA.Fill = GetBrush(container_6.ParkCylinderSenorFault);

            row_5_column_1_FA.Fill = GetBrush(container_1.VLDSensorFault);
            row_5_column_2_FA.Fill = GetBrush(container_2.VLDSensorFault);
            row_5_column_3_FA.Fill = GetBrush(container_3.VLDSensorFault);
            row_5_column_4_FA.Fill = GetBrush(container_4.VLDSensorFault);
            row_5_column_5_FA.Fill = GetBrush(container_5.VLDSensorFault);
            row_5_column_6_FA.Fill = GetBrush(container_6.VLDSensorFault);

            row_6_column_1_FA.Fill = GetBrush(container_1.BSRSenorFault_1);
            row_6_column_2_FA.Fill = GetBrush(container_2.BSRSenorFault_1);
            row_6_column_3_FA.Fill = GetBrush(container_3.BSRSenorFault_1);
            row_6_column_4_FA.Fill = GetBrush(container_4.BSRSenorFault_1);
            row_6_column_5_FA.Fill = GetBrush(container_5.BSRSenorFault_1);
            row_6_column_6_FA.Fill = GetBrush(container_6.BSRSenorFault_1);

            row_7_column_1_FA.Fill = GetBrush(container_1.BSRSenorFault_2);
            row_7_column_2_FA.Fill = GetBrush(container_2.BSRSenorFault_2);
            row_7_column_3_FA.Fill = GetBrush(container_3.BSRSenorFault_2);
            row_7_column_4_FA.Fill = GetBrush(container_4.BSRSenorFault_2);
            row_7_column_5_FA.Fill = GetBrush(container_5.BSRSenorFault_2);
            row_7_column_6_FA.Fill = GetBrush(container_6.BSRSenorFault_2);

            row_8_column_1_FA.Fill = GetBrush(container_1.AirSpringOverflow_1);
            row_8_column_2_FA.Fill = GetBrush(container_2.AirSpringOverflow_1);
            row_8_column_3_FA.Fill = GetBrush(container_3.AirSpringOverflow_1);
            row_8_column_4_FA.Fill = GetBrush(container_4.AirSpringOverflow_1);
            row_8_column_5_FA.Fill = GetBrush(container_5.AirSpringOverflow_1);
            row_8_column_6_FA.Fill = GetBrush(container_6.AirSpringOverflow_1);

            row_9_column_1_FA.Fill = GetBrush(container_1.AirSpringOverflow_2);
            row_9_column_2_FA.Fill = GetBrush(container_2.AirSpringOverflow_2);
            row_9_column_3_FA.Fill = GetBrush(container_3.AirSpringOverflow_2);
            row_9_column_4_FA.Fill = GetBrush(container_4.AirSpringOverflow_2);
            row_9_column_5_FA.Fill = GetBrush(container_5.AirSpringOverflow_2);
            row_9_column_6_FA.Fill = GetBrush(container_6.AirSpringOverflow_2);

            row_10_column_1_FA.Fill = GetBrush(container_1.BSRLowA11);
            row_10_column_2_FA.Fill = GetBrush(container_2.BSRLow1);
            row_10_column_3_FA.Fill = GetBrush(container_3.BSRLow1);
            row_10_column_4_FA.Fill = GetBrush(container_4.BSRLow1);
            row_10_column_5_FA.Fill = GetBrush(container_5.BSRLow1);
            row_10_column_6_FA.Fill = GetBrush(container_6.BSRLowA11);

            row_11_column_1_FA.Fill = GetBrush(container_1.BCUFail_Serious);
            row_11_column_2_FA.Fill = GetBrush(container_2.BCUFail_Serious);
            row_11_column_3_FA.Fill = GetBrush(container_3.BCUFail_Serious);
            row_11_column_4_FA.Fill = GetBrush(container_4.BCUFail_Serious);
            row_11_column_5_FA.Fill = GetBrush(container_5.BCUFail_Serious);
            row_11_column_6_FA.Fill = GetBrush(container_6.BCUFail_Serious);

            row_12_column_1_FA.Fill = GetBrush(container_1.BCUFail_Middle);
            row_12_column_2_FA.Fill = GetBrush(container_2.BCUFail_Middle);
            row_12_column_3_FA.Fill = GetBrush(container_3.BCUFail_Middle);
            row_12_column_4_FA.Fill = GetBrush(container_4.BCUFail_Middle);
            row_12_column_5_FA.Fill = GetBrush(container_5.BCUFail_Middle);
            row_12_column_6_FA.Fill = GetBrush(container_6.BCUFail_Middle);

            row_13_column_1_FA.Fill = GetBrush(container_1.BCUFail_Slight);
            row_13_column_2_FA.Fill = GetBrush(container_2.BCUFail_Slight);
            row_13_column_3_FA.Fill = GetBrush(container_3.BCUFail_Slight);
            row_13_column_4_FA.Fill = GetBrush(container_4.BCUFail_Slight);
            row_13_column_5_FA.Fill = GetBrush(container_5.BCUFail_Slight);
            row_13_column_6_FA.Fill = GetBrush(container_6.BCUFail_Slight);

            row_14_column_1_FA.Fill = GetBrush(container_1.EmergencyBrakeFault);
            row_14_column_2_FA.Fill = GetBrush(container_2.EmergencyBrakeFault);
            row_14_column_3_FA.Fill = GetBrush(container_3.EmergencyBrakeFault);
            row_14_column_4_FA.Fill = GetBrush(container_4.EmergencyBrakeFault);
            row_14_column_5_FA.Fill = GetBrush(container_5.EmergencyBrakeFault);
            row_14_column_6_FA.Fill = GetBrush(container_6.EmergencyBrakeFault);

            row_15_column_1_FA.Fill = GetBrush(container_1.CanASPEnable, 1);
            row_15_column_6_FA.Fill = GetBrush(container_6.CanASPEnable, 6);

            row_16_column_1_FA.Fill = GetBrush(container_1.BCPLowA, 1);
            row_16_column_6_FA.Fill = GetBrush(container_6.BCPLowA, 6);

            row_17_column_1_FA.Fill = GetBrush(container_1.BCPLowB, 1);
            row_17_column_6_FA.Fill = GetBrush(container_6.BCPLowB, 6);

            row_18_column_1_FA.Fill = GetBrush(container_1.BCPLowC, 1);
            row_18_column_6_FA.Fill = GetBrush(container_6.BCPLowC, 6);

            //第二列
            row_1_column_8_FA.Fill = GetBrush(container_1.SpeedSenorFault_1);
            row_1_column_9_FA.Fill = GetBrush(container_2.SpeedSenorFault_1);
            row_1_column_10_FA.Fill = GetBrush(container_3.SpeedSenorFault_1);
            row_1_column_11_FA.Fill = GetBrush(container_4.SpeedSenorFault_1);
            row_1_column_12_FA.Fill = GetBrush(container_5.SpeedSenorFault_1);
            row_1_column_13_FA.Fill = GetBrush(container_6.SpeedSenorFault_1);

            row_2_column_8_FA.Fill = GetBrush(container_1.SpeedSenorFault_2);
            row_2_column_9_FA.Fill = GetBrush(container_2.SpeedSenorFault_2);
            row_2_column_10_FA.Fill = GetBrush(container_3.SpeedSenorFault_2);
            row_2_column_11_FA.Fill = GetBrush(container_4.SpeedSenorFault_2);
            row_2_column_12_FA.Fill = GetBrush(container_5.SpeedSenorFault_2);
            row_2_column_13_FA.Fill = GetBrush(container_6.SpeedSenorFault_2);

            row_3_column_8_FA.Fill = GetBrush(container_1.WSPFault_1);
            row_3_column_9_FA.Fill = GetBrush(container_2.WSPFault_1);
            row_3_column_10_FA.Fill = GetBrush(container_3.WSPFault_1);
            row_3_column_11_FA.Fill = GetBrush(container_4.WSPFault_1);
            row_3_column_12_FA.Fill = GetBrush(container_5.WSPFault_1);
            row_3_column_13_FA.Fill = GetBrush(container_6.WSPFault_1);

            row_4_column_8_FA.Fill = GetBrush(container_1.WSPFault_2);
            row_4_column_9_FA.Fill = GetBrush(container_2.WSPFault_2);
            row_4_column_10_FA.Fill = GetBrush(container_3.WSPFault_2);
            row_4_column_11_FA.Fill = GetBrush(container_4.WSPFault_2);
            row_4_column_12_FA.Fill = GetBrush(container_5.WSPFault_2);
            row_4_column_13_FA.Fill = GetBrush(container_6.WSPFault_2);

            row_5_column_8_FA.Fill = GetBrush(container_1.CodeConnectorFault);
            row_5_column_9_FA.Fill = GetBrush(container_2.CodeConnectorFault);
            row_5_column_10_FA.Fill = GetBrush(container_3.CodeConnectorFault);
            row_5_column_11_FA.Fill = GetBrush(container_4.CodeConnectorFault);
            row_5_column_12_FA.Fill = GetBrush(container_5.CodeConnectorFault);
            row_5_column_13_FA.Fill = GetBrush(container_6.CodeConnectorFault);

            row_6_column_8_FA.Fill = GetBrush(container_1.AirSpringLimit);
            row_6_column_9_FA.Fill = GetBrush(container_2.AirSpringLimit);
            row_6_column_10_FA.Fill = GetBrush(container_3.AirSpringLimit);
            row_6_column_11_FA.Fill = GetBrush(container_4.AirSpringLimit);
            row_6_column_12_FA.Fill = GetBrush(container_5.AirSpringLimit);
            row_6_column_13_FA.Fill = GetBrush(container_6.AirSpringLimit);

            row_7_column_8_FA.Fill = GetBrush(container_1.BrakeNotRealease);
            row_7_column_9_FA.Fill = GetBrush(container_2.BrakeNotRealease);
            row_7_column_10_FA.Fill = GetBrush(container_3.BrakeNotRealease);
            row_7_column_11_FA.Fill = GetBrush(container_4.BrakeNotRealease);
            row_7_column_12_FA.Fill = GetBrush(container_5.BrakeNotRealease);
            row_7_column_13_FA.Fill = GetBrush(container_6.BrakeNotRealease);

            row_8_column_8_FA.Fill = GetBrush(container_1.BCPLowA11);
            row_8_column_9_FA.Fill = GetBrush(container_2.BCPLow1);
            row_8_column_10_FA.Fill = GetBrush(container_3.BCPLow1);
            row_8_column_11_FA.Fill = GetBrush(container_4.BCPLow1);
            row_8_column_12_FA.Fill = GetBrush(container_5.BCPLow1);
            row_8_column_13_FA.Fill = GetBrush(container_6.BCPLowA11);

            row_9_column_8_FA.Fill = GetBrush(container_1.SpeedDetection);
            row_10_column_8_FA.Fill = GetBrush(container_1.CanBusFail1);
            row_10_column_13_FA.Fill = GetBrush(container_6.CanBusFail1);

            row_10_column_9_FA.Fill = GetBrush(container_2.OCANFault1);
            row_10_column_10_FA.Fill = GetBrush(container_3.OCANFault1);
            row_10_column_11_FA.Fill = GetBrush(container_4.OCANFault1);
            row_10_column_12_FA.Fill = GetBrush(container_5.OCANFault1);

            row_11_column_8_FA.Fill = GetBrush(container_1.CanBusFail2);
            row_11_column_9_FA.Fill = GetBrush(container_2.OCANFault2);
            row_11_column_10_FA.Fill = GetBrush(container_3.OCANFault2);
            row_11_column_11_FA.Fill = GetBrush(container_4.OCANFault2);
            row_11_column_12_FA.Fill = GetBrush(container_5.OCANFault2);
            row_11_column_13_FA.Fill = GetBrush(container_6.CanBusFail2);
            row_12_column_8_FA.Fill = GetBrush(container_1.HardDifferent);
            row_12_column_13_FA.Fill = GetBrush(container_6.HardDifferent);
            row_13_column_8_FA.Fill = GetBrush(container_1.EventHigh);
            row_13_column_13_FA.Fill = GetBrush(container_6.EventHigh);
            row_14_column_8_FA.Fill = GetBrush(container_1.EventMid);
            row_14_column_13_FA.Fill = GetBrush(container_6.EventMid);
            row_15_column_8_FA.Fill = GetBrush(container_1.EventLow);
            row_15_column_13_FA.Fill = GetBrush(container_6.EventLow);

            row_1_column_1_sli.Content = container_1.SlipLvl1;
            row_1_column_2_sli.Content = container_2.SlipLvl1;
            row_1_column_3_sli.Content = container_3.SlipLvl1;
            row_1_column_4_sli.Content = container_4.SlipLvl1;
            row_1_column_5_sli.Content = container_5.SlipLvl1;
            row_1_column_6_sli.Content = container_6.SlipLvl1;

            //row_2_column_1_sli.Content = container_1.

            // ICAN1
            row_16_column_8_FA.Fill = GetBrush(container_1.ICANFault1);
            row_16_column_9_FA.Fill = GetBrush(container_2.ICANFault1);
            row_16_column_10_FA.Fill = GetBrush(container_3.ICANFault1);
            row_16_column_11_FA.Fill = GetBrush(container_4.ICANFault1);
            row_16_column_12_FA.Fill = GetBrush(container_5.ICANFault1);
            row_16_column_13_FA.Fill = GetBrush(container_6.ICANFault1);
            // ICAN2
            row_17_column_8_FA.Fill = GetBrush(container_1.ICANFault2);
            row_17_column_9_FA.Fill = GetBrush(container_2.ICANFault2);
            row_17_column_10_FA.Fill = GetBrush(container_3.ICANFault2);
            row_17_column_11_FA.Fill = GetBrush(container_4.ICANFault2);
            row_17_column_12_FA.Fill = GetBrush(container_5.ICANFault2);
            row_17_column_13_FA.Fill = GetBrush(container_6.ICANFault2);
            // OCAN1
            row_18_column_8_FA.Fill = GetBrush(container_1.OCANFault1);
            row_18_column_9_FA.Fill = GetBrush(container_2.OCANFault1);
            row_18_column_10_FA.Fill = GetBrush(container_3.OCANFault1);
            row_18_column_11_FA.Fill = GetBrush(container_4.OCANFault1);
            row_18_column_12_FA.Fill = GetBrush(container_5.OCANFault1);
            row_18_column_13_FA.Fill = GetBrush(container_6.OCANFault1);
            // OCAN2
            row_19_column_8_FA.Fill = GetBrush(container_1.OCANFault2);
            row_19_column_9_FA.Fill = GetBrush(container_2.OCANFault2);
            row_19_column_10_FA.Fill = GetBrush(container_3.OCANFault2);
            row_19_column_11_FA.Fill = GetBrush(container_4.OCANFault2);
            row_19_column_12_FA.Fill = GetBrush(container_5.OCANFault2);
            row_19_column_13_FA.Fill = GetBrush(container_6.OCANFault2);

        }

        private SolidColorBrush GetBrush(bool b)
        {
            return b == true ? new SolidColorBrush(Colors.LimeGreen) : new SolidColorBrush(Colors.LightGray);
        }

        private int Check(int value, int ax)
        {
            if (ax == 1)
            {
                if (!container_6.GateValveState)
                {
                    return value;
                }
                else
                {
                    return 0;
                }
            }
            if (ax == 6)
            {
                if (!container_1.GateValveState)
                {
                    return value;
                }
                else
                {
                    return 0;
                }
            }
            return 0;
        }

        private int Check_Int(int value, int ax)
        {
            if (container_1.GateValveState == true && container_6.GateValveState == true)
            {
                return value;
            }
            if (ax == 1)
            {
                if (!container_6.GateValveState)
                {
                    return value;
                }
                else
                {
                    return 0;
                }
            }
            if (ax == 6)
            {
                if (!container_1.GateValveState)
                {
                    return value;
                }
                else
                {
                    return 0;
                }
            }
            return 0;
        }


        private double Check_Double(double value, int ax)
        {
            if (container_1.GateValveState == true && container_6.GateValveState == true)
            {
                return value;
            }
            if (ax == 1)
            {
                if (!container_6.GateValveState)
                {
                    return value;
                }
                else
                {
                    return 0;
                }
            }
            if (ax == 6)
            {
                if (!container_1.GateValveState)
                {
                    return value;
                }
                else
                {
                    return 0;
                }
            }
            return 0;
        }



        private SolidColorBrush GetBrush(bool b, int ax)
        {
            if (ax == 1)
            {
                if (!container_6.GateValveState)
                {
                    return b == true ? new SolidColorBrush(Colors.LimeGreen) : new SolidColorBrush(Colors.LightGray);
                }
                else
                {
                    return new SolidColorBrush(Colors.LightGray);
                    
                }
            }
            else if (ax == 6)
            {
                if (!container_1.GateValveState)
                {
                    return b == true ? new SolidColorBrush(Colors.LimeGreen) : new SolidColorBrush(Colors.LightGray);
                }
                else
                {
                    return new SolidColorBrush(Colors.LightGray);
                }
            }
            return null;
        }

        
        public void UpdateData(MainDevDataContains container_1, SliverDataContainer container_2, SliverDataContainer container_3, SliverDataContainer container_4, SliverDataContainer container_5, MainDevDataContains container_6)
        {
            this.container_1 = container_1;
            this.container_2 = container_2;
            this.container_3 = container_3;
            this.container_4 = container_4;
            this.container_5 = container_5;
            this.container_6 = container_6;
        }



        #endregion



        #region window method
        private void Storyboard_Completed(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ChartWindow_Loaded(object sender, RoutedEventArgs e)
        {
            double x = SystemParameters.WorkArea.Width;
            double y = SystemParameters.WorkArea.Height;
            double x1 = SystemParameters.PrimaryScreenWidth;//得到屏幕整体宽度
            double y1 = SystemParameters.PrimaryScreenHeight;//得到屏幕整体高度
            this.Width = x1 * 2 / 3;
            this.Height = y1 * 4 / 5;
        }

        private void miniumBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void maximunBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OverviewWindow_Closed(object sender, EventArgs e)
        {
            CloseWindowEvent?.Invoke(true, "overview");
        }
        #endregion
    }
}
