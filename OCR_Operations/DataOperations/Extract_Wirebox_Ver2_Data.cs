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
        private string GetSlotLabeledValues(string label, int value_length, int slotNumber)
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

            for (int i = 1; i < slotNumber; i++)
            {//OCRText.IndexOf("\r\n", (index_Of_Field + countOfField + 2))
                startIndex = OCRText.IndexOf("\r\n", startIndex) + 2;
            }
            if (startIndex == countOfField + 1)
            {
                return "";
            }
            string value = OCRText.Substring(startIndex, value_length); // add +2 for /r/n characters
            return value;
        }
        private string RemoveGeneralError_WireboxVer2(string value)
        {
            value = value.Replace(" ", "");
            if (value.Contains(","))
            {
                value = value.Replace(",", ".");
            }
            else if (value.Contains("-"))
            {
                value = value.Replace("-", ".");
            }
            //Pattern to match decimal number
            string decimal_Pattern = @"^[0-9]+(\.\d{1,3})?$";
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

            //To insert . if not identified by OCR
            if (!value.Contains("."))
            {
                value = value.Insert(value.Length - 1, ".");
            }
            return value;
        }
        private string RemoveError_WireboxVer2_Minimum(string value)
        {
            value = value.Replace(" ", "");
            if (value.Contains(","))
            {
                value = value.Replace(",", ".");
            }
            else if (value.Contains("-") && value.IndexOf("-") > 1)
            {
                value = value.Replace("-", ".");
            }
            if (value.Contains("-"))
            {
                value = value.Replace("-", "");
            }
            //Pattern to match decimal number
            string decimal_Pattern = @"^[0-9]+(\.\d{1,3})?$";
            Regex regex = new Regex(decimal_Pattern);
            Match match = regex.Match(value);
            while (match.Value == "")
            {
                // If value doesn't match then return blank
                if (value.Length == 0)
                {
                    return "";                                   //If value not a number
                }
                value = value.Remove((value.Length - 1), 1);
                match = regex.Match(value);
            }

            //To insert . if not identified by OCR
            if (!value.Contains("."))
            {
                value = value.Insert(value.Length - 1, ".");
            }
            value = "-" + value;
            return value;
        }
        public bool Wirebox_Ver2(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            int value_Length = 6;                                                               //  Expected Length of values
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeDefinitionId);

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                //To get Only Slot Maximum field values
                if (dataPointDefinition.Title.Contains("Slot") && dataPointDefinition.Title.Contains("Maximum"))
                {
                    //Define label for values to find in Text
                    int slotNumber = Convert.ToInt32(dataPointDefinition.Title.Replace("Slot", "").Replace("_Maximum", ""));
                    value_Label = dataPointDefinition.Title.Replace($"Slot{slotNumber}_", "");
                    value = GetSlotLabeledValues(value_Label, value_Length, slotNumber);  //to get Values of Target, Maximum, Minimum
                    value = RemoveGeneralError_WireboxVer2(value);
                }
                //To get Only Slot Target field values
                //else if (dataPointDefinition.Title.Contains("Slot") && dataPointDefinition.Title.Contains("Target"))
                //{
                //    //Define label for values to find in Text
                //    int slotNumber = Convert.ToInt32(dataPointDefinition.Title.Replace("Slot", "").Replace("_Target", ""));
                //    value_Label = dataPointDefinition.Title.Replace($"Slot{slotNumber}_", "");
                //    value = GetSlotLabeledValues(value_Label, value_Length, slotNumber);  //to get Values of Target, Maximum, Minimum
                //    value = RemoveGeneralError_WireboxVer2(value);
                //}
                //To get Only Slot Minimum field values
                else if (dataPointDefinition.Title.Contains("Slot") && dataPointDefinition.Title.Contains("Minimum"))
                {
                    //Define label for values to find in Text
                    int slotNumber = Convert.ToInt32(dataPointDefinition.Title.Replace("Slot", "").Replace("_Minimum", ""));
                    value_Label = dataPointDefinition.Title.Replace($"Slot{slotNumber}_", "");
                    value = GetSlotLabeledValues(value_Label, value_Length, slotNumber);  //to get Values of Target, Maximum, Minimum
                    value = RemoveError_WireboxVer2_Minimum(value);
                }
                //else if (dataPointDefinition.Title.Contains("Slot") && dataPointDefinition.Title.Contains("Average"))
                //{

                //}
                //else if(dataPointDefinition.Title.Contains("Slot 1 Width/"))
                //{
                //    //Define label for values to find in Text
                //    value_Label = dataPointDefinition.Title.Replace("Slot 1 Width/", "");
                //    value = GetMeasurementValue(value_Label, value_Length);               //Define function to get Values of Column No 1,2,3....12
                //}
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
