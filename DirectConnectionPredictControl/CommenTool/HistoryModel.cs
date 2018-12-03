using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectConnectionPredictControl.CommenTool
{
    public class HistoryModel
    {
        public List<MainDevDataContains> Containers_1 { get; set; }
        public List<SliverDataContainer> Containers_2 { get; set; }
        public List<SliverDataContainer> Containers_3 { get; set; }
        public List<SliverDataContainer> Containers_4 { get; set; }
        public List<SliverDataContainer> Containers_5 { get; set; }
        public List<MainDevDataContains> Containers_6 { get; set; }

        public List<int> ListID { get; set; }
        public int Count { get; set; }
        public List<double> X { get; set; }
        public long FileLength { get; set; }

        public HistoryModel()
        {
            Containers_1 = new List<MainDevDataContains>();
            Containers_2 = new List<SliverDataContainer>();
            Containers_3 = new List<SliverDataContainer>();
            Containers_4 = new List<SliverDataContainer>();
            Containers_5 = new List<SliverDataContainer>();
            Containers_6 = new List<MainDevDataContains>();
            Count = 0;
            X = new List<double>();
            ListID = new List<int>();            
        }


        /// <summary>
        /// 读取指定的txt文件，返回里面内容的字符串数组
        /// </summary>
        /// <returns></returns>
        public string[] ReadTxtFiles(string path)
        {
            StreamReader sr = new StreamReader(path, Encoding.Default);
            string line = sr.ReadLine();
            string[] temp = line.Split('\t');
            return temp;
        }

        /// <summary>
        /// 将历史文件导出为json文件
        /// </summary>
        public void ExportToJson()
        {
            string[] analogDataArray = ReadTxtFiles("./1架模拟量数据.txt");
            string[] analogData_2Array = ReadTxtFiles("./2架模拟量数据.txt");
            string[] analogData_3Array = ReadTxtFiles("./3架模拟量数据.txt");
            string[] analogData_4Array = ReadTxtFiles("./4架模拟量数据.txt");
            string[] analogData_5Array = ReadTxtFiles("./5架模拟量数据.txt");
            string[] analogData_6Array = ReadTxtFiles("./6架模拟量数据.txt");
            string[] antiskidDataArray = ReadTxtFiles("./防滑数据.txt");
            JsonData jsonData = new JsonData();
            JsonData temp;

            jsonData["Container_1"] = new JsonData();
            for(int i = 0; i < Containers_1.Count; i++)
            {
                temp = new JsonData();
                temp[analogDataArray[0].ToString()] = Containers_1[i].dateTime.ToString("yyyy/MM/dd HH:mm:ss");
                temp[analogDataArray[1].ToString()] = Containers_1[i].UnixHour + ":" + Containers_1[i].UnixMinute;
                temp[analogDataArray[2].ToString()] = Containers_1[i].RefSpeed;
                temp[analogDataArray[3].ToString()] = Containers_1[i].BrakeLevel;
                temp[analogDataArray[4].ToString()] = Containers_1[i].TrainBrakeForce;
                temp[analogDataArray[5].ToString()] = Containers_1[i].LifeSig;
                temp[analogDataArray[6].ToString()] = Containers_1[i].SpeedA1Shaft1;
                temp[analogDataArray[7].ToString()] = Containers_1[i].SpeedA1Shaft2;
                temp[analogDataArray[8].ToString()] = Containers_1[i].BrakeCylinderSourcePressure;
                temp[analogDataArray[9].ToString()] = Containers_1[i].AirSpring1PressureA1Car1;
                temp[analogDataArray[10].ToString()] = Containers_1[i].AirSpring2PressureA1Car1;
                temp[analogDataArray[11].ToString()] = Containers_1[i].ParkPressureA1;
                temp[analogDataArray[12].ToString()] = Containers_1[i].VldRealPressureAx1;
                temp[analogDataArray[13].ToString()] = Containers_1[i].Bcp1PressureAx1;
                temp[analogDataArray[14].ToString()] = Containers_1[i].Bcp2PressureAx2;
                temp[analogDataArray[15].ToString()] = Containers_1[i].VldPressureSetupAx1;
                temp[analogDataArray[15].ToString()] = Containers_1[i].MassA1;
                temp[analogDataArray[16].ToString()] = Containers_1[i].SelfTestSetup;
                temp[analogDataArray[17].ToString()] = Containers_1[i].VCMLifeSig;
                temp[analogDataArray[18].ToString()] = Containers_1[i].DcuLifeSig[0];
                temp[analogDataArray[19].ToString()] = Containers_1[i].DcuLifeSig[1];
                temp[analogDataArray[20].ToString()] = Containers_1[i].DcuLifeSig[2];
                temp[analogDataArray[21].ToString()] = Containers_1[i].DcuLifeSig[3];
                temp[analogDataArray[22].ToString()] = Containers_1[i].DcuEbRealValue[0];
                temp[analogDataArray[23].ToString()] = Containers_1[i].DcuMax[0];
                temp[analogDataArray[24].ToString()] = Containers_1[i].DcuEbRealValue[1];
                temp[analogDataArray[25].ToString()] = Containers_1[i].DcuMax[1];
                temp[analogDataArray[26].ToString()] = Containers_1[i].DcuEbRealValue[2];
                temp[analogDataArray[27].ToString()] = Containers_1[i].DcuMax[2];
                temp[analogDataArray[28].ToString()] = Containers_1[i].DcuEbRealValue[3];
                temp[analogDataArray[29].ToString()] = Containers_1[i].DcuMax[3];
                temp[analogDataArray[30].ToString()] = Containers_1[i].AbRealValue[0];
                temp[analogDataArray[31].ToString()] = Containers_1[i].AbRealValue[1];
                temp[analogDataArray[32].ToString()] = Containers_1[i].AbRealValue[2];
                temp[analogDataArray[33].ToString()] = Containers_1[i].AbRealValue[3];
                temp[analogDataArray[34].ToString()] = Containers_1[i].AbRealValue[4];
                temp[analogDataArray[35].ToString()] = Containers_1[i].AbRealValue[5].ToString();
                temp[analogDataArray[36].ToString()] = Containers_1[i].AbCapacity[0];
                temp[analogDataArray[37].ToString()] = Containers_1[i].AbCapacity[1];
                temp[analogDataArray[38].ToString()] = Containers_1[i].AbCapacity[2];
                temp[analogDataArray[39].ToString()] = Containers_1[i].AbCapacity[3];
                temp[analogDataArray[40].ToString()] = Containers_1[i].AbCapacity[4].ToString();
                temp[analogDataArray[41].ToString()] = Containers_1[i].AbCapacity[5];
                temp[analogDataArray[42].ToString()] = Containers_1[i].SoftwareVersionCPU;
                temp[analogDataArray[43].ToString()] = Containers_1[1].SoftwareVersionEP;

                temp[antiskidDataArray[0]] = Containers_1[i].SlipLvl1;
                temp[antiskidDataArray[6]] = Containers_1[i].SlipLvl2;
                temp[antiskidDataArray[12]] = Containers_1[i].AccValue1;
                temp[antiskidDataArray[18]] = Containers_1[i].AccValue2;

                jsonData["Container_1"].Add(temp);
            }

            jsonData["Container_2"] = new JsonData();
            for(int i = 0; i < Containers_2.Count; i++)
            {
                temp = new JsonData();
                temp[analogData_2Array[0]] = Containers_2[i].LifeSig;
                temp[analogData_2Array[1]] = Containers_2[i].SpeedShaft1;
                temp[analogData_2Array[2]] = Containers_2[i].SpeedShaft2;
                temp[analogData_2Array[3]] = Containers_2[i].BrakeCylinderSourcePressure;
                temp[analogData_2Array[4]] = Containers_2[i].AirSpringPressure1;
                temp[analogData_2Array[5]] = Containers_2[i].AirSpringPressure2;
                temp[analogData_2Array[6]] = Containers_2[i].VldRealPressure;
                temp[analogData_2Array[7]] = Containers_2[i].Bcp1Pressure;
                temp[analogData_2Array[8]] = Containers_2[i].Bcp2Pressure;
                temp[analogData_2Array[9]] = Containers_2[i].VldSetupPressure;
                temp[analogData_2Array[10]] = Containers_2[i].MassValue;
                temp[analogData_2Array[11]] = Containers_2[i].SelfTestSetup;
                temp[analogData_2Array[12]] = Containers_2[i].SoftwareVersionCPU;
                temp[analogData_2Array[13]] = Containers_2[1].SoftwareVersionEP;

                temp[antiskidDataArray[1]] = Containers_2[i].SlipLvl1;
                temp[antiskidDataArray[7]] = Containers_2[i].SlipLvl2;
                temp[antiskidDataArray[13]] = Containers_2[i].AccValue1;
                temp[antiskidDataArray[19]] = Containers_2[i].AccValue2;

                jsonData["Container_2"].Add(temp);
            }

            jsonData["Container_3"] = new JsonData();
            for(int i = 0; i < Containers_3.Count; i++)
            {
                temp = new JsonData();
                temp[analogData_3Array[0]] = Containers_3[i].LifeSig;
                temp[analogData_3Array[1]] = Containers_3[i].SpeedShaft1;
                temp[analogData_3Array[2]] = Containers_3[i].SpeedShaft2;
                temp[analogData_3Array[3]] = Containers_3[i].BrakeCylinderSourcePressure;
                temp[analogData_3Array[4]] = Containers_3[i].AirSpringPressure1;
                temp[analogData_3Array[5]] = Containers_3[i].AirSpringPressure2;
                temp[analogData_3Array[6]] = Containers_3[i].ParkPressure;
                temp[analogData_3Array[7]] = Containers_3[i].VldRealPressure;
                temp[analogData_3Array[8]] = Containers_3[i].Bcp1Pressure;
                temp[analogData_3Array[9]] = Containers_3[i].Bcp2Pressure;
                temp[analogData_3Array[10]] = Containers_3[i].VldSetupPressure;
                temp[analogData_3Array[11]] = Containers_3[i].MassValue;
                temp[analogData_3Array[12]] = Containers_3[i].SelfTestSetup;
                temp[analogData_3Array[13]] = Containers_3[i].SoftwareVersionCPU;
                temp[analogData_3Array[14]] = Containers_3[1].SoftwareVersionEP;

                temp[antiskidDataArray[2]] = Containers_3[i].SlipLvl1;
                temp[antiskidDataArray[8]] = Containers_3[i].SlipLvl2;
                temp[antiskidDataArray[14]] = Containers_3[i].AccValue1;
                temp[antiskidDataArray[20]] = Containers_3[i].AccValue2;

                jsonData["Container_3"].Add(temp);
            }

            jsonData["Container_4"] = new JsonData();
            for (int i = 0; i < Containers_4.Count; i++)
            {
                temp = new JsonData();
                temp[analogData_4Array[0]] = Containers_4[i].LifeSig;
                temp[analogData_4Array[1]] = Containers_4[i].SpeedShaft1;
                temp[analogData_4Array[2]] = Containers_4[i].SpeedShaft2;
                temp[analogData_4Array[3]] = Containers_4[i].BrakeCylinderSourcePressure;
                temp[analogData_4Array[4]] = Containers_4[i].AirSpringPressure1;
                temp[analogData_4Array[5]] = Containers_4[i].AirSpringPressure2;
                temp[analogData_4Array[6]] = Containers_4[i].VldRealPressure;
                temp[analogData_4Array[7]] = Containers_4[i].Bcp1Pressure;
                temp[analogData_4Array[8]] = Containers_4[i].Bcp2Pressure;
                temp[analogData_4Array[9]] = Containers_4[i].VldSetupPressure;
                temp[analogData_4Array[10]] = Containers_4[i].MassValue;
                temp[analogData_4Array[11]] = Containers_4[i].SelfTestSetup;
                temp[analogData_4Array[12]] = Containers_4[i].SoftwareVersionCPU;
                temp[analogData_4Array[13]] = Containers_4[1].SoftwareVersionEP;
                temp[analogData_4Array[14]] = Containers_4[i].ParkPressure;

                temp[antiskidDataArray[3]] = Containers_4[i].SlipLvl1;
                temp[antiskidDataArray[9]] = Containers_4[i].SlipLvl2;
                temp[antiskidDataArray[15]] = Containers_4[i].AccValue1;
                temp[antiskidDataArray[21]] = Containers_4[i].AccValue2;

                jsonData["Container_4"].Add(temp);

            }

            jsonData["Container_5"] = new JsonData();
            for(int i = 0; i < Containers_5.Count; i++)
            {
                temp = new JsonData();
                temp[analogData_5Array[0]] = Containers_5[i].LifeSig;
                temp[analogData_5Array[1]] = Containers_5[i].SpeedShaft1;
                temp[analogData_5Array[2]] = Containers_5[i].SpeedShaft2;
                temp[analogData_5Array[3]] = Containers_5[i].BrakeCylinderSourcePressure;
                temp[analogData_5Array[4]] = Containers_5[i].AirSpringPressure1;
                temp[analogData_5Array[5]] = Containers_5[i].AirSpringPressure2;
                temp[analogData_5Array[6]] = Containers_5[i].ParkPressure;
                temp[analogData_5Array[7]] = Containers_5[i].VldRealPressure;
                temp[analogData_5Array[8]] = Containers_5[i].Bcp1Pressure;
                temp[analogData_5Array[9]] = Containers_5[i].Bcp2Pressure;
                temp[analogData_5Array[10]] = Containers_5[i].VldSetupPressure;
                temp[analogData_5Array[11]] = Containers_5[i].MassValue;
                temp[analogData_5Array[12]] = Containers_5[i].SelfTestSetup;
                temp[analogData_5Array[13]] = Containers_5[i].SoftwareVersionCPU;
                temp[analogData_5Array[14]] = Containers_5[1].SoftwareVersionEP;

                temp[antiskidDataArray[4]] = Containers_5[i].SlipLvl1;
                temp[antiskidDataArray[10]] = Containers_5[i].SlipLvl2;
                temp[antiskidDataArray[16]] = Containers_5[i].AccValue1;
                temp[antiskidDataArray[22]] = Containers_5[i].AccValue2;

                jsonData["Container_5"].Add(temp);
            }

            jsonData["Container_6"] = new JsonData();
            for (int i = 0; i < Containers_6.Count; i++)
            {
                temp = new JsonData();
                temp[analogData_6Array[0]] = Containers_6[i].RefSpeed;
                temp[analogData_6Array[1]] = Containers_6[i].BrakeLevel;
                temp[analogData_6Array[2]] = Containers_6[i].TrainBrakeForce;
                temp[analogData_6Array[3]] = Containers_6[i].LifeSig;
                temp[analogData_6Array[4]] = Containers_6[i].SpeedA1Shaft1;
                temp[analogData_6Array[5]] = Containers_6[i].SpeedA1Shaft2;
                temp[analogData_6Array[6]] = Containers_6[i].BrakeCylinderSourcePressure;
                temp[analogData_6Array[7]] = Containers_6[i].AirSpring1PressureA1Car1;
                temp[analogData_6Array[8]] = Containers_6[i].AirSpring2PressureA1Car1;
                temp[analogData_6Array[9]] = Containers_6[i].VldRealPressureAx1;
                temp[analogData_6Array[10]] = Containers_6[i].Bcp1PressureAx1;
                temp[analogData_6Array[11]] = Containers_6[i].Bcp2PressureAx2;
                temp[analogData_6Array[12]] = Containers_6[i].VldPressureSetupAx1;
                temp[analogData_6Array[13]] = Containers_6[i].MassA1;
                temp[analogData_6Array[14]] = Containers_6[i].SelfTestSetup;
                temp[analogData_6Array[15]] = Containers_6[i].VCMLifeSig;
                temp[analogData_6Array[16]] = Containers_6[i].DcuLifeSig[0];
                temp[analogData_6Array[17]] = Containers_6[i].DcuLifeSig[1];
                temp[analogData_6Array[18]] = Containers_6[i].DcuLifeSig[2];
                temp[analogData_6Array[19]] = Containers_6[i].DcuLifeSig[3];
                temp[analogData_6Array[20]] = Containers_6[i].DcuEbRealValue[0];
                temp[analogData_6Array[21]] = Containers_6[i].DcuMax[0];
                temp[analogData_6Array[22]] = Containers_6[i].DcuEbRealValue[1];
                temp[analogData_6Array[23]] = Containers_6[i].DcuMax[1];
                temp[analogData_6Array[24]] = Containers_6[i].DcuEbRealValue[2];
                temp[analogData_6Array[25]] = Containers_6[i].DcuMax[2];
                temp[analogData_6Array[26]] = Containers_6[i].DcuEbRealValue[3];
                temp[analogData_6Array[27]] = Containers_6[i].DcuMax[3];
                temp[analogData_6Array[28]] = Containers_6[i].AbRealValue[0];
                temp[analogData_6Array[29]] = Containers_6[i].AbRealValue[1];
                temp[analogData_6Array[30]] = Containers_6[i].AbRealValue[2];
                temp[analogData_6Array[31]] = Containers_6[i].AbRealValue[3];
                temp[analogData_6Array[32]] = Containers_6[i].AbRealValue[4];
                temp[analogData_6Array[33]] = Containers_6[i].AbRealValue[5];
                temp[analogData_6Array[34]] = Containers_6[i].AbCapacity[0];
                temp[analogData_6Array[35]] = Containers_6[i].AbCapacity[1];
                temp[analogData_6Array[36]] = Containers_6[i].AbCapacity[2];
                temp[analogData_6Array[37]] = Containers_6[i].AbCapacity[3];
                temp[analogData_6Array[38]] = Containers_6[i].AbCapacity[4];
                temp[analogData_6Array[39]] = Containers_6[i].AbCapacity[5];
                temp[analogData_6Array[40]] = Containers_6[i].SoftwareVersionCPU;
                temp[analogData_6Array[41]] = Containers_6[1].SoftwareVersionEP;

                temp[antiskidDataArray[5]] = Containers_6[i].SlipLvl1;
                temp[antiskidDataArray[11]] = Containers_6[i].SlipLvl2;
                temp[antiskidDataArray[17]] = Containers_6[i].AccValue1;
                temp[antiskidDataArray[23]] = Containers_6[i].AccValue2;

                jsonData["Container_6"].Add(temp);
            }

            string json = jsonData.ToJson();
            string path = "./Test.json";

            StreamWriter sw = new StreamWriter(path);
            sw.Write(json);
            sw.Close();
            sw.Dispose();
        }

        public object[] GetProperties()
        {
            List<object> temp = new List<object>();
            MainDevDataContains mainContainer = new MainDevDataContains();
            Type mainContainerType = mainContainer.GetType();
            foreach(System.Reflection.PropertyInfo p in mainContainerType.GetProperties())
            {
                temp.Add(p.Name);
            }
            return temp.ToArray();
        }

        #region 合成数组

        #region 滑行等级_1
        public string[] GetSlip1_1()
        {
            List<string> temp = new List<string>();
            for(int i = 0; i < Containers_1.Count; i++)
            {
                temp.Add(Containers_1[i].SlipLvl1.ToString());
            }
            return temp.ToArray();
        }

        public string[] GetSlip1_2()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < Containers_2.Count; i++)
            {
                temp.Add(Containers_2[i].SlipLvl1.ToString());
            }
            return temp.ToArray();
        }

        public string[] GetSlip1_3()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < Containers_3.Count; i++)
            {
                temp.Add(Containers_3[i].SlipLvl1.ToString());
            }
            return temp.ToArray();
        }

        public string[] GetSlip1_4()
        {
            List<string> temp = new List<string>();
            for(int i = 0; i < Containers_4.Count; i++)
            {
                temp.Add(Containers_4[i].SlipLvl1.ToString());
            }
            return temp.ToArray();
        }

        public string[] GetSlip1_5()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < Containers_5.Count; i++)
            {
                temp.Add(Containers_5[i].SlipLvl1.ToString());
            }
            return temp.ToArray();
        }

        public string[] GetSlip1_6()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < Containers_6.Count; i++)
            {
                temp.Add(Containers_6[i].SlipLvl1.ToString());
            }
            return temp.ToArray();
        }
        #endregion
             
        #region 滑行等级_2
        public string[] GetSlip2_1()
        {
            List<string> temp = new List<string>();
            for(int i = 0; i < Containers_1.Count; i++)
            {
                temp.Add(Containers_1[i].SlipLvl2.ToString());
            }
            return temp.ToArray();
        }

        public string[] GetSlip2_2()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < Containers_2.Count; i++)
            {
                temp.Add(Containers_2[i].SlipLvl2.ToString());
            }
            return temp.ToArray();
        }

        public string[] GetSlip2_3()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < Containers_3.Count; i++)
            {
                temp.Add(Containers_3[i].SlipLvl2.ToString());
            }
            return temp.ToArray();
        }

        public string[] GetSlip2_4()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < Containers_4.Count; i++)
            {
                temp.Add(Containers_4[i].SlipLvl2.ToString());
            }
            return temp.ToArray();
        }

        public string[] GetSlip2_5()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < Containers_5.Count; i++)
            {
                temp.Add(Containers_5[i].SlipLvl2.ToString());
            }
            return temp.ToArray();
        }

        public string[] GetSlip2_6()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < Containers_6.Count; i++)
            {
                temp.Add(Containers_6
[i].SlipLvl2.ToString());
            }
            return temp.ToArray();
        }
        #endregion

        #region 各车轴一减速度
        public string[] GetAcc1_1()
        {
            List<string> temp = new List<string>();
            for(int i = 0; i < Containers_1.Count; i++)
            {
                temp.Add(Containers_1[i].AccValue1.ToString());
            }
            return temp.ToArray();
        }

        public string[] GetAcc1_2()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < Containers_2.Count; i++)
            {
                temp.Add(Containers_2[i].AccValue1.ToString());
            }
            return temp.ToArray();
        }

        public string[] GetAcc1_3()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < Containers_3.Count; i++)
            {
                temp.Add(Containers_3[i].AccValue1.ToString());
            }
            return temp.ToArray();
        }

        public string[] GetAcc1_4()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < Containers_4.Count; i++)
            {
                temp.Add(Containers_4[i].AccValue1.ToString());
            }
            return temp.ToArray();
        }

        public string[] GetAcc1_5()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < Containers_5.Count; i++)
            {
                temp.Add(Containers_5[i].AccValue1.ToString());
            }
            return temp.ToArray();
        }

        public string[] GetAcc1_6()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < Containers_6.Count; i++)
            {
                temp.Add(Containers_6[i].AccValue1.ToString());
            }
            return temp.ToArray();
        }

        #endregion

        #region 各车轴二减速度
        public string[] GetAcc2_1()
        {
            List<string> temp = new List<string>();
            for(int i = 0; i < Containers_1.Count; i++)
            {
                temp.Add(Containers_1[i].AccValue2.ToString());
            }
            return temp.ToArray();
        }

        public string[] GetAcc2_2()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < Containers_2.Count; i++)
            {
                temp.Add(Containers_2[i].AccValue2.ToString());
            }
            return temp.ToArray();
        }

        public string[] GetAcc2_3()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < Containers_3.Count; i++)
            {
                temp.Add(Containers_3[i].AccValue2.ToString());
            }
            return temp.ToArray();
        }

        public string[] GetAcc2_4()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < Containers_4.Count; i++)
            {
                temp.Add(Containers_4[i].AccValue2.ToString());
            }
            return temp.ToArray();
        }

        public string[] GetAcc2_5()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < Containers_5.Count; i++)
            {
                temp.Add(Containers_5[i].AccValue2.ToString());
            }
            return temp.ToArray();
        }

        public string[] GetAcc2_6()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < Containers_6.Count; i++)
            {
                temp.Add(Containers_6[i].AccValue2.ToString());
            }
            return temp.ToArray();
        }


        #endregion
        #endregion


    }
}
