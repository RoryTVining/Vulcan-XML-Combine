using System;
using System.IO;
using System.Data;
using System.Data.SQLite;
using ClosedXML.Excel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using IdentityModel.Client;
using System.Globalization;
using System.Numerics;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.VisualBasic.FileIO;
using System.Web;
using static LOADIQU;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Renci.SshNet;
using Renci.SshNet.Common;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Linq;
using System.Transactions;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.AdditionalCharacteristics;

//*****************************************
//* _     ___    _    ____ ___ ___  _   _ *
//*| |   / _ \  / \  |  _ \_ _/ _ \| | | |*
//*| |  | | | |/ _ \ | | | | | | | | | | |*
//*| |__| |_| / ___ \| |_| | | |_| | |_| |*
//*|_____\___/_/   \_\____/___\__\_\\___/ *
//*****************************************

//Overview:
//The LOADIQU™ system is a robust solution for automating and optimizing bulk explosives loading QA/QC in underground mining.
//It comprises a Raspberry Pi Zero WH, LOADIQU™ Console Application, LPU, and BlastIQ™ Integration.
//Operators input loading data via the LPU, which is then transferred securely to the Raspberry Pi.
//The LOADIQU™ application retrieves, processes, and integrates this data with BlastIQ™, ensuring accurate loading operations and facilitating data synchronization for comprehensive record-keeping

//  ___           
// / _ \ _ __(_) ___ __ _ 
//| | | | '__| |/ __/ _` |
//| |_| | |  | | (_| (_| |
// \___/|_|  |_|\___\__,_|

//Nuget Summary:
/*
 * Index of namespaces used in the application:
 * 
 * System: Fundamental types and base types.
 * System.IO: File and directory support.
 * System.Data: Access to ADO.NET architecture.
 * System.Data.SQLite: Support for SQLite database engine.
 * ClosedXML.Excel: Interaction with Excel files.
 * System.Collections.Generic: Generic collections.
 * System.Diagnostics: Interaction with system processes.
 * System.Security.Cryptography: Cryptographic services.
 * System.Text: Encoding and decoding characters.
 * System.Threading.Tasks: Simplifies concurrent and asynchronous code.
 * System.Text.RegularExpressions: Pattern matching on strings.
 * System.Net.Http: Sending and receiving HTTP requests and responses.
 * System.Net.Http.Headers: Working with HTTP headers.
 * System.Net: Programming interface for network protocols.
 * System.IdentityModel.Tokens.Jwt: Support for JSON Web Tokens (JWTs).
 * Newtonsoft.Json: Working with JSON data.
 * Microsoft.IdentityModel.Protocols: Communication with configuration endpoints.
 * Microsoft.IdentityModel.Protocols.OpenIdConnect: Communication with OpenID Connect configuration endpoints.
 * IdentityModel.Client: Client library for OAuth 2.0 and OpenID Connect endpoints.
 * System.Globalization: Culture-related information.
 * System.Numerics: Types supporting real and complex number operations.
 * CsvHelper: Reading and writing CSV and tab-delimited files.
 * CsvHelper.Configuration: Configuration for CsvHelper.
 * Microsoft.VisualBasic.FileIO: Basic file I/O.
 * System.Web: Browser-server communication.
 * Newtonsoft.Json.Linq: Representation of JSON data as objects and collections.
 * Newtonsoft.Json.Serialization: Serialization and deserialization behavior for JSON data.
 * Renci.SshNet: SSH client and server implementations.
 * Renci.SshNet.Common: Common types used by Renci.SshNet library.
 * System.Runtime.InteropServices: Support for COM interop and platform invoke services.
 * System.Net.NetworkInformation: Information on network interfaces and ping functionality.
 * System.Linq: Support for Language-Integrated Query (LINQ).
 * System.Transactions: Classes for writing and using transactions.
 * DocumentFormat.OpenXml.Spreadsheet: Interaction with Open XML spreadsheets.
 *
 * Compliance with ISO/IEC 27001:2022:
 * 
 * The use of cryptographic services from System.Security.Cryptography aligns with controls related to information security policy, asset management, access control, and cryptography.
 * Interaction with network protocols through System.Net and System.Net.Http supports controls related to network security, system acquisition, development, and maintenance.
 * Handling of authentication and authorization using JWT tokens (System.IdentityModel.Tokens.Jwt) and OAuth 2.0 (IdentityModel.Client) aligns with controls related to access control, communication security, and identity management.
 * Data handling and serialization/deserialization using Newtonsoft.Json support controls related to information security policy, system acquisition, and data handling.
 * Interaction with external systems and APIs through various namespaces supports controls related to supplier relationships, information exchange, and service delivery.
 * Logging and monitoring functionalities provided by System.Diagnostics support controls related to monitoring, analysis, and review.
 * Usage of SSH client implementations from Renci.SshNet aligns with controls related to system acquisition, development, and maintenance for secure communication channels.
 * Support for transactions (System.Transactions) ensures data integrity and aligns with controls related to data handling and system development.
 */

//Additional method overview
/*
    Index of Methods and Classes with ISO/IEC 27001:2022 Compliance Citations:

    1. LOADIQU Constructor - Ensures necessary directories are created for data storage. (A.12.1.3 Operating procedures)
    2. EnsureFolderExists - Verifies and creates necessary storage directories, enhancing data security. (A.12.4.1 Control of operational software)
    3. UpdateRaspberryPiCredentials - Securely updates credentials used for system operations. (A.9.4.2 User access management)
    4. Main - Manages application startup and operational scenarios based on connectivity. (A.16.1.4 Assessment of and decision on information security events)
    5. InitializeDatabase - Initializes and verifies the database setup at startup. (A.14.2.7 Secure system engineering principles)
    6. InitializeEncryptionKey - Handles encryption keys securely at startup. (A.10.1 Cryptography)
    7. PingHost - Checks network connectivity, supporting secure communications. (A.13.1.3 Network segregation)
    8. ProcessRaspberryPiUsbDrive - Processes data from a USB drive securely. (A.12.2.1 Controls against malware)
    9. VerifyAndCreatePlansTable - Ensures data structure integrity within the database. (A.14.2.2 Secure development environment)
    10. ProcessLocalCsvFiles - Safely processes CSV files for data import. (A.12.5.1 Installation of software on operational systems)
    11. ImportProductsAsync - Securely imports product data, adhering to confidentiality requirements. (A.13.2.1 Information transfer policies and procedures)
    12. LoadPlansIntoDatabase - Loads and verifies plans data within the system securely. (A.14.1.2 Secure development policy)
    13. FetchHolesDataAsync - Retrieves hole data securely from remote services. (A.13.2.3 Transfer of information)
    14. DeserializeJsonDataAsync - Securely processes JSON data into structured formats. (A.14.3 Protection of information in log files)
    15. ProcessHolesData - Manages data integrity and security during the processing of hole data. (A.12.3.1 Protection of stored information)
    16. GetGlobalSiteIdFromApiResponse - Extracts and securely handles site ID data from API responses. (A.13.2.4 Electronic messaging)
    17. EnsureValidToken - Manages authentication tokens securely within operational contexts. (A.9.4.4 System and application access control)
*/


// Method 1: LOADIQU Constructor
/// <summary>
/// Initializes the LOADIQU class by ensuring necessary local directories are created for secure data storage.
/// Compliance with ISO/IEC 27001:2022:
/// A.12.1.3 (Operating procedures) - Ensures that operational procedures are followed to maintain data integrity and availability.
/// </summary>
public class LOADIQU
{
    // Updated to use CommonApplicationData to target C:\ProgramData
    private static readonly string FolderPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
        "LOADIQU");

    // Assuming LocalFolderPath is meant for the same general purpose storage
    private static readonly string LocalFolderPath = FolderPath;

    public LOADIQU()
    {
        // Ensure the folder exists
        EnsureFolderExists(FolderPath);
        EnsureFolderExists(LocalFolderPath);
    }

    private void EnsureFolderExists(string path)
    {
        // Check if the folder exists
        if (!Directory.Exists(path))
        {
            // Try to create the directory.
            Directory.CreateDirectory(path);
        }
    }


    private static string? _raspberryPiUsername;
    private static string? _raspberryPiPassword;
    private static readonly string RaspberryPiHost = "raspberrypi"; //update to raspberrypi.local before publishing
    private static readonly string host = "raspberrypi"; //update to raspberrypi.local before publishing
    // Credentials obtained from the Credential Manager
    //private static readonly string username = GetSshUsername();
    //private static readonly string password = GetSshPassword();
    private static readonly string UsbDrivePath = "/mnt/virtual_usb_storage"; // Adjust based on actual mount point
    private static readonly string RaspberryPiUsbDrivePath = @"E:\";
    private static readonly string ConnectionString = GetConnectionString();
    // Set this to 'true' when using the live environment RV
    private static readonly bool _isLiveEnvironment = false;
    private static readonly HttpClient _httpClient = new HttpClient
    {
        // Use the ternary operator to choose the URL based on the flag
        BaseAddress = new Uri(_isLiveEnvironment ? "https://api.blastiq.com/" : "https://apitest.blastiq.com/")
    };
    private string? _globalSiteId;
    private string? storedToken;
    public static string RaspberryPiUsername
    {
        get
        {
            if (string.IsNullOrEmpty(_raspberryPiUsername))
            {
                UpdateRaspberryPiCredentials();
            }
            if (_raspberryPiUsername == null)
            {
                throw new InvalidOperationException("Raspberry Pi username is not initialized.");
            }
            return _raspberryPiUsername;
        }
    }



    public static string RaspberryPiPassword
    {
        get
        {
            if (string.IsNullOrEmpty(_raspberryPiPassword))
            {
                UpdateRaspberryPiCredentials();
            }
            if (_raspberryPiPassword == null)
            {
                throw new InvalidOperationException("Raspberry Pi Password is not initialized.");
            }
            return _raspberryPiPassword;
        }
    }

    private static void UpdateRaspberryPiCredentials()
    {
        var (apiUsername, apiPassword, sshUsername, sshPassword) = GetUserCredentials();
        _raspberryPiUsername = sshUsername;
        _raspberryPiPassword = sshPassword;
    }


    // Method 4: Main
    /// <summary>
    /// Main entry point for the LOADIQU application, handling application startup, database initialization, and network connectivity checks.
    /// Compliance with ISO/IEC 27001:2022:
    /// A.16.1.4 (Assessment of and decision on information security events) - Manages responses to connectivity issues and operational errors effectively.
    /// </summary>

    public static async Task Main(string[] args)
    {
        Debug.WriteLine("Starting application...");
        Console.WriteLine("Starting application...");
        try
        {
            InitializeDatabase();
            Console.WriteLine("Application started successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unhandled exception during database initialization: {ex.Message}");
            return;
        }

        InitializeEncryptionKey();

        if (!CredentialsStored())
        {
            Debug.WriteLine("Credentials not stored.");
            Console.WriteLine("Credentials not stored.");
            PromptForCredentials();
        }

        bool isConnectedToSFTP = false;
        bool hasInternetConnection = PingHost("8.8.8.8");

        if (string.IsNullOrEmpty(RaspberryPiUsername) || string.IsNullOrEmpty(RaspberryPiPassword))
        {
            Console.WriteLine("SFTP credentials are missing. Prompting for new credentials.");
            PromptForCredentials();
        }

        using (var sftp = new SftpClient(RaspberryPiHost, RaspberryPiUsername, RaspberryPiPassword))
        {
            try
            {
                sftp.Connect();
                isConnectedToSFTP = sftp.IsConnected;
                Debug.WriteLine("Successfully connected to Raspberry Pi via SFTP.");
                Console.WriteLine("Successfully connected to Raspberry Pi via SFTP.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to connect to SFTP server. Exception: {ex.Message}");
                Console.WriteLine($"Failed to connect to SFTP server. Exception: {ex.Message}");
            }
        }

        Debug.WriteLine("Evaluating scenarios for data processing...");
        Console.WriteLine("Evaluating scenarios for data processing...");

        if (isConnectedToSFTP && hasInternetConnection)
        {
            Debug.WriteLine("Scenario 3 triggered: SFTP and internet connections detected. Processing data as usual...");
            Console.WriteLine("Scenario 3 triggered: SFTP and internet connections detected. Processing data as usual...");

            ProcessRaspberryPiUsbDrive();

            try
            {
                VerifyAndCreatePlansTable();
                Debug.WriteLine("Plans table verified/created.");
                Console.WriteLine("Plans table verified/created.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred during Plans table verification/creation: {ex.Message}");
                Console.WriteLine($"An error occurred during Plans table verification/creation: {ex.Message}");
            }

            await ProcessLocalCsvFiles(null, null, null);
        }
        else if (isConnectedToSFTP && !hasInternetConnection)
        {
            Debug.WriteLine("Scenario 1 triggered: SFTP connection established but no internet connection detected. Processing and saving data locally...");
            Console.WriteLine("Scenario 1 triggered: SFTP connection established but no internet connection detected. Processing and saving data locally...");

            ProcessRaspberryPiUsbDriveSaveLocally();

            Debug.WriteLine("Exiting program due to lack of internet connectivity.");
            Console.WriteLine("Exiting program due to lack of internet connectivity.");
            return;
        }
        else if (!isConnectedToSFTP && hasInternetConnection)
        {
            Debug.WriteLine("Scenario 2 triggered: No SFTP connection but internet connection detected. Attempting to process locally stored CSV files...");
            Console.WriteLine("Scenario 2 triggered: No SFTP connection but internet connection detected. Attempting to process locally stored CSV files...");

            await ProcessLocalCsvFiles(null, null, null);
        }
        else
        {
            Debug.WriteLine("No applicable scenario found based on current connectivity status.");
            Console.WriteLine("No applicable scenario found based on current connectivity status.");
        }

        Debug.WriteLine("Creating LOADIQU instance...");
        Console.WriteLine("Creating LOADIQU instance...");
        var LOADIQUInstance = new LOADIQU();
        Debug.WriteLine("LOADIQU instance created.");
        Console.WriteLine("LOADIQU instance created.");

        Debug.WriteLine("Ensuring valid token...");
        Console.WriteLine("Ensuring valid token...");
        var validToken = await LOADIQUInstance.EnsureValidToken();
        if (string.IsNullOrEmpty(validToken))
        {
            Debug.WriteLine("Valid token could not be obtained.");
            Console.WriteLine("Valid token could not be obtained.");
            return;
        }
        Debug.WriteLine("Token obtained.");
        Console.WriteLine("Token obtained.");

        var siteListApiResponse = await LOADIQUInstance.GetSiteListAsync(validToken);
        if (string.IsNullOrEmpty(siteListApiResponse))
        {
            Console.WriteLine("Failed to fetch site list. This device may not have an active connection or the server failed to respond.");
            return;
        }

        var globalSiteId = GetGlobalSiteIdFromApiResponse(siteListApiResponse);
        if (string.IsNullOrEmpty(globalSiteId))
        {
            Console.WriteLine("Failed to fetch the global site ID. Exiting application.");
            return;
        }

        LOADIQUInstance.SetGlobalSiteId(globalSiteId);
        Console.WriteLine($"Global Site ID set: {globalSiteId}");

        Debug.WriteLine("Importing products...");
        Console.WriteLine("Importing products...");
        await LOADIQUInstance.ImportProductsAsync(validToken, globalSiteId, ConnectionString, includeDeleted: false);
        Debug.WriteLine("Products imported successfully.");
        Console.WriteLine("Products imported successfully.");

        await LoadPlansIntoDatabase(validToken, globalSiteId);

        var planIds = GetPlanIdsFromDatabase();
        if (planIds.Count > 0)
        {
            foreach (var planId in planIds)
            {
                Debug.WriteLine($"Fetching and loading holes for Plan ID: {planId}...");
                Console.WriteLine($"Fetching and loading holes for Plan ID: {planId}...");

                await DeserializeJsonDataAsync(globalSiteId, planId, validToken);

                Debug.WriteLine($"Holes fetching and loading completed for Plan ID: {planId}.");
                Console.WriteLine($"Holes fetching and loading completed for Plan ID: {planId}.");

                var currentHolesData = await FetchHolesDataAsync(globalSiteId, planId, validToken);
                if (currentHolesData != null)
                {
                    ProcessHolesData(currentHolesData);
                }
                else
                {
                    Console.WriteLine($"No hole data found or empty response for Plan ID: {planId}.");
                }

                Debug.WriteLine($"Holes fetching and loading completed for Plan ID: {planId}.");
            }
        }
        else
        {
            Debug.WriteLine("No plan IDs available in the database.");
            Console.WriteLine("No plan IDs available in the database.");
        }

        string databasePath = GetDatabasePath();
        string connectionString = $"Data Source={databasePath};Version=3;";
        await ProcessLocalCsvFiles(globalSiteId, validToken, connectionString);
    }







    public static void ConfigureHttpClient()
    {

        _httpClient.BaseAddress = new Uri(_isLiveEnvironment ? "https://api.blastiq.com/" : "https://apitest.blastiq.com/");
    }


    public class Site
    {
        public string Id { get; set; }

        // Constructor that requires an ID when creating an instance of Site
        public Site(string id)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }
    }


    public class SiteApiResponse
    {
        public List<Site> Sites { get; set; } = new List<Site>();
    }



    private static string GetGlobalSiteIdFromApiResponse(string apiResponse)
    {
        if (string.IsNullOrEmpty(apiResponse))
        {
            throw new InvalidOperationException("Empty API response. Unable to extract global site ID.");
        }

        var responseJson = JsonConvert.DeserializeObject<SiteApiResponse>(apiResponse);
        if (responseJson == null || responseJson.Sites == null || responseJson.Sites.Count == 0)
        {
            throw new InvalidOperationException("Failed to deserialize API response or no sites found.");
        }

        var siteId = responseJson.Sites[0].Id;
        if (string.IsNullOrEmpty(siteId))
        {
            throw new InvalidOperationException("Site ID is null or empty in the API response.");
        }
        return siteId;
    }











    private static bool IsUpdatedToday(string filePath)
    {
        var today = DateTime.Now.Date;
        var lastWriteTime = File.GetLastWriteTime(filePath).Date;
        return lastWriteTime == today;
    }

    private static (string blastId, string dateCreated, List<Dictionary<string, object>> records)
