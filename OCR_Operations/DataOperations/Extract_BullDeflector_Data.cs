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
        public bool BullDeflector(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeEntryId);

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                if (dataPointDefinition.DataSetDefinitionId == 55)
                {
                    int labelIndex = GetLabelIndex("Wire Angle");
                    int slotNumber = 0;
                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    //Check with CPE site if Wire Angle /Target field is filled or not.
                    else if (dataPointDefinition.Title.Contains("Wire Angle /Target"))
                    {
                        continue;
                    }
                    else if (dataPointDefinition.Title.Contains("Box Angle"))
                    {
                        slotNumber = 1;
                        value_Label = dataPointDefinition.Title.Replace("Box Angle/", "");
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        //Error Function
                    }
                    else
                    {
                        slotNumber = 2;
                        value_Label = dataPointDefinition.Title.Replace("Wire Angle /", "");
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        //Error Function
                    }
                }
                // Check on CPE for BoxTwist/DS is available or not  
                else if (dataPointDefinition.DataSetDefinitionId == 56)
                {
                    int labelIndex = GetLabelIndex("Box Twist");
                    int slotNumber = 0;
                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else if (dataPointDefinition.Title.Contains("Indentation"))
                    {
                        slotNumber = 1;
                        value_Label = dataPointDefinition.Title.Replace("Indentation/", "");
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        //Error Function
                    }
                    else if (dataPointDefinition.Title.Contains("Overbite"))
                    {
                        slotNumber = 2;
                        value_Label = dataPointDefinition.Title.Replace("Overbite/", "");
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        //Error Function
                    }
                    else
                    {
                        slotNumber = 3;
                        value_Label = dataPointDefinition.Title.Replace("BoxTwist/", "");
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        //Error Function
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
