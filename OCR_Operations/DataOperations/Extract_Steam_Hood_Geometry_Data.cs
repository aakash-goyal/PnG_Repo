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
        private int GetFrontLabelIndex_SteamHood(string label, int slotNumber, int labelIndex)
        {
            int index_Of_Field = labelIndex, countOfField = 0;
            for (int i = 1; i <= slotNumber; i++)
            {
                index_Of_Field = OCRText.IndexOf(label, index_Of_Field);
                countOfField = label.Length;
                while (index_Of_Field == -1)
                {
                    label = label.Remove(0, 1);
                    countOfField = label.Length;
                    index_Of_Field = OCRText.IndexOf(label, index_Of_Field);
                    //Error Handle if label not exist 
                    if (countOfField <= 4)
                    {
                        return -1;        //If label not found
                    }
                }
                index_Of_Field += countOfField;
            }

            return index_Of_Field - countOfField;
        }
        private string RemoveGeneralError_SteamHood(string value)
        {
            value = value.Replace(" ", "");
            if (value.Contains(","))
            {
                value = value.Replace(",", ".");
            }
            else if (value.Contains("-") && value.IndexOf("-") > 2)
            {
                value = value.Replace("-", ".");
            }
            //Pattern to match decimal number
            string decimal_Pattern = @"^[0-9]*(\.\d{1,3})?$";
            Regex regex = new Regex(decimal_Pattern);
            Match match = regex.Match(value);
            while (match.Value == "")
            {
                // If value doesn't match then return blank
                if (value.Length == 0)
                {
                    return "";
                }
                value = value.Remove((value.Length - 1), 1);
                match = regex.Match(value);
            }

            return value;
        }
        public bool Steam_Hood_Geometry(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeDefinitionId);
            OCRText = OCRText.Replace("LEADING EDGE\r\n", "").Replace("TRAILING EDGE\r\n", "");

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                int labelIndex = GetFrontLabelIndex_SlotWise("STEAM HOOD", 2);
                int slotNumber = 0;
                int labelSlotNumber = 0;
                value_Label = "DISTANCE TO WIRE";
                if (dataPointDefinition.DataSetDefinitionId == 381)
                {
                    if (dataPointDefinition.Title.Contains("DS Trailing Edge"))
                    {
                        labelSlotNumber = 4;
                    }
                    else if (dataPointDefinition.Title.Contains("DS Leading Edge"))
                    {
                        labelSlotNumber = 3;
                    }
                    else if (dataPointDefinition.Title.Contains("TS Trailing Edge"))
                    {
                        labelSlotNumber = 2;
                    }
                    else if (dataPointDefinition.Title.Contains("TS Leading Edge"))
                    {
                        labelSlotNumber = 1;
                    }
                    if (labelSlotNumber == 0)
                    {
                        continue;
                    }
                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else if (dataPointDefinition.Title.Contains("- Raised"))
                    {
                        labelIndex = GetFrontLabelIndex_SteamHood("DISTANCE TO WIRE", labelSlotNumber, labelIndex);
                        slotNumber = 2;                                                                           // As OCR not reading Target Hood Down value
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else if (dataPointDefinition.Title.Contains("Distance Down"))
                    {
                        labelIndex = GetFrontLabelIndex_SteamHood("DISTANCE TO WIRE", labelSlotNumber, labelIndex);
                        slotNumber = 1;
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
                // Add new Object to list
                cpeDataList.Add(CreateCpeEntryDataPoint(value, dataPointDefinition.Id, cpeDefinitionId, cpeEntryId));
            }
            //Insert list of cpeentrydatapointvalues in to database using datainsertion class
            bool success = dataInsertion.Insert(cpeDataList);

            return success;
        }
    }
}
