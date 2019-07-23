using OCR_Operations.DataAcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCR_Operations.DataOperations
{
    partial class ExtractData
    {
        private double GetPreProcessValue_HeaderboxIntercept(string value_Label, int slotNumber, int labelIndex)
        {
            var value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
            value = RemoveGeneralError_SteamHood(value);
            if (value != ErrorText)
            {
                return Convert.ToDouble(value);
            }
            return 0.000;
        }
        public bool Headbox_Intercept(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeDefinitionId);
            string[] labels = new string[] { "ATG = Gap", "Thick part of Intercept Tool (A)", "Thin part of Intercept Tool (B)", "Measured distance from tool to Breast Roll (M)" };

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                if (dataPointDefinition.DataSetDefinitionId == 447)
                {
                    int labelIndex = GetLabelIndex("Apron Tip Gap");
                    int slotNumber = 0;
                    if (dataPointDefinition.Title.ToUpper().Contains("TS"))
                    {
                        slotNumber = 1;
                    }
                    else if (dataPointDefinition.Title.ToUpper().Contains("DS"))
                    {
                        slotNumber = 2;
                    }
                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else if (dataPointDefinition.Title.Contains("Target"))      //As it depend on drop down select by user
                    {
                        continue;
                    }
                    else if (dataPointDefinition.Title.Contains("Apron tip gap"))
                    {
                        value_Label = labels[0];
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);                   // To handle Values of type 2.356
                    }
                    else if (dataPointDefinition.Title.Contains("Intercept Tool A"))
                    {
                        value_Label = labels[1];
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);                   // To handle Values of type 2.356
                    }
                    else if (dataPointDefinition.Title.Contains("Intercept Tool B"))
                    {
                        value_Label = labels[2];
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);                   // To handle Values of type 2.356
                    }
                    else if (dataPointDefinition.Title.Contains("Measured Distance"))
                    {
                        value_Label = labels[3];
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);                   // To handle Values of type 2.356
                    }
                    else if (dataPointDefinition.Title.Contains("Apron tip Difference"))
                    {
                        var atgTS = GetPreProcessValue_HeaderboxIntercept(labels[0], 1, labelIndex);
                        var atgDS = GetPreProcessValue_HeaderboxIntercept(labels[0], 2, labelIndex);
                        if (atgDS != 0.000 && atgTS != 0.000)
                        {
                            value = Math.Abs(atgTS - atgDS).ToString();
                        }
                        else
                        {
                            value = ErrorText;
                        }
                    }
                    else if (dataPointDefinition.Title.Contains("Breast roll intercept") && dataPointDefinition.Title.Length < 25)
                    {
                        var A = GetPreProcessValue_HeaderboxIntercept(labels[1], slotNumber, labelIndex);
                        var B = GetPreProcessValue_HeaderboxIntercept(labels[2], slotNumber, labelIndex);
                        var M = GetPreProcessValue_HeaderboxIntercept(labels[3], slotNumber, labelIndex);
                        if (A != 0.000 && B != 0.000 && M != 0.000)
                        {
                            value = (A - B - M).ToString();
                        }
                        else
                        {
                            value = ErrorText;
                        }
                    }
                    else if (dataPointDefinition.Title.Contains("Breast roll intercept Difference"))
                    {
                        var ATS = GetPreProcessValue_HeaderboxIntercept(labels[1], 1, labelIndex);
                        var BTS = GetPreProcessValue_HeaderboxIntercept(labels[2], 1, labelIndex);
                        var MTS = GetPreProcessValue_HeaderboxIntercept(labels[3], 1, labelIndex);
                        var ADS = GetPreProcessValue_HeaderboxIntercept(labels[1], 2, labelIndex);
                        var BDS = GetPreProcessValue_HeaderboxIntercept(labels[2], 2, labelIndex);
                        var MDS = GetPreProcessValue_HeaderboxIntercept(labels[3], 2, labelIndex);
                        if (ATS != 0.000 && BTS != 0.000 && MTS != 0.000 && ADS != 0.000 && BDS != 0.000 && MDS != 0.000)
                        {
                            value = (Math.Abs(ATS - BTS - MTS - ADS + BDS + MDS)).ToString();
                        }
                        else
                        {
                            value = ErrorText;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (dataPointDefinition.DataSetDefinitionId == 448)
                {
                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else
                    {
                        continue;
                    }
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
