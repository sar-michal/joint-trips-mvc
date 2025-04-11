# Joint Trips

## Table of Contents

- [Description](#description)
- [Features](#features)
- [Dependencies](#dependencies)
- [Setup Instructions](#setup-instructions)
- [Concurrency Handling](#concurrency-handling)
- [Screenshots](#screenshots)

## Description

This project is a web application for organizing trips developed using ASP.NET MVC with Entity Framework Core. It allows users to register and log in, create and manage trip proposals, and sign up for upcoming trips. The application also supports advanced features such as granting ownership rights to a second user and handling concurrency conflicts (optimistic concurrency for editing trips).

## Features

- **User Management:**  
    - Registration and login using ASP.NET Identity.  
    - Secure authentication with password hashing.

- **Trip Management:**  
    - Create, view, edit, and delete trip proposals.  
    - Each trip includes details such as title, start/end dates, price, location, capacity, and a description.

- **Ownership and Access Control:**  
    - The creator of a trip is automatically assigned as an owner.  
    - Only owners can modify trip details or grant/take away ownership rights from other participants.

- **Trip Participation:**  
    - Logged-in users (non-owners) can sign up (join) or unsubscribe (leave) from trips that have not yet started and have available capacity.

- **Edit Concurrency Handling:**  
    - The application implements optimistic concurrency for editing trips using a concurrency token (timestamp).  
    - When multiple users attempt to edit the same trip simultaneously, the system detects conflicts and displays the current database values, allowing the user to review and resubmit their changes.

## Dependencies

- **.NET SDK:** .NET 8
- **ASP.NET MVC:** The project uses the ASP.NET MVC framework  
- **Entity Framework Core:** For data access and migrations  
- **ASP.NET Identity:** For user management and authentication  
- **Bootstrap:** For responsive CSS styling  
- **Visual Studio:** For development and project management  
- **SQL Server Express LocalDB:** The application uses a local database instance for data storage.

## Setup Instructions

Follow the steps below to set up and run the project on your local machine:

1. **Clone the Repository:**  
     ```sh
     git clone https://github.com/sar-michal/joint-trips-mvc.git
     ```
2. **Navigate to the Project Directory**  
     ```sh
     cd joint-trips-mvc/JointTrips
     ```
3. **Restore Dependencies**  
     ```sh
     dotnet restore
     ```
4. **Configure the Database**  
Ensure your `appsettings.json` contains a valid connection string and adjust it if needed. For example:
    ```json
    "ConnectionStrings": {
    "JointTripsContext": "Server=(localdb)\\mssqllocaldb;Database=JointTripsDB;Trusted_Connection=True;MultipleActiveResultSets=true"
    }
    ```
    This connection string will create or connect to an SQL Server Express LocalDB instance with the database named "JointTripsDB".
5. **Update Database**
    ```sh
    dotnet ef database update
    ```
6. **Run the Application:**  
    Start the project by running:
    ```sh
    dotnet run --launch-profile https
    ```
    The application is available at **https://localhost:7267**.  
    
    Or alternatively run:
    ```sh
    dotnet run
    ```
    The application is available at **http://localhost:5221**.

## Concurrency Handling

This project implements optimistic concurrency for editing trips. Each Trip entity includes a concurrency token (a timestamp/rowversion) that is checked during updates. When a user attempts to save changes to a trip, the application compares the token from the client with the current token in the database:

- If the tokens match, the update is applied.
- If the tokens differ (indicating that another user has modified the trip in the meantime), a DbUpdateConcurrencyException is thrown. The application catches this exception, retrieves the current values from the database, and redisplays the edit form along with error messages indicating the current database values. This ensures that no concurrent updates are lost and that the user can review and resubmit their changes if desired.

## Screenshots

### Login
![Login](https://github.com/user-attachments/assets/fb936c76-7952-4898-95e7-c2dac0fb1324)

### Trips
![Trips](https://github.com/user-attachments/assets/6d6942e0-78a9-49da-96f0-88afcd9d34ef)

### Details
![Details](https://github.com/user-attachments/assets/26fa7837-876d-41ef-bdc1-03f0d3f9a197)

### Join
![Join](https://github.com/user-attachments/assets/39452dd2-e485-47cf-9b19-fa89382e3ed2)

### Participants
![Participants](https://github.com/user-attachments/assets/bb6f2b07-322b-47fc-844b-143ee1aeaf3c)

### Concurrency Edit
![Edit_Concurrency](https://github.com/user-attachments/assets/6bfffd91-1ed4-4bef-b988-a89d6d9c4f69)
