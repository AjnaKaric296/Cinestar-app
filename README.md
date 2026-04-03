# CineStar Mobile App

CineStar is a cross-platform mobile application developed using .NET MAUI. The application provides users with a simple and intuitive way to browse movies, view detailed information, and make reservations.

The goal of this project was to simulate a real-world cinema application by combining external data sources, local storage, and user interaction features. The application enables users to explore movies, manage reservations, and interact with a personalized interface.

## Features

- Movie browsing using external API (OMDb)
- Detailed movie view (title, description, actors, ratings, poster)
- City selection with dynamic content adaptation
- User registration and login system
- Reservation system (date, time, number of tickets)
- Seat selection with validation logic
- Loyalty points tracking
- View and manage reservations
- QR code generated for each reservation
- QR code scanning leads to a simple web page displaying reservation details (movie, date, time)
- Local data persistence using SQLite

## OMDb API Integration

The application integrates the OMDb API to fetch real-time movie data. This includes:

- Fetching a list of movies based on the selected city
- Retrieving detailed information for a selected movie (plot, cast, ratings, poster)
- Displaying movie posters and metadata dynamically in the UI


## Reservation Logic

The reservation flow includes:
- Selecting a movie
- Choosing date and time
- Defining number of tickets
- Selecting seats

The seat selection system ensures that users cannot select more seats than the number of tickets, providing real-time validation and feedback.

## Data Storage

- SQLite is used for storing users, reservations, and loyalty points
- Preferences are used for storing:
  - Logged-in user (session handling)
  - Selected city

## UI Design

The application follows a clean and minimal design with a consistent color theme. The focus was on usability and simple navigation between core features such as movie browsing, reservations, and user profile.


## Team

Ajna Karić  
Aida Begagić  
Lamija Mehić  

Software Engineering Students
