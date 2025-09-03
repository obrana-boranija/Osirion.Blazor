---
id: 'osirion-page-loading'
order: 2
layout: docs
title: OsirionPageLoading Component
permalink: /docs/components/core/layout/page-loading
description: Learn how to use the OsirionPageLoading component to display elegant loading states with customizable animations and messages.
author: Dejan Demonjić
date: 2025-09-03
featured_image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
categories:
- Components
- Core Components
- Layout
- Loading States
tags:
- blazor
- loading
- spinner
- page-loading
- loading-states
- animations
- user-experience
is_featured: true
published: true
slug: components/core/layout/page-loading
lang: en
custom_fields: {}
seo_properties:
  title: 'OsirionPageLoading Component - Elegant Loading States | Osirion.Blazor'
  description: 'Display elegant loading states with the OsirionPageLoading component. Features customizable animations, messages, and responsive design.'
  image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
  canonical: 'https://getosirion.com/docs/components/core/layout/page-loading'
  lang: en
  robots: index, follow
  og_title: 'OsirionPageLoading Component - Osirion.Blazor'
  og_description: 'Display elegant loading states with customizable animations and messages.'
  og_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'OsirionPageLoading Component - Osirion.Blazor'
  twitter_description: 'Display elegant loading states with customizable animations and messages.'
  twitter_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  meta_tags: {}
---

# OsirionPageLoading Component

The OsirionPageLoading component provides elegant and customizable loading states for pages and content areas. It features smooth animations, customizable messages, and responsive design to enhance user experience during data loading or page transitions.

## Component Overview

OsirionPageLoading displays a visually appealing loading indicator with optional text messages, progress indicators, and custom animations. It's designed to provide immediate feedback to users while content is being loaded, reducing perceived waiting time and improving overall user experience.

### Key Features

**Multiple Animation Types**: Support for spinner, dots, pulse, skeleton, and custom animations
**Customizable Messages**: Display loading messages, progress updates, and contextual information
**Progress Indicators**: Optional progress bars and percentage displays for long-running operations
**Responsive Design**: Adapts to different screen sizes and container dimensions
**Accessibility**: Full ARIA support for screen readers and assistive technologies
**Theme Integration**: Compatible with all CSS frameworks and custom theme systems
**Performance Optimized**: Lightweight animations with minimal impact on performance
**SSR Compatible**: Works seamlessly with Server-Side Rendering

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Message` | `string?` | `"Loading..."` | Text message displayed below the loading animation. |
| `ShowMessage` | `bool` | `true` | Whether to display the loading message. |
| `AnimationType` | `LoadingAnimationType` | `Spinner` | Type of loading animation to display. |
| `Size` | `LoadingSize` | `Medium` | Size of the loading indicator. |
| `Color` | `string?` | `null` | Custom color for the loading indicator. |
| `ShowProgress` | `bool` | `false` | Whether to show a progress indicator. |
| `Progress` | `int` | `0` | Progress value (0-100) when ShowProgress is true. |
| `FullScreen` | `bool` | `false` | Whether to display as full-screen overlay. |
| `Overlay` | `bool` | `false` | Whether to show background overlay. |
| `OverlayOpacity` | `double` | `0.5` | Opacity of the background overlay (0.0-1.0). |
| `Class` | `string?` | `null` | Additional CSS classes to apply to the loading container. |
| `Style` | `string?` | `null` | Inline styles to apply to the loading container. |

## Enums

### LoadingAnimationType

- `Spinner` - Classic rotating spinner
- `Dots` - Animated dots sequence
- `Pulse` - Pulsing circle animation
- `Skeleton` - Skeleton placeholder animation
- `Wave` - Wave loading animation
- `Custom` - Custom animation (requires CSS)

### LoadingSize

- `Small` - Small loading indicator
- `Medium` - Medium loading indicator (default)
- `Large` - Large loading indicator
- `ExtraLarge` - Extra large loading indicator

## Basic Usage

### Simple Loading State

```razor
@using Osirion.Blazor.Components

