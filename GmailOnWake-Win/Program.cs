using System.Configuration;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

class Program
{
    static bool EnableConsoleLog => bool.TryParse(ConfigurationManager.AppSettings["EnableConsoleLog"], out var val) && val;
    static bool EnableWakeEmail => bool.TryParse(ConfigurationManager.AppSettings["EnableWakeEmail"], out var val) && val;
    static string GmailAddress => ConfigurationManager.AppSettings["GmailAddress"];
    static string GmailAppPassword => ConfigurationManager.AppSettings["GmailAppPassword"];
    static string LogFilePath => ConfigurationManager.AppSettings["LogFilePath"] ?? @"C:\system files\logs\waketimes.txt";
    static string WakeSubjectPrefix => ConfigurationManager.AppSettings["WakeSubjectPrefix"] ?? "PC Wake";
    
    static int MaxRetryAttempts
    {
        get
        {
            var raw = ConfigurationManager.AppSettings["MaxRetryAttempts"];
            return int.TryParse(raw, out var val) && val > 0 ? val : 3; // Default to 3
        }
    }
    
    static int DelayBeforeFirstSendMs
    {
        get
        {
            var raw = ConfigurationManager.AppSettings["DelayBeforeFirstSendMs"];
            return int.TryParse(raw, out var val) && val >= 0 ? val : 15000; // Default to 15s
        }
    }

    static void Log(string message)
    {
        if (EnableConsoleLog)
        {
            if (!File.Exists(LogFilePath))
            {
                string? dir = Path.GetDirectoryName(LogFilePath);
                if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                File.Create(LogFilePath).Close();
            }
        
            try
            {
                File.AppendAllText(LogFilePath, message + Environment.NewLine);
            }
            catch (Exception logEx)
            {
                Console.WriteLine($"Logging failed: {logEx.Message}");
            }
        }
    }

    static void Main()
    {
        // Validate config
        if (string.IsNullOrWhiteSpace(GmailAddress) ||
            string.IsNullOrWhiteSpace(GmailAppPassword) ||
            ConfigurationManager.AppSettings["EnableConsoleLog"] is null ||
            ConfigurationManager.AppSettings["EnableWakeEmail"] is null)
        {
            Log("Missing required config values. Exiting.");
            return;
        }

        // Global disable switch
        if (!EnableWakeEmail)
        {
            Log("WakeEmail disabled by config.");
            return;
        }

        // Wait some time incase WiFi is still connecting
        Thread.Sleep(DelayBeforeFirstSendMs);

        Console.WriteLine("Hi");
        
        // Format subject string
        string formattedTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        string subject = $"{WakeSubjectPrefix} - {formattedTime}";
        
        // Construct email message
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Wake Alert", GmailAddress));
        message.To.Add(new MailboxAddress("You", GmailAddress));
        message.Subject = subject;
        message.Body = new TextPart("plain") { Text = "" };

        // Send with 3 retry attempts
        int attempts = 0;
        bool sent = false;
        while (attempts < MaxRetryAttempts && !sent)
        {
            try
            {
                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                    client.Authenticate(GmailAddress, GmailAppPassword);
                    client.Send(message);
                    client.Disconnect(true);
                }
                sent = true;
            }
            catch (Exception ex)
            {
                attempts++;
                Log($"Email send attempt {attempts} failed: {ex.Message}");
                Thread.Sleep(5000);
            }
        }

        // Log to file
        Log(subject);
    }
}
