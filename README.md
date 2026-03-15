# 🕊️ Diwan - A Peaceful Haven for Writers & Poets

[![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET%20Core%20MVC-8.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![Entity Framework Core](https://img.shields.io/badge/EF%20Core-Code%20First-339933?style=flat-square)](https://docs.microsoft.com/en-us/ef/core/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-Database-CC2927?style=flat-square&logo=microsoftsqlserver)](https://www.microsoft.com/en-us/sql-server)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?style=flat-square&logo=bootstrap)](https://getbootstrap.com/)
[![jQuery](https://img.shields.io/badge/jQuery-Dynamic%20UI-0769AD?style=flat-square&logo=jquery)](https://jquery.com/)
[![Architecture](https://img.shields.io/badge/Architecture-3--Tier%20Layered-orange?style=flat-square)](#)

> **Diwan** is a cultural social platform designed as a quiet corner for written content, specifically targeting readers, writers, poets, and intellectuals in the Arab world. The vision is to create an elegant, classic, and calm user experience, reminiscent of a literary cafe or a warm library, encouraging thoughtful interaction rather than fast-paced content consumption.

🌐 **[Explore the Live Application Here](https://diwan.runasp.net/)**

---

## ✨ Key Features

### 👤 User Authentication & Profiles
* **Secure Access:** Complete registration, login, and password reset workflows powered by **ASP.NET Core Identity**.
* **Personalized Profiles:** Users can customize their presence with profile pictures, cover photos, and personal biographies.

### 🤝 Comprehensive Friendship System
* **Connect & Network:** Search for specific users across the platform.
* **Request Management:** Send, accept, decline, or cancel friend requests. Users can also remove or block existing connections.
* **Dynamic UI:** User profiles display dynamic action buttons reflecting the exact current friendship status (e.g., *Add Friend, Request Sent, Friends, Respond to Request*).

### 📝 Core Content & Interaction
* **Rich Posting:** Create, edit, and delete text-based posts with optional image uploads.
* **Dynamic Reactions:** Express specific emotions using a dynamic reaction system (Like, Love, Haha, Wow, Sad, Angry).
* **Threaded Discussions:** A fully functional nested comment system supporting "Threaded Replies" for deep, organized conversations.

### 🔔 Smart Notification System
* **Awareness:** Generates and alerts users for key social events: New friend requests, accepted requests, new comments, and replies.
* **Dynamic Menu:** Integrated directly into the main navigation bar for instant, seamless access.

### ⚡ Performance Optimization
* **Dynamic Batch Loading(Pagination):** To ensure a smooth and responsive UX, posts and notifications are loaded dynamically rather than fetching the entire database payload at once.

---

## 🛠️ Technical Architecture & Stack

Diwan is built utilizing modern .NET technologies and adheres to professional architectural patterns to ensure scalability and maintainability.

**Backend & Data Access:**
* **Framework:** C# with ASP.NET Core MVC (.NET 8)
* **Database:** Microsoft SQL Server
* **ORM:** Entity Framework Core 8 (Code-First approach with comprehensive Fluent API configurations)

**Architecture & Design Patterns:**
* **Structure:** Strict 3-Tier Layered Architecture (Presentation, Business Logic, Data Access).
* **Patterns Used:** Implemented the **Generic Repository** and **Unit of Work** design patterns to decouple the application logic from data access.

**Frontend:**
* **Views:** Razor Pages (HTML5, CSS3)
* **Styling & Responsiveness:** Bootstrap
* **Interactivity:** jQuery & AJAX for asynchronous, seamless data loading without page reloads.

---
