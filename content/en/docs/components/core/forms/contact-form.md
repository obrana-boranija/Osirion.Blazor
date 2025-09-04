---
id: 'osirion-contact-form'
order: 1
layout: docs
title: OsirionContactForm Component
permalink: /docs/components/core/forms/contact-form
description: Learn how to implement the OsirionContactForm component for comprehensive contact forms with validation, accessibility, and email integration.
author: Dejan Demonjić
date: 2025-09-03
featured_image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
categories:
- Components
- Core Components
- Forms
tags:
- blazor
- contact-form
- forms
- validation
- email-integration
- accessibility
- user-input
is_featured: true
published: true
slug: contact-form
lang: en
custom_fields: {}
seo_properties:
  title: 'OsirionContactForm Component - Contact Forms with Validation | Osirion.Blazor'
  description: 'Create comprehensive contact forms with the OsirionContactForm component. Features validation, email integration, and full accessibility support.'
  image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
  canonical: 'https://getosirion.com/docs/components/core/forms/contact-form'
  lang: en
  robots: index, follow
  og_title: 'OsirionContactForm Component - Osirion.Blazor'
  og_description: 'Comprehensive contact forms with validation, email integration, and accessibility support.'
  og_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'OsirionContactForm Component - Osirion.Blazor'
  twitter_description: 'Create comprehensive contact forms with validation and email integration.'
  twitter_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  meta_tags: {}
---

# OsirionContactForm Component

The OsirionContactForm component provides a comprehensive contact form solution with built-in validation, email integration, accessibility features, and customizable styling. It includes optional contact information display and follows modern form design principles for optimal user experience.

## Component Overview

OsirionContactForm combines form functionality with contact information display, creating a complete contact solution for websites and applications. It features robust validation, email sending capabilities, loading states, success/error handling, and comprehensive accessibility support.

### Key Features

**Form Validation**: Built-in validation with custom error messages and real-time feedback
**Email Integration**: Direct email sending with configurable SMTP settings
**Contact Info Display**: Optional contact information section with social links
**Accessibility Compliant**: Full ARIA support, keyboard navigation, and screen reader compatibility
**Responsive Design**: Mobile-first design that works across all device sizes
**Loading States**: Visual feedback during form submission and processing
**Success/Error Handling**: Comprehensive feedback for form submission results
**Framework Agnostic**: Compatible with Bootstrap, Tailwind CSS, and custom styling

## Parameters

### Form Configuration

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Id` | `string` | `"contact-form"` | **Required.** Unique identifier for the contact form. |
| `FormTitle` | `string` | `"Write us"` | Title displayed above the contact form. |
| `FormDescription` | `string` | `""` | Description text displayed below the form title. |
| `SubmitButtonText` | `string` | `"Send Message"` | Text displayed on the submit button. |
| `SuccessMessage` | `string` | `"Thank you! Your message has been sent successfully."` | Message shown after successful form submission. |
| `ErrorMessage` | `string` | `"Sorry, there was an error sending your message. Please try again."` | Message shown when form submission fails. |

### Form Field Configuration

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `NamePlaceholder` | `string` | `"Your name"` | Placeholder text for the name field. |
| `EmailPlaceholder` | `string` | `"Your email address"` | Placeholder text for the email field. |
| `SubjectPlaceholder` | `string` | `"Subject"` | Placeholder text for the subject field. |
| `MessagePlaceholder` | `string` | `"Your message"` | Placeholder text for the message field. |
| `RequireSubject` | `bool` | `true` | Whether the subject field is required. |
| `MinMessageLength` | `int` | `10` | Minimum required length for the message field. |
| `MaxMessageLength` | `int` | `1000` | Maximum allowed length for the message field. |

### Contact Information Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ShowContactInfo` | `bool` | `false` | Whether to display the contact information section. |
| `ContactInfoTitle` | `string` | `"Get in Touch"` | Title for the contact information section. |
| `ContactInfoDescription` | `string` | `""` | Description for the contact information section. |
| `ContactEmail` | `string?` | `null` | Contact email address to display. |
| `ContactPhone` | `string?` | `null` | Contact phone number to display. |
| `ContactAddress` | `string?` | `null` | Contact address to display. |
| `SocialLinks` | `List<SocialLink>?` | `null` | Collection of social media links to display. |

