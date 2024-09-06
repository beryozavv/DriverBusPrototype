#include <stdio.h>
#include <windows.h>
#include <string>
#include <iostream>
#include <fstream>
#include <sys/types.h>
#include <sys/stat.h>
#include <fcntl.h>
#include <vector>
#include "json.hpp"

#define MAX_BUF_SIZE 1024

unsigned long __stdcall ReadCommandPipe(void* pParam);
unsigned long __stdcall ReadResponsePipe(void* pParam);
HANDLE hPipe1, hPipe2, hLogFile;
BOOL Finished;

class Command {
public:
	std::string id;
	bool isEncrypted;
	std::string parameters; 
	// CommandType CommandType  todo

	void Deserialize(const std::string& json) {
		auto j = nlohmann::json::parse(json);

		id = j.at("Id").get<std::string>();
		isEncrypted = j.at("IsEncrypted").get<bool>();
		parameters = j.at("Parameters").get<std::string>();
	}
};

class CommandResult {
public:
	std::string id;
	bool isSuccess;
	std::string result;
	std::string errorMessage;	
	int errorCode;
	// CommandType CommandType  todo

	void Deserialize(const std::string& json) {
		auto j = nlohmann::json::parse(json);

		id = j.at("Id").get<std::string>();
		isSuccess = j.at("IsSuccess").get<bool>();
		result = j.at("Result").get<std::string>();
		//errorMessage = j.at("ErrorMessage").get<std::string>();
		//errorCode = j.at("ErrorCode").get<int>();
	}
};

