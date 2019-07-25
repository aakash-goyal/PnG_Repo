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
        public bool E_Spray(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeEntryId);

            //OCRTEXT have 3 TS only 
            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                int labelIndex = GetLabelIndex("Emulsion Spray Header Angle");
                if (dataPointDefinition.DataSetDefinitionId == 190)
                {
                    int slotNumber = 0;
                    if (dataPointDefinition.Title.Contains("Fabric"))
                    {
                        if (dataPointDefinition.IsConstantValue == 1)
                        {
                            value = dataPointDefinition.ConstantValue;
                        }
                        else if (dataPointDefinition.Title.Contains("Target"))
                        {
                            slotNumber = 1;
                            value_Label = dataPointDefinition.Title.Replace(" Target", "");
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        }
                        else if (dataPointDefinition.Title.Contains("TS"))
                        {
                            slotNumber = 2;
                            value_Label = dataPointDefinition.Title.Replace(" TS", "");
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        }
                        else if (dataPointDefinition.Title.Contains("DS"))
                        {
                            slotNumber = 3;
                            value_Label = dataPointDefinition.Title.Replace(" DS", "");
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        }
                        else
                        {
                            continue;
                        }
                        // To Handle error of Emulsion Spray Header Angle table
                    }
                    else if (dataPointDefinition.Title.Contains("Emulsion"))
                    {
                        labelIndex = GetFrontLabelIndex_SlotWise("Emulsion Spray Header", 3);
                        if (dataPointDefinition.IsConstantValue == 1)
                        {
                            value = dataPointDefinition.ConstantValue;
                        }
                        else if (dataPointDefinition.Title.Contains("History"))
                        {
                            continue;
                        }
                        else if (dataPointDefinition.Title.Contains("Target"))
                        {
                            slotNumber = 1;
                            value_Label = dataPointDefinition.Title.Replace(" Target", "");
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        }
                        else if (dataPointDefinition.Title.Contains("TS"))
                        {
                            slotNumber = 2;
                            value_Label = dataPointDefinition.Title.Replace(" TS", "");
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        }
                        else if (dataPointDefinition.Title.Contains("DS"))
                        {
                            slotNumber = 3;
                            value_Label = dataPointDefinition.Title.Replace(" DS", "");
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        }
                        else
                        {
                            continue;
                        }
                        // To Handle error of Emulsion Spray Header Angle table
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (dataPointDefinition.DataSetDefinitionId == 191 && dataPointDefinition.Title.Contains("Distance nozzle tip to belt run"))
                {
                    int slotNumber = 0;
                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else if (dataPointDefinition.Title.Contains("TS"))
                    {
                        slotNumber = 3;
                        value_Label = dataPointDefinition.Title.Replace(" TS", "");
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                    }
                    else if (dataPointDefinition.Title.Contains("DS"))
                    {
                        slotNumber = 4;
                        value_Label = dataPointDefinition.Title.Replace(" DS", "");
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                    }
                    else
                    {
                        continue;
                    }
                    // To Handle error of Emulsion Spray Header Distance table
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
