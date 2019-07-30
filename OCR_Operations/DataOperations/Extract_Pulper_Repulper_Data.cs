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
        public bool Pulper_Repulper(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeEntryId);

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                if (dataPointDefinition.DataSetDefinitionId == 44)
                {
                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else if (!dataPointDefinition.Title.Contains("/"))                              // To ignore Min Max And Target fields of 3,6,9 o'clock of database
                    {
                        int labelIndex = GetLabelIndex("Clearance Inspection");
                        value_Label = dataPointDefinition.Title.Replace("  ", " ");
                        value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (dataPointDefinition.DataSetDefinitionId == 45)
                {
                    int labelIndex = GetLabelIndex("General Pulper Information");
                    if (dataPointDefinition.Title.Contains("Manufacturer"))
                    {
                        value_Label = "Pulper Mfg.";
                        value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                    }
                    else if (dataPointDefinition.Title.Contains("Type"))
                    {
                        value_Label = "Pulper Type";
                        value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                    }
                    else         // to handle discrepancies in database
                    {
                        continue;
                    }
                }
                else if (dataPointDefinition.DataSetDefinitionId == 46)
                {
                    int labelIndex = GetLabelIndex("Rotor Visual Inspection");
                    if (dataPointDefinition.Title.Contains("Standard"))                                 // Check what will be standard amnd non-standard values of wear on rotor
                    {
                        continue;
                    }
                    else
                    {
                        value_Label = dataPointDefinition.Title;
                        value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                        value = RemoveYesNoError_RefinerPlate(value);
                    }
                }
                else if (dataPointDefinition.DataSetDefinitionId == 47)           // Check if it's filled or not
                {
                    int labelIndex = GetLabelIndex("Extraction Plate Visual Inspection");
                    value_Label = dataPointDefinition.Title;
                    value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                    value = RemoveYesNoError_RefinerPlate(value);
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
