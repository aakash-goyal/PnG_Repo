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
        public bool Headbox_Vane(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeDefinitionId);
            //Label of Laocation to remove from OCRText to make text uniform
            string[] locationLabel = new string[] { "\r\n200\r\n", "\r\n190\r\n", "\r\n180\r\n", "\r\n170\r\n", "\r\n160\r\n", "\r\n150\r\n", "\r\n140\r\n", "\r\n130\r\n", "\r\n120\r\n", "\r\n110\r\n", "\r\n100\r\n", "\r\n90\r\n", "\r\n80\r\n", "\r\n70\r\n", "\r\n60\r\n", "\r\n50\r\n", "\r\n40\r\n", "\r\n30\r\n", "\r\n20\r\n", "\r\n10\r\n", "\r\n0.5\r\n", "\r\n22 0.5 from DS\r\n", "\r\n21\r\n" };
            foreach (string label in locationLabel)
            {
                OCRText = OCRText.Replace(label, "\r\n");
            }

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                if (dataPointDefinition.Title.Contains("Average"))     //As for now Average values are not clear
                {
                    continue;
                }
                else if (dataPointDefinition.IsConstantValue == 1)
                {
                    value = dataPointDefinition.ConstantValue;
                }
                else if (dataPointDefinition.DataSetDefinitionId == 128)
                {
                    int labelIndex = GetFrontLabelIndex_SlotWise("TS", 2);
                    int slotNumber = (Convert.ToInt32(dataPointDefinition.Title.Remove(dataPointDefinition.Title.Length - 3).Replace("TSE ", "")) * 4 - 4) + Convert.ToInt32(dataPointDefinition.Title.Substring(dataPointDefinition.Title.Length - 1));
                    value_Label = "TS";
                    value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                    // RemoveError function for data
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
