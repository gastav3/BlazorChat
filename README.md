# BlazorChat
BlazorChat is a real-time web chat application built entirely with .NET stack. It demonstrates how to create simple chat lobby using Blazor for the UI and SignalR for instant, two-way communication.

## Overview

BlazorChat lets multiple users chat in real time through their browsers. Messages appear instantly for everyone without reloading the page, using WebSockets via SignalR.
The project is designed as a full-stack C# solution to highlight clean architecture, responsive UI, and modern .NET web development practices.

## Tech Stack

Blazor WebAssembly – Client-side UI in C#, running in the browser via WebAssembly.  
ASP.NET Core – Backend services, hosting the client, handling APIs, and SignalR hubs.  
SignalR – Real-time messaging over WebSockets.  
Entity Framework Core – Database access and message persistence.  
xUnit  
Docker  

## Architecture

BlazorChat uses a client–server model with shared C# models:
Client (Blazor WebAssembly) – Renders the chat UI and connects to the server via SignalR.  
Server (ASP.NET Core) – Hosts the client app, manages real-time messaging, authentication, and data storage.  
Shared Project – Common models and types used by both client and server for consistency.  

### Messages flow like this: ###
User sends a message in the client.  
The message is sent to the SignalR hub on the server.  
The server stores it (via EF Core) and broadcasts it to all connected clients.  
All clients update instantly.  

## Key Features

Instant Messaging – Real-time message updates using SignalR.  
Persistent History – Messages stored in a database with EF Core.  
Responsive UI – Modern, mobile-friendly design with MudBlazor.  
Clean Code – Shared models, dependency injection, async programming, and scalable architecture.  
Testing with xUnit  
Easy deployment with docker compose  

## Images

<img width="1917" height="935" alt="image" src="https://github.com/user-attachments/assets/574d373c-4d61-4384-b845-03d5484df64a" />
<img width="1349" height="556" alt="image" src="https://github.com/user-attachments/assets/85b94c88-df36-46b2-b8a1-ec78ee3cd19a" />
<img width="1882" height="900" alt="image" src="https://github.com/user-attachments/assets/77a02404-dbb7-413f-ab6a-e5b718cf85a0" />