### Email Configuration

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ToEmail` | `string?` | `null` | Email address where form submissions will be sent. |
| `FromEmail` | `string?` | `null` | Email address used as the sender. |
| `EmailSubjectPrefix` | `string` | `"Contact Form:"` | Prefix added to email subjects. |
| `SendEmailOnSubmit` | `bool` | `true` | Whether to send email when form is submitted. |

### Callback Events

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `OnFormSubmitted` | `EventCallback<ContactFormModel>` | - | Callback invoked when form is successfully submitted. |
| `OnFormError` | `EventCallback<string>` | - | Callback invoked when form submission encounters an error. |
| `OnValidationFailed` | `EventCallback<IEnumerable<string>>` | - | Callback invoked when form validation fails. |

## Basic Usage

### Simple Contact Form

```razor
@using Osirion.Blazor.Components

<OsirionContactForm 
    Id="main-contact-form"
    FormTitle="Contact Us"
    FormDescription="We'd love to hear from you. Send us a message and we'll respond as soon as possible."
    ToEmail="contact@yourcompany.com"
    OnFormSubmitted="HandleFormSubmission" />

@code {
    private async Task HandleFormSubmission(ContactFormModel model)
    {
        // Handle successful form submission
        Console.WriteLine($"Received message from {model.Name} ({model.Email})");
        
        // Optional: Log to analytics, save to database, etc.
        await Analytics.TrackEvent("contact_form_submitted", new
        {
            name = model.Name,
            email = model.Email,
            subject = model.Subject
        });
    }
}
```

### Contact Form with Contact Information

```razor
<OsirionContactForm 
    Id="contact-with-info"
    FormTitle="Let's Connect"
    FormDescription="Get in touch with our team for any questions or support needs."
    ToEmail="support@yourcompany.com"
    ShowContactInfo="true"
    ContactInfoTitle="Reach Us Directly"
    ContactInfoDescription="Prefer to contact us directly? Here are our contact details."
    ContactEmail="hello@yourcompany.com"
    ContactPhone="+1 (555) 123-4567"
    ContactAddress="123 Business Ave, Suite 100, City, State 12345"
    SocialLinks="@socialMediaLinks"
    OnFormSubmitted="HandleContactSubmission"
    OnFormError="HandleContactError" />

@code {
    private List<SocialLink> socialMediaLinks = new()
    {
        new SocialLink
        {
            Platform = SocialPlatform.Twitter,
            Url = "https://twitter.com/yourcompany",
            DisplayText = "@yourcompany"
        },
        new SocialLink
        {
            Platform = SocialPlatform.LinkedIn,
            Url = "https://linkedin.com/company/yourcompany",
            DisplayText = "Company LinkedIn"
        },
        new SocialLink
        {
            Platform = SocialPlatform.GitHub,
            Url = "https://github.com/yourcompany",
            DisplayText = "GitHub Profile"
        }
    };
    
    private async Task HandleContactSubmission(ContactFormModel model)
    {
        // Success handling
        await NotificationService.ShowSuccessAsync($"Thank you {model.Name}! We'll get back to you soon.");
    }
    
    private async Task HandleContactError(string error)
    {
        // Error handling
        await NotificationService.ShowErrorAsync($"Failed to send message: {error}");
        Logger.LogError("Contact form error: {Error}", error);
    }
}
```

### Customized Contact Form

```razor
<OsirionContactForm 
    Id="custom-contact-form"
    FormTitle="Drop Us a Line"
    FormDescription="Have a project in mind? Let's discuss how we can help bring your vision to life."
    NamePlaceholder="Full Name"
    EmailPlaceholder="Email Address"
    SubjectPlaceholder="Project Type"
    MessagePlaceholder="Tell us about your project..."
    SubmitButtonText="Start Conversation"
    SuccessMessage="Excellent! We've received your project inquiry and will contact you within 24 hours."
    ErrorMessage="Oops! Something went wrong. Please try again or email us directly at hello@agency.com"
    ToEmail="projects@agency.com"
    FromEmail="website@agency.com"
    EmailSubjectPrefix="New Project Inquiry:"
    RequireSubject="true"
    MinMessageLength="25"
    MaxMessageLength="2000"
    OnFormSubmitted="HandleProjectInquiry"
    OnValidationFailed="HandleValidationErrors" />

