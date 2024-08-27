# Для повторной генерации контрактов Command и CommandResult
### Устанавливаем утилиту protogen 
```shell
dotnet tool install --global protobuf-net.Protogen --version 3.2.42
```
### Генерируем контракт Command
```shell
protogen --proto_path=DriverBusPrototype\DriverCommands\Models --csharp_out=DriverBusPrototype\DriverCommands\Models Command.proto
```
### Генерируем контракт CommandResult
```shell
protogen --proto_path=DriverBusPrototype\DriverCommands\Models --csharp_out=DriverBusPrototype\DriverCommands\Models CommandResult.proto
```

# Контракты команд
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