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
        public bool DuoCleaner(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeEntryId);

            // DS for second table not readable to OCR in blank check if filled one show value or not
            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                if (dataPointDefinition.DataSetDefinitionId == 486)
                {
                    int labelIndex = GetLabelIndex("Duocleaner Head Distance");
                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else
                    {
                        value_Label = dataPointDefinition.Title.Replace("Head Distance - ", "");
                        value = GetSlotLabelValue_RefinerPlate(value_Label, 3, labelIndex);
                        // Error handler
                    }
                }
                else if (dataPointDefinition.DataSetDefinitionId == 487)
                {
                    int labelIndex = GetLabelIndex("Verify Nozzle runs just to deckle area of roll");
                    value_Label = dataPointDefinition.Title.Replace("Nozzle ", "");
                    value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                    value = RemoveYesNoError_RefinerPlate(value);                          //As value only yes or no;
                }
                else if (dataPointDefinition.DataSetDefinitionId == 488)
                {
                    int labelIndex = GetLabelIndex("Verify Nozzle runs just to deckle area of roll");
                    value = GetFrontLabelValue_PulperCurve("Verify nozzle setup as shown below", labelIndex);
                    value = RemoveYesNoError_RefinerPlate(value);                             //As value only yes or no;
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
