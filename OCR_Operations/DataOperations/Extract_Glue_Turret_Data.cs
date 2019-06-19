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
        private string GetTargetValue(string label, int value_length)
        {
            int index_Of_Field = OCRText.IndexOf(label);
            int countOfField = label.Length;
            while (index_Of_Field == -1)
            {
                label = label.Remove(countOfField - 1, 1);
                countOfField = label.Length;
                index_Of_Field = OCRText.IndexOf(label);
                if (countOfField <= 0)
                {
                    return ErrorText;
                }
            }
            string value = OCRText.Substring(OCRText.IndexOf("\r\n", (index_Of_Field + countOfField + 2)) + 2, value_length); // add +2 for /r/n characters
            return value;
        }
        private string GetMeasurementValue(string label, int value_length)
        {
            int index_Of_Field = OCRText.IndexOf(label);
            int countOfField = label.Length;
            while (index_Of_Field == -1)
            {
                label = label.Remove(countOfField - 1, 1);
                countOfField = label.Length;
                index_Of_Field = OCRText.IndexOf(label);
                //Error Handle if label not exist 
                if (countOfField <= 0)
                {
                    return ErrorText;
                }
            }
            string value = OCRText.Substring(index_Of_Field + countOfField + 2, value_length); // add +2 for /r/n characters
            return value;
        }
        private string RemoveError_GlueTurret(string value)
        {
            if (value.ToUpper().Contains("YES"))
            {
                value = "Yes";
                return value;
            }
            if (value.ToUpper().Contains("NO"))
            {
                value = "No";
                return value;
            }
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
            string decimal_Pattern = @"^[0-9]+(\.\d{1,2})?$";
            Regex regex = new Regex(decimal_Pattern);
            Match match = regex.Match(value);
            while (match.Value == "")
            {
                // If value doesn't match then return blank
                if(value.Length == 0)
                {
                    return "";
                }
                value = value.Remove((value.Length - 1), 1);
                match = regex.Match(value);
            }

            //To insert . if not identified by OCR
            if (!value.Contains("."))
            {
                value = value.Insert(value.Length-1, ".");
            }
            return value;
        }
        public bool Glue_Turret_Data(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>(); 
            int value_Length = 6;
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeDefinitionId);
            
            foreach(var dataPointDefinition in dataPointDefinitions)
            {
                if (dataPointDefinition.Title.Contains("Measurement/"))
                {
                    //Define label for values to find in Text
                    value_Label = dataPointDefinition.Title.Replace("Measurement/", "");
                    value = GetMeasurementValue(value_Label, value_Length);
                }
                else if (dataPointDefinition.Title.Contains("Yes/No/"))
                {
                    //Define label for values to find in Text
                    value_Label = dataPointDefinition.Title.Replace("Yes/No/", "");
                    value = GetMeasurementValue(value_Label, value_Length);
                }
                else if (dataPointDefinition.Title.Contains("Target/"))
                {
                    //Define label for values to find in Text
                    value_Label = dataPointDefinition.Title.Replace("Target/", "");
                    value = GetTargetValue(value_Label, value_Length);
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
                value = RemoveError_GlueTurret(value);
                // Create CpeEntryDataPoint object with the values obtained
                CpeEntryDataPointValue cpeEntryDataPointValue = new CpeEntryDataPointValue{
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
