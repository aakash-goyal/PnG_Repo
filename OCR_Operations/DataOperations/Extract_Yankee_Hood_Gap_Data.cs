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
        public bool Yankee_Hood_Gap(int cpeEntryId, int cpeDefinitionId)
        {
            OCRText = OCRText.Replace("TS WE\r\n", "").Replace("TS DE\r\n", "").Replace("DS WE\r\n", "").Replace("DS DE\r\n", "");
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeDefinitionId);

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                int labelIndex = GetLabelIndex("Yankee Hood Gap");
                //To get Only Target,Maximum,Minimum field values
                if (dataPointDefinition.Title.Contains("Slot 1 Width/") && dataPointDefinition.Title.Length > 16) // 16 is length of Title like Slot 1 Width/1
                {
                    //Define label for values to find in Text
                    value_Label = dataPointDefinition.Title.Replace("Slot 1 Width/", "");
                    value = GetLabelValue_RefinerPlate(value_Label, labelIndex);  //to get Values of Target, Maximum, Minimum
                }
                else if (dataPointDefinition.Title.Contains("Slot 1 Width/"))
                {
                    //Define label for values to find in Text
                    value_Label = "Minimum";
                    int slot_number = Convert.ToInt32(dataPointDefinition.Title.Replace("Slot 1 Width/", "")) + 1;
                    value = GetSlotLabelValue_RefinerPlate(value_Label, slot_number, labelIndex);  //Define function to get Values of Column No 1,2,3....12
                }
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

                // Add new Object to list
                cpeDataList.Add(CreateCpeEntryDataPoint(value, dataPointDefinition.Id, cpeDefinitionId, cpeEntryId));
            }
            //Insert list of cpeentrydatapointvalues in to database using datainsertion class
            bool success = dataInsertion.Insert(cpeDataList);

            return success;
        }
    }
}
