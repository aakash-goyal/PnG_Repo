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
        public bool PressureRoll_NipImpression(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeDefinitionId);

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                if (dataPointDefinition.DataSetDefinitionId == 373)
                {
                    int labelIndex = GetFrontLabelIndex_SlotWise("Nip Impression Data", 2); //Nip Data of second page table need to considered
                    value_Label = "Sheet Position (CD location on nip impression)";                                                        // Check with data filled Form
                    if (dataPointDefinition.Title.Contains("TS"))
                    {
                        int slotNumber = 1;
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        // Function to remove error from Values of TS position
                    }
                    else if (dataPointDefinition.Title.Contains("DS"))
                    {
                        int slotNumber = 2;
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        // Function to remove error from Values of DS position
                    }
                    else               //For handling database discrepancies
                    {
                        continue;
                    }
                }
                else if (dataPointDefinition.DataSetDefinitionId == 372)
                {
                    int labelIndex = GetFrontLabelIndex_SlotWise("Nip Impression Data", 2); //Nip Data of second page table need to considered
                    if (dataPointDefinition.Title.Contains("Target"))
                    {
                        value_Label = dataPointDefinition.Title.Replace(" Target", "");
                        int slotNumber = 2;
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        // Function to remove error from Values of Nip Width
                    }
                    else if (dataPointDefinition.Title.Contains("Min"))
                    {
                        value_Label = dataPointDefinition.Title.Replace(" Min", "");
                        int slotNumber = 3;
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        // Function to remove error from Values of Nip Width
                    }
                    else if (dataPointDefinition.Title.Contains("Max"))
                    {
                        value_Label = dataPointDefinition.Title.Replace(" Max", "");
                        int slotNumber = 4;
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        // Function to remove error from Values of Nip Width
                    }
                    else
                    {
                        value_Label = dataPointDefinition.Title;
                        int slotNumber = 1;
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        // Function to remove error from Values of Nip Width
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
