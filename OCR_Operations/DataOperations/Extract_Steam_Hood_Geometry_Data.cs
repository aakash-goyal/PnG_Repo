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
        private string GetDefectLabeledValues(string label, int defectNumber, int limitIndex)
        {
            int index_Of_Field = OCRText.IndexOf(label);
            int countOfField = label.Length;
            while (index_Of_Field == -1)
            {
                label = label.Remove(countOfField - 1, 1);
                countOfField = label.Length;
                index_Of_Field = OCRText.IndexOf(label);
                //Error Handle if label not exist 
                if (countOfField <= 2)
                {
                    return ErrorText;
                }
            }
            int startIndex = index_Of_Field + countOfField + 2;

            for (int i = 1; i < defectNumber; i++)
            {//OCRText.IndexOf("\r\n", (index_Of_Field + countOfField + 2))
                startIndex = OCRText.IndexOf("\r\n", startIndex) + 2;
            }
            int lastIndex = OCRText.IndexOf("\r\n", startIndex);
            if (startIndex != countOfField + 1 && startIndex < limitIndex)
            {
                int value_length = lastIndex - startIndex;
                string value = OCRText.Substring(startIndex, value_length); // add +2 for /r/n characters
                return value;
            }
            else
            {
                return "";               // If defect content is not available
            }
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
            if (value.Contains("+"))
            {
                value = value.Replace("+", "").Replace("-", "").Replace("/", "");
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
            int value_Length;                                                               //  Expected Length of values
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeDefinitionId);

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                value_Length = 6;
                int slotNumber = 0;
                if (dataPointDefinition.DataSetDefinitionId == 382)
                {
                    value_Label = "Location";
                    int limitIndex = GetLabelIndex("STEAM HOOD");
                    if (dataPointDefinition.Title.Contains("Defects 1"))
                    {
                        slotNumber = 1;
                        value = GetDefectLabeledValues(value_Label, slotNumber, limitIndex);
                    }
                    else if (dataPointDefinition.Title.Contains("Location & Description 1"))
                    {
                        value = GetDefectLabeledValues(value_Label, 3, limitIndex);
                        value += GetDefectLabeledValues(value_Label, 2, limitIndex);
                    }
                    else if (dataPointDefinition.Title.Contains("Defects 2"))
                    {
                        slotNumber = 4;
                        value = GetDefectLabeledValues(value_Label, slotNumber, limitIndex);
                    }
                    else if (dataPointDefinition.Title.Contains("Location & Description 2"))
                    {
                        value = GetDefectLabeledValues(value_Label, 6, limitIndex);
                        value += GetDefectLabeledValues(value_Label, 5, limitIndex);
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (dataPointDefinition.DataSetDefinitionId == 381)
                {
                    if (dataPointDefinition.Title.Contains("- Target Hood Raised"))
                    {
                        slotNumber = 4;
                        value_Label = dataPointDefinition.Title.Replace(" - Target Hood Raised", "");
                        value = GetSlotLabeledValues(value_Label, value_Length, slotNumber);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else if (dataPointDefinition.Title.Contains("- Raised"))
                    {
                        slotNumber = 3;
                        value_Label = dataPointDefinition.Title.Replace(" - Raised", "");
                        value = GetSlotLabeledValues(value_Label, value_Length, slotNumber);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else if (dataPointDefinition.Title.Contains("Target Hood Down"))
                    {
                        slotNumber = 2;
                        value_Label = dataPointDefinition.Title.Replace(" - Target Hood Down", "");
                        value = GetSlotLabeledValues(value_Label, value_Length, slotNumber);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else if (dataPointDefinition.Title.Contains("Distance Down"))
                    {
                        slotNumber = 1;
                        value_Label = dataPointDefinition.Title.Replace(" - Distance Down", "");
                        value = GetSlotLabeledValues(value_Label, value_Length, slotNumber);
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
                //If value is not found then assign null to value
                if (value.Equals(ErrorText))
                {
                    value = "";
                }                              //to handle possible common errors in Values
                                               // Create CpeEntryDataPoint object with the values obtained
                CpeEntryDataPointValue cpeEntryDataPointValue = new CpeEntryDataPointValue
                {
                    DataPointDefinitionId = dataPointDefinition.Id,
                    Value = value,
                    IsBlobValue = false,  // Not saving any file for now
                    CpeDefinitionId = cpeDefinitionId,
                    CPEEntryId = cpeEntryId
                };
                // Add new Object to list
                cpeDataList.Add(cpeEntryDataPointValue);
            }
            //Insert list of cpeentrydatapointvalues in to database using datainsertion class
            bool success = dataInsertion.Insert(cpeDataList);

            return success;
        }
    }
}
