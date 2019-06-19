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
        private string RemoveError_YankeeHood(string value)
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
        public bool Yankee_Hood_Gap(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            int value_Length = 6;
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeDefinitionId);

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                //To get Only Target,Maximum,Minimum field values
                if (dataPointDefinition.Title.Contains("Slot 1 Width/") && dataPointDefinition.Title.Length > 16) // 16 is length of Title like Slot 1 Width/1
                {
                    //Define label for values to find in Text
                    value_Label = dataPointDefinition.Title.Replace("Slot 1 Width/", "");
                    value = GetMeasurementValue(value_Label, value_Length);  //to get Values of Target, Maximum, Minimum
                }
                //else if(dataPointDefinition.Title.Contains("Slot 1 Width/"))
                //{
                //    //Define label for values to find in Text
                //    value_Label = dataPointDefinition.Title.Replace("Slot 1 Width/", "");
                //    value = GetMeasurementValue(value_Label, value_Length);  //Define function to get Values of Column No 1,2,3....12
                //}
                else
                {
                    continue;
                }

                //If value is not found then assign null to value
                if (value.Equals(ErrorText))
                {
                    value = "";
                }
                //Remove Common Error from value
                value = RemoveError_YankeeHood(value); //to handle possible common errors in Values
                // Create CpeEntryDataPoint object with the values obtained
                CpeEntryDataPointValue cpeEntryDataPointValue = new CpeEntryDataPointValue
                {
                    DataPointDefinitionId = dataPointDefinition.Id,
                    Value = value,
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
