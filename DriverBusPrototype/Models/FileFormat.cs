namespace DriverBusPrototype.Models;

/// <summary>
/// Форматы файлов, которые обрабатывает агент
/// </summary>
[Flags]
public enum FileFormat
{
    None = 0,
    Pdf = 1,
    Docx = 2,
    Xlsx = 3,
    Pptx = 4,
    Vsdx = 5,
    Odt = 6,
    Ods = 7,
    Odp = 8,
    Doc = 9,
    Xls = 10,
    Ppt = 11,
    Txt = 12,
    Raw = 13
}