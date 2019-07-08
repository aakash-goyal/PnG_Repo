using Newtonsoft.Json.Linq;
using OCR_Operations.DataOperations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCR_Operations
{
    public class OcrProgram
    {

        public bool InsertFileData(int cpeEntryId, int cpeDefinitionId, string filePath)
        {
            bool success;
            if (File.Exists(filePath))
            {
                OCR ocr = new OCR();
                var json = ocr.GetJSON(filePath);
                string ocrText = ocr.GetProperty(json);
                ExtractData extractData = new ExtractData();
                extractData.OCRText = ocrText;

                // To add new form definitionID and method to handle it
                switch (cpeDefinitionId)
                {
                    case 1:
                        success = extractData.Yankee_Doctor_Blade(cpeEntryId, cpeDefinitionId);
                        break;
                    case 3:
                        success = extractData.Refiner_Mechanical_Inspection(cpeEntryId, cpeDefinitionId);
                        break;
                    case 6:
                        success = extractData.VSI(cpeEntryId, cpeDefinitionId);
                        break;
                    case 9:
                        success = extractData.Pulper_Repulper(cpeEntryId, cpeDefinitionId);
                        break;
                    case 17:
                        success = extractData.Yankee_Hood_Gap(cpeEntryId, cpeDefinitionId);
                        break;
                    case 19:
                        success = extractData.Pressure_Screen(cpeEntryId, cpeDefinitionId);
                        break;
                    case 27:
                        success = extractData.Glue_Turret_Data(cpeEntryId, cpeDefinitionId);
                        break;
                    case 28:
                        success = extractData.Spool_Spinner(cpeEntryId, cpeDefinitionId);
                        break;
                    case 32:
                        success = extractData.Wirebox_Ver2(cpeEntryId, cpeDefinitionId);
                        break;
                    case 34:
                        success = extractData.Wire_Section_Shower(cpeEntryId, cpeDefinitionId);
                        break;
                    case 45:
                        success = extractData.Refiner_Plate_Inspection(cpeEntryId, cpeDefinitionId);
                        break;
                    case 48:
                        success = extractData.Wire_Edge_Trim(cpeEntryId, cpeDefinitionId);
                        break;
                    case 112:
                        success = extractData.Steam_Hood_Geometry(cpeEntryId, cpeDefinitionId);
                        break;
                    case 113:
                        success = extractData.PressureRoll_NipImpression(cpeEntryId, cpeDefinitionId);
                        break;
                    case 116:
                        success = extractData.Slice_Opening_Profile(cpeEntryId, cpeDefinitionId);
                        break;
                    case 118:
                        success = extractData.PressureRoll_AirBag(cpeEntryId, cpeDefinitionId);
                        break;
                    case 120:
                        success = extractData.Refiner_Curve(cpeEntryId, cpeDefinitionId);
                        break;
                    case 122:
                        success = extractData.Pulper_Curve(cpeEntryId, cpeDefinitionId);
                        break;
                    default:
                        success = false;                                     //if wrong definitionId is given
                        break;
                }
                return success;
            }
            else
            {
                success = false; //if file is not found
                return success;
            }
        }
    }
}
