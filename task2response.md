---
title: "NotificationService API"
version: "1.0.0"
date: "2025-07-19"
---

# Overview
A service for sending notifications via multiple channels (email, SMS, push notifications). Supports SMTP for emails, Twilio for SMS, and Firebase Cloud Messaging for push notifications.

# API Reference
| Method                                           | Description                                        | Return Type                   |
|--------------------------------------------------|----------------------------------------------------|-------------------------------|
| `SendEmailAsync(to, subject, body)`              | Sends an email to the specified recipient.         | `Task<NotificationResult>`    |
| `SendSmsAsync(phoneNumber, message)`             | Sends an SMS to the specified phone number.       | `Task<NotificationResult>`    |
| `SendPushAsync(deviceToken, title, body)`        | Sends a push notification to the specified device. | `Task<NotificationResult>`    |

# Examples

## C# Usage
```csharp
// Send email
var emailResult = await notificationService.SendEmailAsync(
    "user@example.com",
    "Welcome to our service",
    "Thank you for registering with us!"
);
if (emailResult.IsSuccess)
{
    Console.WriteLine("Email sent successfully");
}

// Send SMS
var smsResult = await notificationService.SendSmsAsync(
    "+15551234567",
    "Your verification code is 123456"
);
if (smsResult.IsSuccess)
{
    Console.WriteLine("SMS sent successfully");
}

// Send push notification
var pushResult = await notificationService.SendPushAsync(
    "device-token-abc123",
    "New message",
    "You have a new notification"
);
if (pushResult.IsSuccess)
{
    Console.WriteLine("Push notification sent successfully");
}
