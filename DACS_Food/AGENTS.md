# DACS_Food Project Guide

## Project type
ASP.NET Core MVC project.

## Structure
- Controllers/HomeController.cs controls page navigation.
- Models/FoodItem.cs is prepared for future database work.
- Views/Shared/_Layout.cshtml contains header, footer, cart panel, food detail modal, golden hour banner, and floating chatbot.
- Views/Home contains the actual pages.
- wwwroot/assets/css/style.css contains frontend styling.
- wwwroot/assets/js/main.js contains frontend behavior and temporary menu data.

## Important rules for Codex
- Keep Vietnamese text.
- Do not remove floating chatbot.
- Do not remove golden hour banner.
- Do not remove food detail popup.
- Preserve responsive design.
- Prefer small, targeted changes.

## Run
Use Visual Studio Run button, or: dotnet run
