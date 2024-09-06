# Контракты команд
## Команда с вложенным json
```json
{
  "Id": "4adb5e42-9983-465a-947b-dedcf6295a0d",
  "Type": "GetEncryptionKey",
  "IsEncrypted": false,
  "Parameters": "json"
}
```
## Ответ на команду
```json
{
  "Id": "4adb5e42-9983-465a-947b-dedcf6295a0d",
  "CommandType": "GetEncryptionKey",
  "IsSuccess": true,
  "ErrorCode": 0,
  "ErrorMessage": "test",
  "Result": "json"
}
```

# Контракты вложенных команд
### Json для запроса на отправку пачки событий
```json
[
  {
    "DriverEventId": 120,
    "EventType": "OpenDocEvent",
    "EventDateTimeUtc": "2024-09-06T14:46:03.8337162Z",
    "DocGuid": "3ab40d6f-67a3-4318-b94b-b78f2a50b370",
    "ParentGuid": null,
    "DocType": "??",
    "DocAuthor": "test",
    "MarkerGuid": "abe71359-9cb5-46d3-a6ad-036a6cabd4c8",
    "EncryptionPolicyId": null,
    "FileName": "test.docx",
    "FilePath": "c:\\test.docx",
    "ParentFileName": null,
    "ParentFilePath": null,
    "UserId": "123testSid789",
    "UserName": null
  },
  {
    "DriverEventId": 125,
    "EventType": "SaveDocEvent",
    "EventDateTimeUtc": "2024-09-06T14:46:03.8338869Z",
    "DocGuid": "edc93b8a-2994-4088-8f44-813d7782b2f1",
    "ParentGuid": null,
    "DocType": "??",
    "DocAuthor": "test",
    "MarkerGuid": "5633d268-a1c0-41f8-98ac-6aefd940c0fc",
    "EncryptionPolicyId": null,
    "FileName": "test2.docx",
    "FilePath": "c:\\test2.docx",
    "ParentFileName": null,
    "ParentFilePath": null,
    "UserId": "456123testSid789",
    "UserName": null
  }
]
```
### Json для команды SetParams
```json
{
  "IsDriverEnabled": true,
  "EventsBatchSize": 10,
  "TrustedApps": [
    {
      "AppName": "Word",
      "Hash": "abc123Hash"
    },
    {
      "AppName": "Acrobat",
      "Hash": "qwe123Hash"
    }
  ],
  "FileFormats": [
    "Doc",
    "Docx",
    "Pdf"
  ]
}
```
**Форматы файлов**
```csharp
enum FileFormat
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
```

### Json для команды SetPermissions
```json
{
  "UserId": "123 My test SID 123",
  "MarkerPermisions": {
    "99873b04-0089-4bf7-a489-375174c4dc78": "SaveAs",
    "b6d63d97-4b9a-48af-810a-0759746e6523": "Edit",
    "d9874a24-2b05-4e9d-a6c9-b4aac7875dde": "Print, CopyContent, SetMarkerWithLowerCriticalLevel"
  },
  "EncryptionPermissions": {
    "882e84b8-ed30-436d-8848-5caf6e0e92ea": "Open, Set",
    "0c45c37e-ec59-42ee-bca1-f4d9c2f119d7": "Print",
    "0d57d2b5-afb0-4416-80db-28832e375041": "Edit, Screenshot"
  }
}
```
**Список Permissions**
```csharp
public enum ePermissions
  {
    None = 0,
    Open = 1,
    Set = 2,
    Print = 4,
    Section1 = 262144, // 0x00040000
    SaveAs = 8,
    Edit = 16, // 0x00000010
    Section2 = 32, // 0x00000020
    CopyContent = 64, // 0x00000040
    Section3 = 128, // 0x00000080
    SetMarkerWithLowerCriticalLevel = 256, // 0x00000100
    Section4 = 512, // 0x00000200
    SetAnonymization = 1024, // 0x00000400
    Section5 = 2048, // 0x00000800
    Screenshot = 4096, // 0x00001000
    Section6 = 8192, // 0x00002000
    ExternalEmailSending = 16384, // 0x00004000
    All = ExternalEmailSending | Section6 | Screenshot | Section5 | SetAnonymization | Section4 | SetMarkerWithLowerCriticalLevel | Section3 | CopyContent | Section2 | Edit | SaveAs | Section1 | Print | Set | Open, // 0x00047FFF
  }
```