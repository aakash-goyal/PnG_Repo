using OCR_Operations.DataAcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OCR_Operations.DataOperations
{
    partial class ExtractData
    {
        private string RemoveError_String_WireEdge(string value)
        {
            string target_Angle_Pattern = @"^[a-zA-Z]+[\s]?[a-zA-Z]+$";  // only alphabet space alphabets
            Regex regex = new Regex(target_Angle_Pattern);
            Match match = regex.Match(value);
            while (match.Value == "")
            {
                // If value doesn't match then return blank
                if (value.Length == 0)
                {
                    return ErrorText;
                }
                value = value.Remove((value.Length - 1), 1);
                match = regex.Match(value);
            }
            return value;
        }
        private string RemoveError_NegativeInt_WireEdge(string value)
        {
            value = value.Replace(" ", "").Replace("O", "0").Replace("S", "5").Replace("--", "-");
            if (value.Contains(","))
            {
                value = value.Replace(",", ".");
            }
            else if (value.Contains("-") && value.IndexOf("-") >= 2)
            {
                value = value.Replace("-", ".");
            }
            //Pattern to match decimal number
            string decimal_Pattern = @"^-?[0-9]*(\.\d{1,3})?$";
            Regex regex = new Regex(decimal_Pattern);
            Match match = regex.Match(value);
            while (match.Value == "")
            {
                // If value doesn't match then return blank
                if (value.Length == 0)
                {
                    return ErrorText;
                }
                value = value.Remove((value.Length - 1), 1);
                match = regex.Match(value);
            }

            return value;
        }
        public bool Wire_Edge_Trim(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeEntryId);

            OCRText = OCRText.Replace("\r\nFigure 3\r\n", "\r\n").Replace("\r\nWE Out corner\r\n", "\r\n");

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                int slotNumber = 0;
                int labelIndex = 0;
                if (dataPointDefinition.DataSetDefinitionId == 240)
                {
                    labelIndex = GetLabelIndex("System Manufacturer");
                    if (dataPointDefinition.Title.Contains("Manufacturer"))
                    {
                        value_Label = dataPointDefinition.Title;
                        value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                        value = RemoveError_String_WireEdge(value);
                    }
                    else if (dataPointDefinition.Title.Contains("Nozzle Type"))          // Error function update needed if it's value can be Dual Parallel only
                    {
                        value_Label = "Nozzle Oriface";
                        if (dataPointDefinition.IsConstantValue == 1)
                        {
                            value = dataPointDefinition.ConstantValue;
                        }
                        else if (dataPointDefinition.Title.Contains("TS Wet End"))
                        {
                            slotNumber = 2;
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                            value = RemoveError_String_WireEdge(value);
                        }
                        else if (dataPointDefinition.Title.Contains("TS Dry End"))
                        {
                            slotNumber = 3;
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                            value = RemoveError_String_WireEdge(value);
                        }
                        else if (dataPointDefinition.Title.Contains("DS Wet End"))
                        {
                            slotNumber = 4;
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                            value = RemoveError_String_WireEdge(value);
                        }
                        else if (dataPointDefinition.Title.Contains("DS Dry End"))
                        {
                            slotNumber = 5;
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                            value = RemoveError_String_WireEdge(value);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (dataPointDefinition.Title.Contains("Nozzle Oriface"))
                    {
                        value_Label = "Nozzle Oriface";
                        if (dataPointDefinition.IsConstantValue == 1)
                        {
                            value = dataPointDefinition.ConstantValue;
                        }
                        else if (dataPointDefinition.Title.Contains("TS Wet End"))
                        {
                            slotNumber = 7;
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                            value = RemoveGeneralError_SteamHood(value);
                        }
                        else if (dataPointDefinition.Title.Contains("TS Dry End"))
                        {
                            slotNumber = 8;
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                            value = RemoveGeneralError_SteamHood(value);
                        }
                        else if (dataPointDefinition.Title.Contains("DS Wet End"))
                        {
                            slotNumber = 9;
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                            value = RemoveGeneralError_SteamHood(value);
                        }
                        else if (dataPointDefinition.Title.Contains("DS Dry End"))
                        {
                            slotNumber = 10;
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
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
                }
                else if (dataPointDefinition.DataSetDefinitionId == 241)
                {
                    labelIndex = GetLabelIndex("Distance to Fabric WE");
                    if (dataPointDefinition.Title.Contains("Distance to Fabric WE"))
                    {
                        value_Label = "Distance to Fabric WE";
                        if (dataPointDefinition.IsConstantValue == 1)
                        {
                            value = dataPointDefinition.ConstantValue;
                        }
                        else if (dataPointDefinition.Title.EndsWith("TS Wet End"))
                        {
                            slotNumber = 3;
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                            value = RemoveGeneralError_SteamHood(value);
                        }
                        else if (dataPointDefinition.Title.EndsWith("DS Wet End"))
                        {
                            slotNumber = 4;
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                            value = RemoveGeneralError_SteamHood(value);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (dataPointDefinition.Title.Contains("Distance to Fabric DE"))
                    {
                        value_Label = "Distance to Fabric DE";
                        if (dataPointDefinition.IsConstantValue == 1)
                        {
                            value = dataPointDefinition.ConstantValue;
                        }
                        else if (dataPointDefinition.Title.Contains("TS Dry End"))
                        {
                            slotNumber = 3;
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                            value = RemoveGeneralError_SteamHood(value);
                        }
                        else if (dataPointDefinition.Title.Contains("DS Dry End"))
                        {
                            slotNumber = 4;
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                            value = RemoveGeneralError_SteamHood(value);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (dataPointDefinition.Title.Contains("MD Angle"))
                    {
                        value_Label = "MD Angle";
                        if (dataPointDefinition.IsConstantValue == 1)
                        {
                            value = dataPointDefinition.ConstantValue;
                        }
                        else if (dataPointDefinition.Title.EndsWith("TS Wet End"))
                        {
                            slotNumber = 2;
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                            value = RemoveGeneralError_SteamHood(value);
                        }
                        else if (dataPointDefinition.Title.Contains("TS Dry End"))
                        {
                            slotNumber = 3;
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                            value = RemoveGeneralError_SteamHood(value);
                        }
                        else if (dataPointDefinition.Title.Contains("DS Wet End"))
                        {
                            slotNumber = 4;
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                            value = RemoveGeneralError_SteamHood(value);
                        }
                        else if (dataPointDefinition.Title.Contains("DS Dry End"))
                        {
                            slotNumber = 5;
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                            value = RemoveGeneralError_SteamHood(value);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (dataPointDefinition.Title.Contains("CD Angle"))
                    {
                        value_Label = "CD Angle";
                        if (dataPointDefinition.IsConstantValue == 1)
                        {
                            value = dataPointDefinition.ConstantValue;
                        }
                        else if (dataPointDefinition.Title.Contains("TS Wet End"))
                        {
                            slotNumber = 3;
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                            value = RemoveGeneralError_SteamHood(value);
                        }
                        else if (dataPointDefinition.Title.Contains("TS Dry End"))
                        {
                            slotNumber = 4;
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                            value = RemoveGeneralError_SteamHood(value);
                        }
                        else if (dataPointDefinition.Title.Contains("DS Wet End"))
                        {
                            slotNumber = 5;
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                            value = RemoveGeneralError_SteamHood(value);
                        }
                        else if (dataPointDefinition.Title.Contains("DS Dry End"))
                        {
                            slotNumber = 6;
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
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
                }
                else if (dataPointDefinition.DataSetDefinitionId == 242)
                {
                    labelIndex = GetLabelIndex("DS Box");
                    if (dataPointDefinition.Title.Contains("WE Out Corner"))
                    {
                        value_Label = "DS Box";
                        if (dataPointDefinition.IsConstantValue == 1)
                        {
                            value = dataPointDefinition.ConstantValue;
                        }
                        else if (dataPointDefinition.Title.Contains("TS Box"))
                        {
                            slotNumber = 1;
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                            value = RemoveError_NegativeInt_WireEdge(value);
                        }
                        else if (dataPointDefinition.Title.Contains("DS Box"))
                        {
                            slotNumber = 2;
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                            value = RemoveError_NegativeInt_WireEdge(value);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (dataPointDefinition.Title.Contains("WE In Corner"))
                    {
                        value_Label = "WE In corner";
                        if (dataPointDefinition.IsConstantValue == 1)
                        {
                            value = dataPointDefinition.ConstantValue;
                        }
                        else if (dataPointDefinition.Title.Contains("TS Box"))
                        {
                            slotNumber = 3;
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                            value = RemoveError_NegativeInt_WireEdge(value);
                        }
                        else if (dataPointDefinition.Title.Contains("DS Box"))
                        {
                            slotNumber = 4;
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                            value = RemoveError_NegativeInt_WireEdge(value);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (dataPointDefinition.Title.Contains("DE Out Corner"))
                    {
                        value_Label = "DE Out corner";
                        if (dataPointDefinition.IsConstantValue == 1)
                        {
                            value = dataPointDefinition.ConstantValue;
                        }
                        else if (dataPointDefinition.Title.Contains("TS Box"))
                        {
                            slotNumber = 2;
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                            value = RemoveError_NegativeInt_WireEdge(value);
                        }
                        else if (dataPointDefinition.Title.Contains("DS Box"))
                        {
                            slotNumber = 3;
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                            value = RemoveError_NegativeInt_WireEdge(value);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (dataPointDefinition.Title.Contains("DE In Corner"))
                    {
                        value_Label = "DE In corner";
                        if (dataPointDefinition.IsConstantValue == 1)
                        {
                            value = dataPointDefinition.ConstantValue;
                        }
                        else if (dataPointDefinition.Title.Contains("TS Box"))
                        {
                            slotNumber = 1;
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                            value = RemoveError_NegativeInt_WireEdge(value);
                        }
                        else if (dataPointDefinition.Title.Contains("DS Box"))
                        {
                            slotNumber = 2;
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                            value = RemoveError_NegativeInt_WireEdge(value);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (dataPointDefinition.Title.Contains("Box Overbite"))
                    {
                        value_Label = "Box Overbite";
                        if (dataPointDefinition.IsConstantValue == 1)
                        {
                            value = dataPointDefinition.ConstantValue;
                        }
                        else if (dataPointDefinition.Title.Contains("TS Box"))
                        {
                            slotNumber = 3;
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                            value = RemoveError_NegativeInt_WireEdge(value);
                        }
                        else if (dataPointDefinition.Title.Contains("DS Box"))
                        {
                            slotNumber = 4;
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                            value = RemoveError_NegativeInt_WireEdge(value);
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
                }
                else
                {
                    continue;
                }
                // Add new Object to list
                cpeDataList.Add(CreateCpeEntryDataPoint(value, dataPointDefinition.Id, cpeDefinitionId, cpeEntryId));
            }
            //Insert list of cpeentrydatapointvalues in to database using datainsertion class
            bool success = dataInsertion.Insert(cpeDataList);

            return success;
        }
    }
}