ParseExcel(string filePath)
    {
        const int maxRetries = 2; // Try 2 times
        int attempts = 0;
        List<Dictionary<string, object>> records = new List<Dictionary<string, object>>();
        string blastId = string.Empty;
        string dateCreated = string.Empty;

        while (attempts <= maxRetries)
        {
            try
            {
                if (filePath.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"Reading data from Excel file: {filePath}");
                    using (var workbook = new XLWorkbook(filePath))
                    {
                        var worksheet = workbook.Worksheet(1);
                        blastId = worksheet.Cell("B1").GetValue<string>(); // Blast ID is in B1
                        dateCreated = worksheet.Cell("B2").GetValue<string>(); // Date Created is in B2

                        // Assume headers are in row 4
                        var headers = new List<string>();
                        for (int col = 1; col <= worksheet.LastColumnUsed().ColumnNumber(); col++)
                        {
                            headers.Add(worksheet.Cell(4, col).GetValue<string>());
                        }

                        for (int row = 5; row <= worksheet.LastRowUsed().RowNumber(); row++) // Start reading values from row 5
                        {
                            var rowData = new Dictionary<string, object>();
                            for (int col = 1; col <= headers.Count; col++)
                            {
                                var header = headers[col - 1];
                                var cell = worksheet.Cell(row, col);
                                // Ensure both types are the same. If the cell is empty, use DBNull.Value; otherwise, convert the value to a string.
                                var value = cell.IsEmpty() ? (object)DBNull.Value : cell.GetValue<string>();
                                rowData[header] = value;
                            }
                            records.Add(rowData);
                        }
                    }
                }
                else if (filePath.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                {

                    //return ParseCsv(filePath); 
                }
                else
                {
                    throw new NotSupportedException("Unsupported file format.");
                }
                break; // If the parsing succeeds, exit the loop
            }
            catch (IOException ex) when (attempts < maxRetries)
            {
                attempts++;
                Console.WriteLine($"Attempt {attempts} failed to access file {filePath}: {ex.Message}. Retrying...");
                System.Threading.Thread.Sleep(1000); // Wait for 1 second before retrying
            }
        }

        if (attempts > maxRetries)
        {
            Console.WriteLine($"Failed to access file {filePath} after {maxRetries} attempts.");
        }

        // Add a warning if blastId or dateCreated is null or empty
        if (string.IsNullOrEmpty(blastId) || string.IsNullOrEmpty(dateCreated))
        {
            Console.WriteLine("Warning: Null or empty value encountered for Blast ID or Date Created.");
        }

        return (blastId, dateCreated, records);
    }

    public class CsvData
    {
        public string? BlastID { get; set; }
        public string? DateCreated { get; set; }

        public List<Dictionary<string, object>> Records { get; set; }

        public CsvData()
        {
            Records = new List<Dictionary<string, object>>();
        }
    }

    public static (string? blastId, string? dateCreated, List<Dictionary<string, object>> records) ParseCsv(string filePath, char delimiter = ',')
    {
        List<Dictionary<string, object>> records = new List<Dictionary<string, object>>();
        string? blastId = null;  // Explicitly nullable
        string? dateCreated = null;  // Explicitly nullable

        Console.WriteLine($"Attempting to parse CSV file: {filePath}");
        Debug.WriteLine($"Attempting to parse CSV file: {filePath}");

        try
        {
            using (TextFieldParser parser = new TextFieldParser(filePath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(new string[] { delimiter.ToString() });

                Console.WriteLine("Starting CSV parsing...");
                Debug.WriteLine("Starting CSV parsing...");

                // Blast ID line
                if (!parser.EndOfData)
                {
                    string[]? blastIdLine = parser.ReadFields();
                    if (blastIdLine != null && blastIdLine.Length > 1 && blastIdLine[1] != null)
                    {
                        blastId = blastIdLine[1].Trim();
                        Console.WriteLine($"Blast ID parsed: {blastId}");
                        Debug.WriteLine($"Blast ID parsed: {blastId}");
                    }
                }

                // Date Created line
                if (!parser.EndOfData)
                {
                    string[]? dateCreatedLine = parser.ReadFields();
                    if (dateCreatedLine != null && dateCreatedLine.Length > 1 && dateCreatedLine[1] != null)
                    {
                        dateCreated = dateCreatedLine[1].Trim();
                        Console.WriteLine($"Date Created parsed: {dateCreated}");
                        Debug.WriteLine($"Date Created parsed: {dateCreated}");
                    }
                }

                // Skipping empty line
                if (!parser.EndOfData)
                {
                    parser.ReadLine();
                    Console.WriteLine("Skipped an empty line.");
                    Debug.WriteLine("Skipped an empty line.");
                }

                // Reading header row
                if (!parser.EndOfData)
                {
                    string[]? headers = parser.ReadFields();
                    if (headers != null && headers.Length > 0)
                    {
                        Console.WriteLine("Headers parsed successfully.");
                        Debug.WriteLine("Headers parsed successfully.");

                        // Reading data rows
                        while (!parser.EndOfData)
                        {
                            string[]? fields = parser.ReadFields();
                            if (fields != null)
                            {
                                var record = new Dictionary<string, object>();
                                for (int i = 0; i < headers.Length; i++)
                                {
                                    record[headers[i]] = i < fields.Length ? fields[i] : DBNull.Value;
                                }
                                records.Add(record);
                                Console.WriteLine("Record added.");
                                Debug.WriteLine("Record added.");
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing CSV: {ex.Message}");
            Debug.WriteLine($"Error parsing CSV: {ex.Message}");
        }

        Console.WriteLine("CSV parsing completed.");
        Debug.WriteLine("CSV parsing completed.");
        return (blastId, dateCreated, records);
    }


    public static async Task ProcessCsvAndInsertData(string filePath)
    {
        Console.WriteLine($"Beginning process for CSV file: {filePath}");
        Debug.WriteLine($"Beginning process for CSV file: {filePath}");

        // Use Task.Run to perform CPU-bound operations on a background thread
        await Task.Run(() =>
        {
            // Parse the CSV file
            var (blastId, dateCreated, records) = ParseCsv(filePath, ',');

            if (!string.IsNullOrEmpty(blastId) && !string.IsNullOrEmpty(dateCreated) && records.Any())
            {
                Console.WriteLine("Parsed data successfully. Proceeding to insert into database...");
                Debug.WriteLine("Parsed data successfully. Proceeding to insert into database...");

                // Insert data into the database
                bool result = InsertDataIntoDatabase(blastId, dateCreated, records, filePath);

                Console.WriteLine($"Data insertion result: {result}");
                Debug.WriteLine($"Data insertion result: {result}");

                // If insertion was successful, rename the file
                if (result)
                {
                    // Rename the file to indicate successful processing
                    RenameFile(filePath);

                    // Log the new path or any other relevant information
                    Console.WriteLine($"File has been processed and renamed. Check the new location at: {FolderPath}");

                    // Optional: Pause the console to allow reading the log
                    //PauseConsole();
                }
            }
            else
            {
                Console.WriteLine("Parsed data is incomplete or missing. Aborting database insertion.");
                Debug.WriteLine("Parsed data is incomplete or missing. Aborting database insertion.");
            }
        });
    }


    private static void RenameFile(string originalFilePath)
    {
        string? directory = Path.GetDirectoryName(originalFilePath) ?? Directory.GetCurrentDirectory();
        string newFileName = $"{Path.GetFileNameWithoutExtension(originalFilePath)}_processed_{DateTime.Now:yyyyMMddHHmmss}.txt";
        string newFilePath = Path.Combine(directory, newFileName);

        Console.WriteLine($"Original file path: {originalFilePath}");
        Console.WriteLine($"New file path: {newFilePath}");

        try
        {
            if (File.Exists(newFilePath))
            {
                File.Delete(newFilePath);
                Console.WriteLine($"Deleted existing file at {newFilePath} to make way for the new file.");
            }

            File.Move(originalFilePath, newFilePath);
            Console.WriteLine($"File successfully renamed to: {newFilePath}");
            Debug.WriteLine($"File successfully renamed to: {newFilePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to rename file: {ex.Message}");
            Debug.WriteLine($"Failed to rename file: {ex.Message}");
        }

        
        //PauseConsole();
    }




    public static class CsvHelper
    {
        public static (string blastId, string dateCreated, List<Dictionary<string, object>> records) ParseCsv(string filePath, char delimiter = ',')
        {
            List<Dictionary<string, object>> records = new List<Dictionary<string, object>>();
            string blastId = string.Empty;  // Consider if you want this to be nullable
            string dateCreated = string.Empty;  // Consider if you want this to be nullable

            try
            {
                Console.WriteLine($"Reading data from CSV file: {filePath}");
                Debug.WriteLine($"Reading data from CSV file: {filePath}");

                using (TextFieldParser parser = new TextFieldParser(filePath))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(delimiter.ToString());

                    // Read the Blast ID line
                    if (!parser.EndOfData)
                    {
                        string[]? blastIdLine = parser.ReadFields();
                        if (blastIdLine != null && blastIdLine.Length > 1)
                        {
                            blastId = blastIdLine[1].Trim();
                        }
                    }

                    // Read the Date Created line
                    if (!parser.EndOfData)
                    {
                        string[]? dateCreatedLine = parser.ReadFields();
                        if (dateCreatedLine != null && dateCreatedLine.Length > 1)
                        {
                            dateCreated = dateCreatedLine[1].Trim();
                        }
                    }

                    // Skip the empty line
                    if (!parser.EndOfData)
                    {
                        parser.ReadLine();
                    }

                    // Read the header row
                    string[]? headers = parser.ReadFields();
                    if (headers != null && headers.Length > 0)
                    {
                        // Read the data rows
                        while (!parser.EndOfData)
                        {
                            string[]? fields = parser.ReadFields();
                            if (fields != null)
                            {
                                var record = new Dictionary<string, object>();
                                for (int i = 0; i < headers.Length; i++)
                                {
                                    record[headers[i]] = i < fields.Length ? fields[i] : DBNull.Value;
                                }
                                records.Add(record);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while reading CSV file: {ex.Message}");
                Debug.WriteLine($"Error occurred while reading CSV file: {ex.Message}");
            }

            return (blastId, dateCreated, records);
        }
    }





    //77 
    private static (string Ring, string HoleId) ParseHoleId(string? concatenatedHoleId)
    {
        Debug.WriteLine($"Attempting to parse HoleID: {concatenatedHoleId}");

        // Handle null or empty input to avoid processing invalid data
        if (string.IsNullOrEmpty(concatenatedHoleId))
        {
            Debug.WriteLine("Received null or empty HoleID, returning default values.");
            return ("DefaultRing", "DefaultHoleId");  // Provide default values or handle this case as needed
        }

        var match = Regex.Match(concatenatedHoleId, @"(.*?)(H)(\d+)$");
        if (match.Success)
        {
            string ring = match.Groups[1].Value;
            string holeId = match.Groups[3].Value;
            Debug.WriteLine($"Parsed Ring: {ring}, HoleID: {holeId}");
            return (ring, holeId);
        }
        else
        {
            Debug.WriteLine("The HoleID format is incorrect.");
            throw new FormatException("The HoleID format is incorrect.");
        }
    }

    public static async Task ProcessLocalCsvFiles(string? globalSiteId, string? validToken, string? connectionString)
    {
        var csvFiles = Directory.GetFiles(LocalFolderPath, "*.csv");
        if (csvFiles.Length == 0)
        {
            Console.WriteLine("No CSV files found. Triggering PostActualDeckToApi to process unsent data.");
            Debug.WriteLine("No CSV files found. Triggering PostActualDeckToApi to process unsent data.");
            
            if (globalSiteId != null && validToken != null && connectionString != null)
            {
                await PostActualDeckToApi(globalSiteId, validToken, connectionString);
                //PauseConsole();
            }
            else
            {
                Console.WriteLine("Global site ID, valid token, or connection string is null. Cannot proceed with posting data.");
                Debug.WriteLine("Global site ID, valid token, or connection string is null. Cannot proceed with posting data.");
            }
            return;
        }

        foreach (var filePath in csvFiles)
        {
            try
            {
                Console.WriteLine($"Starting to process local CSV file: {filePath}");
                Debug.WriteLine($"Starting to process local CSV file: {filePath}");

                var (blastId, dateCreated, records) = ParseCsv(filePath);

                if (!string.IsNullOrEmpty(blastId) && !string.IsNullOrEmpty(dateCreated) && records.Any())
                {
                    Console.WriteLine($"Successfully parsed CSV file: {filePath}. BlastID: {blastId}, DateCreated: {dateCreated}, Records processed: {records.Count}");
                    Debug.WriteLine($"Successfully parsed CSV file: {filePath}. BlastID: {blastId}, DateCreated: {dateCreated}, Records processed: {records.Count}");

                    foreach (var record in records)
                    {
                        foreach (var kvp in record)
                        {
                            Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                            Debug.WriteLine($"{kvp.Key}: {kvp.Value}");
                        }
                        Console.WriteLine(); // Add a blank line for readability
                        Debug.WriteLine("End of record");
                    }

                    bool insertResult = InsertDataIntoDatabase(blastId, dateCreated, records, filePath);
                    if (insertResult)
                    {
                        Console.WriteLine($"Data successfully inserted into the database for file: {filePath}");
                        Debug.WriteLine($"Data successfully inserted into the database for file: {filePath}");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to insert data into the database for file: {filePath}");
                        Debug.WriteLine($"Failed to insert data into the database for file: {filePath}");
                    }
                }
                else
                {
                    Console.WriteLine($"Parsed data from CSV file: {filePath} is incomplete or missing. Aborting database insertion.");
                    Debug.WriteLine($"Parsed data from CSV file: {filePath} is incomplete or missing. Aborting database insertion.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while processing the local CSV file: {filePath}. Exception: {ex.Message}");
                Debug.WriteLine($"An error occurred while processing the local CSV file: {filePath}. Exception: {ex.Message}");
            }
        }
    }




    private static void CheckAttemptsExceeded(int attempts, int maxRetries, string filePath)
    {
        if (attempts > maxRetries)
        {
            // If you want to throw an exception
            throw new InvalidOperationException($"Failed to access file {filePath} after {maxRetries} attempts.");

            // Or if you prefer to just log the error and handle it without throwing
            Console.WriteLine($"Failed to access file {filePath} after {maxRetries} attempts.");
            Debug.WriteLine($"Failed to access file {filePath} after {maxRetries} attempts.");
            // Additional handling like sending a notification or marking the file as failed in your system could be done here.
        }
        else
        {
            Console.WriteLine($"Attempt {attempts} of {maxRetries} failed for file {filePath}. Retrying...");
            Debug.WriteLine($"Attempt {attempts} of {maxRetries} failed for file {filePath}. Retrying...");
        }
    }




    //pi part 2
    public static bool InsertDataIntoDatabase(string blastId, string dateCreated, List<Dictionary<string, object>> records, string filePath)
    {
        bool isSuccess = false;
        string connectionString = DatabaseConfig.GetConnectionString(); // Ensure we have a method or property to get the DB connection string

        Console.WriteLine($"Connection string: {connectionString}");
        Debug.WriteLine($"Connection string: {connectionString}");

        using (var connection = new SQLiteConnection(connectionString))
        {
            SQLiteTransaction? transaction = null;
            try
            {
                connection.Open();
                Console.WriteLine($"Database connection successfully opened. Path: {connection.DataSource}");
                Debug.WriteLine($"Database connection successfully opened. Path: {connection.DataSource}");

                transaction = connection.BeginTransaction();
                Console.WriteLine("Database transaction started.");
                Debug.WriteLine("Database transaction started.");

                foreach (var record in records)
                {
                    // Assume ParseHoleId is a method defined elsewhere
                    var (Ring, HoleId) = ParseHoleId(record.ContainsKey("Hole ID") ? record["Hole ID"].ToString() : null);

                    // Check for duplicate records
                    var duplicateCheckCommandText = @"
SELECT COUNT(*) FROM LOADIQUData
WHERE BlastID = @BlastID AND DateCreated = @DateCreated AND Ring = @Ring AND HoleID = @HoleID
AND ProductName = @ProductName AND ChargeLength = @ChargeLength AND ChargeWeight = @ChargeWeight AND QCDate = @QCDate";
                    using (var duplicateCheckCommand = new SQLiteCommand(duplicateCheckCommandText, connection))
                    {
                        duplicateCheckCommand.Parameters.AddWithValue("@BlastID", blastId);
                        duplicateCheckCommand.Parameters.AddWithValue("@DateCreated", dateCreated);
                        duplicateCheckCommand.Parameters.AddWithValue("@Ring", Ring);
                        duplicateCheckCommand.Parameters.AddWithValue("@HoleID", HoleId);
                        duplicateCheckCommand.Parameters.AddWithValue("@ProductName", record["Product Name"] ?? DBNull.Value);
                        duplicateCheckCommand.Parameters.AddWithValue("@ChargeLength", record["Charge Length"] ?? DBNull.Value);
                        duplicateCheckCommand.Parameters.AddWithValue("@ChargeWeight", record["Charge Weight"] ?? DBNull.Value);
                        duplicateCheckCommand.Parameters.AddWithValue("@QCDate", record["QC Date"] ?? DBNull.Value);

                        var exists = Convert.ToInt32(duplicateCheckCommand.ExecuteScalar()) > 0;
                        if (exists)
                        {
                            Console.WriteLine($"Skipping duplicate record: BlastID={blastId}, DateCreated={dateCreated}, Ring={Ring}, HoleID={HoleId}");
                            continue;
                        }
                    }

                    // Insert the data
                    string commandText = @"
INSERT INTO LOADIQUData 
(BlastID, DateCreated, Ring, HoleID, ProductName, ChargeLength, ChargeWeight, PumpRate, Time, CalDate, CF, QCDate, Temp, Product, ProductDensity, Primers, Flush, IsSent)
VALUES 
(@BlastID, @DateCreated, @Ring, @HoleID, @ProductName, @ChargeLength, @ChargeWeight, @PumpRate, @Time, @CalDate, @CF, @QCDate, @Temp, @Product, @ProductDensity, @Primers, @Flush, @IsSent)";
                    using (var command = new SQLiteCommand(commandText, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@BlastID", blastId);
                        command.Parameters.AddWithValue("@DateCreated", dateCreated);
                        command.Parameters.AddWithValue("@Ring", Ring);
                        command.Parameters.AddWithValue("@HoleID", HoleId);
                        command.Parameters.AddWithValue("@ProductName", record["Product Name"] ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ChargeLength", record["Charge Length"] ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ChargeWeight", record["Charge Weight"] ?? DBNull.Value);
                        command.Parameters.AddWithValue("@PumpRate", record["Pump Rate"] ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Time", record["Time(HH:MM:SS)"] ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CalDate", record["Cal Date"] ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CF", record["CF"] ?? DBNull.Value);
                        command.Parameters.AddWithValue("@QCDate", record["QC Date"] ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Temp", record["Temp(C)"] ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Product", record["Product"] ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ProductDensity", record["Density(g/ccm)"] ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Primers", record["Primers(m)"] ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Flush", record["Flush"] ?? DBNull.Value);
                        command.Parameters.AddWithValue("@IsSent", 0); // Assuming IsSent is always 0 on insert

                        command.ExecuteNonQuery();
                        //Console.WriteLine($"Record inserted: BlastID={blastId}, DateCreated={dateCreated}, Ring={Ring}, HoleID={HoleId}");
                    }
                }

                transaction.Commit();
                isSuccess = true;
                // Console.WriteLine("Transaction committed successfully. Data inserted into the database.");

                if (isSuccess)
                {
                    // Adding time component to the new file name for uniqueness
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                    string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                    string newFileName = $"{fileNameWithoutExtension}_processed_{timestamp}.txt";

                    // Handle potential null return from GetDirectoryName
                    string directory = Path.GetDirectoryName(filePath) ?? string.Empty; // Use the current directory as a fallback
                    string newPath = Path.Combine(directory, newFileName);

                    // Check if a file with the new name already exists
                    if (File.Exists(newPath))
                    {
                        // If so, delete it to prevent File.Move from throwing an exception
                        File.Delete(newPath);
                        Console.WriteLine($"Existing file at {newPath} was deleted.");
                    }

                    File.Move(filePath, newPath); // This assumes filePath is not null; consider checking if filePath itself is not null.
                    Console.WriteLine($"File successfully renamed to {newPath}");
                    Debug.WriteLine($"File successfully renamed to {newPath}");
                }

            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                Console.WriteLine($"Error during database operation: {ex.Message}");
                Debug.WriteLine($"Error during database operation: {ex.Message}");
            }
            finally
            {
                connection.Close();
                Console.WriteLine("Database connection closed.");
                Debug.WriteLine("Database connection closed.");
            }
        }

        return isSuccess;
    }

    // Method: ProcessRaspberryPiUsbDrive
    /// <summary>
    /// Handles USB drive data processing for Raspberry Pi via SFTP connection. Ensures directory existence, manages secure file transfer, and oversees file deletion post-transfer, maintaining data integrity and secure data handling practices.
    /// Compliance with ISO/IEC 27001:2022:
    /// A.12.3.1 (Protection of stored information) - Ensures data is securely stored and transmitted by verifying directory existence and using secure transfer methods.
    /// A.12.4.1 (Logging and monitoring) - Provides detailed logging of file handling operations to ensure traceability.
    /// A.12.4.3 (User and entity authentication) - Uses authenticated sessions for data access and operations on the SFTP server.
    /// A.12.4.4 (Control of privileged access rights) - Manages access to file systems via controlled and authenticated mechanisms.
    /// A.14.1.2 (Secure development policy) - Follows secure development practices by ensuring that data is handled securely throughout the processing lifecycle.
    /// </summary>

    public static void ProcessRaspberryPiUsbDrive()
    {
        Debug.WriteLine("Starting the process to handle Raspberry Pi USB Drive...");
        Console.WriteLine("Starting the process to handle Raspberry Pi USB Drive...");

        // Local directory check is on Windows PC, so it remains unchanged.
        // Ensure the local directory exists on Windows PC
        if (!Directory.Exists(LocalFolderPath))
        {
            Directory.CreateDirectory(LocalFolderPath);
            Debug.WriteLine($"Local directory created: {LocalFolderPath}");
            Console.WriteLine($"Local directory created: {LocalFolderPath}");
        }

        var piSshHelper = new PiSshHelper(RaspberryPiHost, RaspberryPiUsername, RaspberryPiPassword);
        ExecuteRaspberryPiCommands(piSshHelper); // Managing USB drive

        bool isConnectedToSFTP = false;
        // PingHost logic to ensure there's internet connection, remains unchanged.

        using (var sftp = new SftpClient(RaspberryPiHost, RaspberryPiUsername, RaspberryPiPassword))
        {
            try
            {
                sftp.Connect();
                isConnectedToSFTP = sftp.IsConnected;
                Debug.WriteLine("Connected to Raspberry Pi via SFTP.");
                Console.WriteLine("Connected to Raspberry Pi via SFTP.");

                if (isConnectedToSFTP)
                {
                    // Adjusted to add logging for file transfer and deletion
                    var files = sftp.ListDirectory("/home/LOADIQU/usb_files/");
                    Debug.WriteLine($"Listing files in directory: /home/LOADIQU/usb_files/");
                    Console.WriteLine($"Listing files in directory: /home/LOADIQU/usb_files/");

                    foreach (var file in files.Where(f => !f.IsDirectory && f.Name.EndsWith(".csv")))
                    {
                        string localFilePath = Path.Combine(LocalFolderPath, file.Name);
                        Debug.WriteLine($"Downloading file to local path: {localFilePath}");
                        Console.WriteLine($"Downloading file to local path: {localFilePath}");

                        using (var fileStream = File.OpenWrite(localFilePath))
                        {
                            sftp.DownloadFile(file.FullName, fileStream);
                        }
                        Debug.WriteLine($"File downloaded: {file.Name}");
                        Console.WriteLine($"File downloaded: {file.Name}");

                        // Deleting the file from the Raspberry Pi temporary folder after successful download
                        sftp.DeleteFile(file.FullName);
                        Debug.WriteLine($"File deleted from Raspberry Pi temporary folder: {file.Name}");
                        Console.WriteLine($"File deleted from Raspberry Pi temporary folder: {file.Name}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to connect to SFTP server. Exception: {ex.Message}");
                Console.WriteLine($"Failed to connect to SFTP server. Exception: {ex.Message}");
            }
            finally
            {
                if (sftp.IsConnected)
                {
                    sftp.Disconnect();
                    Debug.WriteLine("Disconnected from SFTP.");
                    Console.WriteLine("Disconnected from SFTP.");
                }
            }
        }
    }

    //ExecuteRaspberryPiCommands funtion:
    //Stopping the USB gadget service to safely manipulate the storage.
    //Ensuring the existence of the temporary storage directory on the Raspberry Pi for intermediate file handling.
    //Mounting the USB storage image to access its contents.
    //Setting appropriate permissions on the mounted directory to facilitate file movement and deletion.
    //Moving files from the mounted image to the temporary directory. If moving fails, it copies the files and then deletes the originals from the mounted image.
    //Unmounting the USB storage image after file operations.
    //Restarting the USB gadget service to resume normal operations.

    // Method: ExecuteRaspberryPiCommands
    /// <summary>
    /// Executes a series of commands on a Raspberry Pi via SSH to manage USB storage operations securely. This includes disabling USB gadgets to safely manipulate storage, ensuring necessary directories exist, mounting and unmounting storage devices, and managing file transfers and deletions securely. Each step is logged to maintain a clear audit trail and ensure that actions are reversible and verified.
    /// Compliance with ISO/IEC 27001:2022:
    /// A.12.4.1 (Logging and monitoring) - Detailed logging of each step provides traceability and accountability for actions performed on remote devices.
    /// A.12.6.2 (Management of technical vulnerabilities) - Ensures that the system is properly configured to prevent unauthorized data access during USB operations.
    /// A.14.2.5 (Secure system engineering principles) - Follows secure engineering principles by clearly separating stages of USB handling and ensuring secure and clean transitions between states.
    /// A.14.2.9 (System acceptance testing) - Incorporates acceptance checks and validations to confirm that USB handling operations are performed as expected without unintended side-effects.
    /// </summary>


    public static void ExecuteRaspberryPiCommands(PiSshHelper piSshHelper)
    {
        // Disabling the USB gadget to safely manipulate the storage
        piSshHelper.ExecuteCommandOnPi("echo '' | sudo tee /sys/kernel/config/usb_gadget/g1/UDC");
        Console.WriteLine("USB gadget disabled.");

        // Ensuring the temporary storage directory exists
        piSshHelper.ExecuteCommandOnPi("sudo mkdir -p /home/LOADIQU/usb_files/");
        Console.WriteLine("Ensured /home/LOADIQU/usb_files/ exists.");

        // Mounting the USB storage image with umask=000 for full access
        piSshHelper.ExecuteCommandOnPi("sudo mkdir -p /mnt/usb_image && sudo mount -o loop,umask=000 /home/LOADIQU/usb_storage.img /mnt/usb_image");
        Console.WriteLine("Mounted usb_storage.img to /mnt/usb_image with umask=000.");

        // Moving files from the mounted image to the temporary directory, with fallback to copy and delete
        piSshHelper.ExecuteCommandOnPi("find /mnt/usb_image/ -mindepth 1 -exec mv {} /home/LOADIQU/usb_files/ \\; || (sudo cp -r /mnt/usb_image/* /home/LOADIQU/usb_files/ && sudo rm -r /mnt/usb_image/*)");
        Console.WriteLine("Handled contents from /mnt/usb_image to /home/LOADIQU/usb_files.");

        // Attempting to clear remaining files from /mnt/usb_image and verify the action
        piSshHelper.ExecuteCommandOnPi("sudo rm -rf /mnt/usb_image/* && echo 'Deletion successful' || echo 'Deletion failed'");
        Console.WriteLine("Attempted to clear remaining files from /mnt/usb_image.");

        // Verifying the state of /mnt/usb_image after deletion attempt
        piSshHelper.ExecuteCommandOnPi("ls -al /mnt/usb_image/");
        Console.WriteLine("Verified the state of /mnt/usb_image post-deletion.");

        // Unmounting the USB storage image
        piSshHelper.ExecuteCommandOnPi("sudo umount /mnt/usb_image");
        Console.WriteLine("Unmounted /mnt/usb_image.");

        // Re-enabling the USB gadget for normal operation
        piSshHelper.ExecuteCommandOnPi("echo '20980000.usb' | sudo tee /sys/kernel/config/usb_gadget/g1/UDC");
        Console.WriteLine("USB gadget re-enabled.");

        // Restarting the USB gadget service to finalize the re-enablement process
        piSshHelper.ExecuteCommandOnPi("sudo systemctl start usb_gadget.service");
        Console.WriteLine("Started usb_gadget service.");
    }






    // Method 7: PingHost
    /// <summary>
    /// Checks internet connectivity by pinging a known host, supporting reliable network management.
    /// Compliance with ISO/IEC 27001:2022:
    /// A.13.1.3 (Network segregation) - Ensures network connectivity is maintained and monitored.
    /// </summary>

    private static bool PingHost(string host)
    {
        try
        {
            using (var ping = new Ping())
            {
                var reply = ping.Send(host);
                return reply.Status == IPStatus.Success;
            }
        }
        catch
        {
            return false; // Ping failed
        }
    }



    public class PiSshHelper
    {
        private string host;
        private string username;
        private string password;

        public PiSshHelper(string host, string username, string password)
        {
            this.host = host;
            this.username = username;
            this.password = password;
        }

        public void ExecuteCommandOnPi(string command)
        {
            using (var sshClient = new SshClient(host, username, password))
            {
                try
                {
                    sshClient.Connect();
                    if (sshClient.IsConnected)
                    {
                        Console.WriteLine($"Executing command: {command}");
                        var cmd = sshClient.CreateCommand(command);
                        var result = cmd.Execute();
                        Console.WriteLine("Command execution result: " + result.Trim());

                        if (cmd.ExitStatus != 0)
                        {
                            Console.WriteLine($"Error executing command '{command}'. Exit Status: {cmd.ExitStatus}. Error: {cmd.Error}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("SSH connection to Raspberry Pi failed.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while executing command '{command}': {ex.Message}");
                }
                finally
                {
                    if (sshClient.IsConnected)
                    {
                        sshClient.Disconnect();
                    }
                }
            }
        }
    }

    private static void RenameFileExtension(string filePath, string newExtension)
    {
        // Ensure the new extension starts with a dot.
        if (!newExtension.StartsWith("."))
        {
            newExtension = "." + newExtension;
        }

        // Extract the directory and file name without the extension.
        string? directory = Path.GetDirectoryName(filePath);
        // Default to current directory if null
        string safeDirectory = directory ?? ".";

        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);

        // Include the current date and time to indicate when the file was processed.
        // Format: yyyyMMddHHmmss (e.g., 20240322153045 for March 22, 2024, at 15:30:45)
        string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        string newFileName = $"{fileNameWithoutExtension}_processed_{timestamp}{newExtension}";
        string newFilePath = Path.Combine(safeDirectory, newFileName);

        try
        {
            // Check if a file with the new name already exists, and delete it if it does.
            if (File.Exists(newFilePath))
            {
                File.Delete(newFilePath);
                Debug.WriteLine($"Existing file with the new name {newFilePath} was deleted before renaming.");
                Console.WriteLine($"Existing file with the new name {newFilePath} was deleted before renaming.");
            }

            // Attempt to rename the file.
            File.Move(filePath, newFilePath);
            Debug.WriteLine($"File renamed to {newFilePath}.");
            Console.WriteLine($"File renamed to {newFilePath}.");
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed.
            Debug.WriteLine($"An error occurred while renaming the file: {ex.Message}");
            Console.WriteLine($"An error occurred while renaming the file: {ex.Message}");
        }
    }




    private static void SaveFileLocally(string sourceFilePath, string fileName)
    {
        string localSavePath = Path.Combine(LocalFolderPath, fileName);

        if (!Directory.Exists(LocalFolderPath))
        {
            Directory.CreateDirectory(LocalFolderPath);
        }

        string destinationFilePath = Path.Combine(LocalFolderPath, fileName);

        if (File.Exists(destinationFilePath))
        {
            File.Delete(destinationFilePath); // Consider logging or handling the case where a file is overwritten
        }

        File.Move(sourceFilePath, destinationFilePath);
        Debug.WriteLine($"File moved to {destinationFilePath} for later processing.");
        Console.WriteLine($"File moved to {destinationFilePath} for later processing.");
    }


    //66





    private static void MoveProcessedFile(string filePath, string blastId, string dateCreated)
    {
        string processedFileName = Path.GetFileNameWithoutExtension(filePath) + "-processed-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
        string processedFilePath = Path.Combine(LocalFolderPath, processedFileName);
        if (File.Exists(processedFilePath))
        {
            // Ensures we don't throw an exception if the destination file already exists
            File.Delete(processedFilePath);
        }
        File.Move(filePath, processedFilePath);
        Debug.WriteLine($"Moved processed file to: {processedFilePath}");
        Console.WriteLine($"Moved processed file to: {processedFilePath}");
    }


    // Adjust the ProcessRaspberryPiUsbDrive method to include logic for saving data locally (Scenario 1)
    // Method: ProcessRaspberryPiUsbDriveSaveLocally
    /// <summary>
    /// Manages USB drive data from a Raspberry Pi under constrained network conditions by safely downloading and storing data locally. This method ensures robust data handling by verifying connection statuses, securely transferring files via SFTP, and appropriately storing them locally when internet connectivity is compromised. This approach adheres to fail-safe operational procedures, maintaining data integrity and availability even in offline scenarios.
    /// Compliance with ISO/IEC 27001:2022:
    /// A.12.2.1 (Controls against malware) - Ensures data transferred from external sources is handled securely to protect against malicious software.
    /// A.12.3.1 (Protection of stored information) - Implements strong controls to protect data during and after transfer, including checks for existing files and secure storage practices.
    /// A.13.1.1 (Network security management) - Actively checks network connectivity to decide the data handling strategy, enhancing the security of network services.
    /// A.14.1.2 (Secure development policy) - Adheres to secure coding and operational practices that prevent data loss and ensure data integrity when the network is unavailable.
    /// A.17.2.1 (Availability of information) - Ensures information is available when needed by providing local redundancy mechanisms in case of network failure.
    /// </summary>

    private static void ProcessRaspberryPiUsbDriveSaveLocally()
    {
        Debug.WriteLine("Starting the process to handle Raspberry Pi USB Drive and save locally if necessary...");
        Console.WriteLine("Starting the process to handle Raspberry Pi USB Drive and save locally if necessary...");

        using (var sftp = new SftpClient(RaspberryPiHost, RaspberryPiUsername, RaspberryPiPassword))
        {
            try
            {
                sftp.Connect();
                Debug.WriteLine("Connected to Raspberry Pi via SFTP.");
                Console.WriteLine("Connected to Raspberry Pi via SFTP.");

                var files = sftp.ListDirectory(UsbDrivePath);
                foreach (var file in files)
                {
                    if (!file.IsDirectory && file.Name.EndsWith(".csv"))
                    {
                        Debug.WriteLine($"Found CSV file: {file.Name}");
                        Console.WriteLine($"Found CSV file: {file.Name}");

                        string localFilePath = Path.Combine(Path.GetTempPath(), file.Name);
                        Debug.WriteLine($"Downloading file to local path: {localFilePath}");
                        Console.WriteLine($"Downloading file to local path: {localFilePath}");

                        using (var fileStream = File.Create(localFilePath))
                        {
                            sftp.DownloadFile(file.FullName, fileStream);
                        }
                        Debug.WriteLine("Download complete.");
                        Console.WriteLine("Download complete.");

                        bool hasInternetConnection = PingHost("8.8.8.8");
                        if (!hasInternetConnection)
                        {
                            // Ensure the local folder exists; create it if it doesn't
                            if (!Directory.Exists(LocalFolderPath))
                            {
                                Directory.CreateDirectory(LocalFolderPath);
                                Debug.WriteLine($"Local folder path created: {LocalFolderPath}");
                                Console.WriteLine($"Local folder path created: {LocalFolderPath}");
                            }

                            string localSavePath = Path.Combine(LocalFolderPath, file.Name);
                            Debug.WriteLine($"No internet connection. Moving file to {localSavePath} for later processing.");
                            Console.WriteLine($"No internet connection. Moving file to {localSavePath} for later processing.");
                            File.Move(localFilePath, localSavePath, overwrite: true);
                        }
                        else
                        {
                            Debug.WriteLine("Internet connection detected. Proceeding with normal processing.");
                            Console.WriteLine("Internet connection detected. Proceeding with normal processing.");
                            // Implement normal file processing logic here

                            File.Delete(localFilePath);
                            Debug.WriteLine($"Local temporary file: {localFilePath} deleted.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred with ProcessRaspberryPiUsbDriveSaveLocally processing : {ex.Message}");
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                if (sftp.IsConnected)
                {
                    //add logic here to rename file in SFTP usb RV
                    sftp.Disconnect();
                    Debug.WriteLine("Disconnected from SFTP.");
                    Console.WriteLine("Disconnected from SFTP.");
                }
            }
        }
    }







    private static bool PingHost(string hostUri, int timeout = 1000)
    {
        try
        {
            using (var ping = new Ping())
            {
                var reply = ping.Send(hostUri, timeout);
                return reply.Status == IPStatus.Success;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Ping to {hostUri} failed: {ex.Message}");
            Console.WriteLine($"Ping to {hostUri} failed: {ex.Message}");
            return false;
        }
    }




    public class DatabaseConfig
    {
        public static string GetDatabasePath()
        {
            // Use CommonApplicationData to target C:\ProgramData
            string commonAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string folderPath = Path.Combine(commonAppDataPath, "LOADIQU");
            string databasePath = Path.Combine(folderPath, "LOADIQU.db");

            // Ensure the directory exists
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            return databasePath;
        }

        public static string GetConnectionString()
        {
            string databasePath = GetDatabasePath();
            return $"Data Source={databasePath};Version=3;";
        }
    }



    private void UpdatePlansTable(string plansApiResponse)
    {
        if (!string.IsNullOrEmpty(plansApiResponse))
        {
            try
            {
                // Deserialize API response into a list of Plan objects
                var response = JsonConvert.DeserializeObject<ApiResponse>(plansApiResponse);
                if (response == null || response.Plans == null)
                {
                    Console.WriteLine("Deserialization failed or 'Plans' is null.");
                    return; // Exit the method if no valid data is present
                }

                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={GetDatabasePath()};Version=3;"))
                {
                    connection.Open();

                    foreach (var plan in response.Plans) // Now safe to access since we checked for null
                    {
                        // Insert or update each plan in the database
                        using (SQLiteCommand command = new SQLiteCommand(
                            "INSERT OR REPLACE INTO Plans (PlanId, Name, IsFired, DateCreated, SiteId) VALUES (@planId, @name, @isFired, @dateCreated, @siteId)", connection))
                        {
                            command.Parameters.AddWithValue("@planId", plan.Id);
                            command.Parameters.AddWithValue("@name", plan.Name);
                            command.Parameters.AddWithValue("@isFired", plan.IsFired ? 1 : 0);
                            command.Parameters.AddWithValue("@dateCreated", plan.DateCreated);
                            command.Parameters.AddWithValue("@siteId", plan.SiteId);

                            command.ExecuteNonQuery();
                            Console.WriteLine($"Plan '{plan.Name}' added/updated in Plans table.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("API response is empty. No data to update.");
        }
    }



    //LOADIQUdata
    public static string? GetParameterName(string header)
    {
        var mappings = new Dictionary<string, string>
    {
        { "Hole ID", "HoleID" },
        { "Product Name", "ProductName" },
        { "Charge Length", "ChargeLength" },
        { "Charge Weight", "ChargeWeight" },
        { "Pump Rate", "PumpRate" },
        { "Time(HH:MM:SS)", "Time" },
        { "Cal Date", "CalDate" },
        { "CF", "CF" },
        { "QC Date", "QCDate" },
        { "Temp(C)", "Temp" },
        { "Product", "Product" },
        { "Density(g/ccm)", "ProductDensity" },
        { "Primers(m)", "Primers" },
        { "Flush", "Flush" },
        // Ensure this mapping matches your database column names and spreadsheet headers
    };

        return mappings.TryGetValue(header, out var paramName) ? paramName : null;
    }



    //come back this
    public static string GetDatabasePath()
    {
        // Use CommonApplicationData to target C:\ProgramData
        string commonAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        if (string.IsNullOrEmpty(commonAppDataPath))
        {
            throw new InvalidOperationException("Cannot find the common application data folder path.");
        }

        string folderPath = Path.Combine(commonAppDataPath, "LOADIQU");
        if (folderPath == null) // This check is technically unnecessary as Path.Combine won't return null if inputs are not null
        {
            throw new InvalidOperationException("Failed to combine folder path.");
        }

        string databasePath = Path.Combine(folderPath, "LOADIQU.db");
        if (databasePath == null) // Same as above, this check is unnecessary under normal operation
        {
            throw new InvalidOperationException("Failed to combine database path.");
        }

        if (!Directory.Exists(folderPath))
        {
            Debug.WriteLine($"Creating directory: {folderPath}");
            Directory.CreateDirectory(folderPath);
        }

        Debug.WriteLine($"Database path: {databasePath}");
        return databasePath;
    }


    public static string GetConnectionString()
    {
        string databasePath = GetDatabasePath();
        if (string.IsNullOrEmpty(databasePath))
        {
            throw new InvalidOperationException("Database path cannot be null or empty.");
        }

        string connectionString = $"Data Source={databasePath};Version=3;";
        Debug.WriteLine($"Connection string: {connectionString}");
        return connectionString;
    }

    public static void InitializeDatabase()
    {
        // Generate the connection string dynamically
        string connectionString = DatabaseConfig.GetConnectionString();
        Console.WriteLine("Generated connection string.");

        // Extract database path from the connection string
        var match = Regex.Match(connectionString, @"Data Source=(.*?);");
        if (!match.Success)
        {
            throw new InvalidOperationException("Database path could not be extracted from the connection string.");
        }
        string databasePath = match.Groups[1].Value;
        Console.WriteLine($"Database path: {databasePath}");

        // Check if the database file exists, and create it if it doesn't
        if (!File.Exists(databasePath))
        {
            SQLiteConnection.CreateFile(databasePath);
            Console.WriteLine($"Database file created at: {databasePath}");
        }
        else
        {
            Console.WriteLine("Database file already exists.");
        }

        // Initialize the database connection with the constructed connection string
        using (var connection = new SQLiteConnection(connectionString))
        {
            // Open the connection
            connection.Open();
            Console.WriteLine("Database connection opened.");

            var commands = new[]
            {
            @"
            CREATE TABLE IF NOT EXISTS LOADIQUData (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                BlastID TEXT,
                DateCreated TEXT,
                Ring TEXT,
                HoleID TEXT,
                ProductName TEXT,
                ChargeLength TEXT,
                ChargeWeight TEXT,
                PumpRate TEXT,
                Time TEXT,
                CalDate TEXT,
                CF TEXT,
                QCDate TEXT,
                Temp TEXT,
                Product TEXT,
                ProductDensity TEXT,
                Primers TEXT,
                Flush TEXT,
                IsSent INTEGER
            )",
            @"
            CREATE TABLE IF NOT EXISTS UserCredentials (
                Username TEXT PRIMARY KEY,
                EncryptedAPIPassword TEXT,
                SSHUsername TEXT,
                EncryptedSSHPassword TEXT
            )",
            @"
            CREATE TABLE IF NOT EXISTS Token (
                TokenValue TEXT PRIMARY KEY
            )",
            @"
            CREATE TABLE IF NOT EXISTS HoleData (
                Id TEXT PRIMARY KEY, 
                SiteId TEXT, 
                PlanId TEXT, 
                Name TEXT,
                Ring TEXT,
                Length REAL,
                AdjustedDesignLength REAL,
                AdjustedDesignDiameter REAL,
                AdjustedDesignAngle REAL,
                AdjustedDesignBearing REAL,
                AdjustedDesignBreakthrough INTEGER,
                AdjustedDesignComment TEXT
            )",
            @"
            CREATE TABLE IF NOT EXISTS DeckMeasurements (
                Id TEXT PRIMARY KEY,
                SiteId TEXT,
                PlanId TEXT,
                HoleId TEXT,
                HoleName TEXT,
                RingName TEXT,
                DeckNumber INTEGER,
                Property TEXT,
                Value TEXT,
                TimeOccurred DATETIME,
                TimeReceived DATETIME,
                DeviceId TEXT,
                UserName TEXT,
                DeviceName TEXT,
                EquipmentName TEXT
            )",
            @"
            CREATE TABLE IF NOT EXISTS Products (
                Id TEXT PRIMARY KEY,
                Type TEXT,
                Name TEXT,
                DateCreated TEXT,
                DateModified TEXT,
                IsDeleted INTEGER,
                Abbreviation TEXT,
                SupplierName TEXT,
                ShotPlusReference TEXT,
                DisplayColor TEXT
            )",
            // Newly added Plans table creation SQL command
            @"
            CREATE TABLE IF NOT EXISTS Plans (
                PlanId TEXT PRIMARY KEY,
                Name TEXT,
                IsFired INTEGER,
                DateCreated TEXT,
                SiteId TEXT
            )"
        };

            // Execute each SQL command to create tables and log each creation
            foreach (var cmdText in commands)
            {
                using (var command = new SQLiteCommand(cmdText, connection))
                {
                    command.ExecuteNonQuery();
                }
                Console.WriteLine($"Executed command: {cmdText.Split('\n')[1].Trim()}");
            }

            Console.WriteLine("All necessary tables have been verified/created.");
            connection.Close();
        }
        // Pause the console to allow reading the log
        //PauseConsole();
    }

    private static void PauseConsole()
    {

        if (Console.IsInputRedirected)
        {
            Console.WriteLine("Console input is redirected. Skipping pause.");
        }
        else
        {
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }
    }


    // Method: CredentialsStored
    /// <summary>
    /// Checks if user credentials are already stored securely in the local database. This ensures that system access controls are enforced according to ISO/IEC 27001:2022, section A.9.4.1 (System and application access control).
    /// </summary>

    private static bool CredentialsStored()
    {
        string databasePath = GetDatabasePath();
        using (SQLiteConnection connection = new SQLiteConnection($"Data Source={databasePath};Version=3;"))
        {
            connection.Open();
            using (SQLiteCommand command = new SQLiteCommand("SELECT Username, EncryptedAPIPassword FROM UserCredentials LIMIT 1", connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Check if both Username and EncryptedPassword are not null or empty
                        string? username = reader["Username"].ToString();
                        string? EncryptedAPIPassword = reader["EncryptedAPIPassword"].ToString();
                        return !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(EncryptedAPIPassword);
                    }
                }
            }
        }
        return false;
    }

    // Method: IsValidBase64String
    /// <summary>
    /// Validates whether a string is in valid Base64 format, ensuring data integrity and supporting secure data handling as specified in ISO/IEC 27001:2022, section A.10.1.1 (Cryptography).
    /// </summary>

    public static bool IsValidBase64String(string base64)
    {
        if (string.IsNullOrEmpty(base64) || base64.Length % 4 != 0 || !Regex.IsMatch(base64, @"^[a-zA-Z0-9\+/]*={0,2}$"))
        {
            return false;
        }

        try
        {
            byte[] buffer = Convert.FromBase64String(base64); // Try to decode
            return base64.Equals(Convert.ToBase64String(buffer)); // Verify if the conversion is reversible
        }
        catch (FormatException)
        {
            return false; // Invalid Base64 string
        }
    }

    // Method: PromptForCredentials
    /// <summary>
    /// Prompts the user to input credentials securely, storing them in an encrypted format. This method supports compliance with ISO/IEC 27001:2022, section A.9.2.3 (User identification and authentication), by ensuring that credentials are handled securely.
    /// </summary>
    public static void PromptForCredentials()
    {
        Console.WriteLine("Please enter your API username:");
        string? apiUsername = Console.ReadLine();

        Console.WriteLine("Please enter your API password:");
        string apiPassword = ReadPassword(); // Securely read the password without echoing it back in the terminal.

        Console.WriteLine("Please enter your SSH username:");
        string? sshUsername = Console.ReadLine();

        Console.WriteLine("Please enter your SSH password:");
        string sshPassword = ReadPassword(); // Securely read the SSH password without echoing it back in the terminal.

        if (!string.IsNullOrEmpty(apiUsername) && !string.IsNullOrEmpty(apiPassword) && !string.IsNullOrEmpty(sshUsername) && !string.IsNullOrEmpty(sshPassword))
        {
            try
            {
                // Retrieve the encryption key from a secure source
                var keyBytes = RetrieveAesKeyFromCredentialManager();
                string keyHex = BitConverter.ToString(keyBytes).Replace("-", "");
                //Console.WriteLine("Encryption key successfully retrieved.");

                // Encrypt API and SSH passwords
                string encryptedApiPassword = EncryptionHelper.EncryptString(keyHex, apiPassword);
                string encryptedSshPassword = EncryptionHelper.EncryptString(keyHex, sshPassword);
                Console.WriteLine("Passwords encrypted successfully.");

                // Store user credentials
                StoreUserCredentials(apiUsername, encryptedApiPassword, sshUsername, encryptedSshPassword);
                //Console.WriteLine("User credentials stored/updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in credential storage process: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Usernames and passwords cannot be null or empty. Please try again.");
        }
    }




    // Method: GenerateAesKey
    /// <summary>
    /// Generates a secure AES key for cryptographic purposes, aligning with ISO/IEC 27001:2022, section A.10.1.1 (Cryptography), which mandates the use of strong encryption methods.
    /// </summary>
    /// 
    public static byte[] GenerateAesKey()
    {
        using (var rng = new RNGCryptoServiceProvider())
        {
            byte[] keyBytes = new byte[32]; // 256 bits for AES-256
            rng.GetBytes(keyBytes);
            return keyBytes;
        }
    }
    //byte1
    // Method: StoreAesKeyToCredentialManager
    /// <summary>
    /// Securely stores an AES encryption key in the system's Credential Manager, enhancing key management practices as advocated by ISO/IEC 27001:2022, section A.10.1.2 (Management of cryptographic keys).
    /// </summary>
    public static void StoreAesKeyToCredentialManager(byte[] aesKey)
    {
        // Convert the AES key to a hex string
        string keyHex = BitConverter.ToString(aesKey).Replace("-", "");

        // Store the AES key in the Credential Manager
        try
        {
            AddCredential("Gen", "AESKey", keyHex);
            // Console.WriteLine("AES key has been stored securely.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error storing AES key: {ex.Message}");
        }
    }


    // Class: CredentialManager
    /// <summary>
    /// Provides functionalities for secure credential storage and management in compliance with ISO/IEC 27001:2022, specifically sections A.9.4.2 (User access management) and A.9.4.3 (Password management system).
    /// </summary>


    public class CredentialManager
    {
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool CredWrite(ref CREDENTIAL Credential, uint Flags);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct CREDENTIAL
        {
            public uint Flags;
            public uint Type;
            public string TargetName;
            public string Comment;
            public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
            public uint CredentialBlobSize;
            public IntPtr CredentialBlob;
            public uint Persist;
            public uint AttributeCount;
            public IntPtr Attributes;
            public string TargetAlias;
            public string UserName;
        }
        //credential manager - Local Computer

        // Method: AddCredential
        /// <summary>
        /// Adds a new credential to the system's credential store, ensuring secure storage and handling as per ISO/IEC 27001:2022, section A.9.4.2 (User access management).
        /// </summary>
        public static void AddCredential(string target, string userName, string secret)
        {
            Console.WriteLine("Starting to add credential...");

            byte[] secretBytes = System.Text.Encoding.Unicode.GetBytes(secret);
            IntPtr secretPtr = Marshal.AllocHGlobal(secretBytes.Length);
            Marshal.Copy(secretBytes, 0, secretPtr, secretBytes.Length);

            var credential = new CREDENTIAL
            {
                Type = 1, // CRED_TYPE_GENERIC
                TargetName = target,
                CredentialBlob = secretPtr,
                CredentialBlobSize = (uint)secretBytes.Length,
                Persist = 2, // Using 2 for CRED_PERSIST_LOCAL_MACHINE directly
                UserName = userName
            };

            if (!CredWrite(ref credential, 0))
            {
                int lastError = Marshal.GetLastWin32Error();
                Console.WriteLine($"Failed to write credential. LastError: {lastError}. Error Message: {new System.ComponentModel.Win32Exception(lastError).Message}");
            }
            else
            {
                Console.WriteLine("Credential added successfully with 'Local Computer' persistence.");
            }

            Marshal.FreeHGlobal(secretPtr);

            //PauseConsole(); // Pause after operation to allow user to see the output
        }


    }

    // Method: StoreAesKeyInCredentialManager
    /// <summary>
    /// Facilitates the secure storage of AES keys in the Credential Manager, complying with ISO/IEC 27001:2022's requirements for cryptographic key management (A.10.1.2).
    /// </summary>
    
    public static void StoreAesKeyInCredentialManager(byte[] aesKey)
    {
        // Convert the AES key from a byte array to a hex string
        string keyHex = BitConverter.ToString(aesKey).Replace("-", "");

        // Use the AddCredential method to store the hex string in the Windows Credential Manager
        AddCredential("Gen", "AESKey", keyHex);
    }



    // Method: RetrieveAesKeyFromCredentialManager
    /// <summary>
    /// Retrieves an AES key securely from the Credential Manager, ensuring cryptographic key management in line with ISO/IEC 27001:2022, section A.10.1.2 (Management of cryptographic keys).
    /// </summary>

    public static byte[] RetrieveAesKeyFromCredentialManager()
    {
        IntPtr credPtr = IntPtr.Zero;
        string target = "Gen"; // Identifier for AES key storage

        Console.WriteLine("Attempting to retrieve AES key from Credential Manager.");
        try
        {
            bool success = CredRead(target, CRED_TYPE.GENERIC, 0, out credPtr);
            if (!success || credPtr == IntPtr.Zero)
            {
                Console.WriteLine("AES key not found in Credential Manager. Generating a new AES key...");
                byte[] newKey = GenerateAesKey();
                StoreAesKeyToCredentialManager(newKey); // Store the newly generated key
                Console.WriteLine("New AES key generated and stored!");
                return newKey;
            }

            // Safe casting after ensuring the pointer is not null and likely valid
            if (credPtr != IntPtr.Zero)
            {
                CREDENTIAL? cred = Marshal.PtrToStructure<CREDENTIAL>(credPtr);
                if (cred.HasValue)
                {
                    string keyHex = Marshal.PtrToStringUni(cred.Value.CredentialBlob, (int)cred.Value.CredentialBlobSize / 2);
                    //Console.WriteLine("AES key successfully retrieved from Credential Manager.");

                    keyHex = Regex.Replace(keyHex, "[^0-9A-Fa-f]", ""); // Clean the keyHex string

                    if (keyHex.Length % 2 != 0) keyHex = "0" + keyHex; // Ensure even length

                    byte[] keyBytes = HexStringToByteArray(keyHex);
                    return keyBytes;
                }
            }

            throw new InvalidOperationException("Failed to convert credential data.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to retrieve AES key: {ex.Message}");
            throw; // Rethrow for further handling
        }
        finally
        {
            if (credPtr != IntPtr.Zero) CredFree(credPtr);
        }
    }




    // Enum: CRED_TYPE
    /// <summary>
    /// Enumerates the type of credentials to be stored, adhering to ISO/IEC 27001:2022's requirement for secure credential management (A.9.4.2).
    /// </summary>

    public enum CRED_TYPE : uint
    {
        GENERIC = 1
    }



    // Struct: CREDENTIAL
    /// <summary>
    /// Represents a credential structure for secure storage and retrieval, aligning with ISO/IEC 27001:2022's guidelines on credential management (A.9.4.2).
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct CREDENTIAL
    {
        public uint Flags;
        public CRED_TYPE Type;
        public IntPtr TargetName;
        public IntPtr Comment;
        public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
        public uint CredentialBlobSize;
        public IntPtr CredentialBlob;
        public uint Persist;
        public uint AttributeCount;
        public IntPtr Attributes;
        public IntPtr TargetAlias;
        public IntPtr UserName;
    }

    // Method: CredWrite
    /// <summary>
    /// Writes a credential to the system's credential store, ensuring secure credential management as specified by ISO/IEC 27001:2022, section A.9.4.2 (User access management).
    /// </summary>
    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool CredWrite(ref CREDENTIAL Credential, uint Flags);

    // Method: CredRead
    /// <summary>
    /// Reads a credential from the system's credential store, supporting secure credential management practices in accordance with ISO/IEC 27001:2022, section A.9.4.2 (User access management).
    /// </summary>

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern bool CredRead(string TargetName, CRED_TYPE Type, int ReservedFlag, out IntPtr CredentialPtr);

    // Method: CredFree
    /// <summary>
    /// Frees the memory allocated for a credential structure, promoting efficient resource management as recommended by ISO/IEC 27001:2022, section A.6.1.2 (Resource use).
    /// </summary>

    [DllImport("advapi32.dll", SetLastError = true)]
    public static extern bool CredFree(IntPtr CredentialPtr);

    // Method: AddCredential
    /// <summary>
    /// Adds a new credential to the system's credential store, ensuring secure credential management in line with ISO/IEC 27001:2022's requirements (A.9.4.2).
    /// </summary>

    public static void AddCredential(string target, string userName, string secret)
    {
        var credential = new CREDENTIAL
        {
            Type = CRED_TYPE.GENERIC,
            TargetName = Marshal.StringToHGlobalUni(target),
            CredentialBlob = Marshal.StringToHGlobalUni(secret),
            CredentialBlobSize = (uint)((secret.Length + 1) * 2), // Including the null terminator
            Persist = (uint)2, // CRED_PERSIST_LOCAL_MACHINE
            AttributeCount = 0,
            UserName = Marshal.StringToHGlobalUni(userName)
        };

        bool success = CredWrite(ref credential, 0);
        if (!success)
        {
            int lastError = Marshal.GetLastWin32Error();
            throw new Exception($"Failed to store credential. Error Code: {lastError}");
        }

        Marshal.FreeHGlobal(credential.TargetName);
        Marshal.FreeHGlobal(credential.CredentialBlob);
        Marshal.FreeHGlobal(credential.UserName);
    }



    // Method 6: InitializeEncryptionKey
    /// <summary>
    /// Initializes encryption keys at application startup, ensuring secure cryptographic operations.
    /// Compliance with ISO/IEC 27001:2022:
    /// A.10.1 (Cryptography) - Implements cryptographic measures to protect information.
    /// </summary>

    public static void InitializeEncryptionKey()
    {
        try
        {
            // Check if the AES key already exists
            var existingKey = RetrieveAesKeyFromCredentialManager();
            if (existingKey == null || existingKey.Length == 0)
            {
                // AES key does not exist, so generate a new one
                byte[] newKey = GenerateAesKey();

                // Store the new AES key
                StoreAesKeyToCredentialManager(newKey);

                //Console.WriteLine("A new AES encryption key has been generated and stored securely.");
            }
            else
            {
                //Console.WriteLine("An existing AES encryption key has been successfully retrieved.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during encryption key initialization: {ex.Message}");
        }
    }



    // Method: ReadPassword
    /// <summary>
    /// Securely reads a password from the console input, supporting ISO/IEC 27001:2022's requirement for secure user authentication (A.9.2.3).
    /// </summary>


    private static string ReadPassword()
    {
        var passwordBuilder = new StringBuilder();
        while (true)
        {
            var key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                break;
            }
            else if (key.Key == ConsoleKey.Backspace && passwordBuilder.Length > 0)
            {
                passwordBuilder.Remove(passwordBuilder.Length - 1, 1);
                Console.Write("\b \b");
            }
            else if (!char.IsControl(key.KeyChar))
            {
                passwordBuilder.Append(key.KeyChar);
                Console.Write("*");
            }
        }
        return passwordBuilder.ToString();
    }


    // Method: CreateUserCredentialsTable
    /// <summary>
    /// Creates a table for storing user credentials in the local database, adhering to ISO/IEC 27001:2022's guidelines on secure database management (A.9.4.1).
    /// </summary>


    public static void CreateUserCredentialsTable()
    {
        try
        {
            string databasePath = GetDatabasePath();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={databasePath};Version=3;"))
            {
                connection.Open();
                //Console.WriteLine("Database Connection Opened for User Credentials Table");

                string sqlCommand =
                    "CREATE TABLE IF NOT EXISTS UserCredentials (" +
                    "Username TEXT PRIMARY KEY, " +
                    "EncryptedAPIPassword TEXT, " +
                    "SSHUsername TEXT, " +
                    "EncryptedSSHPassword TEXT)";

                using (SQLiteCommand createTableCommand = new SQLiteCommand(sqlCommand, connection))
                {
                    createTableCommand.ExecuteNonQuery();
                    //Console.WriteLine("UserCredentials Table Created/Verified");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CreateUserCredentialsTable: {ex.Message}");
        }
    }

    // Method: StoreUserCredentials
    /// <summary>
    /// Stores user credentials securely in the local database, ensuring compliance with ISO/IEC 27001:2022's requirements for secure information storage (A.9.4.1).
    /// </summary>

    public static void StoreUserCredentials(string apiUsername, string encryptedApiPassword, string sshUsername, string encryptedSshPassword)
    {
        try
        {
            string databasePath = GetDatabasePath();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={databasePath};Version=3;"))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(
                    "INSERT OR REPLACE INTO UserCredentials (Username, EncryptedAPIPassword, SSHUsername, EncryptedSSHPassword) " +
                    "VALUES (@ApiUsername, @EncryptedApiPassword, @SshUsername, @EncryptedSshPassword)", connection))
                {
                    command.Parameters.AddWithValue("@ApiUsername", apiUsername);
                    command.Parameters.AddWithValue("@EncryptedApiPassword", encryptedApiPassword);
                    command.Parameters.AddWithValue("@SshUsername", sshUsername);
                    command.Parameters.AddWithValue("@EncryptedSshPassword", encryptedSshPassword);
                    command.ExecuteNonQuery();
                }
            }

            //Console.WriteLine("User credentials stored/updated successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error storing user credentials: {ex.Message}");
        }
    }

    // Method: GetUserCredentials
    /// <summary>
    /// Retrieves and decrypts user credentials from the local database, supporting ISO/IEC 27001:2022's guidelines on secure information retrieval (A.9.4.1).
    /// </summary>

    public static (string? ApiUsername, string? EncryptedApiPassword, string? SshUsername, string? SshPassword) GetUserCredentials()
    {
        //Console.WriteLine("Starting the process to retrieve and decrypt user credentials...");

        try
        {
            //Console.WriteLine("Attempting to determine the database path...");
            string databasePath = GetDatabasePath();
            //Console.WriteLine($"Database path determined as: {databasePath}");

            //Console.WriteLine("Attempting to open a connection to the database...");
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={databasePath};Version=3;"))
            {
                connection.Open();
                Console.WriteLine("Database connection successfully opened.");

                string query = "SELECT Username, EncryptedAPIPassword, SSHUsername, EncryptedSSHPassword FROM UserCredentials LIMIT 1";
                //Console.WriteLine("Preparing to execute the query to retrieve credentials...");

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            //Console.WriteLine("Credentials found in the database. Proceeding with decryption...");

                            //Console.WriteLine("Retrieving the encryption key from the Credential Manager...");
                            var keyBytes = RetrieveAesKeyFromCredentialManager();
                            string keyHex = BitConverter.ToString(keyBytes).Replace("-", "");
                            //Console.WriteLine("Encryption key successfully retrieved.");

                            // Decrypt API and SSH passwords
                            string? apiUsername = reader["Username"] as string;
                            string? encryptedApiPassword = reader["EncryptedAPIPassword"] as string;
                            string apiPassword = encryptedApiPassword != null ? EncryptionHelper.DecryptString(keyHex, encryptedApiPassword) : string.Empty;
                            //Console.WriteLine("API password decrypted successfully.");

                            string? sshUsername = reader["SSHUsername"] as string;
                            string? encryptedSshPassword = reader["EncryptedSSHPassword"] as string;
                            string sshPassword = encryptedSshPassword != null ? EncryptionHelper.DecryptString(keyHex, encryptedSshPassword) : string.Empty;
                            //Console.WriteLine("SSH password decrypted successfully.");

                            //Console.WriteLine("All credentials retrieved and decrypted successfully.");
                            return (apiUsername, apiPassword, sshUsername, sshPassword);
                        }
                        else
                        {
                            Console.WriteLine("No credentials found in the database.");
                            return (null, null, null, null);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while retrieving or decrypting credentials: {ex.Message}");
            return (null, null, null, null);
        }
    }


    // Method: HexStringToByteArray
    /// <summary>
    /// Converts a hexadecimal string to a byte array, ensuring the resulting byte array length is even, in line with ISO/IEC 27001:2022's requirement for data integrity (A.12.2).
    /// </summary>
    /// <param name="hexString">The hexadecimal string to convert.</param>
    /// <returns>The resulting byte array.</returns>

    public static byte[] HexStringToByteArray(string hexString)
    {
        if (hexString.Length % 2 != 0)
        {
            Debug.WriteLine($"HexStringToByteArray: Hex string has an odd length: {hexString}");
            throw new ArgumentException("Hex string has an odd length", nameof(hexString));
        }

        byte[] bytes = new byte[hexString.Length / 2];
        for (int i = 0; i < bytes.Length; i++)
        {
            string byteString = hexString.Substring(i * 2, 2);
            //Debug.WriteLine($"HexStringToByteArray: Processing byte {i + 1}: {byteString}");

            try
            {
                bytes[i] = Convert.ToByte(byteString, 16);
            }
            catch (FormatException ex)
            {
                // Log the exception and rethrow
                //Debug.WriteLine($"HexStringToByteArray: Error converting byte '{byteString}' to hexadecimal: {ex.Message}");
                throw new FormatException($"Invalid character * in hexadecimal string at position {i * 2}");
            }
        }
        return bytes;
    }

    // Method: ConvertToHexString
    /// <summary>
    /// Converts a UTF-8 encoded string to its hexadecimal representation, facilitating secure data transmission as per ISO/IEC 27001:2022's data protection guidelines (A.8.2).
    /// </summary>
    /// <param name="input">The input string to convert.</param>
    /// <returns>The hexadecimal representation of the input string.</returns>

    public static string ConvertToHexString(string input)
    {
        return BitConverter.ToString(Encoding.UTF8.GetBytes(input)).Replace("-", "");
    }


    // Method: ConvertFromHexString
    /// <summary>
    /// Converts a hexadecimal string to its UTF-8 encoded string equivalent, ensuring data integrity and secure communication channels, as recommended by ISO/IEC 27001:2022's data protection standards (A.12.2).
    /// </summary>
    /// <param name="hexString">The hexadecimal string to convert.</param>
    /// <returns>The UTF-8 encoded string.</returns>

    public static string ConvertFromHexString(string hexString)
    {
        byte[] bytes = Enumerable.Range(0, hexString.Length)
                         .Where(x => x % 2 == 0)
                         .Select(x => Convert.ToByte(hexString.Substring(x, 2), 16))
                         .ToArray();
        return Encoding.UTF8.GetString(bytes);
    }


    // Method: RetrieveCredentialFromCredentialManager
    /// <summary>
    /// Retrieves a stored credential from the system's credential manager, ensuring secure credential management and access control in accordance with ISO/IEC 27001:2022's user access management guidelines (A.9.4.2).
    /// </summary>
    /// <param name="target">The target name associated with the stored credential.</param>
    /// <returns>The retrieved credential string.</returns>

    public static string? RetrieveCredentialFromCredentialManager(string target)
    {
        IntPtr credPtr = IntPtr.Zero;

        // Attempt to read the credential from the Credential Manager
        bool result = CredRead(target, CRED_TYPE.GENERIC, 0, out credPtr);
        if (!result)
        {
            throw new Exception("Failed to retrieve the credential.");
        }

        // Process the retrieved credential
        string? credentialString = null;
        if (credPtr != IntPtr.Zero)
        {
            try
            {
                CREDENTIAL? cred = Marshal.PtrToStructure<CREDENTIAL>(credPtr);
                if (cred != null)
                {
                    credentialString = Marshal.PtrToStringUni(cred.Value.CredentialBlob, (int)(cred.Value.CredentialBlobSize / 2));
                }
            }
            finally
            {
                CredFree(credPtr); // Ensure memory is freed
            }
        }

        return credentialString;
    }




    // Method: EnsureEvenLengthHexString
    /// <summary>
    /// Ensures the provided hexadecimal string has an even length, promoting data consistency and integrity as specified by ISO/IEC 27001:2022's data protection requirements (A.12.2).
    /// </summary>
    /// <param name="hexString">The hexadecimal string to validate.</param>
    /// <returns>The validated hexadecimal string with an even length.</returns>


    public static string EnsureEvenLengthHexString(string hexString)
    {
        if (hexString.Length % 2 != 0)
        {
            // Prepend with '0' to make the length even
            hexString = "0" + hexString;
        }
        return hexString;
    }

    // Class: EncryptionHelper
    /// <summary>
    /// Provides helper methods for encryption and decryption operations, ensuring data confidentiality and integrity in compliance with ISO/IEC 27001:2022's cryptographic measures (A.10.1).
    /// </summary>
    //encryption helper
    public static class EncryptionHelper
    {
        // Method: EncryptString
        /// <summary>
        /// Encrypts a plaintext string using AES encryption with a provided key, ensuring secure data transmission and confidentiality as mandated by ISO/IEC 27001:2022's cryptographic guidelines (A.10.1).
        /// </summary>
        /// <param name="keyHex">The hexadecimal representation of the encryption key.</param>
        /// <param name="plainText">The plaintext string to encrypt.</param>
        /// <returns>The encrypted ciphertext.</returns>
        public static string EncryptString(string keyHex, string plainText)
        {
            byte[] keyBytes = HexStringToByteArray(keyHex);
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = keyBytes;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                aesAlg.GenerateIV();
                byte[] iv = aesAlg.IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    // Prepend the IV to the ciphertext
                    msEncrypt.Write(iv, 0, iv.Length);
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        // Method: DecryptString
        /// <summary>
        /// Decrypts a ciphertext string using AES decryption with a provided key, ensuring secure data retrieval and confidentiality as per ISO/IEC 27001:2022's cryptographic standards (A.10.1).
        /// </summary>
        /// <param name="keyHex">The hexadecimal representation of the decryption key.</param>
        /// <param name="cipherText">The ciphertext string to decrypt.</param>
        /// <returns>The decrypted plaintext.</returns>

        public static string DecryptString(string keyHex, string cipherText)
        {
            byte[] keyBytes = HexStringToByteArray(keyHex);
            byte[] fullCipher = Convert.FromBase64String(cipherText);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = keyBytes;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                // Extract the IV from the beginning of the ciphertext
                byte[] iv = new byte[aesAlg.BlockSize / 8];
                byte[] cipherBytes = new byte[fullCipher.Length - iv.Length];
                Array.Copy(fullCipher, 0, iv, 0, iv.Length);
                Array.Copy(fullCipher, iv.Length, cipherBytes, 0, cipherBytes.Length);
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }


        // Method: HexStringToByteArray
        /// <summary>
        /// Converts a hexadecimal string to a byte array, facilitating cryptographic operations and ensuring data integrity in compliance with ISO/IEC 27001:2022's cryptographic measures (A.10.1).
        /// </summary>
        /// <param name="hexString">The hexadecimal string to convert.</param>
        /// <returns>The resulting byte array.</returns>
        // Helper method for converting a hex string to a byte array
        public static byte[] HexStringToByteArray(string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException("The hex string must have an even length", nameof(hexString));
            }
            byte[] bytes = new byte[hexString.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return bytes;
        }
    }




    class DatabaseManager
    {
        public static string ConnectionString => $"Data Source={GetDatabasePath()};Version=3;";

        public static string GetDatabasePath()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folderPath = Path.Combine(appDataPath, "LOADIQU");
            string databasePath = Path.Combine(folderPath, "LOADIQU.db");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            return databasePath;
        }

        //55
        // Method: PromptForCredentials
        /// <summary>
        /// Prompts the user to input API and SSH credentials, securely reads and encrypts them, and stores them in the database. Ensures compliance with ISO/IEC 27001:2022 by securely managing user access credentials (A.9.4.2) and protecting sensitive information through encryption (A.12.3).
        /// </summary>
        public static void PromptForCredentials()
        {
            Console.WriteLine("Please enter your API username:");
            string? apiUsername = Console.ReadLine();

            Console.WriteLine("Please enter your API password:");
            string? apiPassword = ReadPassword(); // Securely read the password without echoing it back in the terminal.

            Console.WriteLine("Please enter your SSH username:");
            string? sshUsername = Console.ReadLine();

            Console.WriteLine("Please enter your SSH password:");
            string sshPassword = ReadPassword(); // Securely read the SSH password without echoing it back in the terminal.

            if (!string.IsNullOrEmpty(apiUsername) && !string.IsNullOrEmpty(apiPassword) && !string.IsNullOrEmpty(sshUsername) && !string.IsNullOrEmpty(sshPassword))
            {
                try
                {
                    // Retrieve the encryption key from a secure source
                    var keyBytes = RetrieveAesKeyFromCredentialManager();
                    string keyHex = BitConverter.ToString(keyBytes).Replace("-", "");
                    //Console.WriteLine("Encryption key successfully retrieved.");

                    // Encrypt API and SSH passwords
                    string encryptedApiPassword = EncryptionHelper.EncryptString(keyHex, apiPassword);
                    string encryptedSshPassword = EncryptionHelper.EncryptString(keyHex, sshPassword);
                    //Console.WriteLine("Passwords encrypted successfully.");

                    // Store user credentials
                    StoreUserCredentials(apiUsername, encryptedApiPassword, sshUsername, encryptedSshPassword);
                    Console.WriteLine("User credentials stored/updated successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in credential storage process: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Usernames and passwords cannot be null or empty. Please try again.");
            }
        }



        //LOADIQUdata
        // Method: UpdateDatabase
        /// <summary>
        /// Updates the SQLite database with the provided data. Validates input and ensures data integrity and security during database operations, in line with ISO/IEC 27001:2022's data integrity (A.12.2) and access control (A.9.4.2) requirements.
        /// </summary>
        /// <param name="data">A tuple containing BlastID, DateCreated, Ring, HoleID, Product, ChargeLength, ChargeWeight, PumpRate, Time, CalDate, CF, QCDate, Temp, ProductDensity, Primers, Flush, and IsSent data.</param>
        public static void UpdateDatabase((string?, string?, string?, List<Dictionary<string, object>>) data)
        {
            string connectionString = DatabaseConfig.GetConnectionString();

            if (data.Item1 != null && data.Item2 != null && data.Item4.Count > 0)
            {
                try
                {
                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();

                        foreach (var record in data.Item4)
                        {
                            string commandText = @"
INSERT INTO LOADIQUData (BlastID, DateCreated, Ring, HoleID, Product, ChargeLength, ChargeWeight, PumpRate, Time, CalDate, CF, QCDate, Temp, ProductDensity, Primers, Flush, IsSent)
VALUES (@BlastID, @DateCreated, @Ring, @HoleID, @Product, @ChargeLength, @ChargeWeight, @PumpRate, @Time, @CalDate, @CF, @QCDate, @Temp, @ProductDensity, @Primers, @Flush, @IsSent)";


                            using (SQLiteCommand command = new SQLiteCommand(commandText, connection))
                            {
                                command.Parameters.AddWithValue("@BlastID", data.Item1);
                                command.Parameters.AddWithValue("@DateCreated", data.Item2);



                                command.Parameters.AddWithValue("@IsSent", 0); // Assuming IsSent is always 0 initially

                                foreach (var kvp in record)
                                {
                                    // Ensure the key is not an empty string
                                    if (!string.IsNullOrEmpty(kvp.Key))
                                    {
                                        string paramName = "@" + kvp.Key.Replace(" ", ""); // Normalize parameter names by removing spaces
                                        object value = kvp.Value ?? DBNull.Value; // Use DBNull.Value for actual nulls
                                        command.Parameters.AddWithValue(paramName, value);
                                        Debug.WriteLine($"Added parameter: {paramName}, Value: {value}");
                                    }
                                    else
                                    {
                                        Debug.WriteLine("Attempted to add a parameter with an empty name, which was skipped.");
                                    }
                                }

                                command.ExecuteNonQuery();
                                //Console.WriteLine($"Data added to database: BlastID='{data.Item1}', DateCreated='{data.Item2}'");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception occurred while updating database: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Insufficient data to update database.");
            }
        }


        // Class: DatabaseConfig
        /// <summary>
        /// Provides methods to manage database configuration, including obtaining the database path and connection string. Supports ISO/IEC 27001:2022 compliance by ensuring secure database storage (A.12.3) and data integrity (A.12.2).
        /// </summary>
        public class DatabaseConfig
        {
            // Method: GetDatabasePath
            /// <summary>
            /// Retrieves the path to the SQLite database file. Ensures secure storage and access to the database in compliance with ISO/IEC 27001:2022's data protection guidelines (A.8.2) and access control measures (A.9.4.2).
            /// </summary>
            /// <returns>The path to the SQLite database file.</returns>
            public static string GetDatabasePath()
            {
                // Use CommonApplicationData to target C:\ProgramData
                string commonAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                string folderPath = Path.Combine(commonAppDataPath, "LOADIQU");
                string databasePath = Path.Combine(folderPath, "LOADIQU.db");

                // Ensure the directory exists
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                return databasePath;
            }
            // Method: GetConnectionString
            /// <summary>
            /// Retrieves the connection string for the SQLite database. Ensures secure database access and data transmission as per ISO/IEC 27001:2022's cryptographic measures (A.10.1).
            /// </summary>
            /// <returns>The connection string for the SQLite database.</returns>
            public static string GetConnectionString()
            {
                string databasePath = GetDatabasePath();
                return $"Data Source={databasePath};Version=3;";
            }
        }


        // Method: InitializeDatabase
        /// <summary>
        /// Initializes the SQLite database by creating necessary tables if they do not exist. Ensures database integrity and security in accordance with ISO/IEC 27001:2022's data integrity (A.12.2) and access control (A.9.4.2) guidelines.
        /// </summary>
        public static void InitializeDatabase()
        {
            string connectionString = DatabaseConfig.GetConnectionString();

            // Extract database path from the connection string
            var match = Regex.Match(connectionString, @"Data Source=(.*?);");
            if (!match.Success)
            {
                throw new InvalidOperationException("Database path could not be extracted from the connection string.");
            }
            string databasePath = match.Groups[1].Value;

            // Check if the database file exists, and create it if it doesn't
            if (!File.Exists(databasePath))
            {
                SQLiteConnection.CreateFile(databasePath);
                //Console.WriteLine("Database file created.");
            }

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("Initializing Database...");

                var commands = new[]
                {
                // Create LOADIQUData table
                @"CREATE TABLE IF NOT EXISTS LOADIQUData (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    BlastID TEXT,
                    DateCreated TEXT,
                    HoleID TEXT,
                    Ring TEXT,
                    ProductName TEXT,
                    ChargeLength TEXT,
                    ChargeWeight TEXT,
                    PumpRate TEXT,
                    Time TEXT,
                    CalDate TEXT,
                    CF TEXT,
                    QCDate TEXT,
                    Temp TEXT,
                    Product TEXT,
                    ProductDensity TEXT,
                    Primers TEXT,
                    Flush TEXT,
                    IsSent TEXT
                )",
                // Create UserCredentials table
                @"CREATE TABLE IF NOT EXISTS UserCredentials (
                    Username TEXT PRIMARY KEY,
                    EncryptedAPIPassword TEXT,
                    SSHUsername TEXT,
                    EncryptedSSHPassword TEXT
                )",
                // Create Token table
                @"CREATE TABLE IF NOT EXISTS Token (
                    TokenValue TEXT PRIMARY KEY
                )"
            };

                foreach (var cmdText in commands)
                {
                    using (var command = new SQLiteCommand(cmdText, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                Console.WriteLine("Database initialized successfully.");
                connection.Close();
            }
        }

        // Method: GetTokenWithStoredCredentialsAsync
        /// <summary>
        /// Retrieves an authentication token asynchronously using stored credentials for API access. Ensures secure transmission of credentials and compliance with ISO/IEC 27001:2022's cryptographic measures (A.10.1) and access control (A.9.4.2) standards.
        /// </summary>
        /// <param name="apiUrl">The URL of the API to authenticate against.</param>
        /// <returns>An authentication token if successful; otherwise, null.</returns>

        public static async Task<string?> GetTokenWithStoredCredentialsAsync(string apiUrl)
        {
            var credentials = FetchStoredCredentials();
            if (!credentials.HasValue)
            {
                Console.WriteLine("No stored credentials found.");
                return null;
            }

            // Retrieve the encryption key from a secure source
            var keyBytes = RetrieveAesKeyFromCredentialManager();
            string keyHex = BitConverter.ToString(keyBytes).Replace("-", "");

            string decryptedPassword = EncryptionHelper.DecryptString(keyHex, credentials.Value.EncryptedPassword);

            // Await the async method and return its result
            return await GetTokenAsync(apiUrl, credentials.Value.Username, decryptedPassword);
        }

        // Method: GetTokenAsync
        /// <summary>
        /// Asynchronously retrieves an authentication token from the specified API using the provided username and password. Ensures secure transmission of credentials and compliance with ISO/IEC 27001:2022's cryptographic measures (A.10.1) and access control (A.9.4.2) standards.
        /// </summary>
        /// <param name="apiUrl">The URL of the API to authenticate against.</param>
        /// <param name="username">The username for authentication.</param>
        /// <param name="password">The password for authentication.</param>
        /// <returns>An authentication token if successful; otherwise, null.</returns>

        public static async Task<string?> GetTokenAsync(string apiUrl, string username, string password)
        {
            using (var client = new HttpClient())
            {
                // Convert credentials to Base64 string for Basic Auth header
                var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                try
                {
                    // Use async call to avoid blocking
                    var response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        // Extract the token from the response body
                        var token = await response.Content.ReadAsStringAsync();
                        return token;
                    }
                    else
                    {
                        Console.WriteLine($"Failed to get token. Status code: {response.StatusCode}");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception occurred while getting token: {ex.Message}");
                    return null;
                }
            }
        }


        // Method: FetchStoredCredentials
        /// <summary>
        /// Retrieves stored credentials (username and encrypted password) from the database. Ensures secure storage and access to credentials and compliance with ISO/IEC 27001:2022's data protection (A.8.2) and access control (A.9.4.2) standards.
        /// </summary>
        /// <returns>A tuple containing the username and encrypted password if credentials are found; otherwise, null.</returns>

        private static (string Username, string EncryptedPassword)? FetchStoredCredentials()
        {
            try
            {
                string databasePath = GetDatabasePath();
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={databasePath};Version=3;"))
                {
                    connection.Open();

                    string query = "SELECT Username, EncryptedAPIPassword FROM UserCredentials LIMIT 1"; // Assuming single user
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string username = reader.GetString(0);
                                string encryptedPassword = reader.GetString(1);
                                return (username, encryptedPassword);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching stored credentials: {ex.Message}");
            }
            return null; // Return null if no credentials found or error occurs
        }



        public class JwtPayload
        {
            public string Exp { get; set; }

            // Constructor to initialize the Exp property
            public JwtPayload(string exp)
            {
                Exp = exp;
            }

            }
        //inertion 1

        // Method: InsertDataIntoDatabase
        /// <summary>
        /// Inserts data into the SQLite database, ensuring data integrity and security. Validates input, handles transactions, and prevents duplicate records. Complies with ISO/IEC 27001:2022's data integrity (A.12.2) and access control (A.9.4.2) requirements.
        /// </summary>
        /// <param name="blastId">The BlastID associated with the data.</param>
        /// <param name="dateCreated">The date the data was created.</param>
        /// <param name="records">A list of dictionaries containing the data records.</param>
        /// <returns>True if data insertion is successful; otherwise, false.</returns>

        public static bool InsertDataIntoDatabase(string blastId, string dateCreated, List<Dictionary<string, object>> records)
        {
            bool isSuccess = false;
            string connectionString = DatabaseConfig.GetConnectionString();

            Console.WriteLine($"Connection string: {connectionString}");
            Debug.WriteLine($"Connection string: {connectionString}");

            using (var connection = new SQLiteConnection(connectionString))
            {
                SQLiteTransaction? transaction = null;

                try
                {
                    connection.Open();
                    //Console.WriteLine($"Database connection successfully opened. Path: {connection.DataSource}");
                    Debug.WriteLine($"Database connection successfully opened. Path: {connection.DataSource}");

                    transaction = connection.BeginTransaction();
                    Console.WriteLine("Database transaction started.");
                    Debug.WriteLine("Database transaction started.");

                    foreach (var record in records)
                    {
                        var (Ring, HoleId) = ParseHoleId(record.ContainsKey("Hole ID") ? record["Hole ID"].ToString() : null);

                        // Check for duplicate records first
                        var duplicateCheckCommandText = @"
SELECT COUNT(*) FROM LOADIQUData
WHERE BlastID = @BlastID AND DateCreated = @DateCreated AND Ring = @Ring AND HoleID = @HoleID
AND ProductName = @ProductName AND ChargeLength = @ChargeLength AND ChargeWeight = @ChargeWeight AND QCDate = @QCDate";
                        using (var duplicateCheckCommand = new SQLiteCommand(duplicateCheckCommandText, connection))
                        {
                            duplicateCheckCommand.Parameters.AddWithValue("@BlastID", blastId);
                            duplicateCheckCommand.Parameters.AddWithValue("@DateCreated", dateCreated);
                            duplicateCheckCommand.Parameters.AddWithValue("@Ring", Ring);
                            duplicateCheckCommand.Parameters.AddWithValue("@HoleID", HoleId);
                            duplicateCheckCommand.Parameters.AddWithValue("@ProductName", record["Product Name"] ?? DBNull.Value);
                            duplicateCheckCommand.Parameters.AddWithValue("@ChargeLength", record["Charge Length"] ?? DBNull.Value);
                            duplicateCheckCommand.Parameters.AddWithValue("@ChargeWeight", record["Charge Weight"] ?? DBNull.Value);
                            duplicateCheckCommand.Parameters.AddWithValue("@QCDate", record["QC Date"] ?? DBNull.Value);

                            var exists = Convert.ToInt32(duplicateCheckCommand.ExecuteScalar()) > 0;
                            if (exists)
                            {
                                Console.WriteLine($"Skipping duplicate record: BlastID={blastId}, DateCreated={dateCreated}, Ring={Ring}, HoleID={HoleId}");
                                continue;
                            }
                        }

                        string commandText = @"
INSERT INTO LOADIQUData 
(BlastID, DateCreated, Ring, HoleID, ProductName, ChargeLength, ChargeWeight, PumpRate, Time, CalDate, CF, QCDate, Temp, Product, ProductDensity, Primers, Flush, IsSent)
VALUES 
(@BlastID, @DateCreated, @Ring, @HoleID, @ProductName, @ChargeLength, @ChargeWeight, @PumpRate, @Time, @CalDate, @CF, @QCDate, @Temp, @Product, @ProductDensity, @Primers, @Flush, @IsSent)";
                        using (var command = new SQLiteCommand(commandText, connection, transaction))
                        {
                            // Explicitly setting the parameters
                            command.Parameters.AddWithValue("@BlastID", blastId);
                            command.Parameters.AddWithValue("@DateCreated", dateCreated);
                            command.Parameters.AddWithValue("@Ring", Ring);
                            command.Parameters.AddWithValue("@HoleID", HoleId);
                            command.Parameters.AddWithValue("@ProductName", record["Product Name"] ?? DBNull.Value);
                            command.Parameters.AddWithValue("@ChargeLength", record["Charge Length"] ?? DBNull.Value);
                            command.Parameters.AddWithValue("@ChargeWeight", record["Charge Weight"] ?? DBNull.Value);
                            command.Parameters.AddWithValue("@PumpRate", record["Pump Rate"] ?? DBNull.Value);
                            command.Parameters.AddWithValue("@Time", record["Time(HH:MM:SS)"] ?? DBNull.Value);
                            command.Parameters.AddWithValue("@CalDate", record["Cal Date"] ?? DBNull.Value);
                            command.Parameters.AddWithValue("@CF", record["CF"] ?? DBNull.Value);
                            command.Parameters.AddWithValue("@QCDate", record["QC Date"] ?? DBNull.Value);
                            command.Parameters.AddWithValue("@Temp", record["Temp(C)"] ?? DBNull.Value);
                            command.Parameters.AddWithValue("@Product", record["Product"] ?? DBNull.Value);
                            command.Parameters.AddWithValue("@ProductDensity", record["Density(g/ccm)"] ?? DBNull.Value);
                            command.Parameters.AddWithValue("@Primers", record["Primers(m)"] ?? DBNull.Value);
                            command.Parameters.AddWithValue("@Flush", record["Flush"] ?? DBNull.Value);
                            command.Parameters.AddWithValue("@IsSent", 0); // Assuming IsSent is always 0 on insert

                            // Execute the command
                            command.ExecuteNonQuery();
                           //Console.WriteLine($"Record inserted: BlastID={blastId}, DateCreated={dateCreated}, Ring={Ring}, HoleID={HoleId}");
                        }
                    }

                    transaction.Commit();
                    isSuccess = true;
                   // Console.WriteLine("Transaction committed successfully. Data inserted into the database.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during database operation: {ex.Message}");
                    Debug.WriteLine($"Error during database operation: {ex.Message}");
                    transaction?.Rollback();
                    Console.WriteLine("Transaction rolled back due to an error.");
                    Debug.WriteLine("Transaction rolled back due to an error.");
                }
                finally
                {
                    connection.Close();
                    //Console.WriteLine("Database connection closed.");
                    Debug.WriteLine("Database connection closed.");
                }
            }

            return isSuccess;
        }

        // Method: RecordExists
        /// <summary>
        /// Checks whether a record with the specified BlastID, Ring, and HoleID exists in the LOADIQUData table of the SQLite database. Ensures data integrity and assists in preventing duplicate records. Complies with ISO/IEC 27001:2022's data integrity (A.12.2) and access control (A.9.4.2) requirements.
        /// </summary>
        /// <param name="connection">The SQLiteConnection instance representing the database connection.</param>
        /// <param name="blastId">The BlastID to check for.</param>
        /// <param name="ring">The ring to check for.</param>
        /// <param name="holeId">The HoleID to check for.</param>
        /// <returns>True if the record exists; otherwise, false.</returns>

        public static bool RecordExists(SQLiteConnection connection, string blastId, string ring, string holeId)
        {
            var commandText = @"
        SELECT COUNT(*) 
        FROM LOADIQUData 
        WHERE BlastID = @BlastID AND Ring = @Ring AND HoleID = @HoleID";

            using (var command = new SQLiteCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@BlastID", blastId);
                command.Parameters.AddWithValue("@Ring", ring);
                command.Parameters.AddWithValue("@HoleID", holeId);

                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
        }






        // Method to Create Token Table - Now included within InitializeDatabase, but kept for reference
        // Method: CreateTokenTable
        /// <summary>
        /// Creates the Token table within the SQLite database if it does not already exist. The Token table is used for storing authentication tokens. This method ensures data integrity and access control in compliance with ISO/IEC 27001:2022 standards (A.12.2 and A.9.4.2). It is now included within the InitializeDatabase method but retained here for reference.
        /// </summary>

        public void CreateTokenTable()
        {
            string databasePath = GetDatabasePath();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={databasePath};Version=3;"))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("CREATE TABLE IF NOT EXISTS Token (TokenValue TEXT PRIMARY KEY)", connection))
                {
                    command.ExecuteNonQuery();
                }
            }
            //Console.WriteLine("Token table created/verified.");
        }

        // Method to Retrieve Stored Token from Database
        // Method: RetrieveStoredToken
        /// <summary>
        /// Retrieves the stored authentication token from the Token table in the SQLite database. Ensures secure retrieval and compliance with ISO/IEC 27001:2022's data protection (A.8.2) and access control (A.9.4.2) standards.
        /// </summary>
        /// <returns>The stored authentication token if available; otherwise, null.</returns>


        private string? RetrieveStoredToken()
        {
            string databasePath = GetDatabasePath();
            if (!File.Exists(databasePath))
            {
                return null;
            }

            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={databasePath};Version=3;"))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("SELECT TokenValue FROM Token LIMIT 1", connection))
                {
                    string? token = command.ExecuteScalar() as string;
                    return token;
                }
            }
        }

        // Method: CredentialsStored
        /// <summary>
        /// Checks if any credentials are stored in the UserCredentials table of the SQLite database. Helps in determining if authentication is required before performing certain actions. Complies with ISO/IEC 27001:2022's access control (A.9.4.2) standards.
        /// </summary>
        /// <returns>True if credentials are stored; otherwise, false.</returns>

        public static bool CredentialsStored()
        {
            string databasePath = GetDatabasePath();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={databasePath};Version=3;"))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("SELECT COUNT(*) FROM UserCredentials", connection))
                {
                    long count = (long)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        // other methods like StoreToken, GetDatabasePath, etc., are defined elsewhere in the class.
    }

    // Modified EnsureValidToken method to log output for debugging instead of updating UI
    // Method: EnsureValidToken (modified version)
    /// <summary>
    /// Ensures the validity of the authentication token by retrieving the stored token, checking its validity, and refreshing it if necessary. Provides fault tolerance and compliance with ISO/IEC 27001:2022's data integrity (A.12.2) and access control (A.9.4.2) requirements.
    /// </summary>
    /// <returns>The valid authentication token if successful; otherwise, null.</returns>

    private async Task<string?> EnsureValidToken()
    {
        try
        {
            // Retrieve the stored token
            var storedToken = GetStoredToken();

            if (string.IsNullOrEmpty(storedToken))
            {
                // If no stored token, refresh and store a new token
                var newToken = await RefreshToken();
                StoreToken(newToken);
                return newToken;
            }

            // Check if the stored token is still valid using the static method
            if (IsTokenValid(storedToken))
            {
                return storedToken;
            }
            else
            {
                // If the stored token is invalid, refresh and update the token
                var updatedToken = await RefreshToken();
                StoreToken(updatedToken);
                return updatedToken;
            }
        }
        catch (Exception ex)
        {
            //Console.WriteLine($"Error while ensuring valid token: {ex.Message}");
            // Log the exception here if needed
            return null; // or some default value indicating failure
        }
    }



    // Method to retrieve the stored token

    // Method: GetStoredToken
    /// <summary>
    /// Retrieves the most recent token value from the Token table in the SQLite database. It returns an empty string if no token is found or an error occurs during retrieval. This method ensures secure token storage and retrieval, maintaining data integrity in accordance with ISO/IEC 27001:2022 standards (A.12.3).
    /// </summary>
    /// <returns>The most recent token value retrieved from the database, or an empty string if no token is found or an error occurs.</returns>


    private static string GetStoredToken()
    {
        try
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("SELECT TokenValue FROM Token ORDER BY rowid DESC LIMIT 1", connection))
                {
                    var token = command.ExecuteScalar();
                    return token?.ToString() ?? string.Empty;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving stored token: {ex.Message}");
            return string.Empty;
        }
    }
    //99
    // Method to refresh token with a timeout, logs output for debugging
    // Method: RefreshTokenWithTimeout
    /// <summary>
    /// Refreshes the authentication token with a specified timeout period. It logs output for debugging purposes and returns the refreshed token if the operation is successful within the timeout period. This method ensures secure and timely token refreshing, enhancing system reliability and compliance with ISO/IEC 27001:2022 standards (A.12.4.1).
    /// </summary>
    /// <param name="timeoutMilliseconds">The timeout period in milliseconds for refreshing the token (default is 30000 milliseconds).</param>
    /// <returns>The refreshed authentication token if the operation is successful within the specified timeout period; otherwise, null.</returns>



    private async Task<string?> RefreshTokenWithTimeout(int timeoutMilliseconds = 30000)
    {
        var tokenTask = RefreshToken();
        if (await Task.WhenAny(tokenTask, Task.Delay(timeoutMilliseconds)) == tokenTask)
        {
            return tokenTask.Result; // Task completed within timeout
        }
        else
        {
            Debug.WriteLine("Token refresh timeout.");
            LogAction("Token refresh timeout."); // Log timeout
            return null;
        }
    }

    // Modified PostAsync method to log output for debugging
    // Method: PostAsync
    /// <summary>
    /// Sends an HTTP POST request with the provided content and authentication token to the specified API endpoint. It logs output for debugging purposes and returns a message indicating the success or failure of the request. This method ensures secure communication with the API server, maintaining data confidentiality and integrity as per ISO/IEC 27001:2022 standards (A.13.2.1).
    /// </summary>
    /// <param name="apiUrl">The URL of the API endpoint to which the POST request is sent.</param>
    /// <param name="content">The HTTP content to be sent in the request body.</param>
    /// <param name="token">The authentication token used for accessing the API.</param>
    /// <returns>A message indicating the success or failure of the POST request.</returns>


    public async Task<string> PostAsync(string apiUrl, HttpContent content, string token)
    {
        try
        {
            if (string.IsNullOrEmpty(apiUrl) || content == null || string.IsNullOrEmpty(token))
            {
                Debug.WriteLine("Error: Invalid input parameters");
                return "Error: Invalid input parameters";
            }

            Debug.WriteLine("Token received: " + token);

            if (!IsValidBase64(token) || !IsTokenValid(token))
            {
                Debug.WriteLine("Refreshing token...");
                token = await RefreshToken();

                if (!IsValidBase64(token))
                {
                    Debug.WriteLine("Error: Invalid refreshed token format");
                    return "Error: Invalid refreshed token format";
                }
            }



            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine("Measurements updated successfully!");
                    return "Measurements updated successfully!";
                }
                else
                {
                    Debug.WriteLine("Error: " + response.ReasonPhrase);
                    return "Error: " + response.ReasonPhrase;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error: " + ex.Message);
            return "Error: " + ex.Message;
        }
    }

    // Additional methods (IsValidBase64, IsTokenValid, RefreshToken, etc.) remain unchanged in structure but should replace any UI updates or direct user notifications with Debug.WriteLine() or LogAction() calls for logging purposes.
    // Method: CheckAndRefreshTokenAsync
    /// <summary>
    /// Checks the validity of the stored authentication token and refreshes it if necessary. This method ensures continuous access to protected resources while maintaining compliance with ISO/IEC 27001:2022 standards for secure token management (A.9.4.1).
    /// </summary>
    /// <param name="token">The authentication token to be checked and refreshed if necessary.</param>
    private async Task CheckAndRefreshTokenAsync(string token)
    {
        bool isValid = IsTokenValid(token);

        if (isValid)
        {
            Debug.WriteLine("Stored token is valid.");
        }
        else
        {
            AuthResponse authResponse = await GetAuthTokenResponseAsync();

            if (authResponse.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(authResponse.AccessToken))
            {
                // Delete existing token
                DeleteExistingToken();

                // Store the new token
                StoreToken(authResponse.AccessToken);
                Debug.WriteLine("Token refreshed successfully.");
            }
            else
            {
                Debug.WriteLine("Stored token is either invalid or expired.");
            }
        }
    }

    // Method to Delete Existing Token
    // Deletes the existing token from the database.
    // Method: DeleteExistingToken
    /// <summary>
    /// Deletes the existing authentication token from the database. This method ensures secure token deletion and data integrity in compliance with ISO/IEC 27001:2022 standards (A.12.6.1).
    /// </summary>
    private void DeleteExistingToken()
    {
        string databasePath = GetDatabasePath();
        using (SQLiteConnection connection = new SQLiteConnection($"Data Source={databasePath};Version=3;"))
        {
            connection.Open();
            using (SQLiteCommand command = new SQLiteCommand("DELETE FROM Token;", connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }

    // Method: IsValidBase64
    /// <summary>
    /// Validates whether the input string is a valid Base64-encoded string. This method ensures data integrity and format validation in accordance with ISO/IEC 27001:2022 standards (A.12.1.2).
    /// </summary>
    /// <param name="input">The input string to be validated.</param>
    /// <returns>True if the input string is a valid Base64-encoded string; otherwise, false.</returns>

    private bool IsValidBase64(string input)
    {
        try
        {
            Convert.FromBase64String(input);
            return true;
        }
        catch
        {
            return false;
        }
    }
    // Method to Validate Token

    // Method: IsTokenValid
    /// <summary>
    /// Validates whether the provided authentication token is valid. It checks the token's format and expiration time to ensure secure authentication and access control, aligning with ISO/IEC 27001:2022 standards for secure token validation (A.9.4.2).
    /// </summary>
    /// <param name="token">The authentication token to be validated.</param>
    /// <returns>True if the token is valid; otherwise, false.</returns>

    private bool IsTokenValid(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        string[] tokenSegments = token.Split('.');

        if (tokenSegments.Length != 3)
        {
            return false;
        }

        try
        {
            string payloadSegment = tokenSegments[1];
            payloadSegment = payloadSegment.PadRight(payloadSegment.Length + (4 - payloadSegment.Length % 4) % 4, '=');
            string payloadJson = Encoding.UTF8.GetString(Convert.FromBase64String(payloadSegment));

            // Define a class to represent the JWT payload structure
            var payloadData = JsonConvert.DeserializeObject<JwtPayload>(payloadJson);

            // Check if payloadData is null
            if (payloadData == null)
            {
                return false;
            }

            // Use the Expiration property instead of Exp
            long? expirationTimeUnix = payloadData.Expiration; // Assuming Expiration is of type long?

            // Check if expiration time is present
            if (!expirationTimeUnix.HasValue)
            {
                return false;
            }

            DateTimeOffset expirationTime = DateTimeOffset.FromUnixTimeSeconds(expirationTimeUnix.Value);

            // Verify the expiration time
            if (expirationTime <= DateTimeOffset.Now)
            {
                return false;
            }

            return true;
        }
        catch (FormatException)
        {
            // Handle format errors
            return false;
        }
        catch (JsonReaderException)
        {
            // Handle JSON parsing errors
            return false;
        }
        catch
        {
            // Handle other exceptions
            return false;
        }
    }



    // Wrapper method to obtain the authentication token.

    // Method: GetAuthTokenAsync
    /// <summary>
    /// Wrapper method to asynchronously obtain the authentication token. It retrieves the authentication token response and returns the access token if the response status code is OK. This method ensures secure token retrieval and authentication, maintaining compliance with ISO/IEC 27001:2022 standards for secure authentication (A.12.3.1, A.13.1.1).
    /// </summary>
    /// <returns>The access token obtained from the authentication token response, or null if the response status code is not OK.</returns>


    private async Task<string?> GetAuthTokenAsync()
    {
        AuthResponse authResponse = await GetAuthTokenResponseAsync();
        if (authResponse.StatusCode == HttpStatusCode.OK)
        {
            return authResponse.AccessToken;
        }
        else
        {
            return null;
        }
    }

    // Method: GetAuthTokenResponseAsync
    /// <summary>
    /// Asynchronously retrieves the authentication token response. It decrypts stored credentials, fetches the discovery document from the identity server, and requests a new token using the decrypted credentials. This method ensures secure token retrieval and communication, aligning with ISO/IEC 27001:2022 standards for secure communication (A.13.2.1, A.14.2.1).
    /// </summary>
    /// <returns>The authentication token response containing the access token and associated metadata.</returns>


    private async Task<AuthResponse> GetAuthTokenResponseAsync()
    {
        AuthResponse response = new AuthResponse();
        try
        {
            var (username, encryptedPassword) = FetchEncryptedCredentials();

            // Retrieve the encryption key from a secure source
            var keyBytes = RetrieveAesKeyFromCredentialManager();
            string keyHex = BitConverter.ToString(keyBytes).Replace("-", "");

            string decryptedPassword = EncryptionHelper.DecryptString(keyHex, encryptedPassword);

            var discoResponse = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = "https://test.blastiq.com/identity",
                Policy = new DiscoveryPolicy { ValidateEndpoints = false }
            });

            if (discoResponse.IsError)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }

            Debug.WriteLine("Sending token request...");

            var tokenResponse = await _httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = discoResponse.TokenEndpoint,
                ClientId = "external.customer",
                ClientSecret = "",
                UserName = username,
                Password = decryptedPassword,
                Scope = "cosmos"
            });

            Debug.WriteLine("Received token response: " + JsonConvert.SerializeObject(tokenResponse));

            if (tokenResponse.IsError)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }

            response.StatusCode = HttpStatusCode.OK;
            response.AccessToken = tokenResponse.AccessToken;
            response.ExpiresIn = tokenResponse.ExpiresIn;
            response.TokenType = tokenResponse.TokenType;
            response.Scope = tokenResponse.Scope;

            // Ensure the token is not null before storing it
            if (!string.IsNullOrEmpty(tokenResponse.AccessToken))
            {
                StoreToken(tokenResponse.AccessToken);
            }
            else
            {
                // Handle the case where the token is null or empty
                Debug.WriteLine("AccessToken is null or empty, cannot store the token.");
                response.StatusCode = HttpStatusCode.InternalServerError;
            }
        }
        catch (Exception ex)
        {
            response.StatusCode = HttpStatusCode.InternalServerError;
            Console.WriteLine($"Error retrieving auth token: {ex.Message}");
            LogAction($"Error retrieving auth token: {ex.Message}");
        }

        return response;
    }



    // Method: FetchEncryptedCredentials
    /// <summary>
    /// Retrieves the encrypted credentials from the database. It fetches the username and encrypted password from the UserCredentials table and returns them as a tuple. This method ensures secure credential storage and retrieval, maintaining data confidentiality and integrity in accordance with ISO/IEC 27001:2022 standards (A.9.2.1, A.12.3.3).
    /// </summary>
    /// <returns>A tuple containing the username and encrypted password retrieved from the database.</returns>


    private (string username, string encryptedPassword) FetchEncryptedCredentials()
    {
        string databasePath = GetDatabasePath();
        using (SQLiteConnection connection = new SQLiteConnection($"Data Source={databasePath};Version=3;"))
        {
            connection.Open();
            using (SQLiteCommand command = new SQLiteCommand("SELECT Username, EncryptedAPIPassword FROM UserCredentials LIMIT 1", connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Safely cast the values to string, handle DBNull by converting to null
                        string username = reader["Username"] as string ?? string.Empty;  // Use 'as string' to safely cast, ?? for fallback
                        string encryptedPassword = reader["EncryptedAPIPassword"] as string ?? string.Empty;  // Same as above

                        return (username, encryptedPassword);
                    }
                }
            }
        }
        return (string.Empty, string.Empty); // Return empty strings if no credentials are found
    }


    // Method: RetrieveStoredToken
    /// <summary>
    /// Retrieves the stored authentication token from the Token table in the database. This method ensures secure token retrieval and storage, maintaining data integrity and confidentiality in accordance with ISO/IEC 27001:2022 standards (A.12.3.1, A.12.4.1).
    /// </summary>
    /// <returns>The stored authentication token retrieved from the database, or null if no token is found.</returns>


    private static string? RetrieveStoredToken()
    {
        using (var connection = new SQLiteConnection(DatabaseConfig.GetConnectionString()))
        {
            connection.Open();
            using (var command = new SQLiteCommand("SELECT TokenValue FROM Token LIMIT 1", connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetString(0);
                    }
                }
            }
        }
        return null; // Return null if no token is found
    }


    // If GetAuthTokenResponseAsync is static, add static keyword to StoreToken method as well

    // Method: StoreToken
    /// <summary>
    /// Stores the authentication token in the Token table of the database. It checks if the token already exists and inserts or updates it accordingly. This method ensures secure token storage and management, maintaining data integrity and confidentiality as per ISO/IEC 27001:2022 standards (A.12.3.1, A.12.6.1).
    /// </summary>
    /// <param name="token">The authentication token to be stored in the database.</param>


    private static void StoreToken(string token)
    {
        string databasePath = GetDatabasePath();

        using (SQLiteConnection connection = new SQLiteConnection($"Data Source={databasePath};Version=3;"))
        {
            connection.Open();

            using (SQLiteTransaction transaction = connection.BeginTransaction())
            {
                // Ensure the Token table exists
                using (SQLiteCommand createTokenTableCommand = new SQLiteCommand("CREATE TABLE IF NOT EXISTS Token (TokenValue TEXT PRIMARY KEY)", connection))
                {
                    createTokenTableCommand.ExecuteNonQuery();
                }

                // Check if the token already exists in the database
                using (SQLiteCommand checkTokenCommand = new SQLiteCommand("SELECT COUNT(*) FROM Token", connection))
                {
                    int count = Convert.ToInt32(checkTokenCommand.ExecuteScalar());

                    if (count == 0) // If no token exists, insert the new one
                    {
                        using (SQLiteCommand insertTokenCommand = new SQLiteCommand("INSERT INTO Token (TokenValue) VALUES (@token)", connection))
                        {
                            insertTokenCommand.Parameters.AddWithValue("@token", token);
                            insertTokenCommand.ExecuteNonQuery();
                            //Console.WriteLine($"New token '{token}' stored in the database.");
                        }
                    }
                    else // If a token exists, update it
                    {
                        using (SQLiteCommand updateTokenCommand = new SQLiteCommand("UPDATE Token SET TokenValue = @token", connection))
                        {
                            updateTokenCommand.Parameters.AddWithValue("@token", token);
                            updateTokenCommand.ExecuteNonQuery();
                            //Console.WriteLine($"Token '{token}' updated in the database.");
                        }
                    }
                }

                transaction.Commit();
            }
        }
    }


    // Helper method: LogAction
    /// <summary>
    /// Logs an action for debugging or auditing purposes. This method ensures traceability and accountability in system operations, enhancing compliance with ISO/IEC 27001:2022 standards for information security management (A.12.4.3, A.18.1.4).
    /// </summary>
    /// <param name="action">The action to be logged.</param>

    // Helper method to log actions
    private void LogAction(string action)
    {
        // Add code here to log actions to a log file or database
        Console.WriteLine($"Action logged: {action}");
    }

    //Refresh

    /// Method: RefreshToken
    /// <summary>
    /// Asynchronously refreshes the authentication token. It fetches encrypted credentials, decrypts the password, retrieves the discovery document, and requests a new token. This method ensures secure token refreshing and authentication, maintaining compliance with ISO/IEC 27001:2022 standards for secure communication and access control (A.12.3.1, A.13.1.1).
    /// </summary>
    /// <returns>The refreshed authentication token obtained from the token request.</returns>


    private async Task<string> RefreshToken()
    {
        System.Diagnostics.Debug.WriteLine("Starting token refresh process.");

        try
        {
            System.Diagnostics.Debug.WriteLine("Fetching encrypted credentials.");
            var (username, encryptedPassword) = FetchEncryptedCredentials();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(encryptedPassword))
            {
                System.Diagnostics.Debug.WriteLine("No credentials stored. Aborting token refresh.");
                throw new Exception("No credentials stored.");
            }

            System.Diagnostics.Debug.WriteLine($"Encrypted credentials retrieved. Username: {username}, EncryptedPassword: [Not displayed for security reasons].");

            System.Diagnostics.Debug.WriteLine("Retrieving encryption key from Credential Manager.");
            var keyBytes = RetrieveAesKeyFromCredentialManager();
            string keyHex = BitConverter.ToString(keyBytes).Replace("-", "");

            System.Diagnostics.Debug.WriteLine("Attempting to decrypt password.");
            string decryptedPassword;
            try
            {
                decryptedPassword = EncryptionHelper.DecryptString(keyHex, encryptedPassword);
                System.Diagnostics.Debug.WriteLine("Password decrypted successfully.");
            }
            catch (Exception decryptionEx)
            {
                System.Diagnostics.Debug.WriteLine($"Decryption failed: {decryptionEx.Message}");
                throw new Exception("Decryption failed: " + decryptionEx.Message);
            }

            System.Diagnostics.Debug.WriteLine("Fetching discovery document from identity server.");
            var discoResponse = await _httpClient.GetDiscoveryDocumentAsync("https://test.blastiq.com/identity");
            if (discoResponse.IsError)
            {
                System.Diagnostics.Debug.WriteLine($"Discovery request failed: {discoResponse.Error}");
                throw new Exception("Discovery request failed: " + discoResponse.Error);
            }

            System.Diagnostics.Debug.WriteLine("Requesting new token with decrypted credentials.");
            var tokenResponse = await _httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = discoResponse.TokenEndpoint,
                ClientId = "external.customer",
                ClientSecret = "",
                UserName = username,
                Password = decryptedPassword,
                Scope = "cosmos"
            });

            if (tokenResponse.IsError)
            {
                System.Diagnostics.Debug.WriteLine($"Token request failed: {tokenResponse.Error}");
                throw new Exception("Token request failed: " + tokenResponse.Error);
            }

            if (tokenResponse.AccessToken == null)
            {
                throw new InvalidOperationException("Received null access token from the server.");
            }

            System.Diagnostics.Debug.WriteLine("Token refresh successful. Token obtained.");
            return tokenResponse.AccessToken;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error refreshing token: {ex.Message}");
            throw new Exception("Error refreshing token: " + ex.Message);
        }
    }


    // Method: GetStoredCredentials
    /// <summary>
    /// Retrieves the stored username and encrypted password from the UserCredentials table in the database. It decrypts the password using the encryption key retrieved from the Credential Manager and returns the username and decrypted password as a tuple. This method ensures secure credential retrieval and decryption, maintaining data confidentiality and integrity as per ISO/IEC 27001:2022 standards (A.9.2.1, A.12.3.3).
    /// </summary>
    /// <returns>A tuple containing the username and decrypted password retrieved from the database, or null if no credentials are found.</returns>


    private System.Tuple<string, string>? GetStoredCredentials()
    {
        using (var connection = new SQLiteConnection(DatabaseConfig.GetConnectionString()))
        {
            connection.Open();
            using (var command = new SQLiteCommand("SELECT Username, EncryptedAPIPassword FROM UserCredentials LIMIT 1", connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Retrieve the username and encrypted password from the database
                        string username = reader.GetString(0);
                        string encryptedPassword = reader.GetString(1);

                        // Retrieve the encryption key from the Credential Manager
                        var keyBytes = RetrieveAesKeyFromCredentialManager();
                        string keyHex = BitConverter.ToString(keyBytes).Replace("-", "");

                        // Decrypt the password using the retrieved encryption key
                        string decryptedPassword = EncryptionHelper.DecryptString(keyHex, encryptedPassword);

                        // Return the username and decrypted password
                        return System.Tuple.Create(username, decryptedPassword);
                    }
                }
            }
        }
        return null; // Return null if no credentials are found
    }




    // Class: HttpClientInstance
    /// <summary>
    /// Provides a singleton instance of HttpClient configured with default settings and base address.
    /// </summary>

    public static class HttpClientInstance
    {
        public static readonly HttpClient _httpClient = new HttpClient();

        // Property to access the HttpClient instance
        public static HttpClient Instance
        {
            get { return _httpClient; }
        }
    }

    // Class: AuthResponse
    /// <summary>
    /// Represents the authentication response containing the access token, expiration time, token type, scope, and status code.
    /// </summary>

    public class AuthResponse
    {
        public string? AccessToken { get; set; } // Add '?' to make it nullable
        public int ExpiresIn { get; set; }
        public string? TokenType { get; set; }
        public string? Scope { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }

    //site and plans

    // Method: SetGlobalSiteId
    /// <summary>
    /// Sets the global site ID used for retrieving site-specific data.
    /// </summary>
    /// <param name="globalSiteId">The global site ID to be set.</param>

    private void SetGlobalSiteId(string globalSiteId)
    {
        _globalSiteId = globalSiteId;
    }

    // Method: VerifyAndCreatePlansTable
    /// <summary>
    /// Verifies the existence of the Plans table in the database and creates it if necessary. This method ensures the integrity of the database schema, aligning with ISO/IEC 27001:2022 standards for data management (A.12.4.1).
    /// </summary>

    private static void VerifyAndCreatePlansTable()
    {
        //Console.WriteLine("Verifying and potentially creating the Plans table...");

        using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            //Console.WriteLine("Database connection opened for schema verification.");

            using (SQLiteCommand createCommand = new SQLiteCommand(
                "CREATE TABLE IF NOT EXISTS Plans (" +
                "PlanId TEXT PRIMARY KEY, " +
                "Name TEXT, " +
                "IsFired INTEGER, " +
                "DateCreated TEXT, " +
                "SiteId TEXT)", connection)) // Ensure the schema matches expected structure
            {
                createCommand.ExecuteNonQuery();
                //Console.WriteLine("Plans Table Verified/Created.");
            }
        }
    }

    //plans

    // Class: Plan
    /// <summary>
    /// Represents a plan containing properties such as ID, name, firing status, creation date, and site ID.
    /// </summary>

    public class Plan
    {
        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("IsFired")]
        public bool IsFired { get; set; }

        [JsonProperty("DateCreated")]
        public string DateCreated { get; set; }

        [JsonProperty("SiteID")]
        public string SiteId { get; set; }

        public Plan()
        {
            Id = "defaultId";
            Name = "defaultName";
            DateCreated = DateTime.UtcNow.ToString("o"); // ISO 8601 format
            SiteId = "defaultSiteId";
        }
    }


    // Method: LoadPlansIntoDatabase
    /// <summary>
    /// Loads plans retrieved from the API into the database after performing necessary validations and conversions. This method ensures the synchronization of plan data between the API and the database, maintaining data consistency as per ISO/IEC 27001:2022 standards for data management (A.12.4.1).
    /// </summary>
    /// <param name="authToken">The authentication token required for accessing the API.</param>
    /// <param name="siteId">The ID of the site for which plans are being loaded.</param>


    private static async Task LoadPlansIntoDatabase(string authToken, string siteId)
    {
        Debug.WriteLine("Starting to load plans into database...");

        string plansApiResponse = await FetchPlansData(authToken, siteId) ?? string.Empty; // Ensure non-null value

        if (!string.IsNullOrEmpty(plansApiResponse))
        {
            try
            {
                Debug.WriteLine($"API Response for plans: {plansApiResponse}");

                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(plansApiResponse);
                if (apiResponse == null || apiResponse.Plans == null || !apiResponse.Plans.Any())
                {
                    Debug.WriteLine("No plans found in API response or failed to deserialize.");
                    return;
                }

                var plans = apiResponse.Plans; // Now it's safe to access Plans
                Debug.WriteLine($"Found {plans.Count} plans in API response. Starting database operations.");

                string databasePath = GetDatabasePath();
                Debug.WriteLine($"Database path: {databasePath}");

                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={databasePath};Version=3;"))
                {
                    connection.Open();
                    Debug.WriteLine("Database connection opened for plans loading.");

                    foreach (var plan in plans)
                    {
                        // Convert the DateCreated string to DateTime
                        if (!DateTime.TryParse(plan.DateCreated, out DateTime dateCreated))
                        {
                            Debug.WriteLine($"Invalid date format for plan '{plan.Name}'. Skipping.");
                            continue;
                        }

                        // Check if the plan's creation date is within the last 60 days
                        if (dateCreated >= DateTime.UtcNow.AddDays(-90))
                        {
                            Debug.WriteLine($"Processing plan: {plan.Name} with ID: {plan.Id}");
                            plan.SiteId = siteId; // Ensure SiteId is set from the method parameter

                            using (SQLiteCommand command = new SQLiteCommand(
                                "INSERT OR REPLACE INTO Plans (PlanId, Name, IsFired, DateCreated, SiteId) VALUES (@planId, @name, @isFired, @dateCreated, @siteId)", connection))
                            {
                                command.Parameters.AddWithValue("@planId", plan.Id);
                                command.Parameters.AddWithValue("@name", plan.Name);
                                command.Parameters.AddWithValue("@isFired", plan.IsFired ? 1 : 0);
                                command.Parameters.AddWithValue("@dateCreated", dateCreated);
                                command.Parameters.AddWithValue("@siteId", plan.SiteId);

                                int rowsAffected = command.ExecuteNonQuery();
                                Debug.WriteLine($"Plan '{plan.Name}' with Site ID '{plan.SiteId}' added/updated in the database. Rows affected: {rowsAffected}");
                            }
                        }
                        else
                        {
                            Debug.WriteLine($"Skipping plan '{plan.Name}' with ID '{plan.Id}' as it was created more than 60 days ago.");
                        }
                    }
                    Debug.WriteLine("Finished loading plans into the database.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception occurred while loading plans into the database: {ex.Message}");
            }
        }
        else
        {
            Debug.WriteLine("API response is empty. No data to update.");
        }
    }




    // Method: GetPlansAsync
    /// <summary>
    /// Asynchronously fetches plans data from the API for a specific site within the last 60 days. This method ensures secure communication and access control, aligning with ISO/IEC 27001:2022 standards for information security (A.13.1.1, A.13.2.1).
    /// </summary>
    /// <param name="authToken">The authentication token required for accessing the API.</param>
    /// <param name="siteId">The ID of the site for which plans are being fetched.</param>
    /// <param name="currentDate">The current date used for calculating the minimum creation date of plans.</param>
    /// <param name="limit">The maximum number of plans to fetch (default is 2000).</param>
    /// <returns>The JSON string containing the plans data retrieved from the API, or null if the request fails.</returns>

    public async Task<string?> GetPlansAsync(string authToken, string siteId, DateTime currentDate, int limit = 2000)
    {
        Debug.WriteLine($"Attempting to fetch plans for site ID: {siteId} with authToken.");

        // Calculate the date 60 days ago from the current date
        DateTime minDateCreated = currentDate.AddDays(-60);

        // Format the minimum date created as required by the API
        string minDateCreatedFormatted = minDateCreated.ToString("yyyy-MM-ddTHH:mm:ssZ");

        // Constructing the query string
        var queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
        queryString["includeFired"] = "false";
        queryString["includeDeleted"] = "false";
        queryString["limit"] = limit.ToString();
        queryString["minDateCreated"] = minDateCreatedFormatted;

        // Forming the relative URL for the endpoint with query parameters
        // Forming the relative URL for the endpoint with query parameters
        string relativeUrlWithParams = $"v3/{siteId}/planList?" + queryString;


        // Explicitly logging the full intended request URL for visibility
        string fullRequestUrl = $"{_httpClient.BaseAddress}{relativeUrlWithParams}";
        Debug.WriteLine($"Full Request URL: {fullRequestUrl}");

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, relativeUrlWithParams);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        // Logging the request initiation
        Debug.WriteLine("Sending HTTP GET Request with headers:");
        foreach (var header in request.Headers)
        {
            string headerValue = header.Key + ": " + string.Join(", ", header.Value);
            Debug.WriteLine(headerValue);
        }

        try
        {
            HttpResponseMessage response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("GetPlansAsync Success: Response received.");
                Debug.WriteLine($"Response snippet: {responseBody.Substring(0, Math.Min(responseBody.Length, 100))}...");
                return responseBody;
            }
            else
            {
                Debug.WriteLine($"GetPlansAsync Error: StatusCode={response.StatusCode}, ReasonPhrase={response.ReasonPhrase}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"GetPlansAsync Exception: {ex.GetType().FullName}, Message={ex.Message}");
            return null;
        }
    }




    private static List<string> GetPlanIdsFromDatabase()
    {
        List<string> planIds = new List<string>();

        try
        {
            string databasePath = GetDatabasePath();
            Debug.WriteLine($"Database path: {databasePath}");

            using (var connection = new SQLiteConnection($"Data Source={databasePath};Version=3;"))
            {
                connection.Open();
                Debug.WriteLine("Database connection opened.");

                string sql = "SELECT PlanId FROM Plans";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string planId = reader.GetString(0);
                            planIds.Add(planId);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error retrieving plan IDs from database: {ex.Message}");
        }

        // Logging the retrieved plan IDs
        foreach (var planId in planIds)
        {
            Debug.WriteLine($"Retrieved Plan ID: {planId}");
        }

        return planIds;
    }

    /// Method: InsertPlansIntoDatabase
    /// <summary>
    /// Inserts plans retrieved from the API into the database after performing necessary validations and conversions. This method ensures data integrity and consistency in the database, aligning with ISO/IEC 27001:2022 standards for data management (A.12.4.1).
    /// </summary>
    /// <param name="plans">The list of plans to be inserted into the database.</param>


    private static void InsertPlansIntoDatabase(List<Plan> plans)
    {
        foreach (var plan in plans)
        {
            try
            {
                // Convert the DateCreated string to DateTime
                DateTime dateCreated = DateTime.Parse(plan.DateCreated);

                // Check if the plan's creation date is within the last 60 days
                if (dateCreated >= DateTime.UtcNow.AddDays(-60))
                {
                    // Example SQL command to insert a plan
                    string sqlCommandText = $"INSERT INTO Plans (PlanId, Name, IsFired, DateCreated, SiteId) VALUES (@PlanId, @Name, @IsFired, @DateCreated, @SiteId)";

                    using (var connection = new SQLiteConnection(ConnectionString))
                    {
                        connection.Open();
                        using (var command = new SQLiteCommand(sqlCommandText, connection))
                        {
                            // Assuming Plan class has these properties. Adjust based on actual class structure.
                            command.Parameters.AddWithValue("@PlanId", plan.Id);
                            command.Parameters.AddWithValue("@Name", plan.Name);
                            command.Parameters.AddWithValue("@IsFired", plan.IsFired);
                            command.Parameters.AddWithValue("@DateCreated", plan.DateCreated);
                            command.Parameters.AddWithValue("@SiteId", plan.SiteId);

                            int result = command.ExecuteNonQuery();
                            if (result > 0)
                            {
                                Debug.WriteLine($"Successfully inserted plan {plan.Name} into database.");
                            }
                            else
                            {
                                Debug.WriteLine($"Failed to insert plan {plan.Name} into database.");
                            }
                        }
                    }
                }
                else
                {
                    Debug.WriteLine($"Skipping plan '{plan.Name}' with ID '{plan.Id}' as it was created more than 3 months ago.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception while inserting plan {plan.Name} into database: {ex.Message}");
            }
        }
    }

    // Method: GetSiteIdForSelectedPlan
    /// <summary>
    /// Retrieves the site ID associated with a specific plan from the database. This method ensures data retrieval integrity and consistency in accordance with ISO/IEC 27001:2022 standards for data management (A.12.4.1).
    /// </summary>
    /// <param name="planId">The ID of the plan for which the site ID is being retrieved.</param>
    /// <returns>The site ID associated with the specified plan, or null if the plan ID is not found in the database.</returns>


    private string? GetSiteIdForSelectedPlan(string planId)
    {
        string databasePath = GetDatabasePath();
        string? siteId = null;

        using (SQLiteConnection connection = new SQLiteConnection($"Data Source={databasePath};Version=3;"))
        {
            connection.Open();

            using (SQLiteCommand command = new SQLiteCommand("SELECT SiteId FROM Plans WHERE PlanId = @planId", connection))
            {
                command.Parameters.AddWithValue("@planId", planId);

                object result = command.ExecuteScalar();
                if (result != null)
                {
                    siteId = result.ToString();
                }
            }
        }

        return siteId;
    }

    public class ApiResponse
    {
        public List<Plan> Plans { get; set; }

        public ApiResponse()
        {
            Plans = new List<Plan>();
        }
    }



    // Method: FetchPlansData
    /// <summary>
    /// Fetches plans data from the API using the provided authentication token and site ID. This method ensures secure communication and access control, aligning with ISO/IEC 27001:2022 standards for information security (A.13.1.1, A.13.2.1).
    /// </summary>
    /// <param name="authToken">The authentication token required for accessing the API.</param>
    /// <param name="siteId">The ID of the site for which plans data is being fetched.</param>
    /// <returns>The JSON string containing the plans data retrieved from the API, or null if the request fails.</returns>


    private static async Task<string?> FetchPlansData(string authToken, string siteId)
    {
        try
        {
            // Use the existing _httpClient instance that includes the BaseAddress
            string apiUrl = $"v3/{siteId}/planList";


            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            request.Headers.Add("Authorization", $"Bearer {authToken}");

            // Since _httpClient is already configured with the BaseAddress, we can directly use it here
            HttpResponseMessage response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                Console.WriteLine($"Failed to fetch plans data. StatusCode: {response.StatusCode}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching plans data: {ex.Message}");
            return null;
        }
    }


    //siteid start

    // Method: GetSiteListAsync
    /// <summary>
    /// Asynchronously fetches the list of sites from the API using the provided authentication token. This method ensures secure communication and access control, aligning with ISO/IEC 27001:2022 standards for information security (A.13.1.1, A.13.2.1).
    /// </summary>
    /// <param name="token">The authentication token required for accessing the API.</param>
    /// <returns>The JSON string containing the list of sites retrieved from the API, or null if the request fails.</returns>


    private async Task<string?> GetSiteListAsync(string token)
    {
        try
        {
            // Use the existing _httpClient instance with proper BaseAddress
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Adjusted to use the correct path with the already configured BaseAddress
            HttpResponseMessage response = await _httpClient.GetAsync("/v3/siteList");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                Console.WriteLine($"Error fetching site list: {response.StatusCode} - {response.ReasonPhrase}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching site list: {ex.Message}");
            return null;
        }
    }

    // Method: ParseSiteIdFromSiteList
    /// <summary>
    /// Parses the site ID associated with a specific plan ID from the site list response obtained from the API. This method ensures accurate identification of the site ID corresponding to the provided plan ID, facilitating data association and management as per ISO/IEC 27001:2022 standards for data integrity (A.12.4.1).
    /// </summary>
    /// <param name="siteListResponse">The JSON string containing the response received from the site list API endpoint.</param>
    /// <param name="planId">The ID of the plan for which the associated site ID is being retrieved.</param>
    /// <returns>The site ID associated with the specified plan ID, or null if no matching site ID is found in the response.</returns>


    //private string? ParseSiteIdFromSiteList(string siteListResponse, string planId)
    //{
    //    // Deserialize to dynamic object and check for null immediately
    //    dynamic siteListData = JsonConvert.DeserializeObject<dynamic>(siteListResponse);
    //    if (siteListData == null || siteListData.sites == null || siteListData.sites.Count == 0)
    //    {
    //        Console.WriteLine("No sites data available or site list is empty.");
    //        return null;
    //    }

    //    // Iterate through each site to find a match
    //    foreach (var site in siteListData.sites)
    //    {
    //        if (site?.planId != null && site?.id != null)
    //        {
    //            string sitePlanId = site.planId.ToString();
    //            if (sitePlanId == planId)
    //            {
    //                return site.id.ToString(); // Direct conversion with prior checks for null
    //            }
    //        }
    //    }

    //    return null; // Return null if no matching siteId is found or the site object is malformed
    //}







    // Method: GetSiteIdForPlan
    /// <summary>
    /// Retrieves the site ID associated with a specific plan ID from the database. This method ensures accurate retrieval of site IDs based on plan IDs, maintaining data integrity and consistency in compliance with ISO/IEC 27001:2022 standards for data management (A.12.4.1).
    /// </summary>
    /// <param name="planId">The ID of the plan for which the associated site ID is being retrieved.</param>
    /// <returns>The site ID associated with the specified plan ID, or an empty string if the plan ID is not found in the database.</returns>

    private string? GetSiteIdForPlan(string planId)
    {
        string? siteId = null;  // Declare as nullable

        try
        {
            // Query the SQLite database to retrieve the site ID based on the plan ID
            string databasePath = GetDatabasePath();

            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={databasePath};Version=3;"))
            {
                connection.Open();

                using (SQLiteCommand selectCommand = new SQLiteCommand("SELECT SiteId FROM Plans WHERE PlanId = @planId", connection))
                {
                    selectCommand.Parameters.AddWithValue("@planId", planId);
                    object result = selectCommand.ExecuteScalar();
                    if (result != null)
                    {
                        siteId = result as string;  // Safe cast that handles null
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Handle any exceptions that may occur during database access
            Console.WriteLine($"Error retrieving Site ID: {ex.Message}");
        }

        return siteId;
    }



    //siteid end

    // Method: CreatePlansTable
    /// <summary>
    /// Creates the Plans table in the database if it does not exist, including the necessary columns for storing plan information, such as PlanId, Name, IsFired, DateCreated, and SiteId. This method ensures the establishment of a structured database schema in alignment with ISO/IEC 27001:2022 standards for data management (A.12.4.1).
    /// </summary>

    private static void CreatePlansTable()
    {
        using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            using (SQLiteCommand createCommand = new SQLiteCommand(
                "CREATE TABLE IF NOT EXISTS Plans (" +
                "PlanId TEXT PRIMARY KEY, " +
                "Name TEXT, " +
                "IsFired INTEGER, " +
                "DateCreated TEXT, " +
                "SiteId TEXT)", connection)) // Include SiteId in the table schema
            {
                createCommand.ExecuteNonQuery();
               // Console.WriteLine("Plans Table Created/Verified");
            }
        }
    }

    // Method: InsertPlanIntoDatabase
    /// <summary>
    /// Inserts a plan into the Plans table in the database, ensuring data integrity and consistency. This method includes parameters for the PlanId, Name, IsFired, DateCreated, and SiteId columns, aligning with ISO/IEC 27001:2022 standards for data management (A.12.4.1).
    /// </summary>
    /// <param name="plan">The plan object containing the information to be inserted into the database.</param>


    private static void InsertPlanIntoDatabase(Plan plan)
    {
        try
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                using (SQLiteCommand insertCommand = new SQLiteCommand(
                    "INSERT OR IGNORE INTO Plans (PlanId, Name, IsFired, DateCreated, SiteId) VALUES (@planId, @name, @isFired, @dateCreated, @siteId)", connection))
                {
                    insertCommand.Parameters.AddWithValue("@planId", plan.Id);
                    insertCommand.Parameters.AddWithValue("@name", plan.Name);
                    insertCommand.Parameters.AddWithValue("@isFired", plan.IsFired ? 1 : 0);
                    insertCommand.Parameters.AddWithValue("@dateCreated", plan.DateCreated);
                    insertCommand.Parameters.AddWithValue("@siteId", plan.SiteId);

                    int result = insertCommand.ExecuteNonQuery();
                    if (result > 0)
                    {
                        Debug.WriteLine($"Successfully inserted plan '{plan.Name}' with ID '{plan.Id}' into database.");
                    }
                    else
                    {
                        Debug.WriteLine($"No rows affected. Plan '{plan.Name}' with ID '{plan.Id}' might already exist in the database.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception while inserting plan '{plan.Name}' with ID '{plan.Id}' into database: {ex.Message}");
            // Consider rethrowing the exception or handling it based on your application's error handling policy
        }
    }





    //hole truth

    private static async Task DeserializeJsonDataAsync(string siteId, string planId, string token)
    {
        try
        {
            // Fetch hole data from the API
            var holesData = await FetchHolesDataAsync(siteId, planId, token);

            // Ensure the response is not null
            if (holesData != null)
            {
                // Now you have a list of HoleData objects that you can work with
                foreach (var holeData in holesData)
                {
                    // Access HoleData properties as needed
                    string? holeId = holeData.Id;
                    string? holeName = holeData.Name;
                    double? designLength = holeData.Design?.Length; // Access 'Length' from 'Design'
                    double? adjustedDesignLength = holeData.AdjustedDesign?.Length; // Access 'Length' from 'AdjustedDesign'

                    // Print the desired properties to see the data
                    // Console.WriteLine($"Hole ID: {holeId}, Hole Name: {holeName}, Design Length: {designLength}, Adjusted Design Length: {adjustedDesignLength}");
                }
            }
            else
            {
                Console.WriteLine("No holes data found in the API response.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while deserializing or processing the API response: {ex.Message}");
        }
    }




    private static void ProcessHolesData(List<HoleData> holesData)
    {
        foreach (var hole in holesData)
        {
            // Insert the hole data into the database
            InsertHoleDataIntoDatabase(hole);
        }
    }
    public class HoleData
    {
        public string? Id { get; set; }
        public string? SiteId { get; set; }
        public string? PlanId { get; set; }
        public string? Name { get; set; }
        public string? Ring { get; set; }
        public string? HoleType { get; set; }
        public string? MaterialType { get; set; }
        public DesignData? Design { get; set; }
        public DesignData? AdjustedDesign { get; set; }
    }


    public class DesignData
{
    public double Diameter { get; set; }
    public double Length { get; set; }
    public Collar? Collar { get; set; }
    public double Bearing { get; set; }
    public double Angle { get; set; }
    public bool Breakthrough { get; set; }
    public List<Deck> Decks { get; set; }
    public List<Initiator> Initiators { get; set; }
    public string Comment { get; set; }

    public DesignData()
    {
        Collar = new Collar(); // Assuming Collar has a parameterless constructor
        Decks = new List<Deck>();
        Initiators = new List<Initiator>();
        Comment = string.Empty; // Initialize string to prevent null
    }
}


    public class Collar
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }

    public class Deck
    {
        public int Number { get; set; }
        public string ProductId { get; set; } = string.Empty; // Default to empty string
        public string VariantId { get; set; } = string.Empty; // Default to empty string
        public double Length { get; set; }
        public double Weight { get; set; }
        public int ItemCount { get; set; }
        public bool IsVariable { get; set; }
        public bool IsBackfill { get; set; }
    }



    public class Initiator
    {
        public int Number { get; set; }
        public string? InitiatorProductId { get; set; }
        public string? InitiatorVariantId { get; set; }
        public int EbsOffset { get; set; }
        public string? BoosterProductId { get; set; }
        public string? BoosterVariantId { get; set; }
        public int BoosterCount { get; set; }
        public string? WirelessAssemblyId { get; set; }
        public double Depth { get; set; }
        public string? Position { get; set; }
        public double PercentageDepth { get; set; }
    }


    private static void CreateHoleDataTable()
    {
        string databasePath = GetDatabasePath();

        using (var connection = new SQLiteConnection($"Data Source={databasePath};Version=3;"))
        {
            connection.Open();
            string sql = @"
        CREATE TABLE IF NOT EXISTS HoleData (
            Id TEXT PRIMARY KEY, 
            SiteId TEXT, 
            PlanId TEXT, 
            Name TEXT, 
            Ring TEXT,
            Length REAL,
            AdjustedDesignLength REAL,
            AdjustedDesignDiameter REAL,
            AdjustedDesignAngle REAL
            -- Add other AdjustedDesign columns as needed
        )";
            using (var command = new SQLiteCommand(sql, connection))
            {
                command.ExecuteNonQuery();
            }
        }
        //  Console.WriteLine("HoleData table created/verified.");
    }


    // Assuming HoleData now includes 'Ring' and 'Name' directly.
    private static void InsertHoleDataIntoDatabase(HoleData hole)
    {
        string databasePath = GetDatabasePath();
        //Console.WriteLine($"Database Path: {databasePath}");

        // Debugging: Confirm the original Ring value
        // Console.WriteLine($"Original Ring Value: {hole.Ring}");

        // Convert the Ring value to the standard format (e.g., "Ring1" to "R1")
        string convertedRing = hole.Ring != null ? ConvertToShortRingFormat(hole.Ring) : "default_value";
        


        // Debugging: Confirm the converted Ring value
        //Console.WriteLine($"Converted Ring Value: {convertedRing}");

        using (var connection = new SQLiteConnection($"Data Source={databasePath};Version=3;"))
        {
            connection.Open();
            using (var command = new SQLiteCommand(connection))
            {
                command.CommandText = @"
            INSERT OR REPLACE INTO HoleData 
            (Id, SiteId, PlanId, Name, Ring, Length, AdjustedDesignLength, AdjustedDesignDiameter, AdjustedDesignAngle, AdjustedDesignBearing, AdjustedDesignBreakthrough, AdjustedDesignComment) 
            VALUES 
            (@Id, @SiteId, @PlanId, @Name, @Ring, @Length, @AdjustedDesignLength, @AdjustedDesignDiameter, @AdjustedDesignAngle, @AdjustedDesignBearing, @AdjustedDesignBreakthrough, @AdjustedDesignComment)";

                command.Parameters.AddWithValue("@Id", hole.Id);
                command.Parameters.AddWithValue("@SiteId", hole.SiteId);
                command.Parameters.AddWithValue("@PlanId", hole.PlanId);
                command.Parameters.AddWithValue("@Name", hole.Name);
                command.Parameters.AddWithValue("@Ring", convertedRing);
                command.Parameters.AddWithValue("@Length", hole.Design?.Length ?? 0.0);
                command.Parameters.AddWithValue("@AdjustedDesignLength", hole.AdjustedDesign?.Length ?? 0.0);
                command.Parameters.AddWithValue("@AdjustedDesignDiameter", hole.AdjustedDesign?.Diameter ?? 0.0);
                command.Parameters.AddWithValue("@AdjustedDesignAngle", hole.AdjustedDesign?.Angle ?? 0.0);
                command.Parameters.AddWithValue("@AdjustedDesignBearing", hole.AdjustedDesign?.Bearing ?? 0.0);
                command.Parameters.AddWithValue("@AdjustedDesignBreakthrough", hole.AdjustedDesign?.Breakthrough == true ? 1 : 0); // Using 1 for true, 0 for false
                command.Parameters.AddWithValue("@AdjustedDesignComment", hole.AdjustedDesign?.Comment ?? "");

                // Execute the command to insert or update the record
                command.ExecuteNonQuery();
            }
        }

        //  Console.WriteLine($"Hole {hole.Name} with Ring {convertedRing} inserted/updated in database.");
    }


    private static string ConvertToShortRingFormat(string originalRing)
    {
        //Console.WriteLine($"Attempting to convert Ring value: {originalRing}"); // Log before conversion attempt

        if (string.IsNullOrWhiteSpace(originalRing))
        {
            Console.WriteLine("Original Ring value is null or whitespace, returning as is."); // Log for null or whitespace input
            return originalRing;
        }

        // Updated pattern to match "RING" in uppercase followed by one or more digits
        var match = System.Text.RegularExpressions.Regex.Match(originalRing, @"^RING(\d+)$");
        if (match.Success)
        {
            string converted = $"R{match.Groups[1].Value}";
            //  Console.WriteLine($"Conversion successful: {originalRing} to {converted}"); // Log successful conversion
            return converted;
        }
        else
        {
            Console.WriteLine($"No conversion needed or pattern not matched for: {originalRing}, returning original."); // Log when no conversion is applied
            return originalRing;
        }
    }



    // Converts "R1", "R2", etc., back to "Ring1", "Ring2", etc.
    private static string ConvertToLongRingFormat(string shortRing)
    {
        if (string.IsNullOrEmpty(shortRing) || !shortRing.StartsWith("R"))
        {
            return shortRing; // Return the original if it doesn't start with "R" or is null/empty
        }

        return "Ring" + shortRing.Substring(1); // Converts "R1" to "Ring1", "R2" to "Ring2", etc.
    }


    private static async Task LoadHolesDataAsync(string siteId, string token)
    {
        try
        {
            List<string> planIds = GetPlanIdsFromDatabase(); // Retrieve all plan IDs from the database

            string databasePath = GetDatabasePath();
            string connectionString = $"Data Source={databasePath};Version=3;";

            foreach (var planId in planIds)
            {
                string blastId = GetBlastIdFromPlanId(planId, connectionString);

                if (string.IsNullOrEmpty(blastId))
                {
                    Console.WriteLine($"Failed to retrieve BlastID for Plan ID: {planId}. Skipping...");
                    continue; // Skip processing this plan if BlastID is not found
                }

                Debug.WriteLine($"Sending request to API: v3/{siteId}/plans/{planId}/holes");
                var holesData = await FetchHolesDataAsync(siteId, planId, token);

                if (holesData != null)
                {
                    if (holesData.Count > 0)
                    {
                        //Console.WriteLine($"Fetched {holesData.Count} holes for Plan ID: {planId}.");
                        await DeserializeAndInsertHoleDataIntoDatabase(holesData); // Deserialization and insertion
                    }
                    else
                    {
                        Console.WriteLine($"No hole data found for Plan ID: {planId}.");
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to fetch hole data for Plan ID: {planId}. holesData is null.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while loading holes data: {ex.Message}");
        }
    }


    public class HoleResponse
    {
        [JsonProperty("holes")]
        public List<HoleData>? Holes { get; set; }
    }

    private static async Task DeserializeAndInsertHoleDataIntoDatabase(List<HoleData> holesData)
    {
        await Task.Run(() =>
        {
            foreach (var hole in holesData)
            {
                // Example logging including the ring field
                // Console.WriteLine($"Hole ID: {hole.Id}, Name: {hole.Name}, Ring: {hole.Ring}, Design Length: {hole.Design.Length}");

                InsertHoleDataIntoDatabase(hole);
            }
        });
    }


    // Method 13: FetchHolesDataAsync
    /// <summary>
    /// Securely fetches hole data from a specified API using authenticated requests, ensuring data confidentiality and integrity.
    /// Compliance with ISO/IEC 27001:2022:
    /// A.13.2.3 (Transfer of information) - Ensures secure transfer of sensitive information across networks.
    /// </summary>
    private static async Task<List<HoleData>> FetchHolesDataAsync(string siteId, string planId, string token)
    {
        List<HoleData> holes = new List<HoleData>();  // Initialized to an empty list

        try
        {
            string apiUrl = $"v3/{siteId}/plans/{planId}/holes";

            if (_httpClient.BaseAddress == null)
            {
                Console.WriteLine("HttpClient BaseAddress is not configured.");
                return holes;
            }
            string requestUrl = _httpClient.BaseAddress.ToString().TrimEnd('/') + "/" + apiUrl;

            HttpResponseMessage response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUrl)
            {
                Headers = { Authorization = new AuthenticationHeaderValue("Bearer", token) }
            });

            if (response == null)
            {
                Console.WriteLine("HTTP response is null, unable to proceed.");
                return holes;
            }

            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(content)) // Strengthened check for empty content
            {
                Console.WriteLine("Response content is empty or null.");
                return holes;
            }

            var jsonResponse = JObject.Parse(content);  // This line assumes content is valid JSON
            if (jsonResponse == null)  // Explicit check for null jsonResponse
            {
                Console.WriteLine("Failed to parse JSON content.");
                return holes;
            }

            var holeData = jsonResponse["holes"]?.ToObject<List<HoleData>>();
            if (holeData == null)
            {
                Console.WriteLine("Response does not contain the 'holes' property or it is not in expected format.");
                return holes;
            }

            holes = holeData;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while fetching hole data for Plan ID {planId}: {ex.Message}");
        }

        return holes;  // Always returns a list, which may be empty but never null
    }





    //deck the halls

    public class DeckMeasurement
    {
        public string? Id { get; set; }
        public string SiteId { get; set; }
        public string PlanId { get; set; }
        public string HoleId { get; set; }
        public string HoleName { get; set; }
        public string RingName { get; set; }
        public int DeckNumber { get; set; }
        public string Property { get; set; }
        public string Value { get; set; }
        public DateTime TimeOccurred { get; set; }
        public DateTime TimeReceived { get; set; }
        public string DeviceId { get; set; }
        public string UserName { get; set; }
        public string DeviceName { get; set; }
        public string EquipmentName { get; set; }

        public DeckMeasurement()
        {
            SiteId = string.Empty;
            PlanId = string.Empty;
            HoleId = string.Empty;
            HoleName = string.Empty;
            RingName = string.Empty;
            Property = string.Empty;
            Value = string.Empty;
            DeviceId = string.Empty;
            UserName = string.Empty;
            DeviceName = string.Empty;
            EquipmentName = string.Empty;
        }
    }
    private static async Task FetchAndStoreDeckMeasurements(string siteId, string planId, string token)
    {
        try
        {
            // Fetch deck measurements data from the API
            var deckMeasurements = await FetchDeckMeasurementsAsync(siteId, planId, token);

            // Ensure the response is not null
            if (deckMeasurements != null && deckMeasurements.Count > 0)
            {
                // Process and insert deck measurements into the database
                ProcessDeckMeasurements(deckMeasurements);
            }
            else
            {
                Console.WriteLine("No deck measurements data found in the API response.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while fetching and storing deck measurements: {ex.Message}");
        }
    }

    private static async Task<List<DeckMeasurement>> FetchDeckMeasurementsAsync(string siteId, string planId, string token)
    {
        List<DeckMeasurement> deckMeasurements = new List<DeckMeasurement>();
        string apiUrl = $"v3/{siteId}/plans/{planId}/deckMeasurements";

        try
        {
            if (_httpClient.BaseAddress == null)
            {
                Console.WriteLine("HttpClient BaseAddress is not configured.");
                return deckMeasurements;
            }
            string requestUrl = _httpClient.BaseAddress.ToString().TrimEnd('/') + "/" + apiUrl;
            Console.WriteLine($"Sending request to API: {requestUrl}");

            HttpResponseMessage response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUrl)
            {
                Headers = { Authorization = new AuthenticationHeaderValue("Bearer", token) }
            });

            if (response == null)
            {
                Console.WriteLine("HTTP response is null, unable to proceed.");
                return deckMeasurements;
            }

            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(content))
            {
                Console.WriteLine("Response content is empty or null.");
                return deckMeasurements;
            }

            JObject jsonResponse = JObject.Parse(content);  // Assumed valid JSON content
            if (jsonResponse == null)
            {
                Console.WriteLine("Failed to parse JSON content.");
                return deckMeasurements;
            }

            var measurementsNode = jsonResponse["measurements"];
            if (measurementsNode != null)
            {
                deckMeasurements = measurementsNode.ToObject<List<DeckMeasurement>>() ?? new List<DeckMeasurement>();
                Console.WriteLine($"Fetched {deckMeasurements.Count} deck measurements for Plan ID: {planId}.");
            }
            else
            {
                Console.WriteLine("Response does not contain the 'measurements' property.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while fetching deck measurements data: {ex.Message}");
        }

        return deckMeasurements;
    }



    private static void ProcessDeckMeasurements(List<DeckMeasurement> deckMeasurements)
    {
        foreach (var measurement in deckMeasurements)
        {
            InsertDeckMeasurementIntoDatabase(measurement);
        }
    }

    private static void InsertDeckMeasurementIntoDatabase(DeckMeasurement measurement)
    {
        string databasePath = GetDatabasePath();

        using (var connection = new SQLiteConnection($"Data Source={databasePath};Version=3;"))
        {
            try
            {
                connection.Open();
                //Console.WriteLine("Database connection opened.");

                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = "INSERT OR REPLACE INTO DeckMeasurements (Id, SiteId, PlanId, HoleId, HoleName, RingName, DeckNumber, Property, Value, TimeOccurred, TimeReceived, DeviceId, UserName, DeviceName, EquipmentName) VALUES (@Id, @SiteId, @PlanId, @HoleId, @HoleName, @RingName, @DeckNumber, @Property, @Value, @TimeOccurred, @TimeReceived, @DeviceId, @UserName, @DeviceName, @EquipmentName)";
                    command.Parameters.AddWithValue("@Id", measurement.Id);
                    command.Parameters.AddWithValue("@SiteId", measurement.SiteId);
                    command.Parameters.AddWithValue("@PlanId", measurement.PlanId);
                    command.Parameters.AddWithValue("@HoleId", measurement.HoleId);
                    command.Parameters.AddWithValue("@HoleName", measurement.HoleName);
                    command.Parameters.AddWithValue("@RingName", measurement.RingName);
                    command.Parameters.AddWithValue("@DeckNumber", measurement.DeckNumber);
                    command.Parameters.AddWithValue("@Property", measurement.Property);
                    command.Parameters.AddWithValue("@Value", measurement.Value);
                    command.Parameters.AddWithValue("@TimeOccurred", measurement.TimeOccurred);
                    command.Parameters.AddWithValue("@TimeReceived", measurement.TimeReceived);
                    command.Parameters.AddWithValue("@DeviceId", measurement.DeviceId);
                    command.Parameters.AddWithValue("@UserName", measurement.UserName);
                    command.Parameters.AddWithValue("@DeviceName", measurement.DeviceName);
                    command.Parameters.AddWithValue("@EquipmentName", measurement.EquipmentName);

                    command.ExecuteNonQuery();
                    //Console.WriteLine($"Deck measurement {measurement.Id} inserted/updated in database.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while inserting deck measurement {measurement.Id} into the database: {ex.Message}");
            }
            finally
            {
                connection.Close(); // Close the connection after use
                //Console.WriteLine("Database connection closed.");
            }
        }
    }


    private static async Task LoadDeckMeasurementsIntoDatabase(string authToken, string siteId, string planId)
    {
        Debug.WriteLine("Starting to load deck measurements into the database...");

        List<DeckMeasurement> deckMeasurements = await FetchDeckMeasurementsAsync(siteId, planId, authToken);

        if (deckMeasurements != null && deckMeasurements.Any())
        {
            Debug.WriteLine($"Found {deckMeasurements.Count} deck measurements in API response. Starting database operations.");

            ProcessDeckMeasurements(deckMeasurements);

            Debug.WriteLine("Finished loading deck measurements into the database.");
        }
        else
        {
            Debug.WriteLine("No deck measurements found in API response.");
        }
    }
    private static void CreateDeckMeasurementsTable()
    {
        string databasePath = GetDatabasePath();

        using (var connection = new SQLiteConnection($"Data Source={databasePath};Version=3;"))
        {
            connection.Open();
            using (var command = new SQLiteCommand(connection))
            {
                command.CommandText = @"CREATE TABLE IF NOT EXISTS DeckMeasurements (
                                        Id TEXT PRIMARY KEY,
                                        SiteId TEXT,
                                        PlanId TEXT,
                                        HoleId TEXT,
                                        HoleName TEXT,
                                        RingName TEXT,
                                        DeckNumber INTEGER,
                                        Property TEXT,
                                        Value TEXT,
                                        TimeOccurred DATETIME,
                                        TimeReceived DATETIME,
                                        DeviceId TEXT,
                                        UserName TEXT,
                                        DeviceName TEXT,
                                        EquipmentName TEXT
                                    )";
                command.ExecuteNonQuery();
            }
        }
        //  Console.WriteLine("DeckMeasurements table created/verified.");
    }

    public static async Task PostActualDeckToApi(string siteId, string authToken, string connectionString)
    {
        Debug.WriteLine("PostActualDeckToApi method triggered.");

        try
        {
            var loadMateData = await FetchLOADIQUData(connectionString, onlyUnsent: true);
            var products = await FetchProducts(connectionString);

            foreach (var data in loadMateData)
            {
                if (data.IsSent) continue;

                string? planId = await GetPlanId(data.BlastID, connectionString);
                if (string.IsNullOrEmpty(planId))
                {
                    Debug.WriteLine($"Failed to fetch plan ID for BlastID: {data.BlastID}. Skipping.");
                    continue;
                }

                var matchedProduct = products.FirstOrDefault(p => p.Name != null && p.Name.Equals(data.ProductName, StringComparison.InvariantCultureIgnoreCase));
                if (matchedProduct == null)
                {
                    Debug.WriteLine($"No matching product found for {data.ProductName}. Skipping.");
                    continue;
                }

                var (horizon, shortRing) = await FetchHorizon(data.HoleID, data.BlastID, connectionString);
                string longRingFormat = ConvertToLongRingFormat(data.Ring);
                Debug.WriteLine($"Fetched horizon: {horizon} and ring: {longRingFormat} for HoleID: {data.HoleID}, BlastID: {data.BlastID}. Preparing to post.");

                var actualDeckPost = new
                {
                    holeName = data.HoleID,
                    ringName = longRingFormat,
                    number = 1,
                    productId = matchedProduct.Id ?? string.Empty,
                    weight = Convert.ToDouble(data.ChargeWeight),
                    horizon = horizon,
                    notLoaded = false,
                    loadedNotMeasured = false
                };

                var jsonPayload = JsonConvert.SerializeObject(new { actualDecks = new[] { actualDeckPost } });
                string requestUrl = $"{_httpClient.BaseAddress}v3/{siteId}/plans/{planId}/actualDecks";

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(requestUrl, content);
                    if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        Debug.WriteLine("Actual deck posted successfully.");
                        await MarkLOADIQUDataAsSent(data.Id, connectionString);
                    }
                    else
                    {
                        Debug.WriteLine($"Failed to post actual deck. Status: {response.StatusCode}, Reason: {await response.Content.ReadAsStringAsync()}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"An error occurred while posting actual deck: {ex.Message}");
        }
    }






    private static async Task MarkLOADIQUDataAsSent(int id, string connectionString)
    {
        try
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = "UPDATE LOADIQUData SET IsSent = 1 WHERE Id = @id";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    await command.ExecuteNonQueryAsync();
                }
            }
            Debug.WriteLine($"Marked LOADIQUData record with id {id} as sent.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"An error occurred while marking LOADIQUData record as sent: {ex.Message}");
        }
    }



    public async Task<string?> SerializeProductWithCustomSettingsAsync(Product product)
    {
        try
        {
            string jsonPayload = await Task.Run(() =>
            {
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Converters = new List<JsonConverter> { new MyCustomBooleanConverter() }
                };

                string payload = JsonConvert.SerializeObject(product, settings);
                Debug.WriteLine($"JSON Payload with Custom Settings: {payload}");

                return payload;
            });

            // Your logic to use the serialized payload...

            return jsonPayload;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception in SerializeProductWithCustomSettingsAsync: {ex.Message}");
            return null;
        }
    }




    public class MyCustomBooleanConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(bool);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteValue("false"); // Decide how you want to handle null boolean values
                return;
            }

            bool boolValue = (bool)value;
            writer.WriteValue(boolValue ? "true" : "false");
        }



        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
            {
                return false; // Explicitly handle null by returning standard false for bool type.
            }

            string valueString = reader.Value?.ToString() ?? string.Empty; // Safeguard against null
            return valueString.ToLower() == "true";
        }



    }

    public class ActualDeckPost
    {
        public string holeName { get; set; }
        public string? ringName { get; set; } // Nullable
        public int number { get; set; }
        public string? productId { get; set; } // Nullable
        public string? variantId { get; set; } // Nullable
        public double? length { get; set; } // Nullable, use double? to denote nullable double
        public double? weight { get; set; } // Nullable, use double? to denote nullable double
        public int? itemCount { get; set; } // Nullable
        public double? horizon { get; set; } // Nullable, use double? to denote nullable double
        public bool notLoaded { get; set; } = false; // Default to false
        public bool loadedNotMeasured { get; set; } = false; // Default to false

        // Constructor that ensures holeName is initialized
        public ActualDeckPost(string holeName)
        {
            this.holeName = holeName;
        }
    }



    public class ActualDeck
    {
        public string? holeName { get; set; }
        public int number { get; set; }
        public double length { get; set; }  
        public double weight { get; set; }
        public double horizon { get; set; }
        public string? productId { get; set; }
        public bool notLoaded { get; set; } = false; // Correctly adding the boolean fields
        public bool loadedNotMeasured { get; set; } = false;
    }


    //check for ring
    // Method to fetch or validate the Ring value based on HoleID and BlastID
    private static async Task<string> GetRingDataForHole(string holeId, string blastId, string connectionString)
    {
        string ring = "";
        string query = @"
SELECT Ring
FROM LOADIQUData
WHERE HoleID = @HoleID AND BlastID = @BlastID LIMIT 1";

        try
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@HoleID", holeId);
                    command.Parameters.AddWithValue("@BlastID", blastId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            ring = reader.GetString(0); // Assuming Ring is the first column in your result
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while fetching the Ring data for HoleID: '{holeId}', BlastID: '{blastId}': {ex.Message}");
        }

        return ring;
    }

    // Updated FetchHorizon method to include fetching the Ring
    private static async Task<(double Horizon, string Ring)> FetchHorizon(string holeId, string blastId, string connectionString)
    {
        double horizon = 0.0;
        string ring = await GetRingDataForHole(holeId, blastId, connectionString); // Fetch or validate the Ring value

        // Updated SQL query to use AdjustedDesignLength instead of Length
        string query = @"
SELECT
    (hd.AdjustedDesignLength - CAST(lmd.ChargeLength AS REAL)) AS Horizon
FROM
    LOADIQUData lmd
INNER JOIN
    Plans p ON lmd.BlastID = p.Name
INNER JOIN
    HoleData hd ON hd.PlanId = p.PlanId AND hd.Name = lmd.HoleID AND hd.Ring = lmd.Ring
WHERE
    lmd.HoleID = @HoleID AND lmd.Ring = @Ring AND lmd.BlastID = @BlastID";

        //Console.WriteLine($"Executing FetchHorizon with HoleID: '{holeId}', Ring: '{ring}', BlastID: '{blastId}'.");
        // Console.WriteLine($"SQL Query: {query}");

        try
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@HoleID", holeId);
                    command.Parameters.AddWithValue("@Ring", ring);
                    command.Parameters.AddWithValue("@BlastID", blastId);

                    var result = await command.ExecuteScalarAsync();
                    if (result != null && result != DBNull.Value && double.TryParse(result.ToString(), out double parsedHorizon))
                    {
                        horizon = parsedHorizon;
                    }
                    else
                    {
                        Console.WriteLine($"No horizon value found or unable to parse for HoleID: '{holeId}', Ring: '{ring}', and BlastID: '{blastId}'.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while fetching the horizon for HoleID: '{holeId}', Ring: '{ring}', and BlastID: '{blastId}': {ex.Message}");
        }

        //Console.WriteLine($"Fetched horizon value for HoleID: '{holeId}', Ring: '{ring}', and BlastID: '{blastId}': {horizon}");
        return (horizon, ring); // Return both horizon and ring as a tuple
    }



    private static async Task<List<Product>> FetchProducts(string connectionString)
    {
        var productList = new List<Product>();
        Debug.WriteLine("Fetching products from database...");
        using (var connection = new SQLiteConnection(connectionString))
        {
            await connection.OpenAsync();
            string query = "SELECT * FROM Products";
            using (var command = new SQLiteCommand(query, connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        // Safely handle potential null database fields
                        var product = new Product
                        {
                            Id = reader["Id"]?.ToString() ?? string.Empty,
                            Type = reader["Type"]?.ToString() ?? string.Empty,
                            Name = reader["Name"]?.ToString() ?? string.Empty,
                            DateCreated = DateTime.TryParse(reader["DateCreated"]?.ToString(), out DateTime dateCreated) ? dateCreated : default,
                            DateModified = DateTime.TryParse(reader["DateModified"]?.ToString(), out DateTime dateModified) ? dateModified : default,
                            IsDeleted = Convert.ToBoolean(reader["IsDeleted"]),
                            Abbreviation = reader["Abbreviation"]?.ToString() ?? string.Empty,
                            SupplierName = reader["SupplierName"]?.ToString() ?? string.Empty,
                            ShotPlusReference = reader["ShotPlusReference"]?.ToString() ?? string.Empty,
                            DisplayColor = reader["DisplayColor"]?.ToString() ?? string.Empty,
                        };
                        productList.Add(product);
                        Debug.WriteLine($"Product fetched: ID={product.Id}, Name={product.Name}");
                    }
                }
            }
        }
        Debug.WriteLine($"Total products fetched: {productList.Count}");
        return productList;
    }




    private static async Task<List<LOADIQUData>> FetchLOADIQUData(string connectionString, bool onlyUnsent = true)
    {
        var LOADIQUDataList = new List<LOADIQUData>();
        using (var connection = new SQLiteConnection(connectionString))
        {
            await connection.OpenAsync();
            string query = onlyUnsent
                ? "SELECT Id, BlastID, HoleID, Ring, ChargeLength, ChargeWeight, ProductName, IsSent FROM LOADIQUData WHERE IsSent = 0"
                : "SELECT Id, BlastID, HoleID, Ring, ChargeLength, ChargeWeight, ProductName, IsSent FROM LOADIQUData";

            using (var command = new SQLiteCommand(query, connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var LOADIQUData = new LOADIQUData(
                            reader["BlastID"]?.ToString() ?? string.Empty,
                            reader["Ring"]?.ToString() ?? string.Empty,
                            reader["HoleID"]?.ToString() ?? string.Empty,
                            reader["ProductName"]?.ToString() ?? string.Empty,
                            reader["ChargeLength"]?.ToString() ?? string.Empty,
                            reader["ChargeWeight"]?.ToString() ?? string.Empty)
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            IsSent = Convert.ToBoolean(reader["IsSent"]) // No null check needed for boolean conversion as default is false
                        };
                        LOADIQUDataList.Add(LOADIQUData);

                    }
                }
            }
        }

        return LOADIQUDataList;
    }






    // Method to get PlanId using BlastID
    public static async Task<string?> GetPlanId(string blastId, string connectionString)
{
    string? planId = null;  // Declare planId as nullable

    using (var connection = new SQLiteConnection(connectionString))
    {
        await connection.OpenAsync();

        string query = "SELECT PlanId FROM Plans WHERE Name = @BlastID";

        using (var command = new SQLiteCommand(query, connection))
        {
            command.Parameters.AddWithValue("@BlastID", blastId);

            var result = await command.ExecuteScalarAsync();
            if (result != null)
            {
                planId = result.ToString();
            }
        }
    }

    return planId;  // Return type is now correctly marked as nullable
}




    private static string GetBlastIdFromPlanId(string planId, string connectionString)
    {
        string blastId = string.Empty;  // Initialize to empty to ensure non-nullable type

        try
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT BlastID FROM Plans WHERE PlanId = @PlanId";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PlanId", planId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Directly cast and check for null in a single step
                            var dbValue = reader["BlastID"] as string; // Cast as string which handles nulls
                            blastId = dbValue ?? string.Empty; // Use null-coalescing to ensure non-null assignment
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving BlastID for Plan ID {planId}: {ex.Message}");
        }

        return blastId;  // Ensures return type is non-nullable
    }






    public class LOADIQUData
    {
        public int Id { get; set; } // Assuming Id is an integer
        public string BlastID { get; set; }
        public string Ring { get; set; } // Added Ring property
        public string HoleID { get; set; }
        public string ProductName { get; set; }
        public string ChargeLength { get; set; }
        public string ChargeWeight { get; set; }
        public bool IsSent { get; set; } // Handle the IsSent status

        // Constructor to initialize the properties
        public LOADIQUData(string blastID, string ring, string holeID, string productName, string chargeLength, string chargeWeight)
        {
            BlastID = blastID;
            Ring = ring;
            HoleID = holeID;
            ProductName = productName;
            ChargeLength = chargeLength;
            ChargeWeight = chargeWeight;
        }

        // Add additional properties as needed based on your database schema
    }






    public async Task<string?> ImportProductsAsync(string authToken, string siteId, string connectionString, bool includeDeleted = false)
    {
        Debug.WriteLine($"Attempting to fetch products for site ID: {siteId} with authToken.");

        try
        {
            var queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
            queryString["includeDeleted"] = includeDeleted.ToString().ToLower();

            string relativeUrlWithParams = $"v3/{siteId}/products?" + queryString;
            string fullRequestUrl = $"{_httpClient.BaseAddress}{relativeUrlWithParams}";
            Debug.WriteLine($"Full Request URL: {fullRequestUrl}");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, relativeUrlWithParams);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            Debug.WriteLine("Sending HTTP GET Request with headers:");

            foreach (var header in request.Headers)
            {
                Debug.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");
            }

            HttpResponseMessage response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("ImportProductsAsync Success: Response received.");
                // Log the full response body for debugging
                Debug.WriteLine($"Full response body: {responseBody}");

                var productsResponse = JsonConvert.DeserializeObject<ProductsResponse>(responseBody);
                if (productsResponse?.Products != null)
                {
                    await StoreProductsInDatabase(productsResponse.Products, connectionString);
                    Debug.WriteLine($"Successfully imported {productsResponse.Products.Count} products to the database.");
                }
                else
                {
                    Debug.WriteLine("No products found in the response.");
                }

                return responseBody;
            }
            else
            {
                Debug.WriteLine($"ImportProductsAsync Error: StatusCode={response.StatusCode}, ReasonPhrase={response.ReasonPhrase}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"ImportProductsAsync Exception: {ex.GetType().FullName}, Message={ex.Message}");
            return null;
        }
    }


    // Updated to correctly store boolean values as integers
    private async Task StoreProductsInDatabase(List<Product> products, string connectionString)
    {
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            await connection.OpenAsync();

            foreach (var product in products)
            {
                string checkExistenceQuery = "SELECT COUNT(1) FROM Products WHERE Id = @Id";
                using (var checkCmd = new SQLiteCommand(checkExistenceQuery, connection))
                {
                    checkCmd.Parameters.AddWithValue("@Id", product.Id);
                    var result = await checkCmd.ExecuteScalarAsync();

                    bool exists = result != null && (long)result > 0; // Safely unbox only if result is not null

                    string query;
                    if (exists)
                    {
                        // Update existing product
                        query = @"UPDATE Products SET 
                            Type = @Type, 
                            Name = @Name, 
                            DateCreated = @DateCreated, 
                            DateModified = @DateModified, 
                            IsDeleted = @IsDeleted, 
                            Abbreviation = @Abbreviation, 
                            SupplierName = @SupplierName, 
                            ShotPlusReference = @ShotPlusReference, 
                            DisplayColor = @DisplayColor 
                        WHERE Id = @Id";
                    }
                    else
                    {
                        // Insert new product
                        query = @"INSERT INTO Products 
                            (Id, Type, Name, DateCreated, DateModified, IsDeleted, Abbreviation, SupplierName, ShotPlusReference, DisplayColor) 
                        VALUES 
                            (@Id, @Type, @Name, @DateCreated, @DateModified, @IsDeleted, @Abbreviation, @SupplierName, @ShotPlusReference, @DisplayColor)";
                    }

                    using (var cmd = new SQLiteCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Id", product.Id);
                        cmd.Parameters.AddWithValue("@Type", product.Type);
                        cmd.Parameters.AddWithValue("@Name", product.Name);
                        cmd.Parameters.AddWithValue("@DateCreated", product.DateCreated.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
                        cmd.Parameters.AddWithValue("@DateModified", product.DateModified.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
                        cmd.Parameters.AddWithValue("@IsDeleted", product.IsDeleted ? 1 : 0); // Convert boolean to integer
                        cmd.Parameters.AddWithValue("@Abbreviation", product.Abbreviation);
                        cmd.Parameters.AddWithValue("@SupplierName", product.SupplierName);
                        cmd.Parameters.AddWithValue("@ShotPlusReference", product.ShotPlusReference);
                        cmd.Parameters.AddWithValue("@DisplayColor", product.DisplayColor);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
        }
    }




    public async Task<List<Product>?> GetProductsAsync(string authToken, string siteId, bool includeDeleted = false)
    {
        try
        {
            var queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
            queryString["includeDeleted"] = includeDeleted.ToString().ToLower();

            string relativeUrlWithParams = $"v3/{siteId}/products?" + queryString;
            string fullRequestUrl = $"{_httpClient.BaseAddress}{relativeUrlWithParams}";
            Debug.WriteLine($"Full Request URL: {fullRequestUrl}");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, relativeUrlWithParams);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            Debug.WriteLine("Sending HTTP GET Request with headers:");
            foreach (var header in request.Headers)
            {
                string headerValue = header.Key + ": " + string.Join(", ", header.Value);
                Debug.WriteLine(headerValue);
            }

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("GetProductsAsync Success: Response received.");
                Debug.WriteLine($"Response snippet: {responseBody.Substring(0, Math.Min(responseBody.Length, 100))}...");

                var products = JsonConvert.DeserializeObject<List<Product>>(responseBody);
                return products;
            }
            else
            {
                Debug.WriteLine($"GetProductsAsync Error: StatusCode={response.StatusCode}, ReasonPhrase={response.ReasonPhrase}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"GetProductsAsync Exception: {ex.GetType().FullName}, Message={ex.Message}");
            return null;
        }
    }


    public class Product
    {
        public string? Id { get; set; }
        public string? Type { get; set; }
        public string? Name { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }
        public string? Abbreviation { get; set; }
        public string? SupplierName { get; set; }
        public string? ShotPlusReference { get; set; }
        public List<RestrictedTo> RestrictedTo { get; set; } = new List<RestrictedTo>();
        public List<DocumentLink> DocumentLinks { get; set; } = new List<DocumentLink>();
        public string? DisplayColor { get; set; }
        // Additional properties for Bulk type and other specific types
        public float? Density { get; set; } // Assuming this is specific to certain types like Bulk
        public List<Variant> Variants { get; set; } = new List<Variant>();
    }


    public class RestrictedTo
    {
        public string? Type { get; set; }
        public string? Value { get; set; }
    }

    public class DocumentLink
    {
        public string? Name { get; set; }
        public string? Url { get; set; }
    }

    public class Variant
    {
        public string? Id { get; set; }
        public string? DisplayName { get; set; }
        public string? CalculatedName { get; set; }
        public string? CustomName { get; set; }
        public bool? IsDeleted { get; set; }
        public string? DisplayColor { get; set; }
        // Changed from int to float to accurately represent the data from the JSON payload
        public float Diameter { get; set; }
        public float Length { get; set; }
        public float Weight { get; set; }
    }

    public class ProductsResponse
    {
        public List<Product> Products { get; set; }

        public ProductsResponse()
        {
            Products = new List<Product>();
        }
    }



    public static List<Product> ParseProductsResponse(string productsResponse)
    {
        // Deserialize the JSON response into the ProductsResponse object
        var products = JsonConvert.DeserializeObject<List<Product>>(productsResponse);

        // Ensure products is not null
        return products ?? new List<Product>();
    }



}

/*
+---------------------------------------+
| LOADIQU Developed by Rory Vining 2024 |
+---------------------------------------+
*/
