/*
    FoodieLab EF migrations idempotent script

    This project currently has no Migrations/ folder. Because there are no EF Core
    migrations to script, there is no generated migration SQL here.

    Current database setup path:
    1. Create the SQL Server database with FoodieLab_FullDatabase.sql.
    2. Run the app once. Program.cs calls DatabaseSeeder.InitializeAsync(), which
       uses EnsureCreatedAsync() and idempotent SQL helpers to create/update schema.
    3. The seeder also creates demo data and the admin account.

    If the project later switches to migrations, regenerate this file with:
    dotnet ef migrations script --idempotent --output Database/FoodieLab_EF_Migrations_Idempotent.sql
*/

IF DB_ID(N'DACS_Food') IS NULL
BEGIN
    CREATE DATABASE [DACS_Food];
END
GO

PRINT N'No EF migrations were found in this project. Use DatabaseSeeder on app startup.';
GO
