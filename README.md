# CourseLMS - ASP.NET Core Project

CourseLMS is a web application developed using ASP.NET Core that provides a Learning Management System (LMS) for managing courses, assignments, and user enrollments.

## Technologies Used

- **ASP.NET Core MVC:** The project is built using ASP.NET Core MVC, which provides a robust framework for building web applications.

- **Entity Framework:** Entity Framework is used for data access and provides an object-relational mapping (ORM) for interacting with the database.

- **Identity Framework:** Identity Framework is used for user authentication and management.

- **Microsoft SQL Server:** SQL Server is the relational database used for storing application data.

- **Bootstrap:** Bootstrap is used for creating a responsive and user-friendly user interface.

- **EPPlus:** EPPlus is used for exporting data to Excel format.

## Models

- **AspNetUsers:** Stores information about application users, including admins, students, and instructors.

- **AspNetUserRoles:** Junction table for the many-to-many relationship between AspNetUsers and AspNetRoles.

- **AspNetRoleClaims:** Used to store additional claims associated with roles in the application.

- **AspNetRoles:** Used to manage roles such as admin, student, and instructor in the application.

- **Courses:** Used to store information about courses.

- **Assignments:** Used to store information about assignments.

- **Enrollments:** Used to store information about course enrollments.

- **AspNetUserLogins:** Contains information about external login providers. The UserId links this external login information to the corresponding user in the application.

- **AspNetUserClaims:** Used to store claims associated with individual users.

- **AspNetUserTokens:** Used to store security tokens associated with individual users.

## Controllers

- **AssignmentController:** This controller manages essential assignment tasks, including creating, editing, and deleting assignments. It also enables the convenient export of assignments in Excel format.

- **CoursesController:** This controller manages essential courses tasks, including creating, editing, and deleting courses. It also enables the convenient export of courses in Excel format.

- **EnrollmentsController:** Responsible for managing course enrollments, using security measures to ensure only authorized users can use its features. Additionally, it can export enrollment records in an Excel format.

- **HomeController:** Responsible for handling requests related to the home page. It displays home page content, handles authentication, and provides course enrollment information.

- **UserController:** Handles tasks related to users, especially those related to administrators. This includes creating, editing, and deleting user accounts, as well as viewing user details. It also allows administrators to export user information in an Excel format.

## ER Diagram
![ER Diagram](https://user-images.githubusercontent.com/59375288/274730056-5b96f64a-e390-4de1-be67-d2c9557b7beb.png)


## Getting Started

To run the CourseLMS project on your local machine, follow these steps:

1. Clone the repository to your local environment.

2. Ensure you have ASP.NET Core SDK, Entity Framework, and Microsoft SQL Server installed.

3. Set up the database connection in the appsettings.json file.

4. Run the Entity Framework migrations to create the database schema.

5. Build and run the project.


## Contribute

If you'd like to contribute to the CourseLMS project, please open an issue or submit a pull request on the GitHub repository. Your contributions are welcome and appreciated! Special thanks to the following contributors:

- [kaanfsd](https://github.com/kaanfsd) - Kaan Özkan
- [Ilyas-Emre](https://github.com/Ilyas-Emre) - İlyas Emre Keskin
- [erenyardimci](https://github.com/erenyardimci) - Hasan Eren Yardımcı
- [drdemon12](https://github.com/drdemon12) - Eren Arda KAPLAN


## License

This project is licensed under the [MIT License](LICENSE). You are free to use, modify, and distribute this project as per the terms of the license.

---

Thank you for your interest in CourseLMS. If you have any questions or need further assistance, feel free to contact us. Happy learning!
