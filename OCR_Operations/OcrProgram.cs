using Newtonsoft.Json.Linq;
using OCR_Operations.DataOperations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spire.Pdf;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Drawing.Imaging;

namespace OCR_Operations
{
    public class OcrProgram
    {
        private List<string> SaveIndividualPageAsImage(string filePath, params int[] pages)
        {
            PdfReader reader = new PdfReader(filePath);

            // List of new Images path
            List<string> imagesPath = new List<string>();

            foreach (int idx in pages)
            {
                // Create new document
                string newFilePath = String.Format("{0} - Page {1}.pdf", filePath.Replace(".pdf", ""), idx);
                using (FileStream fs = new FileStream(newFilePath, FileMode.Create))
                {
                    Document doc = new Document(reader.GetPageSize(idx));
                    PdfCopy copy = new PdfCopy(doc, fs);
                    doc.Open();
                    PdfImportedPage page = copy.GetImportedPage(reader, idx);
                    copy.AddPage(page);
                    doc.Close();
                }
                string imagePath = PdfToPng(newFilePath);
                imagesPath.Add(imagePath);
            }

            //Delete individual pdfs
            foreach (string image in imagesPath)
            {
                File.Delete(image.Replace(".png", ".pdf"));
            }
            //Delete duplicate pdf
            //File.Delete(duplicateFilePath);
            return imagesPath;
        }
        private string PdfToPng(string filePath)
        {
            Spire.Pdf.PdfDocument doc = new Spire.Pdf.PdfDocument();
            doc.LoadFromFile(filePath);
            filePath = filePath.Replace(".pdf", ".png");
            // Check if file already exists. If yes, delete it. 
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            System.Drawing.Image emf = doc.SaveAsImage(0, Spire.Pdf.Graphics.PdfImageType.Metafile);
            emf.Save(filePath, ImageFormat.Png);
            return filePath;
        }
        private string GetOCRText(string filePath, params int[] pages)
        {
            OCR ocr = new OCR();
            string ocrText = "";
            List<string> imagesPath = SaveIndividualPageAsImage(filePath, pages);
            foreach (string imagePath in imagesPath)
            {
                var JSON = ocr.GetJSON(imagePath);
                //To get text format data from JSON output
                ocrText += ocr.GetProperty(JSON) + "\r\n";
            }
            return ocrText.Remove(ocrText.Length - 2);
        }
        public bool InsertFileData(int cpeEntryId, int cpeDefinitionId, string filePath)
        {
            bool success;
            if (File.Exists(filePath))
            {
                //OCR ocr = new OCR();
                //var json = ocr.GetJSON(filePath);
                //string ocrText = ocr.GetProperty(json);
                ExtractData extractData = new ExtractData();
                //extractData.OCRText = ocrText;
                // To add new form definitionID and method to handle it
                switch (cpeDefinitionId)
                {
                    case 1:
                        success = extractData.Yankee_Doctor_Blade(cpeEntryId, cpeDefinitionId);
                        break;
                    case 3:
                        extractData.OCRText = GetOCRText(filePath, 2);
                        success = extractData.Refiner_Mechanical_Inspection(cpeEntryId, cpeDefinitionId);
                        break;
                    case 6:
                        extractData.OCRText = GetOCRText(filePath, 3);
                        success = extractData.VSI(cpeEntryId, cpeDefinitionId);
                        break;
                    case 7:
                        extractData.OCRText = GetOCRText(filePath, 5);
                        success = extractData.NossScreen(cpeEntryId, cpeDefinitionId);
                        break;
                    case 9:
                        extractData.OCRText = GetOCRText(filePath, 6);
                        success = extractData.Pulper_Repulper(cpeEntryId, cpeDefinitionId);
                        break;
                    case 11:
                        extractData.OCRText = GetOCRText(filePath, 3);
                        success = extractData.BullDeflector(cpeEntryId, cpeDefinitionId);
                        break;
                    case 12:
                        success = extractData.LFSR_SoftDoctor(cpeEntryId, cpeDefinitionId);
                        break;
                    case 13:
                        success = extractData.LFSR_Shower(cpeEntryId, cpeDefinitionId);
                        break;
                    case 17:
                        extractData.OCRText = GetOCRText(filePath, 3);
                        success = extractData.Yankee_Hood_Gap(cpeEntryId, cpeDefinitionId);
                        break;
                    case 19:
                        success = extractData.Pressure_Screen(cpeEntryId, cpeDefinitionId);
                        break;
                    case 21:
                        success = extractData.Headbox_JetImpingement(cpeEntryId, cpeDefinitionId);
                        break;
                    case 23:
                        success = extractData.Headbox_Vane(cpeEntryId, cpeDefinitionId);
                        break;
                    case 25:
                        extractData.OCRText = GetOCRText(filePath, 3);
                        success = extractData.Gluebox_GlueHeader(cpeEntryId, cpeDefinitionId);
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
                        extractData.OCRText = GetOCRText(filePath, 3);
                        success = extractData.Wire_Section_Shower(cpeEntryId, cpeDefinitionId);
                        break;
                    case 37:
                        extractData.OCRText = GetOCRText(filePath, 3);
                        success = extractData.E_Spray(cpeEntryId, cpeDefinitionId);
                        break;
                    case 38:
                        success = extractData.Glue_Containmentbox(cpeEntryId, cpeDefinitionId);
                        break;
                    case 39:
                        success = extractData.PressSection_Shower(cpeEntryId, cpeDefinitionId);
                        break;
                    case 40:
                        success = extractData.MSVB(cpeEntryId, cpeDefinitionId);
                        break;
                    case 45:
                        success = extractData.Refiner_Plate_Inspection(cpeEntryId, cpeDefinitionId);
                        break;
                    case 46:
                        success = extractData.MtHopeRoll_Press(cpeEntryId, cpeDefinitionId);
                        break;
                    case 48:
                        extractData.OCRText = GetOCRText(filePath, 3);
                        success = extractData.Wire_Edge_Trim(cpeEntryId, cpeDefinitionId);
                        break;
                    case 100:
                        extractData.OCRText = GetOCRText(filePath, 3);
                        success = extractData.BreastRoll_Internal(cpeEntryId, cpeDefinitionId);
                        break;
                    case 101:
                        extractData.OCRText = GetOCRText(filePath, 2);
                        success = extractData.DryEnd_Geometry(cpeEntryId, cpeDefinitionId);
                        break;
                    case 106:
                        extractData.OCRText = GetOCRText(filePath, 2);
                        success = extractData.Autopic(cpeEntryId, cpeDefinitionId);
                        break;
                    case 107:
                        extractData.OCRText = GetOCRText(filePath, 2,3);
                        success = extractData.CameraSystem(cpeEntryId, cpeDefinitionId);
                        break;
                    case 109:
                        success = extractData.Headbox_Geometry(cpeEntryId, cpeDefinitionId);
                        break;
                    case 112:
                        extractData.OCRText = GetOCRText(filePath, 2);
                        success = extractData.Steam_Hood_Geometry(cpeEntryId, cpeDefinitionId);
                        break;
                    case 113:
                        success = extractData.PressureRoll_NipImpression(cpeEntryId, cpeDefinitionId);
                        break;
                    case 114:
                        success = extractData.Hydrofoil(cpeEntryId, cpeDefinitionId);
                        break;
                    case 116:
                        success = extractData.Slice_Opening_Profile(cpeEntryId, cpeDefinitionId);
                        break;
                    case 118:
                        success = extractData.PressureRoll_AirBag(cpeEntryId, cpeDefinitionId);
                        break;
                    case 117:
                        success = extractData.Papilion_RefinerPlate(cpeEntryId, cpeDefinitionId);
                        break;
                    case 120:
                        success = extractData.Refiner_Curve(cpeEntryId, cpeDefinitionId);
                        break;
                    case 121:
                        success = extractData.Headbox_Intercept(cpeEntryId, cpeDefinitionId);
                        break;
                    case 122:
                        extractData.OCRText = GetOCRText(filePath, 4, 5);
                        success = extractData.Pulper_Curve(cpeEntryId, cpeDefinitionId);
                        break;
                    case 128:
                        extractData.OCRText = GetOCRText(filePath, 3);
                        success = extractData.DuoCleaner(cpeEntryId, cpeDefinitionId);
                        break;
                    case 131:
                        extractData.OCRText = GetOCRText(filePath, 3);
                        success = extractData.PD_Deckle_Position(cpeEntryId, cpeDefinitionId);
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
