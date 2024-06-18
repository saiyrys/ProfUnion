using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using Profunion.Interfaces.ReservationInterface;

namespace Profunion.Services.ReportService
{
    public class ReportRepository : IReportRepository
    {
        private readonly IEventRepository _eventRepository;
        private readonly IUserRepository _userRepository;
        private readonly IReservationList _reservation;
        private readonly DataContext _context;
        public ReportRepository(IEventRepository eventRepository, IUserRepository userRepository,  
            IReservationList reservation, DataContext context)
        {
            _eventRepository = eventRepository;
            _userRepository = userRepository;
            _reservation = reservation;
            _context = context;
        }

        public async Task GenerateReport(string eventId)
        {
            var events = await _eventRepository.GetEvents();

            var membersOfEvent = await _reservation.GetReservationByEvent(eventId);
            var currentEvents = events.FirstOrDefault(e => e.eventId == eventId);

            if (currentEvents == null)
                throw new ArgumentException("Ивент не найден");

            var category = currentEvents.categories.Select(category => category.name);

            string eventCategories = string.Join(", ", category);

            string mainTitle = $"Отчёт сформирован по мероприятию «{currentEvents.title}»";

            string organizator = $"Организатор: тест ";

            string implementer = "«ГБПОУ КСТ»";

            string secondTitle = $"по мероприятию «{currentEvents.title}»";

            string filePath = "Отчёт.docx";

            // Создаем документ Word
            using (WordprocessingDocument doc = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
            {
                // Добавляем MainDocumentPart
                MainDocumentPart mainPart = doc.AddMainDocumentPart();
                // Добавляем Document
                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());

                StyleDefinitionsPart styleDefinitionsPart = mainPart.AddNewPart<StyleDefinitionsPart>();
                Styles styles = new Styles();
                styleDefinitionsPart.Styles = styles;
                styleDefinitionsPart.Styles.Save();

                Paragraph mainTitleParagraph = new Paragraph(new Run(new Text(mainTitle)));
                ApplyHeaderStyle(mainTitleParagraph);
                body.Append(mainTitleParagraph);
                body.AppendChild(new Paragraph(new Run(new Text(Environment.NewLine))));
                body.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { After = "100" })));
                // Добавляем информацию об организаторе и исполнителе
                body.AppendChild(new Paragraph(new Run(new Text(organizator))));
                body.AppendChild(new Paragraph(new Run(new Text(Environment.NewLine))));
                body.AppendChild(new Paragraph(new Run(new Text("Исполнитель: " + implementer))));
                body.AppendChild(new Paragraph(new Run(new Text(Environment.NewLine))));

                Paragraph secondTitleParagraph = new Paragraph(new Run(new Text("Материалы " + secondTitle)));
                ApplyCenteredStyle(secondTitleParagraph);
                body.AppendChild(secondTitleParagraph);

                body.AppendChild(new Paragraph(new Run(new Text(Environment.NewLine))));


                Table table = new Table();

                table.AppendChild(new TableProperties(new TableWidth() { Width = "5500" }));
                table.AppendChild(new TableGrid(new GridColumn(), new GridColumn()));

                table.AppendChild(CreateTableRow("Мероприятие: ", currentEvents.title));
                table.AppendChild(CreateTableRow("Категории: ", eventCategories));
                table.AppendChild(CreateTableRow("Количество участников: ", membersOfEvent.Count().ToString()));
                table.AppendChild(CreateTableRow("Сроки проведения", currentEvents.eventDate.ToString()));


                TableProperties tblProps = new TableProperties(
                    new TableBorders(
                        new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                        new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                        new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                        new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                        new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                        new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 }
                    )
                );

                table.AppendChild(tblProps);
                body.Append(table);
            }
        }

        private TableRow CreateTableRow(string cell1Text, string cell2Text)
        {
            TableCell cell1 = new TableCell(new Paragraph(new Run(new Text(cell1Text))));
            TableCellProperties cell1Properties = new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "3000" });
            cell1Properties.AppendChild(new TableCellBorders(new TopBorder(), new BottomBorder(), new LeftBorder(), new RightBorder()));
            cell1.Append(cell1Properties);

            TableCell cell2 = new TableCell(new Paragraph(new Run(new Text(cell2Text))));
            TableCellProperties cell2Properties = new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "2500" });
            cell2Properties.AppendChild(new TableCellBorders(new TopBorder(), new BottomBorder(), new LeftBorder(), new RightBorder()));
            cell2.Append(cell2Properties);

            ApplyFontStyle(cell1);
            ApplyFontStyle(cell2);

            return new TableRow(cell1, cell2);
        }
        private void ApplyFontStyle(TableCell cell)
        {
            Paragraph paragraph = cell.Elements<Paragraph>().FirstOrDefault();
            if (paragraph != null)
            {
                Run run = paragraph.Elements<Run>().FirstOrDefault();
                if (run != null)
                {
                    RunProperties runProperties = new RunProperties(
                        new RunFonts() { Ascii = "Times New Roman" },
                        new FontSize() { Val = "28" } 
                    );
                    run.AppendChild(runProperties);
                }
            }
        }

        private void ApplyHeaderStyle(Paragraph paragraph)
        {
            ParagraphProperties paragraphProperties = new ParagraphProperties(
                new Justification() { Val = JustificationValues.Center },
                new SpacingBetweenLines() { After = "100" }
            );
            paragraph.ParagraphProperties = paragraphProperties;
            RunProperties runProperties = new RunProperties(
                new RunFonts() { Ascii = "Times New Roman" },
                new FontSize() { Val = "36" }
            );
            paragraph.ParagraphProperties.AppendChild(runProperties);
        }
        private void ApplyCenteredStyle(Paragraph paragraph)
        {
            ParagraphProperties paragraphProperties = new ParagraphProperties(
                new Justification() { Val = JustificationValues.Center },
                new SpacingBetweenLines() { Before = "100", After = "0", LineRule = LineSpacingRuleValues.Auto }
            );
            paragraph.ParagraphProperties = paragraphProperties;
            RunProperties runProperties = new RunProperties(
                new RunFonts() { Ascii = "Times New Roman" },
                new FontSize() { Val = "28" }
            );
            paragraph.ParagraphProperties.AppendChild(runProperties);
        }

    }
}
