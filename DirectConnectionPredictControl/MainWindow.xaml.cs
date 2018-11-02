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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using DirectConnectionPredictControl.IO;
using System.Windows.Media.Animation;
using System.Threading;
using DirectConnectionPredictControl.CommenTool;
using System.Diagnostics;
using System.Configuration;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Runtime.InteropServices;

namespace DirectConnectionPredictControl
{
    public enum FormatType
    {
        REAL_TIME,
        HISTORY
    }

    public enum FormatCommand
    {
        OK,
        WAIT,
        IGNORE
    }

    public enum ConnectType
    {
        ETH,
        CAN
    }

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private byte[] recvData;
        private delegate void updateUI(MainDevDataContains mainDevDataContains);
        private Thread updateUIHandler;
        private Thread recvThread;
        private Thread recordThread;
        private CanHelper canHelper;
        private ConnectType connectType = ConnectType.CAN;
        private int recordFreq = 16;
        private CanDTO dTO = null;

        //数据组
        private MainDevDataContains container_1;
        private SliverDataContainer container_2;
        private SliverDataContainer container_3;
        private SliverDataContainer container_4;
        private SliverDataContainer container_5;
        private MainDevDataContains container_6;

        //历史数据组
        private HistoryModel history;

        //窗口组
        private DetailWindow detailWindowCar1;
        private SlaveDetailWindow slaveDetailWindowCar2;
        private SlaveDetailWindow slaveDetailWindowCar3;
        private SlaveDetailWindow slaveDetailWindowCar4;
        private SlaveDetailWindow slaveDetailWindowCar5;
        private DetailWindow slaveDetailWindowCar6;
        private RealTimeSpeedChartWindow speedChartWindow;
        private RealTimePressureChartWindow pressureChartWindow;
        private RealTimeOtherWindow otherWindow;
        private OverviewWindow overviewWindow;
        private ChartWindow chartWindow;
        private SingleChart singleChartWindow;
        //2018-9-23:新增一个防滑数据显示窗口
        private Antiskid_Display antiskid_Display_Window;
        //2018-9-24:新增一个防滑数据设置窗口
        private Antiskid_Setting antiskid_Setting_Window;

        private double index = 0.0;
        
        //测试组
        private Thread testThread;

        private UserDateTime userDateTime;
        public MainWindow()
        {
            InitializeComponent();
            byEthItem.IsChecked = false;
            byCanItem.IsChecked = true;
            //用代码调动StoryBoard
            Storyboard storyBoard = (Storyboard)MyWindow.Resources["open"];
            if (storyBoard != null)
            {
                storyBoard.Begin();
            }
            Init();
            //Test();
        }

        #region 测试用
        /// <summary>
        /// 测试用例
        /// </summary>
        private void Test()
        {
            container_1 = new MainDevDataContains();
            container_2 = new SliverDataContainer();
            container_3 = new SliverDataContainer();
            container_4 = new SliverDataContainer();
            container_5 = new SliverDataContainer();
            container_6 = new MainDevDataContains();
            history = new HistoryModel();
            testThread = new Thread(TestHandler);
            recordThread = new Thread(RecordHandler);
            testThread.Start();
            recordThread.Start();
        }

        private void RecordHandler()// 做文件记录
        {
            FileBuilding building = new FileBuilding();
            while (true)
            {
                Thread.Sleep(recordFreq);
                if (dTO != null)
                {
                    building.Record(dTO);
                }
            }
        }

        /// <summary>
        /// 测试线程
        /// 
        /// </summary>
        private void TestHandler()
        {
            Random random = new Random();

            while (true)
            {
                Thread.Sleep(1000);
                int speedValue = random.Next(120);
                int accSetup = random.Next(100);
                int air = random.Next(1000);
                // 2018-10-12:增加一个测试的减速度值试一试
                int Jian_Speed_1 = random.Next(10);
                dTO = new CanDTO();
                dTO.Data = new byte[] { 0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37 };
                dTO.Id = 0x2200000;
                dTO.Time = DateTime.Now;
                container_1.SpeedA1Shaft1 = speedValue;
                container_1.SpeedA1Shaft2 = speedValue;
                container_1.RefSpeed = accSetup;
                container_1.Bcp1PressureAx1 = 400;
                container_1.Bcp2PressureAx2 = 420;
                container_1.MassA1 = 200;
                container_2.SpeedShaft1 = speedValue;
                container_2.SpeedShaft2 = speedValue;
                container_2.Bcp1Pressure = air / 2;
                container_3.Bcp1Pressure = air / 3;
                container_2.MassValue = air / 2;
                container_2.AbBrakeActive = true;
                container_1.BrakeCmd = true;
                container_3.AccValue1 = -Jian_Speed_1;
                updateUIMethod(container_1);
            }
        }
        #endregion

        private void Init()
        {
            container_1 = new MainDevDataContains();
            container_2 = new SliverDataContainer();
            container_3 = new SliverDataContainer();
            container_4 = new SliverDataContainer();
            container_5 = new SliverDataContainer();
            container_6 = new MainDevDataContains();
            history = new HistoryModel();
            userDateTime = new UserDateTime()
            {
                Year = 2018,
                Month = 3,
                Day = 10,
                Hour = 0,
                Minute = 0,
                Second = 0
            };
            if (connectType == ConnectType.ETH)
            {

            }
            if (connectType == ConnectType.CAN)
            {
                canHelper = new CanHelper();
                recvThread = new Thread(RecvDataAsyncByCan);
                recvThread.IsBackground = true;
                recvThread.Start();
                updateUIHandler = new Thread(UpdateUIHandlerMethod);
                updateUIHandler.IsBackground = true;
                recordThread = new Thread(RecordHandler);
                recordThread.IsBackground = true;
                updateUIHandler.Start();
                recordThread.Start();
            }
            
        }

        private void UpdateUIHandlerMethod()
        {
            updateUI update = new updateUI(updateUIMethod);
            while (true)
            {
                Thread.Sleep(100);
                update.Invoke(container_1);
            }
        }

        /// <summary>
        /// 窗口拖动函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        

