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
        private int GetLabelIndex_NossScreen(string label, int slotNumber, int labelIndex)
        {
            int index_Of_Field = labelIndex, countOfField = 0, index = labelIndex;
            for (int i = 1; i <= slotNumber; i++)
            {
                index_Of_Field = OCRText.IndexOf(label, index);
                countOfField = label.Length;
                while (index_Of_Field == -1)
                {
                    label = label.Remove(label.Length - 1, 1);
                    countOfField = label.Length;
                    index_Of_Field = OCRText.IndexOf(label, index);
                    //Error Handle if label not exist 
                    if (countOfField <= 4)
                    {
                        return -1;        //If label not found
                    }
                }
                index_Of_Field += countOfField;
                index = index_Of_Field;
            }

            return index_Of_Field - countOfField;
        }
        public bool NossScreen(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeEntryId);

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                int labelIndex = GetLabelIndex("Noss Screen CPE Data");    //Change it after confirming structure
                if (dataPointDefinition.DataSetDefinitionId == 34)
                {
                    value = dataPointDefinition.ConstantValue;
                }
                else if (dataPointDefinition.DataSetDefinitionId == 35 || dataPointDefinition.DataSetDefinitionId == 36 || dataPointDefinition.DataSetDefinitionId == 37)
                {
                    if (dataPointDefinition.DataSetDefinitionId == 35)
                    {
                        labelIndex = GetLabelIndex("Screen 1 Before Adjustment");
                    }
                    else if (dataPointDefinition.DataSetDefinitionId == 36)
                    {
                        labelIndex = GetLabelIndex("Screen 2 Before Adjustment");
                    }
                    else if (dataPointDefinition.DataSetDefinitionId == 37)
                    {
                        labelIndex = GetLabelIndex("Screen 3 Before Adjustment");
                    }
                    value_Label = "Motor Side Screen Clearance";
                    labelIndex = GetLabelIndex_NossScreen(value_Label, 2, labelIndex);
                    int clearanceNumber = Convert.ToInt32(dataPointDefinition.Title.Substring(dataPointDefinition.Title.Length - 1));
                    int slotNumber = 4 * clearanceNumber - 4;                                                         // This need to be updated if structure of form changes
                    if (dataPointDefinition.Title.Contains("Inlet"))
                    {
                        slotNumber += 1;
                    }
                    else
                    {
                        slotNumber += 2;
                    }
                    value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                    value = RemoveGeneralError_SteamHood(value);
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