@code {
    private async Task HandleProjectInquiry(ContactFormModel model)
    {
        // Custom handling for project inquiries
        await ProjectService.CreateLeadAsync(new ProjectLead
        {
            Name = model.Name,
            Email = model.Email,
            ProjectType = model.Subject,
            Description = model.Message,
            Source = "Website Contact Form",
            CreatedAt = DateTime.UtcNow
        });
        
        // Send notification to sales team
        await NotificationService.NotifySalesTeamAsync(model);
    }
    
    private async Task HandleValidationErrors(IEnumerable<string> errors)
    {
        var errorMessage = string.Join(", ", errors);
        await NotificationService.ShowWarningAsync($"Please fix the following: {errorMessage}");
    }
}
```

## Advanced Usage

### Multi-Step Contact Form

```razor
<div class="multi-step-contact">
    <div class="step-indicator">
        <div class="step @(currentStep >= 1 ? "active" : "")">1. Contact Info</div>
        <div class="step @(currentStep >= 2 ? "active" : "")">2. Project Details</div>
        <div class="step @(currentStep >= 3 ? "active" : "")">3. Confirmation</div>
    </div>
    
    @if (currentStep == 1)
    {
        <OsirionContactForm 
            Id="step-1-contact"
            FormTitle="Step 1: Your Information"
            FormDescription="Let's start with your contact details."
            SubmitButtonText="Next: Project Details"
            OnFormSubmitted="HandleStep1" />
    }
    else if (currentStep == 2)
    {
        <OsirionContactForm 
            Id="step-2-project"
            FormTitle="Step 2: Project Details"
            FormDescription="Tell us about your project requirements."
            SubjectPlaceholder="Project Category"
            MessagePlaceholder="Describe your project in detail..."
            SubmitButtonText="Review & Submit"
            MinMessageLength="50"
            OnFormSubmitted="HandleStep2" />
    }
    else if (currentStep == 3)
    {
        <div class="confirmation-step">
            <h3>Step 3: Confirmation</h3>
            <div class="summary">
                <h4>Contact Information</h4>
                <p><strong>Name:</strong> @step1Data?.Name</p>
                <p><strong>Email:</strong> @step1Data?.Email</p>
                
                <h4>Project Details</h4>
                <p><strong>Category:</strong> @step2Data?.Subject</p>
                <p><strong>Description:</strong> @step2Data?.Message</p>
            </div>
            
            <div class="actions">
                <button class="btn btn-secondary" @onclick="() => currentStep = 2">
                    Back to Edit
                </button>
                <button class="btn btn-primary" @onclick="SubmitFinalForm">
                    Submit Project Request
                </button>
            </div>
        </div>
    }
</div>

@code {
    private int currentStep = 1;
    private ContactFormModel? step1Data;
    private ContactFormModel? step2Data;
    
    private async Task HandleStep1(ContactFormModel model)
    {
        step1Data = model;
        currentStep = 2;
        StateHasChanged();
    }
    
    private async Task HandleStep2(ContactFormModel model)
    {
        step2Data = model;
        currentStep = 3;
        StateHasChanged();
    }
    
    private async Task SubmitFinalForm()
    {
        if (step1Data != null && step2Data != null)
        {
            var combinedData = new ContactFormModel
            {
                Name = step1Data.Name,
                Email = step1Data.Email,
                Subject = step2Data.Subject,
                Message = step2Data.Message
            };
            
            await ProjectService.SubmitProjectRequestAsync(combinedData);
            await NotificationService.ShowSuccessAsync("Project request submitted successfully!");
            
            // Reset form
            currentStep = 1;
            step1Data = null;
            step2Data = null;
        }
    }
}

<style>
.multi-step-contact {
    max-width: 600px;
    margin: 0 auto;
}

.step-indicator {
    display: flex;
    justify-content: space-between;
    margin-bottom: 2rem;
    padding: 0 1rem;
}

.step {
    padding: 0.5rem 1rem;
    background: #f3f4f6;
    color: #6b7280;
    border-radius: 0.5rem;
    font-size: 0.875rem;
    font-weight: 500;
}

.step.active {
    background: #3b82f6;
    color: white;
}

.confirmation-step {
    background: #f9fafb;
    padding: 2rem;
    border-radius: 0.5rem;
    border: 1px solid #e5e7eb;
}

.summary {
    margin-bottom: 2rem;
}

.summary h4 {
    color: #374151;
    margin-bottom: 0.5rem;
    font-size: 1.125rem;
}

.summary p {
    margin-bottom: 0.5rem;
    color: #6b7280;
}

