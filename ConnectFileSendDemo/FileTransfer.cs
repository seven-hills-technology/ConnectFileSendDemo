using System.Net.Http.Headers;

namespace ConnectFileSendDemo;

public class SendFileOptions
{
    public bool createCase { get; set; }
    public string caseTitle { get; set; }
    public string caseDetails { get; set; }
    public string casePriority { get; set; }
}

public class FileTransfer
{
    private string _filePath { get; set; }
    private string _baseUrl { get; set; }
    private string _authUrl { get; set; }
    private string _appUploadUrl { get; set; }
    private HttpClientHandler _clientHandler { get; set; }
    private HttpClient _client { get; set; }

    public FileTransfer()
    {
        _filePath = "C:/Users/SHT VM/projects/ConnectFileSendDemo/test-file.txt"; //! CHANGE ME
        _baseUrl = "https://sigmanest-connect.sht.dev";
        _authUrl = $"{_baseUrl}/Account/AppLogin"; //! CHANGE ME
        _appUploadUrl = $"{_baseUrl}/Files/AppUpload"; //! CHANGE ME
        _clientHandler = new HttpClientHandler();
        _client = new HttpClient(_clientHandler);
    }
    
    public async Task Authenticate(string email, string password)
    {
        try
        {
            using var form = new MultipartFormDataContent();
            form.Add(new StringContent(email), "email");
            form.Add(new StringContent(password), "password");

            var response = await _client.PostAsync(_authUrl, form);

            try
            {
                var setCookieHeader = response.Headers.First(x => x.Key == "Set-Cookie");

                if (string.IsNullOrEmpty(setCookieHeader.Value.First()))
                    throw new Exception("No Cookie Value on Response");
            }
            catch (Exception e)
            {
                Console.WriteLine("No Set-Cookie Header found on response");
                Console.WriteLine(e);
                throw;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    
    public async Task SendFile(SendFileOptions options)
    {
        try
        {
            var fileContents = new ByteArrayContent(await File.ReadAllBytesAsync(_filePath));
            
            fileContents.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

            using var form = new MultipartFormDataContent();
            form.Add(fileContents, "files", Path.GetFileName(_filePath));
            if (options.createCase)
            {
                form.Add(new StringContent("True"), "createCase");
                form.Add(new StringContent(options.caseTitle), "caseTitle");
                form.Add(new StringContent(options.caseDetails), "caseDetails");
                form.Add(new StringContent(options.casePriority), "casePriority");
            }
            else
            {
                form.Add(new StringContent("False"), "createCase");
            }

            await _client.PostAsync(_appUploadUrl, form);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}