namespace Osirion.Blazor.Core.Configuration;

/// <summary>
/// Email configuration options
/// </summary>
public class EmailOptions
{
    /// <summary>
    /// Configuration section name
    /// </summary>
    public const string Section = "Osirion:Email";

    /// <summary>
    /// Email provider type
    /// </summary>
    public EmailProviderType Provider { get; set; } = EmailProviderType.Smtp;

    /// <summary>
    /// SMTP configuration
    /// </summary>
    public SmtpConfiguration Smtp { get; set; } = new();

    /// <summary>
    /// SendGrid configuration
    /// </summary>
    public SendGridConfiguration SendGrid { get; set; } = new();

    /// <summary>
    /// Infobip configuration
    /// </summary>
    public InfobipConfiguration Infobip { get; set; } = new();

    /// <summary>
    /// Default recipient email address
    /// </summary>
    public string ToEmail { get; set; } = string.Empty;

    /// <summary>
    /// Default sender name
    /// </summary>
    public string FromName { get; set; } = "Contact Form";

    /// <summary>
    /// Email subject template (can include {subject} placeholder)
    /// </summary>
    public string SubjectTemplate { get; set; } = "Contact Form: {subject}";
}

/// <summary>
/// Email provider types
/// </summary>
public enum EmailProviderType
{
    /// <summary>
    /// SMTP provider
    /// </summary>
    Smtp,
    
    /// <summary>
    /// SendGrid provider
    /// </summary>
    SendGrid,
    
    /// <summary>
    /// Infobip provider
    /// </summary>
    Infobip
}

/// <summary>
/// SMTP configuration
/// </summary>
public class SmtpConfiguration
{
    /// <summary>
    /// SMTP server host
    /// </summary>
    public string Host { get; set; } = string.Empty;

    /// <summary>
    /// SMTP server port
    /// </summary>
    public int Port { get; set; } = 587;

    /// <summary>
    /// Enable SSL/TLS
    /// </summary>
    public bool EnableSsl { get; set; } = true;

    /// <summary>
    /// SMTP username
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// SMTP password
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// From email address
    /// </summary>
    public string FromEmail { get; set; } = string.Empty;
}

/// <summary>
/// SendGrid configuration
/// </summary>
public class SendGridConfiguration
{
    /// <summary>
    /// SendGrid API key
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// From email address
    /// </summary>
    public string FromEmail { get; set; } = string.Empty;

    /// <summary>
    /// Template ID for SendGrid templates (optional)
    /// </summary>
    public string? TemplateId { get; set; }
}

/// <summary>
/// Infobip configuration
/// </summary>
public class InfobipConfiguration
{
    /// <summary>
    /// Infobip base URL
    /// </summary>
    public string BaseUrl { get; set; } = "https://api.infobip.com";

    /// <summary>
    /// Infobip API key
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// From email address
    /// </summary>
    public string FromEmail { get; set; } = string.Empty;
}