.actions {
    display: flex;
    gap: 1rem;
    justify-content: flex-end;
}
</style>
```

### Contact Form with File Upload

```razor
<div class="contact-form-with-upload">
    <OsirionContactForm 
        Id="contact-with-files"
        FormTitle="Project Consultation"
        FormDescription="Upload relevant files to help us understand your project better."
        OnFormSubmitted="HandleFormWithFiles"
        OnFormError="HandleUploadError">
        
        <div class="file-upload-section">
            <h4>Attach Files (Optional)</h4>
            <p class="upload-description">
                Upload project documents, wireframes, or reference materials (Max 10MB per file)
            </p>
            
            <InputFile id="file-upload" 
                       OnChange="HandleFileSelection"
                       multiple
                       accept=".pdf,.doc,.docx,.jpg,.jpeg,.png,.zip"
                       class="file-input" />
            
            <label for="file-upload" class="file-upload-label">
                <i class="icon-upload"></i>
                Choose Files or Drag & Drop
            </label>
            
            @if (selectedFiles?.Any() == true)
            {
                <div class="selected-files">
                    <h5>Selected Files:</h5>
                    @foreach (var file in selectedFiles)
                    {
                        <div class="file-item">
                            <span class="file-name">@file.Name</span>
                            <span class="file-size">(@(FormatFileSize(file.Size)))</span>
                            <button type="button" class="remove-file" @onclick="() => RemoveFile(file)">
                                ×
                            </button>
                        </div>
                    }
                </div>
            }
        </div>
    </OsirionContactForm>
</div>

@code {
    private IList<IBrowserFile>? selectedFiles;
    private const long MaxFileSize = 10 * 1024 * 1024; // 10MB
    
    private async Task HandleFileSelection(InputFileChangeEventArgs e)
    {
        selectedFiles = e.GetMultipleFiles(5).ToList(); // Max 5 files
        
        // Validate file sizes
        var oversizedFiles = selectedFiles.Where(f => f.Size > MaxFileSize).ToList();
        if (oversizedFiles.Any())
        {
            await NotificationService.ShowWarningAsync(
                $"The following files exceed 10MB limit: {string.Join(", ", oversizedFiles.Select(f => f.Name))}");
            
            selectedFiles = selectedFiles.Except(oversizedFiles).ToList();
        }
        
        StateHasChanged();
    }
    
    private void RemoveFile(IBrowserFile file)
    {
        selectedFiles?.Remove(file);
        StateHasChanged();
    }
    
    private async Task HandleFormWithFiles(ContactFormModel model)
    {
        try
        {
            // Upload files first
            var fileUrls = new List<string>();
            
            if (selectedFiles?.Any() == true)
            {
                foreach (var file in selectedFiles)
                {
                    var url = await FileUploadService.UploadFileAsync(file, $"contact-forms/{DateTime.UtcNow:yyyy-MM}");
                    fileUrls.Add(url);
                }
            }
            
            // Submit form with file attachments
            await ContactService.SubmitContactFormAsync(model, fileUrls);
            
            await NotificationService.ShowSuccessAsync("Form submitted successfully with attachments!");
            
            // Reset
            selectedFiles = null;
            StateHasChanged();
        }
        catch (Exception ex)
        {
            await NotificationService.ShowErrorAsync($"Failed to upload files: {ex.Message}");
        }
    }
    
    private async Task HandleUploadError(string error)
    {
        await NotificationService.ShowErrorAsync($"Upload failed: {error}");
    }
    
    private string FormatFileSize(long bytes)
    {
        if (bytes < 1024) return $"{bytes} B";
        if (bytes < 1024 * 1024) return $"{bytes / 1024:F1} KB";
        return $"{bytes / (1024 * 1024):F1} MB";
    }
}

<style>
.contact-form-with-upload {
    max-width: 600px;
    margin: 0 auto;
}

.file-upload-section {
    margin: 2rem 0;
    padding: 1.5rem;
    border: 2px dashed #d1d5db;
    border-radius: 0.5rem;
    text-align: center;
}

.file-input {
    display: none;
}

.file-upload-label {
    display: inline-block;
    padding: 1rem 2rem;
    background: #3b82f6;
    color: white;
    border-radius: 0.5rem;
    cursor: pointer;
    font-weight: 500;
    transition: background 0.2s;
}

.file-upload-label:hover {
    background: #2563eb;
}

.upload-description {
    color: #6b7280;
    font-size: 0.875rem;
    margin-bottom: 1rem;
}

.selected-files {
    margin-top: 1rem;
    text-align: left;
}

.file-item {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 0.5rem;
    background: #f3f4f6;
    border-radius: 0.25rem;
    margin-bottom: 0.5rem;
}

.file-name {
    font-weight: 500;
}

