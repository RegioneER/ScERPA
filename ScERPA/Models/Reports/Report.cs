using iText.IO.Font.Constants;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Pdfa;
using Serilog.Context;
using iText.Layout;
using Serilog;
using Microsoft.EntityFrameworkCore.Diagnostics;
using OfficeOpenXml;
using System.IO;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OfficeOpenXml.Table;
using static System.Runtime.InteropServices.JavaScript.JSType;



namespace ScERPA.Models.Reports
{
    public class Report
    {
        public string NomeBreve { get; set; } = "";
        public string Titolo { get; set; } = "";
        public string Sottotitolo { get; set; } = "";



        public Report(string nomeBreve, string titolo, string sottotitolo)
        {
            NomeBreve = nomeBreve;
            Titolo = titolo;
            Sottotitolo = sottotitolo;

        }

        public List<SezioneReport> Sezioni { get; set; } = new List<SezioneReport>();

        public async Task<string> ExportAsPdfAsync(string percorsoWWW) {
            string resultFile="";


            try
            {
                bool numerazioneSchede = false;
                int numeroScheda = 0;
                string percorsoFont = Path.Combine(percorsoWWW, "Resources", "titillium-web-v10-latin-ext_latin-regular.ttf");
                string percorsoIntent = Path.Combine(percorsoWWW, "Resources", "sRGB Color Space Profile.icm");
                PdfFont font;
                iText.Layout.Element.Table tabellaScheda;
                bool singolaTabella = false;

                //using Stream fileStream = new FileStream(percorso, FileMode.Open, FileAccess.Read);
                using var memoryStream = new MemoryStream();
                WriterProperties wp = new WriterProperties();
                //wp.UseSmartMode();
                wp.AddPdfUaXmpMetadata(PdfUAConformance.PDF_UA_2);
                wp.SetPdfVersion(PdfVersion.PDF_2_0);

                //wp.AddPdfUaXmpMetadata(PdfUAConformance.PDF_UA_1);
                //wp.AddXmpMetadata();
                await using var writer = new PdfWriter(memoryStream, wp);
                writer.SetCloseStream(false);

                PdfADocument pdfDocument = new PdfADocument(writer, PdfAConformance.PDF_A_3A, new PdfOutputIntent("Custom", "", "https://www.color.org", "sRGB IEC61966-2.1", new FileStream(percorsoIntent, FileMode.Open, FileAccess.Read)));
                //using PdfDocument pdfDocument = new PdfDocument(writer);
                Document doc = new Document(pdfDocument, iText.Kernel.Geom.PageSize.A4, false);


                pdfDocument.SetTagged();
                pdfDocument.GetCatalog().SetLang(new PdfString("it-IT"));
                pdfDocument.GetCatalog().SetViewerPreferences(new PdfViewerPreferences().SetDisplayDocTitle(true));

                PdfDocumentInfo info = pdfDocument.GetDocumentInfo();
                info.SetTitle(Titolo);

                try
                {
                    font = PdfFontFactory.CreateFont(percorsoFont, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);
                }
                catch
                {
                    font = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN);
                };


                //Titolo
                doc.Add(new Paragraph(new Text(Titolo)).SetFont(font).SetTextAlignment(TextAlignment.CENTER).SetFontSize(18));

                //Sottotitolo se presente
                if (!string.IsNullOrEmpty(Sottotitolo))
                    doc.Add(new Paragraph(new Text(Sottotitolo)).SetFont(font).SetTextAlignment(TextAlignment.CENTER).SetFontSize(14));

                var bordoScheda = new iText.Layout.Borders.SolidBorder(new iText.Kernel.Colors.DeviceRgb(186, 186, 186), 1f);

                foreach (SezioneReport sezione in Sezioni)
                {
                    //Titolo della sezione
                    doc.Add(new Paragraph(new Text(sezione.Titolo)
                    .SetFont(font)
                    .SetFontSize(16)
                    .SetTextAlignment(TextAlignment.LEFT)));

                    //Sottotitolo della sezione se presente
                    if (!string.IsNullOrEmpty(Sottotitolo))
                        doc.Add(new Paragraph(new Text(sezione.Sottotitolo)
                        .SetFont(font)
                        .SetFontSize(14)
                        .SetTextAlignment(TextAlignment.LEFT)));


                    //è definita una singola tabella in questa sezione?
                    singolaTabella = sezione.Tabella?.Length > 0;

                    //se ci sono più schede in sezione e non faccio render di singola tabella numero le schede
                    numerazioneSchede = !singolaTabella && sezione.Schede.Count > 1;

                    numeroScheda = 0;

                    if (singolaTabella)
                    {
                        //creo la singola tabella con le proporzioni indicate
                        var tabellaSchede = new iText.Layout.Element.Table(UnitValue.CreatePercentArray(sezione.Tabella)).SetWidth(UnitValue.CreatePercentValue(100)).SetFixedLayout().SetBorder(iText.Layout.Borders.Border.NO_BORDER);

                        foreach (SchedaReport scheda in sezione.Schede)
                        {
                            numeroScheda++;
                            if (numeroScheda == 1)
                            {
                                foreach (var dato in scheda.Dati)
                                {
                                    var nomeIntestazione = new Paragraph(dato.Key).SetFont(font).SetFontSize(12).SetTextAlignment(TextAlignment.LEFT).SetFontColor(new iText.Kernel.Colors.DeviceRgb(66, 66, 66));
                                    var bordoIntestazione = new iText.Layout.Borders.SolidBorder(new iText.Kernel.Colors.DeviceRgb(186, 186, 186), 1f);
                                    var cellaIntestazione = new Cell().Add(nomeIntestazione);
                                    cellaIntestazione.SetBackgroundColor(new iText.Kernel.Colors.DeviceRgb(233, 233, 233)).SetBorder(iText.Layout.Borders.Border.NO_BORDER).SetBorderBottom(bordoIntestazione).SetBorderTop(bordoIntestazione).SetBorderLeft(bordoIntestazione).SetBorderRight(bordoIntestazione);
                                    tabellaSchede.AddHeaderCell(cellaIntestazione);
                                }

                            }
                            foreach (var dato in scheda.Dati)
                            {
                                tabellaSchede.AddCell(new Cell().Add(new Paragraph(dato.Value)).SetFont(font).SetFontSize(12).SetTextAlignment(TextAlignment.LEFT).SetFontColor(new iText.Kernel.Colors.DeviceRgb(66, 66, 66)).SetBackgroundColor(new iText.Kernel.Colors.DeviceRgb(255, 255, 255)).SetBorder(iText.Layout.Borders.Border.NO_BORDER).SetBorderBottom(bordoScheda).SetBorderBottom(bordoScheda).SetBorderTop(bordoScheda).SetBorderLeft(bordoScheda).SetBorderRight(bordoScheda));
                            }

                        };

                        doc.Add(tabellaSchede);

                    }
                    else
                    {
                        foreach (SchedaReport scheda in sezione.Schede)
                        {
                            numeroScheda++;

                            //creo la una tabella per scheda
                            tabellaScheda = new iText.Layout.Element.Table(UnitValue.CreatePercentArray(new float[] { 30f, 70f })).SetWidth(UnitValue.CreatePercentValue(100)).SetFixedLayout().SetBorder(iText.Layout.Borders.Border.NO_BORDER);

                            //se vanno numerate inserisco il numero
                            if (numerazioneSchede)
                            {
                                doc.Add(new Paragraph(new Text(" " + numeroScheda.ToString() + " ").SetFont(font)
                                    .SetFontSize(8).SetFontColor(iText.Kernel.Colors.ColorConstants.WHITE))
                                    .SetTextAlignment(TextAlignment.CENTER).SetWidth(12)
                                    .SetBackgroundColor(new iText.Kernel.Colors.DeviceRgb(119, 119, 119))
                                    .SetBorderRadius(new BorderRadius(5f)));
                            }
                            foreach (var dato in scheda.Dati)
                            {
                                tabellaScheda.AddCell(new Cell().Add(new Paragraph(dato.Key).SetFont(font).SetFontSize(12).SetTextAlignment(TextAlignment.RIGHT).SetFontColor(new iText.Kernel.Colors.DeviceRgb(66, 66, 66)))).SetBackgroundColor(new iText.Kernel.Colors.DeviceRgb(233, 233, 233)).SetBorder(iText.Layout.Borders.Border.NO_BORDER).SetBorderBottom(bordoScheda).SetBorderBottom(bordoScheda).SetBorderTop(bordoScheda).SetBorderLeft(bordoScheda).SetBorderRight(bordoScheda);

                                tabellaScheda.AddCell(new Cell().Add(new Paragraph(dato.Value)).SetFont(font).SetFontSize(12).SetTextAlignment(TextAlignment.LEFT).SetFontColor(new iText.Kernel.Colors.DeviceRgb(66, 66, 66)).SetBackgroundColor(new iText.Kernel.Colors.DeviceRgb(255, 255, 255)).SetBorder(iText.Layout.Borders.Border.NO_BORDER).SetBorderBottom(bordoScheda).SetBorderBottom(bordoScheda).SetBorderTop(bordoScheda).SetBorderLeft(bordoScheda).SetBorderRight(bordoScheda));

                            }
                            doc.Add(tabellaScheda);
                        }
                    }
                };

                doc.Close();
                memoryStream.Position = 0;

                resultFile = Convert.ToBase64String(memoryStream.ToArray());

            } catch (Exception ex)
            {
                Log.Error(ex, "Si è verifcato un errore in esportazione report {NomeBreve} in pdf",NomeBreve);
                resultFile = "";
            }



