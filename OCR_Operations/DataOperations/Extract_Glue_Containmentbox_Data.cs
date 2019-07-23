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
        public bool Glue_Containmentbox(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeDefinitionId);
            OCRText = OCRText.Replace("T'S", "TS");

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                if (dataPointDefinition.DataSetDefinitionId == 196)
                {
                    int labelIndex = GetFrontLabelIndex_SlotWise("TS End Airseal", 2);
                    if (dataPointDefinition.Title.Contains("TS"))
                    {
                        labelIndex = GetFrontLabelIndex_SlotWise("TS End Airseal", 2);
                        value_Label = "TS End Airseal";
                    }
                    else
                    {
                        labelIndex = GetFrontLabelIndex_SlotWise("DS End Airseal", 2);
                        value_Label = "DS End Airseal";
                    }
                    value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                    // Remove Error of End Airseal values
                }
                else if (dataPointDefinition.DataSetDefinitionId == 195)
                {
                    int labelIndex = GetFrontLabelIndex_SlotWise("TS End Airseal", 1);
                    if (dataPointDefinition.Title.Contains("TS"))
                    {
                        labelIndex = GetFrontLabelIndex_SlotWise("TS End Airseal", 1);
                        value_Label = "TS End Airseal";
                    }
                    else
                    {
                        labelIndex = GetFrontLabelIndex_SlotWise("DS End Airseal", 1);
                        value_Label = "DS End Airseal";
                    }
                    value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                    // Remove Error of End Airseal values
                }
                else if (dataPointDefinition.DataSetDefinitionId == 194)
                {
                    int labelIndex = GetLabelIndex("Clearance Checks to HOT Yankee");
                    value_Label = dataPointDefinition.Title.Replace(" (Mouse)", "");
                    if (value_Label == "Trailing Edge TS")                             //For Spelling error in form
                    {
                        value_Label = "Trailing Egge TS";
                    }
                    value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                    //Remove Error from Hot Yankee table values
                }
                else if (dataPointDefinition.DataSetDefinitionId == 193)
                {
                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else
                    {
                        int labelIndex = GetLabelIndex("Trailing Airseal clearance to COLD yankee");
                        value_Label = dataPointDefinition.Title.Replace(" (clearance to COLD Yankee)", "");
                        value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                        //Remove Error from Cold Yankee table values
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
