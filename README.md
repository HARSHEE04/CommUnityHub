# 🌍 CommUnity Hub

A full-stack **ASP.NET Core MVC** web application that helps students across the **GTA** discover trusted, free community resources — all in one place.

---

## 🔎 What Is CommUnity Hub?

**CommUnity Hub** centralizes verified community resources such as **food banks**, **tutoring programs**, **donation drives**, and **housing support**, and displays them through a **searchable interface** and **interactive map**.

The platform also allows students and volunteers to post “Offer Help” listings, which are reviewed and approved by admins to maintain **trust** and **data quality**.

---

## 🧩 Team Collaboration

This project was developed collaboratively by a **group of three** as part of a full-stack web development initiative. We worked together throughout the design, development, and testing phases to ensure the platform was both functional and user-friendly.

---

## 🛠️ Tech Stack

- **ASP.NET Core MVC (C#)**
- **Entity Framework Core + SQL Server**
- **ASP.NET Core Identity** (authentication & role-based authorization)
- **Google Maps JavaScript API**
- **Bootstrap** for responsive UI styling
- **CSV data import** for public resource datasets

---

## ✨ Core Features

- Search and filter community resources  
- View resources on an interactive Google Map  
- User registration and login  
- Volunteer “Offer Help” postings  
- Admin moderation dashboard  
- Role-based access control (Admin vs User)

---

## 🚀 How to Run the Application

1. Clone the repository  
git clone https://github.com/HARSHEE04/CommUnityHub.git


2. Open the solution in **Visual Studio**  
3. Restore **NuGet packages**  
4. Update the **SQL Server connection string** in `appsettings.json`  
5. Run database migrations or initialization (if not already applied)  
6. Start the application using **IIS Express** or **Kestrel**  
7. Open your browser and navigate to:  
👉 [https://localhost:7043/](https://localhost:7043/)

The application includes **seeded data**, such as:
- A predefined **admin account**
- Sample community resources (imported from CSV files)

---

## 🧭 How to Use the App

1. Browse and search community resources from the **Home** or **Resources** page  
2. View resource locations directly on the **interactive map**  
3. Register or log in to create volunteer **“Offer Help”** postings  
4. **Admin users** can review and approve submissions through the **Admin Dashboard**

---

## 🎯 Purpose

CommUnity Hub addresses the lack of a **centralized**, **reliable**, and **student-focused** platform for discovering community resources across the **GTA** by combining:

- Verified public datasets  
- Community-driven contributions  
- Secure admin moderation  
- Location-based visualization tools
