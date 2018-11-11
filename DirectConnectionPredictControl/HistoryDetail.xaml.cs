using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// HistoryDetail.xaml 的交互逻辑
    /// </summary>
    public partial class HistoryDetail : Window
    {
        public event closeWindowHandler CloseWindowEvent;
        private HistoryModel history;
        private CompositeCollection collection = new CompositeCollection();
        private List<HistoryDataModel> dataModelList = new List<HistoryDataModel>();
        private int nowPage = 1;
        private int totalPage;
        private int location;
        private static int LINE_PER_TIME = 100;
        private int id = 0;


        public HistoryDetail()
        {
            InitializeComponent();
            location = 0;
        }

        #region bussiness methods

        #region data insert

        private void GetData(int location, int tail)
        {
            //2018-10-10
            //if (dataModelList.Count > 0)
            //{
                dataModelList.Clear();
                historyList.ItemsSource = null;
                historyList.Items.Clear();
                
            //}
            for (int i = location; i < LINE_PER_TIME + location; i++)
            {
                HistoryDataModel temp = new HistoryDataModel();
                #region detail
                id++;
                temp.ID = id;
                temp.LifeSig_1 = history.Containers_1[i].LifeSig;
                temp.LifeSig_2 = history.Containers_2[i].LifeSig;
                temp.LifeSig_3 = history.Containers_3[i].LifeSig;
                temp.LifeSig_4 = history.Containers_4[i].LifeSig;
                temp.LifeSig_5 = history.Containers_5[i].LifeSig;
                temp.LifeSig_6 = history.Containers_6[i].LifeSig;

                temp.dateTime = history.Containers_1[i].dateTime;

                temp.RefSpeed = history.Containers_1[i].RefSpeed;
                temp.Mode = history.Containers_1[i].Mode;
                temp.BrakeCmd = history.Containers_1[i].BrakeCmd;
                temp.DriveCmd = history.Containers_1[i].DriveCmd;
                temp.FastBrakeCmd = history.Containers_1[i].FastBrakeCmd;
                temp.LazyCmd = history.Containers_1[i].LazyCmd;
                temp.EmergencyBrakeCmd = history.Containers_1[i].EmergencyBrakeCmd;
                temp.HoldBrakeRealease = history.Containers_1[i].HoldBrakeRealease;
                temp.LazyState = history.Containers_1[i].LazyState;
                temp.DriveState = history.Containers_1[i].DriveState;
                temp.NormalBrakeState = history.Containers_1[i].NormalBrakeState;
                temp.EmergencyBrakeState = history.Containers_1[i].EmergencyBrakeState;
                temp.ZeroSpeed = history.Containers_1[i].ZeroSpeed;
                temp.BrakeLevel = history.Containers_1[i].BrakeLevel;
                temp.TrainBrakeForce = history.Containers_1[i].TrainBrakeForce;
                temp.SpeedAx1_1 = history.Containers_1[i].SpeedA1Shaft1;
                temp.SpeedAx2_1 = history.Containers_1[i].SpeedA1Shaft2;
                temp.SpeedAx1_2 = history.Containers_2[i].SpeedShaft1;
                temp.SpeedAx2_2 = history.Containers_2[i].SpeedShaft2;
                temp.SpeedAx1_3 = history.Containers_3[i].SpeedShaft1;
                temp.SpeedAx2_3 = history.Containers_3[i].SpeedShaft2;
                temp.SpeedAx1_4 = history.Containers_4[i].SpeedShaft1;
                temp.SpeedAx2_4 = history.Containers_4[i].SpeedShaft2;
                temp.SpeedAx1_5 = history.Containers_5[i].SpeedShaft1;
                temp.SpeedAx2_5 = history.Containers_5[i].SpeedShaft2;
                temp.SpeedAx1_6 = history.Containers_6[i].SpeedA1Shaft1;
                temp.SpeedAx2_6 = history.Containers_6[i].SpeedA1Shaft2;
                temp.VCM2MVBState_1 = history.Containers_1[i].VCM_MVBConnectionState;
                temp.VCM2MVBState_2 = history.Containers_6[i].VCM_MVBConnectionState;
                temp.Slip_1 = history.Containers_1[i].SlipA1;
                temp.Slip_2 = history.Containers_2[i].Slip;
                temp.Slip_3 = history.Containers_3[i].Slip;
                temp.Slip_4 = history.Containers_4[i].Slip;
                temp.Slip_5 = history.Containers_5[i].Slip;
                temp.Slip_6 = history.Containers_6[i].SlipA1;
                temp.EmergencyBrakeActive_1 = history.Containers_1[i].EmergencyBrakeActiveA1;
                temp.EmergencyBrakeActive_2 = history.Containers_2[i].EmergencyBrakeActive;
                temp.EmergencyBrakeActive_3 = history.Containers_3[i].EmergencyBrakeActive;
                temp.EmergencyBrakeActive_4 = history.Containers_4[i].EmergencyBrakeActive;
                temp.EmergencyBrakeActive_5 = history.Containers_5[i].EmergencyBrakeActive;
                temp.EmergencyBrakeActive_6 = history.Containers_6[i].EmergencyBrakeActiveA1;
                temp.NotZeroSpeed = history.Containers_1[i].NotZeroSpeed;
                temp.AbActive_1 = history.Containers_1[i].AbActive;
                temp.AbActive_2 = history.Containers_2[i].AbBrakeActive;
                temp.AbActive_3 = history.Containers_3[i].AbBrakeActive;
                temp.AbActive_4 = history.Containers_4[i].AbBrakeActive;
                temp.AbActive_5 = history.Containers_5[i].AbBrakeActive;
                temp.AbActive_6 = history.Containers_6[i].AbActive;
                temp.BSRLow_1 = history.Containers_1[i].BSRLowA11;
                temp.BSRLow_2 = history.Containers_2[i].BSRLow1;
                temp.BSRLow_3 = history.Containers_3[i].BSRLow1;
                temp.BSRLow_4 = history.Containers_4[i].BSRLow1;
                temp.BSRLow_5 = history.Containers_5[i].BSRLow1;
                temp.BSRLow_6 = history.Containers_6[i].BSRLowA11;
                temp.ParkBreakRealease = history.Containers_1[i].ParkBreakRealease;
                temp.AbStatues_1 = history.Containers_1[i].AbStatuesA1;
                temp.AbStatues_2 = history.Containers_2[i].AbBrakeSatet;
                temp.AbStatues_3 = history.Containers_3[i].AbBrakeSatet;
                temp.AbStatues_4 = history.Containers_4[i].AbBrakeSatet;
                temp.AbStatues_5 = history.Containers_5[i].AbBrakeSatet;
                temp.AbStatues_6 = history.Containers_6[i].AbStatuesA1;
                temp.MassValid_1 = history.Containers_1[i].MassSigValid;
                temp.MassValid_2 = history.Containers_2[i].MassSigValid;
                temp.MassValid_3 = history.Containers_3[i].MassSigValid;
                temp.MassValid_4 = history.Containers_4[i].MassSigValid;
                temp.MassValid_5 = history.Containers_5[i].MassSigValid;
                temp.MassValid_6 = history.Containers_6[i].MassSigValid;
                temp.AbTargetValue_1 = history.Containers_1[i].AbTargetValueAx1;
                temp.AbTargetValue_2 = history.Containers_1[i].AbTargetValueAx2;
                temp.AbTargetValue_3 = history.Containers_1[i].AbTargetValueAx3;
                temp.AbTargetValue_4 = history.Containers_1[i].AbTargetValueAx4;
                temp.AbTargetValue_5 = history.Containers_1[i].AbTargetValueAx5;
                temp.AbTargetValue_6 = history.Containers_1[i].AbTargetValueAx6;
                temp.SelfInt = history.Containers_1[i].SelfTestInt;
                temp.SelfActive = history.Containers_1[i].SelfTestActive;
                temp.SelfSuccess = history.Containers_1[i].SelfTestSuccess;
                temp.SelfFail = history.Containers_1[i].SelfTestFail;
                temp.UnTest24 = history.Containers_1[i].UnSelfTest24;
                temp.UnTest26 = history.Containers_1[i].UnSelfTest26;
                temp.GatewayValveState = history.Containers_1[i].GateValveState;
                temp.HardDrive = history.Containers_1[i].HardDriveCmd;
                temp.HardBrake = history.Containers_1[i].HardBrakeCmd;
                temp.HardFastBrake = history.Containers_1[i].HardFastBrakeCmd;
                temp.HardEmergencyBrake = history.Containers_1[i].HardEmergencyBrake;
                temp.HardEmergencyDrive = history.Containers_1[i].HardEmergencyDriveCmd;
                temp.CanUnitTestOn = history.Containers_1[i].CanUnitSelfTestOn;
                temp.CanUnitTestOff = history.Containers_1[i].CanUintSelfTestOver;
                temp.CanValveActive = history.Containers_1[i].ValveCanEmergencyActive;
                temp.NetDrive = history.Containers_1[i].NetDriveCmd;
                temp.NetBrake = history.Containers_1[i].NetBrakeCmd;
                temp.NetFastBrake = history.Containers_1[i].NetFastBrakeCmd;
                temp.TowingMode = history.Containers_1[i].TowingMode;
                temp.KeepBrakeRelease = history.Containers_1[i].KeepBrakeState;
                temp.CanTestA = history.Containers_1[i].CanUintSelfTestCmd_A;
                temp.CanTestB = history.Containers_1[i].CanUintSelfTestCmd_B;
                temp.BrakeLevelActive = history.Containers_1[i].BrakeLevelEnable;
                temp.SelfTestCmd = history.Containers_1[i].SelfTestCmd;
                temp.AbFadeOut = history.Containers_1[i].EdFadeOut;
                temp.TrainBrakeEnable = history.Containers_1[i].TrainBrakeEnable;
                temp.EDoutB = history.Containers_1[i].EdOffB1;
                temp.EDoutC = history.Containers_1[i].EdOffC1;
                temp.WheelInputState = history.Containers_1[i].WheelInputState;
                temp.BrakeCylinderSourcePressure_1 = history.Containers_1[i].BrakeCylinderSourcePressure;
                temp.BrakeCylinderSourcePressure_2 = history.Containers_2[i].BrakeCylinderSourcePressure;
                temp.BrakeCylinderSourcePressure_3 = history.Containers_3[i].BrakeCylinderSourcePressure;
                temp.BrakeCylinderSourcePressure_4 = history.Containers_4[i].BrakeCylinderSourcePressure;
                temp.BrakeCylinderSourcePressure_5 = history.Containers_5[i].BrakeCylinderSourcePressure;
                temp.BrakeCylinderSourcePressure_6 = history.Containers_6[i].BrakeCylinderSourcePressure;
                temp.AirSpringPressure1_1 = history.Containers_1[i].AirSpring1PressureA1Car1;
                temp.AirSpringPressure2_1 = history.Containers_1[i].AirSpring2PressureA1Car1;
                temp.AirSpringPressure1_2 = history.Containers_2[i].AirSpringPressure1;
                temp.AirSpringPressure2_2 = history.Containers_2[i].AirSpringPressure2;
                temp.AirSpringPressure1_3 = history.Containers_3[i].AirSpringPressure1;
                temp.AirSpringPressure2_3 = history.Containers_3[i].AirSpringPressure2;
                temp.AirSpringPressure1_4 = history.Containers_4[i].AirSpringPressure1;
                temp.AirSpringPressure2_4 = history.Containers_4[i].AirSpringPressure2;
                temp.AirSpringPressure1_5 = history.Containers_5[i].AirSpringPressure1;
                temp.AirSpringPressure2_5 = history.Containers_5[i].AirSpringPressure2;
                temp.AirSpringPressure1_6 = history.Containers_6[i].AirSpring1PressureA1Car1;
                temp.AirSpringPressure2_6 = history.Containers_6[i].AirSpring2PressureA1Car1;
                temp.ParkPressure_1 = history.Containers_1[i].ParkPressureA1;
                temp.ParkPressure_2 = history.Containers_3[i].ParkPressure;
                temp.ParkPressure_3 = history.Containers_5[i].ParkPressure;
                temp.ParkPressure_4 = history.Containers_4[i].ParkPressure;
                temp.VldRealPressure_1 = history.Containers_1[i].VldRealPressureAx1;
                temp.VldRealPressure_2 = history.Containers_2[i].VldRealPressure;
                temp.VldRealPressure_3 = history.Containers_3[i].VldRealPressure;
                temp.VldRealPressure_4 = history.Containers_4[i].VldRealPressure;
                temp.VldRealPressure_5 = history.Containers_5[i].VldRealPressure;
                temp.VldRealPressure_6 = history.Containers_6[i].VldRealPressureAx1;
                temp.Bcp1Pressure_1 = history.Containers_1[i].Bcp1PressureAx1;
                temp.Bcp2Pressure_1 = history.Containers_1[i].Bcp2PressureAx2;
                temp.Bcp1Pressure_2 = history.Containers_2[i].Bcp1Pressure;
                temp.Bcp2Pressure_2 = history.Containers_2[i].Bcp2Pressure;
                temp.Bcp1Pressure_3 = history.Containers_3[i].Bcp1Pressure;
                temp.Bcp2Pressure_3 = history.Containers_3[i].Bcp2Pressure;
                temp.Bcp1Pressure_4 = history.Containers_4[i].Bcp1Pressure;
                temp.Bcp2Pressure_4 = history.Containers_4[i].Bcp2Pressure;
                temp.Bcp1Pressure_5 = history.Containers_5[i].Bcp1Pressure;
                temp.Bcp2Pressure_5 = history.Containers_5[i].Bcp2Pressure;
                temp.Bcp1Pressure_6 = history.Containers_6[i].Bcp1PressureAx1;
                temp.Bcp2Pressure_6 = history.Containers_6[i].Bcp2PressureAx2;
                temp.BSSRSenorFault_1 = history.Containers_1[i].BSSRSenorFault;
                temp.BSSRSenorFault_2 = history.Containers_2[i].BSSRSenorFault;
                temp.BSSRSenorFault_3 = history.Containers_3[i].BSSRSenorFault;
                temp.BSSRSenorFault_4 = history.Containers_4[i].BSSRSenorFault;
                temp.BSSRSenorFault_5 = history.Containers_5[i].BSSRSenorFault;
                temp.BSSRSenorFault_6 = history.Containers_6[i].BSSRSenorFault;
                temp.AirSpringSenorFault1_1 = history.Containers_1[i].AirSpringSenorFault_1;
                temp.AirSpringSenorFault2_1 = history.Containers_1[i].AirSpringSenorFault_2;
                temp.AirSpringSenorFault1_2 = history.Containers_2[i].AirSpringSenorFault_1;
                temp.AirSpringSenorFault2_2 = history.Containers_2[i].AirSpringSenorFault_2;
                temp.AirSpringSenorFault1_3 = history.Containers_3[i].AirSpringSenorFault_1;
                temp.AirSpringSenorFault2_3 = history.Containers_3[i].AirSpringSenorFault_2;
                temp.AirSpringSenorFault1_4 = history.Containers_4[i].AirSpringSenorFault_1;
                temp.AirSpringSenorFault2_4 = history.Containers_4[i].AirSpringSenorFault_2;
                temp.AirSpringSenorFault1_5 = history.Containers_5[i].AirSpringSenorFault_1;
                temp.AirSpringSenorFault2_5 = history.Containers_5[i].AirSpringSenorFault_2;
                temp.AirSpringSenorFault1_6 = history.Containers_6[i].AirSpringSenorFault_1;
                temp.AirSpringSenorFault2_6 = history.Containers_6[i].AirSpringSenorFault_2;
                temp.ParkCylinderSenorFault_1 = history.Containers_1[i].ParkCylinderSenorFault;
                temp.ParkCylinderSenorFault_2 = history.Containers_2[i].ParkCylinderSenorFault;
                temp.ParkCylinderSenorFault_3 = history.Containers_3[i].ParkCylinderSenorFault;
                temp.ParkCylinderSenorFault_4 = history.Containers_4[i].ParkCylinderSenorFault;
                temp.ParkCylinderSenorFault_5 = history.Containers_5[i].ParkCylinderSenorFault;
                temp.ParkCylinderSenorFault_6 = history.Containers_6[i].ParkCylinderSenorFault;
                temp.VLDSensorFault_1 = history.Containers_1[i].VLDSensorFault;
                temp.VLDSensorFault_2 = history.Containers_2[i].VLDSensorFault;
                temp.VLDSensorFault_3 = history.Containers_3[i].VLDSensorFault;
                temp.VLDSensorFault_4 = history.Containers_4[i].VLDSensorFault;
                temp.VLDSensorFault_5 = history.Containers_5[i].VLDSensorFault;
                temp.VLDSensorFault_6 = history.Containers_6[i].VLDSensorFault;
                temp.BSRSenorFault1_1 = history.Containers_1[i].BSRSenorFault_1;
                temp.BSRSenorFault2_1 = history.Containers_1[i].BSRSenorFault_2;
                temp.BSRSenorFault1_2 = history.Containers_2[i].BSRSenorFault_1;
                temp.BSRSenorFault2_2 = history.Containers_2[i].BSRSenorFault_2;
                temp.BSRSenorFault1_3 = history.Containers_3[i].BSRSenorFault_1;
                temp.BSRSenorFault2_3 = history.Containers_3[i].BSRSenorFault_2;
                temp.BSRSenorFault1_4 = history.Containers_4[i].BSRSenorFault_1;
                temp.BSRSenorFault2_4 = history.Containers_4[i].BSRSenorFault_2;
                temp.BSRSenorFault1_5 = history.Containers_5[i].BSRSenorFault_1;
                temp.BSRSenorFault2_5 = history.Containers_5[i].BSRSenorFault_2;
                temp.BSRSenorFault1_6 = history.Containers_6[i].BSRSenorFault_1;
                temp.BSRSenorFault2_6 = history.Containers_6[i].BSRSenorFault_2;
                temp.AirSpringOverflow1_1 = history.Containers_1[i].AirSpringOverflow_1;
                temp.AirSpringOverflow2_1 = history.Containers_1[i].AirSpringOverflow_2;
                temp.AirSpringOverflow1_2 = history.Containers_2[i].AirSpringOverflow_1;
                temp.AirSpringOverflow2_2 = history.Containers_2[i].AirSpringOverflow_2;
                temp.AirSpringOverflow1_3 = history.Containers_3[i].AirSpringOverflow_1;
                temp.AirSpringOverflow2_3 = history.Containers_3[i].AirSpringOverflow_2;
                temp.AirSpringOverflow1_4 = history.Containers_4[i].AirSpringOverflow_1;
                temp.AirSpringOverflow2_4 = history.Containers_4[i].AirSpringOverflow_2;
                temp.AirSpringOverflow1_5 = history.Containers_5[i].AirSpringOverflow_1;
                temp.AirSpringOverflow2_5 = history.Containers_5[i].AirSpringOverflow_2;
                temp.AirSpringOverflow1_6 = history.Containers_6[i].AirSpringOverflow_1;
                temp.AirSpringOverflow2_6 = history.Containers_6[i].AirSpringOverflow_2;
                temp.VldPressureSetup_1 = history.Containers_1[i].VldPressureSetupAx1;
                temp.VldPressureSetup_2 = history.Containers_2[i].VldSetupPressure;
                temp.VldPressureSetup_3 = history.Containers_3[i].VldSetupPressure;
                temp.VldPressureSetup_4 = history.Containers_4[i].VldSetupPressure;
                temp.VldPressureSetup_5 = history.Containers_5[i].VldSetupPressure;
                temp.VldPressureSetup_6 = history.Containers_6[i].VldPressureSetupAx1;
                temp.Mass_1 = history.Containers_1[i].MassA1;
                temp.Mass_2 = history.Containers_2[i].MassValue;
                temp.Mass_3 = history.Containers_3[i].MassValue;
                temp.Mass_4 = history.Containers_4[i].MassValue;
                temp.Mass_5 = history.Containers_5[i].MassValue;
                temp.Mass_6 = history.Containers_6[i].MassA1;
                temp.BCUFail_Serious_1 = history.Containers_1[i].BCUFail_Serious;
                temp.BCUFail_Middle_1 = history.Containers_1[i].BCUFail_Middle;
                temp.BCUFail_Slight_1 = history.Containers_1[i].BCUFail_Slight;
                temp.BCUFail_Serious_2 = history.Containers_2[i].BCUFail_Serious;
                temp.BCUFail_Middle_2 = history.Containers_2[i].BCUFail_Middle;
                temp.BCUFail_Slight_2 = history.Containers_2[i].BCUFail_Slight;
                temp.BCUFail_Serious_3 = history.Containers_3[i].BCUFail_Serious;
                temp.BCUFail_Middle_3 = history.Containers_3[i].BCUFail_Middle;
                temp.BCUFail_Slight_3 = history.Containers_3[i].BCUFail_Slight;
                temp.BCUFail_Serious_4 = history.Containers_4[i].BCUFail_Serious;
                temp.BCUFail_Middle_4 = history.Containers_4[i].BCUFail_Middle;
                temp.BCUFail_Slight_4 = history.Containers_4[i].BCUFail_Slight;
                temp.BCUFail_Serious_5 = history.Containers_5[i].BCUFail_Serious;
                temp.BCUFail_Middle_5 = history.Containers_5[i].BCUFail_Middle;
                temp.BCUFail_Slight_5 = history.Containers_5[i].BCUFail_Slight;
                temp.BCUFail_Serious_6 = history.Containers_6[i].BCUFail_Serious;
                temp.BCUFail_Middle_6 = history.Containers_6[i].BCUFail_Middle;
                temp.BCUFail_Slight_6 = history.Containers_6[i].BCUFail_Slight;
                temp.EmergencyBrakeFault_1 = history.Containers_1[i].EmergencyBrakeFault;
                temp.EmergencyBrakeFault_2 = history.Containers_2[i].EmergencyBrakeFault;
                temp.EmergencyBrakeFault_3 = history.Containers_3[i].EmergencyBrakeFault;
                temp.EmergencyBrakeFault_4 = history.Containers_4[i].EmergencyBrakeFault;
                temp.EmergencyBrakeFault_5 = history.Containers_5[i].EmergencyBrakeFault;
                temp.EmergencyBrakeFault_6 = history.Containers_6[i].EmergencyBrakeFault;
                temp.SpeedSenorFault1_1 = history.Containers_1[i].SpeedSenorFault_1;
                temp.SpeedSenorFault2_1 = history.Containers_1[i].SpeedSenorFault_2;
                temp.SpeedSenorFault1_2 = history.Containers_2[i].SpeedSenorFault_1;
                temp.SpeedSenorFault2_2 = history.Containers_2[i].SpeedSenorFault_2;
                temp.SpeedSenorFault1_3 = history.Containers_3[i].SpeedSenorFault_1;
                temp.SpeedSenorFault2_3 = history.Containers_3[i].SpeedSenorFault_2;
                temp.SpeedSenorFault1_4 = history.Containers_4[i].SpeedSenorFault_1;
                temp.SpeedSenorFault2_4 = history.Containers_4[i].SpeedSenorFault_2;
                temp.SpeedSenorFault1_5 = history.Containers_5[i].SpeedSenorFault_1;
                temp.SpeedSenorFault2_5 = history.Containers_5[i].SpeedSenorFault_2;
                temp.SpeedSenorFault1_6 = history.Containers_6[i].SpeedSenorFault_1;
                temp.SpeedSenorFault2_6 = history.Containers_6[i].SpeedSenorFault_2;
                temp.WSPFault1_1 = history.Containers_1[i].WSPFault_1;
                temp.WSPFault2_1 = history.Containers_1[i].WSPFault_2;
                temp.WSPFault1_2 = history.Containers_2[i].WSPFault_1;
                temp.WSPFault2_2 = history.Containers_2[i].WSPFault_2;
                temp.WSPFault1_3 = history.Containers_3[i].WSPFault_1;
                temp.WSPFault2_3 = history.Containers_3[i].WSPFault_2;
                temp.WSPFault1_4 = history.Containers_4[i].WSPFault_1;
                temp.WSPFault2_4 = history.Containers_4[i].WSPFault_2;
                temp.WSPFault1_5 = history.Containers_5[i].WSPFault_1;
                temp.WSPFault2_5 = history.Containers_5[i].WSPFault_2;
                temp.WSPFault1_6 = history.Containers_6[i].WSPFault_1;
                temp.WSPFault2_6 = history.Containers_6[i].WSPFault_2;
                temp.CodeConnectorFault_1 = history.Containers_1[i].CodeConnectorFault;
                temp.CodeConnectorFault_2 = history.Containers_2[i].CodeConnectorFault;
                temp.CodeConnectorFault_3 = history.Containers_3[i].CodeConnectorFault;
                temp.CodeConnectorFault_4 = history.Containers_4[i].CodeConnectorFault;
                temp.CodeConnectorFault_5 = history.Containers_5[i].CodeConnectorFault;
                temp.CodeConnectorFault_6 = history.Containers_6[i].CodeConnectorFault;
                temp.AirSpringLimit_1 = history.Containers_1[i].AirSpringLimit;
                temp.AirSpringLimit_2 = history.Containers_2[i].AirSpringLimit;
                temp.AirSpringLimit_3 = history.Containers_3[i].AirSpringLimit;
                temp.AirSpringLimit_4 = history.Containers_4[i].AirSpringLimit;
                temp.AirSpringLimit_5 = history.Containers_5[i].AirSpringLimit;
                temp.AirSpringLimit_6 = history.Containers_6[i].AirSpringLimit;
                temp.BrakeNotRealease_1 = history.Containers_1[i].BrakeNotRealease;
                temp.BrakeNotRealease_2 = history.Containers_2[i].BrakeNotRealease;
                temp.BrakeNotRealease_3 = history.Containers_3[i].BrakeNotRealease;
                temp.BrakeNotRealease_4 = history.Containers_4[i].BrakeNotRealease;
                temp.BrakeNotRealease_5 = history.Containers_5[i].BrakeNotRealease;
                temp.BrakeNotRealease_6 = history.Containers_6[i].BrakeNotRealease;
                temp.BCPLow_1 = history.Containers_1[i].BCPLowA11;
                temp.BCPLow_2 = history.Containers_2[i].BCPLow1;
                temp.BCPLow_3 = history.Containers_3[i].BCPLow1;
                temp.BCPLow_4 = history.Containers_4[i].BCPLow1;
                temp.BCPLow_5 = history.Containers_5[i].BCPLow1;
                temp.BCPLow_6 = history.Containers_6[i].BCPLowA11;
                temp.VCMLifeSig = history.Containers_1[i].VCMLifeSig;
                temp.DCULifeSig_1 = history.Containers_1[i].DcuLifeSig[0];
                temp.DCULifeSig_2 = history.Containers_1[i].DcuLifeSig[1];
                temp.DCULifeSig_3 = history.Containers_1[i].DcuLifeSig[2];
                temp.DCULifeSig_4 = history.Containers_1[i].DcuLifeSig[3];
                temp.DCU_Ed_Ok_1 = history.Containers_1[i].DcuEbOK[0];
                temp.DCU_Ed_Fadeout_1 = history.Containers_1[i].DcuEbFadeout[0];
                temp.DCU_Ed_Slip_1 = history.Containers_1[i].DcuEbSlip[0];
                temp.DCU_Ed_Ok_2 = history.Containers_1[i].DcuEbOK[1];
                temp.DCU_Ed_Fadeout_2 = history.Containers_1[i].DcuEbFadeout[1];
                temp.DCU_Ed_Slip_2 = history.Containers_1[i].DcuEbSlip[1];
                temp.DCU_Ed_Ok_3 = history.Containers_1[i].DcuEbOK[2];
                temp.DCU_Ed_Fadeout_3 = history.Containers_1[i].DcuEbFadeout[2];
                temp.DCU_Ed_Slip_3 = history.Containers_1[i].DcuEbSlip[2];
                temp.DCU_Ed_Ok_4 = history.Containers_1[i].DcuEbOK[3];
                temp.DCU_Ed_Fadeout_4 = history.Containers_1[i].DcuEbFadeout[3];
                temp.DCU_Ed_Slip_4 = history.Containers_1[i].DcuEbSlip[3];
                temp.DcuEbRealValue_1 = history.Containers_1[i].DcuEbRealValue[0];
                temp.DcuEbRealValue_2 = history.Containers_1[i].DcuEbRealValue[1];
                temp.DcuEbRealValue_3 = history.Containers_1[i].DcuEbRealValue[2];
                temp.DcuEbRealValue_4 = history.Containers_1[i].DcuEbRealValue[3];
                temp.DcuMax_1 = history.Containers_1[i].DcuMax[0];
                temp.DcuMax_2 = history.Containers_1[i].DcuMax[1];
                temp.DcuMax_3 = history.Containers_1[i].DcuMax[2];
                temp.DcuMax_4 = history.Containers_1[i].DcuMax[3];
                temp.AbCapacity_1 = history.Containers_1[i].AbCapacity[0];
                temp.AbCapacity_2 = history.Containers_1[i].AbCapacity[1];
                temp.AbCapacity_3 = history.Containers_1[i].AbCapacity[2];
                temp.AbCapacity_4 = history.Containers_1[i].AbCapacity[3];
                temp.AbCapacity_5 = history.Containers_1[i].AbCapacity[4];
                temp.AbCapacity_6 = history.Containers_1[i].AbCapacity[5];
                temp.AbRealValue_1 = history.Containers_1[i].AbRealValue[0];
                temp.AbRealValue_2 = history.Containers_1[i].AbRealValue[1];
                temp.AbRealValue_3 = history.Containers_1[i].AbRealValue[2];
                temp.AbRealValue_4 = history.Containers_1[i].AbRealValue[3];
                temp.AbRealValue_5 = history.Containers_1[i].AbRealValue[4];
                temp.AbRealValue_6 = history.Containers_1[i].AbRealValue[5];
                temp.SpeedDetection = history.Containers_1[i].SpeedDetection;
                temp.CanBusFail1 = history.Containers_1[i].CanBusFail1;
                temp.CanBusFail2 = history.Containers_1[i].CanBusFail2;
                temp.HardDifferent = history.Containers_1[i].HardDifferent;
                temp.Event_High = history.Containers_1[i].EventHigh;
                temp.Event_Middle = history.Containers_1[i].EventMid;
                temp.Event_Low = history.Containers_1[i].EventLow;
                temp.CanASPActive = history.Containers_1[i].CanASPEnable;
                temp.BCPLowA = history.Containers_1[i].BCPLowA;
                temp.BCPLowB = history.Containers_1[i].BCPLowB;
                temp.BCPLowC = history.Containers_1[i].BCPLowC;
                temp.SoftVersion = history.Containers_1[i].SoftVersion;
                #endregion
                dataModelList.Add(temp);

            }
            historyList.ItemsSource = dataModelList;
            historyList.ScrollIntoView(historyList.Items[0]);
        }

        #endregion

        public void SetHistory(HistoryModel history)
        {
            this.history = history;
            GetData(location, location + LINE_PER_TIME);
            totalPage = (int)(history.Count / LINE_PER_TIME) + 1;
            totalPageLbl.Content = totalPage;
            byteHasReadLbl.Content = history.FileLength / 1024 + " KB";
        }

        private void aftEvent(object sender, RoutedEventArgs e)
        {
            if (location + LINE_PER_TIME > history.Count)
            {
                MessageBox.Show("已到达尾页");
            }
            else
            {
                location += LINE_PER_TIME;
                GetData(location, location + LINE_PER_TIME);
                nowPage++;
                nowPageLbl.Content = nowPage;
            }
        }
        
        private void preEvent(object sender, RoutedEventArgs e)
        {
            if (location == 0)
            {
                MessageBox.Show("已到达首页");
            }
            else
            {
                location -= LINE_PER_TIME;
                GetData(location, location + LINE_PER_TIME);
                nowPage--;
                nowPageLbl.Content = nowPage;
            }
        }

        #endregion

        #region window methods

        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.ToolBar toolBar = sender as System.Windows.Controls.ToolBar;
            var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
            if (overflowGrid != null)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }
            var mainPanelBorder = toolBar.Template.FindName("MainPanelBorder", toolBar) as FrameworkElement;
            {
                mainPanelBorder.Margin = new Thickness(0);
            }
        }
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

        private void historyDetail_Closed(object sender, EventArgs e)
        {
            CloseWindowEvent?.Invoke(true, "history");
        }

        #endregion

        /// <summary>
        /// 列显示配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void columnConfigItem_Click(object sender, RoutedEventArgs e)
        {
            //showdialog
            
        }

        private void showAnalog_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void showAnalog_Unchecked(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
