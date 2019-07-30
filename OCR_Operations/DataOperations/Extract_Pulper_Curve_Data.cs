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
        private string GetFrontLabelValue_PulperCurve(string label, int labelIndex)
        {
            int index_Of_Field = OCRText.IndexOf(label, labelIndex);
            int countOfField = label.Length;
            while (index_Of_Field == -1)
            {
                label = label.Remove(0, 1);
                countOfField = label.Length;
                index_Of_Field = OCRText.IndexOf(label, labelIndex);
                //Error Handle if label not exist 
                if (countOfField <= 2)
                {
                    return ErrorText;
                }
            }
            int startIndex = OCRText.IndexOf("\r\n", index_Of_Field) + 2;
            int lastIndex = OCRText.IndexOf("\r\n", startIndex);
            if (startIndex == 1 || startIndex >= OCRText.Length)
            {
                return ErrorText;
            }
            if (lastIndex == -1)
            {
                return OCRText.Substring(startIndex);
            }
            int value_Length = lastIndex - startIndex;
            string value = OCRText.Substring(startIndex, value_Length); // add +2 for /r/n characters
            return value;
        }
        private string RemoveTypeError_PulperCurve(string value)
        {
            value = value.Replace(" ", "");
            if (value.ToUpper().Contains("SOFT"))
            {
                return "Softwood";
            }
            else if (value.ToUpper().Contains("HARD"))
            {
                return "Hardwood";
            }
            return ErrorText;
        }
        private double GetPreProcessValue_FrontLabel_PulperCurve(string value_Label, int slotNumber, int labelIndex)
        {
            var value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
            value = RemoveGeneralError_SteamHood(value);
            if (value != ErrorText)
            {
                return Convert.ToDouble(value);
            }
            return 0.000;
        }
        public bool Pulper_Curve(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeEntryId);

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                int labelIndex = GetLabelIndex("Pulper Curve Data Sheet");
                if (dataPointDefinition.DataSetDefinitionId == 451)  //Check NSK, ESK fields
                {
                    if (dataPointDefinition.Title.Contains("Puple Type Option"))
                    {
                        value_Label = "Pulp Type";
                        value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                        value = RemoveTypeError_PulperCurve(value);
                    }
                    else if (dataPointDefinition.Title.Contains("Time") || dataPointDefinition.Title.Contains("Consistency"))
                    {
                        value_Label = dataPointDefinition.Title;
                        value = GetFrontLabelValue_PulperCurve(value_Label, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (dataPointDefinition.DataSetDefinitionId == 452)
                {
                    if (dataPointDefinition.Title.Contains("Ten/PFR"))
                    {
                        int labelSlotNum = Convert.ToInt32(dataPointDefinition.Title.Substring(dataPointDefinition.Title.IndexOf("#") + 1, 1));
                        double[] avgArray = new double[2];
                        for (int slot = 1; slot < 3; slot++)
                        {
                            double localValue = 0;
                            for (int i = 1; i < 3; i++)
                            {
                                labelIndex = GetFrontLabelIndex_SlotWise("Sample #" + i, labelSlotNum);
                                value_Label = "Sample #" + i;
                                localValue += GetPreProcessValue_FrontLabel_PulperCurve(value_Label, slot, labelIndex);
                            }
                            avgArray[slot - 1] = Math.Round(localValue / 2, 1);
                        }
                        value = Math.Round(avgArray[0] / avgArray[1], 1).ToString();
                    }
                    else if (dataPointDefinition.Title.Contains("Avg"))
                    {
                        int slotNumber = 0;
                        if (dataPointDefinition.Title.Contains("Tensile"))
                        {
                            slotNumber = 1;
                        }
                        else if (dataPointDefinition.Title.Contains("PFR"))
                        {
                            slotNumber = 2;
                        }
                        else if (dataPointDefinition.Title.Contains("Fines"))
                        {
                            slotNumber = 3;
                        }
                        double localValue = 0;
                        int labelSlotNum = Convert.ToInt32(dataPointDefinition.Title.Substring(dataPointDefinition.Title.IndexOf("#") + 1, 1));
                        for (int i = 1; i < 3; i++)
                        {
                            labelIndex = GetFrontLabelIndex_SlotWise("Sample #" + i, labelSlotNum);
                            value_Label = "Sample #" + i;
                            localValue += GetPreProcessValue_FrontLabel_PulperCurve(value_Label, slotNumber, labelIndex);
                        }
                        value = Math.Round(localValue / 2, 1).ToString();
                    }
                    else if (dataPointDefinition.Title.Contains("Tensile "))
                    {
                        char sampleNum = dataPointDefinition.Title.Last();
                        int labelSlotNum = Convert.ToInt32(dataPointDefinition.Title.Substring(dataPointDefinition.Title.IndexOf("#") + 1, 1));
                        labelIndex = GetFrontLabelIndex_SlotWise("Sample #" + sampleNum, labelSlotNum);
                        value_Label = "Sample #" + sampleNum;
                        int slotNumber = 1;
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else if (dataPointDefinition.Title.Contains("PFR "))
                    {
                        char sampleNum = dataPointDefinition.Title.Last();
                        int labelSlotNum = Convert.ToInt32(dataPointDefinition.Title.Substring(dataPointDefinition.Title.IndexOf("#") + 1, 1));
                        labelIndex = GetFrontLabelIndex_SlotWise("Sample #" + sampleNum, labelSlotNum);
                        value_Label = "Sample #" + sampleNum;
                        int slotNumber = 2;
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else if (dataPointDefinition.Title.Contains("%Fines "))
                    {
                        char sampleNum = dataPointDefinition.Title.Last();
                        int labelSlotNum = Convert.ToInt32(dataPointDefinition.Title.Substring(dataPointDefinition.Title.IndexOf("#") + 1, 1));
                        labelIndex = GetFrontLabelIndex_SlotWise("Sample #" + sampleNum, labelSlotNum);
                        value_Label = "Sample #" + sampleNum;
                        int slotNumber = 3;
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else if (dataPointDefinition.Title.Contains("Consistency "))
                    {
                        char sampleNum = dataPointDefinition.Title.Last();
                        int labelSlotNum = Convert.ToInt32(dataPointDefinition.Title.Substring(dataPointDefinition.Title.IndexOf("#") + 1, 1));
                        labelIndex = GetFrontLabelIndex_SlotWise("Sample #" + sampleNum, labelSlotNum);
                        value_Label = "Sample #" + sampleNum;
                        int slotNumber = 4;
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else if (dataPointDefinition.Title.Contains("Smaple Time"))
                    {
                        labelIndex = GetLabelIndex("Test Conditions");
                        char timeLabel = dataPointDefinition.DPShortName.Last();
                        value_Label = "Time #" + timeLabel;
                        int slotNumber = 2;
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
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
