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
        private string RemoveError_AlphaNum_Autopic(string value)
        {
            if (value.Length == 1)
            {
                value = value.Replace("O", "0").Replace("S", "5");
            }
            Regex regexCheck = new Regex("^[a-zA-Z\\s()]+$");
            if (regexCheck.Match(value).Value != "")
            {
                return ErrorText;
            }
            value = value.Replace(" ", "").Replace("O", "0").Replace("S", "5");
            //Pattern to match Alphanumeric expression
            string alphaNum_Pattern = @"^\w*$";
            Regex regex = new Regex(alphaNum_Pattern);
            Match match = regex.Match(value);
            while (match.Value == "")
            {
                // If value doesn't match then return blank
                if (value.Length == 0)
                {
                    return ErrorText;
                }
                value = value.Remove((value.Length - 1), 1);
                match = regex.Match(value);
            }
            return value;
        }
        public bool Autopic(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeEntryId);

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                if (dataPointDefinition.DataSetDefinitionId == 330)
                {// Error function is defined for value as 0.250
                    int labelIndex = GetLabelIndex("Auto Pic Header Angle");
                    value_Label = dataPointDefinition.Title;
                    value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                    if (dataPointDefinition.Title.Contains("Nozzle Size"))
                    {
                        value = RemoveError_AlphaNum_Autopic(value);
                    }
                    else
                    {
                        value = RemoveGeneralError_SteamHood(value);
                    }
                }
                else
                {
                    continue;
                }
                // Add to Data List
                cpeDataList.Add(CreateCpeEntryDataPoint(value, dataPointDefinition.Id, cpeDefinitionId, cpeEntryId));
            }
            //Insert list of cpeentrydatapointvalues in to database using datainsertion class
            bool success = dataInsertion.Insert(cpeDataList);

            return success;
        }
    }
}
