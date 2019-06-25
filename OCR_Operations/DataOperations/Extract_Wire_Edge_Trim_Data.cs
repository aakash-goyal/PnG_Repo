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
                    return "";
                }
                value = value.Remove((value.Length - 1), 1);
                match = regex.Match(value);
            }
            return value;
        }
        private string RemoveGeneralError_WireEdge(string value)
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
        public bool Wire_Edge_Trim(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            int value_Length;                                                               //  Expected Length of values
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeDefinitionId);

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                value_Length = 8;
                int slotNumber = 0;
                int labelIndex = -1;
                if (dataPointDefinition.DataSetDefinitionId == 240)
                {
                    labelIndex = GetLabelIndex("System Manufacturer");
                }
                else if (dataPointDefinition.DataSetDefinitionId == 241)
                {
                    labelIndex = GetLabelIndex("Distance to fabric WE");
                }
                else if (dataPointDefinition.DataSetDefinitionId == 242)
                {
                    labelIndex = GetLabelIndex("WE Out Corner");
                }
                else if (dataPointDefinition.DataSetDefinitionId == 243)
                {
                    labelIndex = GetLabelIndex("Vacuum Box Inspection Sheet");
                }
                if (labelIndex != -1)
                {
                    // Single field columns
                    if (dataPointDefinition.Title.Contains("Manufacturer"))
                    {
                        value_Label = dataPointDefinition.Title;
                        value = GetMeasurementValue(value_Label, value_Length);
                        value = RemoveError_String_WireEdge(value);
                    }
                    //To get Only Target field of formingwireshower values
                    else if (dataPointDefinition.Title.Contains("Target") && !dataPointDefinition.Title.Contains("Header"))
                    {
                        //Define slot number for values to find in Text
                        slotNumber = 1;
                        value_Label = dataPointDefinition.Title.Replace(" - Target", "");
                        if (value_Label.Contains("Corner"))
                        {
                            value_Label = "WE In corner";
                        }
                        value = GetSlotLabeledValues_FormWise(value_Label, value_Length, slotNumber, labelIndex);  //Refer ExtractData_Wire_Section_Shower for Method definition
                        value = RemoveGeneralError_WireEdge(value);
                    }
                    else if (dataPointDefinition.Title.Contains("Range") && !dataPointDefinition.Title.Contains("Header"))
                    {
                        //Define slot number for values to find in Text
                        slotNumber = 2;
                        value_Label = dataPointDefinition.Title.Replace(" - Range", "");
                        if (value_Label.Contains("Corner"))
                        {
                            value_Label = "WE In corner";
                        }
                        value = GetSlotLabeledValues_FormWise(value_Label, value_Length, slotNumber, labelIndex);  //Refer ExtractData_Wire_Section_Shower for Method definition
                        value = RemoveGeneralError_WireEdge(value);
                    }
                    else if (dataPointDefinition.Title.Contains("TS Wet End") && !dataPointDefinition.Title.Contains("Header"))
                    {
                        //Define slot number for values to find in Text
                        slotNumber = 3;
                        value_Label = dataPointDefinition.Title.Replace(" - TS Wet End", "");
                        value = GetSlotLabeledValues_FormWise(value_Label, value_Length, slotNumber, labelIndex);  //Refer ExtractData_Wire_Section_Shower for Method definition
                        value = RemoveGeneralError_WireEdge(value);
                    }
                    else if (dataPointDefinition.Title.Contains("TS Dry End") && !dataPointDefinition.Title.Contains("Header"))
                    {
                        //Define slot number for values to find in Text
                        slotNumber = 4;
                        value_Label = dataPointDefinition.Title.Replace(" - TS Dry End", "");
                        value = GetSlotLabeledValues_FormWise(value_Label, value_Length, slotNumber, labelIndex);  //Refer ExtractData_Wire_Section_Shower for Method definition
                        value = RemoveGeneralError_WireEdge(value);
                    }
                    else if (dataPointDefinition.Title.Contains("DS Wet End") && !dataPointDefinition.Title.Contains("Header"))
                    {
                        //Define slot number for values to find in Text
                        slotNumber = 5;
                        value_Label = dataPointDefinition.Title.Replace(" - DS Wet End", "");
                        value = GetSlotLabeledValues_FormWise(value_Label, value_Length, slotNumber, labelIndex);  //Refer ExtractData_Wire_Section_Shower for Method definition
                        value = RemoveGeneralError_WireEdge(value);
                    }
                    else if (dataPointDefinition.Title.Contains("DS Dry End") && !dataPointDefinition.Title.Contains("Header"))
                    {
                        //Define slot number for values to find in Text
                        slotNumber = 6;
                        value_Label = dataPointDefinition.Title.Replace(" - DS Dry End", "");
                        value = GetSlotLabeledValues_FormWise(value_Label, value_Length, slotNumber, labelIndex);  //Refer ExtractData_Wire_Section_Shower for Method definition
                        value = RemoveGeneralError_WireEdge(value);
                    }
                    //For taking Comments
                    //else if (dataPointDefinition.Title.Contains("Comments"))
                    //{
                    //    // Define new function to take comments
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
            }
            //Insert list of cpeentrydatapointvalues in to database using datainsertion class
            bool success = dataInsertion.Insert(cpeDataList);

            return success;
        }
    }
}
