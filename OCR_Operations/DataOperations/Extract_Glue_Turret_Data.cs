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
            //Set Glue Turret Object Values
            //cpe_Glue_Turret.MD_Angel = Convert.ToDecimal(RemoveError_GlueTurret(GetFieldValue(md_Angel_Label, md_Angel_Value_Length)));
            //cpe_Glue_Turret.CD_Angel_Start = Convert.ToDecimal(RemoveError_GlueTurret(GetFieldValue(cd_Angel_Start_Label, cd_Angel_Start_Value_Length)));
            //cpe_Glue_Turret.CD_Angel_Stop = Convert.ToDecimal(RemoveError_GlueTurret(GetFieldValue(cd_Angel_Stop_Label, cd_Angel_Stop_Value_Length)));
            //cpe_Glue_Turret.Traverse_Time = Convert.ToDecimal(RemoveError_GlueTurret(GetFieldValue(traverse_Time_Label, traverse_Time_Value_Length)));
            //cpe_Glue_Turret.Speed = Convert.ToDecimal(RemoveError_GlueTurret(GetFieldValue(speed_Label, speed_Value_Length)));
            //cpe_Glue_Turret.Is_Spray_Hit = RemoveError_GlueTurret(GetFieldValue(is_Spray_Hit_Label, spray_Hit_Length));
            //cpe_Glue_Turret.Is_Start_Distance_2 = RemoveError_GlueTurret(GetFieldValue(is_Start_Distance_Label, start_Distance_Length));
            //cpe_Glue_Turret.Is_Stop_Distance_2 = RemoveError_GlueTurret(GetFieldValue(is_Stop_Distance_Label, stop_Distance_Length));

            //to print indexes of values
            //Console.WriteLine($"MD_Angel value {cpe_Glue_Turret.MD_Angel}\nCD_Angel_start value {cpe_Glue_Turret.CD_Angel_Start}\nCD_Angel_Stop value {cpe_Glue_Turret.CD_Angel_Stop}\n Traverse_Time value {cpe_Glue_Turret.Traverse_Time}\nSpeed value {cpe_Glue_Turret.Speed}\nSpray Hit {cpe_Glue_Turret.Is_Spray_Hit}\n Start Distance {cpe_Glue_Turret.Is_Start_Distance_2}\n Stop Distance {cpe_Glue_Turret.Is_Stop_Distance_2}");
            return success;
        }
    }
}
