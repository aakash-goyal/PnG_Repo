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
        public bool Yankee_Hood_Gap(int cpeEntryId, int cpeDefinitionId)
        {
            OCRText = OCRText.Replace("TS WE\r\n", "").Replace("TS DE\r\n", "").Replace("DS WE\r\n", "").Replace("DS DE\r\n", "").Replace("\r\nK E 6 D DO N OUT A W N\r\n", "\r\n");
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeDefinitionId);

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                int labelIndex = GetLabelIndex("Yankee Hood Gap");
                if (dataPointDefinition.DataSetDefinitionId == 86)
                {
                    if (dataPointDefinition.Title.Contains("Blank"))
                    {
                        continue;
                    }
                    else if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else if (dataPointDefinition.Title.Contains("Slot 1 Width/"))
                    {
                        value_Label = "Minimum";
                        int slot_number = Convert.ToInt32(dataPointDefinition.Title.Replace("Slot 1 Width/", "")) + 1;
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slot_number, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
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

                // Add new Object to list
                cpeDataList.Add(CreateCpeEntryDataPoint(value, dataPointDefinition.Id, cpeDefinitionId, cpeEntryId));
            }
            //Insert list of cpeentrydatapointvalues in to database using datainsertion class
            bool success = dataInsertion.Insert(cpeDataList);

            return success;
        }
    }
}
