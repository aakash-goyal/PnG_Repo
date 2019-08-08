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
        public bool LFSR_Lower_Wipe(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeEntryId);
            OCRText = OCRText.Replace("\r\nUpdate to have check done across entire roll. Keep 5\r\n", "\r\n").Replace("\r\nlocations..\r\n", "\r\n").Replace("\r\nDate entry as YES / NO if gap is in or out tolerance..\r\n", "\r\n");

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                int labelIndex = GetLabelIndex("Average");
                if (dataPointDefinition.DataSetDefinitionId == 310)
                {
                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else if (dataPointDefinition.Title.Contains("TS"))
                    {
                        value_Label = "TS";
                        value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                        value = RemoveYesNoError_RefinerPlate(value);
                    }
                    else
                    {
                        value_Label = dataPointDefinition.Title.Substring(dataPointDefinition.Title.IndexOf("-") + 2);
                        value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                        value = RemoveYesNoError_RefinerPlate(value);
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
