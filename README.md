# WexoCodeChallenge

This repository contains an ASP.NET MVC application developed in C#. It is structured to facilitate efficient development and organization.

## Prerequisites

- **Visual Studio**: Ensure you have Visual Studio installed with the necessary workloads for ASP.NET and web development.

## Getting Started

To run the application in Visual Studio:

1. **Clone the Repository**:
   - Open Visual Studio.
   - Navigate to `File > Open > Project/Solution`.
   - Clone the repository using the URL: `https://github.com/ThomasUCN1996/WexoCodeChallenge`.

2. **Restore NuGet Packages**:
   - Visual Studio will prompt to restore NuGet packages upon opening the solution.
   - Alternatively, right-click the solution in the Solution Explorer and select "Restore NuGet Packages".

3. **Build the Solution**:
   - Press `Ctrl + Shift + B` or navigate to `Build > Build Solution`.

4. **Run the Application**:
   - Press `F5` or click the "Start" button to build and run the application.

## Project Structure

The solution comprises the following projects:

- **WexoCodeChallenge.Website**: Contains the main ASP.NET MVC application, including Controllers, Views, and Models.
- **WexoCodeChallenge.Tests**: Holds unit tests for the application.

The root directory also includes:

- **WexoCodeChallenge.sln**: The solution file that organizes the projects.
- **.gitignore**: Specifies files and directories to be ignored by Git.
- **.gitattributes**: Defines attributes for path names.
- **README.md**: This file, providing project information.

## ASP.NET MVC Folder Structure

An ASP.NET MVC application typically follows a standard folder structure to separate concerns:

- **Controllers**: Contains classes that handle user requests and interact with models to return views.
- **Models**: Represents the data and business logic of the application.
- **Views**: Holds the UI components (Razor views) that render data to the user.
- **App_Data**: Stores application data like databases or XML files. Note that IIS does not serve files from this folder. citeturn0search4
- **Content**: Contains static files such as CSS, images, and other media.
- **Scripts**: Holds JavaScript files used in the application.

This structure promotes organized code management and separation of concerns. citeturn0search2

## Additional Information

For more details on ASP.NET MVC project structures and best practices, refer to the official Microsoft documentation. citeturn0search2

Ensure your development environment is set up correctly to avoid potential issues. Regularly update NuGet packages and Visual Studio to maintain compatibility and access to the latest features. 
