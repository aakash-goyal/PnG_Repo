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
        private int GetLabelIndex(string label)
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
                    return -1;        //If label not found
                }
            }
            return index_Of_Field;
        }
        private string GetSlotLabeledValues_FormWise(string label, int value_length, int slotNumber, int labelIndex)
        {
            int index_Of_Field = OCRText.IndexOf(label, labelIndex);
            int countOfField = label.Length;
            while (index_Of_Field == -1)
            {
                label = label.Remove(countOfField - 1, 1);
                countOfField = label.Length;
                index_Of_Field = OCRText.IndexOf(label, labelIndex);
                //Error Handle if label not exist 
                if (countOfField <= 0)
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
        private string RemoveError_Angle_WireSection(string value)
        {
            string target_Angle_Pattern = @"^[a-zA-Z]+[\s]?[a-zA-Z]+$";  // only alphabet space alphabets
            Regex regex = new Regex(target_Angle_Pattern);
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
        private string RemoveGeneralError_WireSection(string value)
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
            if (value.Contains("+"))
            {
                value = value.Replace("+", "");
            }
            //Pattern to match decimal number
            string decimal_Pattern = @"^[0-9]+(\.\d{1,2})?$";
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
        public bool Wire_Section_Shower(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            int value_Length;                                                               //  Expected Length of values
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeDefinitionId);

            // dataPoint Definitions which have Forming Wire Shower or Backing wire shower DatasetID
            dataPointDefinitions = dataPointDefinitions.Where(dataPoint => dataPoint.DataSetDefinitionId == 174 || dataPoint.DataSetDefinitionId == 175).ToList();

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                value_Length = 4;
                int labelIndex = -1;
                if (dataPointDefinition.DataSetDefinitionId == 174)
                {
                    labelIndex = GetLabelIndex("Forming Wire Showers");
                }
                else if (dataPointDefinition.DataSetDefinitionId == 175)
                {
                    labelIndex = GetLabelIndex("Backing Wire Showers");
                }
                if (labelIndex != -1)
                {
                    int slotNumber = 0;
                    //To get Only Target field of formingwireshower values
                    if (dataPointDefinition.Title.Contains("Target"))
                    {
                        //Define slot number for values to find in Text
                        slotNumber = 1;
                        value_Label = dataPointDefinition.Title.Replace(" Target", "");
                        if (value_Label.EndsWith("Angle"))
                        {
                            value_Length = 14;
                            value = GetSlotLabeledValues_FormWise(value_Label, value_Length, slotNumber, labelIndex);
                            value = RemoveError_Angle_WireSection(value);
                        }
                        else                // Confirm after checking with filled forms
                        {
                            value = GetSlotLabeledValues_FormWise(value_Label, value_Length, slotNumber, labelIndex);  //Refer ExtractData_Wirebox_Ver2 for Method definition
                            value = RemoveGeneralError_WireSection(value);
                        }
                    }
                    else if (dataPointDefinition.Title.Contains("Range"))
                    {
                        //Define slot number for values to find in Text
                        slotNumber = 2;
                        value_Label = dataPointDefinition.Title.Replace(" Range", "");
                        value = GetSlotLabeledValues_FormWise(value_Label, value_Length, slotNumber, labelIndex);  //Refer ExtractData_Wirebox_Ver2 for Method definition
                        value = RemoveGeneralError_WireSection(value);
                    }
                    else if (dataPointDefinition.Title.Contains("TS"))
                    {
                        //Define slot number for values to find in Text
                        slotNumber = 3;
                        value_Label = dataPointDefinition.Title.Replace(" TS", "");
                        value = GetSlotLabeledValues_FormWise(value_Label, value_Length, slotNumber, labelIndex);  //Refer ExtractData_Wirebox_Ver2 for Method definition
                        value = RemoveGeneralError_WireSection(value);
                    }
                    else if (dataPointDefinition.Title.Contains("DS"))
                    {
                        //Define slot number for values to find in Text
                        slotNumber = 4;
                        value_Label = dataPointDefinition.Title.Replace(" DS", "");
                        value = GetSlotLabeledValues_FormWise(value_Label, value_Length, slotNumber, labelIndex);  //Refer ExtractData_Wirebox_Ver2 for Method definition
                        value = RemoveGeneralError_WireSection(value);
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
            }
            //Insert list of cpeentrydatapointvalues in to database using datainsertion class
            bool success = dataInsertion.Insert(cpeDataList);

            return success;
        }
    }
}
