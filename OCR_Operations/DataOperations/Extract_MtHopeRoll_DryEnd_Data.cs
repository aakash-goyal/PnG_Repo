using OCR_Operations.DataAcess;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCR_Operations.DataOperations
{
    partial class ExtractData
    {
        public bool MtHopeRoll_DryEnd(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeDefinitionId);
            Hashtable datapoints = new Hashtable();

            foreach (var dataPointDefinition in dataPointDefinitions.Where(item => item.IsCalculated == 0))
            {
                int labelIndex = GetLabelIndex("Upper DS");
                int slotNumber = 0;
                if (dataPointDefinition.DPShortName == null || dataPointDefinition.Title.Contains("Orientation"))
                {
                    continue;
                }
                else if (dataPointDefinition.Title.Contains("Calendar Roll"))
                {
                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else if (dataPointDefinition.Title.Contains("Calendar TS") || dataPointDefinition.Title.Contains("Measured Bow (in)"))
                    {
                        value_Label = dataPointDefinition.Title.Replace("Calendar Roll - ", "").Replace(" - Calendar TS", "");
                        slotNumber = 1;
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else if (dataPointDefinition.Title.Contains("Calendar DS"))
                    {
                        value_Label = dataPointDefinition.Title.Replace("Calendar Roll - ", "").Replace(" - Calendar DS", "");
                        slotNumber = 2;
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (dataPointDefinition.Title.Contains("Reel Roll"))
                {
                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else if (dataPointDefinition.Title.Contains("Calendar TS") || dataPointDefinition.Title.Contains("Measured Bow (in)"))
                    {
                        value_Label = dataPointDefinition.Title.Replace("Reel Roll - ", "").Replace(" - Calendar TS", "");
                        slotNumber = 1;
                        labelIndex = GetFrontLabelIndex_SlotWise(value_Label, 2);
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else if (dataPointDefinition.Title.Contains("Calendar DS"))
                    {
                        value_Label = dataPointDefinition.Title.Replace("Reel Roll - ", "").Replace(" - Calendar DS", "");
                        slotNumber = 2;
                        labelIndex = GetFrontLabelIndex_SlotWise(value_Label, 2);
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
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
                if (value != ErrorText)
                {
                    datapoints.Add(dataPointDefinition.Id, value);
                }
                else
                {
                    datapoints.Add(dataPointDefinition.Id, 0);
                }
                // Add new Object to list
                cpeDataList.Add(CreateCpeEntryDataPoint(value, dataPointDefinition.Id, cpeDefinitionId, cpeEntryId));
            }
            foreach (var dataPointDefinition in dataPointDefinitions.Where(item => item.IsCalculated == 1))
            {
                int[] dataPointIds;
                if (dataPointDefinition.DataSetDefinitionId == 265)
                {
                    dataPointIds = new int[] { 4747, 4748, 4750, 4751, 4753, 4754, 4756 };
                }
                else
                {
                    dataPointIds = new int[] { 4816, 4817, 4819, 4820, 4822, 4823, 4825 };
                }
                if (dataPointDefinition.Title.EndsWith("Mt. Hope Roll"))               // It calculation need orientation
                {
                    continue;
                }
                else if (dataPointDefinition.Title.Contains("Roll Twist"))
                {
                    double rkaTs = Convert.ToDouble(datapoints[dataPointIds[4]].ToString());
                    double rkaDs = Convert.ToDouble(datapoints[dataPointIds[5]].ToString());
                    value = Math.Round(Math.Abs(rkaDs - rkaTs), 3).ToString();
                    if (rkaTs == 0 || rkaDs == 0)
                    {
                        value = ErrorText;
                    }
                }
                else if (dataPointDefinition.Title.Contains("Angle In Misalignment"))
                {
                    double saiTs = Convert.ToDouble(datapoints[dataPointIds[0]].ToString());
                    double saiDs = Convert.ToDouble(datapoints[dataPointIds[1]].ToString());
                    value = Math.Round(Math.Abs(saiDs - saiTs), 3).ToString();
                    if (saiTs == 0 || saiDs == 0)
                    {
                        value = ErrorText;
                    }
                }
                else if (dataPointDefinition.Title.Contains("Angle Out Misalignment"))
                {
                    double saoTs = Convert.ToDouble(datapoints[dataPointIds[2]].ToString());
                    double saoDs = Convert.ToDouble(datapoints[dataPointIds[3]].ToString());
                    value = Math.Round(Math.Abs(saoDs - saoTs), 3).ToString();
                    if (saoTs == 0 || saoDs == 0)
                    {
                        value = ErrorText;
                    }
                }
                else if (dataPointDefinition.Title.Contains("Mt. Hope Roll Angle Target"))
                {
                    double saoTs = Convert.ToDouble(datapoints[dataPointIds[2]].ToString());
                    double saoDs = Convert.ToDouble(datapoints[dataPointIds[3]].ToString());
                    double saiTs = Convert.ToDouble(datapoints[dataPointIds[0]].ToString());
                    double saiDs = Convert.ToDouble(datapoints[dataPointIds[1]].ToString());
                    value = Math.Round(((saoDs + saoTs - saiTs - saiDs) / 4), 3).ToString();
                    if (saoTs == 0 || saoDs == 0 || saiTs == 0 || saiDs == 0)
                    {
                        value = ErrorText;
                    }
                }
                else if (dataPointDefinition.Title.Contains("Mt. Hope Roll Angle Minimum"))
                {
                    double saoTs = Convert.ToDouble(datapoints[dataPointIds[2]].ToString());
                    double saoDs = Convert.ToDouble(datapoints[dataPointIds[3]].ToString());
                    double saiTs = Convert.ToDouble(datapoints[dataPointIds[0]].ToString());
                    double saiDs = Convert.ToDouble(datapoints[dataPointIds[1]].ToString());
                    value = Math.Round(((saoDs + saoTs - saiTs - saiDs - 4) / 4), 3).ToString();
                    if (saoTs == 0 || saoDs == 0 || saiTs == 0 || saiDs == 0)
                    {
                        value = ErrorText;
                    }
                }
                else if (dataPointDefinition.Title.Contains("Mt. Hope Roll Angle Maximum"))
                {
                    double saoTs = Convert.ToDouble(datapoints[dataPointIds[2]].ToString());
                    double saoDs = Convert.ToDouble(datapoints[dataPointIds[3]].ToString());
                    double saiTs = Convert.ToDouble(datapoints[dataPointIds[0]].ToString());
                    double saiDs = Convert.ToDouble(datapoints[dataPointIds[1]].ToString());
                    value = Math.Round(((saoDs + saoTs - saiTs - saiDs + 4) / 4), 3).ToString();
                    if (saoTs == 0 || saoDs == 0 || saiTs == 0 || saiDs == 0)
                    {
                        value = ErrorText;
                    }
                }
                else
                {
                    continue;
                }
                // Add new Object to list
                cpeDataList.Add(CreateCpeEntryDataPoint(value, dataPointDefinition.Id, cpeDefinitionId, cpeEntryId));
            }
            //Insert list of cpeentrydatapointvalues in to database using datainsertion class
            bool success = dataInsertion.Insert(cpeDataList);

            return success;
        }
    }
}
