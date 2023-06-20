using ConnectFileSendDemo;

var useHardcodedCredentials = false;
string email;
string password;

if (useHardcodedCredentials)
{
    email = "";
    password = "";
}
else
{
    Console.Write("Enter Connect Email: ");
    email = Console.ReadLine() ?? "";

    password = "";
    Console.Write("Enter Connect Password: ");
    ConsoleKeyInfo keyInfo;
     
    do
    {
        keyInfo = Console.ReadKey(true);
        // Skip if Backspace or Enter is Pressed
        if (keyInfo.Key != ConsoleKey.Backspace && keyInfo.Key != ConsoleKey.Enter)
        {
            password += keyInfo.KeyChar;
            Console.Write("*");
        }
        else
        {
            if (keyInfo.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                // Remove last character if Backspace is Pressed
                password = password.Substring(0, (password.Length - 1));
                Console.Write("\b \b");
            }
        }
    }
    // Stops Getting Password Once Enter is Pressed
    while (keyInfo.Key != ConsoleKey.Enter);
}

Console.WriteLine();
Console.WriteLine("---------------------------");
Console.WriteLine("Welcome " + email);

Console.Write("Create a Case with File (y/n): ");
var createCaseYn = Console.ReadLine() ?? "";
var caseTitle = "";
var caseDetails = "";
var casePriority = "";
var createCase = string.Equals(createCaseYn, "y", StringComparison.InvariantCultureIgnoreCase);
if (createCase)
{
    do
    {
        Console.Write("Enter Case Title: ");
        caseTitle = Console.ReadLine() ?? "";
        if (string.IsNullOrEmpty(caseTitle))
            Console.WriteLine("Sorry, please enter a title for the case.");
    } while (string.IsNullOrEmpty(caseTitle));
    
    do
    {
        Console.Write("Enter Case Details: ");
        caseDetails = Console.ReadLine() ?? "";
        if (string.IsNullOrEmpty(caseDetails))
            Console.WriteLine("Sorry, please enter some details for the case.");
    } while (string.IsNullOrEmpty(caseDetails));

    var priorityOptions = new List<string> {"1", "2", "3", "4"};
    do
    {
        Console.Write("Enter Case Priority (1-4): ");
        casePriority = Console.ReadLine() ?? "";
        if (string.IsNullOrEmpty(casePriority))
            Console.WriteLine("Sorry, please enter a priority for the case.");
        if (!priorityOptions.Contains(casePriority))
            Console.WriteLine("Sorry, please enter a priority value of 1 thru 4.");
    } while (string.IsNullOrEmpty(casePriority) || !priorityOptions.Contains(casePriority));
}

var ft = new FileTransfer();

Console.WriteLine("");
Console.WriteLine("Beginning to Upload Test File...");

await ft.Authenticate(email!, password);

if (createCase)
{
    await ft.SendFile(new SendFileOptions
    {
        createCase = true,
        caseTitle = caseTitle,
        caseDetails = caseDetails,
        casePriority = casePriority
    });
}
else
{
    await ft.SendFile(new SendFileOptions());
}


Console.WriteLine("Finished Uploading Test File");