<div class="content-area">
    @if (isLoading)
    {
        <OsirionPageLoading Message="Loading content..." />
    }
    else
    {
        <div class="loaded-content">
            <h2>Content Loaded</h2>
            <p>This content was loaded dynamically.</p>
        </div>
    }
</div>

@code {
    private bool isLoading = true;
    
    protected override async Task OnInitializedAsync()
    {
        // Simulate loading delay
        await Task.Delay(2000);
        isLoading = false;
        StateHasChanged();
    }
}
```

### Full-Screen Loading

```razor
@if (isInitializing)
{
    <OsirionPageLoading 
        Message="Initializing application..."
        FullScreen="true"
        Overlay="true"
        AnimationType="LoadingAnimationType.Pulse"
        Size="LoadingSize.Large" />
}
else
{
    <!-- Your app content -->
    <div class="app-content">
        <h1>Welcome to the Application</h1>
        <!-- Main content here -->
    </div>
}

@code {
    private bool isInitializing = true;
    
    protected override async Task OnInitializedAsync()
    {
        // Initialize app services
        await InitializeServices();
        isInitializing = false;
        StateHasChanged();
    }
    
    private async Task InitializeServices()
    {
        // Simulate service initialization
        await Task.Delay(3000);
    }
}
```

### Loading with Progress

```razor
<div class="upload-container">
    @if (isUploading)
    {
        <OsirionPageLoading 
            Message="@uploadMessage"
            ShowProgress="true"
            Progress="uploadProgress"
            AnimationType="LoadingAnimationType.Wave"
            Size="LoadingSize.Medium" />
    }
    else
    {
        <div class="upload-form">
            <input type="file" @onchange="HandleFileUpload" />
            <button @onclick="StartUpload" disabled="@(selectedFile == null)">
                Upload File
            </button>
        </div>
    }
</div>

@code {
    private bool isUploading = false;
    private int uploadProgress = 0;
    private string uploadMessage = "Uploading file...";
    private IBrowserFile? selectedFile;
    
    private void HandleFileUpload(InputFileChangeEventArgs e)
    {
        selectedFile = e.File;
    }
    
    private async Task StartUpload()
    {
        if (selectedFile == null) return;
        
        isUploading = true;
        uploadProgress = 0;
        
        try
        {
            // Simulate file upload with progress
            for (int i = 0; i <= 100; i += 5)
            {
                uploadProgress = i;
                uploadMessage = $"Uploading {selectedFile.Name}... {i}%";
                StateHasChanged();
                await Task.Delay(100);
            }
            
            uploadMessage = "Upload completed!";
            await Task.Delay(1000);
        }
        finally
        {
            isUploading = false;
            StateHasChanged();
        }
    }
}
```

## Advanced Usage

### Multi-Step Loading Process

```razor
<div class="multi-step-loading">
    @if (isProcessing)
    {
        <OsirionPageLoading 
            Message="@GetCurrentStepMessage()"
            ShowProgress="true"
            Progress="@GetOverallProgress()"
            AnimationType="LoadingAnimationType.Dots"
            Size="LoadingSize.Large"
            Class="process-loading" />
        
        <div class="step-details">
            <h3>@steps[currentStep].Title</h3>
            <p>@steps[currentStep].Description</p>
            
            <div class="step-progress">
                @for (int i = 0; i < steps.Count; i++)
                {
                    <div class="step-indicator @(i == currentStep ? "active" : i < currentStep ? "completed" : "pending")">
                        <span class="step-number">@(i + 1)</span>
                        <span class="step-name">@steps[i].Name</span>
                    </div>
                }
            </div>
        </div>
    }
    else
    {
        <div class="process-complete">
            <h2>✅ Process Completed Successfully</h2>
            <p>All steps have been completed.</p>
        </div>
    }