.file-size {
    color: #6b7280;
    font-size: 0.875rem;
}

.remove-file {
    background: #ef4444;
    color: white;
    border: none;
    border-radius: 50%;
    width: 24px;
    height: 24px;
    cursor: pointer;
    font-size: 1.2rem;
    display: flex;
    align-items: center;
    justify-content: center;
}
</style>
```

### Contact Form with CRM Integration

```razor
<OsirionContactForm 
    Id="crm-integrated-contact"
    FormTitle="Sales Inquiry"
    FormDescription="Interested in our services? Fill out this form and our sales team will contact you."
    OnFormSubmitted="HandleCRMSubmission"
    OnFormError="HandleCRMError">
    
    <!-- Custom fields for CRM integration -->
    <div class="additional-fields">
        <div class="form-group">
            <label for="company">Company Name</label>
            <input type="text" id="company" @bind="companyName" class="form-control" placeholder="Your company name" />
        </div>
        
        <div class="form-group">
            <label for="budget">Estimated Budget</label>
            <select id="budget" @bind="estimatedBudget" class="form-control">
                <option value="">Select budget range</option>
                <option value="under-10k">Under $10,000</option>
                <option value="10k-25k">$10,000 - $25,000</option>
                <option value="25k-50k">$25,000 - $50,000</option>
                <option value="50k-100k">$50,000 - $100,000</option>
                <option value="over-100k">Over $100,000</option>
            </select>
        </div>
        
        <div class="form-group">
            <label for="timeline">Project Timeline</label>
            <select id="timeline" @bind="projectTimeline" class="form-control">
                <option value="">Select timeline</option>
                <option value="asap">ASAP</option>
                <option value="1-3-months">1-3 months</option>
                <option value="3-6-months">3-6 months</option>
                <option value="6-12-months">6-12 months</option>
                <option value="planning">Just planning</option>
            </select>
        </div>
    </div>
</OsirionContactForm>

@code {
    private string companyName = "";
    private string estimatedBudget = "";
    private string projectTimeline = "";
    
    private async Task HandleCRMSubmission(ContactFormModel model)
    {
        try
        {
            // Create CRM lead
            var crmLead = new CRMLead
            {
                FirstName = ExtractFirstName(model.Name),
                LastName = ExtractLastName(model.Name),
                Email = model.Email,
                Company = companyName,
                Subject = model.Subject,
                Description = model.Message,
                EstimatedBudget = estimatedBudget,
                ProjectTimeline = projectTimeline,
                Source = "Website Contact Form",
                Status = "New",
                CreatedDate = DateTime.UtcNow,
                Tags = new[] { "website-lead", "sales-inquiry" }
            };
            
            // Submit to CRM system
            var leadId = await CRMService.CreateLeadAsync(crmLead);
            
            // Assign to sales rep based on budget and timeline
            var assignedRep = await SalesService.AssignLeadAsync(leadId, estimatedBudget, projectTimeline);
            
            // Send notification to assigned rep
            await NotificationService.NotifySalesRepAsync(assignedRep, crmLead);
            
            // Track in analytics
            await Analytics.TrackEvent("sales_lead_created", new
            {
                lead_id = leadId,
                budget_range = estimatedBudget,
                timeline = projectTimeline,
                company = companyName
            });
            
            await NotificationService.ShowSuccessAsync(
                $"Thank you! Your inquiry has been received. {assignedRep.Name} from our sales team will contact you within 24 hours.");
            
            // Reset form
            ResetForm();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to create CRM lead");
            await NotificationService.ShowErrorAsync("Failed to submit inquiry. Please try again or contact us directly.");
        }
    }
    
    private async Task HandleCRMError(string error)
    {
        Logger.LogError("CRM integration error: {Error}", error);
        await NotificationService.ShowErrorAsync("There was an issue submitting your inquiry. Please try again.");
    }
    
    private void ResetForm()
    {
        companyName = "";
        estimatedBudget = "";
        projectTimeline = "";
        StateHasChanged();
    }
    
    private string ExtractFirstName(string fullName)
    {
        var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 0 ? parts[0] : fullName;
    }
    
    private string ExtractLastName(string fullName)
    {
        var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 1 ? string.Join(" ", parts.Skip(1)) : "";
    }
}

<style>
.additional-fields {
    margin: 1.5rem 0;
}

.form-group {
    margin-bottom: 1rem;
}

