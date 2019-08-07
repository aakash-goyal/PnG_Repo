using OCR_Operations.DataAcess;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCR_Operations.DataOperations
{
    partial class ExtractData
    {
        public bool PD_Deckle_Position(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeEntryId);
            OCRText = OCRText.Replace("\r\nT5", "\r\nTS").Replace("\r\nD5", "\r\nDS").Replace("\r\nTS Decide to face\r\n", "\r\nTS Deckle to face\r\n").Replace("\r\nPD Open Area cold)\r\n", "\r\nPD Open Area (cold)\r\n").Replace("\r\nPO3\r\n", "\r\nPD3\r\n");
            Hashtable headValues = new Hashtable();

            foreach (var dataPointDefinition in dataPointDefinitions.Where(item => item.IsCalculated == 0))
            {
                int labelIndex = GetLabelIndex("Headbox Data");
                if (dataPointDefinition.Title.ToUpper().Contains("TEXT") || dataPointDefinition.Title.ToUpper().Contains("EMPTY") || dataPointDefinition.Title.ToUpper().Contains("ACTUALS MIN") || dataPointDefinition.Title.ToUpper().Contains("ACTUALS MAX"))
                {
                    continue;
                }
                else if (dataPointDefinition.DataSetDefinitionId == 1511)
                {
                    value_Label = dataPointDefinition.Title.Replace("Headbox data - ", "");
                    value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                    value = RemoveGeneralError_SteamHood(value);
                }
                else if (dataPointDefinition.DataSetDefinitionId == 1512)
                {
                    labelIndex = GetLabelIndex("PD1");
                    value_Label = dataPointDefinition.Title.Replace("PD1 - ", "").Replace("Actuals ", "");
                    value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                    value = RemoveGeneralError_SteamHood(value);
                }
                else if (dataPointDefinition.DataSetDefinitionId == 1513)
                {
                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else
                    {
                        labelIndex = GetLabelIndex("PD2");
                        value_Label = dataPointDefinition.Title.Replace("PD2 - ", "").Replace("Actuals ", "");
                        if (value_Label.Contains("Deckle to face"))
                        {
                            labelIndex = GetFrontLabelIndex_SteamHood(value_Label, 2, labelIndex);
                        }
                        value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                }
                else if (dataPointDefinition.DataSetDefinitionId == 1514)
                {
                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else
                    {
                        labelIndex = GetLabelIndex("PD3");
                        value_Label = dataPointDefinition.Title.Replace("PD3 - ", "").Replace("Actuals ", "");
                        value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                }
                else if (dataPointDefinition.DataSetDefinitionId == 1515)
                {
                    value = dataPointDefinition.ConstantValue;
                }
                else
                {
                    continue;
                }
                if (value != ErrorText)
                {
                    headValues.Add(dataPointDefinition.Id, value);
                }
                else
                {
                    headValues.Add(dataPointDefinition.Id, 0);
                }
                // Add to Data List
                cpeDataList.Add(CreateCpeEntryDataPoint(value, dataPointDefinition.Id, cpeDefinitionId, cpeEntryId));
            }
            foreach (var dataPointDefinition in dataPointDefinitions.Where(item => item.IsCalculated == 1 && (item.DataSetDefinitionId == 1511 || item.DataSetDefinitionId == 1515)))
            {
                if (dataPointDefinition.Title.Contains("Headbox Width"))
                {
                    double dsPos = Convert.ToDouble(headValues[25704].ToString());
                    double tsPos = Convert.ToDouble(headValues[25701].ToString());
                    value = Math.Round((dsPos - tsPos), 3).ToString();
                }
                else if (dataPointDefinition.Title.Contains("Sheet growth"))
                {
                    double shtWidth = Convert.ToDouble(headValues[25710].ToString());
                    double dsPos = Convert.ToDouble(headValues[25704].ToString());
                    double tsPos = Convert.ToDouble(headValues[25701].ToString());
                    double hdWidth = Math.Round((dsPos - tsPos), 3);
                    value = Math.Round((shtWidth - hdWidth), 3).ToString();
                }
                else if (dataPointDefinition.DPShortName.Contains("SHEET_TS"))
                {
                    double shtWidth = Convert.ToDouble(headValues[25710].ToString());
                    double dsPos = Convert.ToDouble(headValues[25704].ToString());
                    double tsPos = Convert.ToDouble(headValues[25701].ToString());
                    double hdWidth = Math.Round((dsPos - tsPos), 3);
                    double shtGrowth = Math.Round((shtWidth - hdWidth), 3);
                    value = Math.Round((tsPos - (shtGrowth / 2)), 3).ToString();
                }
                else if (dataPointDefinition.DPShortName.Contains("SHEET_DS"))
                {
                    double shtWidth = Convert.ToDouble(headValues[25710].ToString());
                    double dsPos = Convert.ToDouble(headValues[25704].ToString());
                    double tsPos = Convert.ToDouble(headValues[25701].ToString());
                    double hdWidth = Math.Round((dsPos - tsPos), 3);
                    double shtGrowth = Math.Round((shtWidth - hdWidth), 3);
                    value = Math.Round((dsPos + (shtGrowth / 2)), 3).ToString();
                }
                else if (dataPointDefinition.Title.Contains("Thermal Expansion"))
                {
                    double pdTemp = Convert.ToDouble(headValues[25703].ToString());
                    if (pdTemp > 0)
                    {
                        value = Math.Round((0.75 * ((pdTemp - 70) / 350)), 3).ToString();
                    }
                    value = "";
                }
                else
                {
                    continue;
                }
                if (value != ErrorText && value != "")
                {
                    headValues.Add(dataPointDefinition.Id, value);
                }
                else
                {
                    headValues.Add(dataPointDefinition.Id, 0);
                }
                // Add to Data List
                cpeDataList.Add(CreateCpeEntryDataPoint(value, dataPointDefinition.Id, cpeDefinitionId, cpeEntryId));
            }
            foreach (var dataPointDefinition in dataPointDefinitions.Where(item => item.IsCalculated == 1 && (item.DataSetDefinitionId > 1511 && item.DataSetDefinitionId < 1515)))
            {
                int[] dataPointIds;
                if (dataPointDefinition.DataSetDefinitionId == 1512)
                {
                    dataPointIds = new int[] { 25722, 25725, 25728, 25731 };
                }
                else if (dataPointDefinition.DataSetDefinitionId == 1513)
                {
                    dataPointIds = new int[] { 25755, 25758, 25761, 25764 };
                }
                else
                {
                    dataPointIds = new int[] { 25788, 25791, 25794, 25797 };
                }
                if (dataPointDefinition.Title.Contains("TS Deckle to face"))
                {
                    double trgAmt = Convert.ToDouble(headValues[25706].ToString());
                    double shtTS = Convert.ToDouble(headValues[25716].ToString());
                    double pdBase = Convert.ToDouble(headValues[dataPointIds[3]].ToString());
                    double pdthermal = Convert.ToDouble(headValues[25827].ToString());
                    value = Math.Round((shtTS + pdthermal + trgAmt - pdBase), 3).ToString();
                }
                else if (dataPointDefinition.Title.Contains("DS Deckle to face"))
                {
                    double trgAmt = Convert.ToDouble(headValues[25706].ToString());
                    double pdBase = Convert.ToDouble(headValues[dataPointIds[3]].ToString());
                    double shtDS = Convert.ToDouble(headValues[25719].ToString());
                    double pdWidth = Convert.ToDouble(headValues[dataPointIds[0]].ToString());
                    value = Math.Round((pdWidth + pdBase + trgAmt - shtDS), 3).ToString();
                }
                else if (dataPointDefinition.Title.Contains("Sheet Coverage TS - Min"))
                {
                    double pdBase = Convert.ToDouble(headValues[dataPointIds[3]].ToString());
                    if (pdBase > 0)
                    {
                        value = "0.500";
                    }
                    else
                    {
                        value = "0";
                    }
                }
                else if (dataPointDefinition.Title.Contains("Sheet Coverage TS - Max"))
                {
                    double pdBase = Convert.ToDouble(headValues[dataPointIds[3]].ToString());
                    if (pdBase > 0)
                    {
                        value = "0.750";
                    }
                    else
                    {
                        value = "0";
                    }
                }
                else if (dataPointDefinition.Title.Contains("Sheet Coverage DS - Min"))
                {
                    double pdBase = Convert.ToDouble(headValues[dataPointIds[3]].ToString());
                    double pdTS = Convert.ToDouble(headValues[dataPointIds[1]].ToString());
                    double pdDS = Convert.ToDouble(headValues[dataPointIds[2]].ToString());
                    if (pdBase > 0 || pdTS > 0 || pdDS > 0)
                    {
                        value = "0.500";
                    }
                    else
                    {
                        value = "0";
                    }
                }
                else if (dataPointDefinition.Title.Contains("Sheet Coverage DS - Max"))
                {
                    double pdBase = Convert.ToDouble(headValues[dataPointIds[3]].ToString());
                    double pdTS = Convert.ToDouble(headValues[dataPointIds[1]].ToString());
                    double pdDS = Convert.ToDouble(headValues[dataPointIds[2]].ToString());
                    if (pdBase > 0 || pdTS > 0 || pdDS > 0)
                    {
                        value = "0.750";
                    }
                    else
                    {
                        value = "0";
                    }
                }
                else if (dataPointDefinition.Title.Contains("Sheet Coverage TS"))
                {
                    double shtTS = Convert.ToDouble(headValues[25716].ToString());
                    double pdthermal = Convert.ToDouble(headValues[25827].ToString());
                    double pdBase = Convert.ToDouble(headValues[dataPointIds[3]].ToString());
                    double pdTS = Convert.ToDouble(headValues[dataPointIds[1]].ToString());
                    value = Math.Round((pdBase + pdTS - pdthermal - shtTS), 3).ToString();
                }
                else if (dataPointDefinition.Title.Contains("Sheet Coverage DS"))
                {
                    double shtDS = Convert.ToDouble(headValues[25719].ToString());
                    double pdBase = Convert.ToDouble(headValues[dataPointIds[3]].ToString());
                    double pdDS = Convert.ToDouble(headValues[dataPointIds[2]].ToString());
                    double pdWidth = Convert.ToDouble(headValues[dataPointIds[0]].ToString());
                    value = Math.Round((shtDS - pdBase - pdWidth + pdDS), 3).ToString();
                }
                else if (dataPointDefinition.Title.Contains("TS Deckle to baseline (cold)"))
                {
                    double pdBase = Convert.ToDouble(headValues[dataPointIds[3]].ToString());
                    double pdTS = Convert.ToDouble(headValues[dataPointIds[1]].ToString());
                    value = Math.Round((pdBase + pdTS), 3).ToString();
                }
                else if (dataPointDefinition.Title.Contains("DS Deckle to baseline (cold)"))
                {
                    double pdBase = Convert.ToDouble(headValues[dataPointIds[3]].ToString());
                    double pdDS = Convert.ToDouble(headValues[dataPointIds[2]].ToString());
                    double pdWidth = Convert.ToDouble(headValues[dataPointIds[0]].ToString());
                    value = Math.Round((pdBase + pdWidth - pdDS), 3).ToString();
                }
                else if (dataPointDefinition.Title.Contains("Recommended PD Open Area (Cold)"))
                {
                    double pdthermal = Convert.ToDouble(headValues[25827].ToString());
                    double trgAmt = Convert.ToDouble(headValues[25706].ToString());
                    double shtWidth = Convert.ToDouble(headValues[25710].ToString());
                    value = Math.Round((shtWidth - trgAmt - trgAmt - pdthermal), 3).ToString();
                }
                else if (dataPointDefinition.Title.Contains("Recommended PD Open Area (hot)"))
                {
                    double trgAmt = Convert.ToDouble(headValues[25706].ToString());
                    double shtWidth = Convert.ToDouble(headValues[25710].ToString());
                    value = Math.Round((shtWidth - trgAmt - trgAmt), 3).ToString();
                }
                else if (dataPointDefinition.Title.Contains("PD Open Area (cold)"))
                {
                    double pdBase = Convert.ToDouble(headValues[dataPointIds[3]].ToString());
                    double pdDS = Convert.ToDouble(headValues[dataPointIds[2]].ToString());
                    double pdWidth = Convert.ToDouble(headValues[dataPointIds[0]].ToString());
                    double pdTS = Convert.ToDouble(headValues[dataPointIds[1]].ToString());
                    double dsCold = Math.Round((pdBase + pdWidth - pdDS), 3);
                    double tsCold = Math.Round((pdBase + pdTS), 3);
                    value = (dsCold - tsCold).ToString();
                }
                else
                {
                    continue;
                }
                // Add to Data List
                cpeDataList.Add(CreateCpeEntryDataPoint(value, dataPointDefinition.Id, cpeDefinitionId, cpeEntryId));
            }
            //Insert list of cpeentrydatapointvalues in to database using datainsertion class
            bool success = dataInsertion.Insert(cpeDataList);

            return success;
        }
    }
}