int main(int argc, char* argv[])
{
	hLogFile = CreateFile(L"log.txt", GENERIC_WRITE, 0, NULL, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL | FILE_FLAG_OVERLAPPED, NULL);

	if (hLogFile == INVALID_HANDLE_VALUE) {
		printf("Can't create log file. Error: %s\n", GetLastError());
		return 0;
	}

	char buf[MAX_BUF_SIZE];

	LPTSTR lptstrPipename1 = (LPTSTR)TEXT("\\\\.\\pipe\\outputpipe");
	LPTSTR lptstrPipename2 = (LPTSTR)TEXT("\\\\.\\pipe\\inputpipe");

	DWORD cbWritten;
	DWORD dwBytesToWrite = (DWORD)strlen(buf);

	DWORD threadId;
	HANDLE hThread = NULL;

	BOOL Write_St = TRUE;

	Finished = FALSE;

	hPipe1 = CreateFile(lptstrPipename1, GENERIC_WRITE|GENERIC_READ, 0, NULL, OPEN_EXISTING, FILE_FLAG_OVERLAPPED, NULL);
	hPipe2 = CreateFile(lptstrPipename2, GENERIC_WRITE|GENERIC_READ, 0, NULL, OPEN_EXISTING, FILE_FLAG_OVERLAPPED, NULL);


	if ((hPipe1 == NULL || hPipe1 == INVALID_HANDLE_VALUE) || (hPipe2 == NULL || hPipe2 == INVALID_HANDLE_VALUE))
		printf("Could not open the pipe  - (error %d)\n", GetLastError());
	else
	{
		hThread = CreateThread(NULL, 0, &ReadCommandPipe, NULL, 0, NULL);
		hThread = CreateThread(NULL, 0, &ReadResponsePipe, NULL, 0, NULL);

		std::string getEncryptionKey = "{\"Id\":\"977c16ac-995a-4c9e-b451-b38a578059b4\",\"Type\":\"GetEncryptionKey\",\"IsEncrypted\":false,\"Parameters\":\"\\u0022ebebbb69-f9b3-4246-a937-f3bfe0f8f816\\u0022\"}";
		getEncryptionKey = getEncryptionKey + "\n";
		std::string sendEvents = "{\"Id\":\"109c933d-7457-4434-a24a-f258b7005432\",\"Type\":\"SendEventsBatch\",\"IsEncrypted\":false,\"Parameters\":\"[{\\u0022DriverEventId\\u0022:120,\\u0022EventType\\u0022:4,\\u0022EventDateTimeUtc\\u0022:\\u00222024-09-06T13:01:31.0899814Z\\u0022,\\u0022DocGuid\\u0022:\\u00227a09cf61-2245-4727-b59b-af8f662f18d8\\u0022,\\u0022ParentGuid\\u0022:null,\\u0022DocType\\u0022:\\u0022??\\u0022,\\u0022DocAuthor\\u0022:\\u0022test\\u0022,\\u0022MarkerGuid\\u0022:\\u0022f5dfeb97-b085-4f47-9dd5-4515402e6cb8\\u0022,\\u0022EncryptionPolicyId\\u0022:null,\\u0022FileName\\u0022:\\u0022test.docx\\u0022,\\u0022FilePath\\u0022:\\u0022c:\\\\\\\\test.docx\\u0022,\\u0022ParentFileName\\u0022:null,\\u0022ParentFilePath\\u0022:null,\\u0022UserId\\u0022:\\u0022123testSid789\\u0022,\\u0022UserName\\u0022:null},{\\u0022DriverEventId\\u0022:128,\\u0022EventType\\u0022:6,\\u0022EventDateTimeUtc\\u0022:\\u00222024-09-06T13:01:31.0901452Z\\u0022,\\u0022DocGuid\\u0022:\\u00227fe60b28-b868-4a42-aa57-7d7d5214fa29\\u0022,\\u0022ParentGuid\\u0022:null,\\u0022DocType\\u0022:\\u0022??\\u0022,\\u0022DocAuthor\\u0022:\\u0022test\\u0022,\\u0022MarkerGuid\\u0022:\\u0022da3c5b9d-1dcd-4b7c-905f-e9ef6929f998\\u0022,\\u0022EncryptionPolicyId\\u0022:null,\\u0022FileName\\u0022:\\u0022test2.docx\\u0022,\\u0022FilePath\\u0022:\\u0022c:\\\\\\\\test2.docx\\u0022,\\u0022ParentFileName\\u0022:null,\\u0022ParentFilePath\\u0022:null,\\u0022UserId\\u0022:\\u0022456123testSid789\\u0022,\\u0022UserName\\u0022:null}]\"}";
		sendEvents = sendEvents + "\n";

		do
		{
			printf("Enter your message: ");
			// Read a string from the keyboard
			std::string input;			
			std::getline(std::cin, input);

			//scanf("%s", buf);
			if (input == "quit")
				Write_St = FALSE;
			// отправка запроса на получение ключа
			else if (input == "key") {
				WriteFile(hPipe2, getEncryptionKey.c_str(), getEncryptionKey.size(), &cbWritten, NULL);
				//todo 
			}
			// отправка запроса с батчем событий
			else if (input == "events") {
				WriteFile(hPipe2, sendEvents.c_str(), sendEvents.size(), &cbWritten, NULL);
			}
			else
			{
				// Add a line terminator to the string (e.g., newline character)
				input += "\n";

				printf("Invalid command");
				//WriteFile(hPipe2, input.c_str(), input.size(), &cbWritten, NULL);
				//memset(buf, 0xCC, 100);
			}

		} while (Write_St);

		CloseHandle(hPipe1);
		CloseHandle(hPipe2);
		CloseHandle(hLogFile);

		Finished = TRUE;
	}

	getchar();
}
/// <summary>
/// чтение команд и запись ответов на команды
/// </summary>
/// <param name="pParam"></param>
/// <returns></returns>
unsigned long __stdcall ReadCommandPipe(void* pParam) {	
	const int bufferSize = 256;
	char buffer[bufferSize];
	DWORD bytesRead;
	std::vector<char> message;
	DWORD cbWritten;

	while (true)
	{
		// Read data from the pipe
		if (!ReadFile(hPipe1, buffer, bufferSize - 1, &bytesRead, NULL) || bytesRead == 0)
		{
			printf("Can't Read\n");
			if (Finished)
				break;

			continue; // Exit the loop if no data is read or an error occurs
		}

		// Null-terminate the buffer to treat it as a C-string
		buffer[bytesRead] = '\0';

		// Append the buffer to the message vector
		message.insert(message.end(), buffer, buffer + bytesRead);

		// Check for line terminator
		if (buffer[bytesRead - 1] == '\n') // todo проверять нужно каждый байт, т.к. в буфер могут прилететь одновременно несколько команд
		{
			std::string line(message.begin(), message.end());
			std::cout << "Received: " << line << std::endl;
			message.clear(); // Clear the message vector for the next line

			Command command;
			command.Deserialize(line);

			/*auto json_obj = nlohmann::json::parse(command.parameters);
			bool isEnabled = json_obj.at("IsDriverEnabled").get<bool>();
			int batchSize = json_obj.at("EventsBatchSize").get<int>();*/

			//std::cout << "Received: " << command.id + command.parameters << std::endl;

			std::string response = "{\"id\":\""+command.id+"\",\"isSuccess\":true,\"errorCode\":0,\"errorMessage\":\"Test message Error526526Error123123Error123123Error123123Error123123Error123123Error123123Error123123Error123123commandId=fac75ca5-56e2-4c0b-80ed-10d007fa5ba4\"}";
			response = response + '\n';

			WriteFile(hPipe1, response.c_str(), response.size(), &cbWritten, NULL);
		}
	}
}

/// <summary>
/// чтение ответов на запросы (по ключам и событиям)
/// </summary>
/// <param name="pParam"></param>
/// <returns></returns>
unsigned long __stdcall ReadResponsePipe(void* pParam) {
	const int bufferSize = 256;
	char buffer[bufferSize];
	DWORD bytesRead;
	std::vector<char> message;
	DWORD cbWritten;

	while (true)
	{
		// Read data from the pipe
		if (!ReadFile(hPipe2, buffer, bufferSize - 1, &bytesRead, NULL) || bytesRead == 0)
		{
			if (Finished)
				break;

			continue; // Exit the loop if no data is read or an error occurs
		}

		// Null-terminate the buffer to treat it as a C-string
		buffer[bytesRead] = '\0';

		// Append the buffer to the message vector
		message.insert(message.end(), buffer, buffer + bytesRead);

		// Check for line terminator
		if (buffer[bytesRead - 1] == '\n') // todo проверять нужно каждый байт, т.к. в буфер могут прилететь одновременно несколько команд
		{
			std::string line(message.begin(), message.end());
			std::cout << "Received: " << line << std::endl;
			message.clear(); // Clear the message vector for the next line

			CommandResult commandResult;
			commandResult.Deserialize(line);				

			commandResult.result;
		}
	}
}