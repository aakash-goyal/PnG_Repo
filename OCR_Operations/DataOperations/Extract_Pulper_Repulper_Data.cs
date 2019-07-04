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
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeDefinitionId);

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                if (dataPointDefinition.DataSetDefinitionId == 45)
                {
                    int labelIndex = GetLabelIndex("General Pulper Information");  // GetMeasurement not efficient use Refiner Plate Methods
                    if (dataPointDefinition.Title.Contains("Manufacturer"))
                    {
                        value_Label = dataPointDefinition.Title.Replace("Manufacturer", "Pulper Mfg.");
                        value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                        // Function to remove error from Values of pulper Mfg.
                    }
                    else if (dataPointDefinition.Title.Contains("Type"))
                    {
                        value_Label = dataPointDefinition.Title.Replace("Type", "Pulper Type");
                        value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                        // Function to remove error from Values of pulper type
                    }
                    else         // to hnadle discrepancies in database
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
                else if (dataPointDefinition.DataSetDefinitionId == 47)
                {
                    int labelIndex = GetLabelIndex("Extraction Plate Visual Inspection");
                    value_Label = dataPointDefinition.Title;
                    value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                    value = RemoveYesNoError_RefinerPlate(value);
                }
                else if (dataPointDefinition.DataSetDefinitionId == 44)
                {
                    if (!dataPointDefinition.Title.Contains("/"))                              // To ignore Min Max And Target fields of database
                    {
                        int labelIndex = GetLabelIndex("Clearance Inspection");
                        value_Label = dataPointDefinition.Title.Replace("  ", " ");
                        value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                        // Function to remove error from Values of Clearance
                    }
                    else
                    {
                        continue;
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
