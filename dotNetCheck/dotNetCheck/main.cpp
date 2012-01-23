/*#include <curl/curl.h>
#include <curl/easy.h>

int main(int argc, char *argv[]){
	if (curl_global_init(CURL_GLOBAL_NOTHING) != 0){
		return -1;
	}
	CURL* handle = curl_easy_init();
	if (!handle){
		return -1;
	}
	CURLcode ret = curl_easy_setopt(handle, CURLOPT_URL, "http://www.google.com");

}*/
#pragma comment (lib, "Winhttp.lib")
#include <cstdio>
#include <windows.h>
#include <winhttp.h>
int downloadDotNet(){
    DWORD dwSize = 0;
    DWORD dwDownloaded = 0;
    DWORD result = 0;
    LPSTR pszOutBuffer;
    BOOL  bResults = FALSE;
    HINTERNET  hSession = NULL, 
               hConnect = NULL,
               hRequest = NULL;

    // Use WinHttpOpen to obtain a session handle.
    hSession = WinHttpOpen( L"Musoft Dowmloader/1.0",  
                            WINHTTP_ACCESS_TYPE_DEFAULT_PROXY,
                            WINHTTP_NO_PROXY_NAME, 
                            WINHTTP_NO_PROXY_BYPASS, 0);

    // Specify an HTTP server.
    if (hSession)
        hConnect = WinHttpConnect( hSession, L"www.microsoft.com",
                                   INTERNET_DEFAULT_HTTP_PORT, 0);

    // Create an HTTP request handle.
    if (hConnect)
        hRequest = WinHttpOpenRequest( hConnect, L"GET", L"/downloads/info.aspx?na=41&SrcFamilyId=9CFB2D51-5FF4-4491-B0E5-B386F32C0992&SrcDisplayLang=en&u=http%3a%2f%2fdownload.microsoft.com%2fdownload%2f1%2fB%2fE%2f1BE39E79-7E39-46A3-96FF-047F95396215%2fdotNetFx40_Full_setup.exe",
                                       NULL, WINHTTP_NO_REFERER, 
                                       WINHTTP_DEFAULT_ACCEPT_TYPES, 
                                       WINHTTP_FLAG_REFRESH);

    // Send a request.
    if (hRequest)
        bResults = WinHttpSendRequest( hRequest,
                                       WINHTTP_NO_ADDITIONAL_HEADERS,
                                       0, WINHTTP_NO_REQUEST_DATA, 0, 
                                       0, 0);

 
    // End the request.
    if (bResults)
        bResults = WinHttpReceiveResponse( hRequest, NULL);

    // Keep checking for data until there is nothing left.
    if (bResults){
		FILE *f = fopen("dotNetFx40_Full_setup.exe", "wb");
        do 
        {
            // Check for available data.
            dwSize = 0;
            if (!WinHttpQueryDataAvailable( hRequest, &dwSize))
                result = -1;

            // Allocate space for the buffer.
            pszOutBuffer = new char[dwSize];
            if (!pszOutBuffer)
            {
                dwSize=0;
				result = -1;
            }
            else
            {
                // Read the Data.

                if (!WinHttpReadData( hRequest, (LPVOID)pszOutBuffer, 
                                      dwSize, &dwDownloaded))
                    printf( "Error %u in WinHttpReadData.\n", GetLastError());
                else
					fwrite(pszOutBuffer, 1, dwSize, f); 
            
                // Free the memory allocated to the buffer.
                delete [] pszOutBuffer;
            }

        } while (dwSize > 0);
		fclose(f);
	}

    // Report any errors.
    if (!bResults)
        result = -1;

    // Close any open handles.
    if (hRequest) WinHttpCloseHandle(hRequest);
    if (hConnect) WinHttpCloseHandle(hConnect);
    if (hSession) WinHttpCloseHandle(hSession);
	return result;
}

int runDotNet(){
	return (int)ShellExecute(0, "open", "dotNetFx40_Full_setup.exe", 0, 0, SW_SHOW)>32?0:1;
}
/*int walkDotNet(){
	STARTUPINFO si;
	PROCESS_INFORMATION pi;
    ZeroMemory( &si, sizeof(si) );
    si.cb = sizeof(si);
    ZeroMemory( &pi, sizeof(pi) );
	if (!CreateProcess("dotNetFx40_Full_setup.exe", NULL, NULL, NULL, false, 0, NULL, NULL, &si, &pi)){
		DWORD x = GetLastError();
		return -2;
	}
	if (WaitForSingleObject(pi.hProcess, INFINITE) == WAIT_FAILED){
		return -3;
	}
	CloseHandle( pi.hProcess );
    CloseHandle( pi.hThread );
	return 0;
}*/

int check(){
	WIN32_FIND_DATA FindFileData;
	DWORD len = 64, res;
	res = GetWindowsDirectory(NULL, 0);
	char *windir = new char[res+30];
	res = GetWindowsDirectory(windir, 0);
	strcpy(windir+res, "Microsoft.NET\\Framework\\v4.0*");
	HANDLE hFind = FindFirstFile("c:\\Windows\\Microsoft.NET\\Framework\\v4.0*", &FindFileData);//v4.0.xxxxx only check for one file
	if (hFind == INVALID_HANDLE_VALUE){
		if (GetLastError() == ERROR_FILE_NOT_FOUND){
			return 0;
		}
		return -1;
	}
	FindClose(hFind);
	if (FindFileData.dwFileAttributes == FILE_ATTRIBUTE_DIRECTORY){
		return 1;
	}
	return 0;
}

int main(int argc, char* argv[]){
	bool download = false;
	if (argc != 2){
		return -1;
	}
	if (strncmp(argv[1], "check", 5) == 0){
		switch (check()){
			case 0:
			case -1:
				download = true;
			break;
			case 1:
				return 0;//.net detected - no error
			break;
		}
	}
	else if (strncmp(argv[1], "dontcheck", 9) == 0){
		download = true;
	}
	if (download){
		if (downloadDotNet() >= 0){
			return runDotNet();
		}
		return -4;
	}
	return -5;
}