.form-group label {
    display: block;
    margin-bottom: 0.5rem;
    font-weight: 500;
    color: var(--text-color, #374151);
}

.form-control {
    width: 100%;
    padding: 0.75rem;
    border: 1px solid var(--border-color, #d1d5db);
    border-radius: 0.375rem;
    font-size: 1rem;
    transition: border-color 0.2s, box-shadow 0.2s;
}

.form-control:focus {
    outline: none;
    border-color: var(--primary-color, #3b82f6);
    box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}
</style>
```

## Accessibility Features

### Full Accessibility Support

```razor
<!-- The component automatically provides comprehensive accessibility -->
<OsirionContactForm 
    Id="accessible-contact"
    FormTitle="Accessible Contact Form"
    FormDescription="This form is fully accessible with ARIA labels and keyboard navigation."
    aria-label="Contact form for customer inquiries"
    role="form" />
```

The component automatically includes:
- Proper ARIA labels and descriptions
- Keyboard navigation support
- Screen reader announcements
- Error message associations
- Focus management
- High contrast mode support

### Custom Accessibility Enhancements

```razor
<OsirionContactForm 
    Id="enhanced-accessible"
    OnValidationFailed="AnnounceValidationErrors"
    OnFormSubmitted="AnnounceSuccess" />

@code {
    private async Task AnnounceValidationErrors(IEnumerable<string> errors)
    {
        var message = $"Form has {errors.Count()} validation errors: {string.Join(", ", errors)}";
        await JSRuntime.InvokeVoidAsync("announceToScreenReader", message);
    }
    
    private async Task AnnounceSuccess(ContactFormModel model)
    {
        await JSRuntime.InvokeVoidAsync("announceToScreenReader", "Form submitted successfully");
    }
}

<script>
    window.announceToScreenReader = (message) => {
        const announcement = document.createElement('div');
        announcement.setAttribute('aria-live', 'polite');
        announcement.setAttribute('aria-atomic', 'true');
        announcement.className = 'sr-only';
        announcement.textContent = message;
        
        document.body.appendChild(announcement);
        
        setTimeout(() => {
            document.body.removeChild(announcement);
        }, 1000);
    };
</script>
```

## Email Configuration

### SMTP Configuration

```csharp
// Program.cs or Startup.cs
services.Configure<EmailSettings>(options =>
{
    options.SmtpServer = "smtp.gmail.com";
    options.SmtpPort = 587;
    options.SmtpUsername = "your-email@gmail.com";
    options.SmtpPassword = "your-app-password";
    options.UseSsl = true;
    options.FromEmail = "noreply@yourcompany.com";
    options.FromName = "Your Company Contact Form";
});

services.AddTransient<IEmailService, EmailService>();
```

### Custom Email Templates

```razor
<OsirionContactForm 
    Id="templated-contact"
    ToEmail="support@company.com"
    EmailTemplateId="contact-form-template"
    OnFormSubmitted="HandleTemplatedSubmission" />

@code {
    private async Task HandleTemplatedSubmission(ContactFormModel model)
    {
        // Use custom email template
        await EmailService.SendTemplatedEmailAsync("contact-form-template", new
        {
            CustomerName = model.Name,
            CustomerEmail = model.Email,
            Subject = model.Subject,
            Message = model.Message,
            SubmissionDate = DateTime.Now.ToString("F"),
            CompanyName = "Your Company",
            SupportEmail = "support@company.com"
        });
    }
}
```

## Best Practices

### Form Design Guidelines

1. **Clear Labels**: Use descriptive labels and placeholder text
2. **Validation Feedback**: Provide immediate, helpful validation messages
3. **Required Fields**: Clearly indicate which fields are required
4. **Error Handling**: Show specific, actionable error messages
5. **Success Feedback**: Confirm successful submissions clearly

### Security Considerations

1. **Input Validation**: Always validate and sanitize form inputs
2. **Rate Limiting**: Implement rate limiting to prevent spam
3. **CAPTCHA**: Consider adding CAPTCHA for high-traffic sites
4. **Email Security**: Use secure SMTP settings and authentication
5. **Data Privacy**: Follow GDPR and privacy regulations

### Performance Optimization

1. **Form Validation**: Use client-side validation for immediate feedback
2. **Progressive Enhancement**: Ensure forms work without JavaScript
3. **Loading States**: Show loading indicators during submission
4. **File Uploads**: Validate file types and sizes before upload
5. **Error Recovery**: Preserve form data on submission errors

The OsirionContactForm component provides a robust, accessible, and feature-rich solution for implementing contact forms in Blazor applications with comprehensive customization options and integration capabilities.