        public void OpenFile()
        {
            string fileName;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                double x = 0.0;
                fileName = ofd.FileName;
                List<byte[]> content = FileBuilding.GetFileContent(fileName);
                List<List<CanDTO>> canList = FileBuilding.GetCanList(content);
                history.FileLength = FileBuilding.FileLength;
                for (int i = 0; i < canList.Count; i++)
                {
                    int count = FileBuilding.CAN_MO_NUM;
                    history.Count = canList.Count;
                    history.X.Add(x);
                    x += 0.1;
                    for (int j = 0; j < canList[i].Count; j++)
                    {
                        if (--count == 0)
                        {
                            FormatData(canList[i][j], FormatType.HISTORY, FormatCommand.OK);
                        }
                        else
                        {
                            FormatData(canList[i][j], FormatType.HISTORY, FormatCommand.WAIT);
                        }
                        
                    }
                    index += 0.1;
                }
                HistoryDetail historyDetail = new HistoryDetail();
                historyDetail.SetHistory(history);
                historyDetail.Show();
                //SingleChart historyChart = new SingleChart();
                
                //historyChart.Show();
                //historyChart.SetHistoryModel(history);
                //historyChart.PaintHistory();
            }
        }
        /// <summary>
        /// 最小化按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miniumBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        /// <summary>
        /// 最大化按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void maximun_Click(object sender, RoutedEventArgs e) 
        {
            if (this.WindowState == WindowState.Maximized)
            {

                this.WindowState = WindowState.Normal;
                this.MainDashboard.dashboard.Width = 250;
                this.MainDashboard.dashboard.Height = 250;
                this.MainDashboard.speedtext.FontSize = 12;
                this.MainDashboard.speed.FontSize = 40;
                this.MainDashboard.Kmphtext.FontSize = 12;
                this.MainDashboard.dashTextStack.Margin = new Thickness(0, 0, 0, 80);
            }
            else
            {
                this.WindowState = WindowState.Maximized;
                this.MainDashboard.dashboard.Width = 350;
                this.MainDashboard.dashboard.Height = 350;
                this.MainDashboard.speedtext.FontSize = 16;
                this.MainDashboard.speed.FontSize = 48;
                this.MainDashboard.Kmphtext.FontSize = 16;
                this.MainDashboard.dashTextStack.Margin = new Thickness(0, 0, 0, 110);
            }
           
        }

        /// <summary>
        /// 菜单栏-文件点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFile();
        }

        //点击关闭按钮执行完退出动画后执行
        private void Storyboard_Completed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

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
        /// <summary>
        /// 主窗口加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            double x = SystemParameters.WorkArea.Width;
            double y = SystemParameters.WorkArea.Height;
            double x1 = SystemParameters.PrimaryScreenWidth;//得到屏幕整体宽度
            double y1 = SystemParameters.PrimaryScreenHeight;//得到屏幕整体高度
            this.Width = x1 * 2 / 3 ;
            this.Height = y1 * 4 / 5;
            //Thread recvThread = new Thread(RecvDataAsync);
            //recvThread.Start();
        }

        /// <summary>
        /// 异步方式接收，16毫秒一次
        /// </summary>
        unsafe private void RecvDataAsyncByCan()
        {
            canHelper.Open();
            canHelper.Init();
            canHelper.Start();
            while (true)
            {
                Thread.Sleep(16);
                List<CanDTO> list = canHelper.Recv();
                if (list.Count <= 0)
                {
                    continue;
                }
                dTO = list[list.Count - 1];
                FormatData(dTO, FormatType.REAL_TIME, FormatCommand.IGNORE);
            }
        }

        private void CheckZero(byte[] data, int index)
        {
            if ((data[index] & 0x80) == 0x80)
            {
                data[index] = 0;
                data[index + 1] = 0;
            }
        }

        /// <summary>
        /// 格式化接收的数据至类中
        /// </summary>
        /// <param name="recvData"></param>
        private void FormatData(CanDTO dTO, FormatType type, FormatCommand command)
        {
            //设置can数据指针
            
            int point = 0;

            byte[] recvData = dTO.Data;

            //判断数据来源
            uint canID = dTO.Id;
            canID = dTO.Id >> 21;
            uint canIdHigh = (canID & 0xf0) >> 4;
            uint canIdLow = canID & 0x0f;

            if (type == FormatType.REAL_TIME)
            {
                FormateRealTime(recvData, canIdHigh, canIdLow, FormatType.REAL_TIME, point);
            }
            if (type == FormatType.HISTORY)
            {
                MainDevDataContains container_1 = new MainDevDataContains();
                SliverDataContainer container_2 = new SliverDataContainer();
                SliverDataContainer container_3 = new SliverDataContainer();
                SliverDataContainer container_4 = new SliverDataContainer();
                SliverDataContainer container_5 = new SliverDataContainer();
                MainDevDataContains container_6 = new MainDevDataContains();

                if (type == FormatType.HISTORY && command == FormatCommand.OK)
                {
                    FormateHistory(recvData, canIdHigh, canIdLow, FormatType.HISTORY, FormatCommand.OK, container_1, container_2, container_3,
                        container_4, container_5, container_6);
                }

            }
        }


        private void FormateRealTime(byte[] recvData, uint canIdHigh, uint canIdLow, FormatType type, int point = 0)
        {
            #region 解析CAN数据包中的8个字节，根据CAN ID来决定字段含义
            switch (canIdHigh)
            {
                case 1:
                    #region 主设备A1车CAN消息（6个数据包）
                    switch (canIdLow)
                    {
                        case 0:
                            #region TPDO0(Checked)

                            #region byte0
                            if ((recvData[0] & 0x01) == 0x01)
                            {
                                container_1.Mode = MainDevDataContains.NORMAL_MODE;
                            }
                            else if ((recvData[0] & 0x02) == 0x02)
                            {
                                container_1.Mode = MainDevDataContains.EMERGENCY_DRIVE_MODE;
                            }
                            else if ((recvData[0] & 0x04) == 0x04)
                            {
                                container_1.Mode = MainDevDataContains.CALLBACK_MODE;
                            }
                            container_1.BrakeCmd = (recvData[0] & 0x08) == 0x08 ? true : false;
                            container_1.DriveCmd = (recvData[0] & 0x10) == 0x10 ? true : false;
                            container_1.LazyCmd = (recvData[0] & 0x20) == 0x20 ? true : false;
                            container_1.FastBrakeCmd = (recvData[0] & 0x40) == 0x40 ? true : false;
                            container_1.EmergencyBrakeCmd = (recvData[0] & 0x80) == 0x80 ? true : false;
                            #endregion

                            #region byte1
                            container_1.KeepBrakeState = (recvData[1] & 0x01) == 0x01 ? true : false;
                            container_1.LazyState = (recvData[1] & 0x02) == 0x02 ? true : false;
                            container_1.DriveState = (recvData[1] & 0x04) == 0x04 ? true : false;
                            container_1.NormalBrakeState = (recvData[1] & 0x08) == 0x08 ? true : false;
                            container_1.EmergencyBrakeState = (recvData[1] & 0x10) == 0x10 ? true : false;
                            container_1.ZeroSpeedCan = (recvData[1] & 0x20) == 0x20 ? true : false;
                            container_1.ATOMode1 = (recvData[1] & 0x40) == 0x40 ? true : false;
                            container_1.ATOHold = (recvData[1] & 0X80) == 0X80 ? true : false;
                            #endregion

                            #region byte2
                            container_1.SelfTestInt = (recvData[2] & 0x01) == 0x01 ? true : false;
                            container_1.SelfTestActive = (recvData[2] & 0x02) == 0x02 ? true : false;
                            container_1.SelfTestSuccess = (recvData[2] & 0x04) == 0x04 ? true : false;
                            container_1.SelfTestFail = (recvData[2] & 0x08) == 0x08 ? true : false;
                            container_1.UnSelfTest24 = (recvData[2] & 0x10) == 0x10 ? true : false;
                            container_1.UnSelfTest26 = (recvData[2] & 0x20) == 0x20 ? true : false;
                            #endregion

                            #region byte3
                            container_1.BrakeLevelEnable = (recvData[3] & 0x01) == 0x01 ? true : false;
                            container_1.SelfTestCmd = (recvData[3] & 0x02) == 0x02 ? true : false;
                            container_1.EdFadeOut = (recvData[3] & 0x04) == 0x04 ? true : false;
                            container_1.TrainBrakeEnable = (recvData[3] & 0x08) == 0x08 ? true : false;
                            container_1.ZeroSpeed = (recvData[3] & 0x10) == 0x10 ? true : false;
                            container_1.EdOffB1 = (recvData[3] & 0x20) == 0x20 ? true : false;
                            container_1.EdOffC1 = (recvData[3] & 0x40) == 0x40 ? true : false;
                            container_1.WheelInputState = (recvData[3] & 0x80) == 0x80 ? true : false;
                            #endregion

                            #region byte4~5
                            container_1.BrakeLevel = recvData[4] * 256 + recvData[5];
                            #endregion

                            #region byte6~7
                            container_1.TrainBrakeForce = recvData[6] * 256 + recvData[7];
                            #endregion

                            #endregion
                            break;

                        case 1:
                            #region 主从通用数据包1
                            container_1.LifeSig = recvData[point];

                            container_1.SlipLvl1 = recvData[point + 1] & 0x0f;
                            container_1.SlipLvl2 = recvData[point + 1] & 0xf0;
                            container_1.SpeedA1Shaft1 = (recvData[point + 2] * 256 + recvData[point + 3]) / 10.0;
                            container_1.SpeedA1Shaft2 = (recvData[point + 4] * 256 + recvData[point + 5]) / 10.0;
                            //container_1.AccValue1 = recvData[point + 6] / 10.0;
                            //container_1.AccValue2 = recvData[point + 6] / 10.0;

                            //代表最高位为1
                            if ((recvData[point + 6] & 0x80) == 0x80)
                            {
                                int temp = (recvData[point + 6] & 0x7f);// 先将最高位置0
                                container_1.AccValue1 = -(temp/10.0);
                            }
                            else
                            {
                                container_1.AccValue1 = recvData[point + 6] / 10.0;
                            }

                            if ((recvData[point + 7] & 0x80) == 0x80)
                            {
                                int temp = (recvData[point + 7] & 0x7f);// 先将最高位置0
                                container_1.AccValue2 = -(temp / 10.0);
                            }
                            else
                            {
                                container_1.AccValue2 = recvData[point + 7] / 10.0;
                            }
                            #endregion

                            break;
                        case 2:
                            #region TPDO1(Checked)

                            container_1.AbTargetValueAx1 = recvData[point] * 256 + recvData[point + 1];
                            container_1.AbTargetValueAx2 = recvData[point + 2] * 256 + recvData[point + 3];
                            container_1.AbTargetValueAx3 = recvData[point + 4] * 256 + recvData[point + 5];
                            container_1.AbTargetValueAx4 = recvData[point + 6] * 256 + recvData[point + 7];
                            

                            
                            break;
                        #endregion

                        case 3:
                            #region TPDO2(Checked)
                            container_1.AbTargetValueAx5 = recvData[point] * 256 + recvData[point + 1];
                            container_1.AbTargetValueAx6 = recvData[point + 2] * 256 + recvData[point + 3];
                            container_1.HardDriveCmd = (recvData[5] & 0x01) == 0x01 ? true : false;
                            container_1.HardBrakeCmd = (recvData[5] & 0x02) == 0x02 ? true : false;
                            container_1.HardFastBrakeCmd = (recvData[5] & 0x04) == 0x04 ? true : false;
                            container_1.HardEmergencyBrake = (recvData[5] & 0x08) == 0x08 ? true : false;
                            container_1.HardEmergencyDriveCmd = (recvData[5] & 0x10) == 0x10 ? true : false;
                            container_1.CanUnitSelfTestOn = (recvData[5] & 0x20) == 0x20 ? true : false;
                            container_1.ValveCanEmergencyActive = (recvData[5] & 0x40) == 0x40 ? true : false;
                            container_1.CanUintSelfTestOver = (recvData[5] & 0x80) == 0x80 ? true : false;

                            container_1.NetDriveCmd = (recvData[4] & 0x01) == 0x01 ? true : false;
                            container_1.NetBrakeCmd = (recvData[4] & 0x02) == 0x02 ? true : false;
                            container_1.NetFastBrakeCmd = (recvData[4] & 0x04) == 0x04 ? true : false;
                            container_1.TowingMode = (recvData[4] & 0x08) == 0x08 ? true : false;
                            container_1.HoldBrakeRealease = (recvData[4] & 0x10) == 0x10 ? true : false;
                            container_1.CanUintSelfTestCmd_A = (recvData[4] & 0x20) == 0x20 ? true : false;
                            container_1.CanUintSelfTestCmd_B = (recvData[4] & 0x40) == 0x40 ? true : false;
                            container_1.ATOMode1 = (recvData[4] & 0x80) == 0x80 ? true : false;

                            container_1.RefSpeed = (recvData[6] * 256 + recvData[7]) / 10.0;
                            break;
                        #endregion

                        case 4:
                            #region TPDO4(Checked)
                            container_1.BrakeCylinderSourcePressure = Utils.PositiveToNegative(recvData[point], recvData[point + 1]);
                            container_1.AirSpring1PressureA1Car1 = recvData[point + 2] * 256 + recvData[point + 3];
                            container_1.AirSpring2PressureA1Car1 = recvData[point + 4] * 256 + recvData[point + 5];
                            container_1.ParkPressureA1 = recvData[point + 6] * 256 + recvData[point + 7];
                            break;
                        #endregion

                        case 5:
                            #region TPDO5(Checked)
                            container_1.VldRealPressureAx1 = recvData[point] * 256 + recvData[point + 1];
                            container_1.Bcp1PressureAx1 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);
                            container_1.Bcp2PressureAx2 = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]);

                            container_1.BSSRSenorFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_1.AirSpringSenorFault_1 = (recvData[6] & 0x02) == 0x20 ? true : false;
                            container_1.AirSpringSenorFault_2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_1.ParkCylinderSenorFault = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_1.VLDSensorFault = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_1.BSRSenorFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            container_1.BSRSenorFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_1.AirSpringOverflow_1 = (recvData[6] & 0x80) == 0x80 ? true : false;
                            container_1.AirSpringOverflow_2 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_1.BSRLowA11 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_1.ICANFault1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_1.ICANFault2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            break;
                        #endregion

                        case 6:
                            #region TPDO6(Checked)
                            container_1.VldPressureSetupAx1 = recvData[point] * 256 + recvData[point + 1];
                            container_1.MassA1 = recvData[point + 2] * 256 + recvData[point + 3];
                            container_1.BCUFail_Serious = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_1.BCUFail_Middle = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_1.BCUFail_Slight = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_1.OCANFault1 = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_1.OCANFault2 = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_1.SpeedSenorFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_1.SpeedSenorFault_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_1.WSPFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_1.WSPFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            container_1.CodeConnectorFault = (recvData[7] & 0x10) == 0x10 ? true : false;
                            container_1.AirSpringLimit = (recvData[7] & 0x20) == 0x20 ? true : false;
                            container_1.BrakeNotRealease = (recvData[7] & 0x40) == 0x40 ? true : false;
                            container_1.BCPLowA11 = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                        #endregion

                        case 7:
                            #region TPDO7(Checked)

                            container_1.SelfTestSetup = recvData[0] * 256 + recvData[1];

                            container_1.Ax1SelfTestActive = (recvData[2] & 0x01) == 0x01 ? true : false;
                            container_1.Ax1SelfTestOver = (recvData[2] & 0x02) == 0x02 ? true : false;
                            container_1.Ax1SelfTestSuccess = (recvData[2] & 0x04) == 0x04 ? true : false;
                            container_1.Ax1SelfTestFail = (recvData[2] & 0x08) == 0x08 ? true : false;
                            container_1.SlipRate1 = recvData[4];
                            container_1.SlipRate2 = recvData[5];

                            container_1.EPCutOff = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_1.AxisSlip1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_1.AxisSlip2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_1.WheelStoreFail = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_1.GateValveState = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_1.VCM_MVBConnectionState = (recvData[6] & 0x80) == 0x80 ? true : false;

                            container_1.SlipA1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_1.EmergencyBrakeActiveA1 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_1.NotZeroSpeed = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_1.AbActive = (recvData[7] & 0x08) == 0x08 ? true : false;
                            container_1.BCPLowA11 = (recvData[7] & 0x10) == 0x10 ? true : false;
                            container_1.ParkBreakRealease = (recvData[7] & 0x20) == 0x20 ? true : false;
                            container_1.AbStatuesA1 = (recvData[7] & 0x40) == 0x40 ? true : false;
                            container_1.AirSigValid = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                            #endregion
                    }
                    #endregion
                    break;

                case 3:
                    #region 从设备3车数据（4个数据包）
                    switch (canIdLow)
                    {
                        case 1:
                            #region 2节点数据包1(Checked)
                            container_2.LifeSig = recvData[point];
                            container_2.SlipLvl1 = recvData[point + 1] & 0x0f;
                            container_2.SlipLvl2 = recvData[point + 1] & 0xf0;

                            container_2.SpeedShaft1 = (recvData[point + 2] * 256 + recvData[point + 3]) / 10.0;
                            container_2.SpeedShaft2 = (recvData[point + 4] * 256 + recvData[point + 5]) / 10.0;

                            //container_2.AccValue1 = recvData[point + 6] / 10.0;
                            //container_2.AccValue2 = recvData[point + 6] / 10.0;

                            if ((recvData[point + 6] & 0x80) == 0x80)
                            {
                                int temp = (recvData[point + 6] & 0x7f);// 先将最高位置0
                                container_2.AccValue1 = -(temp / 10.0);
                            }
                            else
                            {
                                container_2.AccValue1 = recvData[point + 6] / 10.0;
                            }

                            if ((recvData[point + 7] & 0x80) == 0x80)
                            {
                                int temp = (recvData[point + 7] & 0x7f);// 先将最高位置0
                                container_2.AccValue2 = -(temp / 10.0);
                            }
                            else
                            {
                                container_2.AccValue2 = recvData[point + 7] / 10.0;
                            }
                            break;
                        #endregion

                        case 4:
                            #region 2节点数据包2(Checked)
                            container_2.BrakeCylinderSourcePressure = recvData[point] * 256 + recvData[point + 1];
                            container_2.AirSpringPressure1 = recvData[point + 2] * 256 + recvData[point + 3];
                            container_2.AirSpringPressure2 = recvData[point + 4] * 256 + recvData[point + 5];
                            container_2.ParkPressure = recvData[6] * 256 + recvData[7];

                            break;
                        #endregion

                        case 5:
                            #region 2节点数据包3(Checked)
                            container_2.VldRealPressure = recvData[point] * 256 + recvData[point + 1];
                            CheckZero(recvData, point + 2);
                            container_2.Bcp1Pressure = recvData[point + 2] * 256 + recvData[point + 3];
                            CheckZero(recvData, point + 4);
                            container_2.Bcp2Pressure = recvData[point + 4] * 256 + recvData[point + 5];

                            container_2.BSSRSenorFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_2.AirSpringSenorFault_1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_2.AirSpringSenorFault_2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_2.VLDSensorFault = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_2.BSRSenorFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            container_2.BSRSenorFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_2.AirSpringOverflow_1 = (recvData[6] & 0x80) == 0x80 ? true : false;
                            container_2.AirSpringOverflow_2 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_2.BSSRSuperLow = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_2.ICANFault1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_2.ICANFault2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            break;
                        #endregion

                        case 6:
                            #region 2节点数据包4
                            container_2.VldSetupPressure = recvData[point] * 256 + recvData[point + 1];
                            container_2.MassValue = recvData[point + 2] * 256 + recvData[point + 3];

                            container_2.BCUFail_Serious = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_2.BCUFail_Middle = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_2.BCUFail_Slight = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_2.EmergencyBrakeFault = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_2.OCANFault1 = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_2.OCANFault2 = (recvData[6] & 0x20) == 0x20 ? true : false;

                            container_2.SpeedSenorFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_2.SpeedSenorFault_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_2.WSPFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_2.WSPFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            container_2.CodeConnectorFault = (recvData[7] & 0x10) == 0x10 ? true : false;
                            container_2.AirSpringLimit = (recvData[7] & 0x20) == 0x20 ? true : false;
                            container_2.BrakeNotRealease = (recvData[7] & 0x40) == 0x40 ? true : false;
                            container_2.BCPLow1 = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                        #endregion

                        case 7:
                            #region 2节点数据包5
                            container_2.SelfTestSetup = recvData[0] * 256 + recvData[1];
                            container_2.SelfTestActive = (recvData[2] & 0x01) == 0x01 ? true : false;
                            container_2.SelfTestOver = (recvData[2] & 0x02) == 0x02 ? true : false;
                            container_2.SelfTestSuccess = (recvData[2] & 0x04) == 0x04 ? true : false;
                            container_2.SelfTestFail = (recvData[2] & 0x08) == 0x08 ? true : false;
                            container_2.SlipRate1 = recvData[4];
                            container_2.SlipRate2 = recvData[5];

                            container_2.EPCutOff = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_2.AxisSlip1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_2.AxisSlip2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_2.WheelStoreFail = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_2.GateValveState = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_2.VCM_MVBConnectionState = (recvData[6] & 0x80) == 0x80 ? true : false;

                            container_2.Slip = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_2.EmergencyBrakeActive = (recvData[7] & 0x02) == 0x20 ? true : false;
                            container_2.AbBrakeActive = (recvData[7] & 0x08) == 0x08 ? true : false;
                            container_2.BSRLow1 = (recvData[7] & 0x10) == 0x10 ? true : false;
                            container_2.ParkBrakeRealease = (recvData[7] & 0x20) == 0x20 ? true : false;
                            container_2.AbBrakeSatet = (recvData[7] & 0x40) == 0x40 ? true : false;
                            container_2.AirSigValid = (recvData[7] & 0x80) == 0x80 ? true : false;


                            break;
                            #endregion
                    }
                    #endregion
                    break;

                case 4:
                    #region 从设备3车数据（4个数据包）
                    switch (canIdLow)
                    {
                        case 1:
                            #region 2节点数据包1(Checked)
                            container_3.LifeSig = recvData[point];
                            container_3.SlipLvl1 = recvData[point + 1] & 0x0f;
                            container_3.SlipLvl2 = recvData[point + 1] & 0xf0;
                        
                            container_3.SpeedShaft1 = (recvData[point + 2] * 256 + recvData[point + 3]) / 10.0;
                            container_3.SpeedShaft2 = (recvData[point + 4] * 256 + recvData[point + 5]) / 10.0;

                            //container_3.AccValue1 = recvData[point + 6] / 10.0;
                            //container_3.AccValue2 = recvData[point + 6] / 10.0;

                            if ((recvData[point + 6] & 0x80) == 0x80)
                            {
                                int temp = (recvData[point + 6] & 0x7f);// 先将最高位置0
                                container_3.AccValue1 = -(temp / 10.0);
                            }
                            else
                            {
                                container_3.AccValue1 = recvData[point + 6] / 10.0;
                            }

                            if ((recvData[point + 7] & 0x80) == 0x80)
                            {
                                int temp = (recvData[point + 7] & 0x7f);// 先将最高位置0
                                container_3.AccValue2 = -(temp / 10.0);
                            }
                            else
                            {
                                container_3.AccValue2 = recvData[point + 7] / 10.0;
                            }
                            break;
                        #endregion

                        case 4:
                            #region 2节点数据包2(Checked)
                            container_3.BrakeCylinderSourcePressure = recvData[point] * 256 + recvData[point + 1];
                            container_3.AirSpringPressure1 = recvData[point + 2] * 256 + recvData[point + 3];
                            container_3.AirSpringPressure2 = recvData[point + 4] * 256 + recvData[point + 5];
                            container_3.ParkPressure = recvData[6] * 256 + recvData[7];

                            break;
                        #endregion

                        case 5:
                            #region 2节点数据包3(Checked)
                            container_3.VldRealPressure = recvData[point] * 256 + recvData[point + 1];
                            CheckZero(recvData, point + 2);
                            container_3.Bcp1Pressure = recvData[point + 2] * 256 + recvData[point + 3];
                            CheckZero(recvData, point + 4);
                            container_3.Bcp2Pressure = recvData[point + 4] * 256 + recvData[point + 5];

                            container_3.BSSRSenorFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_3.AirSpringSenorFault_1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_3.AirSpringSenorFault_2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_3.VLDSensorFault = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_3.BSRSenorFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            container_3.BSRSenorFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_3.AirSpringOverflow_1 = (recvData[6] & 0x80) == 0x80 ? true : false;
                            container_3.AirSpringOverflow_2 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_3.BSSRSuperLow = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_3.ICANFault1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_3.ICANFault2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            break;
                        #endregion

                        case 6:
                            #region 2节点数据包4
                            container_3.VldSetupPressure = recvData[point] * 256 + recvData[point + 1];
                            container_3.MassValue = recvData[point + 2] * 256 + recvData[point + 3];

                            container_3.BCUFail_Serious = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_3.BCUFail_Middle = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_3.BCUFail_Slight = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_3.EmergencyBrakeFault = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_3.OCANFault1 = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_3.OCANFault2 = (recvData[6] & 0x20) == 0x20 ? true : false;

                            container_3.SpeedSenorFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_3.SpeedSenorFault_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_3.WSPFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_3.WSPFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            container_3.CodeConnectorFault = (recvData[7] & 0x10) == 0x10 ? true : false;
                            container_3.AirSpringLimit = (recvData[7] & 0x20) == 0x20 ? true : false;
                            container_3.BrakeNotRealease = (recvData[7] & 0x40) == 0x40 ? true : false;
                            container_3.BCPLow1 = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                        #endregion

                        case 7:
                            #region 2节点数据包5
                            container_3.SelfTestSetup = recvData[0] * 256 + recvData[1];
                            container_3.SelfTestActive = (recvData[2] & 0x01) == 0x01 ? true : false;
                            container_3.SelfTestOver = (recvData[2] & 0x02) == 0x02 ? true : false;
                            container_3.SelfTestSuccess = (recvData[2] & 0x04) == 0x04 ? true : false;
                            container_3.SelfTestFail = (recvData[2] & 0x08) == 0x08 ? true : false;
                            container_3.SlipRate1 = recvData[4];
                            container_3.SlipRate2 = recvData[5];

                            container_3.EPCutOff = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_3.AxisSlip1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_3.AxisSlip2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_3.WheelStoreFail = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_3.GateValveState = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_3.VCM_MVBConnectionState = (recvData[6] & 0x80) == 0x80 ? true : false;

                            container_3.Slip = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_3.EmergencyBrakeActive = (recvData[7] & 0x02) == 0x20 ? true : false;
                            container_3.AbBrakeActive = (recvData[7] & 0x08) == 0x08 ? true : false;
                            container_3.BSRLow1 = (recvData[7] & 0x10) == 0x10 ? true : false;
                            container_3.ParkBrakeRealease = (recvData[7] & 0x20) == 0x20 ? true : false;
                            container_3.AbBrakeSatet = (recvData[7] & 0x40) == 0x40 ? true : false;
                            container_3.AirSigValid = (recvData[7] & 0x80) == 0x80 ? true : false;


                            break;
                            #endregion
                    }
                    #endregion
                    break;

                case 5:
                    #region 从设备4车数据（4个数据包）
                    switch (canIdLow)
                    {
                        case 1:
                            #region 2节点数据包1(Checked)
                            container_4.LifeSig = recvData[point];
                            container_4.SlipLvl1 = recvData[point + 1] & 0x0f;
                            container_4.SlipLvl2 = recvData[point + 1] & 0xf0;

                            container_4.SpeedShaft1 = (recvData[point + 2] * 256 + recvData[point + 3]) / 10.0;
                            container_4.SpeedShaft2 = (recvData[point + 4] * 256 + recvData[point + 5]) / 10.0;

                            //container_4.AccValue1 = recvData[point + 6] / 10.0;
                            //container_4.AccValue2 = recvData[point + 6] / 10.0;

                            if ((recvData[point + 6] & 0x80) == 0x80)
                            {
                                int temp = (recvData[point + 6] & 0x7f);// 先将最高位置0
                                container_4.AccValue1 = -(temp / 10.0);
                            }
                            else
                            {
                                container_4.AccValue1 = recvData[point + 6] / 10.0;
                            }

                            if ((recvData[point + 7] & 0x80) == 0x80)
                            {
                                int temp = (recvData[point + 7] & 0x7f);// 先将最高位置0
                                container_4.AccValue2 = -(temp / 10.0);
                            }
                            else
                            {
                                container_4.AccValue2 = recvData[point + 7] / 10.0;
                            }
                            break;
                        #endregion

                        case 4:
                            #region 2节点数据包2(Checked)
                            container_4.BrakeCylinderSourcePressure = recvData[point] * 256 + recvData[point + 1];
                            container_4.AirSpringPressure1 = recvData[point + 2] * 256 + recvData[point + 3];
                            container_4.AirSpringPressure2 = recvData[point + 4] * 256 + recvData[point + 5];
                            container_4.ParkPressure = recvData[6] * 256 + recvData[7];

                            break;
                        #endregion

                        case 5:
                            #region 2节点数据包3(Checked)
                            container_4.VldRealPressure = recvData[point] * 256 + recvData[point + 1];
                            CheckZero(recvData, point + 2);
                            container_4.Bcp1Pressure = recvData[point + 2] * 256 + recvData[point + 3];
                            CheckZero(recvData, point + 4);
                            container_4.Bcp2Pressure = recvData[point + 4] * 256 + recvData[point + 5];

                            container_4.BSSRSenorFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_4.AirSpringSenorFault_1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_4.AirSpringSenorFault_2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_4.VLDSensorFault = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_4.BSRSenorFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            container_4.BSRSenorFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_4.AirSpringOverflow_1 = (recvData[6] & 0x80) == 0x80 ? true : false;
                            container_4.AirSpringOverflow_2 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_4.BSSRSuperLow = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_4.ICANFault1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_4.ICANFault2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            break;
                        #endregion

                        case 6:
                            #region 2节点数据包4
                            container_4.VldSetupPressure = recvData[point] * 256 + recvData[point + 1];
                            container_4.MassValue = recvData[point + 2] * 256 + recvData[point + 3];

                            container_4.BCUFail_Serious = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_4.BCUFail_Middle = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_4.BCUFail_Slight = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_4.EmergencyBrakeFault = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_4.OCANFault1 = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_4.OCANFault2 = (recvData[6] & 0x20) == 0x20 ? true : false;

                            container_4.SpeedSenorFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_4.SpeedSenorFault_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_4.WSPFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_4.WSPFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            container_4.CodeConnectorFault = (recvData[7] & 0x10) == 0x10 ? true : false;
                            container_4.AirSpringLimit = (recvData[7] & 0x20) == 0x20 ? true : false;
                            container_4.BrakeNotRealease = (recvData[7] & 0x40) == 0x40 ? true : false;
                            container_4.BCPLow1 = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                        #endregion

                        case 7:
                            #region 2节点数据包5
                            container_4.SelfTestSetup = recvData[0] * 256 + recvData[1];
                            container_4.SelfTestActive = (recvData[2] & 0x01) == 0x01 ? true : false;
                            container_4.SelfTestOver = (recvData[2] & 0x02) == 0x02 ? true : false;
                            container_4.SelfTestSuccess = (recvData[2] & 0x04) == 0x04 ? true : false;
                            container_4.SelfTestFail = (recvData[2] & 0x08) == 0x08 ? true : false;
                            container_4.SlipRate1 = recvData[4];
                            container_4.SlipRate2 = recvData[5];

                            container_4.EPCutOff = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_4.AxisSlip1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_4.AxisSlip2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_4.WheelStoreFail = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_4.GateValveState = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_4.VCM_MVBConnectionState = (recvData[6] & 0x80) == 0x80 ? true : false;

                            container_4.Slip = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_4.EmergencyBrakeActive = (recvData[7] & 0x02) == 0x20 ? true : false;
                            container_4.AbBrakeActive = (recvData[7] & 0x08) == 0x08 ? true : false;
                            container_4.BSRLow1 = (recvData[7] & 0x10) == 0x10 ? true : false;
                            container_4.ParkBrakeRealease = (recvData[7] & 0x20) == 0x20 ? true : false;
                            container_4.AbBrakeSatet = (recvData[7] & 0x40) == 0x40 ? true : false;
                            container_4.AirSigValid = (recvData[7] & 0x80) == 0x80 ? true : false;


                            break;
                            #endregion
                    }
                    #endregion
                    break;

                case 6:
                    #region 从设备5车数据（4个数据包）
                    switch (canIdLow)
                    {
                        case 1:
                            #region 2节点数据包1(Checked)
                            container_5.LifeSig = recvData[point];
                            container_5.SlipLvl1 = recvData[point + 1] & 0x0f;
                            container_5.SlipLvl2 = recvData[point + 1] & 0xf0;

                            container_5.SpeedShaft1 = (recvData[point + 2] * 256 + recvData[point + 3]) / 10.0;
                            container_5.SpeedShaft2 = (recvData[point + 4] * 256 + recvData[point + 5]) / 10.0;

                            //container_5.AccValue1 = recvData[point + 6] / 10.0;
                            //container_5.AccValue2 = recvData[point + 6] / 10.0;

                            if ((recvData[point + 6] & 0x80) == 0x80)
                            {
                                int temp = (recvData[point + 6] & 0x7f);// 先将最高位置0
                                container_5.AccValue1 = -(temp / 10.0);
                            }
                            else
                            {
                                container_5.AccValue1 = recvData[point + 6] / 10.0;
                            }

                            if ((recvData[point + 7] & 0x80) == 0x80)
                            {
                                int temp = (recvData[point + 7] & 0x7f);// 先将最高位置0
                                container_5.AccValue2 = -(temp / 10.0);
                            }
                            else
                            {
                                container_5.AccValue2 = recvData[point + 7] / 10.0;
                            }
                            break;
                        #endregion

                        case 4:
                            #region 2节点数据包2(Checked)
                            container_5.BrakeCylinderSourcePressure = recvData[point] * 256 + recvData[point + 1];
                            container_5.AirSpringPressure1 = recvData[point + 2] * 256 + recvData[point + 3];
                            container_5.AirSpringPressure2 = recvData[point + 4] * 256 + recvData[point + 5];
                            container_5.ParkPressure = recvData[6] * 256 + recvData[7];

                            break;
                        #endregion

                        case 5:
                            #region 2节点数据包3(Checked)
                            container_5.VldRealPressure = recvData[point] * 256 + recvData[point + 1];
                            CheckZero(recvData, point + 2);
                            container_5.Bcp1Pressure = recvData[point + 2] * 256 + recvData[point + 3];
                            CheckZero(recvData, point + 4);
                            container_5.Bcp2Pressure = recvData[point + 4] * 256 + recvData[point + 5];

                            container_5.BSSRSenorFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_5.AirSpringSenorFault_1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_5.AirSpringSenorFault_2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_5.VLDSensorFault = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_5.BSRSenorFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            container_5.BSRSenorFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_5.AirSpringOverflow_1 = (recvData[6] & 0x80) == 0x80 ? true : false;
                            container_5.AirSpringOverflow_2 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_5.BSSRSuperLow = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_5.ICANFault1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_5.ICANFault2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            break;
                        #endregion

                        case 6:
                            #region 2节点数据包4
                            container_5.VldSetupPressure = recvData[point] * 256 + recvData[point + 1];
                            container_5.MassValue = recvData[point + 2] * 256 + recvData[point + 3];

                            container_5.BCUFail_Serious = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_5.BCUFail_Middle = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_5.BCUFail_Slight = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_5.EmergencyBrakeFault = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_5.OCANFault1 = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_5.OCANFault2 = (recvData[6] & 0x20) == 0x20 ? true : false;

                            container_5.SpeedSenorFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_5.SpeedSenorFault_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_5.WSPFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_5.WSPFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            container_5.CodeConnectorFault = (recvData[7] & 0x10) == 0x10 ? true : false;
                            container_5.AirSpringLimit = (recvData[7] & 0x20) == 0x20 ? true : false;
                            container_5.BrakeNotRealease = (recvData[7] & 0x40) == 0x40 ? true : false;
                            container_5.BCPLow1 = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                        #endregion

                        case 7:
                            #region 2节点数据包5
                            container_5.SelfTestSetup = recvData[0] * 256 + recvData[1];
                            container_5.SelfTestActive = (recvData[2] & 0x01) == 0x01 ? true : false;
                            container_5.SelfTestOver = (recvData[2] & 0x02) == 0x02 ? true : false;
                            container_5.SelfTestSuccess = (recvData[2] & 0x04) == 0x04 ? true : false;
                            container_5.SelfTestFail = (recvData[2] & 0x08) == 0x08 ? true : false;
                            container_5.SlipRate1 = recvData[4];
                            container_5.SlipRate2 = recvData[5];

                            container_5.EPCutOff = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_5.AxisSlip1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_5.AxisSlip2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_5.WheelStoreFail = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_5.GateValveState = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_5.VCM_MVBConnectionState = (recvData[6] & 0x80) == 0x80 ? true : false;

                            container_5.Slip = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_5.EmergencyBrakeActive = (recvData[7] & 0x02) == 0x20 ? true : false;
                            container_5.AbBrakeActive = (recvData[7] & 0x08) == 0x08 ? true : false;
                            container_5.BSRLow1 = (recvData[7] & 0x10) == 0x10 ? true : false;
                            container_5.ParkBrakeRealease = (recvData[7] & 0x20) == 0x20 ? true : false;
                            container_5.AbBrakeSatet = (recvData[7] & 0x40) == 0x40 ? true : false;
                            container_5.AirSigValid = (recvData[7] & 0x80) == 0x80 ? true : false;


                            break;
                            #endregion
                    }
                    #endregion
                    break;

                case 2:
                    #region 主设备A1车CAN消息（6个数据包）
                    switch (canIdLow)
                    {
                        case 0:
                            #region TPDO0(Checked)

                            #region byte0
                            if ((recvData[0] & 0x01) == 0x01)
                            {
                                container_6.Mode = MainDevDataContains.NORMAL_MODE;
                            }
                            else if ((recvData[0] & 0x02) == 0x02)
                            {
                                container_6.Mode = MainDevDataContains.EMERGENCY_DRIVE_MODE;
                            }
                            else if ((recvData[0] & 0x04) == 0x04)
                            {
                                container_6.Mode = MainDevDataContains.CALLBACK_MODE;
                            }
                            container_6.BrakeCmd = (recvData[0] & 0x08) == 0x08 ? true : false;
                            container_6.DriveCmd = (recvData[0] & 0x10) == 0x10 ? true : false;
                            container_6.LazyCmd = (recvData[0] & 0x20) == 0x20 ? true : false;
                            container_6.FastBrakeCmd = (recvData[0] & 0x40) == 0x40 ? true : false;
                            container_6.EmergencyBrakeCmd = (recvData[0] & 0x80) == 0x80 ? true : false;
                            #endregion

                            #region byte1
                            container_6.KeepBrakeState = (recvData[1] & 0x01) == 0x01 ? true : false;
                            container_6.LazyState = (recvData[1] & 0x02) == 0x02 ? true : false;
                            container_6.DriveState = (recvData[1] & 0x04) == 0x04 ? true : false;
                            container_6.NormalBrakeState = (recvData[1] & 0x08) == 0x08 ? true : false;
                            container_6.EmergencyBrakeState = (recvData[1] & 0x10) == 0x10 ? true : false;
                            container_6.ZeroSpeedCan = (recvData[1] & 0x20) == 0x20 ? true : false;
                            container_6.ATOMode1 = (recvData[1] & 0x40) == 0x40 ? true : false;
                            container_6.ATOHold = (recvData[1] & 0X80) == 0X80 ? true : false;
                            #endregion

                            #region byte2
                            container_6.SelfTestInt = (recvData[2] & 0x01) == 0x01 ? true : false;
                            container_6.SelfTestActive = (recvData[2] & 0x02) == 0x02 ? true : false;
                            container_6.SelfTestSuccess = (recvData[2] & 0x04) == 0x04 ? true : false;
                            container_6.SelfTestFail = (recvData[2] & 0x08) == 0x08 ? true : false;
                            container_6.UnSelfTest24 = (recvData[2] & 0x10) == 0x10 ? true : false;
                            container_6.UnSelfTest26 = (recvData[2] & 0x20) == 0x20 ? true : false;
                            #endregion

                            #region byte3
                            container_6.BrakeLevelEnable = (recvData[3] & 0x01) == 0x01 ? true : false;
                            container_6.SelfTestCmd = (recvData[3] & 0x02) == 0x02 ? true : false;
                            container_6.EdFadeOut = (recvData[3] & 0x04) == 0x04 ? true : false;
                            container_6.TrainBrakeEnable = (recvData[3] & 0x08) == 0x08 ? true : false;
                            container_6.ZeroSpeed = (recvData[3] & 0x10) == 0x10 ? true : false;
                            container_6.EdOffB1 = (recvData[3] & 0x20) == 0x20 ? true : false;
                            container_6.EdOffC1 = (recvData[3] & 0x40) == 0x40 ? true : false;
                            container_6.WheelInputState = (recvData[3] & 0x80) == 0x80 ? true : false;
                            #endregion

                            #region byte4~5
                            container_6.BrakeLevel = recvData[4] * 256 + recvData[5];
                            #endregion

                            #region byte6~7
                            container_6.TrainBrakeForce = recvData[6] * 256 + recvData[7];
                            #endregion

                            #endregion
                            break;

                        case 1:
                            #region 主从通用数据包1
                            container_6.LifeSig = recvData[point];

                            container_6.SlipLvl1 = recvData[point + 1] & 0x0f;
                            container_6.SlipLvl2 = recvData[point + 1] & 0xf0;
                            container_6.SpeedA1Shaft1 = (recvData[point + 2] * 256 + recvData[point + 3]) / 10.0;
                            container_6.SpeedA1Shaft2 = (recvData[point + 4] * 256 + recvData[point + 5]) / 10.0;

                            //container_6.AccValue1 = recvData[point + 6] / 10.0;
                            //container_6.AccValue2 = recvData[point + 6] / 10.0;

                            if ((recvData[point + 6] & 0x80) == 0x80)
                            {
                                int temp = (recvData[point + 6] & 0x7f);// 先将最高位置0
                                container_6.AccValue1 = -(temp / 10.0);
                            }
                            else
                            {
                                container_6.AccValue1 = recvData[point + 6] / 10.0;
                            }

                            if ((recvData[point + 7] & 0x80) == 0x80)
                            {
                                int temp = (recvData[point + 7] & 0x7f);// 先将最高位置0
                                container_6.AccValue2 = -(temp / 10.0);
                            }
                            else
                            {
                                container_6.AccValue2 = recvData[point + 7] / 10.0;
                            }
                            #endregion

                            break;
                        case 2:
                            #region TPDO1(Checked)

                            container_6.AbTargetValueAx1 = recvData[point] * 256 + recvData[point + 1];
                            container_6.AbTargetValueAx2 = recvData[point + 2] * 256 + recvData[point + 3];
                            container_6.AbTargetValueAx3 = recvData[point + 4] * 256 + recvData[point + 5];
                            container_6.AbTargetValueAx4 = recvData[point + 6] * 256 + recvData[point + 7];



                            break;
                        #endregion

                        case 3:
                            #region TPDO2(Checked)
                            container_6.AbTargetValueAx5 = recvData[point] * 256 + recvData[point + 1];
                            container_6.AbTargetValueAx6 = recvData[point + 2] * 256 + recvData[point + 3];
                            container_6.HardDriveCmd = (recvData[5] & 0x01) == 0x01 ? true : false;
                            container_6.HardBrakeCmd = (recvData[5] & 0x02) == 0x02 ? true : false;
                            container_6.HardFastBrakeCmd = (recvData[5] & 0x04) == 0x04 ? true : false;
                            container_6.HardEmergencyBrake = (recvData[5] & 0x08) == 0x08 ? true : false;
                            container_6.HardEmergencyDriveCmd = (recvData[5] & 0x10) == 0x10 ? true : false;
                            container_6.CanUnitSelfTestOn = (recvData[5] & 0x20) == 0x20 ? true : false;
                            container_6.ValveCanEmergencyActive = (recvData[5] & 0x40) == 0x40 ? true : false;
                            container_6.CanUintSelfTestOver = (recvData[5] & 0x80) == 0x80 ? true : false;

                            container_6.NetDriveCmd = (recvData[4] & 0x01) == 0x01 ? true : false;
                            container_6.NetBrakeCmd = (recvData[4] & 0x02) == 0x02 ? true : false;
                            container_6.NetFastBrakeCmd = (recvData[4] & 0x40) == 0x40 ? true : false;
                            container_6.TowingMode = (recvData[4] & 0x08) == 0x08 ? true : false;
                            container_6.HoldBrakeRealease = (recvData[4] & 0x10) == 0x10 ? true : false;
                            container_6.CanUintSelfTestCmd_A = (recvData[4] & 0x20) == 0x20 ? true : false;
                            container_6.CanUintSelfTestCmd_B = (recvData[4] & 0x40) == 0x40 ? true : false;
                            container_6.ATOMode1 = (recvData[4] & 0x80) == 0x80 ? true : false;

                            container_6.RefSpeed = (recvData[6] * 256 + recvData[7]) / 10.0;
                            break;
                        #endregion

                        case 4:
                            #region TPDO4(Checked)
                            container_6.BrakeCylinderSourcePressure = recvData[point] * 256 + recvData[point + 1];
                            container_6.AirSpring1PressureA1Car1 = recvData[point + 2] * 256 + recvData[point + 3];
                            container_6.AirSpring2PressureA1Car1 = recvData[point + 4] * 256 + recvData[point + 5];
                            container_6.ParkPressureA1 = recvData[point + 6] * 256 + recvData[point + 7];
                            break;
                        #endregion

                        case 5:
                            #region TPDO5(Checked)
                            container_6.VldRealPressureAx1 = recvData[point] * 256 + recvData[point + 1];
                            container_6.Bcp1PressureAx1 = Utils.PositiveToNegative(recvData[point + 2], recvData[point + 3]);
                            container_6.Bcp2PressureAx2 = Utils.PositiveToNegative(recvData[point + 4], recvData[point + 5]);

                            container_6.BSSRSenorFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_6.AirSpringSenorFault_1 = (recvData[6] & 0x02) == 0x20 ? true : false;
                            container_6.AirSpringSenorFault_2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_6.ParkCylinderSenorFault = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_6.VLDSensorFault = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_6.BSRSenorFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            container_6.BSRSenorFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_6.AirSpringOverflow_1 = (recvData[6] & 0x80) == 0x80 ? true : false;
                            container_6.AirSpringOverflow_2 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_6.BSRLowA11 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_6.ICANFault1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_6.ICANFault2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            break;
                        #endregion

                        case 6:
                            #region TPDO6(Checked)
                            container_6.VldPressureSetupAx1 = recvData[point] * 256 + recvData[point + 1];
                            container_6.MassA1 = recvData[point + 2] * 256 + recvData[point + 3];
                            container_6.BCUFail_Serious = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_6.BCUFail_Middle = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_6.BCUFail_Slight = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_6.OCANFault1 = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_6.OCANFault2 = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_6.SpeedSenorFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_6.SpeedSenorFault_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_6.WSPFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_6.WSPFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            container_6.CodeConnectorFault = (recvData[7] & 0x10) == 0x10 ? true : false;
                            container_6.AirSpringLimit = (recvData[7] & 0x20) == 0x20 ? true : false;
                            container_6.BrakeNotRealease = (recvData[7] & 0x40) == 0x40 ? true : false;
                            container_6.BCPLowA11 = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                        #endregion

                        case 7:
                            #region TPDO7(Checked)

                            container_6.SelfTestSetup = recvData[0] * 256 + recvData[1];

                            container_6.Ax1SelfTestActive = (recvData[2] & 0x01) == 0x01 ? true : false;
                            container_6.Ax1SelfTestOver = (recvData[2] & 0x02) == 0x02 ? true : false;
                            container_6.Ax1SelfTestSuccess = (recvData[2] & 0x04) == 0x04 ? true : false;
                            container_6.Ax1SelfTestFail = (recvData[2] & 0x08) == 0x08 ? true : false;
                            container_6.SlipRate1 = recvData[4];
                            container_6.SlipRate2 = recvData[5];

                            container_6.EPCutOff = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_6.AxisSlip1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_6.AxisSlip2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_6.WheelStoreFail = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_6.GateValveState = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_6.VCM_MVBConnectionState = (recvData[6] & 0x80) == 0x80 ? true : false;

                            container_6.SlipA1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_6.EmergencyBrakeActiveA1 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_6.NotZeroSpeed = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_6.AbActive = (recvData[7] & 0x08) == 0x08 ? true : false;
                            container_6.BCPLowA11 = (recvData[7] & 0x10) == 0x10 ? true : false;
                            container_6.ParkBreakRealease = (recvData[7] & 0x20) == 0x20 ? true : false;
                            container_6.AbStatuesA1 = (recvData[7] & 0x40) == 0x40 ? true : false;
                            container_6.AirSigValid = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                            #endregion
                    }
                    #endregion
                    break;

                case 7:
                    #region 1车附加数据
                    switch (canIdLow)
                    {
                        case 1:
                            #region 1车附加1数据(Checked)
                            container_1.VCMLifeSig = recvData[1];
                            container_1.DcuLifeSig[0] = recvData[2];
                            container_1.DcuLifeSig[1] = recvData[3];

                            container_1.DcuEbOK[0] = (recvData[4] & 0x01) == 0x01 ? true : false;
                            container_1.DcuEbFadeout[0] = (recvData[4] & 0x02) == 0x02 ? true : false;
                            container_1.DcuEbSlip[0] = (recvData[4] & 0x04) == 0x04 ? true : false;
                            container_1.DcuEbOK[1] = (recvData[4] & 0x10) == 0x10 ? true : false;
                            container_1.DcuEbFadeout[1] = (recvData[4] & 0x20) == 0x20 ? true : false;
                            container_1.DcuEbSlip[1] = (recvData[4] & 0x40) == 0x40 ? true : false;
                            container_1.DcuEbOK[2] = (recvData[5] & 0x01) == 0x01 ? true : false;
                            container_1.DcuEbFadeout[2] = (recvData[5] & 0x02) == 0x02 ? true : false;
                            container_1.DcuEbSlip[2] = (recvData[5] & 0x04) == 0x04 ? true : false;
                            container_1.DcuEbOK[3] = (recvData[5] & 0x10) == 0x10 ? true : false;
                            container_1.DcuEbFadeout[3] = (recvData[5] & 0x20) == 0x20 ? true : false;
                            container_1.DcuEbSlip[3] = (recvData[5] & 0x40) == 0x40 ? true : false;

                            container_1.DcuLifeSig[2] = recvData[6];
                            container_1.DcuLifeSig[3] = recvData[7];
                            #endregion
                            break;
                        case 2:
                            #region 1车附加2数据(Checked)
                            container_1.DcuEbRealValue[0] = recvData[0] * 256 + recvData[1];
                            container_1.DcuMax[0] = recvData[2] * 256 + recvData[3];
                            container_1.DcuEbRealValue[1] = recvData[4] * 256 + recvData[5];
                            container_1.DcuMax[1] = recvData[6] * 256 + recvData[7];
                            #endregion
                            break;
                        case 3:
                            #region 1车附加3数据(Checked)
                            container_1.DcuEbRealValue[2] = recvData[0] * 256 + recvData[1];
                            container_1.DcuMax[2] = recvData[2] * 256 + recvData[3];
                            container_1.DcuEbRealValue[3] = recvData[4] * 256 + recvData[5];
                            container_1.DcuMax[3] = recvData[6] * 256 + recvData[7];
                            #endregion
                            break;
                        case 4:
                            #region 1车附加4数据(Checked)
                            container_1.AbCapacity[0] = recvData[0] * 256 + recvData[1];
                            container_1.AbCapacity[1] = recvData[2] * 256 + recvData[3];
                            container_1.AbCapacity[2] = recvData[4] * 256 + recvData[5];
                            container_1.AbCapacity[3] = recvData[6] * 256 + recvData[7];
                            #endregion
                            break;
                        case 5:
                            #region 1车附加5数据
                            container_1.AbCapacity[4] = Utils.PositiveToNegative(recvData[0], recvData[1]);
                            container_1.AbCapacity[5] = Utils.PositiveToNegative(recvData[2], recvData[3]);
                            container_1.AbRealValue[0] = Utils.PositiveToNegative(recvData[4], recvData[5]);
                            container_1.AbRealValue[1] = Utils.PositiveToNegative(recvData[6], recvData[7]);
                            #endregion
                            break;
                        case 6:
                            #region 1车附加6数据(Checked)
                            container_1.AbRealValue[2] = recvData[0] * 256 + recvData[1];
                            container_1.AbRealValue[3] = recvData[2] * 256 + recvData[3];
                            container_1.AbRealValue[4] = recvData[4] * 256 + recvData[5];
                            container_1.AbRealValue[5] = recvData[6] * 256 + recvData[7];
                            #endregion
                            break;

                        case 7:
                            #region 1车附加7数据
                            container_1.DcuVolta[0] = recvData[0] * 256 + recvData[1];
                            container_1.DcuVolta[1] = recvData[2] * 256 + recvData[3];
                            container_1.DcuVolta[2] = recvData[4] * 256 + recvData[5];
                            container_1.DcuVolta[3] = recvData[6] * 256 + recvData[7];
                            #endregion
                            break;

                        case 8:
                            #region 1车附加7数据(Checked)
                            container_1.SpeedDetection = (recvData[0] & 0x01) == 0x01 ? true : false;
                            container_1.CanBusFail1 = (recvData[0] & 0x02) == 0x02 ? true : false;
                            container_1.CanBusFail2 = (recvData[0] & 0x04) == 0x04 ? true : false;
                            container_1.HardDifferent = (recvData[0] & 0x08) == 0x08 ? true : false;
                            container_1.EventHigh = (recvData[0] & 0x10) == 0x10 ? true : false;
                            container_1.EventMid = (recvData[0] & 0x20) == 0x20 ? true : false;
                            container_1.EventLow = (recvData[0] & 0x40) == 0x40 ? true : false;
                            container_1.CanASPEnable = (recvData[0] & 0x80) == 0x80 ? true : false;

                            container_1.BCPLowA = (recvData[1] & 0x01) == 0x01 ? true : false;
                            container_1.BCPLowB = (recvData[1] & 0x02) == 0x02 ? true : false;
                            container_1.BCPLowC = (recvData[1] & 0x04) == 0x04 ? true : false;

                            container_1.UnixHour = recvData[2] * 256 + recvData[3];
                            container_1.UnixMinute = recvData[4] * 256 + recvData[5];
                            container_1.UnixTimeValid = (recvData[6] & 0x20) == 0x20 ? true : false;
                            
                            #endregion
                            break;
                        case 9:
                            #region 1车附加9数据(Checked)
                            container_1.Tc1 = recvData[0] * 256 + recvData[1];
                            container_1.Mp1 = recvData[2] * 256 + recvData[3];
                            container_1.M1 = recvData[4] * 256 + recvData[5];
                            container_1.Tc1Valid = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_1.Mp1Valid = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_1.M1Valid = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_1.CanWheelInputCondition = (recvData[6] & 0x08) == 0x08 ? true : false;
                            #endregion
                            break;
                        default:
                            break;
                    }
                    #endregion
                    break;

                case 8:
                    #region 2车附加数据
                    switch (canIdLow)
                    {
                        case 1:
                            #region 1车附加1数据(Checked)
                            container_6.VCMLifeSig = recvData[1];
                            container_6.DcuLifeSig[0] = recvData[2];
                            container_6.DcuLifeSig[1] = recvData[3];

                            container_6.DcuEbOK[0] = (recvData[4] & 0x01) == 0x01 ? true : false;
                            container_6.DcuEbFadeout[0] = (recvData[4] & 0x02) == 0x02 ? true : false;
                            container_6.DcuEbSlip[0] = (recvData[4] & 0x04) == 0x04 ? true : false;
                            container_6.DcuEbOK[1] = (recvData[4] & 0x10) == 0x10 ? true : false;
                            container_6.DcuEbFadeout[1] = (recvData[4] & 0x20) == 0x20 ? true : false;
                            container_6.DcuEbSlip[1] = (recvData[4] & 0x40) == 0x40 ? true : false;
                            container_6.DcuEbOK[2] = (recvData[5] & 0x01) == 0x01 ? true : false;
                            container_6.DcuEbFadeout[2] = (recvData[5] & 0x02) == 0x02 ? true : false;
                            container_6.DcuEbSlip[2] = (recvData[5] & 0x04) == 0x04 ? true : false;
                            container_6.DcuEbOK[3] = (recvData[5] & 0x10) == 0x10 ? true : false;
                            container_6.DcuEbFadeout[3] = (recvData[5] & 0x20) == 0x20 ? true : false;
                            container_6.DcuEbSlip[3] = (recvData[5] & 0x40) == 0x40 ? true : false;

                            container_6.DcuLifeSig[2] = recvData[6];
                            container_6.DcuLifeSig[3] = recvData[7];
                            #endregion
                            break;
                        case 2:
                            #region 1车附加2数据(Checked)
                            container_6.DcuEbRealValue[0] = recvData[0] * 256 + recvData[1];
                            container_6.DcuMax[0] = recvData[2] * 256 + recvData[3];
                            container_6.DcuEbRealValue[1] = recvData[4] * 256 + recvData[5];
                            container_6.DcuMax[1] = recvData[6] * 256 + recvData[7];
                            #endregion
                            break;
                        case 3:
                            #region 1车附加3数据(Checked)
                            container_6.DcuEbRealValue[2] = recvData[0] * 256 + recvData[1];
                            container_6.DcuMax[2] = recvData[2] * 256 + recvData[3];
                            container_6.DcuEbRealValue[3] = recvData[4] * 256 + recvData[5];
                            container_6.DcuMax[3] = recvData[6] * 256 + recvData[7];
                            #endregion
                            break;
                        case 4:
                            #region 1车附加4数据(Checked)
                            container_6.AbCapacity[0] = recvData[0] * 256 + recvData[1];
                            container_6.AbCapacity[1] = recvData[2] * 256 + recvData[3];
                            container_6.AbCapacity[2] = recvData[4] * 256 + recvData[5];
                            container_6.AbCapacity[3] = recvData[6] * 256 + recvData[7];
                            #endregion
                            break;
                        case 5:
                            #region 1车附加5数据
                            container_6.AbCapacity[4] = Utils.PositiveToNegative(recvData[0], recvData[1]);
                            container_6.AbCapacity[5] = Utils.PositiveToNegative(recvData[2], recvData[3]);
                            container_6.AbRealValue[0] = Utils.PositiveToNegative(recvData[4], recvData[5]);
                            container_6.AbRealValue[1] = Utils.PositiveToNegative(recvData[6], recvData[7]);
                            #endregion
                            break;
                        case 6:
                            #region 1车附加6数据(Checked)
                            container_6.AbRealValue[2] = recvData[0] * 256 + recvData[1];
                            container_6.AbRealValue[3] = recvData[2] * 256 + recvData[3];
                            container_6.AbRealValue[4] = recvData[4] * 256 + recvData[5];
                            container_6.AbRealValue[5] = recvData[6] * 256 + recvData[7];
                            #endregion
                            break;

                        case 7:
                            #region 1车附加7数据
                            container_6.DcuVolta[0] = recvData[0] * 256 + recvData[1];
                            container_6.DcuVolta[1] = recvData[2] * 256 + recvData[3];
                            container_6.DcuVolta[2] = recvData[4] * 256 + recvData[5];
                            container_6.DcuVolta[3] = recvData[6] * 256 + recvData[7];
                            #endregion
                            break;

                        case 8:
                            #region 1车附加7数据(Checked)
                            container_6.SpeedDetection = (recvData[0] & 0x01) == 0x01 ? true : false;
                            container_6.CanBusFail1 = (recvData[0] & 0x02) == 0x02 ? true : false;
                            container_6.CanBusFail2 = (recvData[0] & 0x04) == 0x04 ? true : false;
                            container_6.HardDifferent = (recvData[0] & 0x08) == 0x08 ? true : false;
                            container_6.EventHigh = (recvData[0] & 0x10) == 0x10 ? true : false;
                            container_6.EventMid = (recvData[0] & 0x20) == 0x20 ? true : false;
                            container_6.EventLow = (recvData[0] & 0x40) == 0x40 ? true : false;
                            container_6.CanASPEnable = (recvData[0] & 0x80) == 0x80 ? true : false;

                            container_6.BCPLowA = (recvData[1] & 0x01) == 0x01 ? true : false;
                            container_6.BCPLowB = (recvData[1] & 0x02) == 0x02 ? true : false;
                            container_6.BCPLowC = (recvData[1] & 0x04) == 0x04 ? true : false;

                            container_6.UnixHour = recvData[2] * 256 + recvData[3];
                            container_6.UnixMinute = recvData[4] * 256 + recvData[5];
                            container_6.UnixTimeValid = (recvData[6] & 0x20) == 0x20 ? true : false;

                            #endregion
                            break;
                        case 9:
                            #region 1车附加9数据(Checked)
                            container_6.Tc2 = recvData[0] * 256 + recvData[1];
                            container_6.Mp2 = recvData[2] * 256 + recvData[3];
                            container_6.M2 = recvData[4] * 256 + recvData[5];
                            container_6.Tc2Valid = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_6.Mp2Valid = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_6.M2Valid = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_6.CanWheelInputCondition = (recvData[6] & 0x08) == 0x08 ? true : false;
                            #endregion
                            break;
                        default:
                            break;
                    }
                    #endregion
                    break;

                case 0x0a:
                    switch (canIdLow)
                    {
                        case 1:
                            container_1.Tc1Stander = recvData[0] * 256 + recvData[1];
                            container_1.ConfirmDownload = recvData[4] == 0xAA;
                            container_1.CPUAddr = recvData[5];
                            container_1.SoftwareVersionCPU = recvData[7];
                            container_1.SoftwareVersionEP = recvData[6];
                            break;
                        case 4:
                            container_3.CPUAddr = recvData[5];
                            container_3.SoftwareVersionCPU = recvData[7];
                            container_3.SoftwareVersionEP = recvData[6];
                            break;
                        case 5:
                            container_4.CPUAddr = recvData[5];
                            container_4.SoftwareVersionCPU = recvData[7];
                            container_4.SoftwareVersionEP = recvData[6];
                            break;
                        case 6:
                            container_5.CPUAddr = recvData[5];
                            container_5.SoftwareVersionCPU = recvData[7];
                            container_5.SoftwareVersionEP = recvData[6];
                            break;
                        case 2:
                            container_6.CPUAddr = recvData[5];
                            container_6.SoftwareVersionCPU = recvData[7];
                            container_6.SoftwareVersionEP = recvData[6];
                            break;
                        case 3:
                            container_2.Tc1Stander = recvData[0] * 256 + recvData[1];
                            container_2.ConfirmDownload = recvData[4] == 0xAA;
                            container_2.CPUAddr = recvData[5];
                            container_2.SoftwareVersionCPU = recvData[7];
                            container_2.SoftwareVersionEP = recvData[6];
                            break;
                        default:
                            break;
                    }
                    break;

            }
            #endregion
        }

        private void FormateHistory(byte[] recvData, uint canIdHigh, uint canIdLow, FormatType type, FormatCommand command, MainDevDataContains data_1, SliverDataContainer data_2, SliverDataContainer data_3, 
            SliverDataContainer data_4, SliverDataContainer data_5, MainDevDataContains data_6, int point = 0)
        {
            #region 解析CAN数据包中的8个字节，根据CAN ID来决定字段含义
            switch (canIdHigh)
            {
                case 1:
                    #region 主设备A1车CAN消息（6个数据包）
                    switch (canIdLow)
                    {
                        case 0:
                            #region TPDO0(Checked)

                            #region byte0
                            if ((recvData[0] & 0x01) == 0x01)
                            {
                                container_1.Mode = MainDevDataContains.NORMAL_MODE;
                            }
                            else if ((recvData[0] & 0x02) == 0x02)
                            {
                                container_1.Mode = MainDevDataContains.EMERGENCY_DRIVE_MODE;
                            }
                            else if ((recvData[0] & 0x04) == 0x04)
                            {
                                container_1.Mode = MainDevDataContains.CALLBACK_MODE;
                            }
                            container_1.BrakeCmd = (recvData[0] & 0x08) == 0x08 ? true : false;
                            container_1.DriveCmd = (recvData[0] & 0x10) == 0x10 ? true : false;
                            container_1.LazyCmd = (recvData[0] & 0x20) == 0x20 ? true : false;
                            container_1.FastBrakeCmd = (recvData[0] & 0x40) == 0x40 ? true : false;
                            container_1.EmergencyBrakeCmd = (recvData[0] & 0x80) == 0x80 ? true : false;
                            #endregion

                            #region byte1
                            container_1.KeepBrakeState = (recvData[1] & 0x01) == 0x01 ? true : false;
                            container_1.LazyState = (recvData[1] & 0x02) == 0x02 ? true : false;
                            container_1.DriveState = (recvData[1] & 0x04) == 0x04 ? true : false;
                            container_1.NormalBrakeState = (recvData[1] & 0x08) == 0x08 ? true : false;
                            container_1.EmergencyBrakeState = (recvData[1] & 0x10) == 0x10 ? true : false;
                            container_1.ZeroSpeedCan = (recvData[1] & 0x20) == 0x20 ? true : false;
                            container_1.ATOMode1 = (recvData[1] & 0x40) == 0x40 ? true : false;
                            container_1.ATOHold = (recvData[1] & 0X80) == 0X80 ? true : false;
                            #endregion

                            #region byte2
                            container_1.SelfTestInt = (recvData[2] & 0x01) == 0x01 ? true : false;
                            container_1.SelfTestActive = (recvData[2] & 0x02) == 0x02 ? true : false;
                            container_1.SelfTestSuccess = (recvData[2] & 0x04) == 0x04 ? true : false;
                            container_1.SelfTestFail = (recvData[2] & 0x08) == 0x08 ? true : false;
                            container_1.UnSelfTest24 = (recvData[2] & 0x10) == 0x10 ? true : false;
                            container_1.UnSelfTest26 = (recvData[2] & 0x20) == 0x20 ? true : false;
                            #endregion

                            #region byte3
                            container_1.BrakeLevelEnable = (recvData[3] & 0x01) == 0x01 ? true : false;
                            container_1.SelfTestCmd = (recvData[3] & 0x02) == 0x02 ? true : false;
                            container_1.EdFadeOut = (recvData[3] & 0x04) == 0x04 ? true : false;
                            container_1.TrainBrakeEnable = (recvData[3] & 0x08) == 0x08 ? true : false;
                            container_1.ZeroSpeed = (recvData[3] & 0x10) == 0x10 ? true : false;
                            container_1.EdOffB1 = (recvData[3] & 0x20) == 0x20 ? true : false;
                            container_1.EdOffC1 = (recvData[3] & 0x40) == 0x40 ? true : false;
                            container_1.WheelInputState = (recvData[3] & 0x80) == 0x80 ? true : false;
                            #endregion

                            #region byte4~5
                            container_1.BrakeLevel = recvData[4] * 256 + recvData[5];
                            #endregion

                            #region byte6~7
                            container_1.TrainBrakeForce = recvData[6] * 256 + recvData[7];
                            #endregion

                            #endregion
                            break;

                        case 1:
                            #region 主从通用数据包1
                            container_1.LifeSig = recvData[point];

                            container_1.SlipLvl1 = recvData[point + 1] & 0x0f;
                            container_1.SlipLvl2 = recvData[point + 1] & 0xf0;
                            container_1.SpeedA1Shaft1 = recvData[point + 2] * 256 + recvData[point + 3];
                            container_1.SpeedA1Shaft2 = recvData[point + 4] * 256 + recvData[point + 5];
                            container_1.AccValue1 = recvData[point + 6] / 10.0;
                            container_1.AccValue2 = recvData[point + 6] / 10.0;
                            #endregion

                            break;
                        case 2:
                            #region TPDO1(Checked)

                            container_1.AbTargetValueAx1 = recvData[point] * 256 + recvData[point + 1];
                            container_1.AbTargetValueAx2 = recvData[point + 2] * 256 + recvData[point + 3];
                            container_1.AbTargetValueAx3 = recvData[point + 4] * 256 + recvData[point + 5];
                            container_1.AbTargetValueAx4 = recvData[point + 6] * 256 + recvData[point + 7];



                            break;
                        #endregion

                        case 3:
                            #region TPDO2(Checked)
                            container_1.AbTargetValueAx5 = recvData[point] * 256 + recvData[point];
                            container_1.AbTargetValueAx6 = recvData[point + 2] * 256 + recvData[point + 3];
                            container_1.HardDriveCmd = (recvData[5] & 0x01) == 0x01 ? true : false;
                            container_1.HardBrakeCmd = (recvData[5] & 0x02) == 0x02 ? true : false;
                            container_1.HardFastBrakeCmd = (recvData[5] & 0x04) == 0x04 ? true : false;
                            container_1.HardEmergencyBrake = (recvData[5] & 0x08) == 0x08 ? true : false;
                            container_1.HardEmergencyDriveCmd = (recvData[5] & 0x10) == 0x10 ? true : false;
                            container_1.CanUnitSelfTestOn = (recvData[5] & 0x20) == 0x20 ? true : false;
                            container_1.ValveCanEmergencyActive = (recvData[5] & 0x40) == 0x40 ? true : false;
                            container_1.CanUintSelfTestOver = (recvData[5] & 0x80) == 0x80 ? true : false;

                            container_1.NetDriveCmd = (recvData[4] & 0x01) == 0x01 ? true : false;
                            container_1.NetBrakeCmd = (recvData[4] & 0x02) == 0x02 ? true : false;
                            container_1.NetFastBrakeCmd = (recvData[4] & 0x40) == 0x40 ? true : false;
                            container_1.TowingMode = (recvData[4] & 0x08) == 0x08 ? true : false;
                            container_1.HoldBrakeRealease = (recvData[4] & 0x10) == 0x10 ? true : false;
                            container_1.CanUintSelfTestCmd_A = (recvData[4] & 0x20) == 0x20 ? true : false;
                            container_1.CanUintSelfTestCmd_B = (recvData[4] & 0x40) == 0x40 ? true : false;
                            container_1.ATOMode1 = (recvData[4] & 0x80) == 0x80 ? true : false;

                            container_1.RefSpeed = (recvData[6] * 256 + recvData[7]) / 10.0;
                            break;
                        #endregion

                        case 4:
                            #region TPDO4(Checked)
                            container_1.BrakeCylinderSourcePressure = recvData[point] * 256 + recvData[point + 1];
                            container_1.AirSpring1PressureA1Car1 = recvData[point + 2] * 256 + recvData[point + 3];
                            container_1.AirSpring2PressureA1Car1 = recvData[point + 4] * 256 + recvData[point + 5];
                            container_1.ParkPressureA1 = recvData[point + 6] * 256 + recvData[point + 7];
                            break;
                        #endregion

                        case 5:
                            #region TPDO5(Checked)
                            container_1.VldRealPressureAx1 = recvData[point] * 256 + recvData[point + 1];
                            container_1.Bcp1PressureAx1 = recvData[point + 2] * 256 + recvData[point + 3];
                            container_1.Bcp2PressureAx2 = recvData[point + 4] * 256 + recvData[point + 5];

                            container_1.BSSRSenorFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_1.AirSpringSenorFault_1 = (recvData[6] & 0x02) == 0x20 ? true : false;
                            container_1.AirSpringSenorFault_2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_1.ParkCylinderSenorFault = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_1.VLDSensorFault = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_1.BSRSenorFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            container_1.BSRSenorFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_1.AirSpringOverflow_1 = (recvData[6] & 0x80) == 0x80 ? true : false;
                            container_1.AirSpringOverflow_2 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_1.BSRLowA11 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_1.ICANFault1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_1.ICANFault2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            break;
                        #endregion

                        case 6:
                            #region TPDO6(Checked)
                            container_1.VldPressureSetupAx1 = recvData[point] * 256 + recvData[point];
                            container_1.MassA1 = recvData[point + 2] * 256 + recvData[point + 3];
                            container_1.BCUFail_Serious = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_1.BCUFail_Middle = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_1.BCUFail_Slight = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_1.OCANFault1 = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_1.OCANFault2 = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_1.SpeedSenorFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_1.SpeedSenorFault_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_1.WSPFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_1.WSPFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            container_1.CodeConnectorFault = (recvData[7] & 0x10) == 0x10 ? true : false;
                            container_1.AirSpringLimit = (recvData[7] & 0x20) == 0x20 ? true : false;
                            container_1.BrakeNotRealease = (recvData[7] & 0x40) == 0x40 ? true : false;
                            container_1.BCPLowA11 = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                        #endregion

                        case 7:
                            #region TPDO7(Checked)

                            container_1.SelfTestSetup = recvData[0] * 256 + recvData[1];

                            container_1.Ax1SelfTestActive = (recvData[2] & 0x01) == 0x01 ? true : false;
                            container_1.Ax1SelfTestOver = (recvData[2] & 0x02) == 0x02 ? true : false;
                            container_1.Ax1SelfTestSuccess = (recvData[2] & 0x04) == 0x04 ? true : false;
                            container_1.Ax1SelfTestFail = (recvData[2] & 0x08) == 0x08 ? true : false;
                            container_1.SlipRate1 = recvData[4];
                            container_1.SlipRate2 = recvData[5];

                            container_1.EPCutOff = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_1.AxisSlip1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_1.AxisSlip2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_1.WheelStoreFail = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_1.GateValveState = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_1.VCM_MVBConnectionState = (recvData[6] & 0x80) == 0x80 ? true : false;

                            container_1.SlipA1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_1.EmergencyBrakeActiveA1 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_1.NotZeroSpeed = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_1.AbActive = (recvData[7] & 0x08) == 0x08 ? true : false;
                            container_1.BCPLowA11 = (recvData[7] & 0x10) == 0x10 ? true : false;
                            container_1.ParkBreakRealease = (recvData[7] & 0x20) == 0x20 ? true : false;
                            container_1.AbStatuesA1 = (recvData[7] & 0x40) == 0x40 ? true : false;
                            container_1.AirSigValid = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                            #endregion
                    }
                    #endregion
                    break;

                case 3:
                    #region 从设备3车数据（4个数据包）
                    switch (canIdLow)
                    {
                        case 1:
                            #region 2节点数据包1(Checked)
                            container_2.LifeSig = recvData[point];
                            container_2.SlipLvl1 = recvData[point + 1] & 0x0f;
                            container_2.SlipLvl2 = recvData[point + 1] & 0xf0;

                            container_2.SpeedShaft1 = recvData[point + 2] * 256 + recvData[point + 3];
                            container_2.SpeedShaft2 = recvData[point + 4] * 256 + recvData[point + 5];

                            container_2.AccValue1 = recvData[point + 6] / 10.0;
                            container_2.AccValue2 = recvData[point + 6] / 10.0;
                            break;
                        #endregion

                        case 4:
                            #region 2节点数据包2(Checked)
                            container_2.BrakeCylinderSourcePressure = recvData[point] * 256 + recvData[point + 1];
                            container_2.AirSpringPressure1 = recvData[point + 2] * 256 + recvData[point + 3];
                            container_2.AirSpringPressure2 = recvData[point + 4] * 256 + recvData[point + 5];
                            container_2.ParkPressure = recvData[6] * 256 + recvData[7];

                            break;
                        #endregion

                        case 5:
                            #region 2节点数据包3(Checked)
                            container_2.VldRealPressure = recvData[point] * 256 + recvData[point + 1];
                            CheckZero(recvData, point + 2);
                            container_2.Bcp1Pressure = recvData[point + 2] * 256 + recvData[point + 3];
                            CheckZero(recvData, point + 4);
                            container_2.Bcp2Pressure = recvData[point + 4] * 256 + recvData[point + 5];

                            container_2.BSSRSenorFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_2.AirSpringSenorFault_1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_2.AirSpringSenorFault_2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_2.VLDSensorFault = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_2.BSRSenorFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            container_2.BSRSenorFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_2.AirSpringOverflow_1 = (recvData[6] & 0x80) == 0x80 ? true : false;
                            container_2.AirSpringOverflow_2 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_2.BSSRSuperLow = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_2.ICANFault1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_2.ICANFault2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            break;
                        #endregion

                        case 6:
                            #region 2节点数据包4
                            container_2.VldSetupPressure = recvData[point] * 256 + recvData[point + 1];
                            container_2.MassValue = recvData[point + 2] * 256 + recvData[point + 3];

                            container_2.BCUFail_Serious = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_2.BCUFail_Middle = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_2.BCUFail_Slight = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_2.EmergencyBrakeFault = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_2.OCANFault1 = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_2.OCANFault2 = (recvData[6] & 0x20) == 0x20 ? true : false;

                            container_2.SpeedSenorFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_2.SpeedSenorFault_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_2.WSPFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_2.WSPFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            container_2.CodeConnectorFault = (recvData[7] & 0x10) == 0x10 ? true : false;
                            container_2.AirSpringLimit = (recvData[7] & 0x20) == 0x20 ? true : false;
                            container_2.BrakeNotRealease = (recvData[7] & 0x40) == 0x40 ? true : false;
                            container_2.BCPLow1 = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                        #endregion

                        case 7:
                            #region 2节点数据包5
                            container_2.SelfTestSetup = recvData[0] * 256 + recvData[1];
                            container_2.SelfTestActive = (recvData[2] & 0x01) == 0x01 ? true : false;
                            container_2.SelfTestOver = (recvData[2] & 0x02) == 0x02 ? true : false;
                            container_2.SelfTestSuccess = (recvData[2] & 0x04) == 0x04 ? true : false;
                            container_2.SelfTestFail = (recvData[2] & 0x08) == 0x08 ? true : false;
                            container_2.SlipRate1 = recvData[4];
                            container_2.SlipRate2 = recvData[5];

                            container_2.EPCutOff = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_2.AxisSlip1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_2.AxisSlip2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_2.WheelStoreFail = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_2.GateValveState = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_2.VCM_MVBConnectionState = (recvData[6] & 0x80) == 0x80 ? true : false;

                            container_2.Slip = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_2.EmergencyBrakeActive = (recvData[7] & 0x02) == 0x20 ? true : false;
                            container_2.AbBrakeActive = (recvData[7] & 0x08) == 0x08 ? true : false;
                            container_2.BSRLow1 = (recvData[7] & 0x10) == 0x10 ? true : false;
                            container_2.ParkBrakeRealease = (recvData[7] & 0x20) == 0x20 ? true : false;
                            container_2.AbBrakeSatet = (recvData[7] & 0x40) == 0x40 ? true : false;
                            container_2.AirSigValid = (recvData[7] & 0x80) == 0x80 ? true : false;


                            break;
                            #endregion
                    }
                    #endregion
                    break;

                case 4:
                    #region 从设备3车数据（4个数据包）
                    switch (canIdLow)
                    {
                        case 1:
                            #region 2节点数据包1(Checked)
                            container_3.LifeSig = recvData[point];
                            container_3.SlipLvl1 = recvData[point + 1] & 0x0f;
                            container_3.SlipLvl2 = recvData[point + 1] & 0xf0;

                            container_3.SpeedShaft1 = recvData[point + 2] * 256 + recvData[point + 3];
                            container_3.SpeedShaft2 = recvData[point + 4] * 256 + recvData[point + 5];

                            container_3.AccValue1 = recvData[point + 6] / 10.0;
                            container_3.AccValue2 = recvData[point + 6] / 10.0;
                            break;
                        #endregion

                        case 4:
                            #region 2节点数据包2(Checked)
                            container_3.BrakeCylinderSourcePressure = recvData[point] * 256 + recvData[point + 1];
                            container_3.AirSpringPressure1 = recvData[point + 2] * 256 + recvData[point + 3];
                            container_3.AirSpringPressure2 = recvData[point + 4] * 256 + recvData[point + 5];
                            container_3.ParkPressure = recvData[6] * 256 + recvData[7];

                            break;
                        #endregion

                        case 5:
                            #region 2节点数据包3(Checked)
                            container_3.VldRealPressure = recvData[point] * 256 + recvData[point + 1];
                            CheckZero(recvData, point + 2);
                            container_3.Bcp1Pressure = recvData[point + 2] * 256 + recvData[point + 3];
                            CheckZero(recvData, point + 4);
                            container_3.Bcp2Pressure = recvData[point + 4] * 256 + recvData[point + 5];

                            container_3.BSSRSenorFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_3.AirSpringSenorFault_1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_3.AirSpringSenorFault_2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_3.VLDSensorFault = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_3.BSRSenorFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            container_3.BSRSenorFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_3.AirSpringOverflow_1 = (recvData[6] & 0x80) == 0x80 ? true : false;
                            container_3.AirSpringOverflow_2 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_3.BSSRSuperLow = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_3.ICANFault1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_3.ICANFault2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            break;
                        #endregion

                        case 6:
                            #region 2节点数据包4
                            container_3.VldSetupPressure = recvData[point] * 256 + recvData[point + 1];
                            container_3.MassValue = recvData[point + 2] * 256 + recvData[point + 3];

                            container_3.BCUFail_Serious = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_3.BCUFail_Middle = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_3.BCUFail_Slight = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_3.EmergencyBrakeFault = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_3.OCANFault1 = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_3.OCANFault2 = (recvData[6] & 0x20) == 0x20 ? true : false;

                            container_3.SpeedSenorFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_3.SpeedSenorFault_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_3.WSPFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_3.WSPFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            container_3.CodeConnectorFault = (recvData[7] & 0x10) == 0x10 ? true : false;
                            container_3.AirSpringLimit = (recvData[7] & 0x20) == 0x20 ? true : false;
                            container_3.BrakeNotRealease = (recvData[7] & 0x40) == 0x40 ? true : false;
                            container_3.BCPLow1 = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                        #endregion

                        case 7:
                            #region 2节点数据包5
                            container_3.SelfTestSetup = recvData[0] * 256 + recvData[1];
                            container_3.SelfTestActive = (recvData[2] & 0x01) == 0x01 ? true : false;
                            container_3.SelfTestOver = (recvData[2] & 0x02) == 0x02 ? true : false;
                            container_3.SelfTestSuccess = (recvData[2] & 0x04) == 0x04 ? true : false;
                            container_3.SelfTestFail = (recvData[2] & 0x08) == 0x08 ? true : false;
                            container_3.SlipRate1 = recvData[4];
                            container_3.SlipRate2 = recvData[5];

                            container_3.EPCutOff = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_3.AxisSlip1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_3.AxisSlip2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_3.WheelStoreFail = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_3.GateValveState = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_3.VCM_MVBConnectionState = (recvData[6] & 0x80) == 0x80 ? true : false;

                            container_3.Slip = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_3.EmergencyBrakeActive = (recvData[7] & 0x02) == 0x20 ? true : false;
                            container_3.AbBrakeActive = (recvData[7] & 0x08) == 0x08 ? true : false;
                            container_3.BSRLow1 = (recvData[7] & 0x10) == 0x10 ? true : false;
                            container_3.ParkBrakeRealease = (recvData[7] & 0x20) == 0x20 ? true : false;
                            container_3.AbBrakeSatet = (recvData[7] & 0x40) == 0x40 ? true : false;
                            container_3.AirSigValid = (recvData[7] & 0x80) == 0x80 ? true : false;


                            break;
                            #endregion
                    }
                    #endregion
                    break;

                case 5:
                    #region 从设备4车数据（4个数据包）
                    switch (canIdLow)
                    {
                        case 1:
                            #region 2节点数据包1(Checked)
                            container_4.LifeSig = recvData[point];
                            container_4.SlipLvl1 = recvData[point + 1] & 0x0f;
                            container_4.SlipLvl2 = recvData[point + 1] & 0xf0;

                            container_4.SpeedShaft1 = recvData[point + 2] * 256 + recvData[point + 3];
                            container_4.SpeedShaft2 = recvData[point + 4] * 256 + recvData[point + 5];

                            container_4.AccValue1 = recvData[point + 6] / 10.0;
                            container_4.AccValue2 = recvData[point + 6] / 10.0;
                            break;
                        #endregion

                        case 4:
                            #region 2节点数据包2(Checked)
                            container_4.BrakeCylinderSourcePressure = recvData[point] * 256 + recvData[point + 1];
                            container_4.AirSpringPressure1 = recvData[point + 2] * 256 + recvData[point + 3];
                            container_4.AirSpringPressure2 = recvData[point + 4] * 256 + recvData[point + 5];
                            container_4.ParkPressure = recvData[6] * 256 + recvData[7];

                            break;
                        #endregion

                        case 5:
                            #region 2节点数据包3(Checked)
                            container_4.VldRealPressure = recvData[point] * 256 + recvData[point + 1];
                            CheckZero(recvData, point + 2);
                            container_4.Bcp1Pressure = recvData[point + 2] * 256 + recvData[point + 3];
                            CheckZero(recvData, point + 4);
                            container_4.Bcp2Pressure = recvData[point + 4] * 256 + recvData[point + 5];

                            container_4.BSSRSenorFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_4.AirSpringSenorFault_1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_4.AirSpringSenorFault_2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_4.VLDSensorFault = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_4.BSRSenorFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            container_4.BSRSenorFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_4.AirSpringOverflow_1 = (recvData[6] & 0x80) == 0x80 ? true : false;
                            container_4.AirSpringOverflow_2 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_4.BSSRSuperLow = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_4.ICANFault1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_4.ICANFault2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            break;
                        #endregion

                        case 6:
                            #region 2节点数据包4
                            container_4.VldSetupPressure = recvData[point] * 256 + recvData[point + 1];
                            container_4.MassValue = recvData[point + 2] * 256 + recvData[point + 3];

                            container_4.BCUFail_Serious = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_4.BCUFail_Middle = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_4.BCUFail_Slight = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_4.EmergencyBrakeFault = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_4.OCANFault1 = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_4.OCANFault2 = (recvData[6] & 0x20) == 0x20 ? true : false;

                            container_4.SpeedSenorFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_4.SpeedSenorFault_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_4.WSPFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_4.WSPFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            container_4.CodeConnectorFault = (recvData[7] & 0x10) == 0x10 ? true : false;
                            container_4.AirSpringLimit = (recvData[7] & 0x20) == 0x20 ? true : false;
                            container_4.BrakeNotRealease = (recvData[7] & 0x40) == 0x40 ? true : false;
                            container_4.BCPLow1 = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                        #endregion

                        case 7:
                            #region 2节点数据包5
                            container_4.SelfTestSetup = recvData[0] * 256 + recvData[1];
                            container_4.SelfTestActive = (recvData[2] & 0x01) == 0x01 ? true : false;
                            container_4.SelfTestOver = (recvData[2] & 0x02) == 0x02 ? true : false;
                            container_4.SelfTestSuccess = (recvData[2] & 0x04) == 0x04 ? true : false;
                            container_4.SelfTestFail = (recvData[2] & 0x08) == 0x08 ? true : false;
                            container_4.SlipRate1 = recvData[4];
                            container_4.SlipRate2 = recvData[5];

                            container_4.EPCutOff = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_4.AxisSlip1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_4.AxisSlip2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_4.WheelStoreFail = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_4.GateValveState = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_4.VCM_MVBConnectionState = (recvData[6] & 0x80) == 0x80 ? true : false;

                            container_4.Slip = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_4.EmergencyBrakeActive = (recvData[7] & 0x02) == 0x20 ? true : false;
                            container_4.AbBrakeActive = (recvData[7] & 0x08) == 0x08 ? true : false;
                            container_4.BSRLow1 = (recvData[7] & 0x10) == 0x10 ? true : false;
                            container_4.ParkBrakeRealease = (recvData[7] & 0x20) == 0x20 ? true : false;
                            container_4.AbBrakeSatet = (recvData[7] & 0x40) == 0x40 ? true : false;
                            container_4.AirSigValid = (recvData[7] & 0x80) == 0x80 ? true : false;


                            break;
                            #endregion
                    }
                    #endregion
                    break;

                case 6:
                    #region 从设备5车数据（4个数据包）
                    switch (canIdLow)
                    {
                        case 1:
                            #region 2节点数据包1(Checked)
                            container_5.LifeSig = recvData[point];
                            container_5.SlipLvl1 = recvData[point + 1] & 0x0f;
                            container_5.SlipLvl2 = recvData[point + 1] & 0xf0;

                            container_5.SpeedShaft1 = recvData[point + 2] * 256 + recvData[point + 3];
                            container_5.SpeedShaft2 = recvData[point + 4] * 256 + recvData[point + 5];

                            container_5.AccValue1 = recvData[point + 6] / 10.0;
                            container_5.AccValue2 = recvData[point + 6] / 10.0;
                            break;
                        #endregion

                        case 4:
                            #region 2节点数据包2(Checked)
                            container_5.BrakeCylinderSourcePressure = recvData[point] * 256 + recvData[point + 1];
                            container_5.AirSpringPressure1 = recvData[point + 2] * 256 + recvData[point + 3];
                            container_5.AirSpringPressure2 = recvData[point + 4] * 256 + recvData[point + 5];
                            container_5.ParkPressure = recvData[6] * 256 + recvData[7];

                            break;
                        #endregion

                        case 5:
                            #region 2节点数据包3(Checked)
                            container_5.VldRealPressure = recvData[point] * 256 + recvData[point + 1];
                            CheckZero(recvData, point + 2);
                            container_5.Bcp1Pressure = recvData[point + 2] * 256 + recvData[point + 3];
                            CheckZero(recvData, point + 4);
                            container_5.Bcp2Pressure = recvData[point + 4] * 256 + recvData[point + 5];

                            container_5.BSSRSenorFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_5.AirSpringSenorFault_1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_5.AirSpringSenorFault_2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_5.VLDSensorFault = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_5.BSRSenorFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            container_5.BSRSenorFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_5.AirSpringOverflow_1 = (recvData[6] & 0x80) == 0x80 ? true : false;
                            container_5.AirSpringOverflow_2 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_5.BSSRSuperLow = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_5.ICANFault1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_5.ICANFault2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            break;
                        #endregion

                        case 6:
                            #region 2节点数据包4
                            container_5.VldSetupPressure = recvData[point] * 256 + recvData[point + 1];
                            container_5.MassValue = recvData[point + 2] * 256 + recvData[point + 3];

                            container_5.BCUFail_Serious = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_5.BCUFail_Middle = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_5.BCUFail_Slight = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_5.EmergencyBrakeFault = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_5.OCANFault1 = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_5.OCANFault2 = (recvData[6] & 0x20) == 0x20 ? true : false;

                            container_5.SpeedSenorFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_5.SpeedSenorFault_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_5.WSPFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_5.WSPFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            container_5.CodeConnectorFault = (recvData[7] & 0x10) == 0x10 ? true : false;
                            container_5.AirSpringLimit = (recvData[7] & 0x20) == 0x20 ? true : false;
                            container_5.BrakeNotRealease = (recvData[7] & 0x40) == 0x40 ? true : false;
                            container_5.BCPLow1 = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                        #endregion

                        case 7:
                            #region 2节点数据包5
                            container_5.SelfTestSetup = recvData[0] * 256 + recvData[1];
                            container_5.SelfTestActive = (recvData[2] & 0x01) == 0x01 ? true : false;
                            container_5.SelfTestOver = (recvData[2] & 0x02) == 0x02 ? true : false;
                            container_5.SelfTestSuccess = (recvData[2] & 0x04) == 0x04 ? true : false;
                            container_5.SelfTestFail = (recvData[2] & 0x08) == 0x08 ? true : false;
                            container_5.SlipRate1 = recvData[4];
                            container_5.SlipRate2 = recvData[5];

                            container_5.EPCutOff = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_5.AxisSlip1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_5.AxisSlip2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_5.WheelStoreFail = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_5.GateValveState = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_5.VCM_MVBConnectionState = (recvData[6] & 0x80) == 0x80 ? true : false;

                            container_5.Slip = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_5.EmergencyBrakeActive = (recvData[7] & 0x02) == 0x20 ? true : false;
                            container_5.AbBrakeActive = (recvData[7] & 0x08) == 0x08 ? true : false;
                            container_5.BSRLow1 = (recvData[7] & 0x10) == 0x10 ? true : false;
                            container_5.ParkBrakeRealease = (recvData[7] & 0x20) == 0x20 ? true : false;
                            container_5.AbBrakeSatet = (recvData[7] & 0x40) == 0x40 ? true : false;
                            container_5.AirSigValid = (recvData[7] & 0x80) == 0x80 ? true : false;


                            break;
                            #endregion
                    }
                    #endregion
                    break;

                case 2:
                    #region 主设备A1车CAN消息（6个数据包）
                    switch (canIdLow)
                    {
                        case 0:
                            #region TPDO0(Checked)

                            #region byte0
                            if ((recvData[0] & 0x01) == 0x01)
                            {
                                container_6.Mode = MainDevDataContains.NORMAL_MODE;
                            }
                            else if ((recvData[0] & 0x02) == 0x02)
                            {
                                container_6.Mode = MainDevDataContains.EMERGENCY_DRIVE_MODE;
                            }
                            else if ((recvData[0] & 0x04) == 0x04)
                            {
                                container_6.Mode = MainDevDataContains.CALLBACK_MODE;
                            }
                            container_6.BrakeCmd = (recvData[0] & 0x08) == 0x08 ? true : false;
                            container_6.DriveCmd = (recvData[0] & 0x10) == 0x10 ? true : false;
                            container_6.LazyCmd = (recvData[0] & 0x20) == 0x20 ? true : false;
                            container_6.FastBrakeCmd = (recvData[0] & 0x40) == 0x40 ? true : false;
                            container_6.EmergencyBrakeCmd = (recvData[0] & 0x80) == 0x80 ? true : false;
                            #endregion

                            #region byte1
                            container_6.KeepBrakeState = (recvData[1] & 0x01) == 0x01 ? true : false;
                            container_6.LazyState = (recvData[1] & 0x02) == 0x02 ? true : false;
                            container_6.DriveState = (recvData[1] & 0x04) == 0x04 ? true : false;
                            container_6.NormalBrakeState = (recvData[1] & 0x08) == 0x08 ? true : false;
                            container_6.EmergencyBrakeState = (recvData[1] & 0x10) == 0x10 ? true : false;
                            container_6.ZeroSpeedCan = (recvData[1] & 0x20) == 0x20 ? true : false;
                            container_6.ATOMode1 = (recvData[1] & 0x40) == 0x40 ? true : false;
                            container_6.ATOHold = (recvData[1] & 0X80) == 0X80 ? true : false;
                            #endregion

                            #region byte2
                            container_6.SelfTestInt = (recvData[2] & 0x01) == 0x01 ? true : false;
                            container_6.SelfTestActive = (recvData[2] & 0x02) == 0x02 ? true : false;
                            container_6.SelfTestSuccess = (recvData[2] & 0x04) == 0x04 ? true : false;
                            container_6.SelfTestFail = (recvData[2] & 0x08) == 0x08 ? true : false;
                            container_6.UnSelfTest24 = (recvData[2] & 0x10) == 0x10 ? true : false;
                            container_6.UnSelfTest26 = (recvData[2] & 0x20) == 0x20 ? true : false;
                            #endregion

                            #region byte3
                            container_6.BrakeLevelEnable = (recvData[3] & 0x01) == 0x01 ? true : false;
                            container_6.SelfTestCmd = (recvData[3] & 0x02) == 0x02 ? true : false;
                            container_6.EdFadeOut = (recvData[3] & 0x04) == 0x04 ? true : false;
                            container_6.TrainBrakeEnable = (recvData[3] & 0x08) == 0x08 ? true : false;
                            container_6.ZeroSpeed = (recvData[3] & 0x10) == 0x10 ? true : false;
                            container_6.EdOffB1 = (recvData[3] & 0x20) == 0x20 ? true : false;
                            container_6.EdOffC1 = (recvData[3] & 0x40) == 0x40 ? true : false;
                            container_6.WheelInputState = (recvData[3] & 0x80) == 0x80 ? true : false;
                            #endregion

                            #region byte4~5
                            container_6.BrakeLevel = recvData[4] * 256 + recvData[5];
                            #endregion

                            #region byte6~7
                            container_6.TrainBrakeForce = recvData[6] * 256 + recvData[7];
                            #endregion

                            #endregion
                            break;

                        case 1:
                            #region 主从通用数据包1
                            container_6.LifeSig = recvData[point];

                            container_6.SlipLvl1 = recvData[point + 1] & 0x0f;
                            container_6.SlipLvl2 = recvData[point + 1] & 0xf0;
                            container_6.SpeedA1Shaft1 = recvData[point + 2] * 256 + recvData[point + 3];
                            container_6.SpeedA1Shaft2 = recvData[point + 4] * 256 + recvData[point + 5];
                            container_6.AccValue1 = recvData[point + 6] / 10.0;
                            container_6.AccValue2 = recvData[point + 6] / 10.0;
                            #endregion

                            break;
                        case 2:
                            #region TPDO1(Checked)

                            container_6.AbTargetValueAx1 = recvData[point] * 256 + recvData[point + 1];
                            container_6.AbTargetValueAx2 = recvData[point + 2] * 256 + recvData[point + 3];
                            container_6.AbTargetValueAx3 = recvData[point + 4] * 256 + recvData[point + 5];
                            container_6.AbTargetValueAx4 = recvData[point + 6] * 256 + recvData[point + 7];



                            break;
                        #endregion

                        case 3:
                            #region TPDO2(Checked)
                            container_6.AbTargetValueAx5 = recvData[point] * 256 + recvData[point];
                            container_6.AbTargetValueAx6 = recvData[point + 2] * 256 + recvData[point + 3];
                            container_6.HardDriveCmd = (recvData[5] & 0x01) == 0x01 ? true : false;
                            container_6.HardBrakeCmd = (recvData[5] & 0x02) == 0x02 ? true : false;
                            container_6.HardFastBrakeCmd = (recvData[5] & 0x04) == 0x04 ? true : false;
                            container_6.HardEmergencyBrake = (recvData[5] & 0x08) == 0x08 ? true : false;
                            container_6.HardEmergencyDriveCmd = (recvData[5] & 0x10) == 0x10 ? true : false;
                            container_6.CanUnitSelfTestOn = (recvData[5] & 0x20) == 0x20 ? true : false;
                            container_6.ValveCanEmergencyActive = (recvData[5] & 0x40) == 0x40 ? true : false;
                            container_6.CanUintSelfTestOver = (recvData[5] & 0x80) == 0x80 ? true : false;

                            container_6.NetDriveCmd = (recvData[4] & 0x01) == 0x01 ? true : false;
                            container_6.NetBrakeCmd = (recvData[4] & 0x02) == 0x02 ? true : false;
                            container_6.NetFastBrakeCmd = (recvData[4] & 0x40) == 0x40 ? true : false;
                            container_6.TowingMode = (recvData[4] & 0x08) == 0x08 ? true : false;
                            container_6.HoldBrakeRealease = (recvData[4] & 0x10) == 0x10 ? true : false;
                            container_6.CanUintSelfTestCmd_A = (recvData[4] & 0x20) == 0x20 ? true : false;
                            container_6.CanUintSelfTestCmd_B = (recvData[4] & 0x40) == 0x40 ? true : false;
                            container_6.ATOMode1 = (recvData[4] & 0x80) == 0x80 ? true : false;

                            container_6.RefSpeed = (recvData[6] * 256 + recvData[7]) / 10.0;
                            break;
                        #endregion

                        case 4:
                            #region TPDO4(Checked)
                            container_6.BrakeCylinderSourcePressure = recvData[point] * 256 + recvData[point + 1];
                            container_6.AirSpring1PressureA1Car1 = recvData[point + 2] * 256 + recvData[point + 3];
                            container_6.AirSpring2PressureA1Car1 = recvData[point + 4] * 256 + recvData[point + 5];
                            container_6.ParkPressureA1 = recvData[point + 6] * 256 + recvData[point + 7];
                            break;
                        #endregion

                        case 5:
                            #region TPDO5(Checked)
                            container_6.VldRealPressureAx1 = recvData[point] * 256 + recvData[point + 1];
                            container_6.Bcp1PressureAx1 = recvData[point + 2] * 256 + recvData[point + 3];
                            container_6.Bcp2PressureAx2 = recvData[point + 4] * 256 + recvData[point + 5];

                            container_6.BSSRSenorFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_6.AirSpringSenorFault_1 = (recvData[6] & 0x02) == 0x20 ? true : false;
                            container_6.AirSpringSenorFault_2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_6.ParkCylinderSenorFault = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_6.VLDSensorFault = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_6.BSRSenorFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            container_6.BSRSenorFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_6.AirSpringOverflow_1 = (recvData[6] & 0x80) == 0x80 ? true : false;
                            container_6.AirSpringOverflow_2 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_6.BSRLowA11 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_6.ICANFault1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_6.ICANFault2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            break;
                        #endregion

                        case 6:
                            #region TPDO6(Checked)
                            container_6.VldPressureSetupAx1 = recvData[point] * 256 + recvData[point];
                            container_6.MassA1 = recvData[point + 2] * 256 + recvData[point + 3];
                            container_6.BCUFail_Serious = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_6.BCUFail_Middle = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_6.BCUFail_Slight = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_6.OCANFault1 = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_6.OCANFault2 = (recvData[6] & 0x10) == 0x10 ? true : false;
                            container_6.SpeedSenorFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_6.SpeedSenorFault_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_6.WSPFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_6.WSPFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            container_6.CodeConnectorFault = (recvData[7] & 0x10) == 0x10 ? true : false;
                            container_6.AirSpringLimit = (recvData[7] & 0x20) == 0x20 ? true : false;
                            container_6.BrakeNotRealease = (recvData[7] & 0x40) == 0x40 ? true : false;
                            container_6.BCPLowA11 = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                        #endregion

                        case 7:
                            #region TPDO7(Checked)

                            container_6.SelfTestSetup = recvData[0] * 256 + recvData[1];

                            container_6.Ax1SelfTestActive = (recvData[2] & 0x01) == 0x01 ? true : false;
                            container_6.Ax1SelfTestOver = (recvData[2] & 0x02) == 0x02 ? true : false;
                            container_6.Ax1SelfTestSuccess = (recvData[2] & 0x04) == 0x04 ? true : false;
                            container_6.Ax1SelfTestFail = (recvData[2] & 0x08) == 0x08 ? true : false;
                            container_6.SlipRate1 = recvData[4];
                            container_6.SlipRate2 = recvData[5];

                            container_6.EPCutOff = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_6.AxisSlip1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_6.AxisSlip2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_6.WheelStoreFail = (recvData[6] & 0x08) == 0x08 ? true : false;
                            container_6.GateValveState = (recvData[6] & 0x40) == 0x40 ? true : false;
                            container_6.VCM_MVBConnectionState = (recvData[6] & 0x80) == 0x80 ? true : false;

                            container_6.SlipA1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            container_6.EmergencyBrakeActiveA1 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            container_6.NotZeroSpeed = (recvData[7] & 0x04) == 0x04 ? true : false;
                            container_6.AbActive = (recvData[7] & 0x08) == 0x08 ? true : false;
                            container_6.BCPLowA11 = (recvData[7] & 0x10) == 0x10 ? true : false;
                            container_6.ParkBreakRealease = (recvData[7] & 0x20) == 0x20 ? true : false;
                            container_6.AbStatuesA1 = (recvData[7] & 0x40) == 0x40 ? true : false;
                            container_6.AirSigValid = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                            #endregion
                    }
                    #endregion
                    break;

                case 7:
                    #region 1车附加数据
                    switch (canIdLow)
                    {
                        case 1:
                            #region 1车附加1数据(Checked)
                            container_1.VCMLifeSig = recvData[1];
                            container_1.DcuLifeSig[0] = recvData[2];
                            container_1.DcuLifeSig[1] = recvData[3];

                            container_1.DcuEbOK[0] = (recvData[4] & 0x01) == 0x01 ? true : false;
                            container_1.DcuEbFadeout[0] = (recvData[4] & 0x02) == 0x02 ? true : false;
                            container_1.DcuEbSlip[0] = (recvData[4] & 0x04) == 0x04 ? true : false;
                            container_1.DcuEbOK[1] = (recvData[4] & 0x10) == 0x10 ? true : false;
                            container_1.DcuEbFadeout[1] = (recvData[4] & 0x20) == 0x20 ? true : false;
                            container_1.DcuEbSlip[1] = (recvData[4] & 0x40) == 0x40 ? true : false;
                            container_1.DcuEbOK[2] = (recvData[5] & 0x01) == 0x01 ? true : false;
                            container_1.DcuEbFadeout[2] = (recvData[5] & 0x02) == 0x02 ? true : false;
                            container_1.DcuEbSlip[2] = (recvData[5] & 0x04) == 0x04 ? true : false;
                            container_1.DcuEbOK[3] = (recvData[5] & 0x10) == 0x10 ? true : false;
                            container_1.DcuEbFadeout[3] = (recvData[5] & 0x20) == 0x20 ? true : false;
                            container_1.DcuEbSlip[3] = (recvData[5] & 0x40) == 0x40 ? true : false;

                            container_1.DcuLifeSig[2] = recvData[6];
                            container_1.DcuLifeSig[3] = recvData[7];
                            #endregion
                            break;
                        case 2:
                            #region 1车附加2数据(Checked)
                            container_1.DcuEbRealValue[0] = recvData[0] * 256 + recvData[1];
                            container_1.DcuMax[0] = recvData[2] * 256 + recvData[3];
                            container_1.DcuEbRealValue[1] = recvData[4] * 256 + recvData[5];
                            container_1.DcuMax[1] = recvData[6] * 256 + recvData[7];
                            #endregion
                            break;
                        case 3:
                            #region 1车附加3数据(Checked)
                            container_1.DcuEbRealValue[2] = recvData[0] * 256 + recvData[1];
                            container_1.DcuMax[2] = recvData[2] * 256 + recvData[3];
                            container_1.DcuEbRealValue[3] = recvData[4] * 256 + recvData[5];
                            container_1.DcuMax[3] = recvData[6] * 256 + recvData[7];
                            #endregion
                            break;
                        case 4:
                            #region 1车附加4数据(Checked)
                            container_1.AbCapacity[0] = recvData[0] * 256 + recvData[1];
                            container_1.AbCapacity[1] = recvData[2] * 256 + recvData[3];
                            container_1.AbCapacity[2] = recvData[4] * 256 + recvData[5];
                            container_1.AbCapacity[3] = recvData[6] * 256 + recvData[7];
                            #endregion
                            break;
                        case 5:
                            #region 1车附加5数据
                            container_1.AbCapacity[4] = recvData[0] * 256 + recvData[1];
                            container_1.AbCapacity[5] = recvData[2] * 256 + recvData[3];
                            container_1.AbRealValue[0] = recvData[4] * 256 + recvData[5];
                            container_1.AbRealValue[1] = recvData[6] * 256 + recvData[7];
                            #endregion
                            break;
                        case 6:
                            #region 1车附加6数据(Checked)
                            container_1.AbRealValue[2] = recvData[0] * 256 + recvData[1];
                            container_1.AbRealValue[3] = recvData[2] * 256 + recvData[3];
                            container_1.AbRealValue[4] = recvData[4] * 256 + recvData[5];
                            container_1.AbRealValue[5] = recvData[6] * 256 + recvData[7];
                            #endregion
                            break;

                        case 7:
                            #region 1车附加7数据
                            container_1.DcuVolta[0] = recvData[0] * 256 + recvData[1];
                            container_1.DcuVolta[1] = recvData[2] * 256 + recvData[3];
                            container_1.DcuVolta[2] = recvData[4] * 256 + recvData[5];
                            container_1.DcuVolta[3] = recvData[6] * 256 + recvData[7];
                            #endregion
                            break;

                        case 8:
                            #region 1车附加7数据(Checked)
                            container_1.SpeedDetection = (recvData[0] & 0x01) == 0x01 ? true : false;
                            container_1.CanBusFail1 = (recvData[0] & 0x02) == 0x02 ? true : false;
                            container_1.CanBusFail2 = (recvData[0] & 0x04) == 0x04 ? true : false;
                            container_1.HardDifferent = (recvData[0] & 0x08) == 0x08 ? true : false;
                            container_1.EventHigh = (recvData[0] & 0x10) == 0x10 ? true : false;
                            container_1.EventMid = (recvData[0] & 0x20) == 0x20 ? true : false;
                            container_1.EventLow = (recvData[0] & 0x40) == 0x40 ? true : false;
                            container_1.CanASPEnable = (recvData[0] & 0x80) == 0x80 ? true : false;

                            container_1.BCPLowA = (recvData[1] & 0x01) == 0x01 ? true : false;
                            container_1.BCPLowB = (recvData[1] & 0x02) == 0x02 ? true : false;
                            container_1.BCPLowC = (recvData[1] & 0x04) == 0x04 ? true : false;

                            container_1.UnixHour = recvData[2] * 256 + recvData[3];
                            container_1.UnixMinute = recvData[4] * 256 + recvData[5];
                            container_1.UnixTimeValid = (recvData[6] & 0x20) == 0x20 ? true : false;

                            #endregion
                            break;
                        case 9:
                            #region 1车附加9数据(Checked)
                            container_1.Tc1 = recvData[0] * 256 + recvData[1];
                            container_1.Mp1 = recvData[2] * 256 + recvData[3];
                            container_1.M1 = recvData[4] * 256 + recvData[5];
                            container_1.Tc1Valid = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_1.Mp1Valid = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_1.M1Valid = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_1.CanWheelInputCondition = (recvData[6] & 0x08) == 0x08 ? true : false;
                            #endregion
                            break;
                        default:
                            break;
                    }
                    #endregion
                    break;

                case 8:
                    #region 2车附加数据
                    switch (canIdLow)
                    {
                        case 1:
                            #region 1车附加1数据(Checked)
                            container_6.VCMLifeSig = recvData[1];
                            container_6.DcuLifeSig[0] = recvData[2];
                            container_6.DcuLifeSig[1] = recvData[3];

                            container_6.DcuEbOK[0] = (recvData[4] & 0x01) == 0x01 ? true : false;
                            container_6.DcuEbFadeout[0] = (recvData[4] & 0x02) == 0x02 ? true : false;
                            container_6.DcuEbSlip[0] = (recvData[4] & 0x04) == 0x04 ? true : false;
                            container_6.DcuEbOK[1] = (recvData[4] & 0x10) == 0x10 ? true : false;
                            container_6.DcuEbFadeout[1] = (recvData[4] & 0x20) == 0x20 ? true : false;
                            container_6.DcuEbSlip[1] = (recvData[4] & 0x40) == 0x40 ? true : false;
                            container_6.DcuEbOK[2] = (recvData[5] & 0x01) == 0x01 ? true : false;
                            container_6.DcuEbFadeout[2] = (recvData[5] & 0x02) == 0x02 ? true : false;
                            container_6.DcuEbSlip[2] = (recvData[5] & 0x04) == 0x04 ? true : false;
                            container_6.DcuEbOK[3] = (recvData[5] & 0x10) == 0x10 ? true : false;
                            container_6.DcuEbFadeout[3] = (recvData[5] & 0x20) == 0x20 ? true : false;
                            container_6.DcuEbSlip[3] = (recvData[5] & 0x40) == 0x40 ? true : false;

                            container_6.DcuLifeSig[2] = recvData[6];
                            container_6.DcuLifeSig[3] = recvData[7];
                            #endregion
                            break;
                        case 2:
                            #region 1车附加2数据(Checked)
                            container_6.DcuEbRealValue[0] = recvData[0] * 256 + recvData[1];
                            container_6.DcuMax[0] = recvData[2] * 256 + recvData[3];
                            container_6.DcuEbRealValue[1] = recvData[4] * 256 + recvData[5];
                            container_6.DcuMax[1] = recvData[6] * 256 + recvData[7];
                            #endregion
                            break;
                        case 3:
                            #region 1车附加3数据(Checked)
                            container_6.DcuEbRealValue[2] = recvData[0] * 256 + recvData[1];
                            container_6.DcuMax[2] = recvData[2] * 256 + recvData[3];
                            container_6.DcuEbRealValue[3] = recvData[4] * 256 + recvData[5];
                            container_6.DcuMax[3] = recvData[6] * 256 + recvData[7];
                            #endregion
                            break;
                        case 4:
                            #region 1车附加4数据(Checked)
                            container_6.AbCapacity[0] = recvData[0] * 256 + recvData[1];
                            container_6.AbCapacity[1] = recvData[2] * 256 + recvData[3];
                            container_6.AbCapacity[2] = recvData[4] * 256 + recvData[5];
                            container_6.AbCapacity[3] = recvData[6] * 256 + recvData[7];
                            #endregion
                            break;
                        case 5:
                            #region 1车附加5数据
                            container_6.AbCapacity[4] = recvData[0] * 256 + recvData[1];
                            container_6.AbCapacity[5] = recvData[2] * 256 + recvData[3];
                            container_6.AbRealValue[0] = recvData[4] * 256 + recvData[5];
                            container_6.AbRealValue[1] = recvData[6] * 256 + recvData[7];
                            #endregion
                            break;
                        case 6:
                            #region 1车附加6数据(Checked)
                            container_6.AbRealValue[2] = recvData[0] * 256 + recvData[1];
                            container_6.AbRealValue[3] = recvData[2] * 256 + recvData[3];
                            container_6.AbRealValue[4] = recvData[4] * 256 + recvData[5];
                            container_6.AbRealValue[5] = recvData[6] * 256 + recvData[7];
                            #endregion
                            break;

                        case 7:
                            #region 1车附加7数据
                            container_6.DcuVolta[0] = recvData[0] * 256 + recvData[1];
                            container_6.DcuVolta[1] = recvData[2] * 256 + recvData[3];
                            container_6.DcuVolta[2] = recvData[4] * 256 + recvData[5];
                            container_6.DcuVolta[3] = recvData[6] * 256 + recvData[7];
                            #endregion
                            break;

                        case 8:
                            #region 1车附加7数据(Checked)
                            container_6.SpeedDetection = (recvData[0] & 0x01) == 0x01 ? true : false;
                            container_6.CanBusFail1 = (recvData[0] & 0x02) == 0x02 ? true : false;
                            container_6.CanBusFail2 = (recvData[0] & 0x04) == 0x04 ? true : false;
                            container_6.HardDifferent = (recvData[0] & 0x08) == 0x08 ? true : false;
                            container_6.EventHigh = (recvData[0] & 0x10) == 0x10 ? true : false;
                            container_6.EventMid = (recvData[0] & 0x20) == 0x20 ? true : false;
                            container_6.EventLow = (recvData[0] & 0x40) == 0x40 ? true : false;
                            container_6.CanASPEnable = (recvData[0] & 0x80) == 0x80 ? true : false;

                            container_6.BCPLowA = (recvData[1] & 0x01) == 0x01 ? true : false;
                            container_6.BCPLowB = (recvData[1] & 0x02) == 0x02 ? true : false;
                            container_6.BCPLowC = (recvData[1] & 0x04) == 0x04 ? true : false;

                            container_6.UnixHour = recvData[2] * 256 + recvData[3];
                            container_6.UnixMinute = recvData[4] * 256 + recvData[5];
                            container_6.UnixTimeValid = (recvData[6] & 0x20) == 0x20 ? true : false;

                            #endregion
                            break;
                        case 9:
                            #region 1车附加9数据(Checked)
                            container_6.Tc2 = recvData[0] * 256 + recvData[1];
                            container_6.Mp2 = recvData[2] * 256 + recvData[3];
                            container_6.M2 = recvData[4] * 256 + recvData[5];
                            container_6.Tc2Valid = (recvData[6] & 0x01) == 0x01 ? true : false;
                            container_6.Mp2Valid = (recvData[6] & 0x02) == 0x02 ? true : false;
                            container_6.M2Valid = (recvData[6] & 0x04) == 0x04 ? true : false;
                            container_6.CanWheelInputCondition = (recvData[6] & 0x08) == 0x08 ? true : false;
                            #endregion
                            break;
                        default:
                            break;
                    }
                    #endregion
                    break;

                case 0x0a:
                    switch (canIdLow)
                    {
                        case 1:
                            container_1.Tc1Stander = recvData[0] * 256 + recvData[1];
                            container_1.ConfirmDownload = recvData[4] == 0xAA;
                            container_1.CPUAddr = recvData[5];
                            container_1.SoftwareVersionCPU = recvData[7];
                            container_1.SoftwareVersionEP = recvData[6];
                            break;
                        case 2:
                            container_2.CPUAddr = recvData[5];
                            container_2.SoftwareVersionCPU = recvData[7];
                            container_2.SoftwareVersionEP = recvData[6];
                            break;
                        case 3:
                            container_3.CPUAddr = recvData[5];
                            container_3.SoftwareVersionCPU = recvData[7];
                            container_3.SoftwareVersionEP = recvData[6];
                            break;
                        case 4:
                            container_4.CPUAddr = recvData[5];
                            container_4.SoftwareVersionCPU = recvData[7];
                            container_4.SoftwareVersionEP = recvData[6];
                            break;
                        case 5:
                            container_5.CPUAddr = recvData[5];
                            container_5.SoftwareVersionCPU = recvData[7];
                            container_5.SoftwareVersionEP = recvData[6];
                            break;
                        case 6:
                            container_6.Tc1Stander = recvData[0] * 256 + recvData[1];
                            container_6.ConfirmDownload = recvData[4] == 0xAA;
                            container_6.CPUAddr = recvData[5];
                            container_6.SoftwareVersionCPU = recvData[7];
                            container_6.SoftwareVersionEP = recvData[6];
                            break;
                        default:
                            break;
                    }
                    break;

            }
            #endregion
            if (type == FormatType.HISTORY && command == FormatCommand.OK)
            {
                data_1.X = index;
                data_2.X = index;
                data_3.X = index;
                data_4.X = index;
                data_5.X = index;
                data_6.X = index;

                data_1.LifeSig = 1;
                data_2.LifeSig = 2;
                data_3.LifeSig = 3;
                data_4.LifeSig = 4;
                data_5.LifeSig = 5;
                data_6.LifeSig = 6;

                data_1.SpeedA1Shaft1 = 60;

                history.Containers_1.Add(data_1);
                history.Containers_2.Add(data_2);
                history.Containers_3.Add(data_3);
                history.Containers_4.Add(data_4);
                history.Containers_5.Add(data_5);
                history.Containers_6.Add(data_6);
            }
        }


        #region 更新所有已实例化的窗口的UI
        /// <summary>
        /// 更新主UI
        /// </summary>
        /// <param name="mainDevDataContains">需要向全体窗口通知的数据DTO</param>
        private void updateUIMethod(MainDevDataContains mainDevDataContains)
        {
            //MainDashboard.slider.Value = (mainDevDataContainers.SpeedA1Shaft1 + mainDevDataContainers.SpeedA1Shaft2) / 2;
            this.Dispatcher.Invoke(new Action(() => {
                MainDashboard.slider.Value = (container_1.SpeedA1Shaft1 + container_1.SpeedA1Shaft2) / 2;
            }));
            if(detailWindowCar1 != null)
            {
                detailWindowCar1.UpdateData(container_1);
            }
            if(slaveDetailWindowCar2 != null)
            {
                slaveDetailWindowCar2.UpdateData(container_2);
            }
            if (slaveDetailWindowCar3 != null)
            {
                slaveDetailWindowCar3.UpdateData(container_3);
            }
            if (slaveDetailWindowCar4 != null)
            {
                slaveDetailWindowCar4.UpdateData(container_4);
            }
            if (slaveDetailWindowCar5 != null)
            {
                slaveDetailWindowCar5.UpdateData(container_5);
            }
            if (slaveDetailWindowCar6 != null)
            {
                slaveDetailWindowCar6.UpdateData(container_6);
            }
            if (speedChartWindow != null)
            {
                speedChartWindow.UpdateData(container_1, container_2, container_3, container_4, container_5, container_6);
            }
            if (pressureChartWindow != null)
            {
                pressureChartWindow.UpdateData(container_1, container_2, container_3, container_4, container_5, container_6);
            }
            if (otherWindow != null)
            {
                otherWindow.UpdateData(container_1, container_2, container_3, container_4, container_5, container_6);
            }
            if (overviewWindow != null)
            {
                overviewWindow.UpdateData(container_1, container_2, container_3, container_4, container_5, container_6);
            }
            if (chartWindow != null)
            {
                chartWindow.UpdateData(container_1, container_2, container_3, container_4, container_5, container_6);
            }
            if (singleChartWindow != null)
            {
                singleChartWindow.UpdateData(container_1, container_2, container_3, container_4, container_5, container_6);
            }
            if (antiskid_Display_Window != null)
            {
                antiskid_Display_Window.UpdateData(container_1, container_2, container_3, container_4, container_5, container_6);
            }
        }
        #endregion

        #region 按键事件处理器，根据事件发出控件的 Name 属性来决定动作
        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender.Equals(trainHeaderFirstBtn))
            {
                if (detailWindowCar1 == null)
                {
                    detailWindowCar1 = new DetailWindow(container_1, "EBCU1");
                    detailWindowCar1.CloseWindowEvent += OtherWindowClosedHandler;
                }
                detailWindowCar1.Show();
            }
            else if (sender.Equals(trainHeaderSecondBtn))
            {
                if (slaveDetailWindowCar2 == null)
                {
                    slaveDetailWindowCar2 = new SlaveDetailWindow(container_2, "EBCU2");
                    slaveDetailWindowCar2.CloseWindowEvent += OtherWindowClosedHandler;
                }
                slaveDetailWindowCar2.Show();
            }
            else if (sender.Equals(trainMiddleFirstBtn))
            {
                if (slaveDetailWindowCar3 == null)
                {
                    slaveDetailWindowCar3 = new SlaveDetailWindow(container_3, "EBCU3");
                    slaveDetailWindowCar3.CloseWindowEvent += OtherWindowClosedHandler;
                }
                slaveDetailWindowCar3.Show();
            }
            else if (sender.Equals(trainMiddleSecondBtn))
            {
                if (slaveDetailWindowCar4 == null)
                {
                    slaveDetailWindowCar4 = new SlaveDetailWindow(container_4, "EBCU4");
                    slaveDetailWindowCar4.CloseWindowEvent += OtherWindowClosedHandler;
                }
                slaveDetailWindowCar4.Show();
            }
            else if (sender.Equals(trainTailFirstBtn))
            {
                if (slaveDetailWindowCar5 == null)
                {
                    slaveDetailWindowCar5 = new SlaveDetailWindow(container_5, "EBCU5");
                    slaveDetailWindowCar5.CloseWindowEvent += OtherWindowClosedHandler;
                }
                slaveDetailWindowCar5.Show();
            }
            else if (sender.Equals(trainTailSecondBtn))
            {
                if (slaveDetailWindowCar6 == null)
                {
                    slaveDetailWindowCar6 = new DetailWindow(container_6, "EBCU6");
                    slaveDetailWindowCar6.CloseWindowEvent += OtherWindowClosedHandler;
                }
                slaveDetailWindowCar6.Show();
            }
            else if (sender.Equals(car1View))
            {
                if (detailWindowCar1 == null)
                {
                    detailWindowCar1 = new DetailWindow(container_1, "EBCU1");
                    detailWindowCar1.CloseWindowEvent += OtherWindowClosedHandler;
                }
                detailWindowCar1.Show();
            }
            else if (sender.Equals(car2View))
            {
                if (slaveDetailWindowCar2 == null)
                {
                    slaveDetailWindowCar2 = new SlaveDetailWindow(container_2, "EBCU2");
                    slaveDetailWindowCar2.CloseWindowEvent += OtherWindowClosedHandler;
                }
                slaveDetailWindowCar2.Show();
            }
            else if (sender.Equals(car3View))
            {
                if (slaveDetailWindowCar3 == null)
                {
                    slaveDetailWindowCar3 = new SlaveDetailWindow(container_3, "EBCU3");
                    slaveDetailWindowCar3.CloseWindowEvent += OtherWindowClosedHandler;
                }
                slaveDetailWindowCar3.Show();
            }
            else if (sender.Equals(car4View))
            {
                if (slaveDetailWindowCar4 == null)
                {
                    slaveDetailWindowCar4 = new SlaveDetailWindow(container_4, "EBCU4");
                    slaveDetailWindowCar4.CloseWindowEvent += OtherWindowClosedHandler;
                }
                slaveDetailWindowCar4.Show();
            }
            else if (sender.Equals(car5View))
            {
                if (slaveDetailWindowCar5 == null)
                {
                    slaveDetailWindowCar5 = new SlaveDetailWindow(container_5, "EBCU5");
                    slaveDetailWindowCar5.CloseWindowEvent += OtherWindowClosedHandler;
                }
                slaveDetailWindowCar5.Show();
            }
            else if (sender.Equals(car6View))
            {
                if (slaveDetailWindowCar6 == null)
                {
                    slaveDetailWindowCar6 = new DetailWindow(container_6, "EBCU6");
                    slaveDetailWindowCar6.CloseWindowEvent += OtherWindowClosedHandler;
                }
                slaveDetailWindowCar6.Show();
            }
            else if (sender.Equals(nodeViewItem))
            {
                NodeWindow nodeWindow = new NodeWindow(container_1.LifeSig, container_2.LifeSig, container_3.LifeSig, container_4.LifeSig, container_5.LifeSig, container_6.LifeSig);
                nodeWindow.Show();
            }
            else if (sender.Equals(wheelDiaItem))
            {
                ParameterSetWindow parameterSetWindow = new ParameterSetWindow();
                parameterSetWindow.ShowDialog();
            }
            else if (sender.Equals(uploadItem))
            {
                DownloadExe();
            }
            else if (sender.Equals(closeBtn))
            {
                CloseDevice();
                App.Current.Shutdown();
            }
            else if (sender.Equals(showSpeedChartItem))
            {
                //显示实时曲线窗体
                if (speedChartWindow == null)
                {
                    speedChartWindow = new RealTimeSpeedChartWindow();
                    speedChartWindow.CloseWindowEvent += OtherWindowClosedHandler;
                }
                speedChartWindow.Show();
            }
            else if (sender.Equals(showPressureChartItem))
            {
                if (pressureChartWindow == null)
                {
                    pressureChartWindow = new RealTimePressureChartWindow();
                    pressureChartWindow.CloseWindowEvent += OtherWindowClosedHandler;
                }
                pressureChartWindow.Show();
            }
            else if (sender.Equals(showOtherChartItem))
            {
                if (otherWindow == null)
                {
                    otherWindow = new RealTimeOtherWindow();
                    otherWindow.CloseWindowEvent += OtherWindowClosedHandler;
                }
                otherWindow.Show();
            }
            else if (sender.Equals(OverViewItem))
            {
                if (overviewWindow == null)
                {
                    overviewWindow = new OverviewWindow();
                    overviewWindow.CloseWindowEvent += OtherWindowClosedHandler;
                }
                overviewWindow.Show();
            }
            else if (sender.Equals(chartViewItem))
            {
                if (chartWindow == null)
                {
                    chartWindow = new ChartWindow();
                    chartWindow.CloseWindowEvent += OtherWindowClosedHandler;
                }
                chartWindow.Show();
            }
            else if (sender.Equals(singleViewItem))
            {
                if (singleChartWindow == null)
                {
                    singleChartWindow = new SingleChart();
                    singleChartWindow.CloseWindowEvent += OtherWindowClosedHandler;
                }
                singleChartWindow.Show();
            }
            else if (sender.Equals(Antiskid_Display_Item))
            {
                if (antiskid_Display_Window == null)
                {
                    antiskid_Display_Window = new Antiskid_Display();
                    antiskid_Display_Window.CloseWindowEvent += OtherWindowClosedHandler;
                }
                antiskid_Display_Window.Show();
            }
            else if (sender.Equals(Antiskid_Setting_Item))
            {
                if (antiskid_Setting_Window == null)
                {
                    antiskid_Setting_Window = new Antiskid_Setting();
                    antiskid_Setting_Window.CloseWindowEvent += OtherWindowClosedHandler;
                }
                antiskid_Setting_Window.Show();
            }
        }

        private void CloseDevice()
        {
            //CanHelper.DeviceState res = canHelper.Close();
        }

        private void OtherWindowClosedHandler(bool winState, string name)
        {
            if ("EBCU1".Equals(name))
            {
                detailWindowCar1 = null;
            }
            else if ("EBCU2".Equals(name))
            {
                slaveDetailWindowCar2 = null;
            }
            else if ("EBCU3".Equals(name))
            {
                slaveDetailWindowCar3 = null;
            }
            else if ("EBCU4".Equals(name))
            {
                slaveDetailWindowCar4 = null;
            }
            else if ("EBCU5".Equals(name))
            {
                slaveDetailWindowCar5 = null;
            }
            else if ("EBCU6".Equals(name))
            {
                slaveDetailWindowCar6 = null;
            }
            else if ("speedChart".Equals(name))
            {
                speedChartWindow = null;
            }
            else if ("pressureChart".Equals(name))
            {
                pressureChartWindow = null;
            }
            else if ("otherChart".Equals(name))
            {
                otherWindow = null;
            }
            else if ("overview".Equals(name))
            {
                overviewWindow = null;
            }
            else if ("chart".Equals(name))
            {
                chartWindow = null;
            }
            else if ("single".Equals(name))
            {
                singleChartWindow = null;
            }
            else if ("Antiskid_Display_Window".Equals(name))
            {
                antiskid_Display_Window = null;
            }
            else if ("Antiskid_Setting_Window".Equals(name))
            {
                antiskid_Setting_Window = null;
            }
        }
        #endregion

        /// <summary>
        /// 打开外部下载程序
        /// </summary>
        private void DownloadExe()
        {
            //string path = ConfigurationManager.AppSettings["text"];
            //if (path == null || path == "")
            //{
            //    System.Windows.Forms.MessageBox.Show("外部程序路径配置错误" + path, "exe error!");
            //    return;
            //}
            //System.Windows.Forms.MessageBox.Show(path + "", "exe error!");
            if (recvThread.ThreadState == System.Threading.ThreadState.Running)
            {
                canHelper.Close();
                recvThread.Suspend();
            }
            Process.Start(@"CANJieMianMFC.exe");
        }

        /// <summary>
        /// 选择以太网为连接方式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void byEthItem_Checked(object sender, RoutedEventArgs e)
        {
            byCanItem.IsChecked = false;
            connectType = ConnectType.ETH;
        }

        /// <summary>
        /// 选择USB-CAN为连接方式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void byCanItem_Checked(object sender, RoutedEventArgs e)
        {
            byEthItem.IsChecked = false;
            connectType = ConnectType.CAN;
        }

        private void RecordFreq(object sender, RoutedEventArgs e)
        {
            if (sender.Equals(frequent16))
            {
                recordFreq = 16;
            }
            else if (sender.Equals(frequent32))
            {
                recordFreq = 32;
            }
            else if (sender.Equals(frequent64))
            {
                recordFreq = 64;
            }
            else if (sender.Equals(frequent128))
            {
                recordFreq = 128;
            }
            else if (sender.Equals(frequent256))
            {
                recordFreq = 256;
            }
            else if (sender.Equals(frequent512))
            {
                recordFreq = 512;
            }
            else if (sender.Equals(frequent1024))
            {
                recordFreq = 1024;
            }
        }

        private void resumeItem_Click(object sender, RoutedEventArgs e)
        {
            if (recvThread.ThreadState == System.Threading.ThreadState.Suspended)
            {
                canHelper.Open();
                canHelper.Init();
                canHelper.Start();
                recvThread.Resume();
            }
        }

        private void ftpDownloadItem_Click(object sender, RoutedEventArgs e)
        {
            FTPWindow fTPWindow = new FTPWindow();
            //fTPWindow.ShowDialog();
            //2018-9-19
            fTPWindow.Show();
            
        }



        

        /// <summary>
        /// 导出为excel 按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportXls_Click(object sender, RoutedEventArgs e)
        {
            txt_to_xml();
        }


        //private HistoryModel history_ForXml = new HistoryModel();
        //public void OpenFile_ForXml(string FileName)
        //{
        //    double x = 0.0;
        //    string fileName = FileName;
        //    List<byte[]> content = FileBuilding.GetFileContent(fileName);
        //    List<List<CanDTO>> canList = FileBuilding.GetCanList(content);
        //    history_ForXml.FileLength = FileBuilding.FileLength;
        //    for (int i = 0; i < canList.Count; i++)
        //    {
        //        int count = FileBuilding.CAN_MO_NUM;
        //        history_ForXml.Count = canList.Count;
        //        history_ForXml.X.Add(x);
        //        x += 0.1;
        //        for (int j = 0; j < canList[i].Count; j++)
        //        {
        //            if (--count == 0)
        //            {
        //                FormatData(canList[i][j], FormatType.HISTORY, FormatCommand.OK);
        //            }
        //            else
        //            {
        //                FormatData(canList[i][j], FormatType.HISTORY, FormatCommand.WAIT);
        //            }

        //        }
        //        index += 0.1;
        //    }
        //    HistoryDetail historyDetail_ForXml = new HistoryDetail();
        //    historyDetail_ForXml.SetHistory(history_ForXml);
        //    //historyDetail_ForXml.Show();

        //}


        public void OpenFile_1(string fileName)
        {                
            double x = 0.0;
                
            List<byte[]> content = FileBuilding.GetFileContent(fileName);
            List<List<CanDTO>> canList = FileBuilding.GetCanList(content);
            history.FileLength = FileBuilding.FileLength;
            for (int i = 0; i < canList.Count; i++)
            {
                int count = FileBuilding.CAN_MO_NUM;
                history.Count = canList.Count;
                history.X.Add(x);
                x += 0.1;
                for (int j = 0; j < canList[i].Count; j++)
                {
                    if (--count == 0)
                    {
                        FormatData(canList[i][j], FormatType.HISTORY, FormatCommand.OK);
                    }
                    else
                    {
                        FormatData(canList[i][j], FormatType.HISTORY, FormatCommand.WAIT);
                    }

                }
                index += 0.1;
            }
            HistoryDetail historyDetail = new HistoryDetail();
            historyDetail.SetHistory(history);
            historyDetail.Hide();
            //SingleChart historyChart = new SingleChart();

            //historyChart.Show();
            //historyChart.SetHistoryModel(history);
            //historyChart.PaintHistory();
            
        }

        private void Set_EBCU1_Array()
        {
            
        }
        

        /// <summary>
        /// 将指定的txt文件首先转成xml格式
        /// </summary>
        private void txt_to_xml()
        {
            Microsoft.Win32.OpenFileDialog openfile = new Microsoft.Win32.OpenFileDialog();
            openfile.DefaultExt = ".LOG";
            openfile.Filter = "Text Document (.log)|*.LOG";
            bool? openfile_result = openfile.ShowDialog();
            if (openfile_result == true)
            {
                StreamReader readFile = new StreamReader(openfile.FileName);               
            }
            else
            {
                return;
            }

            string[] lines = File.ReadAllLines(openfile.FileName);

            OpenFile_1(openfile.FileName);

            // 对应xml中的name属性
            List<string> info = new List<string>();
            info.Add("时间");

            XmlDocument header_xml = new XmlDocument();
            header_xml.Load("header.xml");
            XmlNode header_root = header_xml.SelectSingleNode("root");
            XmlNodeList header_list = header_root.ChildNodes;

            object[,] objData_Header = new object[1, header_list.Count];
            foreach (var item in header_list)
            {
                XmlElement header_item = (XmlElement)item;
                string header_content = header_item.InnerText;
                info.Add(header_content);
            }

            string[] info_Array = info.ToArray();

            // 下面构建xml文档
            StringBuilder sb = new StringBuilder();
            sb.Append(@"<?xml version='1.0' encoding='UTF-8'?>").Append(Environment.NewLine);
            sb.Append("<data>").Append(Environment.NewLine);
            //for (int i = 0; i < lines.Length; i++)
            //{
            //    sb.Append("\t" + "<row>").Append(Environment.NewLine);
                
            //    for (int j = 0; j < info_Array.Length; j++)
            //    {
                    
            //        sb.Append("\t\t" + "<column>").Append(Environment.NewLine);
            //        sb.Append("\t\t\t" + "<name>" + info_Array[j] + "</name>").Append(Environment.NewLine);
            //        sb.Append("\t\t\t" + "<value>" + history.Containers_1[i].dateTime + "</value>").Append(Environment.NewLine);
            //        sb.Append("\t\t" + "</column>").Append(Environment.NewLine);
            //    }
                
            //    sb.Append("\t" + "</row>").Append(Environment.NewLine);
            //}
            for(int i = 0; i < lines.Length; i++)
            {
                sb.Append("\t" + "<row>").Append(Environment.NewLine);
                sb.Append("\t\t" + "<EBCU>" + "1" + "</EBCU>").Append(Environment.NewLine);
                sb.Append("\t\t" + "<column>").Append(Environment.NewLine);
                sb.Append("\t\t\t" + "<name>" + info_Array[0] + "</name>").Append(Environment.NewLine);
                sb.Append("\t\t\t" + "<value>" + history.Containers_1[i].dateTime + "</value>").Append(Environment.NewLine);
                sb.Append("\t\t" + "</column>").Append(Environment.NewLine);

                sb.Append("\t\t" + "<EBCU>" + "1" + "</EBCU>").Append(Environment.NewLine);
                sb.Append("\t\t" + "<column>").Append(Environment.NewLine);
                sb.Append("\t\t\t" + "<name>" + info_Array[1] + "</name>").Append(Environment.NewLine);
                sb.Append("\t\t\t" + "<value>" + history.Containers_1[i].LifeSig + "</value>").Append(Environment.NewLine);
                sb.Append("\t\t" + "</column>").Append(Environment.NewLine);

                sb.Append("\t\t" + "<EBCU>" + "2" + "</EBCU>").Append(Environment.NewLine);
                sb.Append("\t\t" + "<column>").Append(Environment.NewLine);
                sb.Append("\t\t\t" + "<name>" + info_Array[1] + "</name>").Append(Environment.NewLine);
                sb.Append("\t\t\t" + "<value>" + history.Containers_2[i].LifeSig + "</value>").Append(Environment.NewLine);
                sb.Append("\t\t" + "</column>").Append(Environment.NewLine);


                sb.Append("\t\t" + "<EBCU>" + "3" + "</EBCU>").Append(Environment.NewLine);
                sb.Append("\t\t" + "<column>").Append(Environment.NewLine);
                sb.Append("\t\t\t" + "<name>" + info_Array[1] + "</name>").Append(Environment.NewLine);
                sb.Append("\t\t\t" + "<value>" + history.Containers_3[i].LifeSig + "</value>").Append(Environment.NewLine);
                sb.Append("\t\t" + "</column>").Append(Environment.NewLine);

                sb.Append("\t\t" + "<EBCU>" + "4" + "</EBCU>").Append(Environment.NewLine);
                sb.Append("\t\t" + "<column>").Append(Environment.NewLine);
                sb.Append("\t\t\t" + "<name>" + info_Array[1] + "</name>").Append(Environment.NewLine);
                sb.Append("\t\t\t" + "<value>" + history.Containers_4[i].LifeSig + "</value>").Append(Environment.NewLine);
                sb.Append("\t\t" + "</column>").Append(Environment.NewLine);

                sb.Append("\t\t" + "<EBCU>" + "5" + "</EBCU>").Append(Environment.NewLine);
                sb.Append("\t\t" + "<column>").Append(Environment.NewLine);
                sb.Append("\t\t\t" + "<name>" + info_Array[1] + "</name>").Append(Environment.NewLine);
                sb.Append("\t\t\t" + "<value>" + history.Containers_5[i].LifeSig + "</value>").Append(Environment.NewLine);
                sb.Append("\t\t" + "</column>").Append(Environment.NewLine);

                sb.Append("\t\t" + "<EBCU>" + "6" + "</EBCU>").Append(Environment.NewLine);
                sb.Append("\t\t" + "<column>").Append(Environment.NewLine);
                sb.Append("\t\t\t" + "<name>" + info_Array[1] + "</name>").Append(Environment.NewLine);
                sb.Append("\t\t\t" + "<value>" + history.Containers_6[i].LifeSig + "</value>").Append(Environment.NewLine);
                sb.Append("\t\t" + "</column>").Append(Environment.NewLine);



                sb.Append("\t" + "</row>").Append(Environment.NewLine);
            }
            sb.Append("</data>");

            string filePath = openfile.FileName;
            string filePath_1 = System.IO.Path.GetDirectoryName(filePath);
            string file_Name = System.IO.Path.GetFileNameWithoutExtension(filePath);
            string txt_to_xml_road = filePath_1 + "\\" + file_Name + ".xml";
            FileStream fs = new FileStream(txt_to_xml_road, FileMode.Create);

            //获得字节数组
            byte[] data = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(sb.ToString());

            //开始写入
            fs.Write(data, 0, data.Length);

            //清空缓冲区、关闭流
            fs.Flush();
            fs.Close();
            System.Windows.MessageBox.Show("xml创建完成！");
        }

        
        

       
    }

}
