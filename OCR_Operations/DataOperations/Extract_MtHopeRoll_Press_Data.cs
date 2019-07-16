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
        private int GetLabelIndex_MtHopeRoll(string label, string title)
        {
            if (title.Contains("Upper Roll"))
            {
                return GetFrontLabelIndex_SlotWise(label, 2);  // GetMeasurement not efficient use Refiner Plate Methods
            }
            else if (title.Contains("Lower Roll"))
            {
                return GetFrontLabelIndex_SlotWise(label, 1);
            }
            return -1;
        }
        public bool MtHopeRoll_Press(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeDefinitionId);

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                if (dataPointDefinition.DataSetDefinitionId == 262 || dataPointDefinition.DataSetDefinitionId == 263)
                {

                    if (dataPointDefinition.Title.Contains("Header"))
                    {
                        continue;
                    }
                    else if (dataPointDefinition.Title.Contains("Lower TS"))
                    {
                        value_Label = dataPointDefinition.Title.Replace("Upper Roll - ", "").Replace("Lower Roll - ", "").Replace(" - Lower TS", "");
                        int labelIndex = GetLabelIndex_MtHopeRoll(value_Label, dataPointDefinition.Title);
                        int slotNumber = 1;
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        // Function to remove error from Values of Consistency
                    }
                    else if (dataPointDefinition.Title.Contains("Lower DS"))
                    {
                        value_Label = dataPointDefinition.Title.Replace("Upper Roll - ", "").Replace("Lower Roll - ", "").Replace(" - Lower DS", "");
                        int labelIndex = GetLabelIndex_MtHopeRoll(value_Label, dataPointDefinition.Title);
                        int slotNumber = 2;
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        // Function to remove error from Values of Consistency
                    }
                    else if (dataPointDefinition.Title.EndsWith("Measured Bow (in)"))
                    {
                        value_Label = dataPointDefinition.Title.Replace("Upper Roll - ", "").Replace("Lower Roll - ", "");
                        int labelIndex = GetLabelIndex_MtHopeRoll(value_Label, dataPointDefinition.Title);
                        value = GetFrontLabelValue_PulperCurve(value_Label, labelIndex);
                        // Function to remove error from Values of Consistency
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
