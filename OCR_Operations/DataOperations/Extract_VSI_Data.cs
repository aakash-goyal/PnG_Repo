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
        public bool VSI(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            int value_Length = 6;                                                               //  Expected Length of values
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeDefinitionId);

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                //To get Only Consistency field values
                if (dataPointDefinition.Title.Contains("Consistency"))
                {
                    //Define label for values to find in Text
                    int slotNumber = 2;                                                          // Bottle number will have value
                    value_Label = dataPointDefinition.Title.Replace("/Consistency", "");
                    value = GetSlotLabeledValues(value_Label, value_Length, slotNumber);  //to get Values of Consistency
                    value = RemoveGeneralError_WireboxVer2(value);
                }
                //To get Only VSI field values
                else if (dataPointDefinition.Title.Contains("VSI"))
                {
                    //Define label for values to find in Text
                    int slotNumber = 4;
                    value_Label = dataPointDefinition.Title.Replace("/VSI", "");
                    value = GetSlotLabeledValues(value_Label, value_Length, slotNumber);  //to get Values of Target, Maximum, Minimum
                    value = RemoveGeneralError_WireboxVer2(value);
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