</div>

@code {
    private bool isProcessing = true;
    private int currentStep = 0;
    
    private readonly List<ProcessStep> steps = new()
    {
        new("Validate", "Validating Data", "Checking input data for errors and completeness"),
        new("Process", "Processing Data", "Transforming and analyzing the provided data"),
        new("Save", "Saving Results", "Storing processed data to the database"),
        new("Notify", "Sending Notifications", "Notifying relevant parties of completion")
    };
    
    protected override async Task OnInitializedAsync()
    {
        await ExecuteMultiStepProcess();
    }
    
    private async Task ExecuteMultiStepProcess()
    {
        for (int i = 0; i < steps.Count; i++)
        {
            currentStep = i;
            StateHasChanged();
            
            // Simulate step processing
            await Task.Delay(2000);
        }
        
        isProcessing = false;
        StateHasChanged();
    }
    
    private string GetCurrentStepMessage()
    {
        if (currentStep < steps.Count)
        {
            return $"Step {currentStep + 1} of {steps.Count}: {steps[currentStep].Title}";
        }
        return "Processing complete";
    }
    
    private int GetOverallProgress()
    {
        return (int)((double)(currentStep + 1) / steps.Count * 100);
    }
    
    public record ProcessStep(string Name, string Title, string Description);
}

<style>
.multi-step-loading {
    max-width: 600px;
    margin: 2rem auto;
    padding: 2rem;
    background: #ffffff;
    border-radius: 0.5rem;
    box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
}

.step-details {
    margin-top: 2rem;
    text-align: center;
}

.step-progress {
    display: flex;
    justify-content: space-between;
    margin-top: 2rem;
    padding: 0 1rem;
}

.step-indicator {
    display: flex;
    flex-direction: column;
    align-items: center;
    flex: 1;
    position: relative;
}

.step-indicator:not(:last-child)::after {
    content: '';
    position: absolute;
    top: 15px;
    right: -50%;
    width: 100%;
    height: 2px;
    background: #e5e7eb;
    z-index: 1;
}

.step-indicator.completed:not(:last-child)::after {
    background: #10b981;
}

.step-number {
    width: 30px;
    height: 30px;
    border-radius: 50%;
    background: #e5e7eb;
    color: #6b7280;
    display: flex;
    align-items: center;
    justify-content: center;
    font-weight: bold;
    font-size: 0.875rem;
    position: relative;
    z-index: 2;
}

.step-indicator.active .step-number {
    background: #3b82f6;
    color: white;
}

.step-indicator.completed .step-number {
    background: #10b981;
    color: white;
}

.step-name {
    margin-top: 0.5rem;
    font-size: 0.75rem;
    color: #6b7280;
    text-align: center;
}

.step-indicator.active .step-name {
    color: #3b82f6;
    font-weight: 600;
}

.step-indicator.completed .step-name {
    color: #10b981;
}

.process-complete {
    text-align: center;
    padding: 2rem;
}
</style>
```

### Skeleton Loading

```razor
<div class="content-container">
    @if (isLoadingContent)
    {
        <OsirionPageLoading 
            AnimationType="LoadingAnimationType.Skeleton"
            ShowMessage="false"
            Class="skeleton-container" />
        
        <!-- Custom skeleton content -->
        <div class="skeleton-content">
            <div class="skeleton-header">
                <div class="skeleton-avatar"></div>
                <div class="skeleton-text">
                    <div class="skeleton-line skeleton-title"></div>
                    <div class="skeleton-line skeleton-subtitle"></div>
                </div>
            </div>
            
            <div class="skeleton-body">
                <div class="skeleton-line skeleton-paragraph"></div>
                <div class="skeleton-line skeleton-paragraph"></div>
                <div class="skeleton-line skeleton-paragraph short"></div>
            </div>
            
            <div class="skeleton-actions">
                <div class="skeleton-button"></div>
                <div class="skeleton-button secondary"></div>
            </div>
        </div>
    }
    else
    {
        <!-- Actual content -->
        <div class="loaded-content">
            <div class="content-header">
                <img src="/images/user-avatar.jpg" alt="User" class="user-avatar" />
                <div class="content-text">
                    <h2>John Doe</h2>
                    <p>Software Engineer</p>
                </div>
            </div>
            
            <div class="content-body">
                <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.</p>
                <p>Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.</p>
                <p>Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.</p>
            </div>
            
            <div class="content-actions">
                <button class="btn btn-primary">Contact</button>
                <button class="btn btn-secondary">View Profile</button>
            </div>
        </div>
    }
