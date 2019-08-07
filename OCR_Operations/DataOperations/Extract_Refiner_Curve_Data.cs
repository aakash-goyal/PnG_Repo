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
        private int GetFrontLabelIndex_SlotWise(string label, int slotNumber)
        {
            int index_Of_Field = 0, countOfField = 0, index = 0;
            for (int i = 1; i <= slotNumber; i++)
            {
                index_Of_Field = OCRText.IndexOf(label, index_Of_Field);
                countOfField = label.Length;
                while (index_Of_Field == -1)
                {
                    label = label.Remove(0, 1);
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
        private string GetSlotFrontLabelValue_RefinerCurve(string label, int slotNumber, int labelIndex)
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
            if (startIndex == 1 || startIndex >= OCRText.Length)
            {
                return ErrorText;
            }
            for (int i = 1; i < slotNumber; i++)
            {//OCRText.IndexOf("\r\n", (index_Of_Field + countOfField + 2))
                startIndex = OCRText.IndexOf("\r\n", startIndex) + 2; // add +2 for /r/n characters
                if (startIndex == 1 || startIndex >= OCRText.Length)
                {
                    return ErrorText;
                }
            }
            int lastIndex = OCRText.IndexOf("\r\n", startIndex);
            if (startIndex == 1 || startIndex >= OCRText.Length)
            {
                return ErrorText;
            }
            if (lastIndex == -1)
            {
                return OCRText.Substring(startIndex);
            }
            int value_length = lastIndex - startIndex;
            string value = OCRText.Substring(startIndex, value_length);
            return value;
        }
        public bool Refiner_Curve(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeDefinitionId);

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                int labelIndex = GetLabelIndex("Refiner Curve Data Sheet");  // GetMeasurement not efficient use Refiner Plate Methods
                if (dataPointDefinition.DataSetDefinitionId == 436)
                {
                    if (dataPointDefinition.Title.Contains("Loading"))
                    {
                        value_Label = dataPointDefinition.Title.Replace("RC.", "Ref.").Replace(" Value", "");
                        value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                        // Function to remove error from Values of Ref. Loading
                    }
                    else if (dataPointDefinition.Title.Contains("Pres"))
                    {
                        value_Label = dataPointDefinition.Title.Replace("RC. ", "").Replace(" Value", "");
                        value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                        // Function to remove error from Values of Pressure table
                    }
                    else if (dataPointDefinition.Title.Contains("Speed"))
                    {
                        value_Label = dataPointDefinition.Title.Replace("RC. ", "");
                        value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                        // Function to remove error from Values of Yankee Speed
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (dataPointDefinition.DataSetDefinitionId == 437)
                {
                    if (dataPointDefinition.Title.Contains("6"))            //Condition #6 not exist in sample form
                    {
                        continue;
                    }
                    if (dataPointDefinition.Title.Contains("RefLoad"))
                    {
                        value_Label = "Condition #" + dataPointDefinition.Title.Substring(dataPointDefinition.Title.IndexOf("#") + 1, 1);
                        int slotNumber = 1;
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        // Function to remove error from Values of Ref. Loading
                    }
                    else if (dataPointDefinition.Title.Contains("FlowQM"))
                    {
                        value_Label = "Condition #" + dataPointDefinition.Title.Substring(dataPointDefinition.Title.IndexOf("#") + 1, 1);
                        int slotNumber = 3;
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        // Function to remove error from Values of Flow to QM
                    }
                    else if (dataPointDefinition.Title.Contains("Flow"))
                    {
                        value_Label = "Condition #" + dataPointDefinition.Title.Substring(dataPointDefinition.Title.IndexOf("#") + 1, 1);
                        int slotNumber = 2;
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        // Function to remove error from Values of Total Flow
                    }
                    else if (dataPointDefinition.Title.Contains("TDC/Exp"))                                    //For Cons(%) in 2nd table
                    {
                        value_Label = "Condition #" + dataPointDefinition.Title.Substring(dataPointDefinition.Title.IndexOf("#") + 1, 1);
                        int slotNumber = 4;
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        // Function to remove error from Values of Cons. %
                    }
                    else if (dataPointDefinition.Title.Contains("PH"))
                    {
                        value_Label = "Condition #" + dataPointDefinition.Title.Substring(dataPointDefinition.Title.IndexOf("#") + 1, 1);
                        int slotNumber = 5;
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        // Function to remove error from Values of ph
                    }
                    else if (dataPointDefinition.Title.Contains("Avg Tensile"))                    // If Average Ref Loading always remain empty than increment slot number by 1
                    {
                        labelIndex = GetLabelIndex("Averaged Results"); //Condition#1 of last table need to considered
                        value_Label = "Condition #" + dataPointDefinition.Title.Substring(dataPointDefinition.Title.IndexOf("#") + 1, 1);
                        int slotNumber = 1;
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        // Function to remove error from Values of Average Tensile
                    }
                    else if (dataPointDefinition.Title.Contains("Avg PFR"))                    // If Average Ref Loading always remain empty than increment slot number by 1
                    {
                        labelIndex = GetLabelIndex("Averaged Results"); //Condition#1 of last table need to considered
                        value_Label = "Condition #" + dataPointDefinition.Title.Substring(dataPointDefinition.Title.IndexOf("#") + 1, 1);
                        int slotNumber = 2;
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        // Function to remove error from Values of Average PFR
                    }
                    else if (dataPointDefinition.Title.Contains("Ten/PFR"))                    // If Average Ref Loading always remain empty than increment slot number by 1
                    {
                        labelIndex = GetLabelIndex("Averaged Results"); //Condition#1 of last table need to considered
                        value_Label = "Condition #" + dataPointDefinition.Title.Substring(dataPointDefinition.Title.IndexOf("#") + 1, 1);
                        int slotNumber = 3;
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        // Function to remove error from Values of Average Tensile
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