            return resultFile;
        }

        public async Task<string> ExportAsExcelAsync(string percorsoWWW)
        {
            string resultFile = "";
            int riga;
            int colonna;
            int maxcolonna;
            int numTabella = 0;

            try
            {
                //Imposto la licenza a non commericale
               
                ExcelPackage.License.SetNonCommercialOrganization("Regione Emilia-Romagna");

                using (var package = new ExcelPackage())
                {

                    foreach (SezioneReport sezione in Sezioni)
                    {
                        var sheet = package.Workbook.Worksheets.Add(sezione.NomeBreve);

                        numTabella++;
                        riga = 0;
                        maxcolonna = 0;

                        foreach (SchedaReport scheda in sezione.Schede)
                        {
                            riga++;
                            //se è la prima riga scrivo l'intestazione
                            if (riga == 1)
                            {
                                colonna = 0;
                                foreach (var dato in scheda.Dati)
                                {
                                    colonna++;
                                    sheet.Cells[riga, colonna].Value = dato.Key;
                                    maxcolonna = Math.Max(maxcolonna, colonna);
                                }
                                riga++;
                                //passo alla riga dati
                            }
                            //scrivo i dati
                            colonna = 0;
                            foreach (var dato in scheda.Dati)
                            {
                                colonna++;
                                sheet.Cells[riga, colonna].Value = dato.Value;
                            }

                        }

                        ExcelRange rg = sheet.Cells[1, 1, riga, maxcolonna];


                        ExcelTable tab = sheet.Tables.Add(rg, $"Tabella{numTabella}");
                        tab.TableStyle = TableStyles.Medium2;
                    }

                    var excelBytes = await package.GetAsByteArrayAsync();
                    resultFile = Convert.ToBase64String(excelBytes);
                }
            } catch(Exception ex)
            {
                Log.Error(ex, "Si è verifcato un errore in esportazione report {NomeBreve} in excel", NomeBreve);
                resultFile = "";
            }
             
            
            return resultFile;
        }

    }
}