</div>

@code {
    private bool isLoadingContent = true;
    
    protected override async Task OnInitializedAsync()
    {
        await Task.Delay(3000);
        isLoadingContent = false;
        StateHasChanged();
    }
}

<style>
.skeleton-content {
    padding: 1.5rem;
    background: #ffffff;
    border-radius: 0.5rem;
    border: 1px solid #e5e7eb;
}

.skeleton-header {
    display: flex;
    align-items: center;
    margin-bottom: 1.5rem;
}

.skeleton-avatar {
    width: 60px;
    height: 60px;
    border-radius: 50%;
    background: linear-gradient(90deg, #f3f4f6 25%, #e5e7eb 50%, #f3f4f6 75%);
    background-size: 200% 100%;
    animation: shimmer 1.5s infinite;
}

.skeleton-text {
    margin-left: 1rem;
    flex: 1;
}

.skeleton-line {
    height: 1rem;
    background: linear-gradient(90deg, #f3f4f6 25%, #e5e7eb 50%, #f3f4f6 75%);
    background-size: 200% 100%;
    animation: shimmer 1.5s infinite;
    border-radius: 0.25rem;
    margin-bottom: 0.5rem;
}

.skeleton-title {
    width: 40%;
    height: 1.25rem;
}

.skeleton-subtitle {
    width: 60%;
    height: 1rem;
}

.skeleton-paragraph {
    width: 100%;
    margin-bottom: 0.75rem;
}

.skeleton-paragraph.short {
    width: 70%;
}

.skeleton-body {
    margin-bottom: 1.5rem;
}

.skeleton-actions {
    display: flex;
    gap: 1rem;
}

.skeleton-button {
    width: 100px;
    height: 40px;
    background: linear-gradient(90deg, #f3f4f6 25%, #e5e7eb 50%, #f3f4f6 75%);
    background-size: 200% 100%;
    animation: shimmer 1.5s infinite;
    border-radius: 0.375rem;
}

.skeleton-button.secondary {
    width: 120px;
}

@keyframes shimmer {
    0% {
        background-position: 200% 0;
    }
    100% {
        background-position: -200% 0;
    }
}

.loaded-content {
    padding: 1.5rem;
    background: #ffffff;
    border-radius: 0.5rem;
    border: 1px solid #e5e7eb;
    animation: fadeIn 0.3s ease-in;
}

.content-header {
    display: flex;
    align-items: center;
    margin-bottom: 1.5rem;
}

.user-avatar {
    width: 60px;
    height: 60px;
    border-radius: 50%;
    object-fit: cover;
}

.content-text {
    margin-left: 1rem;
}

.content-text h2 {
    margin: 0 0 0.25rem 0;
    font-size: 1.25rem;
    font-weight: 600;
}

.content-text p {
    margin: 0;
    color: #6b7280;
}

.content-body {
    margin-bottom: 1.5rem;
    line-height: 1.6;
}

.content-actions {
    display: flex;
    gap: 1rem;
}

.btn {
    padding: 0.5rem 1rem;
    border-radius: 0.375rem;
    border: none;
    font-weight: 500;
    cursor: pointer;
    transition: background-color 0.2s;
}

.btn-primary {
    background: #3b82f6;
    color: white;
}

.btn-primary:hover {
    background: #2563eb;
}

.btn-secondary {
    background: #f3f4f6;
    color: #374151;
}

.btn-secondary:hover {
    background: #e5e7eb;
}

@keyframes fadeIn {
    from {
        opacity: 0;
        transform: translateY(10px);
    }
    to {
        opacity: 1;
        transform: translateY(0);
    }
}
</style>
```

### Custom Loading Animation

```razor
<div class="custom-loading-container">
    @if (isLoading)
    {
        <OsirionPageLoading 
            AnimationType="LoadingAnimationType.Custom"
            Message="@loadingMessage"
            ShowProgress="@showProgress"
            Progress="@progress"
            Class="custom-loading"
            Style="@GetCustomStyle()" />
        
        <!-- Custom animation overlay -->
        <div class="custom-animation">
            <div class="bouncing-balls">
                <div class="ball ball-1"></div>
                <div class="ball ball-2"></div>
                <div class="ball ball-3"></div>
            </div>
            
            <div class="loading-text">
                <span class="text-animate">@loadingMessage</span>
            </div>
            
            @if (showProgress)
            {
                <div class="custom-progress">
                    <div class="progress-bar">
                        <div class="progress-fill" style="width: @(progress)%"></div>
                    </div>
                    <span class="progress-text">@progress%</span>
                </div>
            }
        </div>
    }
    else
    {
        <div class="content-loaded">
            <h2>🎉 Loading Complete!</h2>
            <p>Content has been successfully loaded.</p>
        </div>
    }
</div>

@code {
    private bool isLoading = true;
    private bool showProgress = false;
    private int progress = 0;
    private string loadingMessage = "Preparing awesome content...";
    
    private readonly string[] messages = {
        "Preparing awesome content...",
        "Fetching latest data...",
        "Optimizing performance...",
        "Almost ready...",
        "Finalizing..."
    };
    
    protected override async Task OnInitializedAsync()
    {
        await SimulateLoadingWithProgress();
    }
    
    private async Task SimulateLoadingWithProgress()
    {
        // Initial loading phase
        await Task.Delay(1000);
        
        // Progress phase
        showProgress = true;
        StateHasChanged();
        
        for (int i = 0; i <= 100; i += 2)
        {
            progress = i;
            
            // Update message based on progress
            int messageIndex = Math.Min((int)(i / 20), messages.Length - 1);
            loadingMessage = messages[messageIndex];
            
            StateHasChanged();
            await Task.Delay(50);
        }
        
        // Completion
        await Task.Delay(500);
        isLoading = false;
        StateHasChanged();
    }
    
    private string GetCustomStyle()
    {
        return $"--progress-width: {progress}%; --animation-speed: {(isLoading ? "1s" : "0s")};";
    }
}

<style>
.custom-loading-container {
    position: relative;
    min-height: 400px;
    display: flex;
    align-items: center;
    justify-content: center;
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    border-radius: 1rem;
    overflow: hidden;
}

.custom-animation {
    text-align: center;
    color: white;
    z-index: 10;
}

.bouncing-balls {
    display: flex;
    justify-content: center;
    gap: 0.5rem;
    margin-bottom: 2rem;
}

.ball {
    width: 20px;
    height: 20px;
    border-radius: 50%;
    background: rgba(255, 255, 255, 0.9);
    animation: bounce 1.4s infinite ease-in-out both;
}

.ball-1 {
    animation-delay: -0.32s;
}

.ball-2 {
    animation-delay: -0.16s;
}

.ball-3 {
    animation-delay: 0s;
}

@keyframes bounce {
    0%, 80%, 100% {
        transform: scale(0.8);
        opacity: 0.5;
    }
    40% {
        transform: scale(1.2);
        opacity: 1;
    }
}

.loading-text {
    margin-bottom: 2rem;
}

.text-animate {
    font-size: 1.25rem;
    font-weight: 600;
    letter-spacing: 0.05em;
    animation: pulse 2s infinite;
}

@keyframes pulse {
    0%, 100% {
        opacity: 1;
    }
    50% {
        opacity: 0.7;
    }
}

.custom-progress {
    max-width: 300px;
    margin: 0 auto;
}

.progress-bar {
    width: 100%;
    height: 8px;
    background: rgba(255, 255, 255, 0.3);
    border-radius: 4px;
    overflow: hidden;
    margin-bottom: 0.5rem;
}

.progress-fill {
    height: 100%;
    background: linear-gradient(90deg, #ffffff, #f0f0f0);
    border-radius: 4px;
    transition: width 0.3s ease;
    animation: progressShimmer 2s infinite;
}

@keyframes progressShimmer {
    0% {
        background-position: -100% 0;
    }
    100% {
        background-position: 100% 0;
    }
}

.progress-text {
    font-size: 0.875rem;
    font-weight: 500;
    opacity: 0.9;
}

.content-loaded {
    text-align: center;
    color: white;
    animation: fadeInUp 0.6s ease-out;
}

@keyframes fadeInUp {
    from {
        opacity: 0;
        transform: translateY(30px);
    }
    to {
        opacity: 1;
        transform: translateY(0);
    }
}

.content-loaded h2 {
    font-size: 2rem;
    margin-bottom: 1rem;
}

.content-loaded p {
    font-size: 1.125rem;
    opacity: 0.9;
}
</style>
```

## Animation Types

### Spinner Animation

```razor
<OsirionPageLoading 
    AnimationType="LoadingAnimationType.Spinner"
    Message="Loading with spinner..."
    Size="LoadingSize.Medium" />
```

### Dots Animation

```razor
<OsirionPageLoading 
    AnimationType="LoadingAnimationType.Dots"
    Message="Loading with animated dots..."
    Color="#3b82f6" />
```

### Pulse Animation

```razor
<OsirionPageLoading 
    AnimationType="LoadingAnimationType.Pulse"
    Message="Pulsing while loading..."
    Size="LoadingSize.Large" />
```

### Wave Animation

```razor
<OsirionPageLoading 
    AnimationType="LoadingAnimationType.Wave"
    Message="Wave loading animation..."
    ShowProgress="true"
    Progress="65" />
```

## Accessibility Features

The OsirionPageLoading component includes comprehensive accessibility support:

- **ARIA Live Regions**: Loading messages are announced to screen readers
- **ARIA Labels**: Proper labeling for all interactive elements
- **Focus Management**: Appropriate focus handling during loading states
- **High Contrast**: Support for high contrast mode and color preferences
- **Reduced Motion**: Respects user's motion preferences
- **Keyboard Navigation**: Full keyboard accessibility for interactive elements

## Best Practices

### Loading State Guidelines

1. **Immediate Feedback**: Show loading state immediately when operation starts
2. **Contextual Messages**: Use descriptive messages that explain what's happening
3. **Progress Indication**: Show progress for operations longer than 3 seconds
4. **Graceful Degradation**: Provide fallback for users with reduced motion preferences
5. **Performance**: Use efficient animations that don't impact performance

### User Experience

1. **Skeleton Loading**: Use skeleton screens for content-heavy pages
2. **Progressive Loading**: Load critical content first, then secondary content
3. **Error Handling**: Provide clear error states and recovery options
4. **Timeout Handling**: Set reasonable timeouts and provide escape routes
5. **Consistent Styling**: Maintain consistent loading patterns across the application

### Technical Considerations

1. **Memory Management**: Dispose of loading components properly
2. **State Management**: Handle loading states in a centralized manner
3. **Performance Monitoring**: Track loading times and optimize accordingly
4. **Responsive Design**: Ensure loading states work on all screen sizes
5. **Testing**: Test loading states across different network conditions

The OsirionPageLoading component provides a comprehensive solution for displaying loading states with excellent user experience and accessibility support.
