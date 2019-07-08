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
        public bool PressureRoll_AirBag(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeDefinitionId);

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                int labelIndex = GetLabelIndex("Pressure Roll Data"); //Refiner PLate has better function than getMeasurement 
                if (dataPointDefinition.DataSetDefinitionId == 366)
                {
                    if (dataPointDefinition.Title.Contains("TS"))
                    {
                        value_Label = dataPointDefinition.Title.Replace(" - TS", "");
                        int slotNumber = 1;
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        // Function to remove error from Values of TS
                    }
                    else if (dataPointDefinition.Title.Contains("DS"))
                    {
                        value_Label = dataPointDefinition.Title.Replace(" - DS", "");
                        int slotNumber = 2;
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        // Function to remove error from Values of DS
                    }
                    else                      // Check What is target and range and accomodate them as well
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
