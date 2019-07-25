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
        public bool CameraSystem(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeEntryId);
            string[] labels = new string[] { "\r\n16\r\n", "\r\n15\r\n", "\r\n14\r\n", "\r\n13\r\n", "\r\n12\r\n", "\r\n11\r\n", "\r\n10\r\n", "\r\n9\r\n", "\r\n8\r\n", "\r\n7\r\n", "\r\n6\r\n", "\r\n5\r\n", "\r\n4\r\n", "\r\n3\r\n", "\r\n2\r\n", "\r\n1\r\n" };
            foreach (string label in labels)
            {
                OCRText = OCRText.Replace(label, "\r\n");
            }
            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                if (dataPointDefinition.DataSetDefinitionId == 335)
                {
                    int labelIndex = GetLabelIndex("Camera System CPE");
                    value_Label = dataPointDefinition.Title;
                    value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                    //ERROR function
                }
                else if (dataPointDefinition.DataSetDefinitionId == 336)
                {
                    int labelIndex = GetLabelIndex("View Comments");
                    int slotNumber = (Convert.ToInt32(dataPointDefinition.Title.Replace("Camera Settings-", "").Substring(0, 2)) * 6) - 6;
                    if (dataPointDefinition.Title.Contains("DVP#"))
                    {
                        slotNumber += 1;
                    }
                    else if (dataPointDefinition.Title.Contains("FrameRate"))
                    {
                        slotNumber += 2;
                    }
                    else if (dataPointDefinition.Title.Contains("CameraType"))
                    {
                        slotNumber += 3;
                    }
                    else if (dataPointDefinition.Title.Contains("ShutterSpeed"))
                    {
                        slotNumber += 4;
                    }
                    else if (dataPointDefinition.Title.Contains("CameraName"))
                    {
                        slotNumber += 5;
                    }
                    else if (dataPointDefinition.Title.Contains("Comments"))
                    {
                        slotNumber += 6;
                    }
                    else
                    {
                        continue;
                    }
                    value_Label = "View Comments";
                    value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                }
                else if (dataPointDefinition.DataSetDefinitionId == 337)
                {
                    int labelIndex = GetLabelIndex("Classifier File Name");
                    int slotNumber = (Convert.ToInt32(dataPointDefinition.Title.Replace("Inspection Settings-", "").Substring(0, 2)) * 10) - 10;
                    if (dataPointDefinition.Title.Contains("Camera"))
                    {
                        slotNumber += 1;
                    }
                    else if (dataPointDefinition.Title.Contains("Analysis"))
                    {
                        slotNumber += 2;
                    }
                    else if (dataPointDefinition.Title.Contains("Dark Grey Change"))
                    {
                        slotNumber += 3;
                    }
                    else if (dataPointDefinition.Title.Contains("Bright Gray Change"))
                    {
                        slotNumber += 4;
                    }
                    else if (dataPointDefinition.Title.Contains("SDV"))
                    {
                        slotNumber += 5;
                    }
                    else if (dataPointDefinition.Title.Contains("Max Defect Size"))
                    {
                        slotNumber += 6;
                    }
                    else if (dataPointDefinition.Title.Contains("Min Defect Size"))
                    {
                        slotNumber += 7;
                    }
                    else if (dataPointDefinition.Title.Contains("Position Allowance"))
                    {
                        slotNumber += 8;
                    }
                    else if (dataPointDefinition.Title.Contains("Consecutive Frames"))
                    {
                        slotNumber += 9;
                    }
                    else if (dataPointDefinition.Title.Contains("Classifier File Name"))
                    {
                        slotNumber += 10;
                    }
                    else
                    {
                        continue;
                    }
                    value_Label = "Classifier File Name";
                    value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
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
