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
        public bool Gluebox_GlueHeader(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeEntryId);
            string[] labels = new string[] { "TS", "Middle", "DS" };

            //OCRTEXT have 3 TS only 
            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                int labelIndex = GetLabelIndex("toward machine center)");
                if (dataPointDefinition.DataSetDefinitionId == 135)
                {
                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else if (dataPointDefinition.Title.Contains("Average"))             // Expect value as 0.205
                    {
                        double localValue = 0;
                        foreach (string label in labels)
                        {
                            labelIndex = GetFrontLabelIndex_SteamHood(label, 1, labelIndex);
                            value_Label = label;
                            localValue += Convert.ToDouble(GetPreProcessValue_HeaderboxIntercept(value_Label, 1, labelIndex));
                        }
                        value = Math.Round((localValue / 3), 3).ToString();
                    }
                    else
                    {
                        value_Label = dataPointDefinition.Title.Replace("Distance from Nozzle to Yankee - ", "");
                        labelIndex = GetFrontLabelIndex_SteamHood(value_Label, 1, labelIndex);
                        value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                    }
                }
                else if (dataPointDefinition.DataSetDefinitionId == 136)
                {
                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else if (dataPointDefinition.Title.Contains("Average"))             // Expect value as 0.205
                    {
                        double localValue = 0;
                        foreach (string label in labels)
                        {
                            labelIndex = GetFrontLabelIndex_SteamHood(label, 2, labelIndex);
                            value_Label = label;
                            localValue += Convert.ToDouble(GetPreProcessValue_HeaderboxIntercept(value_Label, 1, labelIndex));
                        }
                        value = Math.Round((localValue / 3), 3).ToString();
                    }
                    else
                    {
                        value_Label = dataPointDefinition.Title.Replace("Angle of Nozzle to Glue Box - ", "");
                        labelIndex = GetFrontLabelIndex_SteamHood(value_Label, 2, labelIndex);
                        value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                    }
                }
                else if (dataPointDefinition.DataSetDefinitionId == 137)
                {
                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else
                    {
                        value_Label = dataPointDefinition.Title.Replace("Angle of End Nozzles - ", "");
                        labelIndex = GetFrontLabelIndex_SteamHood(value_Label, 3, labelIndex);
                        value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                    }
                }
                else if (dataPointDefinition.DataSetDefinitionId == 138)
                {
                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else
                    {
                        value_Label = dataPointDefinition.Title.Replace("Glue Spray Verification - ", "");
                        labelIndex = GetFrontLabelIndex_SteamHood(value_Label, 4, labelIndex);
                        value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                    }
                }
                // Add for DS table
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
