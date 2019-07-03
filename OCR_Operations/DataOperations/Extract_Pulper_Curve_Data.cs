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
        private string GetFrontLabelValue_PulperCurve(string label, int labelIndex)
        {
            int index_Of_Field = OCRText.IndexOf(label, labelIndex);
            int countOfField = label.Length;
            while (index_Of_Field == -1)
            {
                label = label.Remove(0, 1);
                countOfField = label.Length;
                index_Of_Field = OCRText.IndexOf(label, labelIndex);
                //Error Handle if label not exist 
                if (countOfField <= 2)
                {
                    return ErrorText;
                }
            }
            int startIndex = OCRText.IndexOf("\r\n", index_Of_Field) + 2;
            int lastIndex = OCRText.IndexOf("\r\n", startIndex);
            if (startIndex == 1 || startIndex >= OCRText.Length)
            {
                return ErrorText;
            }
            if (lastIndex == -1)
            {
                return OCRText.Substring(startIndex);
            }
            int value_Length = lastIndex - startIndex;
            string value = OCRText.Substring(startIndex, value_Length); // add +2 for /r/n characters
            return value;
        }
        public bool Pulper_Curve(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeDefinitionId);

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                int labelIndex = GetLabelIndex("Pulper Curve Data Sheet");  // GetMeasurement not efficient use Refiner Plate Methods
                if (dataPointDefinition.DataSetDefinitionId == 451)
                {
                    if (dataPointDefinition.Title.Contains("Puple Type Option"))                                   //Confirm Spelling with database
                    {
                        value_Label = dataPointDefinition.Title.Replace("Puple Type Option", "Pulp Type");
                        value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                        // Function to remove error from Values of pulp type
                    }
                    else if (dataPointDefinition.Title.Contains("Time"))
                    {
                        value_Label = dataPointDefinition.Title;                                                 //Confirm Spelling with database
                        value = GetFrontLabelValue_PulperCurve(value_Label, labelIndex);
                        // Function to remove error from Values of Time
                    }
                    else if (dataPointDefinition.Title.Contains("Consistency"))                                 //Confirm Spelling with database
                    {
                        value_Label = dataPointDefinition.Title;
                        value = GetFrontLabelValue_PulperCurve(value_Label, labelIndex);
                        // Function to remove error from Values of Batch Consistency
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (dataPointDefinition.DataSetDefinitionId == 452)
                {    //Check if Sample time ogf Average table is present and in separate column
                    if (dataPointDefinition.Title.Contains("Avg Tensile"))                    // If Average Ref Loading always remain empty than increment slot number by 1
                    {
                        labelIndex = GetLabelIndex("Averaged Results"); //Time#1 of last table need to considered
                        value_Label = "Time #" + dataPointDefinition.Title.Substring(dataPointDefinition.Title.IndexOf("#") + 1, 1);
                        int slotNumber = 2;
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        // Function to remove error from Values of Average Tensile
                    }
                    else if (dataPointDefinition.Title.Contains("Avg PFR"))                    // If Average Ref Loading always remain empty than increment slot number by 1
                    {
                        labelIndex = GetLabelIndex("Averaged Results"); //Condition#1 of last table need to considered
                        value_Label = "Time #" + dataPointDefinition.Title.Substring(dataPointDefinition.Title.IndexOf("#") + 1, 1);
                        int slotNumber = 3;
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        // Function to remove error from Values of Average PFR
                    }
                    else if (dataPointDefinition.Title.Contains("Avg. % Fines"))                // If Average Ref Loading always remain empty than increment slot number by 1
                    {
                        labelIndex = GetLabelIndex("Averaged Results"); //Condition#1 of last table need to considered
                        value_Label = "Time #" + dataPointDefinition.Title.Substring(dataPointDefinition.Title.IndexOf("#") + 1, 1);
                        int slotNumber = 4;
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        // Function to remove error from Values of Average % Fines
                    }
                    else if (dataPointDefinition.Title.Contains("Ten/PFR"))                    // If Average Ref Loading always remain empty than increment slot number by 1
                    {
                        labelIndex = GetLabelIndex("Averaged Results"); //Condition#1 of last table need to considered
                        value_Label = "Time #" + dataPointDefinition.Title.Substring(dataPointDefinition.Title.IndexOf("#") + 1, 1);
                        int slotNumber = 5;
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        // Function to remove error from Values of Ten/PFR
                    }
                    else if (dataPointDefinition.Title.Contains("Tensile 1"))                    // If Average Ref Loading always remain empty than increment slot number by 1
                    {
                        int labelSlotNum = Convert.ToInt32(dataPointDefinition.Title.Substring(dataPointDefinition.Title.IndexOf("#") + 1, 1));
                        labelIndex = GetFrontLabelIndex_SlotWise("Sample #1", labelSlotNum); //Condition#1 of 3rdt table need to considered
                        value_Label = "Sample #1";
                        int slotNumber = 1;
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        // Function to remove error from Values of Tensile
                    }
                    else if (dataPointDefinition.Title.Contains("PFR 1"))                    // If Average Ref Loading always remain empty than increment slot number by 1
                    {
                        int labelSlotNum = Convert.ToInt32(dataPointDefinition.Title.Substring(dataPointDefinition.Title.IndexOf("#") + 1, 1));
                        labelIndex = GetFrontLabelIndex_SlotWise("Sample #1", labelSlotNum); //Condition#1 of 3rdt table need to considered
                        value_Label = "Sample #1";
                        int slotNumber = 2;
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        // Function to remove error from Values of PFR
                    }
                    else if (dataPointDefinition.Title.Contains("%Fines 1"))                    // If Average Ref Loading always remain empty than increment slot number by 1
                    {
                        int labelSlotNum = Convert.ToInt32(dataPointDefinition.Title.Substring(dataPointDefinition.Title.IndexOf("#") + 1, 1));
                        labelIndex = GetFrontLabelIndex_SlotWise("Sample #1", labelSlotNum); //Condition#1 of 3rdt table need to considered
                        value_Label = "Sample #1";
                        int slotNumber = 3;
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        // Function to remove error from Values of %Fines
                    }
                    else if (dataPointDefinition.Title.Contains("Consistency 1"))                    // If Average Ref Loading always remain empty than increment slot number by 1
                    {
                        int labelSlotNum = Convert.ToInt32(dataPointDefinition.Title.Substring(dataPointDefinition.Title.IndexOf("#") + 1, 1));
                        labelIndex = GetFrontLabelIndex_SlotWise("Sample #1", labelSlotNum); //Condition#1 of 3rdt table need to considered
                        value_Label = "Sample #1";
                        int slotNumber = 4;
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        // Function to remove error from Values of Consistency
                    }
                    else if (dataPointDefinition.Title.Contains("Tensile 2"))                    // If Average Ref Loading always remain empty than increment slot number by 1
                    {
                        int labelSlotNum = Convert.ToInt32(dataPointDefinition.Title.Substring(dataPointDefinition.Title.IndexOf("#") + 1, 1));
                        labelIndex = GetFrontLabelIndex_SlotWise("Sample #2", labelSlotNum); //Condition#1 of 3rdt table need to considered
                        value_Label = "Sample #2";
                        int slotNumber = 1;
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        // Function to remove error from Values of Tensile
                    }
                    else if (dataPointDefinition.Title.Contains("PFR 2"))                    // If Average Ref Loading always remain empty than increment slot number by 1
                    {
                        int labelSlotNum = Convert.ToInt32(dataPointDefinition.Title.Substring(dataPointDefinition.Title.IndexOf("#") + 1, 1));
                        labelIndex = GetFrontLabelIndex_SlotWise("Sample #2", labelSlotNum); //Condition#1 of 3rdt table need to considered
                        value_Label = "Sample #2";
                        int slotNumber = 2;
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        // Function to remove error from Values of PFR
                    }
                    else if (dataPointDefinition.Title.Contains("%Fines 2"))                    // If Average Ref Loading always remain empty than increment slot number by 1
                    {
                        int labelSlotNum = Convert.ToInt32(dataPointDefinition.Title.Substring(dataPointDefinition.Title.IndexOf("#") + 1, 1));
                        labelIndex = GetFrontLabelIndex_SlotWise("Sample #2", labelSlotNum); //Condition#1 of 3rdt table need to considered
                        value_Label = "Sample #2";
                        int slotNumber = 3;
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        // Function to remove error from Values of %Fines
                    }
                    else if (dataPointDefinition.Title.Contains("Consistency 2"))                    // If Average Ref Loading always remain empty than increment slot number by 1
                    {
                        int labelSlotNum = Convert.ToInt32(dataPointDefinition.Title.Substring(dataPointDefinition.Title.IndexOf("#") + 1, 1));
                        labelIndex = GetFrontLabelIndex_SlotWise("Sample #2", labelSlotNum); //Condition#1 of 3rdt table need to considered
                        value_Label = "Sample #2";
                        int slotNumber = 4;
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
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
