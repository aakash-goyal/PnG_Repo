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
        public bool PressSection_Shower(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            int countTotalNozzleTarget = 1, countTotalNozzleMin = 1, countTotalNozzleMax = 1, countTotalNozzleTS = 1, countTotalNozzleDS = 1, countNozzleCapTarget = 1, countNozzleCapMin = 1, countNozzleCapMax = 1, countNozzleCapTS = 1, countNozzleCapDS = 1;

            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeDefinitionId);

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                if (dataPointDefinition.DataSetDefinitionId == 197)
                {
                    if (dataPointDefinition.Title.Contains("History"))                        // To ignore History related field
                    {
                        continue;
                    }
                    //To handle error in reading SS values of form
                    if (dataPointDefinition.Title.StartsWith("SS"))
                    {
                        dataPointDefinition.Title = dataPointDefinition.Title.Replace("SS ", "");
                    }
                    int labelIndex = GetLabelIndex("Press Section Showers"); //Refiner PLate has better function than getMeasurement 
                    if (dataPointDefinition.Title.Contains("Total Nozzles Target"))
                    {
                        labelIndex = GetFrontLabelIndex_SlotWise("# of Total Nozzles", countTotalNozzleTarget);
                        value_Label = dataPointDefinition.Title.Replace(" Target", "");
                        int slotNumber = 1;
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        countTotalNozzleTarget++;
                        // Function to remove error from Values of Total Nozzle
                    }
                    else if (dataPointDefinition.Title.Contains("Total Nozzles TS"))
                    {
                        labelIndex = GetFrontLabelIndex_SlotWise("# of Total Nozzles", countTotalNozzleTS);
                        value_Label = dataPointDefinition.Title.Replace(" TS", "");
                        int slotNumber = 3;
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        countTotalNozzleTS++;
                        // Function to remove error from Values of Total Nozzle
                    }
                    else if (dataPointDefinition.Title.Contains("Total Nozzles DS"))
                    {
                        labelIndex = GetFrontLabelIndex_SlotWise("# of Total Nozzles", countTotalNozzleDS);
                        value_Label = dataPointDefinition.Title.Replace(" DS", "");
                        int slotNumber = 4;
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        countTotalNozzleDS++;
                        // Function to remove error from Values of Total Nozzle
                    }                                                                           // Add for min Max of total nozzles here
                    else if (dataPointDefinition.Title.Contains("Nozzles Capped Target"))
                    {
                        labelIndex = GetFrontLabelIndex_SlotWise("# Nozzles Capped", countNozzleCapTarget);
                        value_Label = dataPointDefinition.Title.Replace(" Target", "");
                        int slotNumber = 1;
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        countNozzleCapTarget++;
                        // Function to remove error from Values of Total Nozzle
                    }
                    else if (dataPointDefinition.Title.Contains("Nozzles Capped TS"))
                    {
                        labelIndex = GetFrontLabelIndex_SlotWise("# Nozzles Capped", countNozzleCapTS);
                        value_Label = dataPointDefinition.Title.Replace(" TS", "");
                        int slotNumber = 3;
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        countNozzleCapTS++;
                        // Function to remove error from Values of Total Nozzle
                    }
                    else if (dataPointDefinition.Title.Contains("Nozzles Capped DS"))
                    {
                        labelIndex = GetFrontLabelIndex_SlotWise("# Nozzles Capped", countNozzleCapDS);
                        value_Label = dataPointDefinition.Title.Replace(" DS", "");
                        int slotNumber = 4;
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        countNozzleCapDS++;
                        // Function to remove error from Values of Nozzle Capped
                    }                                                                          // Add for min Max of nozzles capped here
                    else if (dataPointDefinition.Title.Contains("Target"))
                    {
                        value_Label = dataPointDefinition.Title.Replace(" Target", "");
                        int slotNumber = 1;
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                    }
                    else if (dataPointDefinition.Title.Contains("TS"))
                    {
                        value_Label = dataPointDefinition.Title.Replace(" TS", "");
                        int slotNumber = 3;
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        // Function to handle error
                    }
                    else if (dataPointDefinition.Title.Contains("DS"))
                    {
                        value_Label = dataPointDefinition.Title.Replace(" DS", "");
                        int slotNumber = 4;
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        // Function to handle error
                    }                                                                      // Add for min Max of fields here
                    else
                    {
                        continue;
                    }
                }
                else if (dataPointDefinition.DataSetDefinitionId == 198)
                {
                    int labelIndex = GetLabelIndex("Shower Oscillators"); //Refiner PLate has better function than getMeasurement 
                    if (dataPointDefinition.Title.Contains("Stroke Distance"))
                    {
                        value_Label = dataPointDefinition.Title.Replace(" Stroke Distance", "");
                        int slotNumber = 1;
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        // Function to handle error
                    }
                    else if (dataPointDefinition.Title.Contains("Stroke Time"))
                    {
                        value_Label = dataPointDefinition.Title.Replace(" Stroke Time", "");
                        int slotNumber = 2;
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        // Function to handle error
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (dataPointDefinition.DataSetDefinitionId == 199)
                {
                    int labelIndex = GetLabelIndex("Nip Shower"); //Refiner PLate has better function than getMeasurement
                    if (dataPointDefinition.Title.Contains("nozzles capped"))
                    {
                        dataPointDefinition.Title = dataPointDefinition.Title.Replace("nozzles capped", "of nozzles capped");
                    }
                    if (dataPointDefinition.Title.Contains("Target"))
                    {
                        value_Label = dataPointDefinition.Title.Replace(" Target", "");
                        int slotNumber = 1;
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        // Function to handle error
                    }
                    else if (dataPointDefinition.Title.Contains("TS"))
                    {
                        value_Label = dataPointDefinition.Title.Replace(" TS", "");
                        int slotNumber = 3;
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        // Function to handle error
                    }
                    else if (dataPointDefinition.Title.Contains("DS"))
                    {
                        value_Label = dataPointDefinition.Title.Replace(" DS", "");
                        int slotNumber = 4;
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        // Function to handle error
                    }                                                                          // Add for min Max of fields here
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
