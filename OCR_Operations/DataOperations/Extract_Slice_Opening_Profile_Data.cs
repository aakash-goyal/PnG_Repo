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
        private bool LimitRow_Jack(int rowNumber)
        {
            return rowNumber <= (17 * 4) ? true : false;
        }
        public bool Slice_Opening_Profile(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            int value_Length = 6;                                                               //  Expected Length of values
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeDefinitionId);

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                value_Label = "Average";
                //To get Only Jack field values
                if (dataPointDefinition.Title.Contains("Jack"))
                {

                    if (dataPointDefinition.Title.EndsWith("1"))
                    {
                        int slotNumber = (4 * Convert.ToInt32(dataPointDefinition.Title.Replace("Jack ", "").Replace(" Measure 1", ""))) - 4 + 1;  // 4 for previous row and row number won't print in ocr
                        if (LimitRow_Jack(slotNumber))
                        {
                            value = GetSlotLabeledValues(value_Label, value_Length, slotNumber);  //to get Values of Jack
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (dataPointDefinition.Title.EndsWith("2"))
                    {
                        int slotNumber = (4 * Convert.ToInt32(dataPointDefinition.Title.Replace("Jack ", "").Replace(" Measure 2", ""))) - 4 + 2;  // 4 for previous row and row number won't print in ocr  
                        if (LimitRow_Jack(slotNumber))
                        {
                            value = GetSlotLabeledValues(value_Label, value_Length, slotNumber);  //to get Values of Jack
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (dataPointDefinition.Title.EndsWith("3"))
                    {
                        int slotNumber = (4 * Convert.ToInt32(dataPointDefinition.Title.Replace("Jack ", "").Replace(" Measure 3", ""))) - 4 + 3;  // 4 for previous row and row number won't print in ocr  
                        if (LimitRow_Jack(slotNumber))
                        {
                            value = GetSlotLabeledValues(value_Label, value_Length, slotNumber);  //to get Values of Jack
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (dataPointDefinition.Title.EndsWith("Average"))
                    {
                        int slotNumber = (4 * Convert.ToInt32(dataPointDefinition.Title.Replace("Jack ", "").Replace(" Average", ""))) - 4 + 4;  // 4 for previous row and row number won't print in ocr  
                        if (LimitRow_Jack(slotNumber))
                        {
                            value = GetSlotLabeledValues(value_Label, value_Length, slotNumber);  //to get Values of Jack
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else  // Just to handle mistaken fields
                    {
                        continue;
                    }
                }
                //To get Only Measure Avg field values
                else if (dataPointDefinition.Title.Contains("Avg"))
                {
                    int labelIndex = GetLabelIndex("Average") + 2;  // Form have Two Average in page
                    if (dataPointDefinition.Title.Contains("1"))
                    {
                        int slotNumber = 1;
                        value = GetSlotLabeledValues_FormWise(value_Label, value_Length, slotNumber, labelIndex);  //to get Values of Average  Use Wire_Section_Shower form for definition
                    }
                    else if (dataPointDefinition.Title.Contains("2"))
                    {
                        int slotNumber = 2;
                        value = GetSlotLabeledValues_FormWise(value_Label, value_Length, slotNumber, labelIndex);  //to get Values of Average
                    }
                    else if (dataPointDefinition.Title.Contains("3"))
                    {
                        int slotNumber = 3;
                        value = GetSlotLabeledValues_FormWise(value_Label, value_Length, slotNumber, labelIndex);  //to get Values of Average
                    }
                    else if (dataPointDefinition.Title.Contains("4"))
                    {
                        int slotNumber = 4;
                        value = GetSlotLabeledValues_FormWise(value_Label, value_Length, slotNumber, labelIndex);  //to get Values of Average
                    }
                    else
                    {
                        continue;
                    }
                }
                // Chest Field Values need to take care as their field name take 2 rows
                // Target and Min, Max, Minimum fields not handled
                else
                {
                    continue;
                }

                //If value is not found then assign null to value
                if (value.Equals(ErrorText))
                {
                    value = "";
                }
                //to handle possible common errors in Values
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
